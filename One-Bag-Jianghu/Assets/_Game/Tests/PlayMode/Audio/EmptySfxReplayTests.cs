using System.Collections;
using NUnit.Framework;
using OneBagJianghu.Audio;
using OneBagJianghu.Battle.Replay;
using UnityEngine.TestTools;
using UnityEngine;

namespace OneBagJianghu.Tests.Play.Audio
{
    public sealed class EmptySfxReplayTests
    {
        [UnityTest]
        public IEnumerator EmptyLibraryPlayDoesNotThrow()
        {
            // Arrange
            if (AudioManager.Instance != null)
            {
                Object.DestroyImmediate(AudioManager.Instance.gameObject);
            }

            AudioManager manager = AudioManager.EnsureExists();
            // The real library may or may not have clips for these keys; either way playback must not throw.
            var log = new BattleLog { DurationMs = 100 };
            log.Events.Add(new BattleEvent { TimeMs = 100, Type = BattleEventType.Start });
            var resolver = new BattleSfxResolver(log, SfxData.LoadMap(), SfxData.LoadCatalog());
            var playback = new BattleSfxPlayback(new BattleTimeline(log), resolver);

            // Act
            playback.PlayAdvancedWindow(0, log.DurationMs, 1f, manager.Play);
            yield return null;

            // Assert
            Assert.That(AudioManager.Instance, Is.SameAs(manager));
        }
    }
}
