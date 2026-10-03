using System;
using System.Collections.Generic;
using OneBagJianghu.Core;
using OneBagJianghu.UI.Loading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    /// <summary>Persistent popup service and static facade for cached, stacked popup instances.</summary>
    public sealed class PopupManager : MonoBehaviour
    {
        private const string ResourceFolder = "UI/Popups/";
        private static PopupManager instance;
        private readonly Dictionary<Type, BasePopup> cache = new Dictionary<Type, BasePopup>();
        private readonly PopupStack stack = new PopupStack();
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Canvas backdropCanvas;
        [SerializeField]
        private GameObject backdrop;
        [SerializeField]
        private Button backdropButton;
        /// <summary>Gets the topmost open popup, or null.</summary>
        public static BasePopup Top => instance == null ? null : instance.stack.Top;

        /// <summary>Initializes references authored in the services prefab.</summary>
        public void Initialize()
        {
            instance = this;
            backdropButton.onClick.AddListener(HandleBackdropClicked);
            backdrop.SetActive(false);
        }

        private void OnDestroy()
        {
            if (backdropButton != null)
            {
                backdropButton.onClick.RemoveListener(HandleBackdropClicked);
            }

            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>Shows or reconfigures the cached popup of type T.</summary>
        /// <typeparam name = "T">Concrete popup component type.</typeparam>
        /// <param name = "data">Optional setup data.</param>
        /// <returns>The shown popup, or null when its prefab is missing.</returns>
        public static T Show<T>(object data = null)
            where T : BasePopup
        {
            if (instance == null)
            {
                Debug.LogError("[Popup] PopupManager has not been initialized.");
                return null;
            }

            return instance.ShowInternal<T>(data);
        }

        /// <summary>Closes the open popup of type T with animation.</summary>
        /// <typeparam name = "T">Concrete popup component type.</typeparam>
        public static void Close<T>()
            where T : BasePopup
        {
            if (instance != null && instance.cache.TryGetValue(typeof(T), out BasePopup popup) && popup.IsShowing)
            {
                popup.Close();
            }
        }

        /// <summary>Closes the topmost popup with animation.</summary>
        public static void CloseTop()
        {
            Top?.Close();
        }

        /// <summary>Immediately hides every open popup.</summary>
        public static void CloseAll()
        {
            if (instance == null)
            {
                return;
            }

            BasePopup[] open = new BasePopup[instance.stack.Count];
            for (int i = 0; i < open.Length; i++)
            {
                open[i] = instance.stack.Items[i];
            }

            for (int i = open.Length - 1; i >= 0; i--)
            {
                open[i].Hide();
            }

            instance.stack.Clear();
            instance.RefreshStack();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>Clears cached popup instances so a full data wipe cannot retain profile-derived references.</summary>
        public static void ClearCacheAfterDataWipe()
        {
            if (instance == null)
                return;
            CloseAll();
            foreach (BasePopup popup in instance.cache.Values)
                if (popup != null)
                    Destroy(popup.gameObject);
            instance.cache.Clear();
        }
#endif
        /// <summary>Reports whether the cached popup of type T is open.</summary>
        /// <typeparam name = "T">Concrete popup component type.</typeparam>
        /// <returns>True when that popup is showing.</returns>
        public static bool IsOpen<T>()
            where T : BasePopup
        {
            return instance != null && instance.cache.TryGetValue(typeof(T), out BasePopup popup) && popup.IsShowing;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame)
            {
                return;
            }

            if (LoadingPanel.IsVisible || SceneFlow.IsTransitioning)
            {
                return;
            }

            Top?.OnBackPressed();
        }

        private T ShowInternal<T>(object data)
            where T : BasePopup
        {
            Type type = typeof(T);
            if (!cache.TryGetValue(type, out BasePopup popup))
            {
                string path = ResourceFolder + type.Name;
                GameObject prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogError("[Popup] Prefab was not found. Expected Resources path: " + path);
                    return null;
                }

                GameObject popupObject = Instantiate(prefab, canvas.transform, false);
                popup = popupObject.GetComponent<T>();
                if (popup == null)
                {
                    Debug.LogError("[Popup] Prefab at " + path + " does not contain " + type.Name + ".");
                    Destroy(popupObject);
                    return null;
                }

                Canvas nestedCanvas = popupObject.GetComponent<Canvas>();
                if (nestedCanvas == null)
                {
                    Debug.LogError("[Popup] Prefab at " + path + " is missing its Canvas.");
                    Destroy(popupObject);
                    return null;
                }

                popup.Closed += HandleClosed;
                cache.Add(type, popup);
            }

            stack.Push(popup);
            popup.transform.SetAsLastSibling();
            popup.Setup(data);
            popup.Show();
            RefreshStack();
            Debug.Log($"[Popup] Show {type.Name}, depth={stack.Count}");
            return (T)popup;
        }

        private void HandleClosed(BasePopup popup)
        {
            stack.Remove(popup);
            RefreshStack();
            Debug.Log($"[Popup] Close {popup.GetType().Name}, depth={stack.Count}");
        }

        private void HandleBackdropClicked()
        {
            Top?.OnBackdropClicked();
        }

        private void RefreshStack()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                Canvas nestedCanvas = stack.Items[i].GetComponent<Canvas>();
                // Unity drops overrideSorting when a prefab root canvas is saved; without it the backdrop
                // (its own sorted canvas) draws above the popups and swallows their clicks.
                nestedCanvas.overrideSorting = true;
                nestedCanvas.sortingOrder = PopupStack.PopupSortingOrder(i);
            }

            BasePopup top = stack.Top;
            bool showBackdrop = false;
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack.Items[i].DimsBackground)
                {
                    showBackdrop = true;
                    break;
                }
            }

            backdrop.SetActive(showBackdrop);
            if (showBackdrop)
            {
                backdropCanvas.sortingOrder = PopupStack.BackdropSortingOrder(stack.Count - 1);
            }
        }
    }
}