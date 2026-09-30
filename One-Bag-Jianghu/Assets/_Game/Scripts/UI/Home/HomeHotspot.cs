using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Home
{
    public sealed class HomeHotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private Image area;
        [SerializeField] private RectTransform plaque;
        [SerializeField] private Image plaqueImage;
        [SerializeField] private Outline plaqueOutline;
        [SerializeField] private Text label;
        [SerializeField] private Text tooltip;
        [SerializeField] private GameObject lockContainer;
        [SerializeField] private Image lockIcon;
        [SerializeField] private Text lockText;
        [SerializeField] private GameObject badge;
        [SerializeField] private Image badgeImage;
        [SerializeField] private string labelText;
        [SerializeField] private string tooltipText;
        [SerializeField] private bool locked;
        [SerializeField] private int requiredLevel;
        [SerializeField] private bool badgeVisible;
        [SerializeField] private UnityEvent onClick = new UnityEvent();

        private static readonly Color LockedColor = new Color(0.42f, 0.40f, 0.38f, 0.4f);
        private Vector2 plaquePosition;
        private Coroutine shakeRoutine;

        public bool IsLocked => locked;
        public int RequiredLevel => requiredLevel;
        public UnityEvent OnClick => onClick;

        private void Awake()
        {
            plaquePosition = plaque.anchoredPosition;
            RefreshState();
        }

        public void Configure(string text, string hint, bool isLocked, int level, bool showBadge)
        {
            labelText = text;
            tooltipText = hint;
            locked = isLocked;
            requiredLevel = level;
            badgeVisible = showBadge;
            RefreshState();
        }

        public void SetLocked(bool value, int level)
        {
            locked = value;
            requiredLevel = level;
            RefreshState();
        }

        public void SetAvailableForLevel(int heroLevel)
        {
            SetLocked(heroLevel < requiredLevel, requiredLevel);
        }

        public void SetBadge(bool visible)
        {
            badgeVisible = visible;
            RefreshState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            plaque.localScale = Vector3.one * 1.06f;
            plaqueOutline.enabled = true;
            tooltip.text = locked ? $"Đạt cấp {requiredLevel} mở khóa gia nhập Môn Phái" : tooltipText;
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            plaque.localScale = Vector3.one;
            plaque.anchoredPosition = plaquePosition;
            plaqueOutline.enabled = false;
            tooltip.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData) => plaque.anchoredPosition = plaquePosition + Vector2.down * 2f;
        public void OnPointerUp(PointerEventData eventData) => plaque.anchoredPosition = plaquePosition;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!locked)
            {
                onClick.Invoke();
                return;
            }
            if (shakeRoutine != null) StopCoroutine(shakeRoutine);
            shakeRoutine = StartCoroutine(Shake());
        }

        private void RefreshState()
        {
            if (label != null) label.text = labelText;
            if (lockText != null) lockText.text = $"Cấp {requiredLevel}";
            if (lockContainer != null) lockContainer.SetActive(locked);
            if (badge != null) badge.SetActive(badgeVisible);
            if (plaqueImage != null) plaqueImage.color = locked ? LockedColor : Color.white;
        }

        private IEnumerator Shake()
        {
            float[] offsets = { -5f, 5f, -4f, 4f, -2f, 2f, 0f };
            for (int i = 0; i < offsets.Length; i++)
            {
                plaque.anchoredPosition = plaquePosition + Vector2.right * offsets[i];
                yield return null;
            }
            plaque.anchoredPosition = plaquePosition;
            shakeRoutine = null;
        }
    }
}
