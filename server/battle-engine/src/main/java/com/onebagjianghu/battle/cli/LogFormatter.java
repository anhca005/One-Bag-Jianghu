package com.onebagjianghu.battle.cli;

import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.sim.BattleEvent;
import com.onebagjianghu.battle.sim.BattleResult;

import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

/** Renders a battle log as readable Vietnamese text (debug / balancing only; the client does its own UI). */
final class LogFormatter {
    private static final Map<String, String> STATUS_NAMES = Map.of(
            "BURN", "Liệt Hỏa",
            "POISON", "Kịch Độc",
            "BLEED", "Xuất Huyết",
            "REGEN", "Điều Tức",
            "STUN", "Định Thân",
            "SLOW", "Trì Trệ",
            "SEAL", "Điểm Huyệt",
            "HASTE", "Khinh Công",
            "EXHAUSTION", "Kiệt Lực");

    private final Map<Integer, String> itemNames = new HashMap<>();
    private final boolean verbose;

    LogFormatter(BattleResult result, ItemCatalog catalog, boolean verbose) {
        for (BattleResult.ItemRef ref : result.items()) {
            itemNames.put(ref.uid(), catalog.get(ref.id()).name);
        }
        this.verbose = verbose;
    }

    String format(List<BattleEvent> events) {
        StringBuilder sb = new StringBuilder();
        for (BattleEvent e : events) {
            String line = line(e);
            if (line != null) {
                sb.append(String.format(Locale.ROOT, "%6.1fs  ", e.t / 1000.0)).append(line).append('\n');
            }
        }
        return sb.toString();
    }

    private String line(BattleEvent e) {
        String who = side(e.side);
        return switch (e.type) {
            case START -> who + " vào trận" + state(e);
            case TRIGGER -> switch (e.kind) {
                case "ULTIMATE" -> who + " " + item(e) + " tung TUYỆT KỸ" + state(e);
                case "PASSIVE" -> who + " " + item(e) + " kích hoạt";
                default -> verbose ? who + " " + item(e) + " ra Phổ Công" : null;
            };
            case DAMAGE -> "   → " + who + " nhận " + e.value + " sát thương"
                    + (Boolean.TRUE.equals(e.crit) ? " HỘI TÂM" : "")
                    + (e.absorbed != null && e.absorbed > 0 ? " (Cương Khí đỡ " + e.absorbed + ")" : "")
                    + " từ " + item(e) + state(e);
            case DOT -> "   ~ " + who + " chịu " + status(e.kind) + " " + e.value
                    + (e.absorbed != null && e.absorbed > 0 ? " (Cương Khí đỡ " + e.absorbed + ")" : "") + state(e);
            case HEAL -> "   + " + who + " hồi " + e.value + " khí huyết" + state(e);
            case SHIELD -> "   " + who + (e.value >= 0 ? " +" + e.value + " Cương Khí" : " bị Phá Cương " + (-e.value)) + state(e);
            case QI -> verbose ? "   " + who + " nội lực " + signed(e.value) + state(e) : null;
            case STATUS -> {
                if (isTimed(e.kind)) {
                    yield "   " + who + (e.item != null ? " " + item(e) : "") + " dính " + status(e.kind)
                            + String.format(Locale.ROOT, " %.1fs", e.value / 1000.0);
                }
                if (e.value < 0) {
                    yield verbose ? "   " + who + " " + status(e.kind) + " về " + e.total : null;
                }
                yield "   " + who + " tích " + status(e.kind) + " +" + e.value
                        + (e.total != null ? " (" + e.total + ")" : "");
            }
            case BURST -> "   ✸ " + who + " " + status(e.kind).toUpperCase(Locale.ROOT) + " BỘC PHÁT " + e.value
                    + (e.absorbed != null && e.absorbed > 0 ? " (Cương Khí đỡ " + e.absorbed + ")" : "") + state(e);
            case PARRY -> who + " HÓA KÌNH! Hóa giải Tuyệt Kỹ của " + item(e) + state(e);
            case CHARGE -> "   " + who + " " + item(e) + String.format(Locale.ROOT, " Vận Khí %.1fs", e.value / 1000.0);
            case BUFF -> "   " + who + " " + item(e) + " +" + e.value + ("CRIT".equals(e.kind) ? "‰ Hội Tâm" : " Lực Đạo");
            case MAX_HP -> "   " + who + " bị Nội Thương, khí huyết tối đa " + signed(e.value) + state(e);
            case DMG_REDUCE -> "   " + who + " Hộ Thân " + (e.value / 10.0) + "%";
            case PARRY_COST -> "   " + who + " Hóa Kình chỉ còn tốn " + e.value + " nội lực";
            case END -> e.side < 0 ? "=== HÒA ===" : "=== " + who + " THẮNG ===";
        };
    }

    private static boolean isTimed(String kind) {
        return "STUN".equals(kind) || "SLOW".equals(kind) || "SEAL".equals(kind) || "HASTE".equals(kind);
    }

    private static String side(int side) {
        return side == 0 ? "[A]" : side == 1 ? "[B]" : "";
    }

    private String item(BattleEvent e) {
        return e.item == null ? "?" : itemNames.getOrDefault(e.item, "#" + e.item);
    }

    private static String status(String kind) {
        return STATUS_NAMES.getOrDefault(kind, kind);
    }

    private static String signed(int v) {
        return v >= 0 ? "+" + v : String.valueOf(v);
    }

    private static String state(BattleEvent e) {
        if (e.hp == null) return "";
        return "  | KH " + Math.max(0, e.hp) + "/" + e.maxHp + " CK " + e.shield + " NL " + e.qi;
    }
}
