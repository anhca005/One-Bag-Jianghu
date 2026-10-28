using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    /// <summary>Displays one item in Vạn Bảo Phổ and exposes its available attributes.</summary>
    public sealed class VanBaoPhoItemCard : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Text itemName;
        [SerializeField]
        private Text rarity;
        [SerializeField]
        private Text type;
        [SerializeField]
        private HoverTooltipTarget tooltipTarget;

        public void Bind(ItemInfo item, OwnedItem owned, ItemTooltip tooltip)
        {
            bool isOwned = owned != null;
            group.alpha = isOwned ? 1f : .38f;
            itemName.text = item.Name;
            rarity.text = BagViewStyle.RarityName(isOwned ? owned.Rarity : item.Rarity);
            rarity.color = BagViewStyle.RarityText(isOwned ? owned.Rarity : item.Rarity);
            type.text = TypeName(item.Type);
            ItemIconLibrary icons = ItemIconLibrary.Load();
            icon.sprite = icons != null && icons.TryGet(item.Id, out Sprite sprite) ? sprite : null;
            icon.gameObject.SetActive(icon.sprite != null);
            int tier = isOwned ? owned.Tier : 1;
            string itemRarity = isOwned ? owned.Rarity : item.Rarity;
            tooltipTarget.Initialize(tooltip, () => ItemDescriber.Describe(item.Id, itemRarity, tier));
        }

        private static string TypeName(string value)
        {
            return value switch
            {
                "weapon" => "Binh Khí",
                "armor" => "Hộ Cụ",
                "manual" => "Công Pháp",
                "pill" => "Đan Dược",
                _ => "Kỳ Trân"
            };
        }
    }
}
