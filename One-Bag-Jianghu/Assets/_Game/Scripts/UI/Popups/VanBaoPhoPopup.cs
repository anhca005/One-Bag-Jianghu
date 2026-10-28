using System.Collections.Generic;
using System.Linq;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    /// <summary>Displays the complete item collection and highlights entries owned by the player.</summary>
    public sealed class VanBaoPhoPopup : BasePopup
    {
        private static readonly string[] FilterTypes = { string.Empty, "weapon", "armor", "manual", "pill", "relic" };
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private VanBaoPhoItemCard cardTemplate;
        [SerializeField]
        private Dropdown typeFilter;
        [SerializeField]
        private Text ownedCount;
        [SerializeField]
        private UiButton closeButton;
        [SerializeField]
        private ItemTooltip tooltip;
        private ItemCatalog catalog;
        private HashSet<string> ownedIds;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
            typeFilter.onValueChanged.AddListener(HandleFilterChanged);
        }

        protected override void OnSetup(object data)
        {
            catalog = ItemCatalog.LoadFromResources();
            PlayerProfile profile = GameBootstrap.Session?.Profile;
            ownedIds = new HashSet<string>(profile?.Items?.Select(value => value.ItemId)
                ?? Enumerable.Empty<string>());
            Refresh();
        }

        public override void Refresh()
        {
            ClearCards();
            string filter = typeFilter.value >= 0 && typeFilter.value < FilterTypes.Length
                ? FilterTypes[typeFilter.value]
                : string.Empty;
            IEnumerable<OwnedItem> profileItems = GameBootstrap.Session?.Profile?.Items
                ?? Enumerable.Empty<OwnedItem>();
            foreach (ItemInfo item in catalog.All)
            {
                if (!string.IsNullOrEmpty(filter) && item.Type != filter)
                {
                    continue;
                }

                OwnedItem ownedItem = profileItems.FirstOrDefault(value => value.ItemId == item.Id);
                VanBaoPhoItemCard card = Instantiate(cardTemplate, content);
                card.gameObject.SetActive(true);
                card.Bind(item, ownedItem, tooltip);
            }

            int owned = catalog.All.Count(value => ownedIds.Contains(value.Id));
            ownedCount.text = "Đã có " + UiFormat.Number(owned) + "/" + UiFormat.Number(catalog.All.Count);
        }

        private void HandleFilterChanged(int value)
        {
            Refresh();
        }

        private void ClearCards()
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Transform child = content.GetChild(i);
                if (child != cardTemplate.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

}
