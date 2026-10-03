using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class MessagePopupData
    {
        public MessagePopupData(string title, string message, string buttonLabel = "Đóng")
        {
            Title = title;
            Message = message;
            ButtonLabel = buttonLabel;
        }

        public string Title
        {
            get;
        }
        public string Message
        {
            get;
        }
        public string ButtonLabel
        {
            get;
        }
    }

    public sealed class MessagePopup : BasePopup<MessagePopupData>
    {
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text messageText;
        [SerializeField]
        private UiButton closeButton;
        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(MessagePopupData data)
        {
            data = data ?? new MessagePopupData("Thông báo", string.Empty);
            titleText.text = data.Title;
            messageText.text = data.Message;
            closeButton.SetLabel(data.ButtonLabel);
        }
    }
}