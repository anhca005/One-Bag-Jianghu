using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneBagJianghu.Battle.Replay;
using OneBagJianghu.Economy;

namespace OneBagJianghu.Net
{
    public sealed class ServerError
    {
        [JsonProperty("code")]
        public string Code
        {
            get;
            set;
        }

        [JsonProperty("message")]
        public string Message
        {
            get;
            set;
        }
    }

    public sealed class AuthSession
    {
        [JsonProperty("accountId")]
        public long AccountId
        {
            get;
            set;
        }

        [JsonProperty("username")]
        public string Username
        {
            get;
            set;
        }

        [JsonProperty("heroName")]
        public string HeroName
        {
            get;
            set;
        }

        [JsonProperty("token")]
        public string Token
        {
            get;
            set;
        }

        [JsonProperty("expiresAt")]
        public DateTimeOffset ExpiresAt
        {
            get;
            set;
        }
    }

    public sealed class ArenaStatus
    {
        [JsonProperty("seasonId")]
        public int SeasonId
        {
            get;
            set;
        }

        [JsonProperty("seasonEndsAt")]
        public DateTimeOffset SeasonEndsAt
        {
            get;
            set;
        }

        [JsonProperty("rating")]
        public int Rating
        {
            get;
            set;
        }

        [JsonProperty("rank")]
        public int Rank
        {
            get;
            set;
        }

        [JsonProperty("wins")]
        public int Wins
        {
            get;
            set;
        }

        [JsonProperty("losses")]
        public int Losses
        {
            get;
            set;
        }

        [JsonProperty("rewardedLeftToday")]
        public int RewardedLeftToday
        {
            get;
            set;
        }

        [JsonProperty("weeklyWins")]
        public int WeeklyWins
        {
            get;
            set;
        }

        [JsonProperty("weeklyWinsTarget")]
        public int WeeklyWinsTarget
        {
            get;
            set;
        }

        [JsonProperty("weeklyJadeClaimed")]
        public bool WeeklyJadeClaimed
        {
            get;
            set;
        }
    }

    public sealed class ArenaOpponent : OpponentInfo
    {
        [JsonProperty("rating")]
        public int Rating
        {
            get;
            set;
        }

        [JsonProperty("power")]
        public int Power
        {
            get;
            set;
        }

        [JsonProperty("bot")]
        public bool Bot
        {
            get;
            set;
        }
    }

    public sealed class ArenaReward
    {
        [JsonProperty("reputation")]
        public int Reputation
        {
            get;
            set;
        }

        [JsonProperty("materials")]
        public Dictionary<string, int> Materials
        {
            get;
            set;
        } = new Dictionary<string, int>();
    }

    public sealed class ArenaResult
    {
        [JsonProperty("opponent")]
        public ArenaOpponent Opponent
        {
            get;
            set;
        }

        [JsonProperty("result")]
        public JToken Result
        {
            get;
            set;
        }

        [JsonProperty("ratingBefore")]
        public int RatingBefore
        {
            get;
            set;
        }

        [JsonProperty("ratingAfter")]
        public int RatingAfter
        {
            get;
            set;
        }

        [JsonProperty("reward")]
        public ArenaReward Reward
        {
            get;
            set;
        } = new ArenaReward();

        [JsonProperty("status")]
        public ArenaStatus Status
        {
            get;
            set;
        }

        [JsonIgnore]
        public BattleLog BattleLog => BattleLogParser.Parse(Result.ToString(Formatting.None));
    }

    public sealed class LeaderboardEntry
    {
        [JsonProperty("rank")]
        public int Rank
        {
            get;
            set;
        }

        [JsonProperty("heroName")]
        public string HeroName
        {
            get;
            set;
        }

        [JsonProperty("rating")]
        public int Rating
        {
            get;
            set;
        }

        [JsonProperty("wins")]
        public int Wins
        {
            get;
            set;
        }

        [JsonProperty("losses")]
        public int Losses
        {
            get;
            set;
        }
    }

    public sealed class LeaderboardSelf
    {
        [JsonProperty("rank")]
        public int Rank
        {
            get;
            set;
        }

        [JsonProperty("rating")]
        public int Rating
        {
            get;
            set;
        }
    }

    public sealed class LeaderboardResult
    {
        [JsonProperty("seasonId")]
        public int SeasonId
        {
            get;
            set;
        }

        [JsonProperty("entries")]
        public List<LeaderboardEntry> Entries
        {
            get;
            set;
        } = new List<LeaderboardEntry>();

        [JsonProperty("self")]
        public LeaderboardSelf Self
        {
            get;
            set;
        }
    }

    public sealed class ArenaHistoryEntry
    {
        [JsonProperty("battleId")]
        public long BattleId
        {
            get;
            set;
        }

        [JsonProperty("at")]
        public DateTimeOffset At
        {
            get;
            set;
        }

        [JsonProperty("won")]
        public bool Won
        {
            get;
            set;
        }

        [JsonProperty("opponentName")]
        public string OpponentName
        {
            get;
            set;
        }

        [JsonProperty("opponentSect")]
        public string OpponentSect
        {
            get;
            set;
        }

        [JsonProperty("bot")]
        public bool Bot
        {
            get;
            set;
        }

        [JsonProperty("ratingBefore")]
        public int RatingBefore
        {
            get;
            set;
        }

        [JsonProperty("ratingAfter")]
        public int RatingAfter
        {
            get;
            set;
        }
    }

    public sealed class ArenaHistoryResult
    {
        [JsonProperty("entries")]
        public List<ArenaHistoryEntry> Entries
        {
            get;
            set;
        } = new List<ArenaHistoryEntry>();
    }

    public sealed class ArenaReplayResult
    {
        [JsonProperty("opponent")]
        public ArenaOpponent Opponent
        {
            get;
            set;
        }

        [JsonProperty("result")]
        public JToken Result
        {
            get;
            set;
        }

        [JsonIgnore]
        public BattleLog BattleLog => BattleLogParser.Parse(Result.ToString(Formatting.None));
    }

    public static class OnlineProtocol
    {
        public static T ReadReply<T>(Envelope envelope, string expectedType)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (envelope.Type == "error")
            {
                ServerError error = envelope.Payload.ToObject<ServerError>() ?? new ServerError();
                throw new ServerReplyException(error.Code, error.Message);
            }

            if (envelope.Type != expectedType)
            {
                throw new FormatException("Unexpected server reply: " + envelope.Type);
            }

            return envelope.Payload.ToObject<T>();
        }

        public static void ApplyReward(PlayerProfile profile, ArenaReward reward)
        {
            if (profile == null || reward == null)
            {
                return;
            }

            if (reward.Reputation > 0)
            {
                profile.AddCurrency("danh_vong", reward.Reputation);
            }
            foreach (KeyValuePair<string, int> material in reward.Materials)
            {
                if (material.Value > 0)
                {
                    profile.AddMaterial(material.Key, material.Value);
                }
            }
        }

        public static string LocalizeError(string code, string fallback = null)
        {
            switch (code)
            {
                case "invalid_name":
                    return "Tên đăng nhập 3–20 ký tự, chỉ gồm chữ thường không dấu, số và _";
                case "weak_password":
                    return "Mật khẩu cần 6–64 ký tự";
                case "name_taken":
                    return "Tên đăng nhập đã có người dùng";
                case "auth_failed":
                    return "Sai tên đăng nhập hoặc mật khẩu";
                case "token_expired":
                    return "Phiên đăng nhập đã hết hạn, mời đăng nhập lại";
                case "rate_limited":
                    return "Thử sai nhiều lần, đợi một phút rồi thử lại";
                case "unauthorized":
                    return "Bạn cần đăng nhập để vào Luận Kiếm Đài";
                case "invalid_loadout":
                    return string.IsNullOrWhiteSpace(fallback) ? "Bao Phục không hợp lệ" : fallback;
                default:
                    return string.IsNullOrWhiteSpace(fallback) ? "Máy chủ từ chối yêu cầu" : fallback;
            }
        }
    }

    public sealed class ServerReplyException : Exception
    {
        public ServerReplyException(string code, string message)
            : base(OnlineProtocol.LocalizeError(code, message))
        {
            Code = code ?? string.Empty;
        }

        public string Code { get; }
    }
}


