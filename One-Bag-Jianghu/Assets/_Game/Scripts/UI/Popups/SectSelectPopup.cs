using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Popups
{
    public sealed class SectSelectPopup : BasePopup
    {
        [SerializeField] private Button[] cardButtons;
        [SerializeField] private Image[] cardBackgrounds;
        [SerializeField] private Image[] cardArts;
        [SerializeField] private Text[] cardNames;
        [SerializeField] private Text[] cardWeapons;
        [SerializeField] private Text[] cardBadges;
        [SerializeField] private Image heroArt;
        [SerializeField] private Text sectName;
        [SerializeField] private Text loreText;
        [SerializeField] private Text weaponText;
        [SerializeField] private Image tamPhapArt;
        [SerializeField] private Text tamPhapName;
        [SerializeField] private Text tamPhapLevel;
        [SerializeField] private Text[] milestoneTexts;
        [SerializeField] private Image[] milestoneBackgrounds;
        [SerializeField] private UiButton primaryButton;
        [SerializeField] private UiButton closeButton;
        private SectCatalog catalog;
        private SectArtLibrary artLibrary;
        private PlayerProfile profile;
        private int selectedIndex;

        /// <summary>Gets whether the future Tâm Pháp upgrade action is currently interactable.</summary>
        public bool IsUpgradeButtonInteractable => primaryButton.GetComponent<Button>().interactable;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < cardButtons.Length; i++)
            {
                int index = i;
                cardButtons[i].onClick.AddListener(() => Select(index));
            }

            primaryButton.onClick.AddListener(PrimaryAction);
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            catalog = SectCatalog.LoadFromResources();
            artLibrary = SectArtLibrary.Load();
            profile = GameBootstrap.Session.Profile;
            selectedIndex = FindInitialIndex();
            BindCards();
            RefreshSelection();
        }

        protected override void OnShown()
        {
            OnboardingRuntime.Publish("sect_opened");
        }

        protected override void OnHidden()
        {
            OnboardingRuntime.Publish("sect_selected_or_closed");
        }

        private int FindInitialIndex()
        {
            if (string.IsNullOrWhiteSpace(profile.SectId))
            {
                return 0;
            }

            for (int i = 0; i < catalog.All.Count; i++)
            {
                if (catalog.All[i].Id == profile.SectId)
                {
                    return i;
                }
            }

            return 0;
        }

        private void BindCards()
        {
            int count = Mathf.Min(cardButtons.Length, catalog.All.Count);
            for (int i = 0; i < cardButtons.Length; i++)
            {
                bool visible = i < count;
                cardButtons[i].gameObject.SetActive(visible);
                if (!visible)
                {
                    continue;
                }

                SectInfo sect = catalog.All[i];
                cardNames[i].text = sect.Name;
                cardWeapons[i].text = sect.DescribeWeaponClasses();
                cardBadges[i].text = sect.Id == profile.SectId ? "Bổn môn" : string.Empty;
                ApplyArt(cardArts[i], "sect_" + sect.Id, sect.Accent);
            }
        }

        private void Select(int index)
        {
            if (index < 0 || index >= catalog.All.Count)
            {
                return;
            }

            selectedIndex = index;
            RefreshSelection();
        }

        private void RefreshSelection()
        {
            SectInfo sect = catalog.All[selectedIndex];
            for (int i = 0; i < cardBackgrounds.Length; i++)
            {
                Color accent = i < catalog.All.Count ? catalog.All[i].Accent : Color.clear;
                cardBackgrounds[i].color = i == selectedIndex
                    ? new Color(accent.r, accent.g, accent.b, .72f)
                    : new Color(.12f, .09f, .07f, .28f);
            }

            ApplyArt(heroArt, "sect_" + sect.Id, sect.Accent);
            ApplyArt(tamPhapArt, "tam_phap_" + sect.Id, sect.Accent);
            sectName.text = sect.Name;
            loreText.text = sect.Lore;
            weaponText.text = sect.DescribeWeaponClasses();
            tamPhapName.text = sect.TamPhap;
            tamPhapLevel.text = string.IsNullOrWhiteSpace(profile.SectId)
                ? string.Empty
                : "Tầng " + profile.TamPhapLevel + "/9";
            BindMilestones(sect);
            BindActions(sect);
        }

        private void BindMilestones(SectInfo sect)
        {
            for (int i = 0; i < milestoneTexts.Length; i++)
            {
                if (i >= sect.Levels.Count)
                {
                    milestoneTexts[i].text = string.Empty;
                    continue;
                }

                SectLevelInfo milestone = sect.Levels[i];
                milestoneTexts[i].text = "<b>Tầng " + milestone.Level + "</b>  " + milestone.Describe();
                bool locked = !string.IsNullOrWhiteSpace(profile.SectId)
                    && (sect.Id != profile.SectId || profile.TamPhapLevel < milestone.Level);
                milestoneTexts[i].color = locked
                    ? new Color(.38f, .36f, .33f)
                    : new Color(.16f, .12f, .09f);
                milestoneBackgrounds[i].color = locked
                    ? new Color(.25f, .24f, .22f, .12f)
                    : new Color(sect.Accent.r, sect.Accent.g, sect.Accent.b, .14f);
            }
        }

        private void BindActions(SectInfo sect)
        {
            bool joined = !string.IsNullOrWhiteSpace(profile.SectId);
            bool ownSect = joined && sect.Id == profile.SectId;
            primaryButton.gameObject.SetActive(!joined || ownSect);
            primaryButton.SetLabel(joined ? "Nâng Tâm Pháp (Sắp mở)" : "Gia Nhập");
            primaryButton.GetComponent<Button>().interactable = IsPrimaryActionEnabled(profile, sect);
            closeButton.SetLabel(joined ? "Đóng" : "Để sau");
        }

        private void ApplyArt(Image image, string key, Color accent)
        {
            if (artLibrary != null && artLibrary.TryGet(key, out Sprite sprite))
            {
                image.sprite = sprite;
                image.color = Color.white;
                return;
            }

            image.sprite = null;
            image.color = new Color(accent.r * .65f, accent.g * .65f, accent.b * .65f, .9f);
        }

        private void PrimaryAction()
        {
            SectInfo sect = catalog.All[selectedIndex];
            if (!string.IsNullOrWhiteSpace(profile.SectId))
            {
                OpenTamPhapUpgrade(sect);
                return;
            }

            var data = new ConfirmPopupData(
                "Gia Nhập Môn Phái",
                "Lựa chọn này là vĩnh viễn. Gia nhập " + sect.Name + "?",
                "Gia Nhập",
                "Hủy",
                confirmed => CompleteJoin(confirmed, sect));
            PopupManager.Show<ConfirmPopup>(data);
        }

        /// <summary>Joins the selected sect as if the permanent-choice confirmation was accepted.</summary>
        public void JoinSelected()
        {
            CompleteJoin(true, catalog.All[selectedIndex]);
        }

        private void CompleteJoin(bool confirmed, SectInfo sect)
        {
            if (!confirmed)
            {
                return;
            }

            if (!TryJoin(profile, catalog, sect.Id, out string error))
            {
                PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", error, "Đóng"));
                return;
            }

            GameBootstrap.Session.SaveProfile();
            CloseWithResult(sect.Id);
        }

        /// <summary>Runs the profile mutation used by the confirmed popup flow.</summary>
        public static bool TryJoin(
            PlayerProfile targetProfile,
            SectCatalog targetCatalog,
            string sectId,
            out string error)
        {
            var service = new SectChoiceService(targetCatalog);
            return service.TryChoose(targetProfile, sectId, out error);
        }

        /// <summary>Reports whether the primary action is enabled for the displayed sect.</summary>
        public static bool IsPrimaryActionEnabled(PlayerProfile targetProfile, SectInfo displayedSect)
        {
            return string.IsNullOrWhiteSpace(targetProfile.SectId);
        }

        private void OpenTamPhapUpgrade(SectInfo sect)
        {
            // Reserved hook for the future Tâm Pháp upgrade feature.
        }
    }
}
