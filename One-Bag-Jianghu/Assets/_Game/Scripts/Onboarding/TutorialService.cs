using System;
using OneBagJianghu.Economy;
using OneBagJianghu.Core;

namespace OneBagJianghu.Onboarding
{
    public sealed class TutorialService
    {
        private readonly TutorialCatalog catalog;
        private readonly Func<PlayerProfile> profileProvider;
        // Finished here but not yet confirmed by the server, so a stale profile cannot restart them.
        private readonly System.Collections.Generic.HashSet<string> finished =
            new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
        public TutorialSequence ActiveSequence
        {
            get;
            private set;
        }
        public int StepIndex
        {
            get;
            private set;
        } = -1;
        public TutorialStep CurrentStep => ActiveSequence == null ? null : ActiveSequence.Steps[StepIndex];
        public event Action Changed;
        public string PendingCompletionId
        {
            get;
            private set;
        }

        public TutorialService(TutorialCatalog catalog, Func<PlayerProfile> profileProvider = null)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            this.profileProvider = profileProvider ?? SessionProfile;
            CurrentProfile().CompletedTutorials ??= new System.Collections.Generic.List<string>();
        }

        public bool TryStart(string sequenceId)
        {
            if (ActiveSequence != null || IsCompleted(sequenceId))
                return false;
            ActiveSequence = catalog.Sequences.Find(value => value.Id == sequenceId);
            if (ActiveSequence == null)
                return false;
            StepIndex = 0;
            Changed?.Invoke();
            return true;
        }

        public bool Publish(string eventId)
        {
            if (CurrentStep == null || !string.Equals(CurrentStep.CompleteEvent, eventId, StringComparison.Ordinal))
                return false;
            StepIndex++;
            if (StepIndex >= ActiveSequence.Steps.Count)
                Complete();
            Changed?.Invoke();
            return true;
        }

        /// <summary>Advances past opening the quest log and skips its claim step when no reward is ready.</summary>
        public bool PublishQuestLogOpened(bool hasClaimableQuest)
        {
            bool advanced = Publish("quest_log_opened");
            if (advanced && !hasClaimableQuest)
                Publish("quest_claimed");
            return advanced;
        }

        public void SkipMissingTarget()
        {
            if (CurrentStep == null)
                return;
            StepIndex++;
            if (StepIndex >= ActiveSequence.Steps.Count)
                Complete();
            Changed?.Invoke();
        }

        public void SkipSequence()
        {
            if (ActiveSequence == null)
                return;
            Complete();
            Changed?.Invoke();
        }

        public void Reset()
        {
            CurrentProfile().CompletedTutorials.Clear();
            finished.Clear();
            ActiveSequence = null;
            StepIndex = -1;
            Changed?.Invoke();
        }

        private void Complete()
        {
            string id = ActiveSequence.Id;
            finished.Add(id);
            PendingCompletionId = id;
            ActiveSequence = null;
            StepIndex = -1;
        }

        public bool IsCompleted(string sequenceId)
        {
            return finished.Contains(sequenceId) || CurrentProfile().CompletedTutorials.Contains(sequenceId);
        }

        public string TakePendingCompletion()
        {
            string id = PendingCompletionId;
            PendingCompletionId = null;
            return id;
        }

        private PlayerProfile CurrentProfile()
        {
            return profileProvider()
                ?? throw new InvalidOperationException("Tutorial requires an active player profile.");
        }

        private static PlayerProfile SessionProfile()
        {
            return GameBootstrap.Session?.Profile;
        }
    }
}
