using System.Collections.Generic;
using System.Linq;
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
            UiButton firstRow = null;
            foreach (string category in categories)
            {
                UiButton row = CreateRow(categoryContent, categoryRowTemplate, category);
                if (firstRow == null)
                    firstRow = row;
                row.onClick.AddListener(() => ShowCategory(category, row.GetComponent<Image>()));
            }
            if (firstRow != null)
                ShowCategory(categories[0], firstRow.GetComponent<Image>());
        }

        private void ShowCategory(string category, Image categoryRow)
        {
            SetSelected(ref selectedCategoryRow, categoryRow);
            ClearRows(entryContent, entryRowTemplate.transform);
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
            SetSelected(ref selectedEntryRow, entryRow);
            selectedName.text = entry.Name;
            selectedDescription.text = entry.Description;
        }

        private static UiButton CreateRow(Transform parent, UiButton template, string label)
        {
            UiButton row = Instantiate(template, parent);
            row.name = label;
            row.gameObject.SetActive(true);
            row.SetLabel(label);
            return row;
        }

        private static void SetSelected(ref Image current, Image next)
        {
            if (current != null)
                current.color = NormalRowColor;
            current = next;
            if (current != null)
                current.color = SelectedRowColor;
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
