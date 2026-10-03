using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class ReplayDemoBootstrap : MonoBehaviour
    {
        [SerializeField]
        private TextAsset log;
        [SerializeField]
        private ReplayScreen replayScreen;
        private void Start()
        {
            TextAsset source = log != null ? log : Resources.Load<TextAsset>("Replays/replay_sample");
            if (source == null)
            {
                Debug.LogError("Replay log was not found.");
                return;
            }

            replayScreen.Initialize(BattleLogParser.Parse(source.text),
ItemCatalog.LoadFromResources());
        }
    }
}