using System;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Arena
{
    public sealed class ArenaHistoryRow : MonoBehaviour
    {
        [SerializeField] private Text resultText;
        [SerializeField] private Text opponentText;
        [SerializeField] private Text ratingText;
        [SerializeField] private Text timeText;
        [SerializeField] private UiButton replayButton;
        private long battleId;
        private Action<long> replay;

        private void Awake()
        {
            replayButton.onClick.AddListener(Replay);
        }

        public void Bind(ArenaHistoryEntry entry, Action<long> onReplay)
        {
            battleId = entry.BattleId;
            replay = onReplay;
            resultText.text = entry.Won ? "Thắng" : "Thua";
            resultText.color = entry.Won ? new Color(.65f, .08f, .04f) : new Color(.25f, .25f, .25f);
            opponentText.text = entry.OpponentName + (entry.Bot ? " · máy" : string.Empty);
            int change = entry.RatingAfter - entry.RatingBefore;
            ratingText.text = change >= 0 ? "+" + change : change.ToString();
            timeText.text = RelativeTime(entry.At);
        }

        private void Replay()
        {
            replay?.Invoke(battleId);
        }

        private static string RelativeTime(DateTimeOffset at)
        {
            TimeSpan age = DateTimeOffset.Now - at;
            if (age.TotalMinutes < 60d)
            {
                return Math.Max(1, (int)age.TotalMinutes) + " phút trước";
            }

            if (age.TotalHours < 24d)
            {
                return Math.Max(1, (int)age.TotalHours) + " giờ trước";
            }

            return Math.Max(1, (int)age.TotalDays) + " ngày trước";
        }
    }
}
