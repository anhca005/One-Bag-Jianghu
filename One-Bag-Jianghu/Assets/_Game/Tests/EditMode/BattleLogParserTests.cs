using System;
using System.IO;
using NUnit.Framework;
using OneBagJianghu.Battle.Replay;
using UnityEngine;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class BattleLogParserTests
    {
        [Test]
        public void Parse_ReadsSampleCountsAndWinner()
        {
            BattleLog log = LoadSample();

            Assert.That(log.Items, Has.Count.EqualTo(16));
            Assert.That(log.Events, Has.Count.EqualTo(491));
            Assert.That(log.Winner, Is.EqualTo(1));
            Assert.That(log.DurationMs, Is.EqualTo(30000));
        }

        [Test]
        public void Parse_PreservesMissingAndPresentOptionalFields()
        {
            BattleLog log = LoadSample();
            BattleEvent start = log.Events[0];
            BattleEvent firstDamage = FindFirst(log, BattleEventType.Damage);

            Assert.That(start.Item, Is.Null);
            Assert.That(start.Value, Is.Null);
            Assert.That(start.Hp, Is.EqualTo(1200));
            Assert.That(firstDamage.Item, Is.EqualTo(2));
            Assert.That(firstDamage.Value, Is.EqualTo(14));
            Assert.That(firstDamage.Absorbed, Is.EqualTo(14));
            Assert.That(firstDamage.Crit, Is.Null);
        }

        [Test]
        public void Parse_MapsUnknownEventTypeWithoutFailing()
        {
            const string json = "{\"seed\":1,\"winner\":-1,\"durationMs\":0," +
                                "\"names\":[\"A\",\"B\"],\"finalHp\":[1,1],\"items\":[]," +
                                "\"events\":[{\"t\":0,\"type\":\"NEW_SERVER_EVENT\",\"side\":0}]}";

            BattleLog log = BattleLogParser.Parse(json);

            Assert.That(log.Events[0].Type, Is.EqualTo(BattleEventType.Unknown));
            Assert.That(log.Events[0].RawType, Is.EqualTo("NEW_SERVER_EVENT"));
        }

        [Test]
        public void Parse_ThrowsClearExceptionForMalformedJson()
        {
            FormatException exception = Assert.Throws<FormatException>(() => BattleLogParser.Parse("{bad json"));
            StringAssert.Contains("Malformed battle log JSON", exception.Message);
        }

        [Test]
        public void Parse_ReadsReplayBagAndPlacementFields()
        {
            BattleLog log = Load("replay_sample.json");

            Assert.That(log.Events, Has.Count.EqualTo(331));
            Assert.That(log.Bags[0], Is.EqualTo(new[] { 5, 4 }));
            Assert.That(log.Bags[1], Is.EqualTo(new[] { 5, 4 }));
            Assert.That(log.Items[0].X, Is.EqualTo(0));
            Assert.That(log.Items[0].Y, Is.EqualTo(0));
            Assert.That(log.Items[0].W, Is.EqualTo(1));
            Assert.That(log.Items[0].H, Is.EqualTo(2));
            Assert.That(log.Items[0].Tier, Is.EqualTo(1));
        }

        [Test]
        public void Parse_OldSampleDefaultsBagsAndLeavesPlacementAbsent()
        {
            BattleLog log = Load("sample_battle.json");

            Assert.That(log.Bags[0], Is.EqualTo(new[] { 5, 4 }));
            Assert.That(log.Bags[1], Is.EqualTo(new[] { 5, 4 }));
            Assert.That(log.Items[0].X, Is.Null);
            Assert.That(log.Items[0].Tier, Is.Null);
        }

        private static BattleEvent FindFirst(BattleLog log, BattleEventType type)
        {
            for (int i = 0; i < log.Events.Count; i++)
            {
                if (log.Events[i].Type == type) return log.Events[i];
            }
            Assert.Fail("No event of type " + type + " was found.");
            return null;
        }

        private static BattleLog LoadSample()
        {
            return Load("sample_battle.json");
        }

        private static BattleLog Load(string fileName)
        {
            string path = Path.Combine(Application.dataPath, "_Game/Tests/EditMode/Data/" + fileName);
            return BattleLogParser.Parse(File.ReadAllText(path));
        }
    }
}
