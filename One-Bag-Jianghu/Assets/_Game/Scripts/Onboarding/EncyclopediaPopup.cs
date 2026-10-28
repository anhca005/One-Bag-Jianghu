using System.Collections.Generic;
using System.Linq;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Onboarding
{
    public sealed class EncyclopediaPopup : BasePopup
    {
        [SerializeField]
        private RectTransform categoryContent;
        [SerializeField]
        private RectTransform entryContent;
        [SerializeField]
        private UiButton categoryRowTemplate;
        [SerializeField]
        private UiButton entryRowTemplate;
        [SerializeField]
        private Text selectedName;
        [SerializeField]
        private Text selectedDescription;
        [SerializeField]
        private UiButton closeButton;
        [SerializeField]
        private UiButton replayButton;
        [SerializeField]
        private Sprite normalRowSprite;
        [SerializeField]
        private Sprite selectedRowSprite;
        [SerializeField]
        private Sprite itemsCategoryIcon;
        [SerializeField]
        private Sprite statusCategoryIcon;
        [SerializeField]
        private Sprite storyCategoryIcon;
        [SerializeField]
        private Sprite rulesCategoryIcon;
        private EncyclopediaCatalog catalog;
        private Image selectedCategoryRow;
        private Image selectedEntryRow;
        // Rows are plain paper strips; the builder uses NormalRowColor for the template.
        public static readonly Color NormalRowColor = new Color(.24f, .18f, .12f, .08f);
        private static readonly Color SelectedRowColor = new Color(.86f, .64f, .25f, .55f);

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
            replayButton.onClick.AddListener(ReplaySelectedStory);
        }

        protected override void OnSetup(object data)
        {
            catalog = data as EncyclopediaCatalog ?? EncyclopediaCatalog.Load();
            RebuildCategories();
        }

        private void RebuildCategories()
        {
            ClearRows(categoryContent, categoryRowTemplate.transform);
            List<string> categories = catalog.Entries.Select(value => value.Category).Distinct().ToList();
            categories.Add("Cốt truyện");
            UiButton firstRow = null;
            foreach (string category in categories)
            {
                UiButton row = CreateRow(categoryContent, categoryRowTemplate, category);
                AddCategoryIcon(row, CategoryIcon(category));
                if (firstRow == null)
                    firstRow = row;
                row.onClick.AddListener(() => ShowCategory(category, row.GetComponent<Image>()));
            }
            if (firstRow != null)
                ShowCategory(categories[0], firstRow.GetComponent<Image>());
        }

        private void ShowCategory(string category, Image categoryRow)
        {
            SetSelected(ref selectedCategoryRow, categoryRow, normalRowSprite, selectedRowSprite);
            ClearRows(entryContent, entryRowTemplate.transform);
            if (category == "Cốt truyện")
            {
                ShowStoryCategory();
                return;
            }

            replayButton.gameObject.SetActive(false);
            List<EncyclopediaEntry> entries = catalog.Entries
                .Where(value => value.Category == category)
                .ToList();
            UiButton firstRow = null;
            foreach (EncyclopediaEntry entry in entries)
            {
                UiButton row = CreateRow(entryContent, entryRowTemplate, entry.Name);
                if (firstRow == null)
                    firstRow = row;
                row.onClick.AddListener(() => Select(entry, row.GetComponent<Image>()));
            }
            if (firstRow != null)
                Select(entries[0], firstRow.GetComponent<Image>());
        }

        private void Select(EncyclopediaEntry entry, Image entryRow)
        {
            SetSelected(ref selectedEntryRow, entryRow, normalRowSprite, selectedRowSprite);
            selectedName.text = entry.Name;
            selectedDescription.text = entry.Description;
        }

        private StoryChapter selectedStory;

        public static List<StoryChapter> BuildStoryList(PlayerProfile profile)
        {
            return StoryProgress.UnlockedChapters(StoryCatalog.Load(), PveCatalog.Load(), profile);
        }

        private void ShowStoryCategory()
        {
            List<StoryChapter> stories = BuildStoryList(GameBootstrap.Session.Profile);
            selectedStory = null;
            replayButton.gameObject.SetActive(false);
            selectedName.text = stories.Count == 0 ? "Chưa có hồi ức" : string.Empty;
            selectedDescription.text = stories.Count == 0
                ? "Chưa có hồi ức cốt truyện nào được ghi chép. Hãy hành tẩu giang hồ để mở khoá!"
                : string.Empty;
            UiButton firstRow = null;
            foreach (StoryChapter story in stories)
            {
                UiButton row = CreateRow(entryContent, entryRowTemplate, story.Title);
                if (firstRow == null)
                    firstRow = row;
                row.onClick.AddListener(() => SelectStory(story, row.GetComponent<Image>()));
            }

            if (firstRow != null)
                SelectStory(stories[0], firstRow.GetComponent<Image>());
        }

        private void SelectStory(StoryChapter story, Image entryRow)
        {
            selectedStory = story;
            SetSelected(ref selectedEntryRow, entryRow, normalRowSprite, selectedRowSprite);
            selectedName.text = story.Title;
            PveChapter chapter = PveCatalog.Load().Chapters.Find(value => value.Index == story.Chapter);
            selectedDescription.text = chapter == null
                ? "Khởi đầu hành trình của người mang Cửu Cực Càn Khôn Nang."
                : chapter.Summary;
            replayButton.gameObject.SetActive(true);
        }

        private void ReplaySelectedStory()
        {
            if (selectedStory != null)
                PopupManager.Show<StoryPopup>(selectedStory);
        }

        private static UiButton CreateRow(Transform parent, UiButton template, string label)
        {
            UiButton row = Instantiate(template, parent);
            row.name = label;
            row.gameObject.SetActive(true);
            row.SetLabel(label);
            return row;
        }

        private Sprite CategoryIcon(string category)
        {
            if (category == "Cốt truyện")
                return storyCategoryIcon;
            if (category.IndexOf("Trạng thái", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                category.IndexOf("Hiệu ứng", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return statusCategoryIcon;
            if (category.IndexOf("vật phẩm", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                category.IndexOf("hiếm", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                category.IndexOf("nguyên liệu", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return itemsCategoryIcon;
            if (category.IndexOf("Hệ thống", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                category.IndexOf("Luật", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return rulesCategoryIcon;
            return rulesCategoryIcon;
        }

        private static void AddCategoryIcon(UiButton row, Sprite sprite)
        {
            if (sprite == null)
                return;
            Image icon = new GameObject("Category Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image))
                .GetComponent<Image>();
            icon.transform.SetParent(row.transform, false);
            icon.sprite = sprite;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            RectTransform rect = icon.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0f, .5f);
            rect.pivot = new Vector2(0f, .5f);
            rect.anchoredPosition = new Vector2(12f, 0f);
            rect.sizeDelta = new Vector2(30f, 30f);
            Text label = row.GetComponentInChildren<Text>();
            if (label != null)
                label.rectTransform.offsetMin = new Vector2(48f, label.rectTransform.offsetMin.y);
        }

        private static void SetSelected(ref Image current, Image next, Sprite normalSprite, Sprite selectedSprite)
        {
            if (current != null)
            {
                current.sprite = normalSprite != null ? normalSprite : current.sprite;
                current.color = normalSprite != null ? Color.white : NormalRowColor;
            }
            current = next;
            if (current != null)
            {
                current.sprite = selectedSprite != null ? selectedSprite : current.sprite;
                current.color = selectedSprite != null ? Color.white : SelectedRowColor;
            }
        }

        private static void ClearRows(Transform parent, Transform template)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child != template)
                    Destroy(child.gameObject);
            }
        }
    }
}
