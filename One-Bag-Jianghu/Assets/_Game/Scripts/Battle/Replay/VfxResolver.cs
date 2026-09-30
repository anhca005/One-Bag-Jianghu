using System;
using OneBagJianghu.Bag;

namespace OneBagJianghu.Battle.Replay
{
    public enum ReplayVfxKind
    {
        None = 0,
        Travel,
        Burst,
        Parry,
        Hit
    }

    public readonly struct ReplayVfx
    {
        public ReplayVfx(ReplayVfxKind kind, string spriteKey)
        {
            Kind = kind;
            SpriteKey = spriteKey;
        }

        public ReplayVfxKind Kind { get; }
        public string SpriteKey { get; }
    }

    public static class VfxResolver
    {
        private static readonly ReplayVfx None = new ReplayVfx(ReplayVfxKind.None, null);

        public static ReplayVfx Resolve(BattleEvent battleEvent, ItemInfo item)
        {
            if (battleEvent == null)
            {
                return None;
            }

            if (battleEvent.Type == BattleEventType.Trigger)
            {
                return ResolveTrigger(battleEvent, item);
            }

            if (battleEvent.Type == BattleEventType.Burst && !string.IsNullOrWhiteSpace(battleEvent.Kind))
            {
                return new ReplayVfx(ReplayVfxKind.Burst, "burst_" + battleEvent.Kind.ToLowerInvariant());
            }

            if (battleEvent.Type == BattleEventType.Parry)
            {
                return new ReplayVfx(ReplayVfxKind.Parry, "parry");
            }

            if (battleEvent.Type == BattleEventType.Damage)
            {
                return new ReplayVfx(ReplayVfxKind.Hit, "hit");
            }

            return None;
        }

        public static string ResolveWeaponKey(string weaponClass, string triggerKind)
        {
            if (string.IsNullOrWhiteSpace(weaponClass))
            {
                return null;
            }

            bool basic = string.Equals(triggerKind, "BASIC", StringComparison.OrdinalIgnoreCase);
            bool ultimate = string.Equals(triggerKind, "ULTIMATE", StringComparison.OrdinalIgnoreCase);
            if (!basic && !ultimate)
            {
                return null;
            }

            return weaponClass.ToLowerInvariant() + (ultimate ? "_ultimate" : "_basic");
        }

        private static ReplayVfx ResolveTrigger(BattleEvent battleEvent, ItemInfo item)
        {
            if (item == null || !string.Equals(item.Type, "weapon", StringComparison.OrdinalIgnoreCase))
            {
                return None;
            }

            if (string.IsNullOrWhiteSpace(item.WeaponClass))
            {
                return None;
            }

            string key = ResolveWeaponKey(item.WeaponClass, battleEvent.Kind);
            if (key == null)
            {
                return None;
            }

            return new ReplayVfx(ReplayVfxKind.Travel, key);
        }
    }
}
