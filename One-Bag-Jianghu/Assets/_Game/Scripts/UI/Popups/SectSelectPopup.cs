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
        private int selectedIndex;
        private bool actionPending;
        private PlayerProfile Profile => GameBootstrap.Session.Profile;

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
            if (string.IsNullOrWhiteSpace(Profile.SectId))
            {
                return 0;
            }

            for (int i = 0; i < catalog.All.Count; i++)
            {
                if (catalog.All[i].Id == Profile.SectId)
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
                cardBadges[i].text = sect.Id == Profile.SectId ? "Bổn môn" : string.Empty;
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
            tamPhapLevel.text = string.IsNullOrWhiteSpace(Profile.SectId)
                ? string.Empty
                : "Tầng " + Profile.TamPhapLevel + "/9";
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
                bool locked = !string.IsNullOrWhiteSpace(Profile.SectId)
                    && (sect.Id != Profile.SectId || Profile.TamPhapLevel < milestone.Level);
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
            bool joined = !string.IsNullOrWhiteSpace(Profile.SectId);
            bool ownSect = joined && sect.Id == Profile.SectId;
            primaryButton.gameObject.SetActive(!joined || ownSect);
            primaryButton.SetLabel(joined ? "Đã Nhập Phái" : "Gia Nhập");
            primaryButton.GetComponent<Button>().interactable = !actionPending
                && IsPrimaryActionEnabled(Profile, sect);
            Text hint = primaryButton.transform.parent.Find("Primary Hint")?.GetComponent<Text>();
            if (hint != null)
            {
                hint.gameObject.SetActive(joined);
                hint.text = joined ? "Nâng Tâm Pháp sẽ mở trong bản cập nhật sau." : string.Empty;
            }
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
            if (!string.IsNullOrWhiteSpace(Profile.SectId))
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

            PlayerProfile preview = PlayerProfile.FromJson(Profile.ToJson());
            if (!TryJoin(preview, catalog, sect.Id, out string error))
            {
                PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", error, "Đóng"));
                return;
            }

            SetActionPending(true);
            GameBootstrap.Network.ChooseSect(sect.Id,
                _ => CloseWithResult(sect.Id),
                HandleJoinError);
        }

        private void HandleJoinError(string message)
        {
            SetActionPending(false);
            PopupManager.Show<MessagePopup>(new MessagePopupData("Thông báo", message, "Đóng"));
        }

        private void SetActionPending(bool pending)
        {
            actionPending = pending;
            primaryButton.GetComponent<Button>().interactable = !pending;
            closeButton.GetComponent<Button>().interactable = !pending;
            foreach (Button cardButton in cardButtons)
            {
                cardButton.interactable = !pending;
            }
        }

        /// <summary>Validates the confirmed sect choice against a disposable profile copy.</summary>
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
