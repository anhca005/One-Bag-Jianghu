namespace OneBagJianghu.Bag
{
    public readonly struct GridRect
    {
        public int X
        {
            get;
        }
        public int Y
        {
            get;
        }
        public int W
        {
            get;
        }
        public int H
        {
            get;
        }

        public GridRect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public bool FitsIn(int cols, int rows) => X >= 0 && Y >= 0 && X + W <= cols && Y + H <= rows;
        public bool Overlaps(GridRect other) => X < other.X
+ other.W
&& other.X < X
+ W
&& Y < other.Y
+ other.H
&& other.Y < Y
+ H;
        public bool IsAdjacent(GridRect other)
        {
            bool horizontal = (X + W == other.X || other.X + other.W == X) && Y < other.Y + other.H && other.Y < Y + H;
            bool vertical = (Y + H == other.Y || other.Y + other.H == Y) && X < other.X + other.W && other.X < X + W;
            return horizontal || vertical;
        }
    }
}