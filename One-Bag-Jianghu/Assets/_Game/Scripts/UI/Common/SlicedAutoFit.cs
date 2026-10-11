using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public sealed class SlicedAutoFit : UIBehaviour
    {
        private const float MaximumBorderShare = 0.9f;
        private const float DefaultReferencePixelsPerUnit = 100f;

        // This drives Image.pixelsPerUnitMultiplier (it is raised only when the borders would not fit),
        // so tune the multiplier by hand here, not on the Image.
        [SerializeField]
        [Tooltip("Pixels Per Unit Multiplier to use. Edit this, not the Image's field, which this overwrites.")]
        private float preferredMultiplier = 1f;

        /// <summary>Sets the tuned multiplier that large controls should retain.</summary>
        public void Configure(float multiplier)
        {
            preferredMultiplier = Mathf.Max(0.01f, multiplier);
            FitBorders();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            FitBorders();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            FitBorders();
        }

        private void FitBorders()
        {
            Image image = GetComponent<Image>();
            Sprite sprite = image.sprite;
            if (sprite == null || image.type != Image.Type.Sliced)
            {
                return;
            }

            Rect rect = image.rectTransform.rect;
            if (rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            float referencePixelsPerUnit = GetReferencePixelsPerUnit(image.canvas);
            Vector4 border = sprite.border;
            // A sliced border occupies border * reference PPU / (sprite PPU * multiplier) canvas units.
            float horizontal = RequiredMultiplier(border.x + border.z, rect.width, sprite.pixelsPerUnit,
                referencePixelsPerUnit);
            float vertical = RequiredMultiplier(border.y + border.w, rect.height, sprite.pixelsPerUnit,
                referencePixelsPerUnit);
            image.pixelsPerUnitMultiplier = Mathf.Max(preferredMultiplier, horizontal, vertical);
        }

        private static float RequiredMultiplier(
            float borderPixels,
            float availableSize,
            float spritePixelsPerUnit,
            float referencePixelsPerUnit)
        {
            float denominator = spritePixelsPerUnit * availableSize * MaximumBorderShare;
            if (denominator <= 0f)
            {
                return 1f;
            }

            return borderPixels * referencePixelsPerUnit / denominator;
        }

        private static float GetReferencePixelsPerUnit(Canvas canvas)
        {
            if (canvas == null || canvas.referencePixelsPerUnit <= 0f)
            {
                return DefaultReferencePixelsPerUnit;
            }

            return canvas.referencePixelsPerUnit;
        }
    }
}
