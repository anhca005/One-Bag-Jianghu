package com.onebagjianghu.battle.data;

import java.util.ArrayList;
import java.util.List;

/** Môn phái: its only combat effect is the Tâm Pháp, a passive that buffs some weapon classes. */
public final class SectDef {
    public String id;
    public String name;
    /** Tên Tâm Pháp. */
    public String tamPhap;
    public List<String> weaponClasses = List.of();
    /** Milestones sorted by level (checked by {@link SectCatalog}). */
    public List<TamPhapLevelDef> levels = List.of();

    public boolean buffs(ItemDef def) {
        return def.weaponClass != null && weaponClasses.contains(def.weaponClass);
    }

    /** {@code def} with this Tâm Pháp at {@code level} applied, or {@code def} itself if it is not buffed. */
    public ItemDef applyTo(ItemDef def, int level) {
        if (!buffs(def)) {
            return def;
        }
        ItemDef r = def.copy();
        double attackPct = 0;
        double cooldownPct = 0;
        int ultCost = r.ultimate == null ? 0 : r.ultimate.cost;
        List<EffectDef> basic = new ArrayList<>(r.basic);
        List<EffectDef> ult = r.ultimate == null ? null : new ArrayList<>(r.ultimate.effects);
        List<TriggerDef> triggers = new ArrayList<>(r.triggers);
        for (TamPhapLevelDef l : levels) {
            if (l.level > level) {
                continue;
            }
            attackPct += l.attackPct;
            cooldownPct += l.cooldownPct;
            r.crit += l.crit;
            ultCost += l.ultimateCost;
            basic.addAll(l.basic);
            if (ult != null) ult.addAll(l.ultimate);
            triggers.addAll(l.triggers);
        }
        r.attack = (int) Math.round(r.attack * (1 + attackPct));
        r.cooldown = r.cooldown * (1 - cooldownPct);
        r.basic = List.copyOf(basic);
        r.triggers = List.copyOf(triggers);
        if (ult != null) {
            r.ultimate = new UltimateDef();
            r.ultimate.cost = Math.max(1, ultCost);
            r.ultimate.effects = List.copyOf(ult);
        }
        return r;
    }
}
