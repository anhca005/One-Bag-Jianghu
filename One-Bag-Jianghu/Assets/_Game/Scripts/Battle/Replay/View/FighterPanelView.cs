using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class FighterPanelView : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Image portraitFrame;
        [SerializeField] private Image portrait;
        [SerializeField] private bool enemy;
        [SerializeField] private Text hpText; [SerializeField] private Text shieldText; [SerializeField] private Text qiText;
        [SerializeField] private Image hpFill; [SerializeField] private Image shieldFill; [SerializeField] private Image qiFill;
        [SerializeField] private Image burnIcon; [SerializeField] private Text burnText;
        [SerializeField] private Image poisonIcon; [SerializeField] private Text poisonText;
        [SerializeField] private Image bleedIcon; [SerializeField] private Text bleedText;
        [SerializeField] private Image stunIcon; [SerializeField] private Text stunText;
        [SerializeField] private Image regenIcon; [SerializeField] private Text regenText;
        [SerializeField] private Image hasteIcon; [SerializeField] private Text hasteText;
        private ItemIconLibrary iconLibrary;
        private int cachedStunUntilMs;
        private static Sprite silhouette;

        public RectTransform VfxTarget => portrait.rectTransform;

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
            portrait.color = !found ? ReplayStyle.Ink
                : enemyFallback ? new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, .72f) : Color.white;
            portrait.transform.localScale = new Vector3(enemyFallback ? -1f : 1f, 1f, 1f);
            burnIcon.gameObject.SetActive(false); poisonIcon.gameObject.SetActive(false);
            bleedIcon.gameObject.SetActive(false); stunIcon.gameObject.SetActive(false);
            regenIcon.gameObject.SetActive(false); hasteIcon.gameObject.SetActive(false);
        }

        public void SetState(FighterState state, int timeMs)
        {
            int max = Mathf.Max(1, state.MaxHp);
            hpFill.fillAmount = Mathf.Clamp01((float)state.Hp / max);
            hpText.text = "Sinh Lực  " + Mathf.Max(0, state.Hp) + "/" + state.MaxHp;
            shieldFill.fillAmount = Mathf.Clamp01((float)state.Shield / max);
            shieldText.text = "Cương Khí  " + Mathf.Max(0, state.Shield) + "/" + max;
            qiFill.fillAmount = Mathf.Clamp01(state.Qi / 100f);
            qiText.text = "Nội Lực  " + Mathf.Max(0, state.Qi) + "/100";
            SetStatus(burnIcon, burnText, "burn", state.Burn, ReplayStyle.Burn);
            SetStatus(poisonIcon, poisonText, "poison", state.Poison, ReplayStyle.Poison);
            SetStatus(bleedIcon, bleedText, "bleed", state.Bleed, ReplayStyle.Bleed);
            cachedStunUntilMs = state.StunUntilMs;
            SetStatus(stunIcon, stunText, "stun", cachedStunUntilMs > timeMs ? 1 : 0, ReplayStyle.Burn);
            SetStatus(regenIcon, regenText, "regen", state.Regen, ReplayStyle.Heal);
            hasteIcon.gameObject.SetActive(false);
        }

        public void RefreshTime(int timeMs)
        {
            if (stunIcon.gameObject.activeSelf && cachedStunUntilMs <= timeMs) stunIcon.gameObject.SetActive(false);
        }

        private void SetStatus(Image icon, Text valueText, string code, int value, Color fallbackColor)
        {
            icon.gameObject.SetActive(value > 0);
            if (value <= 0) return;
            if (iconLibrary == null) iconLibrary = ItemIconLibrary.Load();
            Sprite sprite = null;
            bool found = iconLibrary != null && iconLibrary.TryGet("status_" + code, out sprite);
            icon.sprite = found ? sprite : CreateSilhouette();
            icon.color = found ? Color.white : fallbackColor;
            valueText.text = code == "stun" ? string.Empty : value.ToString();
            valueText.fontSize = 18;
        }

        private Sprite ResolvePortrait(string fighterName)
        {
            if (iconLibrary == null) return null;
            Sprite sprite;
            if (!enemy && iconLibrary.TryGet("hero_portrait", out sprite)) return sprite;
            if (enemy)
            {
                int hash = 17;
                string value = fighterName ?? string.Empty;
                for (int i = 0; i < value.Length; i++) hash = unchecked(hash * 31 + value[i]);
                int variant = (hash & int.MaxValue) % 3 + 1;
                if (iconLibrary.TryGet("enemy_portrait_" + variant, out sprite)) return sprite;
            }
            return iconLibrary.TryGet("hero_idle", out sprite) ? sprite : null;
        }

        private static Sprite CreateSilhouette()
        {
            if (silhouette == null) silhouette = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(.5f, .5f), 1f);
            return silhouette;
        }
    }
}
