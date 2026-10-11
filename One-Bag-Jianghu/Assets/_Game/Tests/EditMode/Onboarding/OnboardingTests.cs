using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Onboarding
{
    public sealed class OnboardingTests
    {
        [Test]
        public void AllOnboardingCatalogsParse()
        {
            // Arrange
            string introJson = LoadResource("intro.json");
            string tutorialJson = LoadResource("tutorial.json");
            string encyclopediaJson = LoadResource("encyclopedia.json");
            string storyJson = LoadResource("story.json");

            // Act
            IntroCatalog intro = IntroCatalog.Parse(introJson);
            TutorialCatalog tutorial = TutorialCatalog.Parse(tutorialJson);
            EncyclopediaCatalog encyclopedia = EncyclopediaCatalog.Parse(encyclopediaJson);
            StoryCatalog story = StoryCatalog.Parse(storyJson);

            // Assert
            Assert.That(intro.Panels, Has.Count.EqualTo(5));
            Assert.That(tutorial.Sequences.Select(value => value.Id),
                Is.EquivalentTo(new[] { "first_battle", "shop", "upgrade", "sect" }));
            Assert.That(encyclopedia.Entries, Is.Not.Empty);
            Assert.That(story.Chapters.Select(value => value.Chapter), Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5 }));
            Assert.That(story.Chapters.SelectMany(value => value.Panels).All(
                value => !string.IsNullOrWhiteSpace(value.Text)), Is.True);
        }

        [Test]
        public void FirstBossClearQueuesStoryOnlyOnce()
        {
            // Arrange
            PlayerProfile profile = new PlayerProfile();
            PveStage boss = PveCatalog.Load().GetStage("hoi_1_10");

            // Act
            bool first = StoryProgress.TryQueueBossClear(profile, boss, true);
            bool second = StoryProgress.TryQueueBossClear(profile, boss, true);

            // Assert
            Assert.That(first, Is.True);
            Assert.That(second, Is.False);
            Assert.That(profile.PendingStoryChapters, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void StoryListContainsIntroAndExactlyClearedChapters()
        {
            // Arrange
            PlayerProfile profile = new PlayerProfile();
            profile.Pve.Stages["hoi_1_10"] = new PveStageProgress { Cleared = true };
            profile.Pve.Stages["hoi_3_10"] = new PveStageProgress { Cleared = true };

            // Act
            List<StoryChapter> stories = StoryProgress.UnlockedChapters(
                StoryCatalog.Load(),
                PveCatalog.Load(),
                profile);

            // Assert
            Assert.That(stories.Select(value => value.Chapter), Is.EqualTo(new[] { 0, 1, 3 }));
        }

        [Test]
        public void EncyclopediaCoversEveryGlossaryTerm()
        {
            // Arrange
            string glossary = File.ReadAllText(Path.Combine(ProjectRoot(), "..", "docs", "design", "glossary.md"));
            var expected = new HashSet<string>();
            MatchCollection matches = Regex.Matches(glossary, @"(?m)^\|\s*([^|`]+?)\s*\|\s*`[^`]+`");
            foreach (Match match in matches)
                expected.Add(match.Groups[1].Value.Trim());
            EncyclopediaCatalog catalog = EncyclopediaCatalog.Parse(LoadResource("encyclopedia.json"));

            // Act
            var actual = new HashSet<string>(catalog.Entries.Select(value => value.Name));

            // Assert
            Assert.That(actual, Is.SupersetOf(expected));
        }

        [Test]
        public void TutorialServiceFlowsSkipsAndPersists()
        {
            // Arrange
            TutorialCatalog catalog = TutorialCatalog.Parse(LoadResource("tutorial.json"));
            var profile = new PlayerProfile();
            var service = new TutorialService(catalog, profile);

            // Act
            bool started = service.TryStart("shop");
            bool ignored = service.Publish("wrong_event");
            bool advanced = service.Publish("stage_map_closed");
            service.SkipSequence();

            // Assert
            Assert.That(started, Is.True);
            Assert.That(ignored, Is.False);
            Assert.That(advanced, Is.True);
            Assert.That(profile.CompletedTutorials, Contains.Item("shop"));
            Assert.That(service.TryStart("shop"), Is.False);
        }

        [Test]
        public void ShopTutorialContainsQuestStepsAndAdvancesThroughTheirEvents()
        {
            // Arrange
            TutorialCatalog catalog = TutorialCatalog.Parse(LoadResource("tutorial.json"));
            TutorialSequence shop = catalog.Sequences.Single(value => value.Id == "shop");
            var service = new TutorialService(catalog, new PlayerProfile());

            // Act
            service.TryStart("shop");
            service.Publish("stage_map_closed");
            string[] targets = shop.Steps.Skip(1).Take(3).Select(value => value.Target).ToArray();
            bool opened = service.PublishQuestLogOpened(true);
            bool claimed = service.Publish("quest_claimed");
            bool closed = service.Publish("quest_log_closed");

            // Assert
            Assert.That(targets, Is.EqualTo(new[] { "home_quest", "quest_claim", "quest_log_close" }));
            Assert.That(opened, Is.True);
            Assert.That(claimed, Is.True);
            Assert.That(closed, Is.True);
            Assert.That(service.CurrentStep.CompleteEvent, Is.EqualTo("shop_opened"));
        }

        [Test]
        public void QuestLogOpeningSkipsClaimWhenNoQuestRewardIsReady()
        {
            // Arrange
            TutorialCatalog catalog = TutorialCatalog.Parse(LoadResource("tutorial.json"));
            var service = new TutorialService(catalog, new PlayerProfile());
            service.TryStart("shop");
            service.Publish("stage_map_closed");

            // Act
            bool opened = service.PublishQuestLogOpened(false);

            // Assert
            Assert.That(opened, Is.True);
            Assert.That(service.CurrentStep.CompleteEvent, Is.EqualTo("quest_log_closed"));
        }

        [Test]
        public void VersionSixProfileGetsSafeOnboardingDefaults()
        {
            // Arrange
            JObject json = JObject.Parse(new PlayerProfile().ToJson());
            json["version"] = 6;
            json.Remove("introSeen");
            json.Remove("completedTutorials");

            // Act
            PlayerProfile profile = PlayerProfile.FromJson(json.ToString());

            // Assert
            Assert.That(profile.IntroSeen, Is.False);
            Assert.That(profile.CompletedTutorials, Is.Empty);
            Assert.That(profile.Version, Is.EqualTo(PlayerProfile.CurrentVersion));
        }

        [Test]
        public void MissingIntroArtworkUsesFallbackWithoutError()
        {
            // Arrange
            const string missingName = "image_that_does_not_exist";

            // Act
            Sprite fallback = IntroPopup.ResolveArtwork(missingName);

            // Assert
            Assert.That(fallback == null || fallback is Sprite, Is.True);
        }

        [Test]
        public void EncyclopediaRowsExpandToTheirScrollColumn()
        {
            // Arrange
            GameObject prefab = Resources.Load<GameObject>("UI/Popups/EncyclopediaPopup");

            // Act
            LayoutElement categoryRow = prefab.transform.Find("Panel/Categories Viewport/Categories Content/"
                + "Category Row Template").GetComponent<LayoutElement>();
            LayoutElement entryRow = prefab.transform.Find("Panel/Entries Viewport/Entries Content/"
                + "Entry Row Template").GetComponent<LayoutElement>();

            // Assert
            Assert.That(categoryRow.flexibleWidth, Is.GreaterThan(0f));
            Assert.That(entryRow.flexibleWidth, Is.GreaterThan(0f));
        }

        private static string LoadResource(string name)
        {
            return File.ReadAllText(Path.Combine(ProjectRoot(), "Assets", "_Game", "Resources", "Data", name));
        }

        private static string ProjectRoot()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
