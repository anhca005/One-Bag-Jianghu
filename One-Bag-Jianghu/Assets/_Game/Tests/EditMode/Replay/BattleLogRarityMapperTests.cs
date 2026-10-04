using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Tests.Replay
{
    public sealed class BattleLogRarityMapperTests
    {
        [Test]
        public void MixedLoadoutRaritiesMapByDeterministicUid()
        {
            var log = new BattleLog();
            log.Items.Add(new BattleItemRef { Uid = 0, Side = 0 });
            log.Items.Add(new BattleItemRef { Uid = 1, Side = 0 });
            log.Items.Add(new BattleItemRef { Uid = 100, Side = 1 });
            log.Items.Add(new BattleItemRef { Uid = 101, Side = 1, Rarity = "supreme" });
            BattleLogRarityMapper.Apply(log,
                new List<string> { "common",
                "legendary" },
                new List<string> { "rare",
                "epic" });
            Assert.That(log.Items[0].Rarity, Is.EqualTo("common"));
            Assert.That(log.Items[1].Rarity, Is.EqualTo("legendary"));
            Assert.That(log.Items[2].Rarity, Is.EqualTo("rare"));
            Assert.That(log.Items[3].Rarity,
                Is.EqualTo("supreme"),
                "Server rarity must win when present.");
        }
    }
}