using System;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View.VfxLab
{
    public sealed class VfxLabBootstrap : MonoBehaviour
    {
        [SerializeField] private Image rootBackground;
        [SerializeField] private RectTransform catalogContent;
        [SerializeField] private VfxCatalogRow catalogRowPrefab;
        [SerializeField] private Text countLabel;
        [SerializeField] private RectTransform leftFighter;
        [SerializeField] private RectTransform rightFighter;
        [SerializeField] private ReplayVfxLayer vfxLayer;
        [SerializeField] private Text stageLabel;
        [SerializeField] private RectTransform inspectorBox;
        [SerializeField] private Image inspectorImage;
        [SerializeField] private Text inspectorText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button loopButton;
        [SerializeField] private Button sideButton;
        [SerializeField] private Button[] speedButtons;
        [SerializeField] private Button playAllButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backgroundButton;
        [SerializeField] private Button hitButton;
        [SerializeField] private Slider rotationSlider;
        [SerializeField] private Slider scaleSlider;
        [SerializeField] private Slider offsetXSlider;
        [SerializeField] private Slider offsetYSlider;
        [SerializeField] private Toggle flipXToggle;
        [SerializeField] private Toggle flipYToggle;
        [SerializeField] private Text rotationValue;
        [SerializeField] private Text scaleValue;
        [SerializeField] private Text offsetXValue;
        [SerializeField] private Text offsetYValue;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Text unsavedLabel;

        private const int LoopPauseMs = 400;
        private readonly List<VfxCatalogRow> rows = new List<VfxCatalogRow>(39);
        private IReadOnlyList<VfxCatalogEntry> entries;
        private ItemIconLibrary iconLibrary;
        private VfxTuning tuning;
        private int selectedIndex, clockMs, finishMs, backgroundMode;
        private bool playing, playAll, loop, hitAfter = true, attackerOnLeft = true, updatingUi, unsaved;
        private float speed = 1f, clockRemainder;
        private Sprite checkerSprite;

        private void Awake()
        {
            playButton.onClick.AddListener(PlaySelected); loopButton.onClick.AddListener(ToggleLoop);
            sideButton.onClick.AddListener(ToggleSide); playAllButton.onClick.AddListener(PlayEveryEntry);
            previousButton.onClick.AddListener(() => Move(-1)); nextButton.onClick.AddListener(() => Move(1));
            backgroundButton.onClick.AddListener(CycleBackground); hitButton.onClick.AddListener(ToggleHitAfter);
            resetButton.onClick.AddListener(ResetTuning); saveButton.onClick.AddListener(SaveTuning);
            rotationSlider.onValueChanged.AddListener(_ => TuningChanged()); scaleSlider.onValueChanged.AddListener(_ => TuningChanged());
            offsetXSlider.onValueChanged.AddListener(_ => TuningChanged()); offsetYSlider.onValueChanged.AddListener(_ => TuningChanged());
            flipXToggle.onValueChanged.AddListener(_ => TuningChanged()); flipYToggle.onValueChanged.AddListener(_ => TuningChanged());
            float[] values = { .25f, .5f, 1f, 2f };
            for (int i = 0; i < speedButtons.Length && i < values.Length; i++) { float value = values[i]; speedButtons[i].onClick.AddListener(() => SetSpeed(value)); }
        }

        private void Start()
        {
            entries = VfxCatalog.Entries; iconLibrary = ItemIconLibrary.Load(); tuning = VfxTuning.Load();
            vfxLayer.Initialize(); BuildCatalog(); Select(0, false); SetSpeed(1f);
        }

        private void BuildCatalog()
        {
            int artCount = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                int index = i; bool hasArt = iconLibrary != null && iconLibrary.TryGet(entries[i].Key, out _);
                if (hasArt) artCount++;
                VfxCatalogRow row = Instantiate(catalogRowPrefab, catalogContent);
                row.name = entries[i].Key; row.gameObject.SetActive(true); row.Bind(entries[i], hasArt, () => Select(index, true)); rows.Add(row);
            }
            countLabel.text = artCount + "/" + entries.Count + " có hình";
        }

        private void Update()
        {
            HandleKeyboard(); if (!playing) return;
            clockRemainder += Time.unscaledDeltaTime * 1000f * speed; int advance = Mathf.FloorToInt(clockRemainder);
            clockRemainder -= advance; clockMs += advance; vfxLayer.Tick(clockMs); if (clockMs < finishMs) return;
            if (playAll && selectedIndex + 1 < entries.Count) { Select(selectedIndex + 1, false); StartCurrent(); playAll = true; }
            else if (playAll && loop) { Select(0, false); StartCurrent(); playAll = true; }
            else if (loop) PlaySelected(); else { playing = false; playAll = false; }
        }

        private void Select(int index, bool play)
        {
            selectedIndex = Mathf.Clamp(index, 0, entries.Count - 1);
            for (int i = 0; i < rows.Count; i++) rows[i].Background.color = new Color(ReplayStyle.Ink.r, ReplayStyle.Ink.g, ReplayStyle.Ink.b, i == selectedIndex ? .24f : .06f);
            stageLabel.text = entries[selectedIndex].Label + "\n" + entries[selectedIndex].Key;
            RefreshInspector(); RefreshTuningUi(); RefreshTuningMarks(); if (play) PlaySelected();
        }

        private void PlaySelected() { playAll = false; StartCurrent(); }
        private void PlayEveryEntry() { Select(0, false); StartCurrent(); playAll = true; }
        private void StartCurrent()
        {
            VfxCatalogEntry entry = entries[selectedIndex]; vfxLayer.Clear(); clockMs = 0; clockRemainder = 0f; playing = true;
            RectTransform attacker = attackerOnLeft ? leftFighter : rightFighter, defender = attackerOnLeft ? rightFighter : leftFighter;
            BattleEvent battleEvent = CreateEvent(entry); RectTransform source = entry.Kind == ReplayVfxKind.Travel ? attacker : null;
            Color fallback = entry.Kind == ReplayVfxKind.Burst ? ReplayStyle.Status(entry.EventKind) : ReplayStyle.Gold;
            vfxLayer.Present(new ReplayVfx(entry.Kind, entry.Key), battleEvent, source, defender, fallback, 0);
            int duration = ReplayVfxLayer.GetDuration(entry.Kind, entry.EventKind); finishMs = duration + LoopPauseMs;
            if (entry.Kind == ReplayVfxKind.Travel && hitAfter)
            {
                int at = duration - ReplayStyle.VfxBasicDurationMs;
                vfxLayer.Present(new ReplayVfx(ReplayVfxKind.Hit, "hit"), new BattleEvent { Type = BattleEventType.Damage, Side = battleEvent.Side }, null, defender, ReplayStyle.Hp, at);
                finishMs = duration + ReplayStyle.VfxHitDurationMs + LoopPauseMs;
            }
        }

        private BattleEvent CreateEvent(VfxCatalogEntry entry) => new BattleEvent
        {
            Type = entry.Kind == ReplayVfxKind.Travel ? BattleEventType.Trigger : entry.Kind == ReplayVfxKind.Burst ? BattleEventType.Burst : entry.Kind == ReplayVfxKind.Parry ? BattleEventType.Parry : BattleEventType.Damage,
            Kind = entry.EventKind, Side = attackerOnLeft ? 0 : 1
        };

        private void RefreshInspector()
        {
            VfxCatalogEntry entry = entries[selectedIndex]; Vector2 size = ReplayVfxLayer.GetSize(entry.Kind, entry.EventKind); inspectorBox.sizeDelta = size;
            Sprite sprite = null; bool hasArt = iconLibrary != null && iconLibrary.TryGet(entry.Key, out sprite); inspectorImage.sprite = hasArt ? sprite : null; inspectorImage.enabled = hasArt;
            VfxTuning.Entry tuned = tuning.TryGet(entry.Key, out VfxTuning.Entry found) ? found : null; VfxTransform transform = VfxTuning.Compose(tuned, false);
            inspectorImage.rectTransform.localScale = transform.LocalScale; inspectorImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, transform.Rotation);
            inspectorText.color = hasArt ? ReplayStyle.Ink : ReplayStyle.Hp;
            inspectorText.text = hasArt
                ? "Texture: " + sprite.texture.width + " × " + sprite.texture.height + " px\n" +
                  "Sprite rect: " + sprite.rect.width + " × " + sprite.rect.height + " px\n" +
                  "Khung trong game: " + size.x + " × " + size.y
                : "✗ Thiếu hình — battle sẽ dùng fallback\nKhung trong game: " + size.x + " × " + size.y;
        }

        private void TuningChanged()
        {
            if (updatingUi || entries == null) return; VfxTuning.Entry entry = tuning.GetOrCreate(entries[selectedIndex].Key);
            entry.rotation = rotationSlider.value; entry.scale = scaleSlider.value; entry.offset = new Vector2(offsetXSlider.value, offsetYSlider.value);
            entry.flipX = flipXToggle.isOn; entry.flipY = flipYToggle.isOn; MarkTuningChanged();
        }
        private void MarkTuningChanged() { unsaved = true; RefreshTuningUi(); RefreshTuningMarks(); RefreshInspector(); vfxLayer.RefreshTuning(); PlaySelected(); }
        private void RefreshTuningUi()
        {
            updatingUi = true; VfxTuning.Entry entry = tuning.TryGet(entries[selectedIndex].Key, out VfxTuning.Entry found) ? found : null;
            rotationSlider.SetValueWithoutNotify(entry == null ? 0f : entry.rotation); scaleSlider.SetValueWithoutNotify(entry == null ? 1f : entry.scale);
            offsetXSlider.SetValueWithoutNotify(entry == null ? 0f : entry.offset.x); offsetYSlider.SetValueWithoutNotify(entry == null ? 0f : entry.offset.y);
            flipXToggle.SetIsOnWithoutNotify(entry != null && entry.flipX); flipYToggle.SetIsOnWithoutNotify(entry != null && entry.flipY);
            rotationValue.text = rotationSlider.value.ToString("0") + "°"; scaleValue.text = scaleSlider.value.ToString("0.00") + "×";
            offsetXValue.text = offsetXSlider.value.ToString("0") + " px"; offsetYValue.text = offsetYSlider.value.ToString("0") + " px";
            unsavedLabel.text = unsaved ? "● chưa lưu" : string.Empty; updatingUi = false;
        }
        private void RefreshTuningMarks() { for (int i = 0; i < entries.Count; i++) rows[i].Tuned.text = tuning.TryGet(entries[i].Key, out VfxTuning.Entry entry) && !VfxTuning.IsIdentity(entry) ? "✎" : string.Empty; }
        private void ResetTuning() { tuning.Reset(entries[selectedIndex].Key); MarkTuningChanged(); }
        private void SaveTuning()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(tuning); UnityEditor.AssetDatabase.SaveAssets();
#endif
            unsaved = false; unsavedLabel.text = "Đã lưu";
        }

        private void HandleKeyboard()
        {
            Keyboard keyboard = Keyboard.current; if (keyboard == null || entries == null) return;
            if (keyboard.spaceKey.wasPressedThisFrame) PlaySelected(); if (keyboard.leftArrowKey.wasPressedThisFrame) Move(-1); if (keyboard.rightArrowKey.wasPressedThisFrame) Move(1); if (keyboard.lKey.wasPressedThisFrame) ToggleLoop();
            float step = keyboard.shiftKey.isPressed ? 1f : 5f;
            if (keyboard.qKey.wasPressedThisFrame) Nudge(-step, 0f, Vector2.zero); if (keyboard.eKey.wasPressedThisFrame) Nudge(step, 0f, Vector2.zero);
            if (keyboard.zKey.wasPressedThisFrame) Nudge(0f, -.05f, Vector2.zero); if (keyboard.xKey.wasPressedThisFrame) Nudge(0f, .05f, Vector2.zero);
            if (keyboard.aKey.wasPressedThisFrame) Nudge(0f, 0f, Vector2.left * 5f); if (keyboard.dKey.wasPressedThisFrame) Nudge(0f, 0f, Vector2.right * 5f);
            if (keyboard.wKey.wasPressedThisFrame) Nudge(0f, 0f, Vector2.up * 5f); if (keyboard.sKey.wasPressedThisFrame) Nudge(0f, 0f, Vector2.down * 5f);
        }
        private void Nudge(float rotation, float scale, Vector2 offset)
        {
            VfxTuning.Entry entry = tuning.GetOrCreate(entries[selectedIndex].Key); entry.rotation = Mathf.Clamp(entry.rotation + rotation, -180f, 180f);
            entry.scale = Mathf.Clamp(entry.scale + scale, .2f, 4f); entry.offset = new Vector2(Mathf.Clamp(entry.offset.x + offset.x, -200f, 200f), Mathf.Clamp(entry.offset.y + offset.y, -200f, 200f)); MarkTuningChanged();
        }
        private void Move(int delta) => Select((selectedIndex + delta + entries.Count) % entries.Count, true);
        private void ToggleLoop() { loop = !loop; SetButtonText(loopButton, "Loop: " + (loop ? "On" : "Off")); }
        private void ToggleSide() { attackerOnLeft = !attackerOnLeft; SetButtonText(sideButton, "Side: " + (attackerOnLeft ? "Left" : "Right")); PlaySelected(); }
        private void ToggleHitAfter() { hitAfter = !hitAfter; SetButtonText(hitButton, "Hit after: " + (hitAfter ? "On" : "Off")); }
        private void SetSpeed(float value) { speed = value; float[] values = { .25f, .5f, 1f, 2f }; for (int i = 0; i < speedButtons.Length; i++) speedButtons[i].GetComponent<Image>().color = new Color(ReplayStyle.Gold.r, ReplayStyle.Gold.g, ReplayStyle.Gold.b, Mathf.Approximately(value, values[i]) ? .55f : .18f); }
        private void CycleBackground()
        {
            backgroundMode = (backgroundMode + 1) % 3; rootBackground.sprite = null; rootBackground.type = Image.Type.Simple; rootBackground.color = ReplayStyle.Paper; string label = "BG: Paper";
            if (backgroundMode == 1) { rootBackground.color = new Color(.055f, .05f, .045f, 1f); label = "BG: Dark"; }
            else if (backgroundMode == 2) { if (checkerSprite == null) checkerSprite = CreateCheckerSprite(); rootBackground.sprite = checkerSprite; rootBackground.type = Image.Type.Tiled; rootBackground.color = Color.white; label = "BG: Checker"; }
            SetButtonText(backgroundButton, label);
        }
        private static Sprite CreateCheckerSprite()
        {
            const int size = 32; var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "VFX Lab Checker", hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Repeat };
            Color32 light = new Color32(220, 220, 220, 255), dark = new Color32(160, 160, 160, 255);
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) texture.SetPixel(x, y, ((x / 16) + (y / 16)) % 2 == 0 ? light : dark);
            texture.Apply(false, true); return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect);
        }
        private static void SetButtonText(Button button, string value) => button.GetComponentInChildren<Text>().text = value;
    }
}
