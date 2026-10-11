using System.Collections.Generic;
using OneBagJianghu.Core;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools
{
    /// <summary>Editor menu that deletes the saved progress, so the next Play starts a new game.</summary>
    public static class ClearSaveDataMenu
    {
        private const string MenuPath = "One Bag Jianghu/Clear Save Data";

        [MenuItem(MenuPath)]
        public static void ClearSaveData()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Clear Save Data",
                "Xóa toàn bộ tiến độ đã lưu (hồ sơ, túi đồ, nhiệm vụ) trong:\n"
                + ProfileStore.SaveDirectory
                + "\n\nCài đặt âm thanh/hiển thị được giữ lại. Không thể hoàn tác.",
                "Xóa",
                "Hủy");
            if (!confirmed)
                return;

            List<string> deleted = ProfileStore.DeleteSaveData();
            string summary = deleted.Count == 0 ? "không có file nào" : string.Join(", ", deleted);
            Debug.Log("[Editor] Clear save data: deleted=" + summary);
            EditorUtility.DisplayDialog("Clear Save Data", "Đã xóa: " + summary, "OK");
        }

        // While playing, the running session still holds the profile and would save it back; use the Dev panel's
        // "Chơi lại từ đầu" instead.
        [MenuItem(MenuPath, true)]
        private static bool CanClearSaveData()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
