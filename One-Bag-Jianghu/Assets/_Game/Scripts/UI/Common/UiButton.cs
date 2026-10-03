using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    [RequireComponent(typeof(Button))]
    public sealed class UiButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private Text label;
        public UnityEvent onClick => button.onClick;

        public void SetLabel(string value)
        {
            label.text = value ?? string.Empty;
            SingleLineFit fit = label.GetComponent<SingleLineFit>();
            if (fit != null)
                fit.Fit();
        }
    }
}