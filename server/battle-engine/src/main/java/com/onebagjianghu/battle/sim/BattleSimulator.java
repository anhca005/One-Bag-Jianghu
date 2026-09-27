package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.ItemCatalog;

/**
 * Entry point of the battle engine. Deterministic: the same loadouts, config and seed
 * always produce the same {@link BattleResult}. Thread-safe (each run has its own state).
 */
public final class BattleSimulator {
    private final ItemCatalog catalog;
    private final BattleConfig config;

    public BattleSimulator(ItemCatalog catalog, BattleConfig config) {
        this.catalog = catalog;
        this.config = config;
    }

    public BattleSimulator(ItemCatalog catalog) {
        this(catalog, BattleConfig.defaults());
    }

    public BattleResult run(Loadout a, Loadout b, long seed) {
        return new Battle(config, catalog, a, b, seed).run();
    }
}
