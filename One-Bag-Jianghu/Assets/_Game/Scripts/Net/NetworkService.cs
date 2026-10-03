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

    public sealed class OpponentInfo
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
        private readonly ConcurrentQueue<Action> dispatch = new ConcurrentQueue<Action>();
        private GameClient client;
        public NetworkState State
        {
            get;
            private set;
        } = NetworkState.Offline;

        public event Action<NetworkState> StateChanged;
        public event Action<Envelope> ServerMessage;
        public async Task ConnectAsync(TimeSpan? timeout = null)
        {
            if (State != NetworkState.Offline)
            {
                return;
            }

            SetState(NetworkState.Connecting);
            client?.Dispose();
            client = new GameClient();
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
    heroName = GameBootstrap.Session.HeroName
});
                    if (hello.Type != "hello_ok")
                    {
                        throw new InvalidOperationException(GetServerError(hello));
                    }

                    client.RequestTimeout = TimeSpan.FromSeconds(15);
                    dispatch.Enqueue(() => SetState(NetworkState.Online));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Server connection failed: " + exception.Message);
                    client.Disconnect();
                    dispatch.Enqueue(() => SetState(NetworkState.Offline));
                }
            }
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
            dispatch.Enqueue(() => SetState(NetworkState.Offline));
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
    }
}