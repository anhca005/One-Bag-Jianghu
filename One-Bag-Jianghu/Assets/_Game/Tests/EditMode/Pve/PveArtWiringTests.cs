using System.IO;
using NUnit.Framework;
using OneBagJianghu.EditorTools.Building;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Pve;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Pve
{
    public sealed class PveArtWiringTests
    {
        private const string TempFolder = "Assets/_Game/Textures2D/Pve/__PveArtTest";
        [Test]
        public void ChapterNodeArtIsUsedAndMissingArtKeepsFallbackFrame()
        {
            Texture2D texture = new Texture2D(2, 2);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * .5f);
            PveArtLibrary library = ScriptableObject.CreateInstance<PveArtLibrary>();
            library.Entries.Add(new PveArtLibrary.Entry { Key = "node_hoi_1", Sprite = sprite });
            GameObject root = new GameObject("Node", typeof(RectTransform));
            Image frame = ChildImage(root.transform, "Frame");
            Button button = frame.gameObject.AddComponent<Button>();
            Text index =
                ChildText(root.transform,
                "Index"),
                stars =
                ChildText(root.transform,
                "Stars"),
                kind =
                ChildText(root.transform,
                "Kind");
            Image locked = ChildImage(root.transform, "Lock");
            PveStageNode node = root.AddComponent<PveStageNode>();
            Set(node, "button", button);
            Set(node, "frame", frame);
            Set(node, "lockIcon", locked);
            Set(node, "indexText", index);
            Set(node, "starsText", stars);
            Set(node, "kindText", kind);
            PveStage stage = new PveStage
            {
                Index = 1,
                Kind = "normal"
            };
            PveTheme theme = new PveTheme
            {
                NodeIcon = "node_hoi_1"
            };
            node.Bind(stage,
                true,
                0,
                Color.red,
                theme,
                library,
                _ =>
            {
            });
            Assert.That(node.UsesArt, Is.True);
            Assert.That(frame.sprite, Is.SameAs(sprite));
            library.Entries.Clear();
            node.Bind(stage,
                true,
                0,
                Color.red,
                theme,
                library,
                _ =>
            {
            });
            Assert.That(node.UsesArt, Is.False);
            Assert.That(frame.sprite, Is.Null);
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(library);
            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(texture);
        }

        [Test]
        public void RebuildRegistersSpriteFromPveSubfolder()
        {
            if (!AssetDatabase.IsValidFolder(TempFolder))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Game/Textures2D/Pve"))
                {
                    AssetDatabase.CreateFolder("Assets/_Game/Textures2D", "Pve");
                }

                AssetDatabase.CreateFolder("Assets/_Game/Textures2D/Pve", "__PveArtTest");
            }

            string path = TempFolder + "/temp_pve_sprite.png";
            Texture2D texture = new Texture2D(2, 2);
            texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            try
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                PvePrefabBuilder.RebuildArtLibrary();
                PveArtLibrary library =
                    AssetDatabase.LoadAssetAtPath<PveArtLibrary>("Assets/_Game/Resources/PveArtLibrary.asset");
                Assert.That(library.TryGet("temp_pve_sprite", out Sprite sprite), Is.True);
                Assert.That(sprite, Is.Not.Null);
            }
            finally
            {
                AssetDatabase.DeleteAsset(TempFolder);
                PvePrefabBuilder.RebuildArtLibrary();
            }
        }

        private static Image ChildImage(Transform parent, string name)
        {
            GameObject child =
                new GameObject(name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            child.transform.SetParent(parent);
            return child.GetComponent<Image>();
        }

        private static Text ChildText(Transform parent, string name)
        {
            GameObject child =
                new GameObject(name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));
            child.transform.SetParent(parent);
            return child.GetComponent<Text>();
        }

        private static void Set(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}