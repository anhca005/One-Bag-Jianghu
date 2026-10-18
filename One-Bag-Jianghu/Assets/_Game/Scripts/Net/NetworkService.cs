using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Core;
using UnityEngine;

namespace OneBagJianghu.Net
{
    public enum NetworkState
    {
        Offline,
        Connecting,
        Online
    }

    public class OpponentInfo
    {
        public string Name
        {
            get;
            set;
        }
        public string Sect
        {
            get;
            set;
        }
    }

    public sealed class NetworkService : MonoBehaviour
    {
        public const string AuthTokenKey = "jianghu.auth.token";
        private readonly ConcurrentQueue<Action> dispatch = new ConcurrentQueue<Action>();
        private GameClient client;
        private Func<GameClient> clientFactory = () => new GameClient();
        private AuthSession authSession;
        private bool rememberAuthentication = true;
        public NetworkState State
        {
            get;
            private set;
        } = NetworkState.Offline;

        public event Action<NetworkState> StateChanged;
        public event Action<bool> AuthenticationChanged;
        public event Action<Envelope> ServerMessage;
        public bool IsAuthenticated => authSession != null;
        public AuthSession Authentication => authSession;
        public async Task ConnectAsync(TimeSpan? timeout = null)
        {
            if (State != NetworkState.Offline)
            {
                return;
            }

            SetState(NetworkState.Connecting);
            client?.Dispose();
            client = clientFactory();
            client.Disconnected += HandleClientDisconnected;
            client.ServerMessage += HandleServerMessage;
            TimeSpan connectTimeout = timeout ?? TimeSpan.FromSeconds(3);
            client.RequestTimeout = connectTimeout;
            using (var source = new CancellationTokenSource(connectTimeout))
            {
                try
                {
                    await client.ConnectAsync(ServerConfig.GetUrl(), source.Token);
                    Envelope hello = await client.SendAsync("hello",
                        new
                        {
                            clientVersion = "0.1.0",
                            heroName = GameBootstrap.Session?.HeroName ?? string.Empty
                        });
                    if (hello.Type != "hello_ok")
                    {
                        throw new InvalidOperationException(GetServerError(hello));
                    }

                    client.RequestTimeout = TimeSpan.FromSeconds(15);
                    SetState(NetworkState.Online);
                    string token = PlayerPrefs.GetString(AuthTokenKey, string.Empty);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        await ResumeInternal(token);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Server connection failed: " + exception.Message);
                    client.Disconnect();
                    SetState(NetworkState.Offline);
                }
            }
        }

        public async void Register(string username,
            string password,
            string heroName,
            bool remember,
            Action<AuthSession> onSuccess,
            Action<string> onError)
        {
            rememberAuthentication = remember;
            await Authenticate("register",
                new { username, password, heroName },
                onSuccess,
                onError);
        }

        public async void Login(string username,
            string password,
            bool remember,
            Action<AuthSession> onSuccess,
            Action<string> onError)
        {
            rememberAuthentication = remember;
            await Authenticate("login", new { username, password }, onSuccess, onError);
        }

        public void Login(string username,
            string password,
            Action<AuthSession> onSuccess,
            Action<string> onError)
        {
            Login(username, password, true, onSuccess, onError);
        }

        public async void Logout(Action onSuccess = null, Action<string> onError = null)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync("logout", new { });
                OnlineProtocol.ReadReply<JObject>(reply, "logout_ok");
                dispatch.Enqueue(() =>
                {
                    ClearAuthentication();
                    onSuccess?.Invoke();
                });
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        public async void RequestArenaStatus(Action<ArenaStatus> onSuccess, Action<string> onError)
        {
            await Send("arena_status", new { }, "arena_status_ok", onSuccess, onError);
        }

        public async void RequestLeaderboard(Action<LeaderboardResult> onSuccess, Action<string> onError)
        {
            await Send("leaderboard", new { limit = 50 }, "leaderboard_ok", onSuccess, onError);
        }

        public async void RequestArenaHistory(Action<ArenaHistoryResult> onSuccess, Action<string> onError)
        {
            await Send("arena_history", new { limit = 20 }, "arena_history_ok", onSuccess, onError);
        }

        public async void RequestArenaReplay(long battleId,
            Action<ArenaReplayResult> onSuccess,
            Action<string> onError)
        {
            await Send("arena_replay", new { battleId }, "arena_replay_ok", onSuccess, onError);
        }

        public async void RequestArenaBattle(string loadoutJson,
            int power,
            Action<ArenaResult> onSuccess,
            Action<string> onError)
        {
            try
            {
                JToken loadout = JToken.Parse(loadoutJson);
                await Send("arena_battle", new { loadout, power }, "arena_result", onSuccess, onError);
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        public void SetClientFactory(Func<GameClient> factory)
        {
            clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async void RequestBattle(string mode,
            string loadoutJson,
            Action<BattleLog,
            OpponentInfo> onResult,
            Action<string> onError)
        {
            if (State != NetworkState.Online || client == null)
            {
                onError?.Invoke("Không kết nối được máy chủ.");
                return;
            }

            try
            {
                JToken loadout = JToken.Parse(loadoutJson);
                Envelope reply = await client.SendAsync("battle_request", new
                {
                    mode,
                    loadout
                });
                if (reply.Type == "error")
                {
                    string message = reply.Payload?["message"]?.Value<string>();
                    dispatch.Enqueue(()
                        => onError
                        ?.Invoke(string.IsNullOrWhiteSpace(message)
                        ? "Máy chủ từ chối trận đấu."
                        : message));
                    return;
                }

                if (reply.Type != "battle_result")
                {
                    throw new FormatException("Unexpected server reply: " + reply.Type);
                }

                JToken result = reply.Payload?["result"];
                if (result == null)
                {
                    throw new FormatException("Battle result payload is missing result.");
                }

                BattleLog log = BattleLogParser.Parse(result.ToString(Newtonsoft.Json.Formatting.None));
                OpponentInfo opponent = reply.Payload?["opponent"]?.ToObject<OpponentInfo>();
                dispatch.Enqueue(() => onResult?.Invoke(log, opponent ?? new OpponentInfo()));
            }
            catch (Exception exception)
            {
                client.Disconnect();
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        public void Disconnect()
        {
            client?.Disconnect();
            SetState(NetworkState.Offline);
        }

        private void Update()
        {
            DispatchPending();
        }

        public void DispatchPending()
        {
            while (dispatch.TryDequeue(out Action action))
            {
                action();
            }
        }

        private void OnDestroy()
        {
            client?.Dispose();
        }

        private void HandleClientDisconnected()
        {
            dispatch.Enqueue(() =>
            {
                SetState(NetworkState.Offline);
                SetAuthentication(null);
            });
        }

        private void HandleServerMessage(Envelope envelope)
        {
            dispatch.Enqueue(() => ServerMessage?.Invoke(envelope));
        }

        private void SetState(NetworkState value)
        {
            if (State == value)
            {
                return;
            }

            State = value;
            StateChanged?.Invoke(value);
        }

        private static string GetServerError(Envelope envelope)
        {
            string message = envelope.Payload?["message"]?.Value<string>();
            return string.IsNullOrWhiteSpace(message) ? "Server hello failed." : message;
        }

        private async Task Authenticate(string type,
            object payload,
            Action<AuthSession> onSuccess,
            Action<string> onError)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync(type, payload);
                AuthSession session = OnlineProtocol.ReadReply<AuthSession>(reply, "auth_ok");
                dispatch.Enqueue(() =>
                {
                    SaveAuthentication(session);
                    onSuccess?.Invoke(session);
                });
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        private async Task ResumeInternal(string token)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync("resume", new { token });
                AuthSession session = OnlineProtocol.ReadReply<AuthSession>(reply, "auth_ok");
                SaveAuthentication(session);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not resume login: " + exception.Message);
                ClearAuthentication();
            }
        }

        private async Task Send<T>(string type,
            object payload,
            string expectedType,
            Action<T> onSuccess,
            Action<string> onError)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync(type, payload);
                T value = OnlineProtocol.ReadReply<T>(reply, expectedType);
                dispatch.Enqueue(() => onSuccess?.Invoke(value));
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        private GameClient RequireClient()
        {
            if (State != NetworkState.Online || client == null)
            {
                throw new InvalidOperationException("Không kết nối được máy chủ.");
            }

            return client;
        }

        private void SaveAuthentication(AuthSession session)
        {
            if (session == null || string.IsNullOrWhiteSpace(session.Token))
            {
                ClearAuthentication();
                return;
            }

            if (rememberAuthentication)
            {
                PlayerPrefs.SetString(AuthTokenKey, session.Token);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.DeleteKey(AuthTokenKey);
                PlayerPrefs.Save();
            }
            SetAuthentication(session);
        }

        private void ClearAuthentication()
        {
            PlayerPrefs.DeleteKey(AuthTokenKey);
            PlayerPrefs.Save();
            SetAuthentication(null);
        }

        private void SetAuthentication(AuthSession value)
        {
            bool changed = IsAuthenticated != (value != null);
            authSession = value;
            if (changed)
            {
                AuthenticationChanged?.Invoke(IsAuthenticated);
            }
        }
    }
}
