using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class VfxResolverTests
    {
        [TestCase("BASIC", "sword_basic")]
        [TestCase("ULTIMATE", "sword_ultimate")]
        public void Resolve_WeaponTriggerReturnsTravel(string triggerKind, string expectedKey)
        {
            BattleEvent battleEvent = Event(BattleEventType.Trigger, triggerKind);
            ItemInfo item = Item("weapon", "sword");

            ReplayVfx result = VfxResolver.Resolve(battleEvent, item);

            Assert.That(result.Kind, Is.EqualTo(ReplayVfxKind.Travel));
            Assert.That(result.SpriteKey, Is.EqualTo(expectedKey));
        }

        [TestCase("armor")]
        [TestCase("pill")]
        [TestCase("manual")]
        public void Resolve_NonWeaponTriggerReturnsNone(string itemType)
        {
            ReplayVfx result = VfxResolver.Resolve(Event(BattleEventType.Trigger, "BASIC"), Item(itemType, null));

            Assert.That(result.Kind, Is.EqualTo(ReplayVfxKind.None));
            Assert.That(result.SpriteKey, Is.Null);
        }

        [Test]
        public void Resolve_UnknownWeaponClassUsesNamingContract()
        {
            ReplayVfx result = VfxResolver.Resolve(Event(BattleEventType.Trigger, "ULTIMATE"), Item("weapon", "Spear"));

            Assert.That(result.Kind, Is.EqualTo(ReplayVfxKind.Travel));
            Assert.That(result.SpriteKey, Is.EqualTo("spear_ultimate"));
        }

        [TestCase(BattleEventType.Burst, "BURN", ReplayVfxKind.Burst, "burst_burn")]
        [TestCase(BattleEventType.Parry, null, ReplayVfxKind.Parry, "parry")]
        [TestCase(BattleEventType.Damage, null, ReplayVfxKind.Hit, "hit")]
        public void Resolve_CenteredEventReturnsExpectedEffect(BattleEventType type, string eventKind, ReplayVfxKind expectedKind, string expectedKey)
        {
            ReplayVfx result = VfxResolver.Resolve(Event(type, eventKind), null);

            Assert.That(result.Kind, Is.EqualTo(expectedKind));
            Assert.That(result.SpriteKey, Is.EqualTo(expectedKey));
        }

        [Test]
        public void Resolve_UnsupportedEventReturnsNone()
        {
            ReplayVfx result = VfxResolver.Resolve(Event(BattleEventType.Heal, null), null);

            Assert.That(result.Kind, Is.EqualTo(ReplayVfxKind.None));
        }

        private static BattleEvent Event(BattleEventType type, string kind)
        {
            return new BattleEvent { Type = type, Kind = kind };
        }

        private static ItemInfo Item(string type, string weaponClass)
        {
            return new ItemInfo("test", "Test", type, weaponClass, null, "common", 1, 1);
        }
    }
}
