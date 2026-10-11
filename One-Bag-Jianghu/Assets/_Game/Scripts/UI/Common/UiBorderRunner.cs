using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    public sealed class UiBorderRunner : MonoBehaviour
    {
        private const float LapSeconds = 2.5f;
        private const float TrailSpacing = 0.035f;
        [SerializeField]
        private Image frame;
        [SerializeField]
        private RectTransform[] lights;
        private bool running;
        private Color restingColor = Color.white;

        public bool IsRunning => running;

        private void Awake()
        {
            if (frame != null)
                restingColor = frame.color;
            ApplyVisibility();
        }

        private void Update()
        {
            if (!running)
                return;
            float phase = Mathf.Repeat(Time.unscaledTime / LapSeconds, 1f);
            for (int i = 0; i < lights.Length; i++)
            {
                float trailPhase = Mathf.Repeat(phase - i * TrailSpacing, 1f);
                SetLightPosition(lights[i], trailPhase);
            }
            float pulse = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) * .5f;
            frame.color = Color.Lerp(restingColor, new Color(1f, .9f, .66f, 1f), pulse * .22f);
        }

        /// <summary>Starts or stops the border light without resetting its unscaled-time phase.</summary>
        public void SetRunning(bool value)
        {
            running = value;
            ApplyVisibility();
            if (!running && frame != null)
                frame.color = restingColor;
        }

        private void ApplyVisibility()
        {
            if (lights == null)
                return;
            for (int i = 0; i < lights.Length; i++)
                if (lights[i] != null)
                    lights[i].gameObject.SetActive(running);
        }

        private void SetLightPosition(RectTransform light, float phase)
        {
            Rect rect = ((RectTransform)transform).rect;
            float halfWidth = rect.width * .5f;
            float halfHeight = rect.height * .5f;
            float horizontal = rect.width / (rect.width + rect.height);
            Vector2 position;
            if (phase < horizontal * .5f)
            {
                float amount = phase / (horizontal * .5f);
                position = new Vector2(Mathf.Lerp(-halfWidth, halfWidth, amount), halfHeight);
            }
            else if (phase < .5f)
            {
                float amount = (phase - horizontal * .5f) / ((1f - horizontal) * .5f);
                position = new Vector2(halfWidth, Mathf.Lerp(halfHeight, -halfHeight, amount));
            }
            else if (phase < .5f + horizontal * .5f)
            {
                float amount = (phase - .5f) / (horizontal * .5f);
                position = new Vector2(Mathf.Lerp(halfWidth, -halfWidth, amount), -halfHeight);
            }
            else
            {
                float amount = (phase - .5f - horizontal * .5f) / ((1f - horizontal) * .5f);
                position = new Vector2(-halfWidth, Mathf.Lerp(-halfHeight, halfHeight, amount));
            }
            light.anchoredPosition = position;
        }
    }
}
