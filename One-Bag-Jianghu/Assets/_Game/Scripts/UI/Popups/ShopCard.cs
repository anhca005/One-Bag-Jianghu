using System;
using OneBagJianghu.Bag.View;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class ShopCard : MonoBehaviour
    {
        [SerializeField]
        private Image paperPanel;
        [SerializeField]
        private Image rarityBorder;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image rarityBadge;
        [SerializeField]
        private Text placeholderText;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text detailText;
        [SerializeField]
        private Text statText;
        [SerializeField]
        private Text typeText;
        [SerializeField]
        private Text priceText;
        [SerializeField]
        private Image currencyIcon;
        [SerializeField]
        private Text oldPriceText;
        [SerializeField]
        private Text discountText;
        [SerializeField]
        private UiButton buyButton;
        [SerializeField]
        private Text disabledReasonText;
        [SerializeField]
        private Image boughtSeal;
        [SerializeField]
        private Text boughtSealText;
        public UiButton BuyButton => buyButton;

        public void SetTooltip(ItemTooltip tooltip, Func<string> textProvider)
        {
            HoverTooltipTarget target = GetComponent<HoverTooltipTarget>();
            if (target == null)
                target = gameObject.AddComponent<HoverTooltipTarget>();
            target.Initialize(tooltip, textProvider);
        }

        public void Setup(string displayName,
            string detail,
            string stat,
            string type,
            string price,
            string oldPrice,
            string discount,
            string rarity,
            Sprite icon,
            bool affordable,
            bool bought,
            Action buy,
            string disabledReason = null)
        {
            nameText.text = displayName;
            detailText.text = detail;
            statText.text = stat;
            typeText.text = type;
            priceText.text = price;
            if (currencyIcon != null)
            {
                CurrencyArt.TryGetSprite(price.Contains("Danh Vọng") ? "icon_danh_vong" : "icon_bac",
                    out Sprite currencySprite);
                currencyIcon.sprite = currencySprite;
                currencyIcon.gameObject.SetActive(currencySprite != null);
            }
            bool hasOldPrice = !string.IsNullOrEmpty(oldPrice);
            oldPriceText.gameObject.SetActive(hasOldPrice);
            oldPriceText.text = hasOldPrice ? Strike(oldPrice) : string.Empty;
            discountText.gameObject.SetActive(!string.IsNullOrEmpty(discount));
            discountText.text = discount ?? string.Empty;
            bool hasIcon = icon != null;
            iconImage.gameObject.SetActive(hasIcon);
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            placeholderText.gameObject.SetActive(!hasIcon);
            placeholderText.text = hasIcon ? string.Empty : "◆";
            RarityBadge.Apply(rarityBadge, rarity);
            rarityBorder.color =
                string.IsNullOrEmpty(rarity)
                ? BagViewStyle.Rarity("common")
                : BagViewStyle.Rarity(rarity);
            paperPanel.color = Color.white;
            boughtSeal.gameObject.SetActive(bought);
            boughtSealText.gameObject.SetActive(bought);
            buyButton.gameObject.SetActive(!bought);
            buyButton.GetComponent<Button>().interactable = affordable && !bought;
            disabledReasonText.gameObject.SetActive(!bought && !affordable);
            disabledReasonText.text = !bought
                && !affordable ? "Không đủ "
                + (price.Contains("Danh Vọng") ? "Danh Vọng" : "Bạc") : string.Empty;
            if (!bought && !affordable && !string.IsNullOrEmpty(disabledReason))
                disabledReasonText.text = disabledReason;
            CanvasGroup group = GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = bought ? .55f : 1f;
            buyButton.onClick.RemoveAllListeners();
            if (buy != null)
                buyButton.onClick.AddListener(() => buy());
        }

        private static string Strike(string value)
        {
            var result = new System.Text.StringBuilder(value.Length * 2);
            foreach (char character in value)
                result.Append(character).Append('\u0336');
            return result.ToString();
        }
    }
}
