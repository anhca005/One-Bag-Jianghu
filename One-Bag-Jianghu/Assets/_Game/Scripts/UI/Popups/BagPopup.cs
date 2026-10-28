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
        private bool isSaving;
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
            OneBagJianghu.Economy.PlayerProfile.ProgressChanged += HandleProgressChanged;
        }

        protected override void OnHidden()
        {
            OneBagJianghu.Economy.PlayerProfile.ProgressChanged -= HandleProgressChanged;
            itemTooltip.Hide();
        }

        private void HandleProgressChanged()
        {
            if (IsShowing)
            {
                bagPanel.Bind(GameBootstrap.Session.Profile);
            }
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
            if (isSaving)
            {
                return;
            }

            isSaving = true;
            closeButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            closeButton.SetLabel("Đang lưu…");
            try
            {
                GameBootstrap.Network.SaveLoadout(bagPanel.BuildServerPlacements(),
                    _ => Close(),
                    HandleSaveError);
            }
            catch (System.Exception exception)
            {
                HandleSaveError(exception.Message);
            }
        }

        private void HandleSaveError(string message)
        {
            isSaving = false;
            closeButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            closeButton.SetLabel("Đóng");
            bagPanel.Bind(GameBootstrap.Session.Profile);
            string reason = string.IsNullOrWhiteSpace(message)
                ? "Máy chủ chưa thể lưu hành trang. Xin thử lại."
                : message;
            PopupManager.Show<MessagePopup>(new MessagePopupData("Lỗi Lưu Hành Trang", reason, "Đã hiểu"));
        }
    }
}
