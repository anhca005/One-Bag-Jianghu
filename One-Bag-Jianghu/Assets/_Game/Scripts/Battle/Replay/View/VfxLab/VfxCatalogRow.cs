using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View.VfxLab
{
    public sealed class VfxCatalogRow : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Text status;
        [SerializeField] private Text label;
        [SerializeField] private Text key;
        [SerializeField] private Text tuned;
        [SerializeField] private Button button;
        public Image Background => background;
        public Text Tuned => tuned;
        public void Bind(VfxCatalogEntry entry, bool hasArt, UnityEngine.Events.UnityAction selected)
        {
            status.text = hasArt ? "✓" : "✗";
            status.color = hasArt ? ReplayStyle.Heal : ReplayStyle.Hp;
            label.text = entry.Label;
            key.text = entry.Key;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(selected);
        }
    }
}
