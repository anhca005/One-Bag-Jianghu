using NUnit.Framework;
using OneBagJianghu.Bag.View;

namespace OneBagJianghu.Tests.Edit.Bag
{
    public sealed class DragGridMathTests
    {
        [TestCase(1, 1, 0, 0)]
        [TestCase(1, 2, 0, 0)]
        [TestCase(2, 2, 0, 0)]
        [TestCase(1, 3, 0, 1)]
        public void CentreCellUsesLeftAndAboveForEvenSizes(int width, int height, int x, int y)
        {
            Assert.AreEqual(x, DragGridMath.CentreCellOffset(width));
            Assert.AreEqual(y, DragGridMath.CentreCellOffset(height));
        }

        [TestCase(1, 1, 4, 6)]
        [TestCase(1, 2, 4, 6)]
        [TestCase(2, 2, 4, 6)]
        [TestCase(1, 3, 4, 5)]
        public void PointerCellMapsToItemOrigin(int width, int height, int expectedX, int expectedY)
        {
            Assert.AreEqual(expectedX, DragGridMath.OriginCell(4.5f * 80f, 80f, width));
            Assert.AreEqual(expectedY, DragGridMath.OriginCell(6.5f * 80f, 80f, height));
        }
    }
}