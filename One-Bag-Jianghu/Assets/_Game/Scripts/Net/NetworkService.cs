using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Common;
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
        private GameSession profileSession;
        private AuthSession authSession;
        private bool rememberAuthentication = true;
        private bool recoveringConnection;
        private Action<ArenaSearch> arenaSearching;
        private Action<ArenaResult> arenaResult;
        private Action<string> arenaError;
        private SeasonReward pendingSeasonReward;
        private bool seasonRewardShowing;
        private int shownSeasonRewardId;
        public NetworkState State
        {
            get;
            private set;
        } = NetworkState.Offline;

        public event Action<NetworkState> StateChanged;
        public event Action<bool> AuthenticationChanged;
        public event Action<Envelope> ServerMessage;
        public event Action Disconnected;
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

        public async void Resume(string token, Action<AuthSession> onSuccess, Action<string> onError)
        {
            await Authenticate("resume", new { token }, onSuccess, onError);
        }

        /// <summary>Sends a server-authoritative profile mutation and applies its returned snapshot first.</summary>
        public async void SendProfileAction(string type,
            object payload,
            Action<JObject> onOk,
            Action<string> onError)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync(type, payload);
                JObject value = OnlineProtocol.ReadReply<JObject>(reply, type + "_ok");
                dispatch.Enqueue(() =>
                {
                    ApplyProfilePayload(value);
                    onOk?.Invoke(value);
                });
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => onError?.Invoke(exception.Message));
            }
        }

        public void GetProfile(Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("profile_get", new { }, onOk, onError);
        }

        public void SaveLoadout(JToken placements, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("loadout_save", new { placements }, onOk, onError);
        }

        public void MoveStorageItem(string uid, int toX, int toY, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("storage_move", new { uid, toX, toY }, onOk, onError);
        }

        public void MoveStorageMaterial(string materialId,
            int fromX,
            int fromY,
            int toX,
            int toY,
            Action<JObject> onOk,
            Action<string> onError)
        {
            SendProfileAction("storage_move",
                new { materialId, fromX, fromY, toX, toY },
                onOk,
                onError);
        }

        public void RepackStorage(Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("storage_repack", new { }, onOk, onError);
        }

        public void UpgradeItem(string uid, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("item_upgrade", new { uid }, onOk, onError);
        }

        public void MergeItems(string firstUid, string secondUid, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("item_merge", new { firstUid, secondUid }, onOk, onError);
        }

        public void SellItem(string uid, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("item_sell", new { uid }, onOk, onError);
        }

        public void RefreshShop(bool paid, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("shop_refresh", new { paid }, onOk, onError);
        }

        public void BuyShopSlot(int slot, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("shop_buy", new { slot }, onOk, onError);
        }

        public void ExchangeMaterial(string inputId,
            string targetId,
            Action<JObject> onOk,
            Action<string> onError)
        {
            SendProfileAction("shop_exchange", new { inputId, targetId }, onOk, onError);
        }

        public void BuyReputationOffer(string offerId, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("shop_buy_reputation", new { offerId }, onOk, onError);
        }

        public void ChooseSect(string sectId, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("sect_choose", new { sectId }, onOk, onError);
        }

        public void ClaimQuest(string questId, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("quest_claim", new { questId }, onOk, onError);
        }

        public void SendOnboarding(string action, string id, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("onboarding", new { action, id }, onOk, onError);
        }

        public void StartPveBattle(string stageId, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("pve_battle", new { stageId }, onOk, onError);
        }

        public void SendDevCheat(JObject payload, Action<JObject> onOk, Action<string> onError)
        {
            SendProfileAction("dev_cheat", payload, onOk, onError);
        }

        public async void RequestArenaStatus(Action<ArenaStatus> onSuccess, Action<string> onError)
        {
            await Send<ArenaStatus>("arena_status", new { }, "arena_status_ok", value =>
            {
                CaptureSeasonReward(value.SeasonReward);
                onSuccess?.Invoke(value);
            }, onError);
        }

        /// <summary>Shows an unseen season reward and acknowledges it only after the popup closes.</summary>
        public bool TryShowSeasonReward()
        {
            if (pendingSeasonReward == null || seasonRewardShowing)
            {
                return false;
            }

            SeasonReward reward = pendingSeasonReward;
            MessagePopup popup = PopupManager.Show<MessagePopup>(new MessagePopupData(
                "Thưởng cuối mùa",
                BuildSeasonRewardMessage(reward),
                "Đã nhận"));
            if (popup == null)
            {
                return false;
            }

            seasonRewardShowing = true;
            shownSeasonRewardId = reward.SeasonId;
            popup.Closed += HandleSeasonRewardClosed;
            return true;
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

        public async void FindArenaMatch(string loadoutJson,
            int power,
            Action<ArenaSearch> onSearching,
            Action<ArenaResult> onResult,
            Action<string> onError)
        {
            arenaSearching = onSearching;
            arenaResult = onResult;
            arenaError = onError;
            try
            {
                Envelope reply = await RequireClient().SendAsync("arena_find", new { });
                dispatch.Enqueue(() => HandleArenaMessage(reply));
            }
            catch (Exception exception)
            {
                dispatch.Enqueue(() => FinishArenaError(exception.Message));
            }
        }

        public async void CancelArenaSearch()
        {
            ClearArenaCallbacks();
            try
            {
                Envelope reply = await RequireClient().SendAsync("arena_cancel", new { });
                OnlineProtocol.ReadReply<JObject>(reply, "arena_cancel_ok");
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not cancel arena search: " + exception.Message);
            }
        }

        public void SetClientFactory(Func<GameClient> factory)
        {
            clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void SetProfileSession(GameSession session)
        {
            profileSession = session;
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
                bool shouldRecover = IsAuthenticated;
                FinishArenaError("Mất kết nối, đã huỷ tìm trận");
                SetState(NetworkState.Offline);
                SetAuthentication(null);
                Disconnected?.Invoke();
                if (shouldRecover)
                {
                    RecoverConnection();
                }
            });
        }

        private async void RecoverConnection()
        {
            if (recoveringConnection)
            {
                return;
            }

            recoveringConnection = true;
            MessagePopup blocker = PopupManager.Show<MessagePopup>(new MessagePopupData(
                "Mất kết nối",
                "Đang kết nối lại máy chủ…",
                "Vui lòng chờ"));
            await ConnectAsync(TimeSpan.FromSeconds(5));
            DispatchPending();
            recoveringConnection = false;
            if (IsAuthenticated)
            {
                blocker?.Close();
                return;
            }

            SceneFlow.Go(GameScene.Login);
        }

        private void HandleServerMessage(Envelope envelope)
        {
            dispatch.Enqueue(() =>
            {
                if (envelope.ReplyTo == 0 && (envelope.Type == "arena_searching"
                    || envelope.Type == "arena_result"))
                {
                    HandleArenaMessage(envelope);
                    return;
                }

                ServerMessage?.Invoke(envelope);
            });
        }

        private void HandleArenaMessage(Envelope envelope)
        {
            try
            {
                if (envelope.Type == "arena_searching")
                {
                    ArenaSearch search = OnlineProtocol.ReadReply<ArenaSearch>(envelope, "arena_searching");
                    arenaSearching?.Invoke(search);
                    return;
                }

                ArenaResult result = OnlineProtocol.ReadReply<ArenaResult>(envelope, "arena_result");
                ApplyProfile(result.Profile, result.Revision, result.ServerTime);
                Action<ArenaResult> callback = arenaResult;
                ClearArenaCallbacks();
                callback?.Invoke(result);
            }
            catch (Exception exception)
            {
                FinishArenaError(exception.Message);
            }
        }

        private void FinishArenaError(string message)
        {
            Action<string> callback = arenaError;
            ClearArenaCallbacks();
            callback?.Invoke(message);
        }

        private void ClearArenaCallbacks()
        {
            arenaSearching = null;
            arenaResult = null;
            arenaError = null;
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
                    ApplyProfile(session.Profile, session.Revision, session.ServerTime);
                    CaptureSeasonReward(session.SeasonReward);
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
                dispatch.Enqueue(() =>
                {
                    ApplyProfile(session.Profile, session.Revision, session.ServerTime);
                    CaptureSeasonReward(session.SeasonReward);
                    SaveAuthentication(session);
                });
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

        private void ApplyProfilePayload(JObject payload)
        {
            if (payload == null)
            {
                return;
            }

            ApplyProfile(payload["profile"],
                payload["rev"]?.Value<long>() ?? 0,
                ReadServerTime(payload["serverTime"]));
        }

        private void CaptureSeasonReward(SeasonReward reward)
        {
            if (reward != null
                && reward.SeasonId != shownSeasonRewardId
                && pendingSeasonReward == null
                && !seasonRewardShowing)
            {
                pendingSeasonReward = reward;
            }
        }

        private void HandleSeasonRewardClosed(BasePopup popup)
        {
            popup.Closed -= HandleSeasonRewardClosed;
            SeasonReward reward = pendingSeasonReward;
            pendingSeasonReward = null;
            seasonRewardShowing = false;
            if (reward != null)
            {
                SendSeasonRewardSeen(reward.SeasonId);
            }
        }

        private async void SendSeasonRewardSeen(int seasonId)
        {
            try
            {
                Envelope reply = await RequireClient().SendAsync("season_reward_seen", new { seasonId });
                OnlineProtocol.ReadReply<JObject>(reply, "season_reward_seen_ok");
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not acknowledge season reward: " + exception.Message);
            }
        }

        private static string BuildSeasonRewardMessage(SeasonReward reward)
        {
            return "Kỳ Luận Kiếm " + reward.SeasonId
                + " đã khép lại. Bạn đứng hạng " + reward.Rank
                + " với " + UiFormat.Number(reward.Rating)
                + " điểm, nhận " + UiFormat.Number(reward.Reputation)
                + " Danh Vọng và " + UiFormat.Number(reward.Jade)
                + " Chí Tôn Ngọc Tủy.";
        }

        // Json.NET turns ISO strings into DateTime while parsing, so the token may be a Date or a string.
        private static DateTimeOffset ReadServerTime(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return default;
            }

            if (token.Type == JTokenType.Date)
            {
                object value = ((JValue)token).Value;
                if (value is DateTimeOffset offset)
                {
                    return offset;
                }

                return new DateTimeOffset((DateTime)value);
            }

            return DateTimeOffset.Parse(token.Value<string>(), CultureInfo.InvariantCulture);
        }

        private void ApplyProfile(JToken profile, long revision, DateTimeOffset serverTime)
        {
            if (serverTime != default)
            {
                ServerClock.Synchronize(serverTime);
            }

            (profileSession ?? GameBootstrap.Session)?.ApplyServerProfile(profile, revision);
        }
    }
}
