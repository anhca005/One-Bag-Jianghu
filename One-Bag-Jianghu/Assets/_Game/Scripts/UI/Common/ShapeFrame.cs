using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    /// <summary>
    /// Shows the frame sprite whose aspect is closest to this rect, so item frames of every shape (1x1, 1x2, 1x4…)
    /// use art drawn for that shape instead of one square frame stretched (decision #66).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public sealed class ShapeFrame : UIBehaviour
    {
        [SerializeField] private Sprite[] frames = new Sprite[0];

        private Image image;

        public void Initialize(Sprite[] shapeFrames)
        {
            frames = shapeFrames;
            Refresh();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Refresh();
        }

        private void Refresh()
        {
            if (frames == null || frames.Length == 0)
                return;
            if (image == null)
                image = GetComponent<Image>();
            Rect rect = ((RectTransform)transform).rect;
            if (rect.width <= 0f || rect.height <= 0f)
                return;

            float aspect = rect.width / rect.height;
            Sprite best = null;
            float bestError = float.MaxValue;
            foreach (Sprite frame in frames)
            {
                if (frame == null)
                    continue;
                float error = Mathf.Abs(Mathf.Log(aspect * frame.rect.height / frame.rect.width));
                if (error < bestError)
                {
                    bestError = error;
                    best = frame;
                }
            }

            if (best != null && image.sprite != best)
                image.sprite = best;
        }
    }
}
