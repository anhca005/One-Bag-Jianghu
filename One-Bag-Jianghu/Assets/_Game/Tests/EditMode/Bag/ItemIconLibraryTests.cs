using NUnit.Framework;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class ItemIconLibraryTests
    {
        [Test]
        public void TryGetReturnsMatchingSpriteAndFalseForUnknownId()
        {
            ItemIconLibrary library = ScriptableObject.CreateInstance<ItemIconLibrary>();
            var texture = new Texture2D(2, 2);
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), Vector2.zero);
            library.Entries.Add(new ItemIconLibrary.Entry
            {
                ItemId = "test_item",
                Sprite = sprite
            });
            Assert.That(library.TryGet("test_item", out Sprite found), Is.True);
            Assert.That(found, Is.SameAs(sprite));
            Assert.That(library.TryGet("unknown", out Sprite missing), Is.False);
            Assert.That(missing, Is.Null);
            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(library);
        }

        [Test]
        public void ImportedLibraryContainsEveryReplayStatusIcon()
        {
            ItemIconLibrary library = ItemIconLibrary.Load();
            Assert.That(library, Is.Not.Null);
            string[] codes =
            {
                "burn",
                "poison",
                "bleed",
                "internal_injury",
                "slow",
                "seal",
                "stun",
                "haste",
                "charge",
                "regen",
                "heal"
            };
            foreach (string code in codes)
            {
                Assert.That(library.TryGet("status_" + code,
                    out Sprite sprite),
                    Is.True,
                    code);
                Assert.That(sprite, Is.Not.Null, code);
            }
        }
    }
}