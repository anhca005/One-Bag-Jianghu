using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OneBagJianghu.Economy;
using OneBagJianghu.Core;
using OneBagJianghu.Pve;
using UnityEngine;

namespace OneBagJianghu.Quests
{
    public sealed class QuestObjective
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("stage")]
        public string Stage;
        [JsonProperty("level")]
        public int Level;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("tier")]
        public int Tier;
    }

    public sealed class QuestReward
    {
        [JsonProperty("silver")]
        public long Silver;
        [JsonProperty("heroExp")]
        public long HeroExp;
        [JsonProperty("materials")]
        public Dictionary<string, int> Materials = new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public sealed class QuestDefinition
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("chapter")]
        public int Chapter;
        [JsonProperty("kind")]
        public string Kind;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("giver")]
        public string Giver;
        [JsonProperty("text")]
        public string Text;
        [JsonProperty("objective")]
        public QuestObjective Objective;
        [JsonProperty("goto")]
        public string Goto;
        [JsonProperty("reward")]
        public QuestReward Reward;
    }

    public sealed class QuestCatalog
    {
        private static readonly HashSet<string> ObjectiveTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "stage_clear", "hero_level", "sect_joined", "tam_phap_level", "equip_count", "item_tier",
            "shop_buy", "item_upgrade", "item_merge", "pve_win"
        };

        [JsonProperty("quests")]
        public List<QuestDefinition> Quests;

        /// <summary>Loads all story quests from Resources.</summary>
        public static QuestCatalog Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/quests");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/quests.json.");
            return Parse(asset.text, PveCatalog.Load(), GameSession.LoadMaterialCatalog());
        }

        /// <summary>Parses and validates a quest catalog.</summary>
        public static QuestCatalog Parse(string json, PveCatalog pve, MaterialCatalog materials)
        {
            try
            {
                var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
                QuestCatalog catalog = JsonConvert.DeserializeObject<QuestCatalog>(json, settings)
                    ?? throw new FormatException("Quest catalog JSON is null.");
                catalog.Validate(pve, materials);
                return catalog;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid quest JSON: " + exception.Message, exception);
            }
        }

        public QuestDefinition Get(string id)
        {
            return Quests?.Find(quest => string.Equals(quest.Id, id, StringComparison.Ordinal));
        }

        private void Validate(PveCatalog pve, MaterialCatalog materials)
        {
            if (Quests == null || Quests.Count == 0)
                throw new FormatException("Quest catalog requires at least one quest.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var materialIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (MaterialInfo material in materials.All)
                materialIds.Add(material.Id);
            foreach (QuestDefinition quest in Quests)
            {
                if (quest == null || string.IsNullOrWhiteSpace(quest.Id) || !ids.Add(quest.Id))
                    throw new FormatException("Quest ids must be non-empty and unique.");
                if (quest.Chapter < 1 || (quest.Kind != "main" && quest.Kind != "side"))
                    throw new FormatException("Quest '" + quest.Id + "' has invalid chapter or kind.");
                if (string.IsNullOrWhiteSpace(quest.Title) || string.IsNullOrWhiteSpace(quest.Giver)
                    || string.IsNullOrWhiteSpace(quest.Text) || quest.Objective == null || quest.Reward == null)
                    throw new FormatException("Quest '" + quest.Id + "' is incomplete.");
                ValidateObjective(quest, pve);
                ValidateGoto(quest, pve);
                foreach (KeyValuePair<string, int> reward in quest.Reward.Materials)
                    if (!materialIds.Contains(reward.Key) || reward.Value <= 0)
                        throw new FormatException("Quest '" + quest.Id + "' has an invalid material reward.");
                if (quest.Reward.Silver < 0 || quest.Reward.HeroExp < 0)
                    throw new FormatException("Quest '" + quest.Id + "' has a negative reward.");
            }
        }

        private static void ValidateGoto(QuestDefinition quest, PveCatalog pve)
        {
            if (quest.Goto == "shop" || quest.Goto == "storage" || quest.Goto == "bag" || quest.Goto == "sect")
                return;
            if (quest.Goto != null && quest.Goto.StartsWith("stage:", StringComparison.Ordinal)
                && pve.GetStage(quest.Goto.Substring(6)) != null)
                return;
            throw new FormatException("Quest '" + quest.Id + "' has an unknown goto target.");
        }

        private static void ValidateObjective(QuestDefinition quest, PveCatalog pve)
        {
            QuestObjective objective = quest.Objective;
            if (!ObjectiveTypes.Contains(objective.Type))
                throw new FormatException("Quest '" + quest.Id + "' has an unknown objective type.");
            if (objective.Type == "stage_clear" && pve.GetStage(objective.Stage) == null)
                throw new FormatException("Quest '" + quest.Id + "' references an unknown stage.");
            if ((objective.Type == "hero_level" || objective.Type == "tam_phap_level") && objective.Level < 1)
                throw new FormatException("Quest '" + quest.Id + "' requires a positive level.");
            if ((objective.Type == "equip_count" || objective.Type.EndsWith("_buy", StringComparison.Ordinal)
                || objective.Type == "item_upgrade" || objective.Type == "item_merge"
                || objective.Type == "pve_win") && objective.Count < 1)
                throw new FormatException("Quest '" + quest.Id + "' requires a positive count.");
            if (objective.Type == "item_tier" && (objective.Tier < 1 || objective.Tier > 9))
                throw new FormatException("Quest '" + quest.Id + "' has an invalid item tier.");
        }
    }
}
