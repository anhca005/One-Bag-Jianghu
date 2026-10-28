using OneBagJianghu.Audio;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using OneBagJianghu.Quests;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class QuestLogPopup : BasePopup
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private QuestLogRow rowTemplate;
        [SerializeField] private UiButton closeButton;
        private QuestCatalog catalog;
        private QuestService service;
        private bool claimPending;

        public int RowCount => content == null ? 0 : content.childCount - 1;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnShown()
        {
            catalog = QuestCatalog.Load();
            service = new QuestService(
                catalog,
                PveCatalog.Load(),
                GameSession.LoadMaterialCatalog(),
                ItemCatalog.LoadFromResources());
            Refresh();
            PlayerProfile.ProgressChanged += HandleProgressChanged;
            OnboardingRuntime.PublishQuestLogOpened(HasClaimableQuest());
        }

        protected override void OnHidden()
        {
            PlayerProfile.ProgressChanged -= HandleProgressChanged;
            claimPending = false;
            base.OnHidden();
            OnboardingRuntime.Publish("quest_log_closed");
        }

        public override void Refresh()
        {
            if (service == null)
                return;
            ClearRows();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            QuestDefinition active = service.Active(profile);
            foreach (QuestDefinition quest in catalog.Quests)
            {
                bool claimed = profile.Quest.ClaimedIds.Contains(quest.Id);
                bool isActive = active != null && active.Id == quest.Id;
                QuestProgress progress = isActive || claimed
                    ? service.Progress(profile, quest)
                    : new QuestProgress(0, 1);
                QuestLogRow row = Instantiate(rowTemplate, content);
                row.gameObject.SetActive(true);
                row.Bind(
                    quest,
                    progress,
                    claimed,
                    isActive,
                    GiverName(quest.Giver, PveCatalog.Load()),
                    service.ObjectiveLabel(quest));
                row.ClaimButton.onClick.AddListener(Claim);
                if (isActive && progress.Done)
                {
                    TutorialTarget target = row.ClaimButton.gameObject.AddComponent<TutorialTarget>();
                    target.Configure("quest_claim");
                }
            }
        }

        private bool HasClaimableQuest()
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            QuestDefinition active = service.Active(profile);
            return active != null && service.Progress(profile, active).Done;
        }

        private void ClearRows()
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Transform child = content.GetChild(i);
                if (child != rowTemplate.transform)
                    Destroy(child.gameObject);
            }
        }

        private void Claim()
        {
            if (claimPending)
            {
                return;
            }

            claimPending = true;
            SetClaimButtons(false);
            ClaimActive(service, HandleClaimFinished);
        }

        /// <summary>Claims the active quest's reward and shows it; the Home quest tracker claims through this too.</summary>
        public static bool ClaimActive(QuestService quests, System.Action completed = null)
        {
            PlayerProfile preview = PlayerProfile.FromJson(GameBootstrap.Session.Profile.ToJson());
            QuestDefinition quest = quests.Active(GameBootstrap.Session.Profile);
            QuestClaimResult result = quests.Claim(preview);
            if (!result.Success)
            {
                PopupManager.Show<MessagePopup>(
                    new MessagePopupData("Chưa thể nhận thưởng", result.Error, "Đóng"));
                completed?.Invoke();
                return false;
            }
            GameBootstrap.Network.ClaimQuest(quest.Id,
                _ => HandleClaimed(result.Reward, completed),
                message => HandleClaimError(message, completed));
            return true;
        }

        private static void HandleClaimed(QuestReward reward, System.Action completed)
        {
            AudioManager.Instance?.Play("reward");
            OnboardingRuntime.Publish("quest_claimed");
            PopupManager.Show<MessagePopup>(
                new MessagePopupData("Nhận thưởng", RewardText(reward), "Đã nhận"));
            completed?.Invoke();
        }

        private static void HandleClaimError(string message, System.Action completed)
        {
            PopupManager.Show<MessagePopup>(
                new MessagePopupData("Chưa thể nhận thưởng", message, "Đóng"));
            completed?.Invoke();
        }

        private void HandleClaimFinished()
        {
            claimPending = false;
            Refresh();
        }

        private void HandleProgressChanged()
        {
            if (IsShowing)
            {
                Refresh();
            }
        }

        private void SetClaimButtons(bool interactable)
        {
            foreach (Button button in content.GetComponentsInChildren<Button>(true))
            {
                if (button.gameObject != closeButton.gameObject)
                {
                    button.interactable = interactable;
                }
            }
        }

        private static string RewardText(QuestReward reward)
        {
            string text = UiFormat.Number(reward.Silver) + " Bạc · " + UiFormat.Number(reward.HeroExp) + " EXP";
            MaterialCatalog materials = GameSession.LoadMaterialCatalog();
            foreach (System.Collections.Generic.KeyValuePair<string, int> value in reward.Materials)
                text += " · " + materials.Get(value.Key).Name + " x" + UiFormat.Number(value.Value);
            return text;
        }

        internal static string GiverName(string id, PveCatalog pve)
        {
            PveCharacter character = pve?.GetCharacter(id);
            return character == null || string.IsNullOrWhiteSpace(character.Name)
                ? "Người trong giang hồ"
                : character.Name;
        }
    }
}
