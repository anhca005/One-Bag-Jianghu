using NUnit.Framework;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Tests.Battle.Replay
{
    public sealed class EventTextFormatterTests
    {
        [Test]
        public void Format_DamageIncludesTimeNameValueAndCritical()
        {
            var battleEvent = new BattleEvent { TimeMs = 3500, Type = BattleEventType.Damage, Item = 7, Value = 65, Crit = true };

            string text = EventTextFormatter.Format(battleEvent, _ => "Thanh Phong Kiếm");

            Assert.That(text, Is.EqualTo("3.5s  Thanh Phong Kiếm → 65 sát thương (Hội Tâm)"));
        }

        [TestCase("BURN", "Liệt Hỏa")]
        [TestCase("POISON", "Kịch Độc")]
        [TestCase("BLEED", "Xuất Huyết")]
        public void StatusName_UsesVietnameseLabels(string kind, string expected)
        {
            Assert.That(EventTextFormatter.StatusName(kind), Is.EqualTo(expected));
        }
    }
}
