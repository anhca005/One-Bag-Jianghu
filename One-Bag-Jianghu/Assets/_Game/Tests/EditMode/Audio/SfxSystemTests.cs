using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Audio;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.EditorTools.Audio;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OneBagJianghu.Tests.Audio
{
    public sealed class SfxSystemTests
    {
        private static readonly string[] UiKeys =
        {
            "ui_click", "ui_open", "ui_close", "ui_error", "item_pick", "item_drop",
            "item_drop_bad", "coin", "upgrade", "merge", "level_up", "reward",
            "stage_select", "dialogue_next", "node_unlock"
        };
        private static readonly string[] ResolverKeys =
        {
            "ultimate", "hit_crit", "shield_hit", "burn_apply", "burn_tick",
            "poison_apply", "poison_tick", "bleed_apply", "bleed_tick", "stun",
            "battle_start", "boss_intro", "win", "lose"
        };

        [Test]
        public void CatalogContainsAllMapAndUiKeys()
        {
            // Arrange
            var catalog = new HashSet<string>(SfxData.LoadCatalog().Select(value => value.Key));
            TextAsset mapAsset = Resources.Load<TextAsset>("Data/Audio/sfx-map");
            IEnumerable<string> mapped = JObject.Parse(mapAsset.text)
                .Descendants()
                .OfType<JProperty>()
                .Where(property => property.Name.EndsWith("Key") && property.Value.Type == JTokenType.String)
                .Select(property => property.Value.Value<string>());

            // Act
            List<string> missing = mapped.Concat(UiKeys)
                .Concat(ResolverKeys)
                .Where(key => !catalog.Contains(key))
                .Distinct()
                .ToList();

            // Assert
            Assert.That(missing, Is.Empty);
        }

        [Test]
        public void ResolverMapsBasicUltimateDamageStatusAndEnd()
        {
            // Arrange
            BattleLog log = BuildLog();
            BattleSfxResolver resolver = BuildResolver(log);

            // Act
            IReadOnlyList<string> basic = resolver.Resolve(Event(BattleEventType.Trigger, 10));
            IReadOnlyList<string> signature = resolver.Resolve(Event(BattleEventType.Trigger, 10, "ULTIMATE"));
            IReadOnlyList<string> generic = resolver.Resolve(Event(BattleEventType.Trigger, 11, "ULTIMATE"));

            // Assert
            Assert.That(basic, Is.EqualTo(new[] { "atk_sword" }));
            Assert.That(signature, Is.EqualTo(new[] { "ult_sword_wind" }));
            Assert.That(generic, Is.EqualTo(new[] { "ultimate", "use_pill" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.Damage, 10, crit: true)),
                Is.EqualTo(new[] { "hit_crit" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.Damage, 10, absorbed: 5)),
                Is.EqualTo(new[] { "shield_hit" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.Status, 10, "BURN")),
                Is.EqualTo(new[] { "burn_apply" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.Burst, 10, "POISON")),
                Is.EqualTo(new[] { "poison_tick" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.End, side: 0)), Is.EqualTo(new[] { "win" }));
            Assert.That(resolver.Resolve(Event(BattleEventType.End, side: 1)), Is.EqualTo(new[] { "lose" }));
        }

        [Test]
        public void PlaybackIgnoresSeekAndFiltersHighSpeed()
        {
            // Arrange
            BattleLog log = BuildLog();
            log.DurationMs = 1000;
            log.Events.Add(Event(BattleEventType.Trigger, 10, time: 100));
            log.Events.Add(Event(BattleEventType.Damage, 10, time: 200));
            log.Events.Add(Event(BattleEventType.Damage, 10, crit: true, time: 300));
            log.Events.Add(Event(BattleEventType.End, side: 0, time: 400));
            var playback = new BattleSfxPlayback(new BattleTimeline(log), BuildResolver(log));
            var played = new List<string>();

            // Act
            playback.PlayAdvancedWindow(400, 100, 1f, played.Add);
            playback.PlayAdvancedWindow(0, 400, 4f, played.Add);

            // Assert
            Assert.That(played, Is.EqualTo(new[] { "atk_sword", "hit_crit", "win" }));
        }

        [Test]
        public void BuilderGroupsNumberedVariants()
        {
            // Arrange
            string[] names = { "folder/atk_sword_2.ogg", "folder/ui_click.wav", "folder/atk_sword_1.mp3" };

            // Act
            Dictionary<string, List<string>> groups = SfxLibraryBuilder.GroupFileNames(names);

            // Assert
            Assert.That(groups["atk_sword"], Has.Count.EqualTo(2));
            Assert.That(groups["ui_click"], Has.Count.EqualTo(1));
        }

        private static BattleLog BuildLog()
        {
            var log = new BattleLog();
            log.Items.Add(new BattleItemRef { Uid = 10, Id = "thanh_phong_kiem" });
            log.Items.Add(new BattleItemRef { Uid = 11, Id = "boi_nguyen_dan" });
            return log;
        }

        private static BattleSfxResolver BuildResolver(BattleLog log)
        {
            return new BattleSfxResolver(log, SfxData.LoadMap(), SfxData.LoadCatalog());
        }

        private static BattleEvent Event(BattleEventType type,
            int? item = null,
            string kind = null,
            bool crit = false,
            int absorbed = 0,
            int side = 0,
            int time = 0)
        {
            return new BattleEvent
            {
                Type = type,
                Item = item,
                Kind = kind,
                Crit = crit,
                Absorbed = absorbed,
                Value = 0,
                Side = side,
                TimeMs = time
            };
        }
    }
}
