using UnityEngine;

namespace OneBagJianghu.Bag.View
{
    public static class DragGridMath
    {
        // Integer division chooses the cell left/above centre for even-sized items.
        public static int CentreCellOffset(int cells) => Mathf.Max(0, cells - 1) / 2;
        public static int OriginCell(float coordinate,
float pitch,
int cells) => Mathf.FloorToInt(coordinate / pitch) - CentreCellOffset(cells);
    }
}