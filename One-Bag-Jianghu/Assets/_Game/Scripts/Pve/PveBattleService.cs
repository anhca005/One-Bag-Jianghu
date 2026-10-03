using System;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Engine;
using OneBagJianghu.Engine.Unity;
using OneBagJianghu.Net;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    public static class PveBattleService
    {
        private static BattleSimulator simulator;
        public static LocalBattleOutput RunBattle(string playerLoadoutJson, PveStage stage, long? requestedSeed = null)
        {
            if (stage?.Enemy == null)
                throw new ArgumentNullException(nameof(stage));
            EnsureLoaded();
            Loadout player = Loadout.FromJson(playerLoadoutJson);
            player.cols = 5;
            player.rows = 4;
            var enemy = new Loadout
            {
                name = stage.Enemy.Name,
                level = stage.Enemy.Level,
                cols = 5,
                rows = 4,
                sect = stage.Enemy.Sect,
                tamPhapLevel = stage.Enemy.TamPhapLevel
            };
            foreach (PveEnemyItem item in stage.Enemy.Items)
            {
                if (!Enum.TryParse(item.Rarity, true, out Rarity rarity))
                    throw new DataException("Unknown enemy item rarity '" + item.Rarity + "'.");
                enemy.items.Add(new Loadout.PlacedItem(item.Id, item.X, item.Y) { tier = item.Tier, rarity = rarity });
            }

            long seed = requestedSeed ?? unchecked(((long)(uint)UnityEngine.Random.Range(int.MinValue,
int.MaxValue) << 32) | (uint)UnityEngine.Random.Range(int.MinValue,
int.MaxValue));
            BattleResult result = simulator.Run(player, enemy, seed);
            return new LocalBattleOutput(result.ToJObject(), new OpponentInfo { Name = enemy.name, Sect = enemy.sect });
        }

        private static void EnsureLoaded()
        {
            if (simulator != null)
                return;
            TextAsset itemData =
                Resources.Load<TextAsset>("Data/items"),
                sectData =
                Resources.Load<TextAsset>("Data/sects");
            if (itemData == null || sectData == null)
                throw new DataException("Offline battle data is missing.");
            simulator = new BattleSimulator(ItemCatalog.Load(itemData.text),
SectCatalog.Load(sectData.text),
BattleConfig.Defaults());
        }
    }
}