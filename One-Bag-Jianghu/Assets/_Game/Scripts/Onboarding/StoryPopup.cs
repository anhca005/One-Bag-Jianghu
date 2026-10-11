using System.Collections;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.Onboarding
{
    public sealed class StoryPopup : BasePopup<StoryChapter>
    {
        private const float FadeDuration = .4f;
        [SerializeField]
        private Image artwork;
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text bodyText;
        [SerializeField]
        private UiButton skipButton;
        private StoryChapter chapter;
        private int index;
        private Coroutine transition;

        protected override void Awake()
        {
            base.Awake();
            skipButton.onClick.AddListener(Close);
            if (artwork.TryGetComponent(out Button advanceButton))
                advanceButton.onClick.AddListener(Advance);
        }

        protected override void OnSetup(StoryChapter data)
        {
            chapter = data;
            index = -1;
            ShowPanel();
        }

        protected override void OnShown()
        {
            base.OnShown();
            StartCoroutine(ZoomArtwork());
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null
                && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame))
                Advance();
        }

        public void Advance()
        {
            if (transition != null)
                return;
            index++;
            if (index >= chapter.Panels.Count)
            {
                Close();
                return;
            }

            transition = StartCoroutine(FadePanel());
        }

        public static Sprite ResolveArtwork(string imageName, int chapterIndex)
        {
            PveArtLibrary library = PveArtLibrary.Load();
            if (library == null)
                return null;
            if (library.TryGet(imageName, out Sprite sprite))
                return sprite;
            if (chapterIndex > 0 && library.TryGet("map_hoi_" + chapterIndex, out sprite))
                return sprite;
            library.TryGet("intro_1", out sprite);
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
            bool titleCard = index < 0;
            IntroPanel panelData = titleCard ? chapter.Panels[0] : chapter.Panels[index];
            artwork.sprite = ResolveArtwork(panelData.Image, chapter.Chapter);
            artwork.color = artwork.sprite == null ? new Color(.82f, .76f, .62f, 1f) : Color.white;
            artwork.rectTransform.localScale = Vector3.one;
            titleText.gameObject.SetActive(titleCard);
            titleText.text = chapter.Title;
            bodyText.gameObject.SetActive(!titleCard);
            bodyText.text = titleCard ? string.Empty : panelData.Text;
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
    }
}
