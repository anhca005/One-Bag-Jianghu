using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OneBagJianghu.Onboarding
{
    [Serializable]
    public sealed class IntroPanel
    {
        [JsonProperty("image")]
        public string Image;
        [JsonProperty("text")]
        public string Text;
    }

    [Serializable]
    public sealed class IntroCatalog
    {
        [JsonProperty("panels")]
        public List<IntroPanel> Panels = new List<IntroPanel>();

        public static IntroCatalog Load()
        {
            return Parse(LoadText("Data/intro"));
        }

        public static IntroCatalog Parse(string json)
        {
            IntroCatalog catalog = JsonConvert.DeserializeObject<IntroCatalog>(json);
            if (catalog?.Panels == null || catalog.Panels.Count == 0)
                throw new FormatException("Intro catalog must contain at least one panel.");
            foreach (IntroPanel panel in catalog.Panels)
                if (panel == null || string.IsNullOrWhiteSpace(panel.Image) || string.IsNullOrWhiteSpace(panel.Text))
                    throw new FormatException("Every intro panel requires an image and text.");
            return catalog;
        }

        internal static string LoadText(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null)
                throw new InvalidOperationException("Missing Resources catalog: " + path + ".json");
            return asset.text;
        }
    }

    [Serializable]
    public sealed class StoryChapter
    {
        [JsonProperty("chapter")]
        public int Chapter;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("panels")]
        public List<IntroPanel> Panels = new List<IntroPanel>();
    }

    [Serializable]
    public sealed class StoryCatalog
    {
        [JsonProperty("chapters")]
        public List<StoryChapter> Chapters = new List<StoryChapter>();

        public static StoryCatalog Load()
        {
            return Parse(IntroCatalog.LoadText("Data/story"));
        }

        public static StoryCatalog Parse(string json)
        {
            StoryCatalog catalog = JsonConvert.DeserializeObject<StoryCatalog>(json);
            var chapters = new HashSet<int>();
            if (catalog?.Chapters == null || catalog.Chapters.Count == 0)
                throw new FormatException("Story catalog must contain chapters.");
            foreach (StoryChapter chapter in catalog.Chapters)
            {
                if (chapter == null || chapter.Chapter < 0 || !chapters.Add(chapter.Chapter)
                    || string.IsNullOrWhiteSpace(chapter.Title)
                    || chapter.Panels == null
                    || chapter.Panels.Count == 0)
                    throw new FormatException("Every story chapter requires a unique number, title, and panels.");
                foreach (IntroPanel panel in chapter.Panels)
                    if (panel == null
                        || string.IsNullOrWhiteSpace(panel.Image)
                        || string.IsNullOrWhiteSpace(panel.Text))
                        throw new FormatException("Every story panel requires an image and text.");
            }

            return catalog;
        }

        public StoryChapter Get(int chapter)
        {
            return Chapters.Find(value => value.Chapter == chapter);
        }
    }

    [Serializable]
    public sealed class TutorialStep
    {
        [JsonProperty("line")]
        public string Line;
        [JsonProperty("target")]
        public string Target;
        [JsonProperty("secondaryTarget")]
        public string SecondaryTarget;
        [JsonProperty("completeEvent")]
        public string CompleteEvent;
    }

    [Serializable]
    public sealed class TutorialSequence
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("trigger")]
        public string Trigger;
        [JsonProperty("steps")]
        public List<TutorialStep> Steps = new List<TutorialStep>();
        [JsonProperty("battleHints")]
        public List<string> BattleHints = new List<string>();
    }

    [Serializable]
    public sealed class TutorialCatalog
    {
        [JsonProperty("sequences")]
        public List<TutorialSequence> Sequences = new List<TutorialSequence>();

        public static TutorialCatalog Load()
        {
            return Parse(IntroCatalog.LoadText("Data/tutorial"));
        }

        public static TutorialCatalog Parse(string json)
        {
            TutorialCatalog catalog = JsonConvert.DeserializeObject<TutorialCatalog>(json);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (catalog?.Sequences == null || catalog.Sequences.Count == 0)
                throw new FormatException("Tutorial catalog must contain sequences.");
            foreach (TutorialSequence sequence in catalog.Sequences)
            {
                if (sequence == null || string.IsNullOrWhiteSpace(sequence.Id) || !ids.Add(sequence.Id))
                    throw new FormatException("Tutorial sequence ids must be present and unique.");
                if (string.IsNullOrWhiteSpace(sequence.Trigger) || sequence.Steps == null || sequence.Steps.Count == 0)
                    throw new FormatException("Every tutorial sequence requires a trigger and steps.");
                foreach (TutorialStep step in sequence.Steps)
                    if (step == null || string.IsNullOrWhiteSpace(step.Line)
                        || string.IsNullOrWhiteSpace(step.CompleteEvent))
                        throw new FormatException("Every tutorial step requires a line and completion event.");
            }

            return catalog;
        }
    }

    [Serializable]
    public sealed class EncyclopediaEntry
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("category")]
        public string Category;
        [JsonProperty("description")]
        public string Description;
    }

    [Serializable]
    public sealed class EncyclopediaCatalog
    {
        [JsonProperty("entries")]
        public List<EncyclopediaEntry> Entries = new List<EncyclopediaEntry>();

        public static EncyclopediaCatalog Load()
        {
            return Parse(IntroCatalog.LoadText("Data/encyclopedia"));
        }

        public static EncyclopediaCatalog Parse(string json)
        {
            EncyclopediaCatalog catalog = JsonConvert.DeserializeObject<EncyclopediaCatalog>(json);
            var names = new HashSet<string>(StringComparer.Ordinal);
            if (catalog?.Entries == null || catalog.Entries.Count == 0)
                throw new FormatException("Encyclopedia catalog must contain entries.");
            foreach (EncyclopediaEntry entry in catalog.Entries)
                if (entry == null || string.IsNullOrWhiteSpace(entry.Name) || !names.Add(entry.Name)
                    || string.IsNullOrWhiteSpace(entry.Category) || string.IsNullOrWhiteSpace(entry.Description))
                    throw new FormatException("Encyclopedia entries must be complete and uniquely named.");
            return catalog;
        }
    }
}
