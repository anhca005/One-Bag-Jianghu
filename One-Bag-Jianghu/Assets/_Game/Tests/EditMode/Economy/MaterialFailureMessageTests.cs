using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Tests.Edit.Economy
{
    public sealed class MaterialFailureMessageTests
    {
        [Test]
        public void MissingMaterialUsesVietnameseCatalogNameWithoutId()
        {
            // Arrange
            MaterialCatalog materials = MaterialCatalog.Parse(
                "[{\"id\":\"han_thiet\",\"name\":\"Hàn Thiết\",\"itemType\":\"weapon\"," +
                "\"band\":\"tier1\",\"description\":\"\"}]");
            var profile = new PlayerProfile();
            var storage = new StorageGrid(profile, null, materials);
            var costs = new Dictionary<string, int>
            {
                ["han_thiet"] = 1
            };

            // Act
            bool success = storage.SpendMaterials(costs, out string error);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(error, Does.Contain("Hàn Thiết"));
            Assert.That(error, Does.Not.Contain("_"));
        }
    }
}
