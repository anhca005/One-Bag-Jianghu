using System;
using System.Collections.Generic;
using OneBagJianghu.Economy;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    public static class StagePower
    {
        public static int Enemy(PveStage stage)
        {
            return Enemy(stage, PowerCatalog.LoadFromResources());
        }

        public static int Enemy(PveStage stage, PowerCatalog power)
        {
            if (stage?.Enemy == null)
                throw new ArgumentNullException(nameof(stage));
            if (power == null)
                throw new ArgumentNullException(nameof(power));
            var items = new List<OwnedItem>();
            IEnumerable<PveEnemyItem> enemyItems = stage.Enemy.Items;
            enemyItems ??= Array.Empty<PveEnemyItem>();
            foreach (PveEnemyItem item in enemyItems)
            {
                items.Add(new OwnedItem
                {
                    ItemId = item.Id,
                    Tier = item.Tier,
                    Rarity = item.Rarity
                });
            }

            return power.Fighter(items, stage.Enemy.Level);
        }

        public static int Recommended(PveStage stage)
        {
            return Recommended(stage, PowerCatalog.LoadFromResources());
        }

        public static int Recommended(PveStage stage, PowerCatalog power)
        {
            if (stage == null)
                throw new ArgumentNullException(nameof(stage));
            if (stage.RecommendedPower > 0)
                return stage.RecommendedPower;
            return Mathf.RoundToInt(Enemy(stage, power) * power.RecommendedFactor);
        }
    }
}
