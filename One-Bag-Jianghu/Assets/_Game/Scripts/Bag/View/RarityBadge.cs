using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Bag.View
{
    public static class RarityBadge
    {
        /// <summary>Returns the shared art-library key for a supported rarity.</summary>
        public static string SpriteName(string rarity)
        {
            return rarity switch
            {
                "common" => "rarity_common",
                "uncommon" => "rarity_uncommon",
                "rare" => "rarity_rare",
                "epic" => "rarity_epic",
                "legendary" => "rarity_legendary",
                "supreme" => "rarity_supreme",
                _ => null
            };
        }

        /// <summary>Finds rarity badge art without logging when art is unavailable.</summary>
        public static Sprite FindSprite(string rarity)
        {
            string spriteName = SpriteName(rarity);
            ItemIconLibrary library = ItemIconLibrary.Load();
            if (spriteName != null && library != null && library.TryGet(spriteName, out Sprite sprite))
                return sprite;
            return null;
        }

        /// <summary>Updates an optional badge image for a rarity.</summary>
        public static void Apply(Image image, string rarity)
        {
            if (image == null)
                return;
            Sprite sprite = FindSprite(rarity);
            image.sprite = sprite;
            image.gameObject.SetActive(sprite != null);
        }
    }
}
