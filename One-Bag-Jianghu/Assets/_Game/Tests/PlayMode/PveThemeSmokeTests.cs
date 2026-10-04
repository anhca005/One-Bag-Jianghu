using System.Collections;
using NUnit.Framework;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Pve;
using OneBagJianghu.Pve;
using UnityEngine;
using UnityEngine.TestTools;

namespace OneBagJianghu.Tests.Play
{
    public sealed class PveThemeSmokeTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator StageMapOpensWithoutPveArtErrors()
        {
            DeleteProfile();
            yield return Login("Pve No Art");
            StageMapPopup popup = PopupManager.Show<StageMapPopup>(0);
            yield return null;
            // Tagged Debug.Log lines are expected (decision #29); errors still fail the test.
            Assert.That(popup, Is.Not.Null);
            popup.Hide();
        }

        [UnityTest]
        public IEnumerator ChapterThemeChangesHeaderColor()
        {
            DeleteProfile();
            yield return Login("Pve Theme");
            StageMapPopup first = PopupManager.Show<StageMapPopup>(0);
            yield return null;
            var color1 = first.HeaderColor;
            first.Hide();
            GameBootstrap.Session.Profile.Pve.Stages["hoi_1_03"] = new PveStageProgress
            {
                Cleared = true,
                BestStars = 3
            };
            StageMapPopup second = PopupManager.Show<StageMapPopup>(1);
            yield return null;
            var color2 = second.HeaderColor;
            Assert.AreNotEqual(color1, color2);
            second.Hide();
        }

        [UnityTest]
        public IEnumerator StageNodesUseHoiOnePrefabAnchors()
        {
            DeleteProfile();
            yield return Login("Pve Map Anchors");
            StageMapPopup popup = PopupManager.Show<StageMapPopup>(0);
            yield return null;
            Assert.That(popup.CurrentMapLayout, Is.Not.Null);
            PveChapter chapter = PveCatalog.Load().Chapters[0];
            foreach (PveStage stage in chapter.Stages)
            {
                RectTransform anchor = popup.CurrentMapLayout.FindAnchor(stage.Id);
                Assert.That(anchor, Is.Not.Null, stage.Id);
                GameObject node = null;
                foreach (GameObject candidate in popup.SpawnedNodes)
                {
                    if (candidate.transform.parent == anchor)
                    {
                        node = candidate;
                        break;
                    }
                }

                Assert.That(node, Is.Not.Null, stage.Id);
                Assert.That(Vector3.Distance(node.transform.position,
                    anchor.position),
                    Is.LessThanOrEqualTo(1f),
                    stage.Id);
            }

            popup.Hide();
        }
    }
}