using NUnit.Framework;
using OneBagJianghu.Battle.Replay.View.VfxLab;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.Tests.VfxLab
{
    public sealed class VfxCatalogRowTests
    {
        [Test]
        public void CatalogRowIsAReusablePrefab()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/UI/Tools/VfxCatalogRow.prefab");
            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.GetComponent<VfxCatalogRow>(), Is.Not.Null);
        }
    }
}