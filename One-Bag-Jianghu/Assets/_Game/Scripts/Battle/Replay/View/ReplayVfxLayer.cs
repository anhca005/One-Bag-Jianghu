using System.Collections.Generic;
using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Battle.Replay.View
{
    public sealed class ReplayVfxLayer : MonoBehaviour
    {
        [SerializeField]
        private Image effectPrefab;
        private const int InitialPoolSize = 24;
        private readonly List<Effect> effects = new List<Effect>(InitialPoolSize);
        private RectTransform layerRect;
        private ItemIconLibrary iconLibrary;
        private Sprite fallbackCircle;
        private Sprite fallbackRounded;
        private VfxTuning tuning;
        public void Initialize()
        {
            layerRect = (RectTransform)transform;
            iconLibrary = ItemIconLibrary.Load();
            tuning = VfxTuning.Load();
            fallbackCircle = CreateFallbackSprite(true);
            fallbackRounded = CreateFallbackSprite(false);
            for (int i = 0; i < InitialPoolSize; i++)
            {
                CreateEffect();
            }
        }

        public void Present(ReplayVfx vfx,
BattleEvent battleEvent,
RectTransform source,
RectTransform target,
Color fallbackColor,
int nowMs)
        {
            if (vfx.Kind == ReplayVfxKind.None || target == null)
            {
                return;
            }

            Effect effect = Acquire();
            effect.Kind = vfx.Kind;
            // Impacts wait for the travelling stroke fired in the same tick to arrive.
            effect.StartMs =
vfx.Kind == ReplayVfxKind.Hit ?
nowMs +
ReplayStyle.VfxBasicDurationMs :
nowMs;
            effect.DurationMs = GetDuration(vfx.Kind, battleEvent.Kind);
            // Only travelling strokes leave from the item; impacts, bursts and parries play on the fighter.
            effect.Start =
vfx.Kind == ReplayVfxKind.Travel &&
source != null ?
Center(source) :
Center(target);
            effect.End = Center(target);
            effect.Mirror = vfx.Kind == ReplayVfxKind.Travel && battleEvent.Side == 1;
            effect.Image.sprite = ResolveSprite(vfx.SpriteKey, vfx.Kind, out bool hasArt);
            effect.Image.color = hasArt ? Color.white : ResolveColor(vfx.Kind, fallbackColor);
            effect.Image.preserveAspect = hasArt || vfx.Kind != ReplayVfxKind.Travel;
            effect.Transform =
hasArt &&
tuning.TryGet(vfx.SpriteKey,
out VfxTuning.Entry entry) ?
VfxTuning.Compose(entry,
effect.Mirror) :
VfxTuning.Compose(null,
effect.Mirror);
            effect.Rect.sizeDelta = GetSize(vfx.Kind, battleEvent.Kind);
            effect.Rect.anchoredPosition = effect.Start + effect.Transform.Offset;
            effect.Rect.localScale = effect.Transform.LocalScale;
            effect.Rect.localEulerAngles = new Vector3(0f, 0f, effect.Transform.Rotation);
            effect.Root.SetActive(true);
        }

        public void RefreshTuning()
        {
            tuning = VfxTuning.Load();
        }

        public void Tick(int nowMs)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                Effect effect = effects[i];
                if (!effect.Root.activeSelf)
                {
                    continue;
                }

                if (nowMs < effect.StartMs)
                {
                    Color hidden = effect.Image.color;
                    hidden.a = 0f;
                    effect.Image.color = hidden;
                    continue;
                }

                float progress = Mathf.Clamp01((float)(nowMs - effect.StartMs) / effect.DurationMs);
                if (progress >= 1f)
                {
                    effect.Root.SetActive(false);
                    continue;
                }

                Animate(effect, progress);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Root.SetActive(false);
            }
        }

        private void Animate(Effect effect, float progress)
        {
            float alpha = progress < .75f ? 1f : 1f - ((progress - .75f) / .25f);
            float scale = 1f;
            if (effect.Kind == ReplayVfxKind.Travel)
            {
                float eased = 1f - ((1f - progress) * (1f - progress));
                effect.Rect.anchoredPosition =
Vector2.LerpUnclamped(effect.Start,
effect.End,
eased) +
effect.Transform.Offset;
            }
            else
            {
                scale = Mathf.LerpUnclamped(.35f, 1f, Mathf.Min(1f, progress * 5f));
                effect.Rect.anchoredPosition = effect.Start + effect.Transform.Offset;
            }

            Color color = effect.Image.color;
            color.a = alpha;
            effect.Image.color = color;
            effect.Rect.localScale =
new Vector3(effect.Transform.LocalScale.x * scale,
effect.Transform.LocalScale.y * scale,
1f);
            effect.Rect.localEulerAngles = new Vector3(0f, 0f, effect.Transform.Rotation);
        }

        private Effect Acquire()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (!effects[i].Root.activeSelf)
                {
                    return effects[i];
                }
            }

            Effect oldest = effects[0];
            for (int i = 1; i < effects.Count; i++)
            {
                if (effects[i].StartMs < oldest.StartMs)
                {
                    oldest = effects[i];
                }
            }

            return oldest;
        }

        private Effect CreateEffect()
        {
            Image image;
            if (effectPrefab != null)
            {
                image = Instantiate(effectPrefab, transform);
            }
            else
            {
                // Compatibility for the temporary VfxLab bootstrap; production replay uses the prefab reference.
                image = ReplayUi.Image(transform, "Replay VFX", Color.white);
            }

            image.raycastTarget = false;
            image.gameObject.SetActive(false);
            var effect = new Effect(image);
            effects.Add(effect);
            return effect;
        }

        private Sprite ResolveSprite(string key, ReplayVfxKind kind, out bool hasArt)
        {
            if (iconLibrary != null && iconLibrary.TryGet(key, out Sprite sprite))
            {
                hasArt = true;
                return sprite;
            }

            hasArt = false;
            return kind == ReplayVfxKind.Travel ? fallbackRounded : fallbackCircle;
        }

        private static Color ResolveColor(ReplayVfxKind kind, Color fallbackColor)
        {
            if (kind == ReplayVfxKind.Burst)
            {
                return fallbackColor;
            }

            if (kind == ReplayVfxKind.Parry)
            {
                return ReplayStyle.Gold;
            }

            if (kind == ReplayVfxKind.Hit)
            {
                return ReplayStyle.Hp;
            }

            return fallbackColor;
        }

        private Vector2 Center(RectTransform rect)
        {
            Vector3 world = rect.TransformPoint(rect.rect.center);
            return layerRect.InverseTransformPoint(world);
        }

        public static int GetDuration(ReplayVfxKind kind, string eventKind)
        {
            if (kind == ReplayVfxKind.Travel)
            {
                return string.Equals(eventKind,
"ULTIMATE",
System.StringComparison.OrdinalIgnoreCase) ?
ReplayStyle.VfxUltimateDurationMs :
ReplayStyle.VfxBasicDurationMs;
            }

            if (kind == ReplayVfxKind.Burst)
            {
                return ReplayStyle.VfxBurstDurationMs;
            }

            if (kind == ReplayVfxKind.Parry)
            {
                return ReplayStyle.VfxParryDurationMs;
            }

            return ReplayStyle.VfxHitDurationMs;
        }

        public static Vector2 GetSize(ReplayVfxKind kind, string eventKind)
        {
            if (kind == ReplayVfxKind.Travel)
            {
                return string.Equals(eventKind,
"ULTIMATE",
System.StringComparison.OrdinalIgnoreCase) ?
ReplayStyle.VfxUltimateSize :
ReplayStyle.VfxBasicSize;
            }

            if (kind == ReplayVfxKind.Burst)
            {
                return ReplayStyle.VfxBurstSize;
            }

            if (kind == ReplayVfxKind.Parry)
            {
                return ReplayStyle.VfxParrySize;
            }

            return ReplayStyle.VfxHitSize;
        }

        private static Sprite CreateFallbackSprite(bool circle)
        {
            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = circle ? "Replay VFX Circle" : "Replay VFX Rounded";
            texture.hideFlags = HideFlags.HideAndDontSave;
            Color32 clear = new Color32(255, 255, 255, 0);
            Color32 white = new Color32(255, 255, 255, 210);
            float radius = circle ? 15f : 7f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = circle ? x - 15.5f : Mathf.Max(Mathf.Abs(x - 15.5f) - 8.5f, 0f);
                    float py = circle ? y - 15.5f : Mathf.Max(Mathf.Abs(y - 15.5f) - 8.5f, 0f);
                    texture.SetPixel(x, y, (px * px) + (py * py) <= radius * radius ? white : clear);
                }
            }

            texture.Apply(false, true);
            return Sprite.Create(texture,
new Rect(0f,
0f,
size,
size),
new Vector2(.5f,
.5f),
100f,
0,
SpriteMeshType.FullRect);
        }

        private sealed class Effect
        {
            public Effect(Image image)
            {
                Image = image;
                Rect = image.rectTransform;
                Root = image.gameObject;
            }

            public readonly Image Image;
            public readonly RectTransform Rect;
            public readonly GameObject Root;
            public ReplayVfxKind Kind;
            public int StartMs;
            public int DurationMs;
            public Vector2 Start;
            public Vector2 End;
            public bool Mirror;
            public VfxTransform Transform;
        }
    }
}