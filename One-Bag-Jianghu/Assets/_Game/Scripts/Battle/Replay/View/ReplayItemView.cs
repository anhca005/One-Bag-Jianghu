using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.UI;
namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class ReplayItemView : MonoBehaviour
    {
        [SerializeField] private Image background; [SerializeField] private Image icon;
        [SerializeField] private Text nameText; [SerializeField] private Text statusText;
        private Color baseColor; private float flashUntil; private int statusUntil;
        public int Uid { get; private set; }
        public void Initialize(int uid, string itemName, int tier, Color color) => Initialize(uid, null, itemName, tier, color);
        public void Initialize(int uid, string itemId, string itemName, int tier, Color color)
        {
            Uid = uid; baseColor = color; background.color = color; nameText.text = itemName + (tier > 1 ? "  T" + tier : "");
            ItemIconLibrary library = ItemIconLibrary.Load();
            Sprite sprite = null;
            bool hasIcon = library != null && library.TryGet(itemId, out sprite);
            icon.gameObject.SetActive(hasIcon); icon.sprite = hasIcon ? sprite : null;
            nameText.fontSize = hasIcon ? 14 : ReplayStyle.SmallFont; nameText.alignment = hasIcon ? TextAnchor.LowerCenter : TextAnchor.MiddleCenter;
            nameText.rectTransform.anchorMax = hasIcon ? new Vector2(1f, .28f) : Vector2.one; statusText.gameObject.SetActive(false);
        }
        public void Flash(bool ultimate) { background.color = ultimate ? ReplayStyle.Gold : Color.white; flashUntil = Time.unscaledTime + .22f; }
        public void SetStatus(string label, int untilMs) { statusText.text = label; statusText.gameObject.SetActive(true); statusUntil = untilMs; }
        public void ClearStatus() { statusText.gameObject.SetActive(false); statusUntil = 0; }
        public void Refresh(int timeMs) { if (flashUntil > 0f && Time.unscaledTime >= flashUntil) { background.color = baseColor; flashUntil = 0f; } if (statusText.gameObject.activeSelf && timeMs >= statusUntil) statusText.gameObject.SetActive(false); }
    }
}
