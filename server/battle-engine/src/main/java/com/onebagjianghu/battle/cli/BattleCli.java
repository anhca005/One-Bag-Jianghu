package com.onebagjianghu.battle.cli;

import com.fasterxml.jackson.databind.SerializationFeature;
import com.onebagjianghu.battle.data.DataException;
import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;

import java.io.FileDescriptor;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintStream;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.function.Consumer;
import java.util.stream.Stream;

/**
 * Dev tool for running battles from the command line.
 *
 * <pre>
 *   battle A.json B.json [--seed N] [--verbose] [--json out.json]   one battle, print the log
 *   battle A.json B.json --runs N                                   N seeds, print win rates
 *   battle --matrix DIR [--runs N]                                  every loadout in DIR vs every other
 *   common: [--items items.json]   (default: bundled data/items.json)
 * </pre>
 */
public final class BattleCli {
    private static final PrintStream OUT = new PrintStream(new FileOutputStream(FileDescriptor.out), true, StandardCharsets.UTF_8);

    public static void main(String[] args) {
        try {
            run(args);
        } catch (DataException | IllegalArgumentException e) {
            new PrintStream(new FileOutputStream(FileDescriptor.err), true, StandardCharsets.UTF_8).println("Lỗi: " + e.getMessage());
            System.exit(1);
        }
    }

    private static void run(String[] args) {
        List<String> files = new ArrayList<>();
        long seed = 1;
        int runs = 1;
        boolean verbose = false;
        String json = null;
        String items = null;
        String matrix = null;
        for (int i = 0; i < args.length; i++) {
            switch (args[i]) {
                case "--seed" -> seed = Long.parseLong(value(args, ++i, "--seed"));
                case "--runs" -> runs = Integer.parseInt(value(args, ++i, "--runs"));
                case "--json" -> json = value(args, ++i, "--json");
                case "--items" -> items = value(args, ++i, "--items");
                case "--matrix" -> matrix = value(args, ++i, "--matrix");
                case "--verbose", "-v" -> verbose = true;
                default -> files.add(args[i]);
            }
        }
        ItemCatalog catalog = items == null ? ItemCatalog.loadDefault() : ItemCatalog.load(Path.of(items));
        BattleSimulator sim = new BattleSimulator(catalog);

        if (matrix != null) {
            matrix(sim, Path.of(matrix), runs > 1 ? runs : 200);
            return;
        }
        if (files.size() != 2) {
            throw new IllegalArgumentException("cần 2 file đội hình, hoặc --matrix DIR. Xem javadoc của BattleCli.");
        }
        Loadout a = Loadout.load(Path.of(files.get(0)));
        Loadout b = Loadout.load(Path.of(files.get(1)));
        if (runs > 1) {
            Breakdown ba = new Breakdown(catalog);
            Breakdown bb = new Breakdown(catalog);
            Stats s = stats(sim, a, b, runs, r -> {
                ba.add(r, 0);
                bb.add(r, 1);
            });
            OUT.printf(Locale.ROOT, "[A] %s  vs  [B] %s  (%d trận)%n", a.name, b.name, runs);
            OUT.printf(Locale.ROOT, "A thắng %.1f%%  |  B thắng %.1f%%  |  hòa %.1f%%  |  thời lượng TB %.1fs%n%n",
                    s.pct(s.winA), s.pct(s.winB), s.pct(s.draw), s.avgMs() / 1000.0);
            ba.print(OUT, "[A] " + a.name);
            bb.print(OUT, "[B] " + b.name);
            return;
        }
        BattleResult r = sim.run(a, b, seed);
        OUT.printf("[A] %s  vs  [B] %s  (seed %d)%n%n", a.name, b.name, seed);
        OUT.print(new LogFormatter(r, catalog, verbose).format(r.events()));
        OUT.printf(Locale.ROOT, "%nKết quả: %s sau %.1fs. KH còn lại: A %d, B %d. Số sự kiện: %d%n",
                r.isDraw() ? "HÒA" : (r.winner() == 0 ? "A thắng" : "B thắng"),
                r.durationMs() / 1000.0, r.finalHp()[0], r.finalHp()[1], r.events().size());
        if (json != null) {
            writeJson(r, Path.of(json));
            OUT.println("Đã ghi nhật ký JSON: " + json);
        }
    }

    private static void matrix(BattleSimulator sim, Path dir, int runs) {
        List<Path> files;
        try (Stream<Path> s = Files.list(dir)) {
            files = s.filter(p -> p.toString().endsWith(".json")).sorted().toList();
        } catch (IOException e) {
            throw new DataException("Không đọc được thư mục " + dir, e);
        }
        List<Loadout> loadouts = files.stream().map(Loadout::load).toList();
        OUT.printf("Tỉ lệ thắng của hàng (A) trước cột (B), %d trận mỗi cặp%n%n", runs);
        StringBuilder header = new StringBuilder(String.format("%-36s", ""));
        for (int j = 0; j < loadouts.size(); j++) header.append(String.format("%8s", "#" + (j + 1)));
        header.append(String.format("%9s", "TB"));
        OUT.println(header);
        for (int i = 0; i < loadouts.size(); i++) {
            StringBuilder row = new StringBuilder(String.format("%-36s", "#" + (i + 1) + " " + loadouts.get(i).name));
            double sum = 0;
            int count = 0;
            for (int j = 0; j < loadouts.size(); j++) {
                if (i == j) {
                    row.append(String.format("%8s", "-"));
                    continue;
                }
                Stats s = stats(sim, loadouts.get(i), loadouts.get(j), runs);
                double pct = s.pct(s.winA) + s.pct(s.draw) / 2;
                sum += pct;
                count++;
                row.append(String.format(Locale.ROOT, "%7.1f%%", pct));
            }
            row.append(String.format(Locale.ROOT, "%8.1f%%", count == 0 ? 0 : sum / count));
            OUT.println(row);
        }
        OUT.println("\n(hòa tính nửa trận thắng)");
    }

    private record Stats(int runs, int winA, int winB, int draw, long totalMs) {
        double pct(int n) {
            return 100.0 * n / runs;
        }

        double avgMs() {
            return (double) totalMs / runs;
        }
    }

    private static Stats stats(BattleSimulator sim, Loadout a, Loadout b, int runs) {
        return stats(sim, a, b, runs, r -> { });
    }

    private static Stats stats(BattleSimulator sim, Loadout a, Loadout b, int runs, Consumer<BattleResult> each) {
        int winA = 0;
        int winB = 0;
        int draw = 0;
        long total = 0;
        for (int seed = 1; seed <= runs; seed++) {
            BattleResult r = sim.run(a, b, seed);
            each.accept(r);
            if (r.winner() == 0) winA++;
            else if (r.winner() == 1) winB++;
            else draw++;
            total += r.durationMs();
        }
        return new Stats(runs, winA, winB, draw, total);
    }

    private static void writeJson(BattleResult r, Path path) {
        try {
            ItemCatalog.mapper().writer(SerializationFeature.INDENT_OUTPUT).writeValue(path.toFile(), r);
        } catch (IOException e) {
            throw new DataException("Không ghi được " + path, e);
        }
    }

    private static String value(String[] args, int i, String flag) {
        if (i >= args.length) {
            throw new IllegalArgumentException(flag + " cần giá trị");
        }
        return args[i];
    }
}
