using System.Text.RegularExpressions;
using NUnit.Framework;
using OneBagJianghu.Engine;
using OneBagJianghu.UI.Common;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ItemDescriberTests
    {
        [Test]
        public void WeaponNumbersUseTierThenRarityTruncation()
        {
            string common = ItemDescriber.Describe("thiet_cot_quyen_sao", "common", 1);
            string supreme = ItemDescriber.Describe("thiet_cot_quyen_sao", "supreme", 9);
            StringAssert.Contains("Công: 6", common);
            StringAssert.Contains("Tốc Xuất Chiêu", common);
            StringAssert.Contains("Hội Tâm", common);
            StringAssert.Contains("Công: 12", supreme); // 6*1400/1000=8, then 8*1600/1000=12
        }

        [Test]
        public void TriggerUsesVietnameseSheetWording()
        {
            string value = ItemDescriber.Describe("thai_thanh_tam_kinh", "common", 1);
            StringAssert.Contains("Khi ", value);
            Assert.IsTrue(value.Contains("Cương Khí") ||
value.Contains("nội lực") ||
value.Contains("khí huyết"));
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

        [Test]
        public void EveryCatalogItemHasNoRawIdentifiersOrUnknownOps()
        {
            ItemCatalog catalog = ItemCatalog.Load(Resources.Load<TextAsset>("Data/items").text);
            int count = 0;
            foreach (ItemDef item in catalog.All())
            {
                string value =
ItemDescriber.Describe(item.id,
item.rarity.Value.ToString().ToLowerInvariant(),
9);
                Assert.IsFalse(Regex.IsMatch(value,
@"\b[A-Z]+(?:_[A-Z]+)+\b"),
item.id +
" leaked a raw identifier: " +
value);
                foreach (string op in new[]
                {
                    "BUFF_STACK",
                    "CLEANSE",
                    "LIFESTEAL",
                    "RAISE_MAX_HP"
                }

                )
                {
                    StringAssert.DoesNotContain(op, value, item.id);
                }

                StringAssert.DoesNotContain("Hiệu ứng chưa hỗ trợ", value, item.id);
                StringAssert.DoesNotContain("Sự kiện chưa hỗ trợ", value, item.id);
                count++;
            }

            Assert.AreEqual(73, count);
        }

        [Test]
        public void BuffStackAndActiveUseSheetWording()
        {
            string buff = ItemDescriber.Describe("thiet_cot_quyen_sao", "common", 1);
            StringAssert.Contains("Cộng dồn Công +1 lên món này (tối đa 8 tầng)", buff);
            ItemCatalog catalog = ItemCatalog.Load(Resources.Load<TextAsset>("Data/items").text);
            ItemDef active = null;
            foreach (ItemDef item in catalog.All())
            {
                if (item.active.Count > 0)
                {
                    active = item;
                    break;
                }
            }

            Assert.NotNull(active);
            string value = ItemDescriber.Describe(active.id, "common", 1);
            StringAssert.Contains("Chiêu chủ động (mỗi " +
active.cooldown.ToString("0.##",
System.Globalization.CultureInfo.InvariantCulture) +
" giây)",
value);
        }

        [Test]
        public void ArmorStatLineUsesTierThenRarityTruncation()
        {
            Assert.AreEqual("Khí Huyết +186", ItemDescriber.StatLine("o_tam_y", "rare", 3));
        }
    }
}