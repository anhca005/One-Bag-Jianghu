using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Pve
{
    public sealed class PveCharacter
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("portrait")]
        public string Portrait;
    }

    public sealed class PveTheme
    {
        [JsonProperty("mapBackground")]
        public string MapBackground;
        [JsonProperty("battleBackground")]
        public string BattleBackground;
        [JsonProperty("accent")]
        public string Accent;
        [JsonProperty("paper")]
        public string Paper;
        [JsonProperty("ink")]
        public string Ink;
        [JsonProperty("nodeIcon")]
        public string NodeIcon;
        [JsonProperty("bossNodeIcon")]
        public string BossNodeIcon;
    }

    public sealed class PveUnlock
    {
        [JsonProperty("afterChapter")]
        public string AfterChapter;
    }

    public sealed class PveEnemyItem
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("x")]
        public int X;
        [JsonProperty("y")]
        public int Y;
        [JsonProperty("tier")]
        public int Tier = 1;
        [JsonProperty("rarity")]
        public string Rarity = "common";
    }

    public sealed class PveEnemy
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("characterId")]
        public string CharacterId;
        [JsonProperty("portrait")]
        public string Portrait;
        [JsonProperty("sect")]
        public string Sect;
        [JsonProperty("tamPhapLevel")]
        public int TamPhapLevel = 1;
        [JsonProperty("level")]
        public int Level = 1;
        [JsonProperty("items")]
        public List<PveEnemyItem> Items;
    }

    public sealed class PveRewardItem
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("tier")]
        public int Tier = 1;
        [JsonProperty("rarity")]
        public string Rarity = "common";
    }

    public sealed class PveFirstClearReward
    {
        [JsonProperty("silver")]
        public long Silver;
        [JsonProperty("heroExp")]
        public long HeroExp;
        [JsonProperty("materials")]
        public Dictionary<string, int> Materials;
        [JsonProperty("items")]
        public List<PveRewardItem> Items;
    }

    public sealed class PveDrop
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("chancePermille")]
        public int ChancePermille;
        [JsonProperty("rarity")]
        public string Rarity = "common";
    }

    public sealed class PveRewards
    {
        [JsonProperty("firstClear")]
        public PveFirstClearReward FirstClear;
        [JsonProperty("repeat")]
        public string Repeat;
        [JsonProperty("drops")]
        public List<PveDrop> Drops;
    }

    public sealed class PveDialogueLine
    {
        [JsonProperty("speaker")]
        public string Speaker;
        [JsonProperty("text")]
        public string Text;
    }

    public sealed class PveDialogue
    {
        [JsonProperty("before")]
        public List<PveDialogueLine> Before;
        [JsonProperty("afterWin")]
        public List<PveDialogueLine> AfterWin;
        [JsonProperty("afterLoss")]
        public List<PveDialogueLine> AfterLoss;
    }

    public sealed class PveStage
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("index")]
        public int Index;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("kind")]
        public string Kind;
        [JsonProperty("mapPos")]
        public float[] MapPos;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("staminaCost")]
        public int StaminaCost;
        [JsonProperty("recommendedPower")]
        public int RecommendedPower;
        [JsonProperty("recommendedLevel")]
        public int RecommendedLevel;
        [JsonProperty("enemy")]
        public PveEnemy Enemy;
        [JsonProperty("rewards")]
        public PveRewards Rewards;
        [JsonProperty("dialogue")]
        public PveDialogue Dialogue;
        [JsonIgnore]
        public PveChapter Chapter;
    }

    public sealed class PveChapter
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("index")]
        public int Index;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("summary")]
        public string Summary;
        [JsonProperty("theme")]
        public PveTheme Theme;
        [JsonProperty("unlock")]
        public PveUnlock Unlock;
        [JsonProperty("stages")]
        public List<PveStage> Stages;
    }

    public sealed class PveCatalog
    {
        [JsonProperty("repeatRewardPercent")]
        public int RepeatRewardPercent = 15;
        [JsonProperty("characters")]
        public List<PveCharacter> Characters;
        [JsonProperty("monsterItems")]
        public List<string> MonsterItems;
        [JsonProperty("chapters")]
        public List<PveChapter> Chapters;
        [JsonIgnore]
        private readonly Dictionary<string, PveStage> stages = new Dictionary<string, PveStage>(StringComparer.Ordinal);
        [JsonIgnore]
        private readonly Dictionary<string,
            PveCharacter> characters = new Dictionary<string,
            PveCharacter>(StringComparer.Ordinal);
        public static PveCatalog Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/Pve/chapters");
            if (asset == null)
                throw new InvalidOperationException("Could not load Resources/Data/Pve/chapters.json.");
            return Parse(asset.text, ItemCatalog.LoadFromResources());
        }

        public static PveCatalog Parse(string json, ItemCatalog items)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                PveCatalog value = JsonConvert.DeserializeObject<PveCatalog>(json,
                    settings) ?? throw new FormatException("PvE catalog JSON is null.");
                value.Validate(items ?? throw new ArgumentNullException(nameof(items)));
                return value;
            }
            catch (JsonException e)
            {
                throw new FormatException("Invalid PvE JSON: " + e.Message, e);
            }
        }

        public PveStage GetStage(string id) => id != null && stages.TryGetValue(id, out PveStage stage) ? stage : null;
        public PveCharacter GetCharacter(string id) => id != null
            && characters.TryGetValue(id,
            out PveCharacter value) ? value : null;
        public HashSet<string> MonsterItemSet() => new HashSet<string>(MonsterItems ?? new List<string>(),
            StringComparer.Ordinal);
        private void Validate(ItemCatalog items)
        {
            if (RepeatRewardPercent < 0 || RepeatRewardPercent > 100)
                throw new FormatException("PvE repeatRewardPercent must be from 0 to 100.");
            if (Characters == null || Chapters == null || Chapters.Count == 0)
                throw new FormatException("PvE catalog requires characters and chapters.");
            characters.Clear();
            stages.Clear();
            foreach (PveCharacter c in Characters)
                if (c == null
                    || string.IsNullOrWhiteSpace(c.Id)
                    || string.IsNullOrWhiteSpace(c.Name)
                    || !characters.TryAdd(c.Id,
                    c))
                    throw new FormatException("PvE character ids must be non-empty and unique.");
            var chapterIds = new HashSet<string>(StringComparer.Ordinal);
            var itemIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (ItemInfo item in items.All)
                itemIds.Add(item.Id);
            foreach (string id in MonsterItems ?? new List<string>())
                if (!itemIds.Contains(id))
                    throw new FormatException("Unknown monster item id '" + id + "'.");
            foreach (PveChapter chapter in Chapters)
            {
                if (chapter == null || string.IsNullOrWhiteSpace(chapter.Id) || !chapterIds.Add(chapter.Id))
                    throw new FormatException("PvE chapter ids must be non-empty and unique.");
                ValidateTheme(chapter.Theme, chapter.Id);
                if (chapter.Stages == null || chapter.Stages.Count == 0)
                    throw new FormatException("Chapter '" + chapter.Id + "' has no stages.");
                foreach (PveStage stage in chapter.Stages)
                    ValidateStage(stage, chapter, items, itemIds);
            }

            foreach (PveChapter chapter in Chapters)
                if (!string.IsNullOrEmpty(chapter.Unlock?.AfterChapter)
                    && !chapterIds.Contains(chapter.Unlock.AfterChapter))
                    throw new FormatException("Chapter '" + chapter.Id + "' references unknown prerequisite chapter.");
        }

        private void ValidateStage(PveStage stage, PveChapter chapter, ItemCatalog items, HashSet<string> itemIds)
        {
            if (stage == null || string.IsNullOrWhiteSpace(stage.Id) || !stages.TryAdd(stage.Id, stage))
                throw new FormatException("PvE stage ids must be non-empty and unique.");
            stage.Chapter = chapter;
            if (stage.Kind != "normal" && stage.Kind != "elite" && stage.Kind != "boss")
                throw new FormatException("Stage '" + stage.Id + "' has invalid kind.");
            if (stage.MapPos == null
                || stage.MapPos.Length != 2
                || stage.MapPos[0] < 0
                || stage.MapPos[0] > 1
                || stage.MapPos[1] < 0
                || stage.MapPos[1] > 1)
                throw new FormatException("Stage '" + stage.Id + "' mapPos must contain two values from 0 to 1.");
            if (stage.StaminaCost < 0 || stage.Enemy == null || stage.Enemy.Level < 1 || stage.Enemy.Items == null)
                throw new FormatException("Stage '"
                    + stage.Id
                    + "' has incomplete enemy/stamina data; enemy level must be >= 1.");
            if (!string.IsNullOrWhiteSpace(stage.Enemy.CharacterId) && !characters.ContainsKey(stage.Enemy.CharacterId))
                throw new FormatException("Stage '" + stage.Id + "' enemy references an unknown character.");
            bool[,] occupied = new bool[5, 4];
            foreach (PveEnemyItem entry in stage.Enemy.Items)
            {
                if (entry == null || !itemIds.Contains(entry.Id))
                    throw new FormatException("Stage '"
                        + stage.Id
                        + "' references unknown item id '"
                        + entry?.Id
                        + "'.");
                if (entry.Tier < 1 || entry.Tier > 9 || !Economy.RarityRules.IsValid(entry.Rarity))
                    throw new FormatException("Stage '" + stage.Id + "' has invalid item tier/rarity.");
                ItemInfo info = items.Get(entry.Id);
                for (int x = entry.X; x < entry.X + info.Width; x++)
                    for (int y = entry.Y; y < entry.Y + info.Height; y++)
                    {
                        if (x < 0 || x >= 5 || y < 0 || y >= 4 || occupied[x, y])
                            throw new FormatException("Stage '" + stage.Id + "' enemy bag is outside 5x4 or overlaps.");
                        occupied[x, y] = true;
                    }
            }

            ValidateRewardItems(stage, itemIds);
            ValidateDialogue(stage.Dialogue?.Before, stage.Id);
            ValidateDialogue(stage.Dialogue?.AfterWin, stage.Id);
            ValidateDialogue(stage.Dialogue?.AfterLoss, stage.Id);
        }

        private void ValidateRewardItems(PveStage stage, HashSet<string> itemIds)
        {
            if (stage.Rewards?.FirstClear == null || string.IsNullOrWhiteSpace(stage.Rewards.Repeat))
                throw new FormatException("Stage '" + stage.Id + "' has incomplete rewards.");
            foreach (PveRewardItem item in stage.Rewards.FirstClear.Items ?? new List<PveRewardItem>())
                if (!itemIds.Contains(item.Id))
                    throw new FormatException("Stage '"
                        + stage.Id
                        + "' reward references unknown item id '"
                        + item.Id
                        + "'.");
            foreach (PveDrop drop in stage.Rewards.Drops ?? new List<PveDrop>())
                if (!itemIds.Contains(drop.Id) || drop.ChancePermille < 0 || drop.ChancePermille > 1000)
                    throw new FormatException("Stage '" + stage.Id + "' has an invalid drop.");
        }

        private void ValidateDialogue(List<PveDialogueLine> lines, string stage)
        {
            foreach (PveDialogueLine line in lines ?? new List<PveDialogueLine>())
                if (line == null || !characters.ContainsKey(line.Speaker) || string.IsNullOrWhiteSpace(line.Text))
                    throw new FormatException("Stage '" + stage + "' dialogue has an unknown speaker or empty text.");
        }

        private static void ValidateTheme(PveTheme t, string chapter)
        {
            if (t == null
                || string.IsNullOrWhiteSpace(t.MapBackground)
                || string.IsNullOrWhiteSpace(t.BattleBackground)
                || string.IsNullOrWhiteSpace(t.Accent)
                || string.IsNullOrWhiteSpace(t.Paper)
                || string.IsNullOrWhiteSpace(t.Ink)
                || string.IsNullOrWhiteSpace(t.NodeIcon)
                || string.IsNullOrWhiteSpace(t.BossNodeIcon))
                throw new FormatException("Chapter '" + chapter + "' theme keys are required.");
            if (!ColorUtility.TryParseHtmlString(t.Accent,
                out _)
                || !ColorUtility.TryParseHtmlString(t.Paper,
                out _)
                || !ColorUtility.TryParseHtmlString(t.Ink,
                out _))
                throw new FormatException("Chapter '" + chapter + "' theme colors are invalid.");
        }
    }
}
