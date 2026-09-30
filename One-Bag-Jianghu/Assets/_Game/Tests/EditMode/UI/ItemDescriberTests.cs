using NUnit.Framework;
using OneBagJianghu.UI.Common;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ItemDescriberTests
    {
        [Test]
        public void WeaponNumbersUseTierThenRarityTruncation()
        {
            string common = ItemDescriber.Describe("thiet_cot_quyen_sao", "common", 1);
            string supreme = ItemDescriber.Describe("thiet_cot_quyen_sao", "supreme", 9);
            StringAssert.Contains("Lực Đạo: 9", common);
            StringAssert.Contains("Tốc Xuất Chiêu", common);
            StringAssert.Contains("Hội Tâm", common);
            StringAssert.Contains("Lực Đạo: 24", supreme);
        }

        [Test]
        public void TriggerUsesVietnameseSheetWording()
        {
            string value = ItemDescriber.Describe("thai_thanh_tam_kinh", "common", 1);
            StringAssert.Contains("Khi ", value);
            Assert.IsTrue(value.Contains("Cương Khí") || value.Contains("nội lực") || value.Contains("khí huyết"));
        }

        [Test]
        public void BreakthroughShowsLockedAndUnlockedMilestones()
        {
            string tierOne = ItemDescriber.Describe("thiet_cot_quyen_sao", "common", 1);
            string tierNine = ItemDescriber.Describe("thiet_cot_quyen_sao", "supreme", 9);
            StringAssert.Contains("(mở ở Trọng 3)", tierOne);
            StringAssert.Contains("<color=#777777>", tierOne);
            StringAssert.DoesNotContain("(mở ở Trọng", tierNine);
            StringAssert.DoesNotContain("<color=#777777>", tierNine);
            StringAssert.Contains("Phổ Công", tierNine);
        }
    }
}
