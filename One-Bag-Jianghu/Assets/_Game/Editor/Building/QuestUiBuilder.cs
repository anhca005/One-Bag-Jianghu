using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Onboarding;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.EditorTools.Building
{
    public static class QuestUiBuilder
    {
        private const string HomeFolder = "Assets/_Game/Resources/UI/Home";
        private const string PopupFolder = "Assets/_Game/Resources/UI/Popups";
        private static readonly Vector2 TrackerSize = new Vector2(440f, 128f);
        private static readonly Vector2 RowSize = new Vector2(820f, 150f);

        [MenuItem("One Bag Jianghu/Build/Quest UI (Overwrite)")]
        public static void BuildOverwrite()
        {
            CommonUiPrefabBuilder.BuildMissing();
            UiBuild.EnsureFolder(HomeFolder);
            UiBuild.EnsureFolder(PopupFolder);
            BuildTracker();
            BuildLog();
            AssetDatabase.SaveAssets();
        }

        private static void BuildTracker()
        {
            var root = new GameObject("QuestTracker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(24f, -236f);
            rect.sizeDelta = TrackerSize;
            Image panel = root.GetComponent<Image>();
            panel.color = UiBuild.Paper;
            UiBuild.ApplyUiSprite(panel, "Kit/quest_tracker", Image.Type.Simple);
            QuestTracker tracker = root.AddComponent<QuestTracker>();
            root.AddComponent<TutorialTarget>().Configure("home_quest");
            Button body = root.AddComponent<Button>();
            body.targetGraphic = panel;

            // Round slot of quest_tracker (centre x 121/880, diameter 180/256): the scroll icon sits in it.
            Image icon = AddImage(root.transform, "Quest Icon", new Vector2(.1375f, .5f), new Vector2(58f, 58f));
            UiBuild.ApplyUiSprite(icon, "Kit/quest_icon_scroll", Image.Type.Simple);
            Text title = AddText(root.transform, "Title", "Hồi 1 · Nhiệm vụ", 22,
                new Vector2(.27f, .52f), new Vector2(.79f, .84f), TextAnchor.MiddleLeft);
            Text objective = AddText(root.transform, "Objective", "Mục tiêu  0/1", 17,
                new Vector2(.27f, .18f), new Vector2(.8f, .52f), TextAnchor.MiddleLeft);

            // Quest kind tag: the game writes CHÍNH/PHỤ on it.
            Image badge = AddImage(root.transform, "Kind Badge", new Vector2(.85f, .69f), new Vector2(72f, 30f));
            Sprite mainBadge = UiBuild.LoadUiSprite("Kit/quest_badge_main");
            Sprite sideBadge = UiBuild.LoadUiSprite("Kit/quest_badge_side");
            badge.sprite = mainBadge;
            Text kind = AddText(badge.transform, "Label", "CHÍNH", 12, Vector2.zero, Vector2.one,
                TextAnchor.MiddleCenter);
            kind.color = UiBuild.Paper;

            // Small log button: the full Hành Tẩu Lục can always be opened.
            Button log = AddPlainButton(root.transform, "Log Button", "☰", new Vector2(.88f, .29f),
                new Vector2(44f, 34f), false);
            UiBorderRunner runner = AddRunner(root.transform, panel, TrackerSize);
            UiBuild.Set(tracker, "titleText", title);
            UiBuild.Set(tracker, "objectiveText", objective);
            UiBuild.Set(tracker, "kindText", kind);
            UiBuild.Set(tracker, "badgeImage", badge);
            UiBuild.Set(tracker, "mainBadgeSprite", mainBadge);
            UiBuild.Set(tracker, "sideBadgeSprite", sideBadge);
            UiBuild.Set(tracker, "bodyButton", body);
            UiBuild.Set(tracker, "logButton", log);
            UiBuild.Set(tracker, "runner", runner);
            PrefabUtility.SaveAsPrefabAsset(root, HomeFolder + "/QuestTracker.prefab");
            Object.DestroyImmediate(root);
        }

        private static void BuildLog()
        {
            var root = new GameObject("QuestLogPopup", typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas),
                typeof(GraphicRaycaster));
            UiBuild.Stretch(root.GetComponent<RectTransform>());
            QuestLogPopup popup = root.AddComponent<QuestLogPopup>();
            Image panelImage = UiBuild.Image(root.transform, "Panel", UiBuild.Paper);
            RectTransform panel = panelImage.rectTransform;
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f);
            panel.sizeDelta = new Vector2(940f, 920f);
            UiBuild.ApplyUiSprite(panelImage, "Kit/popup_frame", Image.Type.Simple);
            AddText(panel, "Title", "Hành Tẩu Lục", 40, new Vector2(.08f, .89f),
                new Vector2(.92f, .97f), TextAnchor.MiddleCenter);

            // Scroll view holding one structured row per quest.
            Image viewportImage = UiBuild.Image(panel, "Viewport", new Color(0f, 0f, 0f, .04f));
            UiBuild.Anchors(viewportImage.rectTransform, new Vector2(.05f, .12f), new Vector2(.95f, .88f));
            viewportImage.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform content = UiBuild.Rect(viewportImage.transform, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(.5f, 1f);
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10f;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect scroll = viewportImage.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewportImage.rectTransform;
            scroll.content = content;
            scroll.horizontal = false;
            QuestLogRow template = BuildRow(content);
            template.gameObject.SetActive(false);
            UiButton close = AddUiButton(panel, "Close Button", "Đóng", new Vector2(.5f, .07f),
                new Vector2(200f, 58f));
            close.gameObject.AddComponent<TutorialTarget>().Configure("quest_log_close");
            UiBuild.Set(popup, "content", content);
            UiBuild.Set(popup, "rowTemplate", template);
            UiBuild.Set(popup, "closeButton", close);
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, PopupFolder + "/QuestLogPopup.prefab");
            Object.DestroyImmediate(root);
        }

        private static QuestLogRow BuildRow(Transform parent)
        {
            Image paper = AddImage(parent, "Quest Row Template", new Vector2(.5f, .5f), RowSize);
            UiBuild.ApplyUiSprite(paper, "Kit/quest_log_row", Image.Type.Simple);
            QuestLogRow row = paper.gameObject.AddComponent<QuestLogRow>();
            Image status = AddImage(paper.transform, "Status", new Vector2(.075f, .5f), new Vector2(56f, 56f));
            Image badge = AddImage(paper.transform, "Badge", new Vector2(.19f, .78f), new Vector2(72f, 30f));
            Text badgeText = AddText(badge.transform, "Label", "CHÍNH", 11, Vector2.zero, Vector2.one,
                TextAnchor.MiddleCenter);
            badgeText.color = UiBuild.Paper;
            Text title = AddText(paper.transform, "Title", "Tên nhiệm vụ", 20,
                new Vector2(.25f, .68f), new Vector2(.64f, .9f), TextAnchor.MiddleLeft);
            Text giver = AddText(paper.transform, "Giver", "Người giao", 14,
                new Vector2(.68f, .7f), new Vector2(.93f, .88f), TextAnchor.MiddleRight);
            Text description = AddText(paper.transform, "Text", "Lời dẫn nhiệm vụ", 14,
                new Vector2(.17f, .38f), new Vector2(.72f, .67f), TextAnchor.MiddleLeft);
            Text objective = AddText(paper.transform, "Objective", "Mục tiêu 0/1", 14,
                new Vector2(.17f, .12f), new Vector2(.58f, .38f), TextAnchor.MiddleLeft);
            RectTransform rewards = AddRewards(paper.transform);
            UiButton claim = AddUiButton(paper.transform, "Claim Button", "Nhận thưởng", new Vector2(.83f, .25f),
                new Vector2(150f, 42f));
            UiBorderRunner runner = AddRunner(paper.transform, paper, RowSize);
            SetRowFields(row, status, badge, badgeText, title, giver, description, objective, rewards, claim, runner);
            return row;
        }

        private static RectTransform AddRewards(Transform parent)
        {
            RectTransform rewards = UiBuild.Rect(parent, "Rewards");
            UiBuild.Anchors(rewards, new Vector2(.58f, .08f), new Vector2(.78f, .38f));
            HorizontalLayoutGroup layout = rewards.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            for (int i = 0; i < 6; i++)
            {
                RectTransform entry = UiBuild.Rect(rewards, "Reward " + i);
                entry.sizeDelta = new Vector2(58f, 34f);
                Image icon = AddImage(entry, "Icon", new Vector2(.2f, .5f), new Vector2(26f, 26f));
                icon.color = Color.clear;
                AddText(entry, "Value", "0", 12, new Vector2(.42f, 0f), Vector2.one, TextAnchor.MiddleLeft);
            }
            return rewards;
        }

        private static void SetRowFields(
            QuestLogRow row,
            Image status,
            Image badge,
            Text badgeText,
            Text title,
            Text giver,
            Text description,
            Text objective,
            RectTransform rewards,
            UiButton claim,
            UiBorderRunner runner)
        {
            UiBuild.Set(row, "statusIcon", status);
            UiBuild.Set(row, "badgeImage", badge);
            UiBuild.Set(row, "badgeText", badgeText);
            UiBuild.Set(row, "titleText", title);
            UiBuild.Set(row, "giverText", giver);
            UiBuild.Set(row, "descriptionText", description);
            UiBuild.Set(row, "objectiveText", objective);
            UiBuild.Set(row, "rewards", rewards);
            UiBuild.Set(row, "claimButton", claim);
            UiBuild.Set(row, "runner", runner);
            UiBuild.Set(row, "doneSprite", UiBuild.LoadUiSprite("Kit/quest_status_done"));
            UiBuild.Set(row, "activeSprite", UiBuild.LoadUiSprite("Kit/quest_status_active"));
            UiBuild.Set(row, "lockedSprite", UiBuild.LoadUiSprite("Kit/quest_status_locked"));
            UiBuild.Set(row, "mainBadgeSprite", UiBuild.LoadUiSprite("Kit/quest_badge_main"));
            UiBuild.Set(row, "sideBadgeSprite", UiBuild.LoadUiSprite("Kit/quest_badge_side"));
        }

        private static UiBorderRunner AddRunner(Transform parent, Image frame, Vector2 size)
        {
            RectTransform layer = UiBuild.Rect(parent, "Border Runner");
            UiBuild.Stretch(layer);
            UiBorderRunner runner = layer.gameObject.AddComponent<UiBorderRunner>();
            var lights = new RectTransform[4];
            for (int i = 0; i < lights.Length; i++)
            {
                Image light = AddImage(layer, "Light " + i, new Vector2(.5f, .5f), new Vector2(48f, 48f));
                UiBuild.ApplyUiSprite(light, "Kit/quest_glow_spark", Image.Type.Simple);
                Color color = Color.white;
                color.a = 1f - i * .22f;
                light.color = color;
                light.raycastTarget = false;
                lights[i] = light.rectTransform;
            }
            var serialized = new SerializedObject(runner);
            serialized.FindProperty("frame").objectReferenceValue = frame;
            SerializedProperty array = serialized.FindProperty("lights");
            array.arraySize = lights.Length;
            for (int i = 0; i < lights.Length; i++)
                array.GetArrayElementAtIndex(i).objectReferenceValue = lights[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            runner.SetRunning(false);
            return runner;
        }

        private static Image AddImage(Transform parent, string name, Vector2 anchor, Vector2 size)
        {
            Image image = UiBuild.Image(parent, name, Color.white);
            RectTransform rect = image.rectTransform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = size;
            image.preserveAspect = true;
            return image;
        }

        private static Text AddText(
            Transform parent,
            string name,
            string value,
            int size,
            Vector2 min,
            Vector2 max,
            TextAnchor alignment)
        {
            Text text = UiBuild.Text(parent, name, value, size, alignment);
            UiBuild.Anchors(text.rectTransform, min, max);
            text.color = UiBuild.Ink;
            return text;
        }

        private static Button AddPlainButton(
            Transform parent,
            string name,
            string label,
            Vector2 anchor,
            Vector2 size,
            bool primary)
        {
            Button button = UiBuild.Button(parent, name, label, primary);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = size;
            return button;
        }

        private static UiButton AddUiButton(
            Transform parent,
            string name,
            string label,
            Vector2 anchor,
            Vector2 size)
        {
            return AddPlainButton(parent, name, label, anchor, size, true).GetComponent<UiButton>();
        }
    }
}
