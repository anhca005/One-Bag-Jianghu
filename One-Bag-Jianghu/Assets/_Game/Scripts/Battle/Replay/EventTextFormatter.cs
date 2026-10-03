using System;
using System.Globalization;

namespace OneBagJianghu.Battle.Replay
{
    public static class EventTextFormatter
    {
        public static string Format(BattleEvent battleEvent, Func<int, string> itemName = null)
        {
            if (battleEvent == null)
                throw new ArgumentNullException(nameof(battleEvent));
            string time =
(battleEvent.TimeMs / 1000f).ToString("0.0",
CultureInfo.InvariantCulture) +
"s  ";
            string item =
battleEvent.Item.HasValue &&
itemName != null ?
itemName(battleEvent.Item.Value) :
"Võ hiệp";
            int value = battleEvent.Value ?? 0;
            switch (battleEvent.Type)
            {
                case BattleEventType.Trigger:
                    return time + item + " thi triển" + (battleEvent.Kind == "ULTIMATE" ? " tuyệt kỹ" : "");
                case BattleEventType.Damage:
                    return time +
item +
" → " +
value +
" sát thương" +
(battleEvent.Crit == true ?
" (Hội Tâm)" :
"");
                case BattleEventType.Burst:
                    return time + StatusName(battleEvent.Kind) + " bộc phát → " + value + " sát thương";
                case BattleEventType.Heal:
                    return time + item + " hồi " + value + " sinh lực";
                case BattleEventType.Parry:
                    return time + "Hóa Kình!";
                case BattleEventType.Status:
                    return time +
StatusName(battleEvent.Kind) +
(battleEvent.Total.HasValue ?
" " +
battleEvent.Total.Value :
"");
                case BattleEventType.End:
                    return time + "Kết thúc trận đấu";
                default:
                    return time + (battleEvent.RawType ?? battleEvent.Type.ToString());
            }
        }

        public static string StatusName(string kind)
        {
            switch (kind)
            {
                case "BURN":
                    return "Liệt Hỏa";
                case "POISON":
                    return "Kịch Độc";
                case "BLEED":
                    return "Xuất Huyết";
                case "STUN":
                    return "Định Thân";
                case "SEAL":
                    return "Điểm Huyệt";
                case "SLOW":
                    return "Trì Trệ";
                case "HASTE":
                    return "Khinh Công";
                case "REGEN":
                    return "Hồi Phục";
                default:
                    return string.IsNullOrEmpty(kind) ? "Trạng thái" : kind;
            }
        }
    }
}