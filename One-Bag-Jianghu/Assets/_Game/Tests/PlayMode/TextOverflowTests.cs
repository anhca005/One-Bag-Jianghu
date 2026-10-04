using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Play
{
    public sealed class TextOverflowTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator RealItemTextFitsStorageBagAndShop()
        {
            DeleteProfile();
            yield return Login("Text Overflow");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Session.ResetProfile();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            ItemCatalog catalog = ItemCatalog.LoadFromResources();
            profile.Items.Clear();
            profile.Loadout.Clear();
            profile.Storage.Items.Clear();
            profile.Storage.MaterialStacks.Clear();
            profile.Storage.Overflow.Clear();
            profile.Materials.Clear();
            var grid = new StorageGrid(profile, catalog);
            string[] rarities = RarityRules.Ordered;
            var owned = new List<OwnedItem>();
            var definitions = new List<ItemInfo>(catalog.All);
            definitions.Sort((a, b) => (b.Width * b.Height).CompareTo(a.Width * a.Height));
            for (int i = 0; i < definitions.Count; i++)
            {
                int tier = i == definitions.Count - 1 ? 9 : i % 9 + 1;
                string rarity = i == definitions.Count - 1 ? "supreme" : rarities[i % rarities.Length];
                OwnedItem item = profile.AddItem(definitions[i].Id, tier, rarity: rarity);
                Assert.IsTrue(grid.TryPlaceItem(item),
                    "Every catalog item did not fit in the test storage: " +
                    item.ItemId);
                owned.Add(item);
            }

            GameBootstrap.Session.SaveProfile();
            GetField<HomeHotspot>(home, "storageHotspot").OnClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<StoragePopup>(),
                "StoragePopup did not open.");
            StoragePopup storage = PopupManager.Top as StoragePopup;
            MethodInfo select =
                typeof(StoragePopup).GetMethod("Select",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var failures = new List<string>();
            string longestId = null;
            int longest = -1;
            CheckTexts(home.transform, "HUD", failures);
            foreach (OwnedItem item in owned)
            {
                select.Invoke(storage, new object[] { item.Uid });
                Canvas.ForceUpdateCanvases();
                yield return null;
                CheckTexts(storage.transform, item.ItemId, failures);
                string description = ItemDescriber.Describe(item.ItemId, item.Rarity, item.Tier);
                string visibleDescription =
                    System.Text.RegularExpressions.Regex.Replace(description,
                    "<[^>]+>",
                    string.Empty);
                // Vietnamese has ASCII-only words ("tung", "khi"), so only flag snake_case and known engine identifiers.
                System.Text.RegularExpressions.Match rawWord =
                    System.Text.RegularExpressions.Regex.Match(visibleDescription,
                    @"(?<![\p{L}])([a-z]+_[a-z_]+|[A-Z]+_[A-Z_]+|axe|blade|bow|brush|claw|dual|fan|fist|flute|" +
                    @"hammer|hidden|palm|spear|staff|sword|whip|zither|weapon|armor|manual|pill|relic|burn|poison|" +
                    @"bleed|regen|stun|slow|seal|haste|shield|damage|heal)(?![\p{L}])");
                Assert.IsFalse(rawWord.Success,
                    item.ItemId +
                    " description contains raw ASCII identifier: " +
                    rawWord.Value);
                CheckDetailDoesNotOverlapActions(storage, item.ItemId, failures);
                int length = description.Length;
                if (length > longest)
                {
                    longest = length;
                    longestId = item.Uid;
                }
            }

            select.Invoke(storage, new object[] { longestId });
            Canvas.ForceUpdateCanvases();
            yield return null;
            yield return Capture("StoragePopup_filled.png");
            PopupManager.Close<StoragePopup>();
            yield return null;
            GetField<HomeHotspot>(home, "bagHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<BagPopup>(), "BagPopup did not open.");
            BagPopup bag = PopupManager.Top as BagPopup;
            BagLoadoutPanel bagPanel = GetField<BagLoadoutPanel>(bag, "bagPanel");
            ItemTooltip tooltip = GetField<ItemTooltip>(bag, "itemTooltip");
            SetField(tooltip, "delay", 0f);
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Screen.width * .5f, Screen.height * .5f)
            };
            foreach (OwnedItem item in owned)
            {
                tooltip.ShowDelayed(ItemDescriber.Describe(item.ItemId,
                    item.Rarity,
                    item.Tier),
                    pointer);
                yield return null;
                Canvas.ForceUpdateCanvases();
                CheckTexts(tooltip.transform, "tooltip " + item.ItemId, failures);
                CheckTooltipGeometry(tooltip, item.ItemId, failures);
            }

            profile.HeroLevel = 99;
            bagPanel.Bind(profile);
            Canvas.ForceUpdateCanvases();
            yield return null;
            CheckTexts(bagPanel.transform, "bag stats", failures);
            tooltip.ShowDelayed(ItemDescriber.Describe(profile.FindItem(longestId).ItemId,
                profile.FindItem(longestId).Rarity,
                profile.FindItem(longestId).Tier),
                pointer);
            yield return null;
            Canvas.ForceUpdateCanvases();
            yield return Capture("Tooltip_filled.png");
            PopupManager.Close<BagPopup>();
            yield return null;
            GetField<HomeHotspot>(home, "shopHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ShopPopup>(),
                "ShopPopup did not open.");
            CheckTexts((PopupManager.Top as ShopPopup).transform, "shop", failures);
            Assert.That(failures,
                Is.Empty,
                "Text overflow:\n" + string.Join("\n",
                failures));
        }

        private static void CheckTexts(Transform root, string itemId, List<string> failures)
        {
            foreach (Text text in root.GetComponentsInChildren<Text>(false))
            {
                RectTransform rect = text.rectTransform;
                if (rect.rect.width <= 1f || rect.rect.height <= 1f)
                    continue;
                bool fitted = text.resizeTextForBestFit || text.GetComponent<SingleLineFit>() != null;
                if (text.horizontalOverflow == HorizontalWrapMode.Overflow &&
                    !fitted &&
                    text.preferredWidth > rect.rect.width +
                    1f)
                {
                    failures.Add(itemId +
                        " · " +
                        PathOf(text.transform,
                        root) +
                        " · width " +
                        text.preferredWidth.ToString("0.0") +
                        " > " +
                        rect.rect.width.ToString("0.0"));
                }

                if (text.resizeTextForBestFit)
                {
                    // Best fit shrinks the font when drawing, so preferredHeight (measured at fontSize) overstates it.
                    // What matters is whether every character is still drawn at the minimum size.
                    TextGenerator generator = text.cachedTextGenerator;
                    if (generator.characterCountVisible < generator.characterCount - 1)
                    {
                        failures.Add(itemId +
                            " · " +
                            PathOf(text.transform,
                            root) +
                            " · best fit truncates at " +
                            generator.fontSizeUsedForBestFit +
                            "px (" +
                            generator.characterCountVisible +
                            "/" +
                            generator.characterCount +
                            " chars)");
                    }

                    continue;
                }

                if (text.preferredHeight <= rect.rect.height + 1f)
                    continue;
                ScrollRect scroll = text.GetComponentInParent<ScrollRect>();
                bool growing =
                    scroll != null &&
                    scroll.content != null &&
                    (scroll.content.GetComponent<ContentSizeFitter>() != null ||
                    scroll.content.GetComponent<LayoutGroup>() != null);
                bool masked =
                    scroll != null &&
                    scroll.viewport != null &&
                    (scroll.viewport.GetComponent<Mask>() != null ||
                    scroll.viewport.GetComponent<RectMask2D>() != null);
                if (!growing || !masked)
                {
                    failures.Add(itemId +
                        " · " +
                        PathOf(text.transform,
                        root) +
                        " · height " +
                        text.preferredHeight.ToString("0.0") +
                        " > " +
                        rect.rect.height.ToString("0.0"));
                }
            }
        }

        private static void CheckDetailDoesNotOverlapActions(StoragePopup storage, string itemId, List<string> failures)
        {
            Text detail = GetField<Text>(storage, "detailText");
            Text cost = GetField<Text>(storage, "costText");
            foreach (string field in new[]
            {
                "upgradeButton",
                "mergeButton",
                "sellButton",
                "closeButton"
            }

            )
            {
                Rect button = ScreenRect((RectTransform)GetField<UiButton>(storage, field).transform);
                if (ScreenRect((RectTransform)detail.transform.parent.parent).Overlaps(button))
                {
                    failures.Add(itemId + " · detail overlaps " + field);
                }

                if (ScreenRect(cost.rectTransform).Overlaps(button))
                {
                    failures.Add(itemId + " · cost overlaps " + field);
                }
            }
        }

        private static void CheckTooltipGeometry(ItemTooltip tooltip, string itemId, List<string> failures)
        {
            RectTransform paper = GetField<RectTransform>(tooltip, "panel");
            Text description = GetField<Text>(tooltip, "descriptionText");
            RectTransform viewport = (RectTransform)description.transform.parent.parent;
            Rect paperRect = ScreenRect(paper);
            Rect viewportRect = ScreenRect(viewport);
            Rect textRect = ScreenRect(description.rectTransform);
            const float tolerance = 1f;
            // Screen rects are in pixels; the insets and paper size are in canvas units (CanvasScaler).
            float scale = paper.lossyScale.x <= 0f ? 1f : paper.lossyScale.x;
            float expectedHeight =
                Mathf.Min(description.preferredHeight +
                ItemTooltip.TopInset +
                ItemTooltip.BottomInset,
                Mathf.Max(160f,
                Screen.height / scale * .70f));
            if (paper.rect.height < expectedHeight - tolerance)
            {
                failures.Add("tooltip " +
                    itemId +
                    " · paper height " +
                    paper.rect.height.ToString("0.0") +
                    " < content-sized height " +
                    expectedHeight.ToString("0.0"));
            }

            if (paperRect.xMin < -tolerance ||
                paperRect.yMin < -tolerance ||
                paperRect.xMax > Screen.width +
                tolerance ||
                paperRect.yMax > Screen.height +
                tolerance)
            {
                failures.Add("tooltip " + itemId + " · paper is outside the screen: " + paperRect);
            }

            float px = scale * tolerance;
            if (viewportRect.xMin < paperRect.xMin +
                ItemTooltip.HorizontalInset * scale - px ||
                viewportRect.xMax > paperRect.xMax - ItemTooltip.HorizontalInset * scale +
                px ||
                viewportRect.yMin < paperRect.yMin +
                ItemTooltip.BottomInset * scale - px ||
                viewportRect.yMax > paperRect.yMax - ItemTooltip.TopInset * scale +
                px)
            {
                failures.Add("tooltip " + itemId + " · viewport does not honor paper padding");
            }

            // Text taller than the viewport scrolls, so only the horizontal edges must stay inside.
            if (textRect.xMin < viewportRect.xMin - px || textRect.xMax > viewportRect.xMax + px)
            {
                failures.Add("tooltip " + itemId + " · text is outside its padded paper rect");
            }
        }

        private static Rect ScreenRect(RectTransform value)
        {
            var corners = new Vector3[4];
            value.GetWorldCorners(corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static string PathOf(Transform value, Transform root)
        {
            var parts = new List<string>();
            while (value != null)
            {
                parts.Add(value.name);
                if (value == root)
                    break;
                value = value.parent;
            }

            parts.Reverse();
            return string.Join("/", parts);
        }

        private static IEnumerator Capture(string fileName)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                yield break;
            }

            // WaitForEndOfFrame never resumes in -batchmode, so render overlay canvases through a camera instead.
            yield return null;
            const int width = 1920, height = 1080;
            var target = new RenderTexture(width, height, 24);
            var cameraObject = new GameObject("Capture Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.targetTexture = target;
            var changed = new List<Canvas>();
            foreach (Canvas canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (!canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    continue;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 10f;
                changed.Add(canvas);
            }

            // Let CanvasScaler, layout and text regenerate at the capture resolution before rendering.
            Canvas.ForceUpdateCanvases();
            yield return null;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            var image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            RenderTexture.active = previous;
            foreach (Canvas canvas in changed)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = null;
            }

            camera.targetTexture = null;
            UnityEngine.Object.Destroy(cameraObject);
            target.Release();
            UnityEngine.Object.Destroy(target);
            string folder = Path.Combine(Application.dataPath, "../Logs/Snapshots");
            Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, fileName), image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }

        private static void SetField(object target, string name, object value)
        {
            target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target,
                value);
        }
    }
}