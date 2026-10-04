using System.Collections;
using OneBagJianghu.Audio;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Pve;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.Onboarding
{
    public sealed class IntroPopup : BasePopup
    {
        private const float FadeDuration = .4f;
        [SerializeField]
        private Image artwork;
        [SerializeField]
        private Text bodyText;
        [SerializeField]
        private UiButton skipButton;
        private IntroCatalog catalog;
        private int index;
        private Coroutine transition;

        protected override void Awake()
        {
            base.Awake();
            skipButton.onClick.AddListener(Finish);
            if (artwork.TryGetComponent(out Button advanceButton))
                advanceButton.onClick.AddListener(Advance);
        }

        protected override void OnSetup(object data)
        {
            AudioManager.Instance?.Play("ui_open");
            AudioManager.EnsureExists().PlayMusic(MusicKeys.Intro);
            catalog = data as IntroCatalog ?? IntroCatalog.Load();
            index = 0;
            ShowPanel();
        }

        protected override void OnShown()
        {
            base.OnShown();
            // Setup runs while the popup is still inactive, so the first panel's zoom starts here.
            StartCoroutine(ZoomArtwork());
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                Advance();
        }

        public void Advance()
        {
            if (transition != null)
                return;
            AudioManager.Instance?.Play("ui_click");
            index++;
            if (index >= catalog.Panels.Count)
            {
                Finish();
                return;
            }

            transition = StartCoroutine(FadePanel());
        }

        public static Sprite ResolveArtwork(string imageName)
        {
            // Intro art lives in Textures2D/Pve and is indexed by the PvE art library, keyed by file name.
            PveArtLibrary library = PveArtLibrary.Load();
            if (library == null)
                return null;
            if (library.TryGet(imageName, out Sprite sprite))
                return sprite;
            library.TryGet("map_hoi_1", out sprite);
            return sprite;
        }

        private IEnumerator FadePanel()
        {
            float elapsed = 0f;
            while (elapsed < FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                artwork.color = new Color(1f, 1f, 1f, 1f - elapsed / FadeDuration);
                yield return null;
            }

            ShowPanel();
            elapsed = 0f;
            while (elapsed < FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                artwork.color = new Color(1f, 1f, 1f, elapsed / FadeDuration);
                yield return null;
            }

            transition = null;
        }

        private void ShowPanel()
        {
            IntroPanel introPanel = catalog.Panels[index];
            artwork.sprite = ResolveArtwork(introPanel.Image);
            artwork.color = artwork.sprite == null ? new Color(.82f, .76f, .62f, 1f) : Color.white;
            artwork.rectTransform.localScale = Vector3.one;
            bodyText.text = introPanel.Text;
            if (isActiveAndEnabled)
                StartCoroutine(ZoomArtwork());
        }

        private IEnumerator ZoomArtwork()
        {
            Vector3 startingScale = artwork.rectTransform.localScale;
            float elapsed = 0f;
            while (elapsed < 8f && artwork != null)
            {
                elapsed += Time.unscaledDeltaTime;
                artwork.rectTransform.localScale = startingScale * Mathf.Lerp(1f, 1.035f, elapsed / 8f);
                yield return null;
            }
        }

        private void Finish()
        {
            if (GameBootstrap.Session?.Profile != null)
            {
                GameBootstrap.Session.Profile.IntroSeen = true;
                GameBootstrap.Session.SaveProfile();
            }

            AudioManager.Instance?.PlayMusic(MusicKeys.Home);
            Close();
        }
    }
}
