package com.onebagjianghu.battle.sim;

import com.onebagjianghu.battle.data.StatusType;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.data.DataException;

import java.io.IOException;
import java.nio.file.Path;

/** Global combat constants. Values are the proposals in docs/design/combat.md, not final. */
public final class BattleConfig {
    public int tickMs = 100;
    public int maxQi = 100;
    /** Hóa Kình: nội lực tiêu hao để hóa giải Tuyệt Kỹ. */
    public int parryCost = 60;
    /** Chi phí Hóa Kình thấp nhất sau khi giảm. */
    public int minParryCost = 20;
    /** Định Thân áp lên bên bị Hóa Kình. */
    public int parryStunMs = 1000;
    /** Kiệt Lực bắt đầu sau mốc này. */
    public int exhaustionStartMs = 45_000;
    /** Sát thương Kiệt Lực giây thứ n = n × step. */
    public int exhaustionStep = 20;
    /** Hết giờ thì hòa. */
    public int maxDurationMs = 120_000;
    public int maxTriggerDepth = 8;
    /** Điều Tức tối đa (khí huyết hồi mỗi giây); phần cộng vượt mức bị bỏ. */
    public int maxRegen = 30;
    /** Cương Khí cap as permille of the fighter's current max HP. */
    public int shieldCapPermille = 1000;
    /** Hộ Thân tối đa. */
    public int maxDmgReducePermille = 800;
    /** Hệ số theo Trọng 1..9 (lợi ích giảm dần), áp lên Lực Đạo, value, stacks. */
    public int[] tierMultiplierPermille = {1000, 1200, 1400, 1550, 1700, 1800, 1900, 1950, 2000};
    public int[] rarityMultiplierPermille = {1000, 1060, 1120, 1190, 1260, 1340};

    /** Liệt Hỏa: Cương Khí đỡ được. */
    public Burst burn = new Burst(50, 300, 0, false, 0);
    /** Kịch Độc: xuyên Cương Khí. */
    public Burst poison = new Burst(50, 240, 0, true, 0);
    /** Xuất Huyết: xuyên Cương Khí, cộng thêm 10% khí huyết tối đa. */
    public Burst bleed = new Burst(50, 150, 100, true, 0);

    /**
     * Build-up status: points accumulate with no effect until {@code threshold},
     * then burst for damage + maxHp‰ of the target's max HP and reset to 0.
     */
    public static final class Burst {
        public int threshold;
        public int damage;
        public int maxHpPermille;
        public boolean ignoreShield;
        /** Points lost per second (0 = points never fade). */
        public int decayPerSecond;

        public Burst() {
        }

        public Burst(int threshold, int damage, int maxHpPermille, boolean ignoreShield, int decayPerSecond) {
            this.threshold = threshold;
            this.damage = damage;
            this.maxHpPermille = maxHpPermille;
            this.ignoreShield = ignoreShield;
            this.decayPerSecond = decayPerSecond;
        }
    }

    public static BattleConfig defaults() {
        return new BattleConfig();
    }

    /** Reads a partial JSON object over the defaults; unknown fields are rejected. */
    public static BattleConfig loadOverride(Path path) {
        BattleConfig config = defaults();
        try {
            return com.onebagjianghu.battle.data.ItemCatalog.mapper().readerForUpdating(config).readValue(path.toFile());
        } catch (IOException e) {
            throw new DataException("Cannot read battle config " + path + ": " + e.getMessage(), e);
        }
    }

    public int tierMultiplier(int tier) {
        int i = Math.max(1, Math.min(tier, tierMultiplierPermille.length)) - 1;
        return tierMultiplierPermille[i];
    }

    public int rarityMultiplier(Rarity rarity) {
        return rarityMultiplierPermille[rarity.ordinal()];
    }

    /** Null for statuses that do not build up. */
    public Burst burst(StatusType status) {
        return switch (status) {
            case BURN -> burn;
            case POISON -> poison;
            case BLEED -> bleed;
            default -> null;
        };
    }
}
