using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.Pve;
using UnityEngine;

namespace OneBagJianghu.Engine.Unity
{
    public sealed class LocalBattleOutput
    {
        public JObject Result { get; }
        public OpponentInfo Opponent { get; }

        public LocalBattleOutput(JObject result, OpponentInfo opponent)
        {
            Result = result;
            Opponent = opponent;
        }
    }

    public static class LocalBattleService
    {
        private static BattleSimulator simulator;
        private static List<Loadout> samples;
        public static LocalBattleOutput RunBattle(string playerLoadoutJson, GameMode mode)
        {
            EnsureLoaded();
            Loadout opponent = samples[UnityEngine.Random.Range(0, samples.Count)];
            return Run(playerLoadoutJson, opponent, null);
        }

        public static LocalBattleOutput RunStageBattle(string playerLoadoutJson, PveStage stage, long? seed = null)
        {
            if (stage?.Enemy == null)
                throw new ArgumentNullException(nameof(stage));
            EnsureLoaded();
            var opponent = new Loadout
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
                opponent.items.Add(
                    new Loadout.PlacedItem(item.Id, item.X, item.Y)
                    {
                        tier = item.Tier,
                        rarity = rarity
                    });
            }

            return Run(playerLoadoutJson, opponent, seed);
        }

        private static LocalBattleOutput Run(string playerJson, Loadout opponent, long? requestedSeed)
        {
            Loadout player = Loadout.FromJson(playerJson);
            player.cols = 5;
            player.rows = 4;
            long seed = requestedSeed ?? unchecked(
                ((long)(uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue) << 32) |
                (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            BattleResult result = simulator.Run(player, opponent, seed);
            return new LocalBattleOutput(
                result.ToJObject(),
                new OpponentInfo
                {
                    Name = opponent.name,
                    Sect = opponent.sect
                });
        }

        private static void EnsureLoaded()
        {
            if (simulator != null)
                return;
            TextAsset itemData = Resources.Load<TextAsset>("Data/items"),
                sectData = Resources.Load<TextAsset>("Data/sects");
            if (itemData == null || sectData == null)
                throw new DataException("Offline battle data is missing.");
            simulator = new BattleSimulator(
                ItemCatalog.Load(itemData.text),
                SectCatalog.Load(sectData.text),
                BattleConfig.Defaults());
            samples = new List<Loadout>();
            foreach (TextAsset asset in Resources.LoadAll<TextAsset>("Data/Samples"))
            {
                Loadout sample = Loadout.FromJson(asset.text);
                sample.name = DisplayName(sample.name);
                samples.Add(sample);
            }

            if (samples.Count == 0)
                throw new DataException("Offline opponent samples are missing.");
        }

        internal static string DisplayName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            int open = name.IndexOf(" (", StringComparison.Ordinal);
            return open > 0 ? name.Substring(0, open) : name;
        }
    }
}
