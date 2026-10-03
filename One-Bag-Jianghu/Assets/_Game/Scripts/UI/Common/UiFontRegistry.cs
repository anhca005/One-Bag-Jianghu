using UnityEngine;

namespace OneBagJianghu.UI.Common
{
    // Unity binds a ScriptableObject to its asset only when the file name matches the class name.
    [CreateAssetMenu(menuName = "One Bag Jianghu/UI Font Registry", fileName = "UiFonts")]
    public sealed class UiFontRegistry : ScriptableObject
    {
        public Font display;
        public Font title;
        public Font body;
        public Font emphasis;
    }
}