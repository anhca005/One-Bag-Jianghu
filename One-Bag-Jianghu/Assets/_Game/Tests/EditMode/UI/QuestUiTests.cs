using System.Reflection;
using NUnit.Framework;
using OneBagJianghu.Quests;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Home;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Tests.UI
{
    public sealed class QuestUiTests
    {
        [Test]
        public void TrackerHasNoClaimButtonField()
        {
            // Arrange
            FieldInfo field = typeof(QuestTracker).GetField(
                "claimButton",
                BindingFlags.Instance | BindingFlags.NonPublic);

            // Act
            bool hasClaimButton = field != null;

            // Assert
            Assert.That(hasClaimButton, Is.False);
        }

        [Test]
        public void BorderRunnerOnlyShowsLightsWhileRunning()
        {
            // Arrange
            GameObject root = new GameObject("Runner", typeof(RectTransform), typeof(Image), typeof(UiBorderRunner));
            UiBorderRunner runner = root.GetComponent<UiBorderRunner>();
            RectTransform light = new GameObject("Light", typeof(RectTransform)).GetComponent<RectTransform>();
            light.SetParent(root.transform, false);
            SetField(runner, "frame", root.GetComponent<Image>());
            SetField(runner, "lights", new[] { light });

            // Act
            runner.SetRunning(true);

            // Assert
            Assert.That(runner.IsRunning, Is.True);
            Assert.That(light.gameObject.activeSelf, Is.True);
            runner.SetRunning(false);
            Assert.That(light.gameObject.activeSelf, Is.False);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void LogRowShowsClaimAndRunnerOnlyForDoneActiveQuest()
        {
            // Arrange
            QuestLogRow row = CreateRow();
            var quest = new QuestDefinition
            {
                Id = "q_test",
                Kind = "main",
                Title = "Thử nhiệm vụ",
                Text = "Lời dẫn",
                Reward = new QuestReward()
            };

            // Act
            row.Bind(quest, new QuestProgress(1, 1), false, true, "Người giao", "Mục tiêu");

            // Assert
            Assert.That(row.ClaimButton.gameObject.activeSelf, Is.True);
            Assert.That(row.Runner.IsRunning, Is.True);
            row.Bind(quest, new QuestProgress(0, 1), false, false, "Người giao", "Mục tiêu");
            Assert.That(GetField<Text>(row, "titleText").text, Is.EqualTo("???"));
            Assert.That(row.ClaimButton.gameObject.activeSelf, Is.False);
            Object.DestroyImmediate(row.gameObject);
        }

        [Test]
        public void LogRowCountExcludesTemplate()
        {
            // Arrange
            GameObject root = new GameObject("Popup", typeof(RectTransform), typeof(QuestLogPopup));
            QuestLogPopup popup = root.GetComponent<QuestLogPopup>();
            RectTransform content = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
            content.SetParent(root.transform, false);
            new GameObject("Template", typeof(RectTransform)).transform.SetParent(content, false);
            new GameObject("Row 1", typeof(RectTransform)).transform.SetParent(content, false);
            new GameObject("Row 2", typeof(RectTransform)).transform.SetParent(content, false);
            SetField(popup, "content", content);

            // Act
            int count = popup.RowCount;

            // Assert
            Assert.That(count, Is.EqualTo(2));
            Object.DestroyImmediate(root);
        }

        private static QuestLogRow CreateRow()
        {
            GameObject root = new GameObject("Row", typeof(RectTransform), typeof(QuestLogRow));
            QuestLogRow row = root.GetComponent<QuestLogRow>();
            SetField(row, "statusIcon", Child<Image>(root, "Status"));
            SetField(row, "badgeImage", Child<Image>(root, "Badge"));
            SetField(row, "badgeText", Child<Text>(root, "Badge Text"));
            SetField(row, "titleText", Child<Text>(root, "Title"));
            SetField(row, "giverText", Child<Text>(root, "Giver"));
            SetField(row, "descriptionText", Child<Text>(root, "Description"));
            SetField(row, "objectiveText", Child<Text>(root, "Objective"));
            RectTransform rewards = new GameObject("Rewards", typeof(RectTransform)).GetComponent<RectTransform>();
            rewards.SetParent(root.transform, false);
            SetField(row, "rewards", rewards);
            GameObject claimObject = new GameObject("Claim", typeof(RectTransform), typeof(UiButton));
            claimObject.transform.SetParent(root.transform, false);
            SetField(row, "claimButton", claimObject.GetComponent<UiButton>());
            GameObject runnerObject = new GameObject("Runner", typeof(RectTransform), typeof(UiBorderRunner));
            runnerObject.transform.SetParent(root.transform, false);
            UiBorderRunner runner = runnerObject.GetComponent<UiBorderRunner>();
            SetField(runner, "lights", new RectTransform[0]);
            SetField(row, "runner", runner);
            return row;
        }

        private static T Child<T>(GameObject parent, string name)
            where T : Component
        {
            var child = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(T));
            child.transform.SetParent(parent.transform, false);
            return child.GetComponent<T>();
        }

        private static T GetField<T>(object target, string name)
        {
            return (T)target.GetType()
                .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(target);
        }

        private static void SetField(object target, string name, object value)
        {
            target.GetType()
                .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(target, value);
        }
    }
}
