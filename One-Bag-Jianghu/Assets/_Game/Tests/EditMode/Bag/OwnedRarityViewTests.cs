using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class OwnedRarityViewTests
    {
        [Test]
        public void PlacedLegendaryItemKeepsOwnedColourAfterAdjacency()
        {
            GameObject root =
                new GameObject("item",
                typeof(RectTransform),
                typeof(Image),
                typeof(Outline),
                typeof(CanvasGroup));
            ItemView view = root.AddComponent<ItemView>();
            Set(view, "background", root.GetComponent<Image>());
            Set(view, "outline", root.GetComponent<Outline>());
            Set(view, "canvasGroup", root.GetComponent<CanvasGroup>());
            Set(view, "icon", ChildImage(root));
            Set(view, "nameText", ChildText(root));
            Set(view, "tamPhapText", ChildText(root));
            var info = new ItemInfo("test", "Test", "relic", null, null, "common", 1, 1);
            PlacedItem placed = new BagGrid(5, 4).Place(info, 0, 0, 1, "owned");
            view.InitializePlaced(null, null, placed, "legendary");
            Assert.That(root.GetComponent<Outline>().effectColor,
                Is.EqualTo(BagViewStyle.Rarity("legendary")));
            view.SetAdjacent(true);
            view.SetAdjacent(false);
            Assert.That(root.GetComponent<Outline>().effectColor,
                Is.EqualTo(BagViewStyle.Rarity("legendary")));
            Object.DestroyImmediate(root);
        }

        private static Image ChildImage(GameObject parent)
        {
            var go = new GameObject("image", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent.transform);
            return go.GetComponent<Image>();
        }

        private static Text ChildText(GameObject parent)
        {
            var go = new GameObject("text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent.transform);
            return go.GetComponent<Text>();
        }

        private static void Set(object target,
            string name,
            object value) =>
            target.GetType().GetField(name,
            BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target,
            value);
    }
}