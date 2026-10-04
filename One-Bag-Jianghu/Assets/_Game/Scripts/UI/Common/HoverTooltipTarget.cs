using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneBagJianghu.UI.Common
{
    public sealed class HoverTooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ItemTooltip tooltip;
        private Func<string> textProvider;

        public void Initialize(ItemTooltip value, Func<string> provider)
        {
            tooltip = value;
            textProvider = provider;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            string text = textProvider?.Invoke();
            tooltip?.ShowDelayed(text, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip?.Hide();
        }

        private void OnDisable()
        {
            tooltip?.Hide();
        }
    }
}
