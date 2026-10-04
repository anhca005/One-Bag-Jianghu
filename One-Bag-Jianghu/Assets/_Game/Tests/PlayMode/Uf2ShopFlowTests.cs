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
    public sealed class Uf2ShopFlowTests : SmokeTestBase
    {
        [UnityTest]
        public IEnumerator TabsFollowSelectionAndClosingRefreshesHomeSilver()
        {
            // Arrange
            DeleteProfile();
            yield return Login("UF2 Shop Flow");
            HomeController home = Object.FindFirstObjectByType<HomeController>();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            profile.AddCurrency("bac", 100000);
            GetField<HomeHotspot>(home, "shopHotspot").OnClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ShopPopup>(), "ShopPopup did not open.");
            ShopPopup popup = PopupManager.Top as ShopPopup;
            UiButton trade = GetField<UiButton>(popup, "tradeTab");
            UiButton exchange = GetField<UiButton>(popup, "exchangeTab");
            UiButton reputation = GetField<UiButton>(popup, "reputationTab");
            Sprite primarySprite = trade.GetComponent<Image>().sprite;
            Sprite secondarySprite = exchange.GetComponent<Image>().sprite;
            Color primaryColor = trade.GetComponent<Image>().color;
            Color secondaryColor = exchange.GetComponent<Image>().color;

            // Act: switch both non-default tabs.
            exchange.onClick.Invoke();

            // Assert
            AssertStyle(exchange, primarySprite, primaryColor);
            AssertStyle(trade, secondarySprite, secondaryColor);
            reputation.onClick.Invoke();
            AssertStyle(reputation, primarySprite, primaryColor);
            AssertStyle(exchange, secondarySprite, secondaryColor);

            // Act: buy with silver and close without reloading Home.
            trade.onClick.Invoke();
            ShopCard card = GetField<List<GameObject>>(popup, "tradeCards")[0].GetComponent<ShopCard>();
            card.BuyButton.onClick.Invoke();
            yield return WaitUntil(() => PopupManager.IsOpen<ConfirmPopup>(), "ConfirmPopup did not open.");
            GetField<UiButton>(PopupManager.Top, "confirmButton").onClick.Invoke();
            yield return WaitUntil(() => !PopupManager.IsOpen<ConfirmPopup>(), "ConfirmPopup did not close.");
            profile.Currencies.TryGetValue("bac", out long expectedSilver);
            GetField<UiButton>(popup, "closeButton").onClick.Invoke();
            yield return WaitUntil(() => !PopupManager.IsOpen<ShopPopup>(), "ShopPopup did not close.");

            // Assert
            Text homeSilver = GetField<Text>(home, "silverText");
            Assert.That(homeSilver.text, Does.Contain(UiFormat.Number(expectedSilver)));
        }

        private static void AssertStyle(UiButton button, Sprite sprite, Color color)
        {
            Image image = button.GetComponent<Image>();
            Assert.That(image.sprite, Is.EqualTo(sprite));
            Assert.That(image.color, Is.EqualTo(color));
        }
    }
}
