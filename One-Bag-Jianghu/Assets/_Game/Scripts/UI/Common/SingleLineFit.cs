using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class SingleLineFit : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private int authoredSize;
        [SerializeField, Min(1)]
        private int minimumSize = 14;
        private Text label;
        private string lastText;
        private float lastWidth = -1f;
        private Font lastFont;
        private int lastAuthoredSize = -1;
        private bool renderRetryPending;
        public int AuthoredSize => authoredSize;
        public int MinimumSize => minimumSize;

        public void Configure(int maximumSize, int minimum = 14)
        {
            authoredSize = Mathf.Max(1, maximumSize);
            minimumSize = Mathf.Clamp(minimum, 1, authoredSize);
            Fit();
        }

        public void Fit()
        {
            if (label == null)
                label = GetComponent<Text>();
            if (label == null)
                return;
            if (authoredSize <= 0)
                authoredSize = Mathf.Max(1, label.fontSize);
            float width = label.rectTransform.rect.width;
            if (width <= 0.01f)
            {
                RequestRenderRetry();
                return;
            }

            label.resizeTextForBestFit = false;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            int low = minimumSize;
            int high = authoredSize;
            int best = minimumSize;
            while (low <= high)
            {
                int candidate = (low + high) / 2;
                label.fontSize = candidate;
                if (label.preferredWidth <= width + 0.01f)
                {
                    best = candidate;
                    low = candidate + 1;
                }
                else
                    high = candidate - 1;
            }

            label.fontSize = best;
            lastText = label.text;
            lastWidth = width;
            lastFont = label.font;
            lastAuthoredSize = authoredSize;
        }

        private void OnEnable() => Fit();
        private void OnDisable()
        {
            Canvas.willRenderCanvases -= FitAfterLayout;
            renderRetryPending = false;
        }

        private void OnValidate()
        {
            if (label == null)
                label = GetComponent<Text>();
            if (authoredSize <= 0 && label != null)
                authoredSize = Mathf.Max(1, label.fontSize);
            minimumSize = Mathf.Clamp(minimumSize, 1, Mathf.Max(1, authoredSize));
            Fit();
        }

        private void OnRectTransformDimensionsChange() => Fit();
        private void RequestRenderRetry()
        {
            if (renderRetryPending || !isActiveAndEnabled)
                return;
            renderRetryPending = true;
            Canvas.willRenderCanvases += FitAfterLayout;
        }

        private void FitAfterLayout()
        {
            Canvas.willRenderCanvases -= FitAfterLayout;
            renderRetryPending = false;
            Fit();
        }

        private void LateUpdate()
        {
            if (label == null)
                label = GetComponent<Text>();
            if (label == null)
                return;
            float width = label.rectTransform.rect.width;
            if (lastText != label.text
|| !Mathf.Approximately(lastWidth,
width)
|| lastFont != label.font
|| lastAuthoredSize != authoredSize)
                Fit();
        }
    }
}