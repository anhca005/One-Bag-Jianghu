using System.Collections.Generic;
using System.Linq;
using OneBagJianghu.Audio;
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
        private Text selectedName;
        [SerializeField]
        private Text selectedDescription;
        [SerializeField]
        private UiButton closeButton;
        private EncyclopediaCatalog catalog;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            AudioManager.Instance?.Play("ui_open");
            catalog = data as EncyclopediaCatalog ?? EncyclopediaCatalog.Load();
            RebuildCategories();
        }

        private void RebuildCategories()
        {
            Clear(categoryContent);
            List<string> categories = catalog.Entries.Select(value => value.Category).Distinct().ToList();
            foreach (string category in categories)
                CreateButton(categoryContent, category, () => ShowCategory(category));
            if (categories.Count > 0)
                ShowCategory(categories[0]);
        }

        private void ShowCategory(string category)
        {
            Clear(entryContent);
            List<EncyclopediaEntry> entries = catalog.Entries
                .Where(value => value.Category == category)
                .ToList();
            foreach (EncyclopediaEntry entry in entries)
                CreateButton(entryContent, entry.Name, () => Select(entry));
            if (entries.Count > 0)
                Select(entries[0]);
        }

        private void Select(EncyclopediaEntry entry)
        {
            AudioManager.Instance?.Play("ui_click");
            selectedName.text = entry.Name;
            selectedDescription.text = entry.Description;
        }

        private static void CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
        {
            GameObject root = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            root.transform.SetParent(parent, false);
            root.GetComponent<Image>().color = new Color(.83f, .77f, .64f, .85f);
            Button button = root.GetComponent<Button>();
            button.onClick.AddListener(action);
            LayoutElement layout = root.AddComponent<LayoutElement>();
            layout.preferredHeight = 58f;
            Text text = new GameObject("Label", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            text.transform.SetParent(root.transform, false);
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(.12f, .1f, .08f);
            text.text = label;
            RectTransform rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(8f, 4f);
            rect.offsetMax = new Vector2(-8f, -4f);
        }

        private static void Clear(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }
    }
}
