using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Bag.View;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

namespace OneBagJianghu.Tests.UI
{
    public sealed class GameplayPrefabTests
    {
        [TestCase("Assets/_Game/Prefabs/UI/Bag/BagLoadoutPanel.prefab", typeof(BagLoadoutPanel))]
        [TestCase("Assets/_Game/Prefabs/UI/Bag/BagGrid.prefab", typeof(BagGridView))]
        [TestCase("Assets/_Game/Prefabs/UI/Bag/ItemView.prefab", typeof(ItemView))]
        [TestCase("Assets/_Game/Prefabs/UI/Bag/ItemTray.prefab", typeof(ItemTrayView))]
        [TestCase("Assets/_Game/Prefabs/UI/Bag/BagHighlight.prefab", typeof(Image))]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab", typeof(ReplayScreen))]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/FighterPanel.prefab", typeof(FighterPanelView))]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/ReplayItemView.prefab", typeof(ReplayItemView))]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/VfxEffect.prefab", typeof(Image))]
        [TestCase("Assets/_Game/Prefabs/UI/Replay/ReplayLogRow.prefab", typeof(Text))]
        public void PrefabHasComponentAndAssignedReferences(string path, System.Type type)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, path);
            Component component = prefab.GetComponentInChildren(type, true);
            Assert.That(component, Is.Not.Null, type.Name);
            // Built-in Unity components (Image, Text) legitimately leave optional fields such as sprite/material empty.
            if (type.Namespace == null || !type.Namespace.StartsWith("OneBagJianghu"))
                return;
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() != null &&
                    typeof(Object).IsAssignableFrom(field.FieldType))
                {
                    Assert.That(field.GetValue(component),
                        Is.Not.Null,
                        type.Name + "." + field.Name);
                }
            }
        }

        [Test]
        public void GameplaySceneIsStructuredAndFullyWired()
        {
            var scene =
                EditorSceneManager.OpenScene("Assets/_Game/Scenes/GameplayScene.unity",
                OpenSceneMode.Single);
            EventSystem[] events =
                Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            Assert.That(events, Has.Length.EqualTo(1));
            Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            Assert.That(scaler.referenceResolution,
                Is.EqualTo(new Vector2(1920f,
                1080f)));
            Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(.5f));
            GameplayController controller =
                Object.FindFirstObjectByType<GameplayController>(FindObjectsInactive.Include);
            Assert.That(controller, Is.Not.Null);
            Assert.That(GameObject.Find("Canvas/Preparation"), Is.Null);
            foreach (FieldInfo field in typeof(GameplayController).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() != null &&
                    typeof(Object).IsAssignableFrom(field.FieldType))
                {
                    Assert.That(field.GetValue(controller), Is.Not.Null, field.Name);
                }
            }

            Assert.That(System.Array.Exists(scene.GetRootGameObjects(),
                root =>
                root.name == "GameplayScene"),
                Is.False);
            Image background = GameObject.Find("Canvas/Background").GetComponent<Image>();
            if (File.Exists("Assets/_Game/Textures2D/UI/bg_battle.png"))
            {
                Assert.That(background.sprite, Is.Not.Null);
            }
        }

        [Test]
        public void ReplayPrefabHasBattleBandBarsAndPortraitVfxTargets()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab");
            Assert.That(prefab.transform.Find("TopBar/ClockPlate/Clock"), Is.Not.Null);
            Assert.That(prefab.transform.Find("TopBar/ClockPlate/StageOpponentName"), Is.Not.Null);
            Assert.That(prefab.transform.Find("TopBar/ExitButton"), Is.Not.Null);
            Assert.That(prefab.transform.Find("TopBar/Vs Emblem"), Is.Not.Null);
            Assert.That(prefab.transform.Find("BottomBar/SpeedButtons"), Is.Not.Null);
            Assert.That(prefab.transform.Find("BottomBar/LogToggle"), Is.Not.Null);
            Assert.That(prefab.transform.Find("BottomBar/SpeedButtons/Restart"), Is.Null);
            Assert.That(prefab.transform.Find("ExpandedBattleLog/Viewport/Content"), Is.Not.Null);
            // Each log frame has art drawn at its own aspect (decision #66).
            Assert.That(prefab.transform.Find("BottomBar/LogToggle").GetComponent<Image>().sprite.name,
                Is.EqualTo("log_panel_wide"));
            Assert.That(prefab.transform.Find("ExpandedBattleLog").GetComponent<Image>().sprite.name,
                Is.EqualTo("log_panel_expanded"));
            Assert.That(prefab.transform.Find("TopBar/ExitButton").GetComponent<Image>().sprite.name,
                Is.EqualTo("button_exit"));
            Assert.That(prefab.transform.Find("TopBar/ClockPlate/Clock").GetComponent<Outline>(), Is.Not.Null);
            Assert.That(prefab.transform.Find("TopBar/ClockPlate/StageOpponentName").GetComponent<Outline>(),
                Is.Not.Null);
            foreach (string side in new[]
            {
                "Player",
                "Enemy"
            }

            )
            {
                Transform root = prefab.transform.Find("MiddleBand/" + side);
                Transform bag = root.Find("Bag");
                Assert.That(bag, Is.Not.Null, side + " Bag");
                Assert.That(((RectTransform)bag).sizeDelta.x,
                    Is.InRange(560f,
                    620f),
                    side +
                    " Bag width");
                Transform avatar = root.Find("Avatar");
                Assert.That(avatar.Find("Portrait"), Is.Not.Null, side + " Portrait");
                Assert.That(avatar.Find("Portrait/HeroArt").GetComponent<Image>().sprite,
                    Is.Not.Null,
                    side +
                    " portrait sprite");
                RectTransform backing = (RectTransform)avatar.Find("Portrait/Backing");
                Assert.That(backing.offsetMin.x, Is.GreaterThanOrEqualTo(20f));
                Assert.That(avatar.Find("TitleStrip").GetComponent<Image>().sprite.name, Is.EqualTo("name_plate"));
                RectTransform avatarRect = (RectTransform)avatar;
                RectTransform bagRect = (RectTransform)bag;
                bool separated = side == "Player" ?
                    bagRect.anchoredPosition.x > avatarRect.anchoredPosition.x + avatarRect.sizeDelta.x :
                    avatarRect.anchoredPosition.x > bagRect.anchoredPosition.x + bagRect.sizeDelta.x;
                Assert.That(separated, Is.True, side + " avatar/bag gap");
                string hudName = side == "Player" ? "PlayerHud" : "EnemyHud";
                Transform hud = prefab.transform.Find("TopBar/" + hudName);
                Assert.That(hud, Is.Not.Null);
                Assert.That(hud.Find("HpBar"), Is.Not.Null);
                Assert.That(hud.Find("ShieldBar"), Is.Not.Null);
                Assert.That(hud.Find("QiBar"), Is.Not.Null);
                Assert.That(hud.Find("StatusChips"), Is.Not.Null);
                Assert.That(hud.Find("StatusChips/Burn/Icon"), Is.Not.Null);
                Assert.That(hud.Find("StatusChips/Burn/CountBadge/Count"), Is.Not.Null);
                Assert.That(hud.GetComponent<Image>().sprite.name, Is.EqualTo("hud_panel_simple"));
                Assert.That(hud.Find("HpBar").GetComponent<Image>().sprite.name,
                    Is.EqualTo("gauge_frame_large"));
                Assert.That(hud.Find("ShieldBar").GetComponent<Image>().sprite.name,
                    Is.EqualTo("gauge_frame_small"));
                Assert.That(avatar.Find("HpBar"), Is.Null);
                Image hpFill = hud.Find("HpBar/Fill").GetComponent<Image>();
                int expectedOrigin = side == "Player" ?
                    (int)Image.OriginHorizontal.Left :
                    (int)Image.OriginHorizontal.Right;
                Assert.That(hpFill.fillOrigin, Is.EqualTo(expectedOrigin));
                FighterPanelView fighter = avatar.GetComponent<FighterPanelView>();
                Assert.That(fighter.VfxTarget,
                    Is.SameAs(avatar.Find("Portrait/HeroArt").GetComponent<RectTransform>()));
            }
        }

        [Test]
        public void ReplayItemPrefabHasVerticalCooldownOverlay()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/UI/Replay/ReplayItemView.prefab");
            Transform overlayTransform = prefab.transform.Find("CooldownOverlay");
            Assert.That(overlayTransform, Is.Not.Null);
            Image overlay = overlayTransform.GetComponent<Image>();
            Assert.That(overlay.type, Is.EqualTo(Image.Type.Filled));
            Assert.That(overlay.fillMethod, Is.EqualTo(Image.FillMethod.Vertical));
            Assert.That(overlay.fillOrigin,
                Is.EqualTo((int)Image.OriginVertical.Bottom));
            Assert.That(overlay.raycastTarget, Is.False);
            Assert.That(overlayTransform.GetSiblingIndex(),
                Is.LessThan(prefab.transform.Find("Name").GetSiblingIndex()));
            Assert.That(overlayTransform.GetSiblingIndex(),
                Is.LessThan(prefab.transform.Find("Status").GetSiblingIndex()));
            Assert.That(overlayTransform.GetSiblingIndex(),
                Is.LessThan(prefab.transform.Find("FlashOverlay").GetSiblingIndex()));
        }
    }
}
