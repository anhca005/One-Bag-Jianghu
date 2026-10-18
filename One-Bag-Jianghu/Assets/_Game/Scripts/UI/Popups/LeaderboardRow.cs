using OneBagJianghu.Net;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.UI.Arena;

namespace OneBagJianghu.UI.Popups
{
    public sealed class LeaderboardRow : MonoBehaviour
    {
        [SerializeField]
        private Image background;
        [SerializeField]
        private Text rankText;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text ratingText;
        [SerializeField]
        private Text recordText;
        [SerializeField]
        private Image medalImage;
        [SerializeField]
        private Image rankBadge;
        [SerializeField]
        private Sprite[] medalSprites;
        [SerializeField]
        private Sprite[] rankSprites;
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite selectedSprite;

        public void Bind(LeaderboardEntry entry, bool isSelf)
        {
            rankText.text = "#" + entry.Rank;
            nameText.text = entry.HeroName;
            ratingText.text = entry.Rating.ToString();
            recordText.text = entry.Wins + "/" + entry.Losses;
            medalImage.sprite = entry.Rank >= 1 && entry.Rank <= 3 ? medalSprites[entry.Rank - 1] : null;
            medalImage.gameObject.SetActive(medalImage.sprite != null);
            rankText.gameObject.SetActive(medalImage.sprite == null);
            ArenaRanks ranks = ArenaRanks.Load();
            string imageKey = ranks.ForRating(entry.Rating).ImageKey;
            int rankIndex = -1;
            for (int i = 0; i < ranks.Ranks.Count; i++)
            {
                if (ranks.Ranks[i].ImageKey == imageKey)
                {
                    rankIndex = i;
                    break;
                }
            }

            rankBadge.sprite = rankIndex >= 0 && rankIndex < rankSprites.Length ? rankSprites[rankIndex] : null;
            rankBadge.enabled = rankBadge.sprite != null;
            background.sprite = isSelf ? selectedSprite : normalSprite;
            background.enabled = background.sprite != null;
            background.color = isSelf ? new Color(1f, .9f, .55f, 1f) : Color.white;
        }
    }
}
