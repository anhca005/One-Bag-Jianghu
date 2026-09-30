package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;

/**
 * Entry point of the battle engine. Deterministic: the same loadouts, data, config and seed
 * always produce the same {@link BattleResult}. Thread-safe (each run has its own state).
 */
public final class BattleSimulator {
    private final ItemCatalog catalog;
    private final SectCatalog sects;
    private final BattleConfig config;

    public BattleSimulator(ItemCatalog catalog, SectCatalog sects, BattleConfig config) {
        this.catalog = catalog;
        this.sects = sects;
        this.config = config;
    }

    /** Uses the bundled sects.json and default config. */
    public BattleSimulator(ItemCatalog catalog) {
        this(catalog, SectCatalog.loadDefault(), BattleConfig.defaults());
    }

    public BattleResult run(Loadout a, Loadout b, long seed) {
        return new Battle(config, catalog, sects, a, b, seed).run();
    }
}
