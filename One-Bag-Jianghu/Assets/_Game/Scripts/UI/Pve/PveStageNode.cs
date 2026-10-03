using OneBagJianghu.Pve;
using UnityEngine;
using UnityEngine.UI;

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
int stars,
Color accent,
System.Action<PveStage> clicked) => Bind(stage,
unlocked,
stars,
accent,
null,
null,
clicked);
        public void Bind(PveStage stage,
bool unlocked,
int stars,
Color accent,
PveTheme theme,
PveArtLibrary art,
System.Action<PveStage> clicked)
        {
            Sprite nodeSprite = ResolveNodeSprite(stage.Kind, theme, art);
            frame.sprite = nodeSprite;
            frame.color = nodeSprite != null ? unlocked ? Color.white : new Color(.35f,
.35f,
.35f,
.9f) : unlocked ? stars > 0 ? accent : Color.Lerp(accent,
Color.white,
.45f) : new Color(.35f,
.35f,
.35f,
.9f);
            indexText.text = stage.Index.ToString();
            kindText.text = stage.Kind == "boss" ? "BOSS" : stage.Kind == "elite" ? "TINH ANH" : string.Empty;
            Sprite filled = Get(art, "icon_star"), empty = Get(art, "icon_star_empty");
            bool imageStars = filled != null && empty != null && starIcons != null && starIcons.Length == 3;
            starsText.gameObject.SetActive(!imageStars);
            starsText.text = stars > 0 ? new string('★', stars) : string.Empty;
            if (starIcons != null)
                for (int i = 0; i < starIcons.Length; i++)
                {
                    starIcons[i].gameObject.SetActive(imageStars);
                    if (imageStars)
                        starIcons[i].sprite = i < stars ? filled : empty;
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