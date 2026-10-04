using NUnit.Framework;
using OneBagJianghu.UI.Common;

namespace OneBagJianghu.Tests.Edit.UI
{
    public sealed class UiFormatTests
    {
        [TestCase(0L, "0")]
        [TestCase(999L, "999")]
        [TestCase(1200L, "1.200")]
        [TestCase(1234567L, "1.234.567")]
        [TestCase(-1200L, "-1.200")]
        public void NumberUsesVietnameseThousandsSeparator(long value, string expected)
        {
            // Act
            string result = UiFormat.Number(value);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
