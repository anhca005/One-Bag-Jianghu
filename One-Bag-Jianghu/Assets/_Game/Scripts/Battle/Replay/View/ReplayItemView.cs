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
        private Image flashOverlay;
        [SerializeField]
        private Outline rarityOutline;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text statusText;
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
            background.color = BagViewStyle.Fill(rarity);
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
            nameText.fontSize = hasIcon ? 14 : ReplayStyle.SmallFont;
            nameText.alignment = hasIcon ? TextAnchor.LowerCenter : TextAnchor.MiddleCenter;
            nameText.rectTransform.anchorMax = hasIcon ? new Vector2(1f, .28f) : Vector2.one;
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