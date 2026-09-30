using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Net;
using UnityEngine;

namespace OneBagJianghu.Tests.Net
{
    public sealed class ProtocolNetTests
    {
        [Test]
        public void Envelope_RoundTripPreservesPayload()
        {
            Envelope source = ProtocolJson.Build("battle_request", 7, new
            {
                mode = "pve",
                loadout = new
                {
                    name = "Vân Du"
                }
            });

            Envelope parsed = ProtocolJson.Parse(ProtocolJson.Serialize(source));

            Assert.That(parsed.Type, Is.EqualTo("battle_request"));
            Assert.That(parsed.Id, Is.EqualTo(7));
            Assert.That(parsed.Payload["mode"]?.Value<string>(), Is.EqualTo("pve"));
            Assert.That(parsed.Payload["loadout"]?["name"]?.Value<string>(), Is.EqualTo("Vân Du"));
        }

        // Async tests run on the thread pool with a hard timeout: awaiting on Unity's main-thread
        // SynchronizationContext while the test blocks it deadlocks the whole test run.
        private static void RunOffMainThread(Func<Task> body)
        {
            Task task = Task.Run(body);
            Assert.That(task.Wait(TimeSpan.FromSeconds(5)), Is.True, "Async test timed out");
        }

        [Test]
        public void GameClient_MatchesRepliesById()
        {
            RunOffMainThread(MatchesRepliesById);
        }

        private static async Task MatchesRepliesById()
        {
            var transport = new FakeTransport();
            using (var client = new GameClient(transport))
            {
                await client.ConnectAsync("ws://test/ws");
                Task<Envelope> first = client.SendAsync("first", new JObject());
                Task<Envelope> second = client.SendAsync("second", new JObject());
                await transport.WaitForSentCount(2);
                Envelope firstRequest = ProtocolJson.Parse(transport.Sent[0]);
                Envelope secondRequest = ProtocolJson.Parse(transport.Sent[1]);
                transport.Reply("second_ok", secondRequest.Id);
                transport.Reply("first_ok", firstRequest.Id);

                Assert.That((await first).Type, Is.EqualTo("first_ok"));
                Assert.That((await second).Type, Is.EqualTo("second_ok"));
            }
        }

        [Test]
        public void GameClient_TimesOutUnansweredRequest()
        {
            RunOffMainThread(TimesOutUnansweredRequest);
        }

        private static async Task TimesOutUnansweredRequest()
        {
            var transport = new FakeTransport();
            using (var client = new GameClient(transport))
            {
                client.RequestTimeout = TimeSpan.FromMilliseconds(30);
                await client.ConnectAsync("ws://test/ws");
                bool timedOut = false;
                try
                {
                    await client.SendAsync("ping", new JObject());
                }
                catch (TimeoutException)
                {
                    timedOut = true;
                }

                Assert.That(timedOut, Is.True);
            }
        }

        [Test]
        public void BattleResultPayload_ParsesBundledReplayWithoutMutation()
        {
            TextAsset sample = Resources.Load<TextAsset>("Replays/replay_sample");
            Assert.That(sample, Is.Not.Null);
            JToken original = JToken.Parse(sample.text);
            var wire = new JObject
            {
                ["type"] = "battle_result",
                ["replyTo"] = 9,
                ["payload"] = new JObject
                {
                    ["opponent"] = new JObject
                    {
                        ["name"] = "Kiếm Khách",
                        ["sect"] = "kim_cang"
                    },
                    ["result"] = original.DeepClone()
                }
            };

            Envelope envelope = ProtocolJson.Parse(wire.ToString());
            JToken result = envelope.Payload["result"];
            BattleLog log = BattleLogParser.Parse(result.ToString(Newtonsoft.Json.Formatting.None));

            Assert.That(JToken.DeepEquals(result, original), Is.True);
            Assert.That(log.Events.Count, Is.GreaterThan(0));
            Assert.That(log.Names.Count, Is.EqualTo(2));
        }

        private sealed class FakeTransport : IGameTransport
        {
            private readonly ConcurrentQueue<string> received = new ConcurrentQueue<string>();
            private readonly SemaphoreSlim receiveSignal = new SemaphoreSlim(0);
            private readonly SemaphoreSlim sentSignal = new SemaphoreSlim(0);

            public bool IsConnected { get; private set; }
            public List<string> Sent { get; } = new List<string>();

            public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            {
                IsConnected = true;
                return Task.CompletedTask;
            }

            public Task SendTextAsync(string text, CancellationToken cancellationToken)
            {
                lock (Sent)
                {
                    Sent.Add(text);
                }

                sentSignal.Release();
                return Task.CompletedTask;
            }

            public async Task<string> ReceiveTextAsync(CancellationToken cancellationToken)
            {
                await receiveSignal.WaitAsync(cancellationToken);
                received.TryDequeue(out string value);
                return value;
            }

            public Task CloseAsync(CancellationToken cancellationToken)
            {
                IsConnected = false;
                return Task.CompletedTask;
            }

            public void Reply(string type, long replyTo)
            {
                var envelope = new Envelope
                {
                    Type = type,
                    ReplyTo = replyTo,
                    Payload = new JObject()
                };
                received.Enqueue(ProtocolJson.Serialize(envelope));
                receiveSignal.Release();
            }

            public async Task WaitForSentCount(int count)
            {
                while (true)
                {
                    lock (Sent)
                    {
                        if (Sent.Count >= count)
                        {
                            return;
                        }
                    }

                    await sentSignal.WaitAsync();
                }
            }

            public void Dispose()
            {
                receiveSignal.Dispose();
                sentSignal.Dispose();
            }
        }
    }
}
