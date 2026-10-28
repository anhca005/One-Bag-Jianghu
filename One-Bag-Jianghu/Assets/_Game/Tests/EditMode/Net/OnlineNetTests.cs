using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Arena;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Reflection;

namespace OneBagJianghu.Tests.Net
{
    public sealed class OnlineNetTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(NetworkService.AuthTokenKey);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(NetworkService.AuthTokenKey);
        }

        // The service continues on the Unity synchronization context, so these tests yield frames instead of
        // blocking the main thread.
        [UnityTest]
        public IEnumerator Login_SavesToken()
        {
            // Arrange
            var transport = new ReplyingTransport();
            NetworkService service = CreateService(transport);
            yield return Connect(service);
            bool completed = false;

            // Act
            service.Login("kiem_khach", "secret1", session => completed = true, Assert.Fail);
            yield return WaitFor(service, () => completed);

            // Assert
            Assert.That(completed, Is.True);
            Assert.That(PlayerPrefs.GetString(NetworkService.AuthTokenKey), Is.EqualTo("saved-token"));
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator Connect_WithTokenSendsResume()
        {
            // Arrange
            PlayerPrefs.SetString(NetworkService.AuthTokenKey, "old-token");
            var transport = new ReplyingTransport();
            NetworkService service = CreateService(transport);

            // Act
            yield return Connect(service);

            // Assert
            Assert.That(transport.Types, Does.Contain("resume"));
            Assert.That(service.IsAuthenticated, Is.True);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator Connect_WithRejectedTokenClearsToken()
        {
            // Arrange
            PlayerPrefs.SetString(NetworkService.AuthTokenKey, "bad-token");
            var transport = new ReplyingTransport { RejectResume = true };
            NetworkService service = CreateService(transport);

            // Act
            yield return Connect(service);

            // Assert
            Assert.That(PlayerPrefs.HasKey(NetworkService.AuthTokenKey), Is.False);
            Assert.That(service.IsAuthenticated, Is.False);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator AuthOk_AppliesServerProfile()
        {
            // Arrange
            var transport = new ReplyingTransport();
            NetworkService service = CreateService(transport);
            GameSession session = CreateSession(service);
            yield return Connect(service);
            bool completed = false;

            // Act
            service.Login("kiem_khach", "secret1", _ => completed = true, Assert.Fail);
            yield return WaitFor(service, () => completed);

            // Assert
            Assert.That(session.HeroName, Is.EqualTo("Hồ Sơ Server"));
            Assert.That(session.ProfileRevision, Is.EqualTo(7));
            UnityEngine.Object.DestroyImmediate(session.gameObject);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator AuthOk_SeasonRewardClosesAndSendsSeen()
        {
            // Arrange
            PopupManager popupManager = CreatePopupManager();
            var transport = new ReplyingTransport { IncludeSeasonReward = true };
            NetworkService service = CreateService(transport);
            yield return Connect(service);
            bool completed = false;
            service.Login("kiem_khach", "secret1", _ => completed = true, Assert.Fail);
            yield return WaitFor(service, () => completed);

            // Act
            Assert.That(service.Authentication.SeasonReward.Rank, Is.EqualTo(3));
            bool shown = service.TryShowSeasonReward();
            MessagePopup popup = PopupManager.Top as MessagePopup;
            popup.Hide();
            yield return WaitFor(service, () => transport.Types.Contains("season_reward_seen"));

            // Assert
            Assert.That(shown, Is.True);
            Envelope request = transport.Requests.Find(value => value.Type == "season_reward_seen");
            Assert.That(request.Payload["seasonId"].Value<int>(), Is.EqualTo(1));
            UnityEngine.Object.DestroyImmediate(service.gameObject);
            UnityEngine.Object.DestroyImmediate(popupManager.gameObject);
        }

        [UnityTest]
        public IEnumerator ProfileAction_UsesContractAndAppliesSnapshot()
        {
            // Arrange
            var transport = new ReplyingTransport();
            NetworkService service = CreateService(transport);
            GameSession session = CreateSession(service);
            yield return Connect(service);
            bool completed = false;

            // Act
            service.RefreshShop(true, _ => completed = true, Assert.Fail);
            yield return WaitFor(service, () => completed);

            // Assert
            Envelope request = transport.Requests.Find(value => value.Type == "shop_refresh");
            Assert.That(request.Payload["paid"].Value<bool>(), Is.True);
            Assert.That(session.ProfileRevision, Is.EqualTo(8));
            Assert.That(session.HeroName, Is.EqualTo("Hồ Sơ Server"));
            UnityEngine.Object.DestroyImmediate(session.gameObject);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator ChooseSect_AppliesServerProfile()
        {
            // Arrange
            var transport = new ReplyingTransport();
            NetworkService service = CreateService(transport);
            GameSession session = CreateSession(service);
            yield return Connect(service);
            bool completed = false;

            // Act
            service.ChooseSect("han_son", _ => completed = true, Assert.Fail);
            yield return WaitFor(service, () => completed);

            // Assert
            Envelope request = transport.Requests.Find(value => value.Type == "sect_choose");
            Assert.That(request.Payload["sectId"].Value<string>(), Is.EqualTo("han_son"));
            Assert.That(session.Profile.SectId, Is.EqualTo("han_son"));
            Assert.That(session.Profile.TamPhapLevel, Is.EqualTo(1));
            UnityEngine.Object.DestroyImmediate(session.gameObject);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator RejectedAction_DoesNotChangeProfile()
        {
            // Arrange
            var transport = new ReplyingTransport { RejectNextAction = true };
            NetworkService service = CreateService(transport);
            GameSession session = CreateSession(service);
            session.ApplyServerProfile(ProfileJson("Ban Đầu"), 3);
            yield return Connect(service);
            string error = null;

            // Act
            service.UpgradeItem("a", _ => Assert.Fail(), value => error = value);
            yield return WaitFor(service, () => error != null);

            // Assert
            Assert.That(session.HeroName, Is.EqualTo("Ban Đầu"));
            Assert.That(session.ProfileRevision, Is.EqualTo(3));
            Assert.That(error, Does.Contain("Không đủ Bạc"));
            UnityEngine.Object.DestroyImmediate(session.gameObject);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator ShopPurchaseButton_IsLockedUntilOk()
        {
            // Arrange
            var transport = new ReplyingTransport { HoldNextProfileAction = true };
            NetworkService service = CreateService(transport);
            GameSession session = CreateSession(service);
            yield return Connect(service);
            ShopPopup popup = UnityEngine.Object.Instantiate(Resources.Load<ShopPopup>("UI/Popups/ShopPopup"));
            ShopCard card = UnityEngine.Object.Instantiate(GetField<ShopCard>(popup, "cardPrefab"));
            GetField<List<GameObject>>(popup, "tradeCards").Add(card.gameObject);
            MethodInfo setPending = PrivateMethod(typeof(ShopPopup), "SetActionPending");
            setPending.Invoke(popup, new object[] { true });
            bool completed = false;

            // Act
            service.BuyShopSlot(0,
                _ =>
                {
                    setPending.Invoke(popup, new object[] { false });
                    completed = true;
                },
                Assert.Fail);
            yield return null;
            Assert.That(card.BuyButton.GetComponent<Button>().interactable, Is.False);
            transport.ReleaseHeldReply();
            yield return WaitFor(service, () => completed);

            // Assert
            Assert.That(card.BuyButton.GetComponent<Button>().interactable, Is.True);
            UnityEngine.Object.DestroyImmediate(card.gameObject);
            UnityEngine.Object.DestroyImmediate(popup.gameObject);
            UnityEngine.Object.DestroyImmediate(session.gameObject);
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator FindArenaMatch_HandlesImmediateResult()
        {
            var transport = new ReplyingTransport { ArenaFindReply = "arena_result" };
            NetworkService service = CreateService(transport);
            yield return Connect(service);
            ArenaResult result = null;

            service.FindArenaMatch("{\"name\":\"Vân Du\"}", 1234, _ => Assert.Fail(),
                value => result = value, Assert.Fail);
            yield return WaitFor(service, () => result != null);

            Assert.That(result.SearchId, Is.EqualTo("search-1"));
            Assert.That(transport.Types, Does.Contain("arena_find"));
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator FindArenaMatch_HandlesSearchingThenPushedResult()
        {
            var transport = new ReplyingTransport { ArenaFindReply = "arena_searching" };
            NetworkService service = CreateService(transport);
            yield return Connect(service);
            ArenaSearch search = null;
            ArenaResult result = null;

            service.FindArenaMatch("{\"name\":\"Vân Du\"}", 1234, value => search = value,
                value => result = value, Assert.Fail);
            yield return WaitFor(service, () => search != null);
            transport.PushArenaResult();
            yield return WaitFor(service, () => result != null);

            Assert.That(search.Range, Is.EqualTo(50));
            Assert.That(result.SearchId, Is.EqualTo(search.SearchId));
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [UnityTest]
        public IEnumerator CancelArenaSearch_SendsArenaCancel()
        {
            var transport = new ReplyingTransport { ArenaFindReply = "arena_searching" };
            NetworkService service = CreateService(transport);
            yield return Connect(service);
            ArenaSearch search = null;
            service.FindArenaMatch("{\"name\":\"Vân Du\"}", 1234, value => search = value,
                _ => Assert.Fail(), Assert.Fail);
            yield return WaitFor(service, () => search != null);

            service.CancelArenaSearch();
            yield return WaitFor(service, () => transport.Types.Contains("arena_cancel"));

            Assert.That(transport.Types[transport.Types.Count - 1], Is.EqualTo("arena_cancel"));
            UnityEngine.Object.DestroyImmediate(service.gameObject);
        }

        [Test]
        public void ArenaStatusAndLeaderboard_ParseProtocolFields()
        {
            // Arrange
            Envelope statusEnvelope = Envelope("arena_status_ok", StatusJson());
            Envelope leaderboardEnvelope = Envelope("leaderboard_ok", new JObject
            {
                ["seasonId"] = 1,
                ["entries"] = new JArray
                {
                    new JObject
                    {
                        ["rank"] = 1,
                        ["heroName"] = "Vân Du",
                        ["rating"] = 1240,
                        ["wins"] = 9,
                        ["losses"] = 2
                    }
                },
                ["self"] = new JObject { ["rank"] = 17, ["rating"] = 1032 }
            });

            // Act
            ArenaStatus status = OnlineProtocol.ReadReply<ArenaStatus>(statusEnvelope, "arena_status_ok");
            LeaderboardResult leaderboard = OnlineProtocol.ReadReply<LeaderboardResult>(
                leaderboardEnvelope,
                "leaderboard_ok");

            // Assert
            Assert.That(status.WeeklyWinsTarget, Is.EqualTo(15));
            Assert.That(status.PeakRating, Is.EqualTo(1420));
            Assert.That(leaderboard.Entries[0].HeroName, Is.EqualTo("Vân Du"));
            Assert.That(leaderboard.Self.Rank, Is.EqualTo(17));
        }

        [TestCase("auth_failed", "Sai tên đăng nhập hoặc mật khẩu")]
        [TestCase("rate_limited", "Thử sai nhiều lần, đợi một phút rồi thử lại")]
        [TestCase("token_expired", "Phiên đăng nhập đã hết hạn, mời đăng nhập lại")]
        public void ServerError_IsLocalized(string code, string expected)
        {
            // Arrange

            // Act
            string message = OnlineProtocol.LocalizeError(code);

            // Assert
            Assert.That(message, Is.EqualTo(expected));
        }

        private static NetworkService CreateService(ReplyingTransport transport)
        {
            var root = new GameObject("NetworkServiceTest");
            NetworkService service = root.AddComponent<NetworkService>();
            service.SetClientFactory(() => new GameClient(transport));
            return service;
        }

        private static PopupManager CreatePopupManager()
        {
            var root = new GameObject("PopupManagerTest");
            PopupManager manager = root.AddComponent<PopupManager>();
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.transform.SetParent(root.transform);
            Canvas backdropCanvas = new GameObject("BackdropCanvas").AddComponent<Canvas>();
            backdropCanvas.transform.SetParent(root.transform);
            GameObject backdrop = new GameObject("Backdrop");
            backdrop.transform.SetParent(backdropCanvas.transform);
            Button button = backdrop.AddComponent<Button>();
            SetField(manager, "canvas", canvas);
            SetField(manager, "backdropCanvas", backdropCanvas);
            SetField(manager, "backdrop", backdrop);
            SetField(manager, "backdropButton", button);
            manager.Initialize();
            return manager;
        }

        private static void SetField(object target, string name, object value)
        {
            target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        }

        private static GameSession CreateSession(NetworkService service)
        {
            var root = new GameObject("GameSessionTest");
            GameSession session = root.AddComponent<GameSession>();
            service.SetProfileSession(session);
            return session;
        }

        private static JObject ProfileJson(string heroName)
        {
            return JObject.Parse(new PlayerProfile { HeroName = heroName }.ToJson());
        }

        private static T GetField<T>(object target, string name)
        {
            return (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(target);
        }

        private static MethodInfo PrivateMethod(Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static IEnumerator Connect(NetworkService service)
        {
            Task task = service.ConnectAsync(TimeSpan.FromSeconds(1));
            yield return WaitFor(service, () => task.IsCompleted);
            Assert.That(service.State, Is.EqualTo(NetworkState.Online));
        }

        private static IEnumerator WaitFor(NetworkService service, Func<bool> condition)
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(3);
            while (!condition() && DateTime.UtcNow < deadline)
            {
                service.DispatchPending();
                yield return null;
            }

            service.DispatchPending();
            Assert.That(condition(), Is.True);
        }

        private static Envelope Envelope(string type, JToken payload)
        {
            return new Envelope { Type = type, Payload = payload };
        }

        private static JObject StatusJson()
        {
            return new JObject
            {
                ["seasonId"] = 1,
                ["seasonEndsAt"] = "2026-11-15T00:00:00+07:00",
                ["rating"] = 1000,
                ["peakRating"] = 1420,
                ["rank"] = 17,
                ["wins"] = 3,
                ["losses"] = 1,
                ["rewardedLeftToday"] = 6,
                ["weeklyWins"] = 3,
                ["weeklyWinsTarget"] = 15,
                ["weeklyJadeClaimed"] = false
            };
        }

        private sealed class ReplyingTransport : IGameTransport
        {
            private readonly ConcurrentQueue<string> replies = new ConcurrentQueue<string>();
            private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
            private string heldReply;
            public bool HoldNextProfileAction
            {
                get;
                set;
            }
            public bool RejectResume
            {
                get;
                set;
            }
            public bool RejectNextAction
            {
                get;
                set;
            }
            public bool IncludeSeasonReward
            {
                get;
                set;
            }

            public string ArenaFindReply
            {
                get;
                set;
            }

            public bool IsConnected
            {
                get;
                private set;
            }

            public List<string> Types
            {
                get;
            } = new List<string>();
            public List<Envelope> Requests
            {
                get;
            } = new List<Envelope>();

            public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            {
                IsConnected = true;
                return Task.CompletedTask;
            }

            public Task SendTextAsync(string text, CancellationToken cancellationToken)
            {
                Envelope request = ProtocolJson.Parse(text);
                lock (Types)
                {
                    Types.Add(request.Type);
                    Requests.Add(request);
                }

                string type = request.Type == "hello" ? "hello_ok" : "auth_ok";
                JToken payload = request.Type == "hello" ? new JObject() : AuthPayload();
                if (request.Type == "resume" && RejectResume)
                {
                    type = "error";
                    payload = new JObject { ["code"] = "token_expired", ["message"] = "expired" };
                }
                else if (request.Type == "arena_find")
                {
                    type = ArenaFindReply;
                    payload = type == "arena_searching" ? SearchPayload() : ResultPayload();
                }
                else if (request.Type == "arena_cancel")
                {
                    type = "arena_cancel_ok";
                    payload = new JObject { ["searchId"] = "search-1" };
                }
                else if (request.Type == "season_reward_seen")
                {
                    type = "season_reward_seen_ok";
                    payload = new JObject { ["seasonId"] = request.Payload["seasonId"] };
                }
                else if (IsProfileAction(request.Type))
                {
                    if (RejectNextAction)
                    {
                        RejectNextAction = false;
                        type = "error";
                        payload = new JObject
                        {
                            ["code"] = "rejected",
                            ["message"] = "Không đủ Bạc."
                        };
                    }
                    else
                    {
                        type = request.Type + "_ok";
                        payload = ProfilePayload(request);
                    }
                }

                string reply = ProtocolJson.Serialize(new Envelope
                {
                    Type = type,
                    ReplyTo = request.Id,
                    Payload = payload
                });
                if (HoldNextProfileAction && IsProfileAction(request.Type))
                {
                    HoldNextProfileAction = false;
                    heldReply = reply;
                    return Task.CompletedTask;
                }

                replies.Enqueue(reply);
                signal.Release();
                return Task.CompletedTask;
            }

            public void ReleaseHeldReply()
            {
                replies.Enqueue(heldReply);
                heldReply = null;
                signal.Release();
            }

            public void PushArenaResult()
            {
                replies.Enqueue(ProtocolJson.Serialize(new Envelope
                {
                    Type = "arena_result",
                    ReplyTo = 0,
                    Payload = ResultPayload()
                }));
                signal.Release();
            }

            public async Task<string> ReceiveTextAsync(CancellationToken cancellationToken)
            {
                await signal.WaitAsync(cancellationToken);
                replies.TryDequeue(out string reply);
                return reply;
            }

            public Task CloseAsync(CancellationToken cancellationToken)
            {
                IsConnected = false;
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                signal.Dispose();
            }

            private JObject AuthPayload()
            {
                var payload = new JObject
                {
                    ["accountId"] = 12,
                    ["username"] = "kiem_khach",
                    ["heroName"] = "Vân Du",
                    ["token"] = "saved-token",
                    ["expiresAt"] = "2026-11-17T00:00:00+07:00",
                    ["profile"] = ProfileJson("Hồ Sơ Server"),
                    ["rev"] = 7,
                    ["serverTime"] = "2026-10-21T00:00:00Z"
                };
                if (IncludeSeasonReward)
                {
                    payload["seasonReward"] = new JObject
                    {
                        ["seasonId"] = 1,
                        ["rank"] = 3,
                        ["rating"] = 1420,
                        ["reputation"] = 2000,
                        ["jade"] = 2
                    };
                }

                return payload;
            }

            private static JObject ProfilePayload(Envelope request)
            {
                var profile = new PlayerProfile { HeroName = "Hồ Sơ Server" };
                if (request.Type == "sect_choose")
                {
                    profile.SectId = request.Payload["sectId"].Value<string>();
                    profile.TamPhapLevel = 1;
                }

                return new JObject
                {
                    ["profile"] = JObject.Parse(profile.ToJson()),
                    ["rev"] = 8,
                    ["serverTime"] = "2026-10-21T00:00:01Z"
                };
            }

            private static bool IsProfileAction(string type)
            {
                return type == "profile_get"
                    || type == "loadout_save"
                    || type == "storage_move"
                    || type == "storage_repack"
                    || type == "item_upgrade"
                    || type == "item_merge"
                    || type == "item_sell"
                    || type == "shop_refresh"
                    || type == "shop_buy"
                    || type == "shop_exchange"
                    || type == "shop_buy_reputation"
                    || type == "sect_choose"
                    || type == "quest_claim"
                    || type == "onboarding"
                    || type == "pve_battle"
                    || type == "dev_cheat";
            }

            private static JObject SearchPayload()
            {
                return new JObject
                {
                    ["searchId"] = "search-1",
                    ["rating"] = 1032,
                    ["range"] = 50,
                    ["startedAt"] = "2026-10-21T00:00:00Z",
                    ["nextWidenAt"] = "2026-10-21T00:02:00Z"
                };
            }

            private static JObject ResultPayload()
            {
                return new JObject
                {
                    ["searchId"] = "search-1",
                    ["opponent"] = new JObject
                    {
                        ["name"] = "Đối Thủ",
                        ["rating"] = 1010,
                        ["power"] = 1200,
                        ["bot"] = false
                    },
                    ["result"] = new JObject(),
                    ["ratingBefore"] = 1000,
                    ["ratingAfter"] = 1016,
                    ["reward"] = new JObject
                    {
                        ["reputation"] = 30,
                        ["materials"] = new JObject()
                    },
                    ["status"] = StatusJson()
                };
            }
        }
    }
}
