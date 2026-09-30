using System.Collections.Generic;

namespace OneBagJianghu.Battle.Replay.View.VfxLab
{
    public enum VfxCatalogGroup
    {
        Basic,
        Ultimate,
        Common
    }

    public sealed class VfxCatalogEntry
    {
        public VfxCatalogEntry(string key, ReplayVfxKind kind, string label, VfxCatalogGroup group, string eventKind)
        {
            Key = key;
            Kind = kind;
            Label = label;
            Group = group;
            EventKind = eventKind;
        }

        public string Key { get; }
        public ReplayVfxKind Kind { get; }
        public string Label { get; }
        public VfxCatalogGroup Group { get; }
        public string EventKind { get; }
    }

    public static class VfxCatalog
    {
        private static readonly string[] WeaponClasses =
        {
            "fist", "staff", "sword", "blade", "hidden", "claw", "axe", "brush", "flute",
            "zither", "palm", "fan", "spear", "hammer", "whip", "dual", "bow"
        };

        private static readonly string[] WeaponNames =
        {
            "Quyền Sáo", "Côn", "Kiếm", "Đao", "Ám Khí", "Trảo", "Rìu", "Bút", "Tiêu (sáo)",
            "Cầm", "Chưởng", "Phiến", "Thương", "Chùy", "Tiên", "Song Đao", "Cung"
        };

        private static readonly IReadOnlyList<VfxCatalogEntry> entries = Build();

        public static IReadOnlyList<VfxCatalogEntry> Entries => entries;

        private static IReadOnlyList<VfxCatalogEntry> Build()
        {
            var result = new List<VfxCatalogEntry>(39);
            AddWeapons(result, "BASIC", "Phổ Công", VfxCatalogGroup.Basic);
            AddWeapons(result, "ULTIMATE", "Tuyệt Kỹ", VfxCatalogGroup.Ultimate);
            result.Add(Common(BattleEventType.Damage, null, "Va Chạm"));
            result.Add(Common(BattleEventType.Parry, null, "Hóa Kình"));
            result.Add(Common(BattleEventType.Burst, "BURN", "Bộc Phát — Liệt Hỏa"));
            result.Add(Common(BattleEventType.Burst, "POISON", "Bộc Phát — Kịch Độc"));
            result.Add(Common(BattleEventType.Burst, "BLEED", "Bộc Phát — Xuất Huyết"));
            return result;
        }

        private static VfxCatalogEntry Common(BattleEventType type, string eventKind, string label)
        {
            ReplayVfx resolved = VfxResolver.Resolve(new BattleEvent { Type = type, Kind = eventKind }, null);
            return new VfxCatalogEntry(resolved.SpriteKey, resolved.Kind, label, VfxCatalogGroup.Common, eventKind);
        }

        private static void AddWeapons(List<VfxCatalogEntry> result, string eventKind, string suffix, VfxCatalogGroup group)
        {
            for (int i = 0; i < WeaponClasses.Length; i++)
            {
                result.Add(new VfxCatalogEntry(
                    VfxResolver.ResolveWeaponKey(WeaponClasses[i], eventKind),
                    ReplayVfxKind.Travel,
                    WeaponNames[i] + " — " + suffix,
                    group,
                    eventKind));
            }
        }
    }
}
