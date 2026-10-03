using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class SettingsPopup : BasePopup
    {
        [SerializeField]
        private Toggle fullscreenToggle;
        [SerializeField]
        private UiButton closeButton;
        protected override void Awake()
        {
            base.Awake();
            fullscreenToggle.onValueChanged.AddListener(value => Screen.fullScreen = value);
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        }
    }
}