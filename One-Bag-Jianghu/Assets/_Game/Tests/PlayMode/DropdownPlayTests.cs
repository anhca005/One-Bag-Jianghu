using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Play
{
    public sealed class DropdownPlayTests
    {
        [UnityTest]
        public IEnumerator ShowCreatesFiveVisibleRowsInsideViewport()
        {
            // Arrange
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_Game/Prefabs/UI/Common/Dropdown.prefab");
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Dropdown dropdown = Object.Instantiate(prefab, canvasObject.transform).GetComponent<Dropdown>();
            string[] expected = { "One", "Two", "Three", "Four", "Five" };
            dropdown.AddOptions(expected.ToList());

            // Dropdown creates its fade tween runner in Start(); Show() before that throws.
            yield return null;

            // Act
            dropdown.Show();
            yield return null;

            // Assert
            GameObject list = GameObject.Find("Dropdown List");
            Text[] labels = list.GetComponentsInChildren<Text>(false);
            Text[] optionLabels = labels.Where(label => expected.Contains(label.text)).ToArray();
            Assert.That(optionLabels.Select(label => label.text), Is.EquivalentTo(expected));
            Assert.That(optionLabels.All(label => label.rectTransform.rect.height > 0f), Is.True);
            RectTransform viewport = list.transform.Find("Viewport").GetComponent<RectTransform>();
            Assert.That(optionLabels.All(label => IsInside(label.rectTransform, viewport)),
                Is.True,
                Describe(optionLabels, viewport));
            Object.Destroy(list);
            Object.Destroy(canvasObject);
        }

        private static string Describe(Text[] labels, RectTransform viewport)
        {
            var builder = new System.Text.StringBuilder("viewport " + viewport.rect + "\n");
            foreach (Text label in labels)
            {
                Vector3[] corners = new Vector3[4];
                label.rectTransform.GetWorldCorners(corners);
                builder.Append(label.text)
                    .Append(" min ")
                    .Append(viewport.InverseTransformPoint(corners[0]))
                    .Append(" max ")
                    .Append(viewport.InverseTransformPoint(corners[2]))
                    .Append('\n');
            }

            return builder.ToString();
        }

        private static bool IsInside(RectTransform child, RectTransform viewport)
        {
            Vector3[] corners = new Vector3[4];
            child.GetWorldCorners(corners);
            // Dropdown.Show() sizes the list to fit the rows exactly, so the last row's edge sits on the
            // viewport edge. One unit of slack absorbs float error only; a clipped row is still caught.
            Rect viewportRect = viewport.rect;
            Rect tolerant = new Rect(viewportRect.x - 1f,
                viewportRect.y - 1f,
                viewportRect.width + 2f,
                viewportRect.height + 2f);
            Vector3 minimum = viewport.InverseTransformPoint(corners[0]);
            Vector3 maximum = viewport.InverseTransformPoint(corners[2]);
            return tolerant.Contains(minimum) && tolerant.Contains(maximum);
        }
    }
}
