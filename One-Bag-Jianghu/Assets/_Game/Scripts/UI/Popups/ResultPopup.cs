using System;
using System.Collections.Generic;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public enum ResultPopupChoice
    {
        Home,
        PlayAgain,
        Map,
        Continue
    }

    public sealed class ResultRewardEntry
    {
        public ResultRewardEntry(string iconKey, string amountText, string tooltip)
        {
            IconKey = iconKey;
            AmountText = amountText;
            Tooltip = tooltip;
        }

        public string IconKey { get; }
        public string AmountText { get; }
        public string Tooltip { get; }
    }

    public sealed class ResultPopupData
    {
        public ResultPopupData(string resultText, string winnerName)
        {
            ResultText = resultText;
            WinnerName = winnerName;
        }

        public ResultPopupData(string resultText, string winnerName, string rewards) : this(resultText, winnerName)
        {
            Rewards = rewards;
        }

        public string ResultText
        {
            get;
        }
        public string WinnerName
        {
            get;
        }
        public string Rewards
        {
            get;
        }
        public List<ResultRewardEntry> RewardEntries
        {
            get;
        } = new List<ResultRewardEntry>();
        public string LevelUpText
        {
            get;
            set;
        } = string.Empty;
        public string FirstLabel
        {
            get;
            set;
        } = "Về trấn";
        public string SecondLabel
        {
            get;
            set;
        } = "Đấu tiếp";
        public ResultPopupChoice FirstChoice
        {
            get;
            set;
        } = ResultPopupChoice.Home;
        public ResultPopupChoice SecondChoice
        {
            get;
            set;
        } = ResultPopupChoice.PlayAgain;
        public bool SecondEnabled
        {
            get;
            set;
        } = true;
        public string DisabledReason
        {
            get;
            set;
        } = string.Empty;

        /// <summary>Builds structured popup data from a successful PvE reward.</summary>
        public static ResultPopupData FromPveReward(string resultText,
            string winnerName,
            PveRewardResult reward,
            MaterialCatalog materials)
        {
            if (reward == null || !reward.Success)
            {
                return new ResultPopupData(resultText,
                    winnerName,
                    "Thưởng chưa nhận: " + (reward?.Error ?? string.Empty));
            }

            var data = new ResultPopupData(resultText, winnerName);
            data.RewardEntries.Add(new ResultRewardEntry("icon_bac", UiFormat.Number(reward.Silver), "Bạc"));
            data.RewardEntries.Add(new ResultRewardEntry("icon_exp",
                UiFormat.Number(reward.HeroExp) + " EXP",
                "Kinh nghiệm"));
            foreach (KeyValuePair<string, int> value in reward.Materials)
            {
                data.RewardEntries.Add(new ResultRewardEntry(value.Key,
                    "x " + value.Value,
                    MaterialName(materials, value.Key)));
            }

            if (reward.NewLevel > reward.OldLevel)
                data.LevelUpText = "Lên cấp! Cấp " + reward.NewLevel;
            return data;
        }

        private static string MaterialName(MaterialCatalog materials, string id)
        {
            if (materials == null)
                return id;
            try
            {
                return materials.Get(id).Name;
            }
            catch (KeyNotFoundException)
            {
                return id;
            }
        }
    }

    public sealed class ResultPopup : BasePopup<ResultPopupData>
    {
        private const int MaximumColumns = 6;
        [SerializeField]
        private Text resultText;
        [SerializeField]
        private Image resultBackdrop;
        [SerializeField]
        private Sprite victorySprite;
        [SerializeField]
        private Sprite defeatSprite;
        [SerializeField]
        private Text winnerText;
        [SerializeField]
        private Text rewardText;
        private RectTransform structuredRewards;
        private RectTransform cardsRoot;
        private Text levelUpText;
        [SerializeField]
        private UiButton homeButton;
        [SerializeField]
        private UiButton playAgainButton;
        [SerializeField]
        private Text disabledReason;
        private ItemTooltip rewardTooltip;
        private ResultPopupChoice firstChoice;
        private ResultPopupChoice secondChoice;
        public int RewardCardCount => cardsRoot == null ? 0 : cardsRoot.childCount;

        protected override void Awake()
        {
            base.Awake();
            homeButton.onClick.AddListener(() => CloseWithResult(firstChoice));
            playAgainButton.onClick.AddListener(() => CloseWithResult(secondChoice));
            homeButton.gameObject.AddComponent<TutorialTarget>().Configure("result_first");
        }

        protected override void OnSetup(ResultPopupData data)
        {
            data = data ?? new ResultPopupData(string.Empty, string.Empty);
            resultText.text = data.ResultText;
            UpdateResultBackdrop(data.ResultText);
            winnerText.text = string.IsNullOrWhiteSpace(data.WinnerName)
                ? string.Empty
                : "Người thắng: " + data.WinnerName;
            EnsureRewardLayout();
            bool hasCards = data.RewardEntries.Count > 0;
            structuredRewards.gameObject.SetActive(hasCards);
            rewardText.gameObject.SetActive(!hasCards);
            rewardText.text = data.Rewards ?? string.Empty;
            if (hasCards)
                PopulateStructuredRewards(data);
            firstChoice = data.FirstChoice;
            secondChoice = data.SecondChoice;
            homeButton.SetLabel(data.FirstLabel);
            playAgainButton.SetLabel(data.SecondLabel);
            playAgainButton.GetComponent<Button>().interactable = data.SecondEnabled;
            if (disabledReason != null)
            {
                disabledReason.text = data.DisabledReason ?? string.Empty;
                disabledReason.gameObject.SetActive(!string.IsNullOrEmpty(data.DisabledReason));
            }
        }

        public void SetRewards(string value)
        {
            if (rewardText != null)
                rewardText.text = value ?? string.Empty;
        }

        private void EnsureRewardLayout()
        {
            if (structuredRewards != null)
                return;
            Transform rewardPanel = rewardText.transform.parent;
            Transform builtLayout = rewardPanel.Find("Structured Rewards");
            if (builtLayout != null)
            {
                structuredRewards = builtLayout as RectTransform;
                RemoveLegacyStars(builtLayout);
                cardsRoot = builtLayout.Find("Reward Cards") as RectTransform;
                SetAnchors(cardsRoot, new Vector2(0f, .2f), Vector2.one);
                levelUpText = builtLayout.Find("Level Up").GetComponent<Text>();
                return;
            }

            structuredRewards = CreateRect(rewardPanel, "Structured Rewards");
            Stretch(structuredRewards, 12f);
            cardsRoot = CreateRect(structuredRewards, "Reward Cards");
            SetAnchors(cardsRoot, new Vector2(0f, .2f), Vector2.one);
            levelUpText = UiKit.Text(structuredRewards, "Level Up", string.Empty, 19);
            SetAnchors(levelUpText.rectTransform, Vector2.zero, new Vector2(1f, .2f));
        }

        private void PopulateStructuredRewards(ResultPopupData data)
        {
            ClearChildren(cardsRoot);
            CreateCards(data.RewardEntries);
            levelUpText.text = data.LevelUpText ?? string.Empty;
        }

        private void CreateCards(IReadOnlyList<ResultRewardEntry> entries)
        {
            int columns = Mathf.Min(MaximumColumns, entries.Count);
            int rows = Mathf.CeilToInt(entries.Count / (float)columns);
            float availableWidth = cardsRoot.rect.width > 0f ? cardsRoot.rect.width : 600f;
            float availableHeight = cardsRoot.rect.height > 0f ? cardsRoot.rect.height : 104f;
            const float spacing = 6f;
            float widthSize = (availableWidth - spacing * (columns - 1)) / columns;
            float heightSize = (availableHeight - spacing * (rows - 1)) / rows;
            float cardSize = Mathf.Clamp(Mathf.Min(84f, widthSize, heightSize), 48f, 84f);
            GridLayoutGroup layout = cardsRoot.GetComponent<GridLayoutGroup>();
            if (layout == null)
                layout = cardsRoot.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(cardSize, cardSize);
            layout.spacing = new Vector2(spacing, spacing);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = columns;
            foreach (ResultRewardEntry entry in entries)
                CreateCard(entry);
        }

        private void CreateCard(ResultRewardEntry entry)
        {
            Image card = UiKit.Panel(cardsRoot, "Reward Card", new Color(1f, 1f, 1f, .92f));
            ApplySprite(card, "reward_slot");
            card.type = Image.Type.Sliced;
            Image icon = UiKit.Panel(card.transform, "Icon", Color.white);
            SetAnchors(icon.rectTransform, new Vector2(.16f, .3f), new Vector2(.84f, .9f));
            ApplySprite(icon, entry.IconKey);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text amount = UiKit.Text(card.transform, "Amount", entry.AmountText, 16);
            SetAnchors(amount.rectTransform, new Vector2(.04f, .02f), new Vector2(.96f, .31f));
            amount.color = UiKit.Ink;
            amount.resizeTextForBestFit = true;
            amount.resizeTextMinSize = 11;
            amount.resizeTextMaxSize = 16;
            if (rewardTooltip == null)
            {
                rewardTooltip = ItemTooltip.CreateRuntime(transform);
                rewardTooltip.EnsureAbove(GetComponent<Canvas>());
            }
            HoverTooltipTarget target = card.gameObject.AddComponent<HoverTooltipTarget>();
            target.Initialize(rewardTooltip, () => entry.Tooltip);
        }

        private static void ApplySprite(Image image, string key)
        {
            if (CurrencyArt.TryGetSprite(key, out Sprite sprite))
            {
                image.sprite = sprite;
                return;
            }

            image.color = key == "reward_slot"
                ? new Color(UiKit.Ink.r, UiKit.Ink.g, UiKit.Ink.b, .12f)
                : Color.white;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            var value = new GameObject(name, typeof(RectTransform));
            value.transform.SetParent(parent, false);
            return value.GetComponent<RectTransform>();
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(parent.GetChild(i).gameObject);
                else
                    DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        private static void Stretch(RectTransform rect, float inset)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        private static void SetAnchors(RectTransform rect, Vector2 minimum, Vector2 maximum)
        {
            rect.anchorMin = minimum;
            rect.anchorMax = maximum;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void RemoveLegacyStars(Transform layout)
        {
            Transform legacyStars = layout.Find("Stars");
            if (legacyStars == null)
                return;
            if (Application.isPlaying)
                Destroy(legacyStars.gameObject);
            else
                DestroyImmediate(legacyStars.gameObject);
        }

        private void UpdateResultBackdrop(string value)
        {
            if (resultBackdrop == null)
                return;
            bool isDefeat = !string.IsNullOrEmpty(value)
                && (value.IndexOf("thua", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("bại", StringComparison.OrdinalIgnoreCase) >= 0);
            Sprite sprite = isDefeat ? defeatSprite : victorySprite;
            if (sprite != null)
                resultBackdrop.sprite = sprite;
        }
    }
}
