using NUnit.Framework;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace OneBagJianghu.Tests.UI
{
    public sealed class SingleLineFitTests
    {
        [Test]
        public void LongTextInNarrowRectShrinksAndStaysSingleLine()
        {
            GameObject go = MakeLabel("Đổi nguyên liệu cực kỳ dài", 90f, 30);
            try
            {
                Text text = go.GetComponent<Text>();
                SingleLineFit fit = go.AddComponent<SingleLineFit>();
                fit.Configure(30, 12);
                Assert.That(text.fontSize, Is.LessThan(30));
                Assert.That(text.horizontalOverflow,
                    Is.EqualTo(HorizontalWrapMode.Overflow));
                Assert.That(text.resizeTextForBestFit, Is.False);
                Assert.That(text.cachedTextGenerator.lineCount, Is.LessThanOrEqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void ShortTextKeepsAuthoredSize()
        {
            GameObject go = MakeLabel("Thoát", 500f, 30);
            try
            {
                Text text = go.GetComponent<Text>();
                SingleLineFit fit = go.AddComponent<SingleLineFit>();
                fit.Configure(30, 12);
                Assert.That(text.fontSize, Is.EqualTo(30));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void ZeroWidthDoesNotChangeSize()
        {
            GameObject go = MakeLabel("Zero width", 0f, 27);
            try
            {
                SingleLineFit fit = go.AddComponent<SingleLineFit>();
                fit.Configure(30, 12);
                Assert.That(go.GetComponent<Text>().fontSize, Is.EqualTo(27));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void RefitAfterWidthChangeGrowsBackToAuthoredSize()
        {
            GameObject go = MakeLabel("A moderately long label", 80f, 30);
            try
            {
                Text text = go.GetComponent<Text>();
                SingleLineFit fit = go.AddComponent<SingleLineFit>();
                fit.Configure(30, 12);
                Assert.That(text.fontSize, Is.LessThan(30));
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 60f);
                fit.Fit();
                Assert.That(text.fontSize, Is.EqualTo(30));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void FittingTwiceIsIdempotent()
        {
            GameObject go = MakeLabel("A moderately long label", 100f, 30);
            try
            {
                Text text = go.GetComponent<Text>();
                SingleLineFit fit = go.AddComponent<SingleLineFit>();
                fit.Configure(30, 12);
                int first = text.fontSize;
                fit.Fit();
                Assert.That(text.fontSize, Is.EqualTo(first));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        private static GameObject MakeLabel(string value, float width, int size)
        {
            var go =
                new GameObject("Label",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 60f);
            Text text = go.GetComponent<Text>();
            text.font =
                AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Fonts/Alegreya-Regular.ttf");
            text.text = value;
            text.fontSize = size;
            return go;
        }
    }
}