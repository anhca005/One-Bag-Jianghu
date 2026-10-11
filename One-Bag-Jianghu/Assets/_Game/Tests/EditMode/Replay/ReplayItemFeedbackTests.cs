using NUnit.Framework;
using OneBagJianghu.Battle.Replay.View;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.Tests.Replay
{
    public sealed class ReplayItemFeedbackTests
    {
        private const string PrefabPath = "Assets/_Game/Prefabs/UI/Replay/ReplayItemView.prefab";

        [Test]
        public void Flash_AffectsOnlyFiringItem()
        {
            // Arrange
            ReplayItemView first = CreateView(1);
            ReplayItemView second = CreateView(2);

            // Act
            first.Flash(false, 1000);
            first.Refresh(1075);
            second.Refresh(1075);

            // Assert
            Assert.That(first.IsFlashing, Is.True);
            Assert.That(first.transform.localScale.x, Is.GreaterThan(1f));
            Assert.That(second.IsFlashing, Is.False);
            Assert.That(second.transform.localScale, Is.EqualTo(Vector3.one));
            Object.DestroyImmediate(first.gameObject);
            Object.DestroyImmediate(second.gameObject);
        }

        [Test]
        public void Seal_SetsVisualAndClearsAtEndTime()
        {
            // Arrange
            ReplayItemView view = CreateView(1);

            // Act
            view.SetStatus("SEAL", 2000);
            view.Refresh(1999);

            // Assert
            Assert.That(view.IsSealed, Is.True);
            view.Refresh(2000);
            Assert.That(view.IsSealed, Is.False);
            Object.DestroyImmediate(view.gameObject);
        }

        private static ReplayItemView CreateView(int uid)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = Object.Instantiate(prefab);
            ReplayItemView view = instance.GetComponent<ReplayItemView>();
            view.Initialize(uid, "thanh_phong_kiem", "Kiếm", 1, "common");
            return view;
        }
    }
}
