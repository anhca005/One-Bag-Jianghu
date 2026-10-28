using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Bag.View;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Audio;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class ReplayScreen : MonoBehaviour
    {
        [SerializeField]
        private FighterPanelView leftFighter;
        [SerializeField]
        private FighterPanelView rightFighter;
        [SerializeField]
        private RectTransform leftBagItems;
        [SerializeField]
        private RectTransform rightBagItems;
        [SerializeField]
        private ReplayItemView itemPrefab;
        [SerializeField]
        private Slider scrub;
        [SerializeField]
        private Text playLabel;
        [SerializeField]
        private Image playIcon;
        [SerializeField]
        private Sprite playSprite;
        [SerializeField]
        private Sprite pauseSprite;
        [SerializeField]
        private Text timeLabel;
        [SerializeField]
        private Text stageLabel;
        [SerializeField]
        private Text eventLog;
        [SerializeField]
        private RectTransform logContent;
        [SerializeField]
        private Text logRowPrefab;
        [SerializeField]
        private Text banner;
        [SerializeField]
        private ReplayVfxLayer vfxLayer;
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button speed1Button;
        [SerializeField]
        private Button speed2Button;
        [SerializeField]
        private Button speed4Button;
        [SerializeField]
        private Button restartButton;
        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private Button logToggleButton;
        [SerializeField]
        private GameObject expandedLogPanel;
        [SerializeField]
        private RectTransform collapsedLogContent;
        [SerializeField]
        private RectTransform expandedLogContent;
        private readonly Dictionary<int,
            ReplayItemView> itemViews =
            new Dictionary<int,
            ReplayItemView>();
        private readonly Dictionary<int, ItemInfo> itemInfos = new Dictionary<int, ItemInfo>();
        private readonly Dictionary<int, Color> itemColors = new Dictionary<int, Color>();
        private readonly Dictionary<int, string> itemNames = new Dictionary<int, string>();
        private readonly Dictionary<int,
            ItemCooldownTrack> cooldownTracks =
            new Dictionary<int,
            ItemCooldownTrack>();
        private readonly List<BattleEvent> eventBuffer = new List<BattleEvent>(32);
        private readonly Queue<Text> logRows = new Queue<Text>(30);
        private readonly Queue<string> collapsedLines = new Queue<string>(3);
        private readonly List<FloatingText> floating = new List<FloatingText>(12);
        private BattleLog log;
        private BattleTimeline timeline;
        private ReplayClock clock;
        private BattleSfxPlayback sfxPlayback;
        private FighterPanelView[] fighters;
        private bool settingSlider;
        private bool completionRaised;
        private int displayedTenth = -1;
        private bool logExpanded;
        private ItemTooltip itemTooltip;
        public event System.Action<int> Completed;
        public event System.Action ExitRequested;
        private void Awake()
        {
            itemTooltip = ItemTooltip.CreateRuntime(transform);
            itemTooltip.EnsureAbove(GetComponentInParent<Canvas>());
            playButton.onClick.AddListener(TogglePlay);
            speed1Button.onClick.AddListener(() => SetSpeed(1f));
            speed2Button.onClick.AddListener(() => SetSpeed(2f));
            speed4Button.onClick.AddListener(() => SetSpeed(4f));
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(Restart);
            }
            exitButton.onClick.AddListener(() => ExitRequested?.Invoke());
            logToggleButton.onClick.AddListener(ToggleLog);
            HighlightSpeed(speed1Button);
        }

        public void Play()
        {
            if (clock == null)
            {
                return;
            }

            clock.Play();
            UpdatePlayControl();
        }

        public void Initialize(BattleLog battleLog, ItemCatalog catalog, bool bossStage = false)
        {
            log = battleLog;
            timeline = new BattleTimeline(log);
            clock = new ReplayClock(log.DurationMs);
            var sfxResolver = new BattleSfxResolver(log,
                SfxData.LoadMap(),
                SfxData.LoadCatalog(),
                0,
                bossStage);
            sfxPlayback = new BattleSfxPlayback(timeline, sfxResolver);
            fighters = new[]
            {
                leftFighter,
                rightFighter
            };
            leftFighter.Initialize(log.Names[0]);
            rightFighter.Initialize(log.Names[1]);
            BindBag(0, catalog, leftBagItems);
            BindBag(1, catalog, rightBagItems);
            scrub.minValue = 0f;
            scrub.maxValue = log.DurationMs;
            scrub.wholeNumbers = true;
            scrub.onValueChanged.RemoveListener(OnScrub);
            scrub.onValueChanged.AddListener(OnScrub);
            vfxLayer.Initialize();
            logExpanded = false;
            expandedLogPanel.SetActive(false);
            logContent = expandedLogContent;
            ApplyState(0, true);
        }

        public void SetStageLabel(string value)
        {
            stageLabel.text = value;
        }

        public void HideTooltip()
        {
            itemTooltip?.Hide();
        }

        private void BindBag(int side, ItemCatalog catalog, RectTransform parent)
        {
            float pitch = ReplayStyle.CellSize + ReplayStyle.CellGap;
            for (int i = 0; i < log.Items.Count; i++)
            {
                BattleItemRef item = log.Items[i];
                if (item.Side != side)
                    continue;
                string itemName = item.Id;
                string rarity = item.Rarity;
                if (catalog.TryGet(item.Id, out ItemInfo info))
                {
                    itemName = info.Name;
                    rarity ??= info.Rarity;
                    itemInfos[item.Uid] = info;
                    cooldownTracks[item.Uid] = new ItemCooldownTrack(item.Uid, info, timeline.Events);
                }

                itemNames[item.Uid] = itemName;
                itemColors[item.Uid] = BagViewStyle.Fill(rarity);
                if (!item.X.HasValue || !item.Y.HasValue)
                    continue;
                ReplayItemView view = Instantiate(itemPrefab, parent);
                RectTransform rect = (RectTransform)view.transform;
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                int width = item.W ?? 1;
                float cellX = side == 0 ? item.X.Value : 5 - item.X.Value - width;
                rect.anchoredPosition = new Vector2(cellX * pitch, -item.Y.Value * pitch);
                rect.sizeDelta =
                    new Vector2((item.W ?? 1) * pitch - ReplayStyle.CellGap,
                    (item.H ?? 1) * pitch - ReplayStyle.CellGap);
                view.Initialize(item.Uid, item.Id, itemName, item.Tier ?? 1, rarity);
                view.SetTooltip(itemTooltip);
                itemViews[item.Uid] = view;
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame && PopupManager.Top == null)
            {
                ExitRequested?.Invoke();
            }

            if (clock == null)
                return;
            int previous = clock.Advance(Time.unscaledDeltaTime);
            int now = clock.TimeMs;
            if (now != previous)
            {
                sfxPlayback.PlayAdvancedWindow(previous,
                    now,
                    clock.Speed,
                    PlaySfx);
                eventBuffer.Clear();
                timeline.CollectEvents(previous, now, eventBuffer);
                for (int i = 0; i < eventBuffer.Count; i++)
                {
                    PresentEvent(eventBuffer[i]);
                }

                if (eventBuffer.Count > 0)
                {
                    ApplyState(now, false);
                }

                SetScrub(now);
                if (!clock.IsPlaying)
                {
                    UpdatePlayControl();
                }
            }

            if (!completionRaised && now >= log.DurationMs)
            {
                completionRaised = true;
                itemTooltip.Hide();
                string winner = log.Winner < 0 ? "draw" : log.Names[log.Winner];
                Debug.Log($"[Battle] End winner={winner}, durationMs={log.DurationMs}, events={timeline.Events.Count}");
                Completed?.Invoke(log.Winner);
            }

            foreach (KeyValuePair<int, ReplayItemView> pair in itemViews)
            {
                pair.Value.Refresh(now);
                if (cooldownTracks.TryGetValue(pair.Key, out ItemCooldownTrack track))
                {
                    pair.Value.SetCooldown(track.FillAt(now), track.HasActivations);
                }
                else
                    pair.Value.SetCooldown(0f, false);
            }

            fighters[0].RefreshTime(now);
            fighters[1].RefreshTime(now);
            vfxLayer.Tick(now);
            UpdateFloating();
        }

        private static void PlaySfx(string key)
        {
            AudioManager.Instance?.Play(key);
        }

        private void BuildArena(ItemCatalog catalog)
        {
            RectTransform arena = ReplayUi.Rect(transform, "Arena");
            arena.anchorMin = new Vector2(0f, .31f);
            arena.anchorMax = Vector2.one;
            arena.offsetMin = new Vector2(28f, 15f);
            arena.offsetMax = new Vector2(-28f, -28f);
            var row = arena.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 22f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = false;
            row.childControlHeight = true;
            row.childForceExpandHeight = true;
            fighters = new FighterPanelView[2];
            fighters[0] = CreateFighter(arena, log.Names[0]);
            CreateBag(arena, 0, catalog);
            CreateBag(arena, 1, catalog);
            fighters[1] = CreateFighter(arena, log.Names[1]);
        }

        private static FighterPanelView CreateFighter(Transform parent, string name)
        {
            RectTransform rect = ReplayUi.Rect(parent, "Fighter");
            LayoutElement element = rect.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = 330f;
            element.flexibleHeight = 1f;
            FighterPanelView view = rect.gameObject.AddComponent<FighterPanelView>();
            view.Initialize(name);
            return view;
        }

        private void CreateBag(Transform parent, int side, ItemCatalog catalog)
        {
            int cols = log.Bags[side][0];
            int rows = log.Bags[side][1];
            float pitch = ReplayStyle.CellSize + ReplayStyle.CellGap;
            Image bag =
                ReplayUi.Image(parent,
                "Bag " +
                side,
                new Color(ReplayStyle.Ink.r,
                ReplayStyle.Ink.g,
                ReplayStyle.Ink.b,
                .09f));
            LayoutElement layout = bag.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = cols * pitch;
            layout.preferredHeight = rows * pitch;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Image cell =
                        ReplayUi.Image(bag.transform,
                        "Cell",
                        new Color(ReplayStyle.Paper.r,
                        ReplayStyle.Paper.g,
                        ReplayStyle.Paper.b,
                        .55f));
                    RectTransform rect = cell.rectTransform;
                    rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                    rect.pivot = new Vector2(0f, 1f);
                    rect.anchoredPosition = new Vector2(x * pitch, -y * pitch);
                    rect.sizeDelta = new Vector2(ReplayStyle.CellSize, ReplayStyle.CellSize);
                }
            }

            for (int i = 0; i < log.Items.Count; i++)
            {
                BattleItemRef item = log.Items[i];
                if (item.Side != side)
                    continue;
                string name = item.Id;
                string rarity = item.Rarity;
                if (catalog.TryGet(item.Id, out ItemInfo info))
                {
                    name = info.Name;
                    rarity ??= info.Rarity;
                    itemInfos[item.Uid] = info;
                }

                itemNames[item.Uid] = name;
                itemColors[item.Uid] = BagViewStyle.Fill(rarity);
                if (!item.X.HasValue || !item.Y.HasValue)
                    continue;
                int width = item.W ?? 1;
                int height = item.H ?? 1;
                RectTransform rect = ReplayUi.Rect(bag.transform, name);
                rect.gameObject.AddComponent<Outline>();
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.anchoredPosition = new Vector2(item.X.Value * pitch, -item.Y.Value * pitch);
                rect.sizeDelta =
                    new Vector2(width * pitch - ReplayStyle.CellGap,
                    height * pitch - ReplayStyle.CellGap);
                ReplayItemView view = rect.gameObject.AddComponent<ReplayItemView>();
                view.Initialize(item.Uid, item.Id, name, item.Tier ?? 1, rarity);
                view.SetTooltip(itemTooltip);
                itemViews[item.Uid] = view;
            }
        }

        private void BuildBottom()
        {
            Image bottom = ReplayUi.Image(transform, "Bottom", ReplayStyle.Panel);
            RectTransform rect = bottom.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1f, .3f);
            rect.offsetMin = new Vector2(28f, 24f);
            rect.offsetMax = new Vector2(-28f, -8f);
            RectTransform controls = ReplayUi.Rect(bottom.transform, "Controls");
            controls.anchorMin = new Vector2(0f, .72f);
            controls.anchorMax = Vector2.one;
            controls.offsetMin = new Vector2(12f, 4f);
            controls.offsetMax = new Vector2(-12f, -4f);
            var row = controls.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 10f;
            row.childControlHeight = true;
            row.childForceExpandHeight = true;
            Button play = ReplayUi.Button(controls, "Phát", TogglePlay);
            playLabel = play.GetComponentInChildren<Text>();
            ReplayUi.Button(controls, "x1", () => clock.SetSpeed(1f), 70f);
            ReplayUi.Button(controls, "x2", () => clock.SetSpeed(2f), 70f);
            ReplayUi.Button(controls, "x4", () => clock.SetSpeed(4f), 70f);
            Image sliderImage =
                ReplayUi.Image(controls,
                "Timeline",
                new Color(ReplayStyle.Ink.r,
                ReplayStyle.Ink.g,
                ReplayStyle.Ink.b,
                .18f));
            sliderImage.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            scrub = sliderImage.gameObject.AddComponent<Slider>();
            scrub.minValue = 0f;
            scrub.maxValue = log.DurationMs;
            scrub.wholeNumbers = true;
            Image fill = ReplayUi.Image(sliderImage.transform, "Fill", ReplayStyle.Gold);
            ReplayUi.Stretch(fill.rectTransform);
            scrub.fillRect = fill.rectTransform;
            scrub.targetGraphic = sliderImage;
            scrub.onValueChanged.AddListener(OnScrub);
            timeLabel = ReplayUi.Text(controls, "Time", "0.0s", ReplayStyle.BodyFont);
            timeLabel.gameObject.AddComponent<LayoutElement>().preferredWidth = 115f;
            Image viewport =
                ReplayUi.Image(bottom.transform,
                "Event Log Viewport",
                new Color(0f,
                0f,
                0f,
                .035f));
            RectTransform viewportRect = viewport.rectTransform;
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = new Vector2(1f, .7f);
            viewportRect.offsetMin = new Vector2(18f, 10f);
            viewportRect.offsetMax = new Vector2(-18f, -4f);
            viewport.gameObject.AddComponent<RectMask2D>();
            eventLog = ReplayUi.Text(viewport.transform, "Event Log", "", 18, TextAnchor.UpperLeft);
            RectTransform eventRect = eventLog.rectTransform;
            eventRect.anchorMin = new Vector2(0f, 1f);
            eventRect.anchorMax = new Vector2(1f, 1f);
            eventRect.pivot = new Vector2(.5f, 1f);
            eventRect.sizeDelta = new Vector2(-12f, 700f);
            eventLog.horizontalOverflow = HorizontalWrapMode.Wrap;
            eventLog.verticalOverflow = VerticalWrapMode.Overflow;
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewportRect;
            scroll.content = eventRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            banner =
                ReplayUi.Text(transform,
                "End Banner",
                "",
                56,
                TextAnchor.MiddleCenter,
                UiFontRole.Display);
            banner.color = ReplayStyle.Gold;
            banner.gameObject.SetActive(false);
            RectTransform bannerRect = banner.rectTransform;
            bannerRect.anchorMin = new Vector2(.25f, .42f);
            bannerRect.anchorMax = new Vector2(.75f, .62f);
            bannerRect.offsetMin = bannerRect.offsetMax = Vector2.zero;
        }

        private void BuildVfxLayer()
        {
            RectTransform rect = ReplayUi.Rect(transform, "Replay VFX Layer");
            ReplayUi.Stretch(rect);
            vfxLayer = rect.gameObject.AddComponent<ReplayVfxLayer>();
            vfxLayer.Initialize();
        }

        private void TogglePlay()
        {
            clock.Toggle();
            UpdatePlayControl();
        }

        private void SetSpeed(float speed)
        {
            if (clock == null)
                return;
            clock.SetSpeed(speed);
            HighlightSpeed(speed == 1f ? speed1Button : speed == 2f ? speed2Button : speed4Button);
        }

        private void HighlightSpeed(Button active)
        {
            SetSpeedButtonColor(speed1Button, speed1Button == active);
            SetSpeedButtonColor(speed2Button, speed2Button == active);
            SetSpeedButtonColor(speed4Button, speed4Button == active);
        }

        private static void SetSpeedButtonColor(Button button, bool active)
        {
            if (button != null && button.targetGraphic != null)
            {
                button.targetGraphic.color = active ? ReplayStyle.Gold : Color.white;
            }
        }

        private void Restart()
        {
            completionRaised = false;
            clock.Seek(0);
            ClearPresentation();
            ApplyState(0, true);
            clock.Play();
            UpdatePlayControl();
        }

        private void UpdatePlayControl()
        {
            bool isPlaying = clock != null && clock.IsPlaying;
            playLabel.text = isPlaying ? "Tạm dừng" : "Phát";
            if (playIcon == null)
                return;
            Sprite sprite = isPlaying ? pauseSprite : playSprite;
            if (sprite != null)
                playIcon.sprite = sprite;
        }

        private void OnScrub(float value)
        {
            if (!settingSlider)
            {
                clock.Seek(Mathf.RoundToInt(value));
                ClearPresentation();
                ApplyState(clock.TimeMs, true);
            }
        }

        private void SetScrub(int timeMs)
        {
            settingSlider = true;
            scrub.SetValueWithoutNotify(timeMs);
            settingSlider = false;
            int second = timeMs / 1000;
            if (second != displayedTenth)
            {
                displayedTenth = second;
                timeLabel.text = (second / 60).ToString("00") + ":" + (second % 60).ToString("00");
            }
        }

        private void ApplyState(int timeMs, bool rebuildStatuses)
        {
            fighters[0].SetState(timeline.StateAt(0, timeMs), timeMs);
            fighters[1].SetState(timeline.StateAt(1, timeMs), timeMs);
            if (rebuildStatuses)
            {
                foreach (ReplayItemView view in itemViews.Values)
                {
                    view.ClearStatus();
                }

                IReadOnlyList<BattleEvent> events = timeline.Events;
                for (int i = 0; i < events.Count && events[i].TimeMs <= timeMs; i++)
                {
                    ApplyItemStatus(events[i], timeMs);
                }
            }

            foreach (KeyValuePair<int, ReplayItemView> pair in itemViews)
            {
                if (cooldownTracks.TryGetValue(pair.Key, out ItemCooldownTrack track))
                {
                    pair.Value.SetCooldown(track.FillAt(timeMs), track.HasActivations);
                }
                else
                    pair.Value.SetCooldown(0f, false);
            }

            SetScrub(timeMs);
            banner.gameObject.SetActive(timeMs >= log.DurationMs);
            if (timeMs >= log.DurationMs)
            {
                banner.text =
                    log.Winner < 0 ?
                    "Hòa" :
                    (log.Winner == 0 ?
                    "Thắng — " :
                    "Bại — ") +
                    log.Names[log.Winner];
            }
        }

        private void PresentEvent(BattleEvent battleEvent)
        {
            if (battleEvent.Type == BattleEventType.Trigger &&
                battleEvent.Item.HasValue &&
                IsFiringTrigger(battleEvent.Kind) &&
                itemViews.TryGetValue(battleEvent.Item.Value,
                out ReplayItemView fired))
            {
                fired.Flash(battleEvent.Kind == "ULTIMATE", clock.TimeMs);
            }

            PresentVfx(battleEvent);
            ApplyItemStatus(battleEvent, clock.TimeMs);
            if (battleEvent.Type == BattleEventType.Damage || battleEvent.Type == BattleEventType.Burst)
            {
                if (battleEvent.Type == BattleEventType.Damage)
                {
                    fighters[battleEvent.Side].Shake(clock.TimeMs);
                }

                Color color =
                    battleEvent.Type == BattleEventType.Burst ?
                    ReplayStyle.Status(battleEvent.Kind) :
                    ReplayStyle.Hp;
                SpawnFloating(battleEvent.Side,
                    "-" +
                    (battleEvent.Value ?? 0) +
                    (battleEvent.Crit == true ?
                    "!" :
                    ""),
                    color,
                    battleEvent.Crit == true ||
                    battleEvent.Type == BattleEventType.Burst ?
                    38 :
                    29);
                if (battleEvent.Absorbed.GetValueOrDefault() > 0)
                {
                    SpawnFloating(battleEvent.Side,
                        "Hấp thụ " + battleEvent.Absorbed.Value,
                        Color.gray,
                        20);
                }
            }
            else if (battleEvent.Type == BattleEventType.Heal)
            {
                SpawnFloating(battleEvent.Side,
                    "+" + (battleEvent.Value ?? 0),
                    ReplayStyle.Heal,
                    30);
            }
            else if (battleEvent.Type == BattleEventType.Parry)
            {
                SpawnFloating(battleEvent.Side, "Hóa Kình!", ReplayStyle.Gold, 32);
            }

            AddLog(EventTextFormatter.Format(battleEvent, ItemName));
        }

        private static bool IsFiringTrigger(string kind)
        {
            return kind == "BASIC" || kind == "ULTIMATE" || kind == "ACTIVE";
        }

        private void PresentVfx(BattleEvent battleEvent)
        {
            ItemInfo info = null;
            ReplayItemView itemView = null;
            Color fallbackColor = ReplayStyle.Gold;
            if (battleEvent.Item.HasValue)
            {
                itemInfos.TryGetValue(battleEvent.Item.Value, out info);
                itemViews.TryGetValue(battleEvent.Item.Value, out itemView);
                if (itemColors.TryGetValue(battleEvent.Item.Value, out Color itemColor))
                {
                    fallbackColor = itemColor;
                }
            }

            ReplayVfx vfx = VfxResolver.Resolve(battleEvent, info);
            if (vfx.Kind == ReplayVfxKind.None || battleEvent.Side < 0 || battleEvent.Side > 1)
            {
                return;
            }

            int targetSide =
                vfx.Kind == ReplayVfxKind.Travel ?
                1 - battleEvent.Side :
                battleEvent.Side;
            RectTransform source = itemView == null ? null : (RectTransform)itemView.transform;
            RectTransform target = fighters[targetSide].VfxTarget;
            if (vfx.Kind == ReplayVfxKind.Burst)
            {
                fallbackColor = ReplayStyle.Status(battleEvent.Kind);
            }

            vfxLayer.Present(vfx, battleEvent, source, target, fallbackColor, clock.TimeMs);
        }

        private void ApplyItemStatus(BattleEvent battleEvent, int now)
        {
            if (battleEvent.Type != BattleEventType.Status ||
                !battleEvent.Item.HasValue ||
                !battleEvent.Value.HasValue)
                return;
            if (battleEvent.Kind != "SEAL" && battleEvent.Kind != "SLOW" && battleEvent.Kind != "HASTE")
                return;
            int until = battleEvent.TimeMs + battleEvent.Value.Value;
            if (until > now && itemViews.TryGetValue(battleEvent.Item.Value, out ReplayItemView view))
            {
                view.SetStatus(battleEvent.Kind, until);
            }
        }

        private string ItemName(int uid) =>
            itemNames.TryGetValue(uid,
            out string value) ?
            value :
            "Vật phẩm " +
            uid;
        private void AddLog(string line)
        {
            if (logRows.Count == 30)
            {
                Destroy(logRows.Dequeue().gameObject);
            }

            Text row = Instantiate(logRowPrefab, expandedLogContent);
            row.text = line;
            logRows.Enqueue(row);
            // The collapsed plaque fits two lines; the expanded log keeps the history.
            if (collapsedLines.Count == 2)
            {
                collapsedLines.Dequeue();
            }

            collapsedLines.Enqueue(line);
            eventLog.text = string.Join("\n", collapsedLines);
        }

        private void ToggleLog()
        {
            logExpanded = !logExpanded;
            expandedLogPanel.SetActive(logExpanded);
            logContent = expandedLogContent;
        }

        private void ClearPresentation()
        {
            while (logRows.Count > 0)
            {
                Destroy(logRows.Dequeue().gameObject);
            }

            collapsedLines.Clear();
            eventLog.text = "";
            banner.gameObject.SetActive(false);
            vfxLayer.Clear();
            fighters[0].ClearFeedback();
            fighters[1].ClearFeedback();
            foreach (ReplayItemView view in itemViews.Values)
            {
                view.ClearFeedback();
            }
            for (int i = 0; i < floating.Count; i++)
            {
                Destroy(floating[i].Text.gameObject);
            }

            floating.Clear();
        }

        private void SpawnFloating(int side, string value, Color color, int size)
        {
            Text text = ReplayUi.Text(fighters[side].VfxTarget, "Floating", value, size);
            text.color = color;
            RectTransform rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(300f, 70f);
            floating.Add(new FloatingText(text, Time.unscaledTime));
        }

        private void UpdateFloating()
        {
            float now = Time.unscaledTime;
            for (int i = floating.Count - 1; i >= 0; i--)
            {
                FloatingText item = floating[i];
                float progress = (now - item.Start) / .8f;
                if (progress >= 1f)
                {
                    Destroy(item.Text.gameObject);
                    floating.RemoveAt(i);
                    continue;
                }

                item.Text.rectTransform.anchoredPosition = new Vector2(0f, progress * 75f);
                Color color = item.Text.color;
                color.a = 1f - progress;
                item.Text.color = color;
            }
        }

        private sealed class FloatingText
        {
            public FloatingText(Text text, float start)
            {
                Text = text;
                Start = start;
            }

            public Text Text;
            public float Start;
        }
    }
}
