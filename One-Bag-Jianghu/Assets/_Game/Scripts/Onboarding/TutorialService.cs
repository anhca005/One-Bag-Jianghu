using System;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Onboarding
{
    public sealed class TutorialService
    {
        private readonly TutorialCatalog catalog;
        private readonly PlayerProfile profile;
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

        public TutorialService(TutorialCatalog catalog, PlayerProfile profile)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.profile.CompletedTutorials ??= new System.Collections.Generic.List<string>();
        }

        public bool TryStart(string sequenceId)
        {
            if (ActiveSequence != null || profile.CompletedTutorials.Contains(sequenceId))
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
            profile.CompletedTutorials.Clear();
            ActiveSequence = null;
            StepIndex = -1;
            Changed?.Invoke();
        }

        private void Complete()
        {
            string id = ActiveSequence.Id;
            if (!profile.CompletedTutorials.Contains(id))
                profile.CompletedTutorials.Add(id);
            ActiveSequence = null;
            StepIndex = -1;
        }
    }
}
