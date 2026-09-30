using System;

namespace OneBagJianghu.Battle.Replay
{
    public sealed class ReplayClock
    {
        public ReplayClock(int durationMs)
        {
            DurationMs = Math.Max(0, durationMs);
        }

        public int DurationMs { get; }
        public int TimeMs { get; private set; }
        public bool IsPlaying { get; private set; }
        public float Speed { get; private set; } = 1f;

        public void Play() => IsPlaying = TimeMs < DurationMs;
        public void Pause() => IsPlaying = false;
        public void Toggle() { if (IsPlaying) Pause(); else Play(); }
        public void SetSpeed(float speed)
        {
            if (speed != 1f && speed != 2f && speed != 4f)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be 1, 2, or 4.");
            Speed = speed;
        }

        public void Seek(int timeMs)
        {
            TimeMs = Math.Max(0, Math.Min(DurationMs, timeMs));
            if (TimeMs >= DurationMs) IsPlaying = false;
        }

        public int Advance(float unscaledDeltaSeconds)
        {
            int previous = TimeMs;
            if (!IsPlaying || unscaledDeltaSeconds <= 0f) return previous;
            Seek(TimeMs + (int)Math.Round(unscaledDeltaSeconds * 1000f * Speed));
            return previous;
        }
    }
}
