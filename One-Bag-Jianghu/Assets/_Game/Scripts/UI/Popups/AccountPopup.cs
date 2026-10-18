using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Arena;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class AccountPopup : BasePopup
    {
        [SerializeField] private UiButton loginTab;
        [SerializeField] private UiButton registerTab;
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private GameObject repeatedPasswordGroup;
        [SerializeField] private InputField repeatedPasswordInput;
        [SerializeField] private GameObject heroNameGroup;
        [SerializeField] private InputField heroNameInput;
        [SerializeField] private Toggle rememberToggle;
        [SerializeField] private UiButton passwordEyeButton;
        [SerializeField] private Image passwordEyeIcon;
        [SerializeField] private Sprite eyeOpenSprite;
        [SerializeField] private Sprite eyeClosedSprite;
        [SerializeField] private UiButton confirmButton;
        [SerializeField] private UiButton laterButton;
        [SerializeField] private Text errorText;
        [SerializeField] private RectTransform formFooter;
        private bool registration;

        protected override void Awake()
        {
            base.Awake();
            loginTab.onClick.AddListener(ShowLogin);
            registerTab.onClick.AddListener(ShowRegistration);
            passwordEyeButton.onClick.AddListener(TogglePasswordVisibility);
            confirmButton.onClick.AddListener(Submit);
            laterButton.onClick.AddListener(Close);
            usernameInput.onEndEdit.AddListener(HandleUsernameEndEdit);
            passwordInput.onEndEdit.AddListener(HandlePasswordEndEdit);
            repeatedPasswordInput.onEndEdit.AddListener(HandleRepeatedPasswordEndEdit);
            heroNameInput.onEndEdit.AddListener(HandleHeroNameEndEdit);
        }

        protected override void OnSetup(object data)
        {
            usernameInput.text = string.Empty;
            passwordInput.text = string.Empty;
            repeatedPasswordInput.text = string.Empty;
            heroNameInput.text = GameBootstrap.Session?.HeroName ?? string.Empty;
            rememberToggle.SetIsOnWithoutNotify(true);
            errorText.text = string.Empty;
            ShowLogin();
        }

        private void ShowLogin()
        {
            registration = false;
            RefreshMode();
        }

        private void ShowRegistration()
        {
            registration = true;
            RefreshMode();
        }

        private void RefreshMode()
        {
            repeatedPasswordGroup.SetActive(registration);
            heroNameGroup.SetActive(registration);
            Vector2 footerPosition = formFooter.anchoredPosition;
            footerPosition.y = registration ? -526f : -366f;
            formFooter.anchoredPosition = footerPosition;
            confirmButton.SetLabel(registration ? "Đăng ký" : "Đăng nhập");
            errorText.text = string.Empty;
        }

        private void TogglePasswordVisibility()
        {
            bool hidden = passwordInput.contentType == InputField.ContentType.Password;
            InputField.ContentType type = hidden
                ? InputField.ContentType.Standard
                : InputField.ContentType.Password;
            passwordInput.contentType = type;
            repeatedPasswordInput.contentType = type;
            passwordEyeIcon.sprite = hidden ? eyeOpenSprite : eyeClosedSprite;
            passwordEyeIcon.enabled = passwordEyeIcon.sprite != null;
            passwordInput.ForceLabelUpdate();
            repeatedPasswordInput.ForceLabelUpdate();
        }

        private void Submit()
        {
            string username = usernameInput.text.Trim().ToLowerInvariant();
            string password = passwordInput.text;
            string repeatedPassword = repeatedPasswordInput.text;
            string heroName = heroNameInput.text.Trim();
            string error = AccountFormLogic.Validate(username,
                password,
                repeatedPassword,
                heroName,
                registration);
            if (!string.IsNullOrEmpty(error))
            {
                errorText.text = error;
                return;
            }

            SetBusy(true);
            bool remember = AccountFormLogic.ShouldPersistToken(rememberToggle.isOn);
            if (registration)
            {
                GameBootstrap.Network.Register(username,
                    password,
                    heroName,
                    remember,
                    HandleAuthenticated,
                    HandleError);
                return;
            }

            GameBootstrap.Network.Login(username, password, remember, HandleAuthenticated, HandleError);
        }

        private void HandleAuthenticated(AuthSession session)
        {
            SetBusy(false);
            Close();
            PopupManager.Show<ArenaLobbyPopup>();
        }

        private void HandleError(string message)
        {
            SetBusy(false);
            errorText.text = string.IsNullOrWhiteSpace(message) ? "Không kết nối được máy chủ" : message;
        }

        private void SetBusy(bool busy)
        {
            confirmButton.GetComponent<Button>().interactable = !busy;
            loginTab.GetComponent<Button>().interactable = !busy;
            registerTab.GetComponent<Button>().interactable = !busy;
            confirmButton.SetLabel(busy ? "Đang kết nối…" : registration ? "Đăng ký" : "Đăng nhập");
        }

        private void HandleUsernameEndEdit(string value)
        {
            if (SubmitPressed())
            {
                Focus(passwordInput);
            }
        }

        private void HandlePasswordEndEdit(string value)
        {
            if (SubmitPressed())
            {
                if (registration)
                {
                    Focus(repeatedPasswordInput);
                }
                else
                {
                    Submit();
                }
            }
        }

        private void HandleRepeatedPasswordEndEdit(string value)
        {
            if (SubmitPressed())
            {
                Focus(heroNameInput);
            }
        }

        private void HandleHeroNameEndEdit(string value)
        {
            if (SubmitPressed())
            {
                Submit();
            }
        }

        private static bool SubmitPressed()
        {
            // The project uses the Input System package; the legacy Input class throws.
            Keyboard keyboard = Keyboard.current;
            return keyboard != null
                && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame);
        }

        private static void Focus(InputField field)
        {
            if (field == null)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(field.gameObject);
            field.ActivateInputField();
        }
    }
}
