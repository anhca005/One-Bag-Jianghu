namespace OneBagJianghu.Bag
{
    public sealed class ItemInfo
    {
        public string Id
        {
            get;
        }
        public string Name
        {
            get;
        }
        public string Type
        {
            get;
        }
        public string WeaponClass
        {
            get;
        }
        public string Sect
        {
            get;
        }
        public string Rarity
        {
            get;
        }
        public int Width
        {
            get;
        }
        public int Height
        {
            get;
        }
        public float CooldownSeconds
        {
            get;
        }
        public bool HasActive
        {
            get;
        }

        public ItemInfo(string id,
string name,
string type,
string weaponClass,
string sect,
string rarity,
int width,
int height,
float cooldownSeconds = 0f,
bool hasActive = false)
        {
            Id = id;
            Name = name;
            Type = type;
            WeaponClass = weaponClass;
            Sect = sect;
            Rarity = rarity;
            Width = width;
            Height = height;
            CooldownSeconds = cooldownSeconds;
            HasActive = hasActive;
        }
    }
}