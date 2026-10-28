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
using Newtonsoft.Json.Linq;

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
        private string arenaPortraitId;
        private long battleSeed;
        private BattleLog completedLog;
        private PveStage pveStage;
        private PveStage pveContinuationStage;
        private ArenaResult arenaResult;
        private JObject pveBattle;
        private void Start()
        {
            GameSession session = GameBootstrap.Session;
            replayRoot.SetActive(false);
            SceneFlow.NotifyReady();
            if (session.SelectedMode != GameMode.ArenaReplay
                && !session.TryValidateLoadout(out string warning))
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
                    pveBattle = session.TakePveBattle();
                    if (pveBattle == null || pveBattle["result"] == null)
                    {
                        throw new OneBagJianghu.Engine.DataException("Kết quả PvE từ máy chủ bị thiếu.");
                    }

                    BattleLog log = BattleLogParser.Parse(pveBattle["result"].ToString());
                    OpponentInfo opponent = pveBattle["enemy"]?.ToObject<OpponentInfo>() ?? new OpponentInfo();
                    HandleBattleResult(log, opponent);
                }
                catch (OneBagJianghu.Engine.DataException exception)
                {
                    MessageAndReturnHome("Không thể xuất chiêu", exception.Message);
                }

                return;
            }

            if (session.SelectedMode == GameMode.ArenaReplay)
            {
                ArenaReplayResult replay = session.TakeArenaReplay();
                if (replay == null)
                {
                    MessageAndReturnHome("Không thể xem lại", "Dữ liệu trận xem lại không còn.");
                    return;
                }

                HandleBattleResult(replay.BattleLog, replay.Opponent);
                return;
            }

            arenaResult = session.TakeArenaResult();
            if (arenaResult == null)
            {
                MessageAndReturnHome("Không thể vào trận", "Kết quả Luận Kiếm chưa sẵn sàng.");
                return;
            }

            HandleBattleResult(arenaResult.BattleLog, arenaResult.Opponent);
        }

        private void HandleBattleResult(BattleLog log, OpponentInfo opponent)
        {
            opponentName = ShortFighterName(opponent?.Name);
            // Same id as the arena lobby, so an opponent keeps one face in the lobby and in battle.
            arenaPortraitId = opponent?.Name + ":" + opponent?.Sect;
            // The plaque above the portrait only has room for the name: no sect key, no "· Tâm Pháp" suffix.
            if (log.Names.Count > 1)
                log.Names[1] = opponentName;
            ApplyKnownRarities(log);
            PlayBattle(log);
        }

        private static string ShortFighterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Đối thủ";
            int separator = name.IndexOfAny(new[] { '·', '•' });
            return separator > 0 ? name.Substring(0, separator).Trim() : name.Trim();
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
            MessageAndReturnHome("Luận Kiếm Đài gián đoạn", message);
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
            bool bossStage = GameBootstrap.Session.SelectedMode == GameMode.Pve
                && pveStage != null
                && pveStage.Kind == "boss";
            string musicKey = GameBootstrap.Session.SelectedMode == GameMode.Pvp
                ? MusicKeys.Pvp
                : MusicKeys.ForStage(bossStage);
            AudioManager.EnsureExists().PlayMusic(musicKey);
            replayScreen.Initialize(log,
                OneBagJianghu.Bag.ItemCatalog.LoadFromResources(),
                bossStage);
            ApplyPvePortraits();
            string mode = GameBootstrap.Session.SelectedMode == GameMode.Pve
                ? pveStage.Chapter.Title + " · " + pveStage.Name
                : "Luận Kiếm Đài";
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
            replayScreen.HideTooltip();
            AudioManager.Instance?.PlaySting(MusicKeys.ForResult(winner == 0));
            string result = winner < 0 ? "Hòa" : winner == 0 ? "Thắng" : "Bại";
            string winnerName = winner == 0 ? GameBootstrap.Session.HeroName : winner > 0 ? opponentName : string.Empty;
            string rewardText = string.Empty;
            if (GameBootstrap.Session.SelectedMode == GameMode.Pve && pveStage != null)
            {
                HandlePveCompletion(winner, result, winnerName);
                return;
            }

            bool replay = GameBootstrap.Session.SelectedMode == GameMode.ArenaReplay;
            ResultPopupData data = replay
                ? new ResultPopupData(result, winnerName)
                : ResultPopupData.FromArena(result,
                    winnerName,
                    arenaResult,
                    GameSession.LoadMaterialCatalog());
            data.FirstLabel = "Về Sảnh";
            data.SecondLabel = "Đấu tiếp";
            data.FirstChoice = ResultPopupChoice.Arena;
            data.SecondChoice = ResultPopupChoice.PlayAgain;
            data.SecondEnabled = !replay;
            ResultPopup popup = PopupManager.Show<ResultPopup>(data);
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
            ResultPopupData resultData = null;
            if (won)
            {
                bool first = pveBattle?["firstClear"]?.Value<bool>() ?? false;
                PveRewardResult reward = ReadServerReward(pveBattle?["reward"] as JObject);
                AudioManager.Instance?.Play("reward");
                if (reward.NewLevel > reward.OldLevel)
                    AudioManager.Instance?.Play("level_up");
                if (first)
                    AudioManager.Instance?.Play("node_unlock");
                resultData = ResultPopupData.FromPveReward(result,
                    winnerName,
                    reward,
                    GameSession.LoadMaterialCatalog());
                Debug
                    .Log(
                        $"[Pve] Clear stage={pveStage.Id}, first={first}, rewardSuccess={reward.Success}");
            }

            PveStage target = won ? PveStageFlow.GetNext(pveStage, PveCatalog.Load()) : pveStage;
            pveContinuationStage = target;
            string blocked = PveStageFlow.GetBlockedReason(target);
            var data = resultData ?? new ResultPopupData(result, winnerName, rewardText);
            data.FirstLabel = "Về bản đồ";
            data.SecondLabel = target == null ? "Đã hết ải" : won ? "Ải tiếp theo" : "Đánh lại";
            data.FirstChoice = ResultPopupChoice.Map;
            data.SecondChoice = ResultPopupChoice.Continue;
            data.SecondEnabled = string.IsNullOrEmpty(blocked);
            data.DisabledReason = blocked;
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
            {
                ApplyArenaPortrait();
                return;
            }

            PveArtLibrary pveArt = PveArtLibrary.Load();
            ItemIconLibrary itemArt = ItemIconLibrary.Load();
            FighterPanelView[] panels = replayScreen.GetComponentsInChildren<FighterPanelView>(true);
            foreach (FighterPanelView panel in panels)
            {
                bool enemy = panel.transform.parent != null && panel.transform.parent.name == "Enemy";
                string key = enemy ? pveStage.Enemy.Portrait : "hero_portrait";
                Sprite sprite = null;
                if (pveArt != null)
                {
                    sprite = enemy
                        ? pveArt.ResolveEnemyPortrait(key, pveStage.Id)
                        : pveArt.TryGet(key, out Sprite heroSprite) ? heroSprite : null;
                }
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

        private void ApplyArenaPortrait()
        {
            PveArtLibrary pveArt = PveArtLibrary.Load();
            Sprite sprite = pveArt?.ResolveArenaPortrait(arenaPortraitId);
            if (sprite == null)
                return;
            foreach (FighterPanelView panel in replayScreen.GetComponentsInChildren<FighterPanelView>(true))
            {
                bool enemy = panel.transform.parent != null && panel.transform.parent.name == "Enemy";
                Transform art = enemy ? panel.transform.Find("Portrait/HeroArt") : null;
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
            int pendingChapter = pveStage?.Chapter?.Index ?? 0;
            PlayerProfile profile = GameBootstrap.Session.Profile;
            if (pendingChapter > 0 && profile.PendingStoryChapters.Contains(pendingChapter))
            {
                StoryChapter story = StoryCatalog.Load().Get(pendingChapter);
                StoryPopup storyPopup = story == null ? null : PopupManager.Show<StoryPopup>(story);
                if (storyPopup != null)
                {
                    System.Action<BasePopup> closed = null;
                    closed = closedPopup =>
                    {
                        closedPopup.Closed -= closed;
                        GameBootstrap.Network.SendOnboarding("story_seen",
                            pendingChapter.ToString(),
                            _ => { },
                            Debug.LogWarning);
                        CompleteResultChoice(choice);
                    };
                    storyPopup.Closed += closed;
                    return;
                }

                GameBootstrap.Network.SendOnboarding("story_seen",
                    pendingChapter.ToString(),
                    _ => { },
                    Debug.LogWarning);
            }

            CompleteResultChoice(choice);
        }

        private static PveRewardResult ReadServerReward(JObject payload)
        {
            var reward = new PveRewardResult
            {
                Success = true,
                Silver = payload?["silver"]?.Value<long>() ?? 0,
                HeroExp = payload?["heroExp"]?.Value<long>() ?? 0,
                NewLevel = GameBootstrap.Session.HeroLevel,
                OldLevel = GameBootstrap.Session.HeroLevel - (payload?["levelUps"]?.Value<int>() ?? 0)
            };
            JObject materials = payload?["materials"] as JObject;
            if (materials != null)
            {
                foreach (JProperty property in materials.Properties())
                {
                    reward.Materials[property.Name] = property.Value.Value<int>();
                }
            }

            return reward;
        }

        private void CompleteResultChoice(ResultPopupChoice choice)
        {
            if (GameBootstrap.Session.SelectedMode != GameMode.Pve)
            {
                if (choice == ResultPopupChoice.Arena)
                {
                    GameBootstrap.Session.OpenArenaOnHome = true;
                    SceneFlow.Go(GameScene.Home);
                    return;
                }

                GameBootstrap.Session.OpenArenaOnHome = true;
                GameBootstrap.Session.AutoFindArenaMatch = true;
                SceneFlow.Go(GameScene.Home);
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
