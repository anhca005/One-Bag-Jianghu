using OneBagJianghu.Core;
using OneBagJianghu.Economy;
using OneBagJianghu.Net;

namespace OneBagJianghu.UI.Arena
{
    public sealed class ArenaMatchReceipt
    {
        public bool RewardApplied
        {
            get;
            private set;
        }

        public void ApplyRewardOnce(PlayerProfile profile, ArenaResult result)
        {
            if (RewardApplied)
            {
                return;
            }

            OnlineProtocol.ApplyReward(profile, result?.Reward);
            RewardApplied = true;
        }
    }

    public static class ArenaMatchLogic
    {
        public static void StoreBattle(GameSession session, ArenaResult result, ArenaMatchReceipt receipt)
        {
            receipt.ApplyRewardOnce(session.Profile, result);
            session.StoreArenaResult(result);
        }

        public static void StoreReplay(GameSession session, ArenaReplayResult replay)
        {
            session.StoreArenaReplay(replay);
        }
    }
}
