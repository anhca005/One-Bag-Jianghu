namespace OneBagJianghu.Bag
{
    public sealed class ItemInfo
    {
        public string Id { get; }
        public string Name { get; }
        public string Type { get; }
        public string WeaponClass { get; }
        public string Sect { get; }
        public string Rarity { get; }
        public int Width { get; }
        public int Height { get; }

        public ItemInfo(string id, string name, string type, string weaponClass, string sect,
            string rarity, int width, int height)
        {
            Id = id;
            Name = name;
            Type = type;
            WeaponClass = weaponClass;
            Sect = sect;
            Rarity = rarity;
            Width = width;
            Height = height;
        }
    }
}
