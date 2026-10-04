using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Audio
{
    public sealed class UiSfx : MonoBehaviour
    {
        public enum Cue
        {
            Click,
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

        private void OnEnable()
        {
            if (cue == Cue.Popup)
            {
                AudioManager.Instance?.Play("ui_open");
            }
        }

        private void OnDisable()
        {
            if (cue == Cue.Popup && gameObject.scene.isLoaded)
            {
                AudioManager.Instance?.Play("ui_close");
            }
        }

        private void PlayClick()
        {
            AudioManager.Instance?.Play("ui_click");
        }
    }
}
