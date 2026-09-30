using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using UnityEngine;

namespace OneBagJianghu.Engine.Unity
{
    public sealed class LocalBattleOutput
    {
        public JObject Result { get; }
        public OpponentInfo Opponent { get; }
        public LocalBattleOutput(JObject result, OpponentInfo opponent) { Result = result; Opponent = opponent; }
    }
    public static class LocalBattleService
    {
        private static BattleSimulator simulator;
        private static List<Loadout> samples;
        public static LocalBattleOutput RunBattle(string playerLoadoutJson, GameMode mode)
        {
            EnsureLoaded();
            Loadout player = Loadout.FromJson(playerLoadoutJson);
            player.hp = 1200;
            player.cols = 5;
            player.rows = 4;
            Loadout opponent = samples[Random.Range(0, samples.Count)];
            long seed = unchecked(((long)(uint)Random.Range(int.MinValue, int.MaxValue) << 32) | (uint)Random.Range(int.MinValue, int.MaxValue));
            BattleResult result = simulator.Run(player, opponent, seed);
            return new LocalBattleOutput(result.ToJObject(), new OpponentInfo { Name = opponent.name, Sect = opponent.sect });
        }
        private static void EnsureLoaded()
        {
            if (simulator != null) return;
            TextAsset itemData = Resources.Load<TextAsset>("Data/items");
            TextAsset sectData = Resources.Load<TextAsset>("Data/sects");
            if (itemData == null || sectData == null) throw new DataException("Offline battle data is missing.");
            simulator = new BattleSimulator(ItemCatalog.Load(itemData.text), SectCatalog.Load(sectData.text), BattleConfig.Defaults());
            samples = new List<Loadout>();
            foreach (TextAsset asset in Resources.LoadAll<TextAsset>("Data/Samples"))
            {
                Loadout sample = Loadout.FromJson(asset.text);
                sample.name = DisplayName(sample.name);
                samples.Add(sample);
            }
            if (samples.Count == 0) throw new DataException("Offline opponent samples are missing.");
        }

        // Sample names carry a weapon note for the balance tools, e.g. "Huyền Độc Môn (Ám Khí + Trảo)";
        // players only see the sect part.
        internal static string DisplayName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            int open = name.IndexOf(" (", System.StringComparison.Ordinal);
            return open > 0 ? name.Substring(0, open) : name;
        }
    }
}
