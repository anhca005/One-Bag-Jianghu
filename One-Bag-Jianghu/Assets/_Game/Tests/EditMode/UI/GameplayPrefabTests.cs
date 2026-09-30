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
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path); Assert.That(prefab, Is.Not.Null, path);
            Component component = prefab.GetComponentInChildren(type, true); Assert.That(component, Is.Not.Null, type.Name);
            // Built-in Unity components (Image, Text) legitimately leave optional fields such as sprite/material empty.
            if (type.Namespace == null || !type.Namespace.StartsWith("OneBagJianghu")) return;
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                if (field.GetCustomAttribute<SerializeField>() != null && typeof(Object).IsAssignableFrom(field.FieldType))
                    Assert.That(field.GetValue(component), Is.Not.Null, type.Name + "." + field.Name);
        }

        [Test]
        public void GameplaySceneIsStructuredAndFullyWired()
        {
            var scene = EditorSceneManager.OpenScene("Assets/_Game/Scenes/GameplayScene.unity", OpenSceneMode.Single);
            EventSystem[] events = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None); Assert.That(events, Has.Length.EqualTo(1));
            Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include); CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            Assert.That(scaler.referenceResolution, Is.EqualTo(new Vector2(1920f, 1080f))); Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(.5f));
            GameplayController controller = Object.FindFirstObjectByType<GameplayController>(FindObjectsInactive.Include); Assert.That(controller, Is.Not.Null);
            Assert.That(GameObject.Find("Canvas/Preparation"), Is.Null);
            foreach (FieldInfo field in typeof(GameplayController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                if (field.GetCustomAttribute<SerializeField>() != null && typeof(Object).IsAssignableFrom(field.FieldType)) Assert.That(field.GetValue(controller), Is.Not.Null, field.Name);
            Assert.That(System.Array.Exists(scene.GetRootGameObjects(), root => root.name == "GameplayScene"), Is.False);
            Image background = GameObject.Find("Canvas/Background").GetComponent<Image>();
            if (File.Exists("Assets/_Game/Textures2D/UI/bg_battle.png")) Assert.That(background.sprite, Is.Not.Null);
        }

        [Test]
        public void ReplayPrefabHasBattleBandBarsAndPortraitVfxTargets()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/UI/Replay/ReplayScreen.prefab");
            Assert.That(prefab.transform.Find("TopBar/Clock"), Is.Not.Null);
            Assert.That(prefab.transform.Find("TopBar/ExitButton"), Is.Not.Null);
            Assert.That(prefab.transform.Find("BottomBar/SpeedButtons"), Is.Not.Null);
            Assert.That(prefab.transform.Find("BottomBar/LogToggle"), Is.Not.Null);
            foreach (string side in new[] { "Player", "Enemy" })
            {
                Transform root = prefab.transform.Find("MiddleBand/" + side);
                Transform bag = root.Find("Bag"); Assert.That(bag, Is.Not.Null, side + " Bag");
                Assert.That(((RectTransform)bag).sizeDelta.x, Is.InRange(560f, 620f), side + " Bag width");
                Transform avatar = root.Find("Avatar");
                Assert.That(avatar.Find("Portrait"), Is.Not.Null, side + " Portrait");
                Assert.That(avatar.Find("Portrait/HeroArt").GetComponent<Image>().sprite, Is.Not.Null, side + " portrait sprite");
                Assert.That(avatar.Find("HpBar"), Is.Not.Null);
                Assert.That(avatar.Find("ShieldBar"), Is.Not.Null);
                Assert.That(avatar.Find("QiBar"), Is.Not.Null);
                Assert.That(avatar.Find("StatusChips"), Is.Not.Null);
                FighterPanelView fighter = avatar.GetComponent<FighterPanelView>();
                Assert.That(fighter.VfxTarget, Is.SameAs(avatar.Find("Portrait/HeroArt").GetComponent<RectTransform>()));
            }
        }
    }
}
