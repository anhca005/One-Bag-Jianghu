using System;
using System.Collections.Generic;
using OneBagJianghu.Battle.Replay;

namespace OneBagJianghu.Audio
{
    public sealed class BattleSfxResolver
    {
        private readonly Dictionary<int, ItemSfxMap> byUid = new Dictionary<int, ItemSfxMap>();
        private readonly Dictionary<BattleEventType, string> eventKeys =
            new Dictionary<BattleEventType, string>();
        private readonly int playerSide;
        private readonly bool bossStage;

        public BattleSfxResolver(BattleLog log,
            SfxMapDocument map,
            IReadOnlyList<SfxCatalogRecord> catalog,
            int playerSide = 0,
            bool bossStage = false)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (map == null)
                throw new ArgumentNullException(nameof(map));
            this.playerSide = playerSide;
            this.bossStage = bossStage;
            for (int i = 0; i < log.Items.Count; i++)
            {
                BattleItemRef item = log.Items[i];
                if (map.Items.TryGetValue(item.Id, out ItemSfxMap itemMap))
                {
                    byUid[item.Uid] = itemMap;
                }
            }

            BuildEventKeys(catalog);
        }

        public IReadOnlyList<string> Resolve(BattleEvent battleEvent)
        {
            if (battleEvent == null)
                return Array.Empty<string>();
            switch (battleEvent.Type)
            {
                case BattleEventType.Trigger:
                    return ResolveTrigger(battleEvent);
                case BattleEventType.Damage:
                    return ResolveDamage(battleEvent);
                case BattleEventType.Status:
                    return ResolveStatus(battleEvent.Kind, "_apply");
                case BattleEventType.Dot:
                case BattleEventType.Burst:
                    return ResolveStatus(battleEvent.Kind, "_tick");
                case BattleEventType.End:
                    return One(battleEvent.Side == playerSide ? "win" : "lose");
                case BattleEventType.Start:
                    return ResolveStart(bossStage);
                default:
                    return eventKeys.TryGetValue(battleEvent.Type, out string key) ?
                        One(key) :
                        Array.Empty<string>();
            }
        }

        public IReadOnlyList<string> ResolveStart(bool isBossStage)
        {
            return isBossStage ?
                new[] { "battle_start", "boss_intro" } :
                One("battle_start");
        }

        private IReadOnlyList<string> ResolveTrigger(BattleEvent battleEvent)
        {
            if (!TryGetItem(battleEvent, out ItemSfxMap item))
                return Array.Empty<string>();
            bool ultimate = string.Equals(battleEvent.Kind, "ULTIMATE", StringComparison.OrdinalIgnoreCase);
            if (!ultimate)
                return One(item.AttackKey);
            if (!string.IsNullOrEmpty(item.UltimateKey))
                return One(item.UltimateKey);
            return Pair("ultimate", item.AttackKey);
        }

        private IReadOnlyList<string> ResolveDamage(BattleEvent battleEvent)
        {
            if (battleEvent.Crit == true)
                return One("hit_crit");
            int damage = battleEvent.Value.GetValueOrDefault();
            if (battleEvent.Absorbed.GetValueOrDefault() > 0 && damage <= 0)
                return One("shield_hit");
            return TryGetItem(battleEvent, out ItemSfxMap item) ?
                One(item.HitKey) :
                Array.Empty<string>();
        }

        private static IReadOnlyList<string> ResolveStatus(string kind, string suffix)
        {
            if (string.IsNullOrEmpty(kind))
                return Array.Empty<string>();
            string normalized = kind.ToLowerInvariant();
            if (normalized == "burn" || normalized == "poison" || normalized == "bleed")
                return One(normalized + suffix);
            if (suffix == "_apply" && normalized == "stun")
                return One("stun");
            return Array.Empty<string>();
        }

        private void BuildEventKeys(IReadOnlyList<SfxCatalogRecord> catalog)
        {
            if (catalog == null)
                return;
            for (int i = 0; i < catalog.Count; i++)
            {
                SfxCatalogRecord record = catalog[i];
                if (Enum.TryParse(record.Event, true, out BattleEventType type) &&
                    !eventKeys.ContainsKey(type) &&
                    !IsSpecial(type))
                {
                    eventKeys[type] = record.Key;
                }
            }
        }

        private bool TryGetItem(BattleEvent battleEvent, out ItemSfxMap item)
        {
            item = null;
            return battleEvent.Item.HasValue && byUid.TryGetValue(battleEvent.Item.Value, out item);
        }

        private static bool IsSpecial(BattleEventType type)
        {
            return type == BattleEventType.Trigger || type == BattleEventType.Damage ||
                type == BattleEventType.Status || type == BattleEventType.Dot ||
                type == BattleEventType.Burst || type == BattleEventType.Start ||
                type == BattleEventType.End;
        }

        private static IReadOnlyList<string> One(string key)
        {
            return string.IsNullOrEmpty(key) ? Array.Empty<string>() : new[] { key };
        }

        private static IReadOnlyList<string> Pair(string first, string second)
        {
            if (string.IsNullOrEmpty(second))
                return One(first);
            return new[] { first, second };
        }
    }
}
