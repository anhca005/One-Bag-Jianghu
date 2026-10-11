using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    /// <summary>
    /// Shows a Slider's value through a Filled image instead of resizing the fill rect, so the fill art is
    /// cropped rather than stretched (decision #66). The slider's own fillRect stays empty.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Slider))]
    public sealed class SliderFillAmount : MonoBehaviour
    {
        [SerializeField] private Image fill;

        private Slider slider;

        public void Initialize(Image fillImage)
        {
            fill = fillImage;
            Refresh();
        }

        // Polled rather than event-driven: SetValueWithoutNotify does not raise onValueChanged.
        private void LateUpdate()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (fill == null)
                return;
            if (slider == null)
                slider = GetComponent<Slider>();
            fill.fillAmount = slider.normalizedValue;
        }
    }
}
