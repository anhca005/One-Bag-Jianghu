using System.Reflection;
using System.IO;
using NUnit.Framework;
using OneBagJianghu.Core;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Loading;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class StructuredPrefabTests
    {
        [Test]
        public void GameControllerPrefabContainsRequiredManagersAndCanvases()
        {
            GameObject prefab = Resources.Load<GameObject>("Core/GameController");
            Assert.That(prefab, Is.Not.Null);
            GameController controller = prefab.GetComponent<GameController>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(prefab.transform.childCount, Is.EqualTo(5));
            Assert.That(prefab.transform.Find("GameSession").GetComponent<GameSession>(),
                Is.Not.Null);
            Assert.That(prefab.transform.Find("SceneFlow").GetComponent<SceneFlow>(),
                Is.Not.Null);
            Assert.That(prefab.transform.Find("NetworkService").GetComponent<NetworkService>(),
                Is.Not.Null);
            Assert.That(prefab.transform.Find("PopupManager").GetComponent<PopupManager>(),
                Is.Not.Null);
            Assert.That(prefab.transform.Find("LoadingPanel").GetComponent<LoadingPanel>(),
                Is.Not.Null);
            foreach (FieldInfo field in typeof(GameController).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() != null)
                {
                    Assert.That(field.GetValue(controller),
                        Is.Not.Null,
                        "GameController." + field.Name);
                }
            }

            Canvas[] canvases = prefab.GetComponentsInChildren<Canvas>(true);
            Assert.That(System.Array.Exists(canvases,
                canvas =>
                canvas.sortingOrder >= 100 &&
                canvas.sortingOrder < 1000),
                Is.True);
            Assert.That(System.Array.Exists(canvases,
                canvas =>
                canvas.sortingOrder >= 1000),
                Is.True);
            Image loadingBackground =
                prefab.transform.Find("LoadingPanel/Background").GetComponent<Image>();
            if (File.Exists("Assets/_Game/Textures2D/UI/bg_loading.png"))
            {
                Assert.That(loadingBackground.sprite, Is.Not.Null);
            }
        }

        [TestCase("Button_Primary", "button_primary")]
        [TestCase("Button_Secondary", "button_secondary")]
        public void ButtonPrefabUsesSlicedArtWhenPresent(string prefabName, string artKey)
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/UI/Common/" +
                prefabName +
                ".prefab");
            Image image = prefab.GetComponent<Image>();
            Assert.That(image.type, Is.EqualTo(Image.Type.Sliced));
            if (File.Exists("Assets/_Game/Textures2D/UI/" + artKey + ".png"))
            {
                Assert.That(image.sprite, Is.Not.Null);
            }
        }

        [TestCase(typeof(MessagePopup))]
        [TestCase(typeof(ConfirmPopup))]
        [TestCase(typeof(SettingsPopup))]
        [TestCase(typeof(BagPopup))]
        [TestCase(typeof(ResultPopup))]
        [TestCase(typeof(StoragePopup))]
        [TestCase(typeof(ShopPopup))]
        [TestCase(typeof(DevCheatPanel))]
        public void PopupPrefabHasComponentAndAssignedObjectReferences(System.Type popupType)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/Popups/" + popupType.Name);
            Assert.That(prefab, Is.Not.Null);
            Component popup = prefab.GetComponent(popupType);
            Assert.That(popup, Is.Not.Null);
            for (System.Type type = popupType;
                type != null && typeof(MonoBehaviour).IsAssignableFrom(type);
                type = type.BaseType)
            {
                foreach (FieldInfo field in type.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (field.GetCustomAttribute<SerializeField>() != null &&
                        typeof(Object).IsAssignableFrom(field.FieldType))
                    {
                        Assert.That(field.GetValue(popup),
                            Is.Not.Null,
                            popupType.Name + "." + field.Name);
                    }
                }
            }
        }

        [Test]
        public void ShopPopupUsesThreeColumnCardGridAndWiredCardPrefab()
        {
            GameObject popupPrefab = Resources.Load<GameObject>("UI/Popups/ShopPopup");
            Assert.That(popupPrefab, Is.Not.Null);
            ShopPopup popup = popupPrefab.GetComponent<ShopPopup>();
            RectTransform slots = GetField<RectTransform>(popup, "slotList");
            GridLayoutGroup grid = slots.GetComponent<GridLayoutGroup>();
            Assert.That(grid, Is.Not.Null);
            Assert.That(grid.constraint,
                Is.EqualTo(GridLayoutGroup.Constraint.FixedColumnCount));
            Assert.That(grid.constraintCount, Is.EqualTo(3));
            Assert.That(grid.cellSize, Is.EqualTo(new Vector2(420, 300)));
            ShopCard card = GetField<ShopCard>(popup, "cardPrefab");
            Assert.That(card, Is.Not.Null);
            foreach (FieldInfo field in typeof(ShopCard).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<SerializeField>() != null &&
                    typeof(Object).IsAssignableFrom(field.FieldType))
                {
                    Assert.That(field.GetValue(card), Is.Not.Null, "ShopCard." + field.Name);
                }
            }
        }

        private static T GetField<T>(object target, string name)
            where T : class
        {
            FieldInfo field =
                target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return field.GetValue(target) as T;
        }
    }
}
