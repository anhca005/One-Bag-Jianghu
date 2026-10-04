using System.Collections.Generic;
using OneBagJianghu.Audio;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Onboarding
{
    public sealed class TutorialOverlay : BasePopup<TutorialService>
    {
        [SerializeField]
        private Image topShade;
        [SerializeField]
        private Image bottomShade;
        [SerializeField]
        private Image leftShade;
        [SerializeField]
        private Image rightShade;
        [SerializeField]
        private Image highlight;
        [SerializeField]
        private RectTransform guideBox;
        [SerializeField]
        private Text guideLine;
        [SerializeField]
        private Image portrait;
        [SerializeField]
        private UiButton skipButton;
        private readonly HashSet<string> missingWarnings = new HashSet<string>();
        private TutorialService service;
        private RectTransform target;
        private RectTransform secondary;

        protected override void Awake()
        {
            base.Awake();
            skipButton.onClick.AddListener(Skip);
        }

        protected override void OnSetup(TutorialService value)
        {
            if (service != null)
                service.Changed -= Refresh;
            service = value;
            service.Changed += Refresh;
            Refresh();
        }

        public override void Refresh()
        {
            if (service?.CurrentStep == null)
            {
                Hide();
                return;
            }

            guideLine.text = service.CurrentStep.Line;
            if (string.IsNullOrWhiteSpace(service.CurrentStep.Target))
            {
                Hide();
                return;
            }
            TutorialTarget found = TutorialTarget.Find(service.CurrentStep.Target);
            if (found == null)
            {
                if (missingWarnings.Add(service.CurrentStep.Target))
                    Debug.LogWarning("[Tutorial] Missing target '" + service.CurrentStep.Target + "'; skipping step.");
                service.SkipMissingTarget();
                return;
            }

            target = found.transform as RectTransform;
            // A drag step also opens its drop zone (e.g. the bag grid), so the item can be dropped there.
            TutorialTarget dropZone = TutorialTarget.Find(service.CurrentStep.SecondaryTarget);
            secondary = dropZone == null ? null : dropZone.transform as RectTransform;
            UpdateHole();
        }

        // The target can move (popup open animation, scrolling), so the hole follows it every frame.
        private void LateUpdate()
        {
            if (target != null)
                UpdateHole();
        }

        protected override void OnHidden()
        {
            if (service != null)
                service.Changed -= Refresh;
            target = null;
            secondary = null;
            base.OnHidden();
        }

        private void Skip()
        {
            AudioManager.Instance?.Play("ui_click");
            service.SkipSequence();
            GameBootstrap.Session?.SaveProfile();
        }

        private void UpdateHole()
        {
            RectTransform layer = (RectTransform)topShade.rectTransform.parent;
            Rect hole = LocalRect(layer, target);
            if (secondary != null)
                hole = Union(hole, LocalRect(layer, secondary));
            Rect area = layer.rect;
            float width = area.width;
            float height = area.height;
            SetRect(topShade.rectTransform, 0f, hole.yMax, width, height - hole.yMax);
            SetRect(bottomShade.rectTransform, 0f, 0f, width, hole.yMin);
            SetRect(leftShade.rectTransform, 0f, hole.yMin, hole.xMin, hole.height);
            SetRect(rightShade.rectTransform, hole.xMax, hole.yMin, width - hole.xMax, hole.height);
            if (highlight != null)
                SetRect(highlight.rectTransform, hole.xMin, hole.yMin, hole.width, hole.height);
            PlaceGuideBox(hole, height);
        }

        private void PlaceGuideBox(Rect hole, float height)
        {
            if (guideBox == null)
                return;
            // Keep the guide away from the hole: at the bottom unless the target sits in the lower part.
            bool targetIsLow = hole.center.y < height * .4f;
            // The scroll keeps its built size; only its edge anchor changes. The paper band starts ~19% in,
            // so the scroll can sit slightly past the screen edge.
            var anchor = new Vector2(.45f, targetIsLow ? 1f : 0f);
            guideBox.anchorMin = anchor;
            guideBox.anchorMax = anchor;
            guideBox.pivot = new Vector2(.5f, targetIsLow ? .88f : .12f);
            guideBox.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Target bounds in the overlay layer's space, measured from its bottom-left corner, with a small margin.
        /// Works whatever canvas or scaler the target lives in.
        /// </summary>
        private static Rect LocalRect(RectTransform layer, RectTransform value)
        {
            var corners = new Vector3[4];
            value.GetWorldCorners(corners);
            Canvas canvas = value.GetComponentInParent<Canvas>();
            Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
            Vector2 min = ToLayer(layer, RectTransformUtility.WorldToScreenPoint(camera, corners[0]));
            Vector2 max = ToLayer(layer, RectTransformUtility.WorldToScreenPoint(camera, corners[2]));
            const float Margin = 10f;
            return Rect.MinMaxRect(min.x - Margin, min.y - Margin, max.x + Margin, max.y + Margin);
        }

        private static Rect Union(Rect first, Rect second)
        {
            return Rect.MinMaxRect(
                Mathf.Min(first.xMin, second.xMin),
                Mathf.Min(first.yMin, second.yMin),
                Mathf.Max(first.xMax, second.xMax),
                Mathf.Max(first.yMax, second.yMax));
        }

        private static Vector2 ToLayer(RectTransform layer, Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, screenPoint, null, out Vector2 local);
            return local - layer.rect.min;
        }

        private static void SetRect(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(Mathf.Max(0f, width), Mathf.Max(0f, height));
        }
    }
}
