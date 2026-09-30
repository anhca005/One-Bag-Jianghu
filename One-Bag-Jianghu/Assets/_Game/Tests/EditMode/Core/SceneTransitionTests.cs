using NUnit.Framework;
using OneBagJianghu.Core;

namespace OneBagJianghu.Tests.Core
{
    public sealed class SceneTransitionTests
    {
        [Test]
        public void RunsExpectedStateSequence()
        {
            var transition = new SceneTransition();

            Assert.That(transition.Begin(10d), Is.True);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.FadingIn));
            transition.Update(10.25d, 0f, false, true);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.Loading));
            transition.Update(10.4d, 0.75f, false, false);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.Loading));
            transition.Update(10.5d, 1f, false, false);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.WaitingReady));
            transition.Update(10.7d, 1f, true, false);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.FadingOut));
            transition.Update(10.95d, 1f, true, true);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.Idle));
        }

        [Test]
        public void RejectsReentryWhileRunning()
        {
            var transition = new SceneTransition();

            Assert.That(transition.Begin(0d), Is.True);
            Assert.That(transition.Begin(0.1d), Is.False);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.FadingIn));
        }

        [Test]
        public void CancelReturnsToIdle()
        {
            var transition = new SceneTransition();
            transition.Begin(0d);

            transition.Cancel();

            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.Idle));
            Assert.That(transition.Begin(1d), Is.True);
        }

        [Test]
        public void WaitsForMinimumVisibleTime()
        {
            var transition = new SceneTransition();
            transition.Begin(0d);
            transition.Update(0.25d, 0f, false, true);
            transition.Update(0.3d, 1f, false, false);
            transition.Update(0.4d, 1f, true, false);

            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.WaitingReady));
            transition.Update(0.6d, 1f, true, false);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.FadingOut));
        }

        [Test]
        public void ReadyWaitHasSafetyTimeout()
        {
            var transition = new SceneTransition();
            transition.Begin(0d);
            transition.Update(0.25d, 0f, false, true);
            transition.Update(0.5d, 1f, false, false);
            transition.Update(5.49d, 1f, false, false);

            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.WaitingReady));
            transition.Update(5.5d, 1f, false, false);
            Assert.That(transition.State, Is.EqualTo(SceneTransitionState.FadingOut));
        }
    }
}
