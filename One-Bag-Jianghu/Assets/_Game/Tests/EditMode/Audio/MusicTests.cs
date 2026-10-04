using NUnit.Framework;
using OneBagJianghu.Audio;
using UnityEngine;

namespace OneBagJianghu.Tests.Audio
{
    public sealed class MusicTests
    {
        [TestCase(1, "bgm_map_hoi_1")]
        [TestCase(2, "bgm_map_hoi_2")]
        [TestCase(3, "bgm_map_hoi_3")]
        [TestCase(4, "bgm_map_hoi_4")]
        [TestCase(5, "bgm_map_hoi_5")]
        [TestCase(0, "bgm_map_hoi_1")]
        [TestCase(6, "bgm_map_hoi_1")]
        public void ForChapter_ReturnsExpectedKey(int chapter, string expected)
        {
            // Act
            string result = MusicKeys.ForChapter(chapter);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestCase(false, "bgm_battle")]
        [TestCase(true, "bgm_boss")]
        public void ForStage_ReturnsExpectedKey(bool bossStage, string expected)
        {
            // Act
            string result = MusicKeys.ForStage(bossStage);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void LibraryUnknownKey_ReturnsFalseWithoutThrowing()
        {
            // Arrange
            MusicLibrary library = ScriptableObject.CreateInstance<MusicLibrary>();

            // Act
            bool found = library.TryGet("missing", out AudioClip clip);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(clip);
            Object.DestroyImmediate(library);
        }
    }
}
