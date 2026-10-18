using OneBagJianghu.Audio;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using OneBagJianghu.Quests;
using OneBagJianghu.UI.Common;
using UnityEngine;

namespace OneBagJianghu.UI.Popups
{
    public sealed class QuestLogPopup : BasePopup
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private QuestLogRow rowTemplate;
        [SerializeField] private UiButton closeButton;
        private QuestCatalog catalog;
        private QuestService service;

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
            OnboardingRuntime.PublishQuestLogOpened(HasClaimableQuest());
        }

        protected override void OnHidden()
        {
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
                    GiverName(quest.Giver),
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
            if (ClaimActive(service))
                Refresh();
        }

        /// <summary>Claims the active quest's reward and shows it; the Home quest tracker claims through this too.</summary>
        public static bool ClaimActive(QuestService quests)
        {
            QuestClaimResult result = quests.Claim(GameBootstrap.Session.Profile);
            if (!result.Success)
            {
                PopupManager.Show<MessagePopup>(
                    new MessagePopupData("Chưa thể nhận thưởng", result.Error, "Đóng"));
                return false;
            }
            GameBootstrap.Session.SaveProfile();
            AudioManager.Instance?.Play("reward");
            PopupManager.Show<MessagePopup>(
                new MessagePopupData("Nhận thưởng", RewardText(result.Reward), "Đã nhận"));
            return true;
        }

        private static string RewardText(QuestReward reward)
        {
            string text = reward.Silver + " Bạc · " + reward.HeroExp + " EXP";
            MaterialCatalog materials = GameSession.LoadMaterialCatalog();
            foreach (System.Collections.Generic.KeyValuePair<string, int> value in reward.Materials)
                text += " · " + materials.Get(value.Key).Name + " x" + value.Value;
            return text;
        }

        private static string GiverName(string id)
        {
            if (id == "lao_tieu_phu")
                return "Lục Tiêu Phu";
            if (id == "phong_loi")
                return "Phong Vũ";
            return id;
        }
    }
}
