using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using System.Collections.Generic;

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
        private readonly Dictionary<string, int> statusUntil = new Dictionary<string, int>();
        private readonly Dictionary<string, Image> statusIcons = new Dictionary<string, Image>();
        private Image statusTint;
        private Image sealIcon;
        private RectTransform statusIconRow;
        private int flashStartMs;
        private int flashUntilMs;
        private float punchStrength;
        private float cooldownFill;
        private bool cooldownActive;
        private Vector3 restScale = Vector3.one;
        public bool IsFlashing => flashOverlay != null && flashOverlay.gameObject.activeSelf;
        public bool IsSealed => statusUntil.ContainsKey("SEAL");
        public string ItemId
        {
            get;
            private set;
        }
        public string Rarity
        {
            get;
            private set;
        }
        public int Tier
        {
            get;
            private set;
        }
        public string TooltipText => BuildTooltipText();
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
            BindItemData(itemId, rarity, tier);
            EnsureFillSprite();
            background.raycastTarget = true;
            icon.raycastTarget = false;
            cooldownOverlay.raycastTarget = false;
            rarityFrame.raycastTarget = false;
            flashOverlay.raycastTarget = false;
            nameText.raycastTarget = false;
            statusText.raycastTarget = false;
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
            restScale = transform.localScale;
            EnsureStatusVisuals();
        }

        public void BindItemData(string itemId, string rarity, int tier)
        {
            ItemId = itemId;
            Rarity = rarity;
            Tier = tier;
        }

        public void SetTooltip(ItemTooltip tooltip)
        {
            HoverTooltipTarget target = GetComponent<HoverTooltipTarget>();
            if (target == null)
                target = gameObject.AddComponent<HoverTooltipTarget>();
            target.Initialize(tooltip, BuildTooltipText);
        }

        private string BuildTooltipText()
        {
            if (string.IsNullOrWhiteSpace(ItemId))
                return string.Empty;
            string description = ItemDescriber.Describe(ItemId, Rarity, Tier);
            int power = PowerCatalog.LoadFromResources().ItemPower(ItemId, Tier, Rarity);
            return ItemTooltip.WithPower(description, power);
        }

        /// <summary>Starts firing feedback on this item using replay time.</summary>
        public void Flash(bool ultimate, int nowMs)
        {
            Color color = ultimate ? ReplayStyle.Gold : Color.white;
            color.a = ultimate ? .72f : .48f;
            flashOverlay.color = color;
            flashOverlay.gameObject.SetActive(true);
            flashStartMs = nowMs;
            flashUntilMs = nowMs + (ultimate ? 220 : 150);
            punchStrength = ultimate ? .11f : .08f;
        }

        /// <summary>Shows a timed item status and keeps concurrent status icons visible.</summary>
        public void SetStatus(string kind, int untilMs)
        {
            EnsureStatusVisuals();
            statusUntil[kind] = Mathf.Max(StatusEnd(kind), untilMs);
            EnsureStatusIcon(kind);
            ApplyStatusVisuals();
        }

        /// <summary>Clears all timed item presentation for replay restart or seek.</summary>
        public void ClearStatus()
        {
            statusText.gameObject.SetActive(false);
            statusUntil.Clear();
            ApplyStatusVisuals();
        }

        /// <summary>Clears short-lived firing feedback for replay restart or seek.</summary>
        public void ClearFeedback()
        {
            flashOverlay.gameObject.SetActive(false);
            flashUntilMs = 0;
            transform.localScale = restScale;
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
            if (!IsSealed)
            {
                cooldownFill = Mathf.Clamp01(fill);
                cooldownActive = active;
            }

            cooldownOverlay.fillAmount = cooldownFill;
            cooldownOverlay.gameObject.SetActive(cooldownActive);
        }

        public void Refresh(int timeMs)
        {
            if (flashUntilMs > 0)
            {
                RefreshFlash(timeMs);
            }

            RemoveExpiredStatuses(timeMs);
        }

        private void RefreshFlash(int timeMs)
        {
            if (timeMs >= flashUntilMs || timeMs < flashStartMs)
            {
                ClearFeedback();
                return;
            }

            float progress = Mathf.InverseLerp(flashStartMs, flashUntilMs, timeMs);
            float punch = 1f + Mathf.Sin(progress * Mathf.PI) * punchStrength;
            transform.localScale = restScale * punch;
            Color color = flashOverlay.color;
            color.a *= 1f - progress;
            flashOverlay.color = color;
        }

        private void RemoveExpiredStatuses(int timeMs)
        {
            List<string> expired = null;
            foreach (KeyValuePair<string, int> pair in statusUntil)
            {
                if (timeMs < pair.Value)
                    continue;
                expired ??= new List<string>();
                expired.Add(pair.Key);
            }

            if (expired == null)
                return;
            for (int i = 0; i < expired.Count; i++)
            {
                statusUntil.Remove(expired[i]);
            }

            ApplyStatusVisuals();
        }

        private void EnsureStatusVisuals()
        {
            if (statusTint != null)
                return;
            statusTint = CreateImage("Status Tint", transform);
            Stretch(statusTint.rectTransform);
            statusTint.transform.SetSiblingIndex(flashOverlay.transform.GetSiblingIndex());
            sealIcon = CreateImage("Seal Status", transform);
            RectTransform sealRect = sealIcon.rectTransform;
            sealRect.anchorMin = sealRect.anchorMax = new Vector2(.5f, .5f);
            sealRect.sizeDelta = new Vector2(54f, 54f);
            statusIconRow = CreateRect("Status Icons", transform);
            statusIconRow.anchorMin = statusIconRow.anchorMax = new Vector2(1f, 1f);
            statusIconRow.pivot = new Vector2(1f, 1f);
            statusIconRow.anchoredPosition = new Vector2(-5f, -5f);
            statusIconRow.sizeDelta = new Vector2(90f, 26f);
            var layout = statusIconRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 3f;
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            ApplyStatusVisuals();
        }

        private void EnsureStatusIcon(string kind)
        {
            if (statusIcons.ContainsKey(kind))
                return;
            Image statusIcon = CreateImage(kind + " Icon", statusIconRow);
            statusIcon.rectTransform.sizeDelta = new Vector2(24f, 24f);
            ItemIconLibrary library = ItemIconLibrary.Load();
            string key = "status_" + kind.ToLowerInvariant();
            Sprite sprite = null;
            bool found = library != null && library.TryGet(key, out sprite);
            statusIcon.sprite = found ? sprite : fillSprite;
            statusIcon.color = found ? Color.white : StatusColor(kind);
            statusIcons[kind] = statusIcon;
            if (kind == "SEAL")
            {
                sealIcon.sprite = statusIcon.sprite;
                sealIcon.color = statusIcon.color;
            }
        }

        private void ApplyStatusVisuals()
        {
            if (statusTint == null)
                return;
            bool isSealed = IsSealed;
            bool slowed = statusUntil.ContainsKey("SLOW");
            bool hastened = statusUntil.ContainsKey("HASTE");
            statusTint.gameObject.SetActive(isSealed || slowed || hastened);
            statusTint.color = isSealed ?
                new Color(.07f, .08f, .1f, .62f) :
                slowed ?
                new Color(.25f, .42f, .58f, .35f) :
                new Color(.95f, .65f, .12f, .27f);
            sealIcon.gameObject.SetActive(isSealed);
            icon.color = isSealed ? new Color(.42f, .42f, .42f, 1f) : Color.white;
            background.color = isSealed ? new Color(.32f, .32f, .32f, 1f) : BagViewStyle.Fill(Rarity);
            rarityOutline.effectColor = hastened && !isSealed ? ReplayStyle.Gold : BagViewStyle.Rarity(Rarity);
            foreach (KeyValuePair<string, Image> pair in statusIcons)
            {
                pair.Value.gameObject.SetActive(statusUntil.ContainsKey(pair.Key));
            }
        }

        private int StatusEnd(string kind)
        {
            return statusUntil.TryGetValue(kind, out int value) ? value : 0;
        }

        private static Color StatusColor(string kind)
        {
            return kind == "HASTE" ? ReplayStyle.Gold : new Color(.45f, .62f, .78f, 1f);
        }

        private static Image CreateImage(string name, Transform parent)
        {
            RectTransform rect = CreateRect(name, parent);
            Image image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            return image;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            root.transform.SetParent(parent, false);
            return (RectTransform)root.transform;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
