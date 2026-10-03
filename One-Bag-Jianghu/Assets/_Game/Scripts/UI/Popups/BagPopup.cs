using OneBagJianghu.UI.Common;
using OneBagJianghu.Core;
using UnityEngine;

namespace OneBagJianghu.UI.Popups
{
    public sealed class BagPopup : BasePopup
    {
        [SerializeField]
        private RectTransform bagHost;
        [SerializeField]
        private BagLoadoutPanel bagPanel;
        [SerializeField]
        private UiButton saveButton;
        [SerializeField]
        private UiButton closeButton;
        [SerializeField]
        private ItemTooltip itemTooltip;
        public RectTransform BagHost => bagHost;

        protected override void Awake()
        {
            base.Awake();
            saveButton.onClick.AddListener(Save);
            closeButton.onClick.AddListener(Close);
            bagPanel.SetTooltip(itemTooltip);
        }

        protected override void OnSetup(object data)
        {
            GameSession session = GameBootstrap.Session;
            // PopupManager calls Setup before Awake on first show, so wire the tooltip before the views are built.
            bagPanel.SetTooltip(itemTooltip);
            bagPanel.Bind(session.Profile);
        }

        protected override void OnShown()
        {
            itemTooltip.EnsureAbove(GetComponent<Canvas>());
        }

        protected override void OnHidden()
        {
            itemTooltip.Hide();
        }

        private void Save()
        {
            GameSession session = GameBootstrap.Session;
            if (!bagPanel.TryApplyToProfile(out string error))
            {
                PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", error));
                return;
            }

            session.SaveProfile();
            Close();
        }
    }
}