using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Pve
{
    public sealed class StageMapPopup : BasePopup
    {
        [SerializeField]
        private Image mapBackground, header, detailFrame, staminaIcon, rewardCurrencyIcon;
        [SerializeField]
        private Text chapterText,
            staminaText,
            stageName,
            descriptionText,
            enemyText,
            powerText,
            rewardsText,
            costText,
            disabledReason;
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
        private readonly List<GameObject> nodes = new List<GameObject>();
        private PveCatalog catalog;
        private StaminaService stamina;
        private PveArtLibrary art;
        private ItemIconLibrary itemArt;
        private int chapterIndex;
        private PveStage selected;
        private GameObject mapInstance;
        private PveMapLayout mapLayout;
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
            closeButton.onClick.AddListener(Close);
        }

        protected override void OnSetup(object data)
        {
            catalog = PveCatalog.Load();
            stamina = new StaminaService(StaminaConfig.Load());
            art = PveArtLibrary.Load();
            itemArt = ItemIconLibrary.Load();
            stamina.Refresh(GameBootstrap.Session.Profile.Pve, DateTimeOffset.UtcNow);
            GameBootstrap.Session.SaveProfile();
            string selectedStageId = data as string;
            PveStage requested = catalog.GetStage(selectedStageId);
            chapterIndex = data is int i ? Mathf.Clamp(i,
0,
catalog.Chapters.Count - 1) : requested != null ? catalog.Chapters.IndexOf(requested.Chapter) : 0;
            DrawChapter(requested);
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
            SetOptionalIcon(rewardCurrencyIcon, "icon_bac");
            staminaText.text = "Thể Lực: " + GameBootstrap.Session.Profile.Pve.Stamina;
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
                int stars = GameBootstrap.Session.Profile.Pve.Stages.TryGetValue(stage.Id,
out PveStageProgress progress) ? progress.BestStars : 0;
                node.Bind(stage, unlocked, stars, accent, chapter.Theme, art, SelectStage);
                nodes.Add(node.gameObject);
            }

            SelectStage(requested != null && requested.Chapter == chapter ? requested : chapter.Stages[0]);
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
            powerText.text = stage.RecommendedLevel > 0 ? "Cấp đề nghị: "
+ stage.RecommendedLevel : "Chiến lực đề nghị: "
+ stage.RecommendedPower;
            costText.text = "Tốn " + stage.StaminaCost + " Thể Lực";
            PveFirstClearReward reward = stage.Rewards.FirstClear;
            rewardsText.text = $"Lần đầu: {reward.Silver:N0} Bạc · {reward.HeroExp} EXP";
            enemyPortrait.sprite = Portrait(stage.Enemy.Portrait);
            enemyPortrait.color = enemyPortrait.sprite == null ? new Color(.2f, .2f, .2f, .3f) : Color.white;
            challengeButton.GetComponent<Button>().interactable = unlocked && enough && loadout;
            disabledReason.text =
                !unlocked
                ? "Ải chưa mở"
                : !enough
                ? "Không đủ Thể Lực"
                : !loadout
                ? loadoutReason
                : string.Empty;
        }

        private void StartStage()
        {
            if (selected == null || !PveRules.IsStageUnlocked(selected, catalog, GameBootstrap.Session.Profile))
                return;
            if (!PveStageFlow.TryStart(selected, this))
                SelectStage(selected);
        }

        private Sprite Sprite(string key) => art != null && art.TryGet(key, out Sprite sprite) ? sprite : null;
        private Sprite Portrait(string key)
        {
            Sprite sprite = Sprite(key);
            return sprite != null ? sprite : itemArt != null && itemArt.TryGet(key, out sprite) ? sprite : null;
        }

        private void ApplyFrame(Image image, string key, Color fallback)
        {
            Sprite sprite = Sprite(key);
            image.sprite = sprite;
            image.type = sprite == null ? Image.Type.Simple : Image.Type.Sliced;
            image.color = sprite == null ? fallback : Color.white;
        }

        private void SetOptionalIcon(Image image, string key)
        {
            if (image == null)
                return;
            image.sprite = Sprite(key);
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

        public static bool TryStart(PveStage stage, BasePopup popupToClose = null)
        {
            if (stage == null || !string.IsNullOrEmpty(GetBlockedReason(stage)))
                return false;
            var stamina = new StaminaService(StaminaConfig.Load());
            if (!stamina.TrySpend(GameBootstrap.Session.Profile.Pve, stage.StaminaCost, DateTimeOffset.UtcNow))
                return false;
            GameBootstrap.Session.CurrentPveStageId = stage.Id;
            GameBootstrap.Session.SelectedMode = GameMode.Pve;
            GameBootstrap.Session.SaveProfile();
            Debug.Log($"[Pve] Start stage={stage.Id}, stamina={GameBootstrap.Session.Profile.Pve.Stamina}");
            if (popupToClose == null)
                ShowBeforeDialogue(stage);
            else
            {
                Action<BasePopup> closed = null;
                closed = popup =>
                {
                    popup.Closed -= closed;
                    ShowBeforeDialogue(stage);
                };
                popupToClose.Closed += closed;
                popupToClose.Close();
            }

            return true;
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