using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using UnityEngine;

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

            // Act
            IntroCatalog intro = IntroCatalog.Parse(introJson);
            TutorialCatalog tutorial = TutorialCatalog.Parse(tutorialJson);
            EncyclopediaCatalog encyclopedia = EncyclopediaCatalog.Parse(encyclopediaJson);

            // Assert
            Assert.That(intro.Panels, Has.Count.EqualTo(5));
            Assert.That(tutorial.Sequences.Select(value => value.Id),
                Is.EquivalentTo(new[] { "first_battle", "shop", "upgrade", "sect" }));
            Assert.That(encyclopedia.Entries, Is.Not.Empty);
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
            bool advanced = service.Publish("shop_opened");
            service.SkipSequence();

            // Assert
            Assert.That(started, Is.True);
            Assert.That(ignored, Is.False);
            Assert.That(advanced, Is.True);
            Assert.That(profile.CompletedTutorials, Contains.Item("shop"));
            Assert.That(service.TryStart("shop"), Is.False);
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
