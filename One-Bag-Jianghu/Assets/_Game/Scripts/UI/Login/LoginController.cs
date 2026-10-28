using System;
using System.Collections;
using OneBagJianghu.Audio;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Login
{
    public sealed class LoginController : MonoBehaviour
    {
        [SerializeField]
        private InputField heroName;
        [SerializeField]
        private Button enterButton;
        private bool connecting;
        private void Awake()
        {
            heroName.onValueChanged.AddListener(OnNameChanged);
            enterButton.onClick.AddListener(Enter);
        }

        private void Start()
        {
            AudioManager.EnsureExists().PlayMusic(MusicKeys.Home);
            heroName.readOnly = true;
            enterButton.interactable = false;
            SceneFlow.NotifyReady();
            StartCoroutine(ConnectAndAuthenticate());
        }

        private void OnDestroy()
        {
            heroName.onValueChanged.RemoveListener(OnNameChanged);
            enterButton.onClick.RemoveListener(Enter);
        }

        private void OnNameChanged(string value)
        {
            enterButton.interactable = GameBootstrap.Network.IsAuthenticated
                && !string.IsNullOrWhiteSpace(value);
        }

        private void Enter()
        {
            if (GameBootstrap.Network.IsAuthenticated)
            {
                SceneFlow.Go(GameScene.Home);
            }
        }

        private IEnumerator ConnectAndAuthenticate()
        {
            if (connecting)
            {
                yield break;
            }

            connecting = true;
            System.Threading.Tasks.Task task = GameBootstrap.Network.ConnectAsync(TimeSpan.FromSeconds(3));
            while (!task.IsCompleted)
            {
                yield return null;
            }

            GameBootstrap.Network.DispatchPending();
            connecting = false;
            if (GameBootstrap.Network.State != NetworkState.Online)
            {
                ShowRetry();
                yield break;
            }

            if (GameBootstrap.Network.IsAuthenticated)
            {
                HandleAuthenticated(GameBootstrap.Network.Authentication);
                yield break;
            }

            PopupManager.Show<AccountPopup>(new AccountPopupData
            {
                Required = true,
                Authenticated = HandleAuthenticated
            });
        }

        private void HandleAuthenticated(AuthSession session)
        {
            heroName.SetTextWithoutNotify(GameBootstrap.Session.HeroName);
            OnNameChanged(heroName.text);
        }

        private void ShowRetry()
        {
            MessagePopup popup = PopupManager.Show<MessagePopup>(new MessagePopupData(
                "Không thể kết nối",
                "Không kết nối được máy chủ.",
                "Thử lại"));
            if (popup == null)
            {
                return;
            }

            Action<BasePopup> closed = null;
            closed = value =>
            {
                value.Closed -= closed;
                StartCoroutine(ConnectAndAuthenticate());
            };
            popup.Closed += closed;
        }
    }
}
