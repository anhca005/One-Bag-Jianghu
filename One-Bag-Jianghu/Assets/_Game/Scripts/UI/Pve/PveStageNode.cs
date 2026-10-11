using OneBagJianghu.Pve;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Onboarding;

namespace OneBagJianghu.UI.Pve
{
    public sealed class PveStageNode : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image frame;
        [SerializeField]
        private Image lockIcon;
        [SerializeField]
        private Image[] starIcons;
        [SerializeField]
        private Text indexText;
        [SerializeField]
        private Text starsText;
        [SerializeField]
        private Text kindText;
        public bool UsesArt => frame.sprite != null;

        public void Bind(PveStage stage,
            bool unlocked,
            bool cleared,
            Color accent,
            System.Action<PveStage> clicked) => Bind(stage,
            unlocked,
            cleared,
            accent,
            null,
            null,
            clicked);
        public void Bind(PveStage stage,
            bool unlocked,
            bool cleared,
            Color accent,
            PveTheme theme,
            PveArtLibrary art,
            System.Action<PveStage> clicked)
        {
            TutorialTarget tutorialTarget = GetComponent<TutorialTarget>();
            if (stage.Id == "hoi_1_01")
            {
                tutorialTarget ??= gameObject.AddComponent<TutorialTarget>();
                tutorialTarget.Configure("stage_1");
            }
            else if (tutorialTarget != null)
            {
                Destroy(tutorialTarget);
            }

            Sprite nodeSprite = ResolveNodeSprite(stage.Kind, theme, art);
            frame.sprite = nodeSprite;
            frame.color = nodeSprite != null ? unlocked ? Color.white : new Color(.35f,
                .35f,
                .35f,
                .9f) : unlocked ? cleared ? accent : Color.Lerp(accent,
                Color.white,
                .45f) : new Color(.35f,
                .35f,
                .35f,
                .9f);
            indexText.text = stage.Index.ToString();
            kindText.text = stage.Kind == "boss" ? "Thủ Lĩnh" : stage.Kind == "elite" ? "TINH ANH" : string.Empty;
            if (starsText != null)
                starsText.gameObject.SetActive(false);
            if (starIcons != null)
            {
                for (int i = 0; i < starIcons.Length; i++)
                {
                    if (starIcons[i] != null)
                        starIcons[i].gameObject.SetActive(false);
                }
            }

            Sprite locked = Get(art, "icon_lock");
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!unlocked && locked != null);
                lockIcon.sprite = locked;
                lockIcon.color = Color.white;
            }

            button.interactable = unlocked;
            button.onClick.RemoveAllListeners();
            if (unlocked)
                button.onClick.AddListener(() => clicked(stage));
            transform.localScale =
                stage.Kind == "boss"
                ? Vector3.one * 1.22f
                : stage.Kind == "elite"
                ? Vector3.one * 1.1f
                : Vector3.one;
        }

        private static Sprite ResolveNodeSprite(string kind, PveTheme theme, PveArtLibrary art)
        {
            if (kind == "elite")
                return Get(art, "node_elite");
            if (kind == "boss")
                return Get(art, theme?.BossNodeIcon) ?? Get(art, "node_boss");
            return Get(art, theme?.NodeIcon) ?? Get(art, "node_stage");
        }

        private static Sprite Get(PveArtLibrary art,
            string key) => art != null
            && art.TryGet(key,
            out Sprite value) ? value : null;
    }
}
