using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.EditorTools.Building;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Pve;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.Tests.Pve
{
    public sealed class PveMapPrefabTests
    {
        [Test]
        public void EveryChapterMapHasExactlyItsStageAnchors()
        {
            PveCatalog catalog = PveCatalog.Load();
            foreach (PveChapter chapter in catalog.Chapters)
            {
                string path = PvePrefabBuilder.MapPrefabFolder + "/Map_" + chapter.Id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.That(prefab, Is.Not.Null, path);
                PveMapLayout layout = prefab.GetComponent<PveMapLayout>();
                Assert.That(layout, Is.Not.Null, path);
                Assert.That(layout.NodesRoot.childCount,
Is.EqualTo(chapter.Stages.Count),
path);
                var actual = new HashSet<string>();
                for (int i = 0; i < layout.NodesRoot.childCount; i++)
                {
                    Assert.That(actual.Add(layout.NodesRoot.GetChild(i).name),
Is.True,
"Duplicate anchor in " +
path);
                }

                foreach (PveStage stage in chapter.Stages)
                {
                    Assert.That(actual.Remove(stage.Id),
Is.True,
"Missing " + stage.Id + " in " + path);
                }

                Assert.That(actual, Is.Empty, "Extra anchors in " + path);
            }
        }

        [Test]
        public void NormalBuilderDoesNotMoveExistingAnchor()
        {
            const string path = PvePrefabBuilder.MapPrefabFolder + "/Map_hoi_1.prefab";
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            RectTransform anchor = contents.GetComponent<PveMapLayout>().FindAnchor("hoi_1_01");
            Vector2 original = anchor.anchoredPosition;
            Vector2 moved = original + new Vector2(17f, -23f);
            anchor.anchoredPosition = moved;
            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
            try
            {
                // This is the same map-prefab operation called by Build(true)/BuildAllOverwrite.
                PvePrefabBuilder.EnsureMapPrefabs();
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.That(prefab.GetComponent<PveMapLayout>().FindAnchor("hoi_1_01").anchoredPosition,
Is.EqualTo(moved));
            }
            finally
            {
                contents = PrefabUtility.LoadPrefabContents(path);
                contents.GetComponent<PveMapLayout>().FindAnchor("hoi_1_01").anchoredPosition = original;
                PrefabUtility.SaveAsPrefabAsset(contents, path);
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }
    }
}