using System;
using System.Collections.Generic;

namespace OneBagJianghu.Engine
{
    public readonly struct Rect
    {
        public readonly int x;
        public readonly int y;
        public readonly int w;
        public readonly int h;
        public Rect(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public int Right()
        {
            return x + w;
        }

        public int Bottom()
        {
            return y + h;
        }

        public bool Overlaps(Rect o)
        {
            return x < o.Right() && o.x < Right() && y < o.Bottom() && o.y < Bottom();
        }

        public bool IsAdjacent(Rect o)
        {
            bool ro = y < o.Bottom() && o.y < Bottom();
            bool co = x < o.Right() && o.x < Right();
            return ((Right() == o.x || o.Right() == x) && ro) || ((Bottom() == o.y || o.Bottom() == y) && co);
        }

        public bool FitsIn(int cols, int rows)
        {
            return x >= 0 && y >= 0 && Right() <= cols && Bottom() <= rows;
        }

        public override string ToString()
        {
            return "Rect[x=" + x + ", y=" + y + ", w=" + w + ", h=" + h + "]";
        }
    }

    public sealed class BagGrid
    {
        public int Cols { get; }
        public int Rows { get; }

        public BagGrid(int cols, int rows)
        {
            if (cols < 1 || rows < 1)
                throw new ArgumentException("Bag must be at least 1x1");
            Cols = cols;
            Rows = rows;
        }

        public List<string> Validate(IList<Rect> rs)
        {
            List<string> e = new List<string>();
            for (int i = 0; i < rs.Count; i++)
            {
                if (!rs[i].FitsIn(Cols, Rows))
                    e.Add("item #" + i + " at " + rs[i] + " is outside the " + Cols + "x" + Rows + " bag");
                for (int j = 0; j < i; j++)
                    if (rs[i].Overlaps(rs[j]))
                        e.Add("item #" + i + " overlaps item #" + j);
            }

            return e;
        }

        public static List<List<int>> Adjacency(IList<Rect> rs)
        {
            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < rs.Count; i++)
            {
                List<int> n = new List<int>();
                for (int j = 0; j < rs.Count; j++)
                    if (i != j && rs[i].IsAdjacent(rs[j]))
                        n.Add(j);
                result.Add(n);
            }

            return result;
        }
    }
}