using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OneBagJianghu.Economy;
using OneBagJianghu.Engine;
using UnityEngine;
using EngineItemCatalog = OneBagJianghu.Engine.ItemCatalog;

namespace OneBagJianghu.UI.Common
{
    /// <summary>Vietnamese item copy shared by Bao Phuc and Tang Bao Cac.</summary>
    public static class ItemDescriber
    {
        private static EngineItemCatalog catalog;

        public static string Describe(string itemId, string rarity, int tier)
        {
            ItemDef raw = Catalog.Get(itemId);
            ItemDef item = raw.AtTier(tier);
            var text = new StringBuilder();
            text.Append("<b>").Append(raw.name).Append("</b>\n")
                .Append(TypeName(raw.type)).Append(WeaponClass(raw.weaponClass)).Append("\n")
                .Append(RarityName(rarity)).Append(" • ").Append(tier).Append(" Trọng");
            if (raw.type == ItemType.WEAPON)
            {
                int attack = item.attack * RarityRules.TierMultiplierPermille(tier) / 1000;
                attack = attack * RarityRules.MultiplierPermille(rarity) / 1000;
                text.Append("\nLực Đạo: ").Append(attack)
                    .Append("\nTốc Xuất Chiêu: ").Append(Fmt(item.cooldown)).Append(" giây")
                    .Append("\nHội Tâm: ").Append(Fmt(item.crit * 100)).Append('%');
            }
            if (raw.basic.Count > 0) text.Append("\n\n<b>Phổ Công</b>: ").Append(Effects(raw.basic));
            if (raw.ultimate != null)
                text.Append("\n<b>Tuyệt Kỹ</b> (tốn ").Append(item.ultimate.cost)
                    .Append(" nội lực): ").Append(Effects(raw.ultimate.effects));
            foreach (TriggerDef trigger in raw.triggers) text.Append("\n• ").Append(DescribeTrigger(trigger));
            if (raw.unlocks.Count > 0) text.Append("\n\n<b>Đột Phá</b>");
            foreach (UnlockDef unlock in raw.unlocks)
            {
                bool open = tier >= unlock.tier;
                text.Append("\n");
                if (!open) text.Append("<color=#777777>");
                text.Append("T").Append(unlock.tier).Append(": ").Append(DescribeUnlock(unlock));
                if (!open) text.Append(" (mở ở Trọng ").Append(unlock.tier).Append(")</color>");
            }
            return text.ToString();
        }

        public static string DescribeMaterial(MaterialInfo material, int count = 0)
        {
            var text = new StringBuilder("<b>").Append(material.Name).Append("</b>");
            if (count > 0) text.Append("\nSố lượng: ").Append(count);
            if (!string.IsNullOrWhiteSpace(material.Description)) text.Append("\n").Append(material.Description);
            text.Append("\nDùng cho: ").Append(TypeName(material.ItemType));
            if (material.IsCatalyst) text.Append(" · Chất dẫn Đột Phá");
            else if (material.Tier > 0) text.Append(" · nâng Trọng bậc ").Append(material.Tier);
            if (material.PvpOnly) text.Append(" · nhận từ Luận Kiếm Đài");
            return text.ToString();
        }

        internal static string DescribeEffect(EffectDef effect)
        {
            string target = TargetText(effect.target);
            switch (effect.op)
            {
                case EffectOp.DAMAGE:
                    string amount = effect.value != 0 ? effect.value.ToString() : Fmt(effect.pct * 100) + "% Lực Đạo";
                    string damage = effect.hits > 1 ? $"Sát thương {effect.hits} nhát × {amount}" : "Sát thương " + amount;
                    if (effect.ignoreShield) damage += " (xuyên Cương Khí)";
                    return damage + (effect.target == Target.SELF ? " vào bản thân" : "");
                case EffectOp.GAIN_QI: return "+" + effect.value + " nội lực";
                case EffectOp.DRAIN_QI: return "đối thủ −" + effect.value + " nội lực";
                case EffectOp.SHIELD: return "+" + effect.value + " Cương Khí";
                case EffectOp.HEAL: return "hồi " + effect.value + " khí huyết";
                case EffectOp.SHIELD_BREAK: return "Phá Cương " + effect.value;
                case EffectOp.DMG_REDUCE: return "Hộ Thân +" + Fmt(effect.pct * 100) + "%";
                case EffectOp.REDUCE_MAX_HP: return "Nội Thương: đối thủ −" + effect.value + " khí huyết tối đa";
                case EffectOp.PARRY_DISCOUNT: return "Hóa Kình tốn ít hơn " + effect.value + " nội lực";
                case EffectOp.APPLY:
                    string status = StatusName(effect.status);
                    if (effect.status == StatusType.BURN || effect.status == StatusType.POISON || effect.status == StatusType.BLEED)
                        return "+" + effect.stacks + " điểm " + status;
                    if (effect.status == StatusType.REGEN) return "+" + effect.stacks + " Điều Tức";
                    return status + " " + Fmt(effect.duration) + "s" + target;
                case EffectOp.CHARGE: return "Vận Khí " + Fmt(effect.duration) + "s" + (target.Length > 0 ? target : " lên món này");
                case EffectOp.ADD_ATTACK: return "+" + effect.value + " Lực Đạo" + target;
                case EffectOp.ADD_CRIT: return "+" + Fmt(effect.pct * 100) + "% Hội Tâm" + target;
                default: return effect.op?.ToString() ?? string.Empty;
            }
        }

        internal static string DescribeTrigger(TriggerDef trigger)
        {
            string head = EventName(trigger.on);
            if (trigger.on == TriggerEvent.HP_BELOW) head += " " + Fmt(trigger.hpBelow * 100) + "%";
            if (trigger.status.HasValue) head += " (" + StatusName(trigger.status) + ")";
            if (trigger.from == TriggerSource.SELF) head += " (chính món này)";
            else if (trigger.from == TriggerSource.ADJACENT) head += " (món liền kề)";
            string value = head + ": " + Effects(trigger.effects);
            if (trigger.maxUses > 0) value += " (tối đa " + trigger.maxUses + " lần)";
            return value;
        }

        internal static string DescribeUnlock(UnlockDef unlock)
        {
            var parts = new List<string>();
            if (unlock.attack != 0) parts.Add("Lực Đạo " + Signed(unlock.attack));
            if (Math.Abs(unlock.cooldown) > .00001) parts.Add("Tốc Xuất Chiêu " + Signed(unlock.cooldown) + "s");
            if (Math.Abs(unlock.crit) > .00001) parts.Add("Hội Tâm " + Signed(unlock.crit * 100) + "%");
            if (unlock.ultimateCost != 0) parts.Add("Chi phí Tuyệt Kỹ " + Signed(unlock.ultimateCost));
            if (unlock.basic.Count > 0) parts.Add("Phổ Công thêm: " + Effects(unlock.basic));
            if (unlock.ultimate.Count > 0) parts.Add("Tuyệt Kỹ thêm: " + Effects(unlock.ultimate));
            foreach (TriggerDef trigger in unlock.triggers) parts.Add(DescribeTrigger(trigger));
            return string.Join("; ", parts);
        }

        private static EngineItemCatalog Catalog
        {
            get
            {
                if (catalog != null) return catalog;
                TextAsset asset = Resources.Load<TextAsset>("Data/items");
                if (asset == null) throw new InvalidOperationException("Could not load Resources/Data/items.json.");
                return catalog = EngineItemCatalog.Load(asset.text);
            }
        }

        private static string Effects(IReadOnlyList<EffectDef> effects)
        { var values = new List<string>(); foreach (EffectDef effect in effects) values.Add(DescribeEffect(effect)); return string.Join(", ", values); }
        private static string Fmt(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
        private static string Signed(double value) => (value >= 0 ? "+" : "") + Fmt(value);
        private static string Signed(int value) => (value >= 0 ? "+" : "") + value;
        private static string WeaponClass(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : " · " + value;
        private static string TypeName(ItemType? value) => value == ItemType.WEAPON ? "Binh Khí" : value == ItemType.ARMOR ? "Hộ Cụ" : value == ItemType.MANUAL ? "Công Pháp" : value == ItemType.PILL ? "Đan Dược" : "Kỳ Trân";
        private static string TypeName(string value) => value == "weapon" ? "Binh Khí" : value == "armor" ? "Hộ Cụ" : value == "manual" ? "Công Pháp" : value == "pill" ? "Đan Dược" : value == "all" ? "mọi loại vật phẩm" : "Kỳ Trân";
        private static string RarityName(string value) { string[] names = { "Phàm Phẩm", "Lương Phẩm", "Tinh Phẩm", "Tuyệt Phẩm", "Thần Binh", "Chí Tôn" }; return names[RarityRules.Index(value)]; }
        private static string StatusName(StatusType? value) => value == StatusType.BURN ? "Liệt Hỏa" : value == StatusType.POISON ? "Kịch Độc" : value == StatusType.BLEED ? "Xuất Huyết" : value == StatusType.REGEN ? "Điều Tức" : value == StatusType.STUN ? "Định Thân" : value == StatusType.SLOW ? "Trì Trệ" : value == StatusType.SEAL ? "Điểm Huyệt" : value == StatusType.HASTE ? "Khinh Công" : string.Empty;
        private static string EventName(TriggerEvent? value) => value == TriggerEvent.BATTLE_START ? "Vào trận" : value == TriggerEvent.HIT ? "Khi đánh trúng" : value == TriggerEvent.CRIT ? "Khi Hội Tâm" : value == TriggerEvent.ULTIMATE ? "Khi tung Tuyệt Kỹ" : value == TriggerEvent.PARRY_SUCCESS ? "Khi Hóa Kình thành công" : value == TriggerEvent.PARRIED ? "Khi bị Hóa Kình" : value == TriggerEvent.HP_BELOW ? "Khi khí huyết dưới" : value == TriggerEvent.STATUS_APPLIED ? "Khi gây trạng thái" : "Khi trạng thái bộc phát";
        private static string TargetText(Target? value) => value.HasValue ? " lên " + (value == Target.SELF ? "bản thân" : value == Target.ENEMY ? "đối thủ" : value == Target.SELF_ITEM ? "món này" : value == Target.EVENT_SOURCE ? "nguồn sự kiện" : value == Target.SELF_ALL_ITEMS ? "toàn bộ đồ bản thân" : value == Target.SELF_OTHER_ITEMS ? "các món khác bản thân" : value == Target.SELF_ALL_WEAPONS ? "toàn bộ binh khí bản thân" : value == Target.ADJACENT_ITEMS ? "đồ liền kề" : value == Target.ADJACENT_WEAPONS ? "binh khí liền kề" : value == Target.ENEMY_RANDOM_ITEM ? "1 món ngẫu nhiên của địch" : value == Target.ENEMY_RANDOM_WEAPON ? "1 binh khí ngẫu nhiên của địch" : "toàn bộ đồ địch") : string.Empty;
    }
}
