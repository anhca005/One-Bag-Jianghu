using System;
using System.Collections;
using UnityEngine;

namespace OneBagJianghu.UI.Popups
{
    /// <summary>
    /// Base for reusable popup views. The lifecycle is Awake, Setup, Show, OnEnable,
    /// open animation, OnShown, then Close/Hide, OnHidden, Closed, and OnDisable.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePopup : MonoBehaviour
    {
        private const float AnimationDuration = 0.15f;
        [SerializeField]
        protected RectTransform panel;
        [SerializeField]
        protected bool closeOnBackdrop = true;
        [SerializeField]
        protected bool closeOnEscape = true;
        [SerializeField]
        protected bool dimBackground = true;
        private CanvasGroup canvasGroup;
        private Coroutine animationRoutine;
        private bool isClosing;
        /// <summary>Gets whether this popup is currently active and shown.</summary>
        public bool IsShowing
        {
            get;
            private set;
        }
        /// <summary>Gets whether an opening or closing animation is running.</summary>
        public bool IsAnimating
        {
            get;
            private set;
        }
        /// <summary>Gets the data most recently supplied to <see cref = "Setup"/>.</summary>
        public object Data
        {
            get;
            private set;
        }
        /// <summary>Gets the optional result supplied by <see cref = "CloseWithResult{TResult}"/>.</summary>
        public object Result
        {
            get;
            private set;
        }
        /// <summary>Gets whether the shared dimming backdrop is requested.</summary>
        public bool DimsBackground => dimBackground;

        /// <summary>Raised after the popup has been hidden.</summary>
        public event Action<BasePopup> Closed;
        /// <summary>Caches required components and supplies a default animation panel.</summary>
        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (panel == null)
            {
                panel = transform as RectTransform;
            }
        }

        /// <summary>Stores data and lets the popup bind it to its view.</summary>
        /// <param name = "data">Popup-specific data, or null.</param>
        public void Setup(object data)
        {
            Data = data;
            Result = null;
            OnSetup(data);
        }

        /// <summary>Activates the popup and plays its unscaled opening animation.</summary>
        public void Show()
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }

            isClosing = false;
            IsShowing = true;
            gameObject.SetActive(true);
            animationRoutine = StartCoroutine(Animate(true));
        }

        /// <summary>Plays the closing animation and then hides the popup.</summary>
        public void Close()
        {
            if (!IsShowing || isClosing)
            {
                return;
            }

            isClosing = true;
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }

            animationRoutine = StartCoroutine(Animate(false));
        }

        /// <summary>Hides the popup immediately and raises <see cref = "Closed"/> once.</summary>
        public void Hide()
        {
            if (!IsShowing)
            {
                return;
            }

            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            IsShowing = false;
            IsAnimating = false;
            isClosing = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            OnHidden();
            Closed?.Invoke(this);
            gameObject.SetActive(false);
        }

        /// <summary>Handles a back action, closing when escape is enabled.</summary>
        /// <returns>True when the action was consumed.</returns>
        public virtual bool OnBackPressed()
        {
            if (closeOnEscape)
            {
                Close();
            }

            return closeOnEscape;
        }

        /// <summary>Handles a click on the manager's shared backdrop.</summary>
        public virtual void OnBackdropClicked()
        {
            if (closeOnBackdrop)
            {
                Close();
            }
        }

        /// <summary>Redraws the popup from <see cref = "Data"/>. Default is a no-op.</summary>
        public virtual void Refresh()
        {
        }

        /// <summary>Receives setup data before the popup is shown.</summary>
        /// <param name = "data">Popup-specific data, or null.</param>
        protected virtual void OnSetup(object data)
        {
        }

        /// <summary>Called after the opening animation completes.</summary>
        protected virtual void OnShown()
        {
        }

        /// <summary>Called just before deactivation and the Closed event.</summary>
        protected virtual void OnHidden()
        {
        }

        /// <summary>
        /// Subscribe to data events here. Overrides must call base.OnEnable().
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Unsubscribe from data events here. Overrides must call base.OnDisable().
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>Stores a typed result and starts closing the popup.</summary>
        /// <typeparam name = "TResult">Result type.</typeparam>
        /// <param name = "result">Result made available through <see cref = "Result"/>.</param>
        protected void CloseWithResult<TResult>(TResult result)
        {
            Result = result;
            Close();
        }

        private IEnumerator Animate(bool opening)
        {
            IsAnimating = true;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
            float elapsed = 0f;
            float fromAlpha = opening ? 0f : canvasGroup.alpha;
            float toAlpha = opening ? 1f : 0f;
            Vector3 fromScale = opening ? Vector3.one * 0.9f : panel.localScale;
            Vector3 toScale = opening ? Vector3.one : Vector3.one * 0.9f;
            canvasGroup.alpha = fromAlpha;
            panel.localScale = fromScale;
            while (elapsed < AnimationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float amount = Mathf.Clamp01(elapsed / AnimationDuration);
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, amount);
                panel.localScale = Vector3.Lerp(fromScale, toScale, amount);
                yield return null;
            }

            canvasGroup.alpha = toAlpha;
            panel.localScale = toScale;
            animationRoutine = null;
            IsAnimating = false;
            if (opening)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                OnShown();
            }
            else
            {
                Hide();
            }
        }
    }

    /// <summary>Type-safe popup base that validates setup data.</summary>
    /// <typeparam name = "TData">The data required by the popup.</typeparam>
    public abstract class BasePopup<TData> : BasePopup
    {
        /// <summary>Validates and forwards setup data to the typed overload.</summary>
        /// <param name = "data">Data expected to be <typeparamref name = "TData"/>.</param>
        protected sealed override void OnSetup(object data)
        {
            if (data == null && default(TData) is null)
            {
                OnSetup(default);
                return;
            }

            if (!(data is TData typedData))
            {
                throw new ArgumentException("Expected popup data of type "
                    + typeof(TData).FullName
                    + ".",
                    nameof(data));
            }

            OnSetup(typedData);
        }

        /// <summary>Binds strongly typed setup data to the popup.</summary>
        /// <param name = "data">Typed popup data.</param>
        protected abstract void OnSetup(TData data);
    }
}