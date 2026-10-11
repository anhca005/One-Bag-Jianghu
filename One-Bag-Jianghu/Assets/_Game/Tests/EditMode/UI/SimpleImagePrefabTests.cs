using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class SimpleImagePrefabTests
    {
        [Test]
        public void UiPrefabsDoNotSliceOrDistortUiSprites()
        {
            // Arrange
            string[] prefabGuids = AssetDatabase.FindAssets(
                "t:Prefab",
                new[] { "Assets/_Game/Prefabs/UI", "Assets/_Game/Resources/UI" });
            var failures = new List<string>();

            // Act
            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                InspectPrefab(prefabPath, prefab, failures);
            }

            // Assert
            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        private static void InspectPrefab(string prefabPath, GameObject prefab, List<string> failures)
        {
            foreach (Image image in prefab.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite == null)
                {
                    continue;
                }

                string spritePath = AssetDatabase.GetAssetPath(image.sprite);
                if (!spritePath.Contains("/Textures2D/UI/"))
                {
                    continue;
                }

                string label = prefabPath + " :: " + GetPath(image.transform, prefab.transform);
                if (image.type == Image.Type.Sliced || image.type == Image.Type.Tiled)
                {
                    failures.Add(label + " uses " + image.type);
                    continue;
                }

                if (image.type != Image.Type.Simple || image.preserveAspect)
                {
                    continue;
                }

                Vector2 rectSize = image.rectTransform.rect.size;
                if (rectSize.x <= 0f || rectSize.y <= 0f)
                {
                    continue;
                }

                float rectAspect = rectSize.x / rectSize.y;
                float spriteAspect = image.sprite.rect.width / image.sprite.rect.height;
                float error = Mathf.Abs(rectAspect - spriteAspect) / ((rectAspect + spriteAspect) * .5f);
                if (error > .05f)
                {
                    failures.Add(label + " aspect error " + (error * 100f).ToString("0.00") + "%");
                }
            }
        }

        private static string GetPath(Transform transform, Transform root)
        {
            string path = transform.name;
            while (transform != root && transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
    }
}
