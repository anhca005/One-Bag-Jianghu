using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Net;
using UnityEngine;
using UnityEngine.TestTools;

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

        [Test]
        public void ArenaResult_AppliesReputationAndJade()
        {
            // Arrange
            var profile = new PlayerProfile();
            profile.EnsureStorage(null);
            ArenaResult result = OnlineProtocol.ReadReply<ArenaResult>(Envelope("arena_result", new JObject
            {
                ["ratingBefore"] = 1000,
                ["ratingAfter"] = 1016,
                ["reward"] = new JObject
                {
                    ["reputation"] = 30,
                    ["materials"] = new JObject { ["chi_ton_ngoc_tuy"] = 1 }
                },
                ["status"] = StatusJson()
            }), "arena_result");

            // Act
            OnlineProtocol.ApplyReward(profile, result.Reward);

            // Assert
            Assert.That(profile.Currencies["danh_vong"], Is.EqualTo(30));
            Assert.That(new StorageGrid(profile, null).MaterialCount("chi_ton_ngoc_tuy"), Is.EqualTo(1));
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
            Assert.That(status.WeeklyWinsTarget, Is.EqualTo(10));
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
                ["rank"] = 17,
                ["wins"] = 3,
                ["losses"] = 1,
                ["rewardedLeftToday"] = 6,
                ["weeklyWins"] = 3,
                ["weeklyWinsTarget"] = 10,
                ["weeklyJadeClaimed"] = false
            };
        }

        private sealed class ReplyingTransport : IGameTransport
        {
            private readonly ConcurrentQueue<string> replies = new ConcurrentQueue<string>();
            private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
            public bool RejectResume
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
                }

                string type = request.Type == "hello" ? "hello_ok" : "auth_ok";
                JToken payload = request.Type == "hello" ? new JObject() : AuthPayload();
                if (request.Type == "resume" && RejectResume)
                {
                    type = "error";
                    payload = new JObject { ["code"] = "token_expired", ["message"] = "expired" };
                }

                replies.Enqueue(ProtocolJson.Serialize(new Envelope
                {
                    Type = type,
                    ReplyTo = request.Id,
                    Payload = payload
                }));
                signal.Release();
                return Task.CompletedTask;
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

            private static JObject AuthPayload()
            {
                return new JObject
                {
                    ["accountId"] = 12,
                    ["username"] = "kiem_khach",
                    ["heroName"] = "Vân Du",
                    ["token"] = "saved-token",
                    ["expiresAt"] = "2026-11-17T00:00:00+07:00"
                };
            }
        }
    }
}
