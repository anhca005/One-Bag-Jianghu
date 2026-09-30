using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public enum ResultPopupChoice { Home, PlayAgain }

    public sealed class ResultPopupData
    {
        public ResultPopupData(string resultText, string winnerName) { ResultText = resultText; WinnerName = winnerName; }
        public ResultPopupData(string resultText, string winnerName, string rewards) : this(resultText, winnerName) { Rewards = rewards; }
        public string ResultText { get; }
        public string WinnerName { get; }
        public string Rewards { get; }
    }

    public sealed class ResultPopup : BasePopup<ResultPopupData>
    {
        [SerializeField] private Text resultText;
        [SerializeField] private Text winnerText;
        [SerializeField] private Text rewardText;
        [SerializeField] private UiButton homeButton;
        [SerializeField] private UiButton playAgainButton;

        protected override void Awake()
        {
            base.Awake();
            homeButton.onClick.AddListener(() => CloseWithResult(ResultPopupChoice.Home));
            playAgainButton.onClick.AddListener(() => CloseWithResult(ResultPopupChoice.PlayAgain));
        }

        protected override void OnSetup(ResultPopupData data)
        {
            data = data ?? new ResultPopupData(string.Empty, string.Empty);
            resultText.text = data.ResultText;
            winnerText.text = string.IsNullOrWhiteSpace(data.WinnerName) ? string.Empty : "Người thắng: " + data.WinnerName;
            if (rewardText != null) rewardText.text = data.Rewards ?? string.Empty;
        }
    }
}
