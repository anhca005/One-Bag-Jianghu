using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Battle.Replay.View.VfxLab;

namespace OneBagJianghu.Tests.Battle.Replay.VfxLab
{
    public sealed class VfxCatalogTests
    {
        [Test]
        public void CatalogHasThirtyNineUniqueKeys()
        {
            var keys = new HashSet<string>();
            foreach (VfxCatalogEntry entry in VfxCatalog.Entries)
            {
                Assert.That(keys.Add(entry.Key), Is.True, "Duplicate VFX key: " + entry.Key);
            }

            Assert.That(keys.Count, Is.EqualTo(39));
        }

        [Test]
        public void EveryItemWeaponClassHasBasicAndUltimateEntries()
        {
            ItemCatalog items = ItemCatalog.LoadFromResources();
            var keys = CatalogKeys();
            foreach (ItemInfo item in items.All)
            {
                if (item.Type != "weapon")
                    continue;
                Assert.That(keys,
                    Does.Contain(item.WeaponClass + "_basic"),
                    item.WeaponClass);
                Assert.That(keys,
                    Does.Contain(item.WeaponClass + "_ultimate"),
                    item.WeaponClass);
            }
        }

        [Test]
        public void WeaponEntriesMatchResolver()
        {
            foreach (VfxCatalogEntry entry in VfxCatalog.Entries)
            {
                if (entry.Group == VfxCatalogGroup.Common)
                    continue;
                string weaponClass = entry.Key.Substring(0, entry.Key.LastIndexOf('_'));
                var item = new ItemInfo("test", "Test", "weapon", weaponClass, null, "common", 1, 1);
                var battleEvent = new BattleEvent
                {
                    Type = BattleEventType.Trigger,
                    Kind = entry.EventKind
                };
                ReplayVfx resolved = VfxResolver.Resolve(battleEvent, item);
                Assert.That(resolved.Kind, Is.EqualTo(ReplayVfxKind.Travel));
                Assert.That(entry.Key, Is.EqualTo(resolved.SpriteKey));
            }
        }

        [TestCase("BURN", "burst_burn")]
        [TestCase("POISON", "burst_poison")]
        [TestCase("BLEED", "burst_bleed")]
        public void BurstEntriesMatchResolver(string kind, string expectedKey)
        {
            var battleEvent = new BattleEvent
            {
                Type = BattleEventType.Burst,
                Kind = kind
            };
            ReplayVfx resolved = VfxResolver.Resolve(battleEvent, null);
            Assert.That(resolved.SpriteKey, Is.EqualTo(expectedKey));
            Assert.That(CatalogKeys(), Does.Contain(resolved.SpriteKey));
        }

        private static HashSet<string> CatalogKeys()
        {
            var keys = new HashSet<string>();
            foreach (VfxCatalogEntry entry in VfxCatalog.Entries)
            {
                keys.Add(entry.Key);
            }

            return keys;
        }
    }
}