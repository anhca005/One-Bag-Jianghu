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
                "Hiệp Khách";
            int value = battleEvent.Value ?? 0;
            switch (battleEvent.Type)
            {
                case BattleEventType.Start:
                    return time + "Trận đấu bắt đầu";
                case BattleEventType.Trigger:
                    return time + item + " thi triển" + (battleEvent.Kind == "ULTIMATE" ? " Tuyệt Kỹ" : "");
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
                case BattleEventType.Dot:
                    return time + StatusName(battleEvent.Kind) + " gây " + value + " sát thương";
                case BattleEventType.Heal:
                    return time + item + " hồi " + value + " Khí Huyết";
                case BattleEventType.Shield:
                    return time + item + " nhận " + value + " Cương Khí";
                case BattleEventType.Parry:
                    return time + "Hóa Kình!";
                case BattleEventType.Qi:
                    return time + "Nội Lực +" + value;
                case BattleEventType.Status:
                    return time +
                        StatusName(battleEvent.Kind) +
                        (battleEvent.Total.HasValue ?
                        " " +
                        battleEvent.Total.Value :
                        "");
                case BattleEventType.Charge:
                    return time + item + " Vận Khí";
                case BattleEventType.Buff:
                    return time + item + " nhận hiệu ứng tăng cường";
                case BattleEventType.MaxHp:
                    return time + "Khí Huyết tối đa thay đổi " + Signed(value);
                case BattleEventType.DmgReduce:
                    return time + "Hộ Thân thay đổi " + Signed(value);
                case BattleEventType.ParryCost:
                    return time + "Chi phí Hóa Kình thay đổi " + Signed(value);
                case BattleEventType.End:
                    return time + "Kết thúc trận đấu";
                default:
                    return time + "Sự kiện chiến đấu";
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
                    return "Điều Tức";
                default:
                    return "Trạng thái";
            }
        }

        private static string Signed(int value)
        {
            return (value >= 0 ? "+" : string.Empty) + value;
        }
    }
}
