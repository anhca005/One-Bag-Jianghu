using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class PopupPrefabBuilder
    {
        private const string Folder = "Assets/_Game/Resources/UI/Popups";

        [MenuItem("One Bag Jianghu/Build Popup Prefabs")]
        public static void BuildPopupPrefabs() => Build(false);

        public static void BuildMissing() => Build(false);
        public static void BuildOverwrite() => Build(true);

        public static void Build(bool overwrite)
        {
            CommonUiPrefabBuilder.Build(false);
            BagPrefabBuilder.Build(false);
            UiBuild.EnsureFolder(Folder);
            BuildMessage(overwrite);
            BuildConfirm(overwrite);
            BuildSettings(overwrite);
            BuildBag(overwrite);
            BuildResult(overwrite);
            AssetDatabase.SaveAssets();
        }

        private static void BuildMessage(bool overwrite)
        {
            Build<MessagePopup>(new Vector2(720f, 420f), overwrite, (popup, panel) =>
            {
                Text title = AddText(panel, "Title", "Thông báo", 40, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.92f));
                Text message = AddText(panel, "Message", "Nội dung thông báo", 25, new Vector2(0.08f, 0.3f), new Vector2(0.92f, 0.7f));
                UiButton close = AddButton(panel, "Close Button", "Đóng", true, new Vector2(0.5f, 0.14f));
                UiBuild.Set(popup, "titleText", title); UiBuild.Set(popup, "messageText", message); UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildConfirm(bool overwrite)
        {
            Build<ConfirmPopup>(new Vector2(760f, 440f), overwrite, (popup, panel) =>
            {
                Text title = AddText(panel, "Title", "Xác nhận", 40, new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.92f));
                Text message = AddText(panel, "Message", "Bạn có chắc chắn?", 25, new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.7f));
                UiButton cancel = AddButton(panel, "Cancel Button", "Hủy", false, new Vector2(0.28f, 0.14f));
                UiButton confirm = AddButton(panel, "Confirm Button", "Đồng ý", true, new Vector2(0.72f, 0.14f));
                UiBuild.Set(popup, "titleText", title); UiBuild.Set(popup, "messageText", message);
                UiBuild.Set(popup, "cancelButton", cancel); UiBuild.Set(popup, "confirmButton", confirm);
            });
        }

        private static void BuildSettings(bool overwrite)
        {
            Build<SettingsPopup>(new Vector2(720f, 430f), overwrite, (popup, panel) =>
            {
                AddText(panel, "Title", "Cài đặt", 42, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.92f));
                Toggle toggle = UiBuild.InstantiateLinked<Toggle>(CommonUiPrefabBuilder.Folder + "/Toggle.prefab", panel);
                toggle.name = "Fullscreen Toggle";
                RectTransform toggleRect = toggle.GetComponent<RectTransform>();
                toggleRect.anchorMin = toggleRect.anchorMax = new Vector2(0.32f, 0.5f);
                toggleRect.sizeDelta = new Vector2(48f, 48f);
                AddText(panel, "Fullscreen Label", "Toàn màn hình", 27, new Vector2(0.39f, 0.42f), new Vector2(0.8f, 0.58f), TextAnchor.MiddleLeft);
                UiButton close = AddButton(panel, "Close Button", "Đóng", false, new Vector2(0.5f, 0.16f));
                UiBuild.Set(popup, "fullscreenToggle", toggle); UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildBag(bool overwrite)
        {
            Build<BagPopup>(new Vector2(1700f, 960f), overwrite, (popup, panel) =>
            {
                UiBuild.SetBool(popup, "closeOnBackdrop", false);
                AddText(panel, "Title", "Bao Phục", 40, new Vector2(0.35f, 0.9f), new Vector2(0.65f, 0.98f));
                RectTransform host = UiBuild.Rect(panel, "BagHost");
                UiBuild.Anchors(host, new Vector2(0.03f, 0.1f), new Vector2(0.97f, 0.89f));
                BagLoadoutPanel bagPanel = UiBuild.InstantiateLinked<BagLoadoutPanel>(BagPrefabBuilder.Folder + "/BagLoadoutPanel.prefab", host);
                UiBuild.Stretch((RectTransform)bagPanel.transform);
                UiBuild.SetBool(bagPanel, "showSectSelector", true);
                GameObject tooltipRoot = new GameObject("Item Tooltip", typeof(RectTransform), typeof(Canvas), typeof(ItemTooltip));
                tooltipRoot.transform.SetParent(popup.transform, false); UiBuild.Stretch(tooltipRoot.GetComponent<RectTransform>());
                Canvas tooltipCanvas = tooltipRoot.GetComponent<Canvas>(); tooltipCanvas.overrideSorting = true;
                Image tooltipPanel = UiBuild.Image(tooltipRoot.transform, "Paper", UiBuild.Paper);
                UiBuild.ApplyUiSprite(tooltipPanel, "frame_tooltip", Image.Type.Sliced);
                RectTransform tooltipRect = tooltipPanel.rectTransform; tooltipRect.anchorMin = tooltipRect.anchorMax = Vector2.zero;
                tooltipRect.pivot = new Vector2(0f, 1f); tooltipRect.sizeDelta = new Vector2(440f, 420f);
                RectTransform tooltipViewport = UiBuild.Image(tooltipPanel.transform, "Viewport", Color.clear).rectTransform; tooltipViewport.anchorMin=Vector2.zero;tooltipViewport.anchorMax=Vector2.one;tooltipViewport.offsetMin=new Vector2(ItemTooltip.HorizontalInset,ItemTooltip.BottomInset);tooltipViewport.offsetMax=new Vector2(-ItemTooltip.HorizontalInset,-ItemTooltip.TopInset);tooltipViewport.gameObject.AddComponent<RectMask2D>();
                RectTransform tooltipContent = UiBuild.Rect(tooltipViewport, "Content"); tooltipContent.anchorMin = new Vector2(0f, 1f); tooltipContent.anchorMax = new Vector2(1f, 1f); tooltipContent.pivot = new Vector2(.5f, 1f); tooltipContent.sizeDelta = Vector2.zero;
                VerticalLayoutGroup tooltipLayout = tooltipContent.gameObject.AddComponent<VerticalLayoutGroup>(); tooltipLayout.childControlHeight = true; tooltipLayout.childControlWidth = true; tooltipLayout.childForceExpandHeight = false; tooltipLayout.childForceExpandWidth = true;
                ContentSizeFitter tooltipFitter = tooltipContent.gameObject.AddComponent<ContentSizeFitter>(); tooltipFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                ScrollRect tooltipScroll = tooltipPanel.gameObject.AddComponent<ScrollRect>(); tooltipScroll.viewport = tooltipViewport; tooltipScroll.content = tooltipContent; tooltipScroll.horizontal = false; tooltipScroll.vertical = true; tooltipScroll.movementType = ScrollRect.MovementType.Clamped;
                Text tooltipText = UiBuild.Text(tooltipContent, "Description", string.Empty, 18, TextAnchor.UpperLeft);
                tooltipText.color = UiBuild.Ink; tooltipText.supportRichText = true; tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap; tooltipText.verticalOverflow = VerticalWrapMode.Overflow;
                tooltipText.resizeTextForBestFit = false; tooltipText.resizeTextMinSize = 16; tooltipText.resizeTextMaxSize = 18;
                tooltipText.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                ItemTooltip tooltip = tooltipRoot.GetComponent<ItemTooltip>(); UiBuild.Set(tooltip, "overlayCanvas", tooltipCanvas); UiBuild.Set(tooltip, "panel", tooltipRect); UiBuild.Set(tooltip, "descriptionText", tooltipText);
                UiButton save = AddButton(panel, "Save Button", "Lưu Bao Phục", true, new Vector2(0.38f, 0.045f));
                UiButton close = AddButton(panel, "Close Button", "Đóng", false, new Vector2(0.62f, 0.045f));
                UiBuild.Set(popup, "bagHost", host); UiBuild.Set(popup, "bagPanel", bagPanel); UiBuild.Set(popup, "itemTooltip", tooltip); UiBuild.Set(popup, "saveButton", save); UiBuild.Set(popup, "closeButton", close);
            });
        }

        private static void BuildResult(bool overwrite)
        {
            Build<ResultPopup>(new Vector2(760f, 500f), overwrite, (popup, panel) =>
            {
                UiBuild.SetBool(popup, "closeOnEscape", false); UiBuild.SetBool(popup, "closeOnBackdrop", false);
                Image seal = UiBuild.Image(panel, "Title Seal", Color.clear);
                UiBuild.Anchors(seal.rectTransform, new Vector2(0.39f, 0.61f), new Vector2(0.61f, 0.95f));
                seal.preserveAspect = true; seal.raycastTarget = false; UiBuild.ApplyUiSprite(seal, "seal_title");
                Text result = AddText(panel, "Result Text", "Thắng", 58, new Vector2(0.05f, 0.64f), new Vector2(0.95f, 0.91f));
                Text winner = AddText(panel, "Winner", "Người thắng", 26, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.65f));
                Text rewards = AddText(panel, "Rewards", "", 22, new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.54f));
                Text reason = AddText(panel, "Disabled Reason", "", 18, new Vector2(0.08f, 0.34f), new Vector2(0.92f, 0.42f)); reason.color = UiBuild.Vermilion;
                UiButton home = AddButton(panel, "Home Button", "Về trấn", false, new Vector2(0.5f, 0.3f));
                UiButton again = AddButton(panel, "Play Again Button", "Đấu tiếp", true, new Vector2(0.5f, 0.13f));
                UiBuild.Set(popup, "resultText", result); UiBuild.Set(popup, "winnerText", winner); UiBuild.Set(popup, "rewardText", rewards);
                UiBuild.Set(popup, "homeButton", home); UiBuild.Set(popup, "playAgainButton", again); UiBuild.Set(popup, "disabledReason", reason);
            });
        }

        private static void Build<T>(Vector2 size, bool overwrite, System.Action<T, RectTransform> content) where T : BasePopup
        {
            string path = Folder + "/" + typeof(T).Name + ".prefab";
            if (!AssetGuard.ShouldBuild(path, overwrite)) return;
            var root = new GameObject(typeof(T).Name, typeof(RectTransform), typeof(CanvasGroup));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            root.AddComponent<GraphicRaycaster>();
            T popup = root.AddComponent<T>();
            Image panelImage = UiBuild.InstantiateLinked<Image>(CommonUiPrefabBuilder.Folder + "/Panel_Paper.prefab", root.transform);
            panelImage.name = "Panel";
            RectTransform panel = panelImage.rectTransform;
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = size;
            UiBuild.Set(popup, "panel", panel);
            content(popup, panel);
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static Text AddText(Transform parent, string name, string value, int size, Vector2 min, Vector2 max, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            UiFontRole role = name.IndexOf("Title", System.StringComparison.OrdinalIgnoreCase) >= 0 || name == "Result Text"
                ? UiFontRole.Display : UiFontRole.Body;
            Text text = UiBuild.Text(parent, name, value, size, alignment, role);
            UiBuild.Anchors(text.rectTransform, min, max);
            return text;
        }

        private static UiButton AddButton(Transform parent, string name, string label, bool primary, Vector2 anchor)
        {
            string prefab = primary ? "Button_Primary.prefab" : "Button_Secondary.prefab";
            UiButton button = UiBuild.InstantiateLinked<UiButton>(CommonUiPrefabBuilder.Folder + "/" + prefab, parent);
            button.name = name;
            button.SetLabel(label);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(280f, 64f);
            return button;
        }
    }
}
