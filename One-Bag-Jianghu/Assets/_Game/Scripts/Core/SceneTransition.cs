namespace OneBagJianghu.Core
{
    public enum SceneTransitionState
    {
        Idle,
        FadingIn,
        Loading,
        WaitingReady,
        FadingOut
    }

    public sealed class SceneTransition
    {
        private readonly double minimumVisibleSeconds;
        private readonly double readyTimeoutSeconds;
        private double startedAt;
        private double waitingStartedAt;
        public SceneTransition(double minimumVisibleSeconds = 0.6d, double readyTimeoutSeconds = 5d)
        {
            this.minimumVisibleSeconds = minimumVisibleSeconds;
            this.readyTimeoutSeconds = readyTimeoutSeconds;
        }

        public SceneTransitionState State
        {
            get;
            private set;
        } = SceneTransitionState.Idle;
        public bool IsRunning => State != SceneTransitionState.Idle;

        public bool Begin(double now)
        {
            if (IsRunning)
            {
                return false;
            }

            startedAt = now;
            State = SceneTransitionState.FadingIn;
            return true;
        }

        public void Cancel()
        {
            State = SceneTransitionState.Idle;
        }

        public void Update(double now, float loadProgress, bool sceneReady, bool fadeComplete)
        {
            switch (State)
            {
                case SceneTransitionState.FadingIn when fadeComplete:
                    State = SceneTransitionState.Loading;
                    break;
                case SceneTransitionState.Loading when loadProgress >= 1f:
                    waitingStartedAt = now;
                    State = SceneTransitionState.WaitingReady;
                    break;
                case SceneTransitionState.WaitingReady:
                    bool timedOut = now - waitingStartedAt >= readyTimeoutSeconds;
                    bool visibleLongEnough = now - startedAt >= minimumVisibleSeconds;
                    if ((sceneReady || timedOut) && visibleLongEnough)
                    {
                        State = SceneTransitionState.FadingOut;
                    }

                    break;
                case SceneTransitionState.FadingOut when fadeComplete:
                    State = SceneTransitionState.Idle;
                    break;
            }
        }
    }
}