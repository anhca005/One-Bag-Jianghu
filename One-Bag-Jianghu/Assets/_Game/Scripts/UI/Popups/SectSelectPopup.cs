using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class SectSelectPopup : BasePopup
    {
        [SerializeField]
        private Dropdown sectDropdown;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private UiButton confirmButton;
        [SerializeField]
        private UiButton cancelButton;
        private SectCatalog catalog;
        protected override void Awake()
        {
            base.Awake();
            sectDropdown.onValueChanged.AddListener(RefreshSelection);
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            catalog = SectCatalog.LoadFromResources();
            var options = new List<Dropdown.OptionData>();
            foreach (SectInfo sect in catalog.All)
                options.Add(new Dropdown.OptionData(sect.Name));
            sectDropdown.options = options;
            sectDropdown.SetValueWithoutNotify(0);
            sectDropdown.RefreshShownValue();
            RefreshSelection(0);
        }

        private void RefreshSelection(int index)
        {
            if (catalog == null || catalog.All.Count == 0)
                return;
            SectInfo sect = catalog.All[Mathf.Clamp(index, 0, catalog.All.Count - 1)];
            descriptionText.text =
                $"{sect.Name}\n{sect.TamPhap} — chuyên tinh thông {string.Join(", ", sect.WeaponClasses)}.\n" +
                $"Binh khí được buff: {string.Join(", ", sect.WeaponClasses)}\n" +
                $"Tâm Pháp cấp 1: {sect.LevelOneEffect}\n\nLựa chọn này là vĩnh viễn.";
        }

        public void Confirm()
        {
            SectInfo sect = catalog.All[sectDropdown.value];
            var service = new SectChoiceService(catalog);
            if (!service.TryChoose(GameBootstrap.Session.Profile, sect.Id, out string error))
            {
                PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", error, "Đóng"));
                return;
            }

            GameBootstrap.Session.SaveProfile();
            Debug.Log("[Profile] Joined sect " + sect.Id + ", Tam Phap level 1");
            CloseWithResult(sect.Id);
        }
    }
}
