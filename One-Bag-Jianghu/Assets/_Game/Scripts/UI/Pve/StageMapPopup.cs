using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Onboarding;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Audio;
using OneBagJianghu.Net;

namespace OneBagJianghu.UI.Pve
{
    public sealed class StageMapPopup : BasePopup
    {
        [SerializeField]
        private Image mapBackground,
            header,
            detailFrame,
            staminaIcon,
            recommendedLevelIcon,
            rewardCurrencyIcon;
        [SerializeField]
        private Text chapterText,
            staminaText,
            stageName,
            descriptionText,
            enemyText,
            powerText,
            playerPowerText,
            rewardsText,
            costText,
            disabledReason;
        [SerializeField]
        private Text powerWarningText;
        [SerializeField]
        private Image enemyPortrait;
        [SerializeField]
        private RectTransform nodeLayer;
        [SerializeField]
        private PveStageNode nodePrefab;
        [SerializeField]
        private GameObject[] mapPrefabs;
        [SerializeField]
        private UiButton previousButton, nextButton, challengeButton, closeButton;
        private bool leavingForBattle;
        private readonly List<GameObject> nodes = new List<GameObject>();
        private PveCatalog catalog;
        private StaminaService stamina;
        private PveArtLibrary art;
        private ItemIconLibrary itemArt;
        private int chapterIndex;
        private PveStage selected;
        private GameObject mapInstance;
        private PveMapLayout mapLayout;
        private StaminaConfig staminaConfig;
        private float nextStaminaRefresh;
        private static readonly HashSet<string> warnedLayouts = new HashSet<string>();
        public Color HeaderColor => header.color;
        public PveMapLayout CurrentMapLayout => mapLayout;
        public IReadOnlyList<GameObject> SpawnedNodes => nodes;

        protected override void Awake()
        {
            base.Awake();
            previousButton.onClick.AddListener(() => Switch(-1));
            nextButton.onClick.AddListener(() => Switch(1));
            challengeButton.onClick.AddListener(StartStage);
            closeButton.onClick.AddListener(CloseMap);
            challengeButton.gameObject.AddComponent<TutorialTarget>().Configure("stage_challenge");
            closeButton.gameObject.AddComponent<TutorialTarget>().Configure("stage_map_close");
        }

        protected override void OnSetup(object data)
        {
            Closed -= RestoreHomeMusic;
            Closed += RestoreHomeMusic;
            leavingForBattle = false;
            catalog = PveCatalog.Load();
            staminaConfig = StaminaConfig.Load();
            stamina = new StaminaService(staminaConfig);
            SingleLineFit staminaFit = staminaText.GetComponent<SingleLineFit>();
            if (staminaFit == null)
                staminaFit = staminaText.gameObject.AddComponent<SingleLineFit>();
            staminaFit.Configure(staminaText.fontSize, 14);
            art = PveArtLibrary.Load();
            itemArt = ItemIconLibrary.Load();
            RefreshStamina();
            string selectedStageId = data as string;
            // No stage asked for: open at the player's frontier (first unlocked stage not yet cleared).
            PveStage requested = catalog.GetStage(selectedStageId) ?? (data is int ? null : FrontierStage(null));
            chapterIndex = data is int i ? Mathf.Clamp(i,
                0,
                catalog.Chapters.Count - 1) : requested != null ? catalog.Chapters.IndexOf(requested.Chapter) : 0;
            DrawChapter(requested);
        }

        protected override void OnShown()
        {
            PlayerProfile.ProgressChanged += HandleProgressChanged;
        }

        protected override void OnHidden()
        {
            PlayerProfile.ProgressChanged -= HandleProgressChanged;
        }

        private void Update()
        {
            if (!IsShowing || Time.unscaledTime < nextStaminaRefresh)
                return;
            nextStaminaRefresh = Time.unscaledTime + 1f;
            RefreshStamina();
            if (selected != null)
                SelectStage(selected);
        }

        private void Switch(int delta)
        {
            int target = Mathf.Clamp(chapterIndex + delta, 0, catalog.Chapters.Count - 1);
            if (!PveRules.IsChapterUnlocked(catalog.Chapters[target], catalog, GameBootstrap.Session.Profile))
                return;
            chapterIndex = target;
            DrawChapter(null);
        }

        private void DrawChapter(PveStage requested)
        {
            foreach (GameObject node in nodes)
                Destroy(node);
            nodes.Clear();
            selected = null;
            if (mapInstance != null)
                Destroy(mapInstance);
            mapInstance = null;
            mapLayout = null;
            PveChapter chapter = catalog.Chapters[chapterIndex];
            AudioManager.EnsureExists().PlayMusic(MusicKeys.ForChapter(chapterIndex + 1));
            Color accent = Parse(chapter.Theme.Accent,
                Color.red), paper = Parse(chapter.Theme.Paper,
                new Color(.93f,
                .88f,
                .75f)), ink = Parse(chapter.Theme.Ink,
                Color.black);
            header.color = accent;
            ApplyFrame(detailFrame, "frame_stage_card", paper);
            chapterText.color = paper;
            chapterText.text = chapter.Title + " · " + chapter.Name;
            descriptionText.color = ink;
            GameObject mapPrefab = FindMapPrefab(chapter.Id);
            if (mapPrefab != null)
            {
                mapInstance = Instantiate(mapPrefab, mapBackground.rectTransform, false);
                RectTransform mapRect = (RectTransform)mapInstance.transform;
                mapRect.anchorMin = Vector2.zero;
                mapRect.anchorMax = Vector2.one;
                mapRect.offsetMin = mapRect.offsetMax = Vector2.zero;
                mapRect.SetAsFirstSibling();
                mapLayout = mapInstance.GetComponent<PveMapLayout>();
                mapBackground.sprite = null;
                mapBackground.color = Color.clear;
            }
            else
            {
                mapBackground.sprite = Sprite(chapter.Theme.MapBackground);
                mapBackground.color = mapBackground.sprite == null ? paper : Color.white;
                WarnLayoutOnce(chapter.Id, "map prefab is missing");
            }

            SetOptionalIcon(staminaIcon, "icon_stamina");
            SetOptionalIcon(recommendedLevelIcon, "icon_exp");
            SetOptionalIcon(rewardCurrencyIcon, "icon_bac");
            UpdateStaminaText();
            previousButton.gameObject.SetActive(chapterIndex > 0);
            nextButton.gameObject.SetActive(chapterIndex + 1 < catalog.Chapters.Count);
            if (chapterIndex + 1 < catalog.Chapters.Count)
                nextButton.GetComponent<Button>().interactable =
                    PveRules.IsChapterUnlocked(catalog.Chapters[chapterIndex
                        + 1],
                        catalog,
                        GameBootstrap.Session.Profile);
            foreach (PveStage stage in chapter.Stages)
            {
                RectTransform anchor = mapLayout != null ? mapLayout.FindAnchor(stage.Id) : null;
                PveStageNode node = Instantiate(nodePrefab, anchor != null ? anchor : nodeLayer);
                node.gameObject.SetActive(true);
                RectTransform rect = (RectTransform)node.transform;
                if (anchor != null)
                {
                    rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
                    rect.anchoredPosition = Vector2.zero;
                }
                else
                {
                    rect.anchorMin = rect.anchorMax = new Vector2(stage.MapPos[0], 1f - stage.MapPos[1]);
                    rect.anchoredPosition = Vector2.zero;
                    if (mapPrefab != null)
                        WarnLayoutOnce(chapter.Id + "/" + stage.Id, "anchor '" + stage.Id + "' is missing");
                }

                bool unlocked = PveRules.IsStageUnlocked(stage, catalog, GameBootstrap.Session.Profile);
                bool cleared = GameBootstrap.Session.Profile.Pve.Stages.TryGetValue(stage.Id,
                    out PveStageProgress progress) && progress.Cleared;
                node.Bind(stage, unlocked, cleared, accent, chapter.Theme, art, OnStageClicked);
                nodes.Add(node.gameObject);
            }

            SelectStage(requested != null && requested.Chapter == chapter
                ? requested
                : FrontierStage(chapter) ?? chapter.Stages[0]);
        }

        /// <summary>
        /// The first unlocked, uncleared stage (in <paramref name="chapter"/>, or across all chapters when null).
        /// When everything unlocked is cleared, the last unlocked stage. Null if none is unlocked.
        /// </summary>
        private PveStage FrontierStage(PveChapter chapter)
        {
            PlayerProfile profile = GameBootstrap.Session.Profile;
            PveStage lastUnlocked = null;
            foreach (PveChapter value in catalog.Chapters)
            {
                if (chapter != null && value != chapter)
                    continue;
                foreach (PveStage stage in value.Stages)
                {
                    if (!PveRules.IsStageUnlocked(stage, catalog, profile))
                        continue;
                    if (!PveRules.IsCleared(profile, stage.Id))
                        return stage;
                    lastUnlocked = stage;
                }
            }

            return lastUnlocked;
        }

        private void RestoreHomeMusic(BasePopup popup)
        {
            popup.Closed -= RestoreHomeMusic;
            // Leaving for a battle (pre-battle dialogue, then loading): keep the map music until the battle
            // scene starts its own.
            if (leavingForBattle || SceneFlow.IsTransitioning)
                return;
            AudioManager.Instance?.PlayMusic(MusicKeys.Home);
        }

        private GameObject FindMapPrefab(string chapterId)
        {
            string expected = "Map_" + chapterId;
            foreach (GameObject prefab in mapPrefabs ?? Array.Empty<GameObject>())
                if (prefab != null && prefab.name == expected)
                    return prefab;
            return null;
        }

        private static void WarnLayoutOnce(string key, string reason)
        {
            if (warnedLayouts.Add(key))
                Debug.LogWarning("[Pve] Falling back to chapters.json mapPos because " + reason + ".");
        }

        /// <summary>The player clicked a stage node: sound, tutorial event, then the details.</summary>
        private void OnStageClicked(PveStage stage)
        {
            AudioManager.Instance?.Play("stage_select");
            if (stage.Id == "hoi_1_01")
                OnboardingRuntime.Publish("stage_1_selected");
            SelectStage(stage);
        }

        /// <summary>Shows the stage details. Silent: also used by the once-a-second stamina refresh.</summary>
        private void SelectStage(PveStage stage)
        {
            selected = stage;
            bool unlocked = PveRules.IsStageUnlocked(stage, catalog, GameBootstrap.Session.Profile);
            bool enough = GameBootstrap.Session.Profile.Pve.Stamina >= stage.StaminaCost;
            bool loadout = GameBootstrap.Session.TryValidateLoadout(out string loadoutReason);
            stageName.text = stage.Name;
            descriptionText.text = stage.Description;
            enemyText.text = string.IsNullOrEmpty(stage.Enemy.Sect) ? stage.Enemy.Name : stage.Enemy.Name
                + " · "
                + (OneBagJianghu.Bag.SectCatalog.LoadFromResources().TryGet(stage.Enemy.Sect,
                out var sect) ? sect.Name : stage.Enemy.Sect);
            int recommendedPower = StagePower.Recommended(stage);
            int playerPower = GameBootstrap.Session.PlayerPower();
            float powerRatio = recommendedPower > 0 ? (float)playerPower / recommendedPower : 1f;
            powerText.text = "Chiến Uy đề nghị: " + UiFormat.Number(recommendedPower);
            playerPowerText.text = "Chiến Uy của bạn: " + UiFormat.Number(playerPower);
            playerPowerText.color = powerRatio >= 1f
                ? new Color32(0x4F, 0x7A, 0x3A, 0xFF)
                : powerRatio >= .85f ? UiKit.Gold : UiKit.Vermilion;
            powerWarningText.text = powerRatio < .85f ? "Chiến Uy thấp hơn đề nghị" : string.Empty;
            DateTimeOffset now = ServerClock.UtcNow;
            PveProfileState staminaState = GameBootstrap.Session.Profile.Pve;
            string next = StaminaService.FormatDuration(stamina.SecondsToNext(staminaState, now));
            string regen = staminaState.Stamina >= staminaConfig.Max ? "Đã đầy" : "+1 sau " + next;
            costText.text = "Tốn " + UiFormat.Number(stage.StaminaCost) + " Thể Lực · "
                + UiFormat.Number(staminaState.Stamina) + "/" + UiFormat.Number(staminaConfig.Max) + " · " + regen;
            PveFirstClearReward reward = stage.Rewards.FirstClear;
            bool cleared = PveRules.IsCleared(GameBootstrap.Session.Profile, stage.Id);
            rewardsText.text = BuildRewardText(reward, cleared, catalog.RepeatRewardPercent);
            enemyPortrait.sprite = Portrait(stage.Enemy.Portrait, stage.Id);
            enemyPortrait.gameObject.SetActive(enemyPortrait.sprite != null);
            enemyPortrait.color = Color.white;
            challengeButton.GetComponent<Button>().interactable = unlocked && enough && loadout;
            disabledReason.text =
                !unlocked
                ? "Ải chưa mở"
                : !enough
                ? "Không đủ Thể Lực, đủ sau " + TimeUntilEnough(stage.StaminaCost, now)
                : !loadout
                ? loadoutReason
                : string.Empty;
        }

        private void RefreshStamina()
        {
            PveProfileState state = GameBootstrap.Session.Profile.Pve;
            UpdateStaminaText();
        }

        private void UpdateStaminaText()
        {
            if (staminaText != null && staminaConfig != null)
            {
                PveProfileState state = GameBootstrap.Session.Profile.Pve;
                string value = "Thể Lực: " + UiFormat.Number(state.Stamina) + "/" + UiFormat.Number(staminaConfig.Max);
                if (state.Stamina < staminaConfig.Max)
                {
                    string next = StaminaService.FormatDuration(
                        stamina.SecondsToNext(state, ServerClock.UtcNow));
                    value += "  " + next;
                }
                staminaText.text = value;
            }
        }

        private string TimeUntilEnough(int cost, DateTimeOffset now)
        {
            PveProfileState state = GameBootstrap.Session.Profile.Pve;
            int missing = Math.Max(0, cost - state.Stamina);
            double seconds = stamina.SecondsToNext(state, now);
            seconds += Math.Max(0, missing - 1) * 60d / staminaConfig.RegenPerMinute;
            return StaminaService.FormatDuration(seconds);
        }

        private void CloseMap()
        {
            Close();
            OnboardingRuntime.Publish("stage_map_closed");
        }

        private void StartStage()
        {
            if (selected == null || !PveRules.IsStageUnlocked(selected, catalog, GameBootstrap.Session.Profile))
                return;
            leavingForBattle = true;
            challengeButton.GetComponent<Button>().interactable = false;
            if (!PveStageFlow.TryStart(selected, this, HandleStartError))
            {
                leavingForBattle = false;
                SelectStage(selected);
            }
        }

        private void HandleStartError(string message)
        {
            leavingForBattle = false;
            if (selected != null)
            {
                SelectStage(selected);
            }

            PopupManager.Show<MessagePopup>(new MessagePopupData("Không thể vào ải", message, "Đóng"));
        }

        private void HandleProgressChanged()
        {
            if (!IsShowing || catalog == null)
            {
                return;
            }

            string selectedId = selected?.Id;
            DrawChapter(catalog.GetStage(selectedId));
        }

        private Sprite Sprite(string key) => art != null && art.TryGet(key, out Sprite sprite) ? sprite : null;
        public static string BuildRewardText(PveFirstClearReward reward, bool cleared, int repeatPercent)
        {
            if (!cleared)
            {
                return "Thưởng lần đầu: " + UiFormat.Number(reward.Silver) + " Bạc · "
                    + UiFormat.Number(reward.HeroExp) + " EXP";
            }

            long silver = reward.Silver * (long)repeatPercent / 100L;
            long experience = reward.HeroExp * (long)repeatPercent / 100L;
            return "Thưởng đánh lại (" + UiFormat.Number(repeatPercent) + "%): "
                + UiFormat.Number(silver) + " Bạc · " + UiFormat.Number(experience) + " EXP";
        }

        private Sprite Portrait(string key, string stableId)
        {
            Sprite sprite = Sprite(key);
            if (sprite == null && itemArt != null)
            {
                itemArt.TryGet(key, out sprite);
            }

            return sprite != null ? sprite : art?.ResolveEnemyPortrait(key, stableId);
        }

        private void ApplyFrame(Image image, string key, Color fallback)
        {
            // The stage card is a whole painting (2:3). The builder sizes this rect to the art's aspect and the
            // labels are anchored to its paper, so draw it unsliced or the text drifts off the paper.
            Sprite sprite = Sprite(key);
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.color = sprite == null ? fallback : Color.white;
        }

        private void SetOptionalIcon(Image image, string key)
        {
            if (image == null)
                return;
            image.sprite = Sprite(key);
            if (image.sprite == null && CurrencyArt.TryGetSprite(key, out Sprite fallback))
                image.sprite = fallback;
            image.gameObject.SetActive(image.sprite != null);
        }

        private static Color Parse(string html,
            Color fallback) => ColorUtility.TryParseHtmlString(html,
            out Color color) ? color : fallback;
    }

    public static class PveStageFlow
    {
        public static PveStage GetNext(PveStage stage, PveCatalog catalog)
        {
            if (stage == null || catalog == null)
                return null;
            int index = stage.Chapter.Stages.IndexOf(stage);
            if (index >= 0 && index + 1 < stage.Chapter.Stages.Count)
                return stage.Chapter.Stages[index + 1];
            int chapter = catalog.Chapters.IndexOf(stage.Chapter);
            if (chapter < 0 || chapter + 1 >= catalog.Chapters.Count)
                return null;
            PveChapter next = catalog.Chapters[chapter + 1];
            return PveRules.IsChapterUnlocked(next,
                catalog,
                GameBootstrap.Session.Profile)
                && next.Stages.Count > 0 ? next.Stages[0] : null;
        }

        public static string GetBlockedReason(PveStage stage)
        {
            if (stage == null)
                return "Đã hết ải";
            if (!GameBootstrap.Session.TryValidateLoadout(out string reason))
                return reason;
            return GameBootstrap.Session.Profile.Pve.Stamina < stage.StaminaCost ? "Không đủ Thể Lực" : string.Empty;
        }

        public static bool TryStart(PveStage stage,
            BasePopup popupToClose = null,
            Action<string> onError = null)
        {
            if (stage == null || !string.IsNullOrEmpty(GetBlockedReason(stage)))
                return false;
            GameBootstrap.Network.StartPveBattle(stage.Id,
                payload => HandlePveReady(stage, popupToClose, payload),
                onError ?? ShowStartError);

            return true;
        }

        private static void ShowStartError(string message)
        {
            PopupManager.Show<MessagePopup>(new MessagePopupData("Không thể vào ải", message, "Đóng"));
        }

        private static void HandlePveReady(PveStage stage,
            BasePopup popupToClose,
            Newtonsoft.Json.Linq.JObject payload)
        {
            GameBootstrap.Session.StorePveBattle(payload);
            GameBootstrap.Session.CurrentPveStageId = stage.Id;
            GameBootstrap.Session.SelectedMode = GameMode.Pve;
            if (popupToClose == null)
            {
                ShowBeforeDialogue(stage);
                return;
            }

            Action<BasePopup> closed = null;
            closed = popup =>
            {
                popup.Closed -= closed;
                ShowBeforeDialogue(stage);
            };
            popupToClose.Closed += closed;
            popupToClose.Close();
        }

        private static void ShowBeforeDialogue(PveStage stage)
        {
            List<PveDialogueLine> lines = stage.Dialogue?.Before;
            if (lines == null || lines.Count == 0)
            {
                SceneFlow.Go(GameScene.Gameplay);
                return;
            }

            DialoguePopup dialogue = PopupManager.Show<DialoguePopup>(new DialoguePopupData(PveCatalog.Load(), lines));
            if (dialogue == null)
            {
                SceneFlow.Go(GameScene.Gameplay);
                return;
            }

            Action<BasePopup> closed = null;
            closed = popup =>
            {
                popup.Closed -= closed;
                SceneFlow.Go(GameScene.Gameplay);
            };
            dialogue.Closed += closed;
        }
    }
}
