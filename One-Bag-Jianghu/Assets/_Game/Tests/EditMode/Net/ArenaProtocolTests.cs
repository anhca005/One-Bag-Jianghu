using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Net;

namespace OneBagJianghu.Tests.Net
{
    public sealed class ArenaProtocolTests
    {
        [Test]
        public void ReadReply_ReadsArenaHistoryOk()
        {
            // Arrange
            Envelope envelope = ProtocolJson.Parse("{\"type\":\"arena_history_ok\",\"requestId\":\"1\","
                + "\"payload\":{\"entries\":[{\"battleId\":88,\"at\":\"2026-10-18T00:00:00+07:00\","
                + "\"won\":true,\"opponentName\":\"A\",\"opponentSect\":\"kim_cang\",\"bot\":false,"
                + "\"ratingBefore\":1000,\"ratingAfter\":1016}]}} ");

            // Act
            ArenaHistoryResult result = OnlineProtocol.ReadReply<ArenaHistoryResult>(envelope, "arena_history_ok");

            // Assert
            Assert.That(result.Entries[0].BattleId, Is.EqualTo(88));
        }

        [Test]
        public void ReadReply_ReadsArenaReplayOk()
        {
            // Arrange
            JObject payload = new JObject
            {
                ["opponent"] = JObject.FromObject(new { name = "A", sect = "kim_cang", rating = 1000, power = 10 }),
                ["result"] = new JObject()
            };
            Envelope envelope = ProtocolJson.Parse(new JObject
            {
                ["type"] = "arena_replay_ok",
                ["requestId"] = "1",
                ["payload"] = payload
            }.ToString());

            // Act
            ArenaReplayResult result = OnlineProtocol.ReadReply<ArenaReplayResult>(envelope, "arena_replay_ok");

            // Assert
            Assert.That(result.Opponent.Name, Is.EqualTo("A"));
        }
    }
}
