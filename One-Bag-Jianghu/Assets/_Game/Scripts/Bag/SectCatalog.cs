using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using OneBagJianghu.Engine;
using OneBagJianghu.UI.Common;
using UnityEngine;

namespace OneBagJianghu.Bag
{
    public sealed class SectLevelInfo
    {
        public int Level { get; }
        public float AttackPct { get; }
        public float CooldownPct { get; }
        public float Crit { get; }
        public int UltimateCost { get; }
        public IReadOnlyList<EffectDef> Basic { get; }
        public IReadOnlyList<EffectDef> Ultimate { get; }
        public IReadOnlyList<TriggerDef> Triggers { get; }

        internal SectLevelInfo(SectLevelDto source)
        {
            Level = source.Level;
            AttackPct = source.AttackPct;
            CooldownPct = source.CooldownPct;
            Crit = source.Crit;
            UltimateCost = source.UltimateCost;
            Basic = source.Basic ?? Array.Empty<EffectDef>();
            Ultimate = source.Ultimate ?? Array.Empty<EffectDef>();
            Triggers = source.Triggers ?? Array.Empty<TriggerDef>();
        }

        /// <summary>Builds player-facing Vietnamese copy for this Tâm Pháp milestone.</summary>
        public string Describe()
        {
            var parts = new List<string>();
            AddPercent(parts, AttackPct, "Lực Đạo");
            AddPercent(parts, CooldownPct, "Giảm Hồi Chiêu");
            AddPercent(parts, Crit, "Hội Tâm");
            if (UltimateCost != 0)
            {
                parts.Add("Chi phí Tuyệt Kỹ " + Signed(UltimateCost));
            }

            AddEffects(parts, "Phổ Công thêm: ", Basic);
            AddEffects(parts, "Tuyệt Kỹ thêm: ", Ultimate);
            foreach (TriggerDef trigger in Triggers)
            {
                parts.Add(ItemDescriber.DescribeTrigger(trigger));
            }

            return parts.Count > 0 ? string.Join("; ", parts) : "Mở hiệu ứng Tâm Pháp";
        }

        private static void AddPercent(List<string> parts, float value, string label)
        {
            if (Math.Abs(value) > .00001f)
            {
                parts.Add("+" + Format(value * 100f) + "% " + label);
            }
        }

        private static void AddEffects(
            List<string> parts,
            string prefix,
            IReadOnlyList<EffectDef> effects)
        {
            if (effects.Count == 0)
            {
                return;
            }

            var descriptions = new List<string>();
            foreach (EffectDef effect in effects)
            {
                descriptions.Add(ItemDescriber.DescribeEffect(effect));
            }

            parts.Add(prefix + string.Join(", ", descriptions));
        }

        private static string Format(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static string Signed(int value)
        {
            return (value >= 0 ? "+" : string.Empty) + value;
        }
    }

    public sealed class SectInfo
    {
        public string Id { get; }
        public string Name { get; }
        public string TamPhap { get; }
        public string Lore { get; }
        public Color Accent { get; }
        public IReadOnlyList<string> WeaponClasses { get; }
        public IReadOnlyList<SectLevelInfo> Levels { get; }
        public string LevelOneEffect => Levels.Count > 0 ? Levels[0].Describe() : string.Empty;

        internal SectInfo(SectDto source)
        {
            Id = source.Id;
            Name = source.Name;
            TamPhap = source.TamPhap;
            WeaponClasses = source.WeaponClasses ?? Array.Empty<string>();
            var levels = new List<SectLevelInfo>();
            foreach (SectLevelDto level in source.Levels ?? Array.Empty<SectLevelDto>())
            {
                levels.Add(new SectLevelInfo(level));
            }

            Levels = levels;
            Lore = SectPresentation.Lore(Id);
            Accent = SectPresentation.Accent(Id);
        }

        public bool Buffs(string weaponClass)
        {
            if (string.IsNullOrEmpty(weaponClass))
            {
                return false;
            }

            for (int i = 0; i < WeaponClasses.Count; i++)
            {
                if (string.Equals(WeaponClasses[i], weaponClass, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Returns the sect weapon classes as concise Vietnamese labels.</summary>
        public string DescribeWeaponClasses()
        {
            var names = new List<string>();
            foreach (string weaponClass in WeaponClasses)
            {
                names.Add(SectPresentation.WeaponClassName(weaponClass));
            }

            return string.Join(" · ", names);
        }
    }

    public sealed class SectCatalog
    {
        private readonly List<SectInfo> all;
        private readonly Dictionary<string, SectInfo> byId;

        private SectCatalog(List<SectInfo> sects)
        {
            all = sects;
            byId = new Dictionary<string, SectInfo>(StringComparer.Ordinal);
            foreach (SectInfo sect in sects)
            {
                if (string.IsNullOrWhiteSpace(sect.Id))
                {
                    throw new FormatException("A sect definition has no id.");
                }

                if (!byId.TryAdd(sect.Id, sect))
                {
                    throw new FormatException($"Duplicate sect id '{sect.Id}'.");
                }
            }
        }

        public IReadOnlyList<SectInfo> All => all;

        public static SectCatalog Parse(string json)
        {
            List<SectDto> data = JsonConvert.DeserializeObject<List<SectDto>>(json)
                ?? throw new FormatException("Sect catalog JSON is null.");
            var sects = new List<SectInfo>(data.Count);
            foreach (SectDto dto in data)
            {
                sects.Add(new SectInfo(dto));
            }

            return new SectCatalog(sects);
        }

        public static SectCatalog LoadFromResources()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/sects");
            if (asset == null)
            {
                throw new InvalidOperationException("Could not load Resources/Data/sects.json.");
            }

            return Parse(asset.text);
        }

        public bool TryGet(string id, out SectInfo sect)
        {
            if (id != null)
            {
                return byId.TryGetValue(id, out sect);
            }

            sect = null;
            return false;
        }
    }

    internal sealed class SectDto
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("tamPhap")]
        public string TamPhap;
        [JsonProperty("weaponClasses")]
        public string[] WeaponClasses;
        [JsonProperty("levels")]
        public SectLevelDto[] Levels;
    }

    internal sealed class SectLevelDto
    {
        [JsonProperty("level")]
        public int Level;
        [JsonProperty("attackPct")]
        public float AttackPct;
        [JsonProperty("cooldownPct")]
        public float CooldownPct;
        [JsonProperty("crit")]
        public float Crit;
        [JsonProperty("ultimateCost")]
        public int UltimateCost;
        [JsonProperty("basic")]
        public EffectDef[] Basic;
        [JsonProperty("ultimate")]
        public EffectDef[] Ultimate;
        [JsonProperty("triggers")]
        public TriggerDef[] Triggers;
    }

    internal static class SectPresentation
    {
        public static string WeaponClassName(string value)
        {
            return value switch
            {
                "fist" => "Quyền Sáo",
                "palm" => "Chưởng",
                "claw" => "Trảo",
                "sword" => "Kiếm",
                "blade" => "Đao",
                "dual" => "Song Đao / Song Kiếm",
                "spear" => "Thương",
                "staff" => "Côn / Bổng",
                "axe" => "Phủ",
                "hammer" => "Chùy",
                "whip" => "Roi",
                "fan" => "Phiến",
                "brush" => "Phán Quan Bút",
                "zither" => "Cầm",
                "flute" => "Tiêu",
                "hidden" => "Ám Khí",
                "bow" => "Cung / Nỏ",
                _ => "Binh Khí"
            };
        }

        public static string Lore(string id)
        {
            return id switch
            {
                "kim_cang" => "Phật gia uy nghiêm, lấy cương khắc nhu và bảo hộ bình yên Cửu Châu.",
                "thai_thanh" => "Đạo gia thanh tao, chú trọng Hóa Kình và tích tụ Nội Lực bộc phát.",
                "liet_diem" => "Võ học cuồng mãnh vùng hỏa sơn, đao pháp mang nhiệt khí Liệt Hỏa.",
                "huyen_doc" => "Ẩn nơi đầm lầy, dùng Kịch Độc và Xuất Huyết bào mòn đối thủ.",
                "han_son" => "Danh môn đỉnh núi tuyết, giỏi làm Trì Trệ và Điểm Huyệt đối thủ.",
                "thiet_phu" => "Bang hội hào sảng, nổi danh với lực bộc phát và uy lực Phá Cương.",
                "bich_thuy" => "Dùng âm luật hồi phục Khí Huyết và hóa giải trạng thái bất lợi.",
                _ => string.Empty
            };
        }

        public static Color Accent(string id)
        {
            return id switch
            {
                "kim_cang" => new Color(.72f, .48f, .16f),
                "thai_thanh" => new Color(.24f, .55f, .56f),
                "liet_diem" => new Color(.72f, .18f, .10f),
                "huyen_doc" => new Color(.36f, .42f, .18f),
                "han_son" => new Color(.30f, .52f, .68f),
                "thiet_phu" => new Color(.40f, .31f, .25f),
                "bich_thuy" => new Color(.22f, .58f, .52f),
                _ => new Color(.35f, .25f, .18f)
            };
        }
    }
}
