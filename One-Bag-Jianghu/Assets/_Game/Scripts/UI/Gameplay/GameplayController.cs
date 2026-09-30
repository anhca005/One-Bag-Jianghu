using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Battle.Replay.View;
using OneBagJianghu.Core;
using OneBagJianghu.Engine.Unity;
using OneBagJianghu.Net;
using OneBagJianghu.UI.Popups;
using OneBagJianghu.Economy;
using OneBagJianghu.Bag;
using UnityEngine;

namespace OneBagJianghu.UI.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private GameObject replayRoot;
        [SerializeField] private ReplayScreen replayScreen;
        private string opponentName = "Đối thủ";
        private long battleSeed;

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
            Debug.Log($"[Battle] Start mode={session.SelectedMode}, player={session.HeroName}, loadout={session.Profile.Loadout.Count} items");
            if (GameBootstrap.Network.State == NetworkState.Online) { GameBootstrap.Network.RequestBattle(session.SelectedMode == GameMode.Pvp ? "pvp" : "pve", session.CurrentLoadoutJson, HandleBattleResult, HandleBattleError); return; }
            try { LocalBattleOutput output = LocalBattleService.RunBattle(session.CurrentLoadoutJson, session.SelectedMode); HandleBattleResult(BattleLogParser.Parse(output.Result.ToString()), output.Opponent); }
            catch (OneBagJianghu.Engine.DataException exception) { MessageAndReturnHome("Không thể xuất chiêu", exception.Message); }
            catch (System.Exception exception) { Debug.LogException(exception); ShowFallbackAndPlaySample(); }
        }
        private void HandleBattleResult(BattleLog log, OpponentInfo opponent)
        {
            opponentName = string.IsNullOrWhiteSpace(opponent?.Name) ? "Đối thủ" : opponent.Name;
            if (log.Names.Count > 1) log.Names[1] = string.IsNullOrWhiteSpace(opponent?.Sect) ? opponentName : opponentName + " • " + opponent.Sect;
            PlayBattle(log);
        }
        private void HandleBattleError(string message) { if (GameBootstrap.Network.State == NetworkState.Online) Message("Máy chủ từ chối", message); else ShowFallbackAndPlaySample(); }
        private void ShowFallbackAndPlaySample()
        {
            TextAsset asset = Resources.Load<TextAsset>("Replays/replay_sample");
            if (asset == null) { Debug.LogError("Replay sample was not found."); ShowResult(-1); return; }
            opponentName = "Đối thủ"; PlayBattle(BattleLogParser.Parse(asset.text));
        }
        private void PlayBattle(BattleLog log)
        {
            battleSeed = log.Seed;
            int playerItems = 0, opponentItems = 0;
            foreach (BattleItemRef item in log.Items) { if (item.Side == 0) playerItems++; else if (item.Side == 1) opponentItems++; }
            Debug.Log($"[Battle] Start resolved mode={GameBootstrap.Session.SelectedMode}, seed={log.Seed}, player={log.Names[0]}({playerItems} items), opponent={log.Names[1]}({opponentItems} items)");
            replayRoot.SetActive(true); replayScreen.Initialize(log, OneBagJianghu.Bag.ItemCatalog.LoadFromResources());
            string mode = GameBootstrap.Session.SelectedMode == GameMode.Pvp ? "Luận Kiếm Đài" : "Hành Tẩu Giang Hồ";
            replayScreen.SetStageLabel(mode + "  •  " + opponentName);
            replayScreen.Completed -= ShowResult; replayScreen.Completed += ShowResult; replayScreen.Play();
            replayScreen.ExitRequested -= ConfirmExit; replayScreen.ExitRequested += ConfirmExit;
        }
        private void ConfirmExit() => PopupManager.Show<ConfirmPopup>(new ConfirmPopupData("Thoát trận", "Rời trận và trở về Sảnh?", "Thoát", "Ở lại", yes => { if (yes) SceneFlow.Go(GameScene.Home); }));
        private void ShowResult(int winner)
        {
            string result = winner < 0 ? "Hòa" : winner == 0 ? "Thắng" : "Bại"; string winnerName = winner == 0 ? GameBootstrap.Session.HeroName : winner > 0 ? opponentName : string.Empty;
            string rewardText = string.Empty;
            if (GameBootstrap.Session.SelectedMode == GameMode.Pve && winner >= 0)
            {
                RewardService service = new RewardService(EconomyRuntime.Load("rewards", RewardCatalog.Parse), GameSession.LoadMaterialCatalog(), ItemCatalog.LoadFromResources());
                bool firstClear = winner == 0 && !GameBootstrap.Session.Profile.FirstClears.Contains("offline_pve");
                BattleReward reward = service.Grant(GameBootstrap.Session.Profile, winner == 0, firstClear, unchecked((int)battleSeed));
                if (!reward.Success)
                {
                    Debug.LogWarning("[Reward] Grant failed: " + reward.Error);
                    rewardText = "Thưởng chưa nhận: " + reward.Error;
                }
                else
                {
                if (firstClear) GameBootstrap.Session.Profile.FirstClears.Add("offline_pve");
                rewardText = $"Thưởng: {reward.Silver:N0} Bạc" + (reward.HeroExp > 0 ? $" • {reward.HeroExp} EXP" : string.Empty);
                foreach (System.Collections.Generic.KeyValuePair<string, int> material in reward.Materials) rewardText += $" • {material.Key} x{material.Value}";
                Debug.Log($"[Reward] Granted Bạc={reward.Silver}, materials={FormatMaterials(reward.Materials)}, EXP={reward.HeroExp}");
                GameBootstrap.Session.SaveProfile();
                }
            }
            ResultPopup popup = PopupManager.Show<ResultPopup>(new ResultPopupData(result, winnerName, rewardText)); if (popup != null) { popup.Closed -= HandleResultClosed; popup.Closed += HandleResultClosed; }
        }
        private static string FormatMaterials(System.Collections.Generic.IReadOnlyDictionary<string, int> materials)
        {
            var values = new System.Collections.Generic.List<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, int> value in materials) values.Add(value.Key + "x" + value.Value);
            return string.Join(",", values);
        }
        private static void HandleResultClosed(BasePopup popup) { popup.Closed -= HandleResultClosed; if (popup.Result is ResultPopupChoice choice) SceneFlow.Go(choice == ResultPopupChoice.Home ? GameScene.Home : GameScene.Gameplay); }
        private static void Message(string title, string body) => PopupManager.Show<MessagePopup>(new MessagePopupData(title, body, "Đóng"));
        private static void MessageAndReturnHome(string title, string body)
        {
            MessagePopup popup = PopupManager.Show<MessagePopup>(new MessagePopupData(title, body, "Về Sảnh"));
            if (popup == null) { SceneFlow.Go(GameScene.Home); return; }
            popup.Closed += ReturnHome;
        }
        private static void ReturnHome(BasePopup popup) { popup.Closed -= ReturnHome; SceneFlow.Go(GameScene.Home); }
    }
}
