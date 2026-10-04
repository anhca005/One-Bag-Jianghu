using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class ReplayItemView : MonoBehaviour
    {
        [SerializeField]
        private Image background;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image cooldownOverlay;
        [SerializeField]
        private Image rarityFrame;
        [SerializeField]
        private Image flashOverlay;
        [SerializeField]
        private Outline rarityOutline;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text statusText;
        private static Sprite fillSprite;
        private float flashUntil;
        private int statusUntil;
        public int Uid
        {
            get;
            private set;
        }

        public void Initialize(int uid,
            string itemName,
            int tier,
            string rarity) =>
            Initialize(uid,
            null,
            itemName,
            tier,
            rarity);
        public void Initialize(int uid,
            string itemId,
            string itemName,
            int tier,
            string rarity)
        {
            Uid = uid;
            EnsureFillSprite();
            background.color = BagViewStyle.Fill(rarity);
            rarityFrame.color = BagViewStyle.Rarity(rarity);
            nameText.text = itemName + (tier > 1 ? "  T" + tier : "");
            if (rarityOutline == null)
            {
                rarityOutline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
            }

            rarityOutline.effectColor = BagViewStyle.Rarity(rarity);
            rarityOutline.effectDistance = new Vector2(2f, -2f);
            ItemIconLibrary library = ItemIconLibrary.Load();
            Sprite sprite = null;
            bool hasIcon = library != null && library.TryGet(itemId, out sprite);
            icon.gameObject.SetActive(hasIcon);
            icon.sprite = hasIcon ? sprite : null;
            nameText.gameObject.SetActive(!hasIcon);
            nameText.fontSize = hasIcon ? 14 : ReplayStyle.SmallFont;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.rectTransform.anchorMin = Vector2.zero;
            nameText.rectTransform.anchorMax = Vector2.one;
            rarityFrame.transform.SetAsLastSibling();
            statusText.gameObject.SetActive(false);
            cooldownOverlay.gameObject.SetActive(false);
            flashOverlay.gameObject.SetActive(false);
        }

        public void Flash(bool ultimate)
        {
            Color color = ultimate ? ReplayStyle.Gold : Color.white;
            color.a = .55f;
            flashOverlay.color = color;
            flashOverlay.gameObject.SetActive(true);
            flashUntil = Time.unscaledTime + .22f;
        }

        public void SetStatus(string label, int untilMs)
        {
            statusText.text = label;
            statusText.gameObject.SetActive(true);
            statusUntil = untilMs;
        }

        public void ClearStatus()
        {
            statusText.gameObject.SetActive(false);
            statusUntil = 0;
        }

        /// <summary>
        /// A Filled image ignores fillAmount when it has no sprite and just draws the whole rect, so the cooldown
        /// overlay never drained. Give it a plain white sprite (tinted by the overlay colour).
        /// </summary>
        private void EnsureFillSprite()
        {
            if (cooldownOverlay == null || cooldownOverlay.sprite != null)
                return;
            if (fillSprite == null)
            {
                Texture2D white = Texture2D.whiteTexture;
                fillSprite = Sprite.Create(white, new Rect(0f, 0f, white.width, white.height), new Vector2(.5f, .5f));
                fillSprite.name = "CooldownFill";
            }

            cooldownOverlay.sprite = fillSprite;
            cooldownOverlay.type = Image.Type.Filled;
        }

        public void SetCooldown(float fill, bool active)
        {
            cooldownOverlay.fillAmount = Mathf.Clamp01(fill);
            cooldownOverlay.gameObject.SetActive(active);
        }

        public void Refresh(int timeMs)
        {
            if (flashUntil > 0f && Time.unscaledTime >= flashUntil)
            {
                flashOverlay.gameObject.SetActive(false);
                flashUntil = 0f;
            }

            if (statusText.gameObject.activeSelf && timeMs >= statusUntil)
            {
                statusText.gameObject.SetActive(false);
            }
        }
    }
}
