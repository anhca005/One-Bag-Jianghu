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
    }

    public sealed class ResultPopup : BasePopup<ResultPopupData>
    {
        [SerializeField]
        private Text resultText;
        [SerializeField]
        private Text winnerText;
        [SerializeField]
        private Text rewardText;
        [SerializeField]
        private UiButton homeButton;
        [SerializeField]
        private UiButton playAgainButton;
        [SerializeField]
        private Text disabledReason;
        private ResultPopupChoice firstChoice;
        private ResultPopupChoice secondChoice;
        protected override void Awake()
        {
            base.Awake();
            homeButton.onClick.AddListener(() => CloseWithResult(firstChoice));
            playAgainButton.onClick.AddListener(() => CloseWithResult(secondChoice));
        }

        protected override void OnSetup(ResultPopupData data)
        {
            data = data ?? new ResultPopupData(string.Empty, string.Empty);
            resultText.text = data.ResultText;
            winnerText.text = string.IsNullOrWhiteSpace(data.WinnerName) ? string.Empty : "Người thắng: "
+ data.WinnerName;
            if (rewardText != null)
                rewardText.text = data.Rewards ?? string.Empty;
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
    }
}