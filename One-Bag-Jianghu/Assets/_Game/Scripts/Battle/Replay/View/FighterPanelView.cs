using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class FighterPanelView : MonoBehaviour
    {
        [SerializeField]
        private Text title;
        [SerializeField]
        private Image portraitFrame;
        [SerializeField]
        private Image portrait;
        [SerializeField]
        private bool enemy;
        [SerializeField]
        private Text hpText;
        [SerializeField]
        private Text shieldText;
        [SerializeField]
        private Text qiText;
        [SerializeField]
        private Image hpFill;
        [SerializeField]
        private Image shieldFill;
        [SerializeField]
        private Image qiFill;
        [SerializeField]
        private Image burnIcon;
        [SerializeField]
        private Text burnText;
        [SerializeField]
        private Image poisonIcon;
        [SerializeField]
        private Text poisonText;
        [SerializeField]
        private Image bleedIcon;
        [SerializeField]
        private Text bleedText;
        [SerializeField]
        private Image stunIcon;
        [SerializeField]
        private Text stunText;
        [SerializeField]
        private Image regenIcon;
        [SerializeField]
        private Text regenText;
        [SerializeField]
        private Image hasteIcon;
        [SerializeField]
        private Text hasteText;
        private ItemIconLibrary iconLibrary;
        private int cachedStunUntilMs;
        private static Sprite silhouette;
        private const string FrameOverlayName = "Frame Overlay";
        private static readonly Color PanelBackingColor = new Color(.93f, .89f, .8f, .9f);
        public RectTransform VfxTarget => portrait.rectTransform;

        private void Awake()
        {
            LayerFramesOnTop();
        }

        /// <summary>
        /// Both frames (the panel and the portrait) are drawn by the parent Image, so everything inside was painted
        /// over their ornaments. Turn each parent into a plain backing and redraw its frame as the last child.
        /// </summary>
        private void LayerFramesOnTop()
        {
            if (TryGetComponent(out Image panelImage))
                MoveFrameOnTop(panelImage, PanelBackingColor);
            if (portraitFrame != null)
                MoveFrameOnTop(portraitFrame, Color.clear);
        }

        private static void MoveFrameOnTop(Image owner, Color backing)
        {
            if (owner.sprite == null || owner.transform.Find(FrameOverlayName) != null)
                return;
            var overlay = new GameObject(
                FrameOverlayName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            overlay.transform.SetParent(owner.transform, false);
            overlay.transform.SetAsLastSibling();
            RectTransform rect = (RectTransform)overlay.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image frame = overlay.GetComponent<Image>();
            frame.sprite = owner.sprite;
            frame.type = owner.type;
            frame.pixelsPerUnitMultiplier = owner.pixelsPerUnitMultiplier;
            frame.preserveAspect = owner.preserveAspect;
            frame.color = new Color(1f, 1f, 1f, owner.color.a);
            frame.raycastTarget = false;

            // The parent keeps its rect and raycast; it now only paints the backing colour under the content.
            owner.sprite = null;
            owner.type = Image.Type.Simple;
            owner.color = backing;
        }

        public void Initialize(string fighterName)
        {
            title.text = fighterName;
            iconLibrary = ItemIconLibrary.Load();
            Sprite sprite = ResolvePortrait(fighterName);
            bool found = sprite != null;
            portrait.sprite = found ? sprite : CreateSilhouette();
            // Real enemy portraits are painted facing left already; only the hero fallback art is
            // mirrored and inked into a silhouette for the enemy side.
            bool enemyFallback = enemy && found && !sprite.name.StartsWith("enemy_portrait_");
            portrait.color =
                !found ?
                ReplayStyle.Ink :
                enemyFallback ?
                new Color(ReplayStyle.Ink.r,
                ReplayStyle.Ink.g,
                ReplayStyle.Ink.b,
                .72f) :
                Color.white;
            portrait.transform.localScale = new Vector3(enemyFallback ? -1f : 1f, 1f, 1f);
            burnIcon.gameObject.SetActive(false);
            poisonIcon.gameObject.SetActive(false);
            bleedIcon.gameObject.SetActive(false);
            stunIcon.gameObject.SetActive(false);
            regenIcon.gameObject.SetActive(false);
            hasteIcon.gameObject.SetActive(false);
        }

        public void SetState(FighterState state, int timeMs)
        {
            int max = Mathf.Max(1, state.MaxHp);
            hpFill.fillAmount = Mathf.Clamp01((float)state.Hp / max);
            hpText.text = "Khí Huyết  " + Mathf.Max(0, state.Hp) + "/" + state.MaxHp;
            shieldFill.fillAmount = Mathf.Clamp01((float)state.Shield / max);
            shieldText.text = "Cương Khí  " + Mathf.Max(0, state.Shield) + "/" + max;
            qiFill.fillAmount = Mathf.Clamp01(state.Qi / 100f);
            qiText.text = "Nội Lực  " + Mathf.Max(0, state.Qi) + "/100";
            SetStatus(burnIcon, burnText, "burn", state.Burn, ReplayStyle.Burn);
            SetStatus(poisonIcon,
                poisonText,
                "poison",
                state.Poison,
                ReplayStyle.Poison);
            SetStatus(bleedIcon, bleedText, "bleed", state.Bleed, ReplayStyle.Bleed);
            cachedStunUntilMs = state.StunUntilMs;
            SetStatus(stunIcon,
                stunText,
                "stun",
                cachedStunUntilMs > timeMs ?
                1 :
                0,
                ReplayStyle.Burn);
            SetStatus(regenIcon, regenText, "regen", state.Regen, ReplayStyle.Heal);
            hasteIcon.gameObject.SetActive(false);
        }

        public void RefreshTime(int timeMs)
        {
            if (stunIcon.gameObject.activeSelf && cachedStunUntilMs <= timeMs)
            {
                stunIcon.gameObject.SetActive(false);
            }
        }

        private void SetStatus(Image icon,
            Text valueText,
            string code,
            int value,
            Color fallbackColor)
        {
            icon.gameObject.SetActive(value > 0);
            if (value <= 0)
                return;
            if (iconLibrary == null)
            {
                iconLibrary = ItemIconLibrary.Load();
            }

            Sprite sprite = null;
            bool found = iconLibrary != null && iconLibrary.TryGet("status_" + code, out sprite);
            icon.sprite = found ? sprite : CreateSilhouette();
            icon.color = found ? Color.white : fallbackColor;
            valueText.text = code == "stun" ? string.Empty : value.ToString();
            valueText.fontSize = 18;
        }

        private Sprite ResolvePortrait(string fighterName)
        {
            if (iconLibrary == null)
                return null;
            Sprite sprite;
            if (!enemy && iconLibrary.TryGet("hero_portrait", out sprite))
                return sprite;
            if (enemy)
            {
                int hash = 17;
                string value = fighterName ?? string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    hash = unchecked(hash * 31 + value[i]);
                }

                int variant = (hash & int.MaxValue) % 3 + 1;
                if (iconLibrary.TryGet("enemy_portrait_" + variant, out sprite))
                    return sprite;
            }

            return iconLibrary.TryGet("hero_idle", out sprite) ? sprite : null;
        }

        private static Sprite CreateSilhouette()
        {
            if (silhouette == null)
            {
                silhouette =
                    Sprite.Create(Texture2D.whiteTexture,
                    new Rect(0f,
                    0f,
                    1f,
                    1f),
                    new Vector2(.5f,
                    .5f),
                    1f);
            }

            return silhouette;
        }
    }
}
