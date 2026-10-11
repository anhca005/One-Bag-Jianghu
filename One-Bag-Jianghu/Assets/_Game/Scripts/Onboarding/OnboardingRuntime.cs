using OneBagJianghu.Core;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using System.Collections;

namespace OneBagJianghu.Onboarding
{
    public sealed class OnboardingRuntime : MonoBehaviour
    {
        public static TutorialService Tutorial
        {
            get;
            private set;
        }

        private IEnumerator Start()
        {
            if (GameBootstrap.Session?.Profile == null)
                yield break;
            // Let Home finish its own Start first (it may reopen the stage map after a battle), so a step that
            // points into that popup can find it and the overlay lands on top of it.
            yield return null;
            Tutorial = new TutorialService(TutorialCatalog.Load(), GameBootstrap.Session.Profile);
            if (!GameBootstrap.Session.Profile.IntroSeen)
            {
                IntroPopup intro = PopupManager.Show<IntroPopup>();
                if (intro != null)
                    intro.Closed += HandleIntroClosed;
                yield break;
            }

            TryTriggeredSequences();
        }

        public static void Publish(string eventId)
        {
            if (Tutorial == null || !Tutorial.Publish(eventId))
                return;
            GameBootstrap.Session?.SaveProfile();
            ShowCurrentStep();
        }

        /// <summary>Publishes the quest-log opening and skips an unavailable claim tutorial step.</summary>
        public static void PublishQuestLogOpened(bool hasClaimableQuest)
        {
            if (Tutorial == null || !Tutorial.PublishQuestLogOpened(hasClaimableQuest))
                return;
            GameBootstrap.Session?.SaveProfile();
            ShowCurrentStep();
        }

        private static void TryTriggeredSequences()
        {
            if (Tutorial.TryStart("first_battle"))
                ShowCurrentStep();
            else if (PveRules.IsCleared(GameBootstrap.Session.Profile, "hoi_1_01") && Tutorial.TryStart("shop"))
                ShowCurrentStep();
            else if (GameBootstrap.Session.Profile.HeroLevel >= 10 && Tutorial.TryStart("sect"))
                ShowCurrentStep();
        }

        private static void ShowCurrentStep()
        {
            // A step without a target only waits for its event (e.g. the battle itself); the overlay stays away.
            // An open overlay hides itself through Refresh when the service changes.
            TutorialStep step = Tutorial?.CurrentStep;
            if (step != null && !string.IsNullOrWhiteSpace(step.Target))
                PopupManager.Show<TutorialOverlay>(Tutorial);
        }

        private void HandleIntroClosed(BasePopup popup)
        {
            popup.Closed -= HandleIntroClosed;
            Tutorial.TryStart("first_battle");
            ShowCurrentStep();
        }
    }
}
