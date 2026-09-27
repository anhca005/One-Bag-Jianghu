package com.onebagjianghu.battle.cli;

import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.sim.BattleEvent;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.EventType;

import java.io.PrintStream;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Locale;
import java.util.Map;

/** Where one side's damage, healing and shield come from, averaged over many battles. For balancing. */
final class Breakdown {
    private static final Map<String, String> BURST_NAMES = Map.of(
            "BURN", "(Liệt Hỏa bộc phát)", "POISON", "(Kịch Độc bộc phát)", "BLEED", "(Xuất Huyết bộc phát)");

    private final ItemCatalog catalog;
    private final Map<String, Long> damage = new LinkedHashMap<>();
    private long heal;
    private long shield;
    private long exhaustionTaken;
    private int battles;

    Breakdown(ItemCatalog catalog) {
        this.catalog = catalog;
    }

    /** Accumulates what {@code side} dealt / gained in this battle. */
    void add(BattleResult r, int side) {
        battles++;
        Map<Integer, String> names = new HashMap<>();
        for (BattleResult.ItemRef ref : r.items()) {
            names.put(ref.uid(), catalog.get(ref.id()).name);
        }
        for (BattleEvent e : r.events()) {
            if (e.type == EventType.DAMAGE && e.side != side && e.item != null) {
                damage.merge(names.get(e.item), (long) e.value, Long::sum);
            } else if (e.type == EventType.DOT && "EXHAUSTION".equals(e.kind) && e.side == side) {
                exhaustionTaken += e.value;
            } else if (e.type == EventType.BURST && e.side != side) {
                damage.merge(BURST_NAMES.getOrDefault(e.kind, e.kind), (long) e.value, Long::sum);
            } else if (e.type == EventType.HEAL && e.side == side) {
                heal += e.value;
            } else if (e.type == EventType.SHIELD && e.side == side && e.value > 0) {
                shield += e.value;
            }
        }
    }

    void print(PrintStream out, String title) {
        long total = damage.values().stream().mapToLong(Long::longValue).sum();
        out.printf(Locale.ROOT, "%s — sát thương gây ra TB %.0f/trận%n", title, (double) total / battles);
        damage.entrySet().stream()
                .sorted(Map.Entry.<String, Long>comparingByValue().reversed())
                .forEach(en -> out.printf(Locale.ROOT, "    %-24s %7.0f  %5.1f%%%n",
                        en.getKey(), (double) en.getValue() / battles, 100.0 * en.getValue() / Math.max(1, total)));
        out.printf(Locale.ROOT, "    hồi máu TB %.0f · Cương Khí TB %.0f · chịu Kiệt Lực TB %.0f%n",
                (double) heal / battles, (double) shield / battles, (double) exhaustionTaken / battles);
    }
}
