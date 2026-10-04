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
        private UiButton closeButton;
        [SerializeField]
        private ItemTooltip itemTooltip;
        public RectTransform BagHost => bagHost;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(SaveAndClose);
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

        // There is no save button: closing the bag (button, Escape or backdrop) always saves the layout.
        public override bool OnBackPressed()
        {
            if (closeOnEscape)
                SaveAndClose();
            return closeOnEscape;
        }

        public override void OnBackdropClicked()
        {
            if (closeOnBackdrop)
                SaveAndClose();
        }

        private void SaveAndClose()
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
