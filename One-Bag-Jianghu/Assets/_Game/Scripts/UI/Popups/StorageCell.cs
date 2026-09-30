using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class StorageCell : MonoBehaviour
    {
        [SerializeField] private Image paper;

        public void SetHighlight(bool valid)
        {
            paper.color = valid
                ? new Color(.35f, .65f, .35f, .35f)
                : new Color(.75f, .15f, .12f, .45f);
        }

        public void Clear() => paper.color = new Color(.18f, .15f, .12f, .10f);
    }
}
