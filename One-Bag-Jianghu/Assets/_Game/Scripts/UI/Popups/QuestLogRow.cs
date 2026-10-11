using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Economy;
using OneBagJianghu.Quests;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class QuestLogRow : MonoBehaviour
    {
        [SerializeField]
        private Image statusIcon;
        [SerializeField]
        private Image badgeImage;
        [SerializeField]
        private Text badgeText;
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text giverText;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private Text objectiveText;
        [SerializeField]
        private RectTransform rewards;
        [SerializeField]
        private UiButton claimButton;
        [SerializeField]
        private UiBorderRunner runner;
        [SerializeField]
        private Sprite doneSprite;
        [SerializeField]
        private Sprite activeSprite;
        [SerializeField]
        private Sprite lockedSprite;
        [SerializeField]
        private Sprite mainBadgeSprite;
        [SerializeField]
        private Sprite sideBadgeSprite;

        public UiButton ClaimButton => claimButton;
        public UiBorderRunner Runner => runner;

        public void Bind(
            QuestDefinition quest,
            QuestProgress progress,
            bool claimed,
            bool active,
            string giver,
            string objective)
        {
            bool locked = !claimed && !active;
            statusIcon.sprite = locked ? lockedSprite : claimed ? doneSprite : activeSprite;
            badgeImage.sprite = quest.Kind == "main" ? mainBadgeSprite : sideBadgeSprite;
            badgeText.text = quest.Kind == "main" ? "CHÍNH" : "PHỤ";
            titleText.text = locked ? "???" : quest.Title;
            giverText.text = locked ? string.Empty : giver;
            descriptionText.text = locked ? string.Empty : quest.Text;
            objectiveText.text = locked ? string.Empty : objective + "  " + progress.Current + "/" + progress.Target;
            rewards.gameObject.SetActive(!locked);
            claimButton.gameObject.SetActive(active && progress.Done);
            runner.SetRunning(active && progress.Done);
            gameObject.name = "Quest Row " + quest.Id;
            if (!locked)
                BindRewards(quest.Reward);
        }

        private void BindRewards(QuestReward reward)
        {
            Text[] labels = rewards.GetComponentsInChildren<Text>(true);
            Image[] icons = rewards.GetComponentsInChildren<Image>(true);
            int entryIndex = 0;
            SetReward(labels, icons, entryIndex++, "icon_bac", reward.Silver.ToString());
            SetReward(labels, icons, entryIndex++, "icon_exp", reward.HeroExp + " EXP");
            foreach (KeyValuePair<string, int> material in reward.Materials)
                SetReward(labels, icons, entryIndex++, material.Key, "x" + material.Value);
            for (int i = entryIndex; i < labels.Length; i++)
                labels[i].transform.parent.gameObject.SetActive(false);
        }

        private static void SetReward(Text[] labels, Image[] icons, int index, string key, string value)
        {
            if (index >= labels.Length)
                return;
            labels[index].transform.parent.gameObject.SetActive(true);
            labels[index].text = value;
            if (index < icons.Length)
            {
                CurrencyArt.TryGetSprite(key, out Sprite sprite);
                icons[index].sprite = sprite;
                icons[index].gameObject.SetActive(sprite != null);
            }
        }
    }
}
