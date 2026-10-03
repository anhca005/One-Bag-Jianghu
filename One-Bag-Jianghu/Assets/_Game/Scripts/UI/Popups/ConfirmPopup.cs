using System;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class ConfirmPopupData
    {
        public ConfirmPopupData(string title,
string message,
string confirmLabel,
string cancelLabel,
Action<bool> callback = null)
        {
            Title = title;
            Message = message;
            ConfirmLabel = confirmLabel;
            CancelLabel = cancelLabel;
            Callback = callback;
        }

        public string Title
        {
            get;
        }
        public string Message
        {
            get;
        }
        public string ConfirmLabel
        {
            get;
        }
        public string CancelLabel
        {
            get;
        }
        public Action<bool> Callback
        {
            get;
        }
    }

    public sealed class ConfirmPopup : BasePopup<ConfirmPopupData>
    {
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text messageText;
        [SerializeField]
        private UiButton confirmButton;
        [SerializeField]
        private UiButton cancelButton;
        private Action<bool> callback;
        private bool answered;
        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(() => Answer(true));
            cancelButton.onClick.AddListener(() => Answer(false));
        }

        protected override void OnSetup(ConfirmPopupData data)
        {
            data = data ?? new ConfirmPopupData("Xác nhận", string.Empty, "Đồng ý", "Hủy");
            titleText.text = data.Title;
            messageText.text = data.Message;
            confirmButton.SetLabel(data.ConfirmLabel);
            cancelButton.SetLabel(data.CancelLabel);
            callback = data.Callback;
            answered = false;
        }

        protected override void OnHidden()
        {
            if (!answered)
                callback?.Invoke(false);
        }

        private void Answer(bool value)
        {
            if (answered)
                return;
            answered = true;
            callback?.Invoke(value);
            CloseWithResult(value);
        }
    }
}