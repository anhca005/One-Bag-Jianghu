using OneBagJianghu.Net;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Popups;
using UnityEngine;

namespace OneBagJianghu.Core
{
    public sealed class GameController : MonoBehaviour
    {
        public static GameController Instance
        {
            get;
            private set;
        }

        [SerializeField]
        private GameSession session;
        [SerializeField]
        private SceneFlow flow;
        [SerializeField]
        private PopupManager popups;
        [SerializeField]
        private LoadingPanel loading;
        [SerializeField]
        private NetworkService network;
        public GameSession Session => session;
        public SceneFlow Flow => flow;
        public PopupManager Popups => popups;
        public LoadingPanel Loading => loading;
        public NetworkService Network => network;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log("[Game] Duplicate GameController destroyed: " + gameObject.scene.name);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            session.LoadProfile();
            popups.Initialize();
            flow.Initialize();
            Debug.Log("[Game] GameController created: " + gameObject.scene.name);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}