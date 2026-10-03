using System;
using OneBagJianghu.Core;
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
        private bool isNewProfile;
        private void Awake()
        {
            heroName.onValueChanged.AddListener(OnNameChanged);
            enterButton.onClick.AddListener(Enter);
        }

        private void Start()
        {
            isNewProfile = string.IsNullOrWhiteSpace(GameBootstrap.Session.HeroName);
            heroName.SetTextWithoutNotify(GameBootstrap.Session.HeroName);
            OnNameChanged(heroName.text);
            SceneFlow.NotifyReady();
        }

        private void OnDestroy()
        {
            heroName.onValueChanged.RemoveListener(OnNameChanged);
            enterButton.onClick.RemoveListener(Enter);
        }

        private void OnNameChanged(string value) => enterButton.interactable = !string.IsNullOrWhiteSpace(value);
        private void Enter()
        {
            GameSession session = GameBootstrap.Session;
            session.HeroName = heroName.text.Trim();
            if (isNewProfile)
            {
                session.SectId = null;
                session.TamPhapLevel = 1;
            }

            session.SaveProfile();
            _ = GameBootstrap.Network.ConnectAsync(TimeSpan.FromSeconds(3));
            SceneFlow.Go(GameScene.Home);
        }
    }
}