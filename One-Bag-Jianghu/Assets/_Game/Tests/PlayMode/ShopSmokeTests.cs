using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.Play
{
    public sealed class ShopSmokeTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator ShopBuysIntoStorageAndRefusesWhenFull()
        {
            DeleteProfile();
            yield return Login("Shop Smoke");
            HomeController home = UnityEngine.Object.FindFirstObjectByType<HomeController>();
            GameBootstrap.Session.ResetProfile();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.AddCurrency("bac", 100000);
            int before = profile.Items.Count;
            GetField<HomeHotspot>(home, "shopHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ShopPopup>(),
                "ShopPopup did not open.");
            ShopPopup popup = PopupManager.Top as ShopPopup;
            ShopCard card = FirstCard(popup);
            card.BuyButton.onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<ConfirmPopup>(),
                "Buy confirmation did not open.");
            // A real mouse click must reach the confirm button, not the dimmed backdrop drawn behind it.
            Canvas confirmCanvas = PopupManager.Top.GetComponent<Canvas>();
            Assert.IsTrue(confirmCanvas.overrideSorting,
                "The confirm popup does not sort above the backdrop.");
            foreach (Canvas other in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (other != confirmCanvas &&
                    other.overrideSorting &&
                    other.isActiveAndEnabled &&
                    !other.transform.IsChildOf(confirmCanvas.transform))
                {
                    Assert.Less(other.sortingOrder,
                        confirmCanvas.sortingOrder,
                        other.name +
                        " draws above the confirm popup.");
                }
            }

            GetField<UiButton>(PopupManager.Top, "confirmButton").onClick.Invoke();
            yield return WaitUntil(() =>
                !PopupManager.IsOpen<ConfirmPopup>(),
                "Buy confirmation did not close.");
            Assert.AreEqual(before + 1, profile.Items.Count);
            OwnedItem bought = profile.Items[profile.Items.Count - 1];
            Assert.IsNotNull(profile.Storage.Items.Find(value => value.Uid == bought.Uid));
            profile.Storage.Items.Clear();
            profile.Storage.MaterialStacks.Clear();
            profile.Materials.Clear();
            for (int y = 0; y < profile.Storage.Rows; y++)
            {
                for (int x = 0; x < profile.Storage.Cols; x++)
                {
                    profile.Storage.MaterialStacks.Add(new MaterialStack
                    {
                        MaterialId =
                            "han_thiet",
                        Count =
                            1,
                        X =
                            x,
                        Y =
                            y
                    });
                }
            }

            profile.Materials["han_thiet"] = profile.Storage.Rows * profile.Storage.Cols;
            popup.Refresh();
            card = GetField<List<GameObject>>(popup, "tradeCards")[1].GetComponent<ShopCard>();
            Assert.AreEqual("Tàng Bảo Các không đủ chỗ.",
                GetField<Text>(card,
                "disabledReasonText").text);
            card.BuyButton.onClick.Invoke();
            yield return WaitUntil(() =>
                PopupManager.IsOpen<ConfirmPopup>(),
                "Full-storage confirmation did not open.");
            GetField<UiButton>(PopupManager.Top, "confirmButton").onClick.Invoke();
            yield return WaitUntil(() =>
                !PopupManager.IsOpen<ConfirmPopup>(),
                "Full-storage confirmation did not close.");
            Assert.AreEqual("Tàng Bảo Các không đủ chỗ.",
                GetField<Text>(popup,
                "statusText").text);
            Assert.AreEqual(before + 1, profile.Items.Count);
            // Errors and exceptions already fail the test; tagged Debug.Log lines are expected (decision #29).
        }

        private static ShopCard FirstCard(ShopPopup popup) =>
            GetField<List<GameObject>>(popup,
            "tradeCards")[0].GetComponent<ShopCard>();
    }
}