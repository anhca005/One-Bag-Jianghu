using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using OneBagJianghu.Quests;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.UI.Pve;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Home
{
    public sealed class QuestTracker : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text kindText;
        [SerializeField] private Image badgeImage;
        [SerializeField] private Sprite mainBadgeSprite;
        [SerializeField] private Sprite sideBadgeSprite;
        [SerializeField] private Button bodyButton;
        [SerializeField] private Button logButton;
        [SerializeField] private UiBorderRunner runner;
        private QuestService service;
        private HomeController home;

        public void Initialize(HomeController controller)
        {
            home = controller;
            service = new QuestService(
                QuestCatalog.Load(),
                PveCatalog.Load(),
                GameSession.LoadMaterialCatalog(),
                ItemCatalog.LoadFromResources());
            bodyButton.onClick.AddListener(HandleBodyClick);
            logButton.onClick.AddListener(OpenLog);
            QuestService.Changed += Refresh;
            PlayerProfile.ProgressChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            QuestService.Changed -= Refresh;
            PlayerProfile.ProgressChanged -= Refresh;
        }

        public void Refresh()
        {
            if (service == null || GameBootstrap.Session?.Profile == null)
                return;
            QuestDefinition quest = service.Active(GameBootstrap.Session.Profile);
            gameObject.SetActive(quest != null);
            if (quest == null)
                return;
            QuestProgress progress = service.Progress(GameBootstrap.Session.Profile, quest);
            titleText.text = "Hồi " + quest.Chapter + " · " + quest.Title;
            kindText.text = quest.Kind == "main" ? "CHÍNH" : "PHỤ";
            badgeImage.sprite = quest.Kind == "main" ? mainBadgeSprite : sideBadgeSprite;
            objectiveText.text = service.ObjectiveLabel(quest) + "  " + progress.Current + "/" + progress.Target;
            runner.SetRunning(progress.Done);
        }

        private void HandleBodyClick()
        {
            QuestDefinition quest = service.Active(GameBootstrap.Session.Profile);
            if (quest == null || !service.Progress(GameBootstrap.Session.Profile, quest).Done)
                GoToObjective();
            else if (IsQuestLogTutorialStep())
                OpenLog();
            else
                QuestLogPopup.ClaimActive(service);
        }

        // The tutorial teaches the quest log by having the player open it and claim there.
        private static bool IsQuestLogTutorialStep()
        {
            TutorialStep step = OnboardingRuntime.Tutorial?.CurrentStep;
            return step != null && step.Target == "home_quest";
        }

        private void GoToObjective()
        {
            QuestDefinition quest = service.Active(GameBootstrap.Session.Profile);
            if (quest == null)
                return;
            if (quest.Goto.StartsWith("stage:", System.StringComparison.Ordinal))
                PopupManager.Show<StageMapPopup>(quest.Goto.Substring(6));
            else if (quest.Goto == "shop")
                home.OpenShop();
            else if (quest.Goto == "storage")
                home.OpenStorage();
            else if (quest.Goto == "bag")
                home.OpenBag();
            else if (quest.Goto == "sect")
                home.OpenSect();
        }

        private static void OpenLog()
        {
            PopupManager.Show<QuestLogPopup>();
        }
    }
}
