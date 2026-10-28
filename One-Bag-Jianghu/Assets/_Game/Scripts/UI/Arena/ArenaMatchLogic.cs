using OneBagJianghu.Core;
using OneBagJianghu.Net;

namespace OneBagJianghu.UI.Arena
{
    public static class ArenaMatchLogic
    {
        public static void StoreBattle(GameSession session, ArenaResult result)
        {
            session.StoreArenaResult(result);
        }

        public static void StoreReplay(GameSession session, ArenaReplayResult replay)
        {
            session.StoreArenaReplay(replay);
        }
    }
}
