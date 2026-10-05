using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Audio
{
    public sealed class UiSfx : MonoBehaviour
    {
        public enum Cue
        {
            Click,
            // Popups no longer play an open/close sound (user decision 2026-10-06); kept so existing prefabs load.
            Popup
        }

        [SerializeField]
        private Cue cue;

        public void ConfigureAsPopup()
        {
            cue = Cue.Popup;
        }

        private void Awake()
        {
            if (cue == Cue.Click && TryGetComponent(out Button button))
            {
                button.onClick.AddListener(PlayClick);
            }
        }

        private void PlayClick()
        {
            AudioManager.Instance?.Play("ui_click");
        }
    }
}
