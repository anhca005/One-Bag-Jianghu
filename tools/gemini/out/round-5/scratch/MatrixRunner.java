package round5;

import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.*;
import java.util.concurrent.*;
import java.util.stream.Stream;

public class MatrixRunner {
    record Variant(int tier, int tp) {}

    public static void main(String[] args) throws Exception {
        Path loadoutsDir = Path.of("server/battle-engine/samples");
        Path itemsPath = null;
        Path sectsPath = Path.of("server/battle-engine/src/main/resources/data/sects.json");
        Path configPath = null;
        int runs = 100;
        int level = 10;

        for (int i = 0; i < args.length; i++) {
            switch (args[i]) {
                case "--loadouts" -> loadoutsDir = Path.of(args[++i]);
                case "--items" -> itemsPath = Path.of(args[++i]);
                case "--sects" -> sectsPath = Path.of(args[++i]);
                case "--battle-config" -> configPath = Path.of(args[++i]);
                case "--runs" -> runs = Integer.parseInt(args[++i]);
                case "--level" -> level = Integer.parseInt(args[++i]);
            }
        }

        ItemCatalog catalog = itemsPath == null ? ItemCatalog.loadDefault() : ItemCatalog.load(itemsPath);
        SectCatalog sects = sectsPath == null ? SectCatalog.loadDefault() : SectCatalog.load(sectsPath);
        BattleConfig config = configPath == null ? BattleConfig.defaults() : BattleConfig.loadOverride(configPath);
        BattleSimulator sim = new BattleSimulator(catalog, sects, config);

        List<Path> files;
        try (Stream<Path> s = Files.list(loadoutsDir)) {
            files = s.filter(p -> p.toString().endsWith(".json")).sorted().toList();
        }

        List<Loadout> originals = files.stream().map(Loadout::load).toList();

        List<Variant> variants = List.of(
            new Variant(1, 1), new Variant(1, 9),
            new Variant(3, 1), new Variant(3, 9),
            new Variant(6, 1), new Variant(6, 9),
            new Variant(9, 1), new Variant(9, 9)
        );

        final int heroLevel = level;
        final int battleRuns = runs;

        // Run all 8 variants in parallel
        ExecutorService pool = Executors.newFixedThreadPool(Math.min(variants.size(), Runtime.getRuntime().availableProcessors()));
        List<Future<double[]>> futures = new ArrayList<>();

        for (Variant v : variants) {
            futures.add(pool.submit(() -> {
                int n = originals.size();
                List<Loadout> adapted = new ArrayList<>();
                for (Loadout orig : originals) {
                    Loadout ld = new Loadout();
                    ld.name = orig.name;
                    ld.sect = orig.sect;
                    ld.level = heroLevel;
                    ld.tamPhapLevel = v.tp();
                    ld.cols = orig.cols;
                    ld.rows = orig.rows;
                    for (Loadout.PlacedItem it : orig.items) {
                        Loadout.PlacedItem ni = new Loadout.PlacedItem();
                        ni.id = it.id;
                        ni.x = it.x;
                        ni.y = it.y;
                        ni.tier = v.tier();
                        ni.rarity = it.rarity;
                        ld.items.add(ni);
                    }
                    adapted.add(ld);
                }

                double[] rowAverages = new double[n];
                for (int i = 0; i < n; i++) {
                    double sumPct = 0;
                    int count = 0;
                    for (int j = 0; j < n; j++) {
                        if (i == j) continue;
                        int winA = 0;
                        int draw = 0;
                        Loadout a = adapted.get(i);
                        Loadout b = adapted.get(j);
                        for (int seed = 1; seed <= battleRuns; seed++) {
                            BattleResult r = sim.run(a, b, seed);
                            if (r.winner() == 0) winA++;
                            else if (r.isDraw()) draw++;
                        }
                        double pct = (100.0 * winA + 50.0 * draw) / battleRuns;
                        sumPct += pct;
                        count++;
                    }
                    rowAverages[i] = count == 0 ? 0 : sumPct / count;
                }
                return rowAverages;
            }));
        }

        List<double[]> columns = new ArrayList<>();
        for (var f : futures) {
            columns.add(f.get());
        }
        pool.shutdown();

        int maxNameLen = originals.stream().mapToInt(l -> l.name.length()).max().orElse(20);
        int colWidth = Math.max(36, maxNameLen + 2);

        StringBuilder sb = new StringBuilder();
        sb.append(String.format("%-" + colWidth + "s", "Loadout"));
        for (Variant v : variants) {
            sb.append(String.format("%11s", "T" + v.tier() + "/TP" + v.tp()));
        }
        sb.append("\n").append("-".repeat(colWidth + 11 * variants.size())).append("\n");

        int outside = 0;
        int extremeLow = 0;
        int extremeHigh = 0;

        for (int i = 0; i < originals.size(); i++) {
            sb.append(String.format("%-" + colWidth + "s", originals.get(i).name));
            for (int col = 0; col < variants.size(); col++) {
                double val = columns.get(col)[i];
                boolean isOut = val < 35.0 || val > 65.0;
                if (isOut) outside++;
                if (val < 15.0) extremeLow++;
                if (val > 85.0) extremeHigh++;
                String flag = isOut ? "!" : " ";
                sb.append(String.format(Locale.ROOT, "%9.1f%%%s", val, flag));
            }
            sb.append("\n");
        }

        sb.append(String.format("Outside [35, 65]: %d/%d (extreme <15%%: %d, >85%%: %d)%n",
            outside, originals.size() * variants.size(), extremeLow, extremeHigh));

        System.out.print(sb);
    }
}
