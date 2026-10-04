using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Core;
using OneBagJianghu.Engine.Unity;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Economy;
using OneBagJianghu.Bag;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.Audio;
using OneBagJianghu.Onboarding;

namespace OneBagJianghu.UI.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField]
        private GameObject replayRoot;
        [SerializeField]
        private ReplayScreen replayScreen;
        [SerializeField]
        private Image battleBackground;
        private string opponentName = "Đối thủ";
        private long battleSeed;
        private BattleLog completedLog;
        private PveStage pveStage;
        private PveStage pveContinuationStage;
        private void Start()
        {
            GameSession session = GameBootstrap.Session;
            replayRoot.SetActive(false);
            SceneFlow.NotifyReady();
            if (!session.TryValidateLoadout(out string warning))
            {
                MessageAndReturnHome("Không thể xuất chiêu", warning);
                return;
            }

            BeginBattle();
        }

        private void BeginBattle()
        {
            GameSession session = GameBootstrap.Session;
            if (session.SelectedMode == GameMode.Pve)
            {
                pveStage = PveCatalog.Load().GetStage(session.CurrentPveStageId);
                if (pveStage == null)
                {
                    MessageAndReturnHome("Không thể xuất chiêu", "Ải Hành Tẩu không hợp lệ.");
                    return;
                }

                ApplyBattleBackground(pveStage.Chapter.Theme.BattleBackground);
                if (pveStage.Id == "hoi_1_01")
                    OnboardingRuntime.Publish("first_battle_started");
            }

            Debug
                .Log(
                    $"[Battle] Start mode={session.SelectedMode}, player={session.HeroName}, " +
                    $"loadout={session.Profile.Loadout.Count} items");
            if (session.SelectedMode == GameMode.Pve)
            {
                try
                {
                    LocalBattleOutput stageOutput = PveBattleService.RunBattle(session.CurrentLoadoutJson, pveStage);
                    HandleBattleResult(BattleLogParser.Parse(stageOutput.Result.ToString()), stageOutput.Opponent);
                }
                catch (OneBagJianghu.Engine.DataException exception)
                {
                    MessageAndReturnHome("Không thể xuất chiêu", exception.Message);
                }

                return;
            }

            if (GameBootstrap.Network.State == NetworkState.Online)
            {
                GameBootstrap.Network.RequestBattle(session.SelectedMode == GameMode.Pvp ? "pvp" : "pve",
                    session.CurrentLoadoutJson,
                    HandleBattleResult,
                    HandleBattleError);
                return;
            }

            try
            {
                LocalBattleOutput output = LocalBattleService.RunBattle(session.CurrentLoadoutJson,
                    session.SelectedMode);
                HandleBattleResult(BattleLogParser.Parse(output.Result.ToString()), output.Opponent);
            }
            catch (OneBagJianghu.Engine.DataException exception)
            {
                MessageAndReturnHome("Không thể xuất chiêu", exception.Message);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
                ShowFallbackAndPlaySample();
            }
        }

        private void HandleBattleResult(BattleLog log, OpponentInfo opponent)
        {
            opponentName = string.IsNullOrWhiteSpace(opponent?.Name) ? "Đối thủ" : opponent.Name;
            if (log.Names.Count > 1)
                log.Names[1] = string.IsNullOrWhiteSpace(opponent?.Sect) ? opponentName : opponentName
                    + " • "
                    + opponent.Sect;
            ApplyKnownRarities(log);
            PlayBattle(log);
        }

        private void ApplyKnownRarities(BattleLog log)
        {
            var player = new System.Collections.Generic.List<string>();
            PlayerProfile profile = GameBootstrap.Session.Profile;
            foreach (LoadoutPlacement placement in profile.Loadout)
                player.Add(profile.RequireItem(placement.Uid).Rarity);
            System.Collections.Generic.List<string> enemy = null;
            if (pveStage?.Enemy?.Items != null)
            {
                enemy = new System.Collections.Generic.List<string>();
                foreach (PveEnemyItem item in pveStage.Enemy.Items)
                    enemy.Add(item.Rarity);
            }

            BattleLogRarityMapper.Apply(log, player, enemy);
        }

        private void HandleBattleError(string message)
        {
            if (GameBootstrap.Network.State == NetworkState.Online)
                Message("Máy chủ từ chối", message);
            else
                ShowFallbackAndPlaySample();
        }

        private void ShowFallbackAndPlaySample()
        {
            TextAsset asset = Resources.Load<TextAsset>("Replays/replay_sample");
            if (asset == null)
            {
                Debug.LogError("Replay sample was not found.");
                ShowResult(-1);
                return;
            }

            opponentName = "Đối thủ";
            PlayBattle(BattleLogParser.Parse(asset.text));
        }

        private void PlayBattle(BattleLog log)
        {
            completedLog = log;
            battleSeed = log.Seed;
            int playerItems = 0, opponentItems = 0;
            foreach (BattleItemRef item in log.Items)
            {
                if (item.Side == 0)
                    playerItems++;
                else if (item.Side == 1)
                    opponentItems++;
            }

            Debug
                .Log(
                    $"[Battle] Start resolved mode={GameBootstrap.Session.SelectedMode}, seed={log.Seed}, " +
                    $"player={log.Names[0]}({playerItems} items), opponent={log.Names[1]}({opponentItems} items)");
            replayRoot.SetActive(true);
            bool bossStage = GameBootstrap.Session.SelectedMode == GameMode.Pve && pveStage.Kind == "boss";
            string musicKey = GameBootstrap.Session.SelectedMode == GameMode.Pvp
                ? MusicKeys.Pvp
                : MusicKeys.ForStage(bossStage);
            AudioManager.EnsureExists().PlayMusic(musicKey);
            replayScreen.Initialize(log,
                OneBagJianghu.Bag.ItemCatalog.LoadFromResources(),
                bossStage);
            ApplyPvePortraits();
            string mode = GameBootstrap.Session.SelectedMode == GameMode.Pvp ? "Luận Kiếm Đài" : pveStage.Chapter.Title
                + " · "
                + pveStage.Name;
            replayScreen.SetStageLabel(mode);
            replayScreen.Completed -= ShowResult;
            replayScreen.Completed += ShowResult;
            replayScreen.Play();
            replayScreen.ExitRequested -= ConfirmExit;
            replayScreen.ExitRequested += ConfirmExit;
        }

        private void ConfirmExit() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Thoát trận",
            "Rời trận và trở về Sảnh?",
            "Thoát",
            "Ở lại",
            yes =>
        {
            if (yes)
                SceneFlow.Go(GameScene.Home);
        }));
        private void ShowResult(int winner)
        {
            AudioManager.Instance?.PlaySting(MusicKeys.ForResult(winner == 0));
            string result = winner < 0 ? "Hòa" : winner == 0 ? "Thắng" : "Bại";
            string winnerName = winner == 0 ? GameBootstrap.Session.HeroName : winner > 0 ? opponentName : string.Empty;
            string rewardText = string.Empty;
            if (GameBootstrap.Session.SelectedMode == GameMode.Pve && pveStage != null)
            {
                HandlePveCompletion(winner, result, winnerName);
                return;
            }

            ResultPopup popup = PopupManager.Show<ResultPopup>(new ResultPopupData(result, winnerName, rewardText));
            if (popup != null)
            {
                popup.Closed -= HandleResultClosed;
                popup.Closed += HandleResultClosed;
            }
        }

        private void HandlePveCompletion(int winner, string result, string winnerName)
        {
            bool won = winner == 0;
            PveCatalog catalog = PveCatalog.Load();
            var lines = won ? pveStage.Dialogue?.AfterWin : pveStage.Dialogue?.AfterLoss;
            System.Action finish = () => ShowPveResult(won, result, winnerName);
            if (lines != null && lines.Count > 0)
            {
                DialoguePopup dialogue = PopupManager.Show<DialoguePopup>(new DialoguePopupData(catalog, lines));
                if (dialogue != null)
                {
                    System.Action<BasePopup> closed = null;
                    closed = popup =>
                    {
                        popup.Closed -= closed;
                        finish();
                    };
                    dialogue.Closed += closed;
                }
                else
                    finish();
            }
            else
                finish();
        }

        private void ShowPveResult(bool won, string result, string winnerName)
        {
            string rewardText = string.Empty;
            PlayerProfile profile = GameBootstrap.Session.Profile;
            if (won)
            {
                bool first = !PveRules.IsCleared(profile, pveStage.Id);
                int hp = completedLog != null && completedLog.FinalHp.Count > 0 ? completedLog.FinalHp[0] : 0;
                int maxHp = 0;
                if (completedLog != null)
                    foreach (BattleEvent e in completedLog.Events)
                        if (e.Side == 0 && e.MaxHp.HasValue)
                        {
                            maxHp = e.MaxHp.Value;
                            break;
                        }

                int stars = PveRules.Stars(true, hp, maxHp);
                if (!profile.Pve.Stages.TryGetValue(pveStage.Id, out PveStageProgress progress))
                    profile.Pve.Stages[pveStage.Id] = progress = new PveStageProgress();
                progress.Cleared = true;
                progress.BestStars = Mathf.Max(progress.BestStars, stars);
                string today = System.DateTimeOffset.UtcNow.UtcDateTime.ToString("yyyy-MM-dd");
                if (profile.Pve.DailyFirstClearDate != today)
                {
                    profile.Pve.DailyFirstClearDate = today;
                    profile.Pve.DailyFirstClears.Clear();
                }

                if (!profile.Pve.DailyFirstClears.Contains(pveStage.Id))
                    profile.Pve.DailyFirstClears.Add(pveStage.Id);
                PveRewardResult reward = new PveRewardService(ItemCatalog.LoadFromResources(),
                    PveCatalog.Load().RepeatRewardPercent).Grant(profile,
                    pveStage,
                    first,
                    unchecked((int)battleSeed));
                AudioManager.Instance?.Play("star_rating");
                if (reward.Success)
                    AudioManager.Instance?.Play("reward");
                if (reward.NewLevel > reward.OldLevel)
                    AudioManager.Instance?.Play("level_up");
                if (first)
                    AudioManager.Instance?.Play("node_unlock");
                rewardText = reward.Success
                    ? $"{new string ('★', stars)}  Thưởng: {UiFormat.Number(reward.Silver)} Bạc · "
                        + $"{reward.HeroExp} EXP"
                        + (reward.NewLevel > reward.OldLevel ? $"\nLên cấp! Cấp {reward.NewLevel}" : string.Empty)
                    : "Thưởng chưa nhận: "
                        + reward.Error;
                Debug
                    .Log(
                        $"[Pve] Clear stage={pveStage.Id}, stars={stars}, first={first}, " +
                        $"rewardSuccess={reward.Success}");
            }

            GameBootstrap.Session.SaveProfile();
            PveStage target = won ? PveStageFlow.GetNext(pveStage, PveCatalog.Load()) : pveStage;
            pveContinuationStage = target;
            string blocked = PveStageFlow.GetBlockedReason(target);
            var data = new ResultPopupData(result, winnerName, rewardText)
            {
                FirstLabel = "Về bản đồ",
                SecondLabel = target == null ? "Đã hết ải" : won ? "Ải tiếp theo" : "Đánh lại",
                FirstChoice = ResultPopupChoice.Map,
                SecondChoice = ResultPopupChoice.Continue,
                SecondEnabled = string.IsNullOrEmpty(blocked),
                DisabledReason = blocked
            };
            ResultPopup popup = PopupManager.Show<ResultPopup>(data);
            if (popup != null)
            {
                popup.Closed -= HandleResultClosed;
                popup.Closed += HandleResultClosed;
            }

            // Published after the result popup is up, so the tutorial can point at its buttons.
            if (won && pveStage?.Id == "hoi_1_01")
                OnboardingRuntime.Publish("first_battle_won");
        }

        private void ApplyBattleBackground(string key)
        {
            if (battleBackground == null)
                return;
            PveArtLibrary library = PveArtLibrary.Load();
            Sprite sprite;
            if (library != null && (library.TryGet(key, out sprite) || library.TryGet("bg_battle", out sprite)))
            {
                battleBackground.sprite = sprite;
                battleBackground.color = Color.white;
            }
        }

        private void ApplyPvePortraits()
        {
            if (pveStage == null)
                return;
            PveArtLibrary pveArt = PveArtLibrary.Load();
            ItemIconLibrary itemArt = ItemIconLibrary.Load();
            FighterPanelView[] panels = replayScreen.GetComponentsInChildren<FighterPanelView>(true);
            foreach (FighterPanelView panel in panels)
            {
                bool enemy = panel.transform.parent != null && panel.transform.parent.name == "Enemy";
                string key = enemy ? pveStage.Enemy.Portrait : "hero_portrait";
                Sprite sprite = null;
                if (pveArt != null)
                    pveArt.TryGet(key, out sprite);
                if (sprite == null && itemArt != null)
                    itemArt.TryGet(key, out sprite);
                if (sprite == null)
                    continue;
                Transform art = panel.transform.Find("Portrait/HeroArt");
                if (art == null)
                    continue;
                Image image = art.GetComponent<Image>();
                image.sprite = sprite;
                image.color = Color.white;
                image.transform.localScale = Vector3.one;
            }
        }

        private static string FormatMaterials(System.Collections.Generic.IReadOnlyDictionary<string, int> materials)
        {
            var values = new System.Collections.Generic.List<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, int> value in materials)
                values.Add(value.Key + "x" + value.Value);
            return string.Join(",", values);
        }

        private void HandleResultClosed(BasePopup popup)
        {
            popup.Closed -= HandleResultClosed;
            OnboardingRuntime.Publish("result_closed");
            if (!(popup.Result is ResultPopupChoice choice))
                return;
            if (GameBootstrap.Session.SelectedMode != GameMode.Pve)
            {
                SceneFlow.Go(choice == ResultPopupChoice.Home ? GameScene.Home : GameScene.Gameplay);
                return;
            }

            PveCatalog catalog = PveCatalog.Load();
            PveStage current = catalog.GetStage(GameBootstrap.Session.CurrentPveStageId);
            if (choice == ResultPopupChoice.Map)
            {
                GameBootstrap.Session.PendingPveMapStageId = current?.Id;
                SceneFlow.Go(GameScene.Home);
                return;
            }

            if (choice == ResultPopupChoice.Continue)
            {
                PveStageFlow.TryStart(pveContinuationStage);
            }
        }

        private static void Message(string title,
            string body) => PopupManager.Show<MessagePopup>(new MessagePopupData(title,
            body,
            "Đóng"));
        private static void MessageAndReturnHome(string title, string body)
        {
            MessagePopup popup = PopupManager.Show<MessagePopup>(new MessagePopupData(title, body, "Về Sảnh"));
            if (popup == null)
            {
                SceneFlow.Go(GameScene.Home);
                return;
            }

            popup.Closed += ReturnHome;
        }

        private static void ReturnHome(BasePopup popup)
        {
            popup.Closed -= ReturnHome;
            SceneFlow.Go(GameScene.Home);
        }
    }
}
