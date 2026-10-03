using System.IO;
using NUnit.Framework;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Tests.Bag
{
    public sealed class SectCatalogTests
    {
        [Test]
        public void ParsesRealSectCatalog()
        {
            string path = Path.Combine(Application.dataPath, "_Game/Resources/Data/sects.json");
            SectCatalog catalog = SectCatalog.Parse(File.ReadAllText(path));
            Assert.That(catalog.All.Count, Is.GreaterThan(0));
            Assert.That(catalog.TryGet("thai_thanh", out SectInfo sect), Is.True);
            Assert.That(sect.TamPhap, Is.Not.Empty);
            Assert.That(sect.Buffs("sword"), Is.True);
        }
    }
}