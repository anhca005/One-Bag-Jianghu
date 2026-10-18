using NUnit.Framework;
using OneBagJianghu.UI.Arena;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ArenaRanksTests
    {
        private const string Json = "{\"ranks\":["
            + "{\"minimumRating\":0,\"name\":\"Vô Danh\",\"imageKey\":\"vo_danh\"},"
            + "{\"minimumRating\":1100,\"name\":\"Tân Tú\",\"imageKey\":\"tan_tu\"},"
            + "{\"minimumRating\":1250,\"name\":\"Thiếu Hiệp\",\"imageKey\":\"thieu_hiep\"},"
            + "{\"minimumRating\":1400,\"name\":\"Đại Hiệp\",\"imageKey\":\"dai_hiep\"},"
            + "{\"minimumRating\":1600,\"name\":\"Danh Túc\",\"imageKey\":\"danh_tuc\"},"
            + "{\"minimumRating\":1800,\"name\":\"Tông Sư\",\"imageKey\":\"tong_su\"}]}";

        [TestCase(1000, "Vô Danh")]
        [TestCase(1099, "Vô Danh")]
        [TestCase(1100, "Tân Tú")]
        [TestCase(1249, "Tân Tú")]
        [TestCase(1250, "Thiếu Hiệp")]
        [TestCase(1400, "Đại Hiệp")]
        [TestCase(1600, "Danh Túc")]
        [TestCase(1800, "Tông Sư")]
        public void ForRating_UsesConfiguredThresholds(int rating, string expected)
        {
            // Arrange
            ArenaRanks ranks = ArenaRanks.Parse(Json);

            // Act
            string actual = ranks.ForRating(rating).Name;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
