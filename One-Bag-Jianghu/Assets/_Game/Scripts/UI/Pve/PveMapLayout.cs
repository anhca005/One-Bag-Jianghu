using UnityEngine;

namespace OneBagJianghu.UI.Pve
{
    /// <summary>Hand-authored stage positions for one chapter map.</summary>
    public sealed class PveMapLayout : MonoBehaviour
    {
        [SerializeField]
        private string chapterId;
        [SerializeField]
        private RectTransform nodesRoot;
        public string ChapterId => chapterId;
        public RectTransform NodesRoot => nodesRoot;

        public RectTransform FindAnchor(string stageId)
        {
            if (nodesRoot == null || string.IsNullOrEmpty(stageId))
                return null;
            Transform child = nodesRoot.Find(stageId);
            return child as RectTransform;
        }

        private void Awake()
        {
            // Marker children only exist to make zero-size anchors easy to see in Prefab Mode.
            if (nodesRoot == null)
                return;
            for (int i = 0; i < nodesRoot.childCount; i++)
            {
                Transform marker = nodesRoot.GetChild(i).Find("Editor Marker");
                if (marker != null)
                    marker.gameObject.SetActive(false);
            }
        }
    }
}