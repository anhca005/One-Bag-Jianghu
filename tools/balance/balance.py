#!/usr/bin/env python3
"""Reusable balance matrix, what-if, and coordinate-descent tooling."""

from __future__ import annotations

import argparse
import copy
import json
import os
import re
import subprocess
import sys
import tempfile
import threading
import time
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path
from typing import Any


ROOT = Path(__file__).resolve().parents[2]
ENGINE = ROOT / "server" / "battle-engine"
SAMPLES = ENGINE / "samples"
DEFAULT_ITEMS = ENGINE / "src" / "main" / "resources" / "data" / "items.json"
DEFAULT_SECTS = ENGINE / "src" / "main" / "resources" / "data" / "sects.json"
MAIN_CLASS = "com.onebagjianghu.battle.cli.BattleCli"
ROW_RE = re.compile(r"^#\d+\s+.*?([0-9]+(?:\.[0-9]+)?)%\s*$")
PRINT_LOCK = threading.Lock()


def read_json(path: Path) -> Any:
    try:
        with path.open(encoding="utf-8") as stream:
            return json.load(stream)
    except (OSError, json.JSONDecodeError) as exc:
        raise ValueError(f"Cannot read JSON {path}: {exc}") from exc


def write_json(path: Path, value: Any, *, indent: int | None = 2) -> None:
    with path.open("w", encoding="utf-8", newline="\n") as stream:
        json.dump(value, stream, ensure_ascii=False, indent=indent)
        stream.write("\n")


def run_checked(command: list[str], cwd: Path) -> subprocess.CompletedProcess[str]:
    try:
        return subprocess.run(
            command, cwd=cwd, check=True, capture_output=True, text=True,
            encoding="utf-8", errors="replace",
        )
    except FileNotFoundError as exc:
        raise RuntimeError(f"Command not found: {command[0]}") from exc
    except subprocess.CalledProcessError as exc:
        details = (exc.stderr or exc.stdout or "no output").strip()
        raise RuntimeError(f"Command failed ({' '.join(command)}):\n{details}") from exc


def ensure_classpath() -> str:
    wrapper = ENGINE.parent / ("mvnw.cmd" if os.name == "nt" else "mvnw")
    classes = ENGINE / "target" / "classes"
    cp_file = ENGINE / "target" / "cp.txt"
    if not classes.is_dir():
        print("Compiling battle engine...", file=sys.stderr, flush=True)
        run_checked([str(wrapper), "-q", "-f", "pom.xml", "compile"], ENGINE)
    if not cp_file.is_file():
        print("Building dependency classpath...", file=sys.stderr, flush=True)
        run_checked([
            str(wrapper), "-q", "-f", "pom.xml", "dependency:build-classpath",
            "-Dmdep.outputFile=target/cp.txt",
        ], ENGINE)
    dependencies = cp_file.read_text(encoding="utf-8").strip()
    return os.pathsep.join(part for part in (str(classes), dependencies) if part)


def parse_ints(raw: str, name: str) -> list[int]:
    try:
        values = [int(value.strip()) for value in raw.split(",") if value.strip()]
    except ValueError as exc:
        raise ValueError(f"{name} must be a comma-separated list of integers") from exc
    if not values:
        raise ValueError(f"{name} cannot be empty")
    if any(value < 1 or value > 9 for value in values):
        raise ValueError(f"{name} values must be in 1..9")
    return list(dict.fromkeys(values))


def load_source_loadouts(folder: Path) -> list[tuple[str, dict[str, Any]]]:
    if not folder.is_dir():
        raise ValueError(f"Loadout folder does not exist: {folder}")
    result = []
    for path in sorted(folder.glob("*.json")):
        data = read_json(path)
        if not isinstance(data, dict) or not isinstance(data.get("items"), list):
            raise ValueError(f"Invalid loadout: {path}")
        result.append((path.name, data))
    if not result:
        raise ValueError(f"No JSON loadouts found in {folder}")
    return result


def make_variant(source: list[tuple[str, dict[str, Any]]], folder: Path, tier: int, tp: int,
                 rarity: str | None = None, level: int | None = None) -> None:
    folder.mkdir()
    for filename, original in source:
        loadout = copy.deepcopy(original)
        loadout["tamPhapLevel"] = tp
        if level is not None:
            loadout["level"] = level
        for placed in loadout["items"]:
            placed["tier"] = tier
            if rarity is not None:
                placed["rarity"] = rarity
        write_json(folder / filename, loadout, indent=None)


def cli_matrix(classpath: str, folder: Path, runs: int, items: Path, sects: Path,
               battle_config: Path | None = None) -> list[float]:
    command = [
        "java", "-cp", classpath, MAIN_CLASS, "--matrix", str(folder),
        "--runs", str(runs), "--items", str(items), "--sects", str(sects),
    ]
    if battle_config is not None:
        command.extend(["--battle-config", str(battle_config)])
    completed = run_checked(command, ENGINE)
    values = []
    for line in completed.stdout.splitlines():
        match = ROW_RE.match(line)
        if match:
            values.append(float(match.group(1)))
    expected = len(list(folder.glob("*.json")))
    if len(values) != expected:
        raise RuntimeError(
            f"Could not parse matrix output ({len(values)}/{expected} rows):\n"
            + completed.stdout[-2000:]
        )
    return values


def evaluate_matrix(
    classpath: str, loadouts: Path, tiers: list[int], tp_levels: list[int],
    runs: int, items: Path, sects: Path, parallel: bool, rarity: str | None = None,
    battle_config: Path | None = None, level: int | None = None,
) -> tuple[list[str], list[tuple[int, int]], list[list[float]]]:
    source = load_source_loadouts(loadouts)
    labels = [str(data.get("name") or Path(filename).stem) for filename, data in source]
    variants = [(tier, tp) for tier in tiers for tp in tp_levels]
    with tempfile.TemporaryDirectory(prefix="onebag-balance-") as raw_temp:
        temp = Path(raw_temp)
        folders: dict[tuple[int, int], Path] = {}
        for tier, tp in variants:
            folder = temp / f"t{tier}-tp{tp}"
            make_variant(source, folder, tier, tp, rarity, level)
            folders[(tier, tp)] = folder

        def one(variant: tuple[int, int]) -> list[float]:
            return cli_matrix(classpath, folders[variant], runs, items, sects, battle_config)

        if parallel and len(variants) > 1:
            with ThreadPoolExecutor(max_workers=min(len(variants), os.cpu_count() or 1)) as pool:
                columns = list(pool.map(one, variants))
        else:
            columns = [one(variant) for variant in variants]
    rows = [[column[index] for column in columns] for index in range(len(labels))]
    return labels, variants, rows


def print_table(
    labels: list[str], variants: list[tuple[int, int]], rows: list[list[float]],
    low: float, high: float,
) -> None:
    headers = [f"T{tier}/TP{tp}" for tier, tp in variants]
    width = max(18, min(38, max(len(label) for label in labels)))
    print(f"{'Loadout':<{width}} " + " ".join(f"{header:>10}" for header in headers))
    print("-" * (width + 1 + 11 * len(headers)))
    outside = 0
    for label, values in zip(labels, rows):
        cells = []
        for value in values:
            flag = "!" if value < low or value > high else " "
            outside += flag == "!"
            cells.append(f"{value:6.1f}%{flag:1}")
        print(f"{label[:width]:<{width}} " + " ".join(f"{cell:>10}" for cell in cells))
    print(f"Outside [{low:g}, {high:g}]: {outside}/{len(labels) * len(variants)}")


def effect_value(effect: dict[str, Any], address: str, value: Any = None, *, set_value: bool) -> Any:
    for field in ("value", "stacks", "duration"):
        if field in effect:
            if set_value:
                effect[field] = value
                return value
            return effect[field]
    raise ValueError(f"Effect at {address!r} has no value/stacks/duration field")


def item_parameter(items: list[dict[str, Any]], address: str, value: Any = None, *, set_value: bool) -> Any:
    item_id, separator, path = address.partition(".")
    if not separator:
        raise ValueError(f"Invalid item address: {address!r}")
    item = next((entry for entry in items if entry.get("id") == item_id), None)
    if item is None:
        raise ValueError(f"Unknown item in address: {address!r}")
    if path in {"attack", "cooldown", "crit"}:
        if path not in item:
            raise ValueError(f"Missing field in address: {address!r}")
        if set_value:
            item[path] = value
            return value
        return item[path]
    if path == "ultimate.cost":
        if not isinstance(item.get("ultimate"), dict) or "cost" not in item["ultimate"]:
            raise ValueError(f"Missing ultimate cost in address: {address!r}")
        if set_value:
            item["ultimate"]["cost"] = value
            return value
        return item["ultimate"]["cost"]
    group, dot, key = path.partition(".")
    if not dot or group not in {"basic", "triggers"}:
        raise ValueError(f"Unsupported item address: {address!r}")
    effects = item.get("basic", []) if group == "basic" else [
        effect for trigger in item.get("triggers", []) for effect in trigger.get("effects", [])
    ]
    effect = next((entry for entry in effects if entry.get("op") == key or entry.get("status") == key), None)
    if effect is None:
        raise ValueError(f"No matching effect in address: {address!r}")
    return effect_value(effect, address, value, set_value=set_value)


def sect_parameter(sects: list[dict[str, Any]], address: str, value: Any = None, *, set_value: bool) -> Any:
    match = re.fullmatch(r"sect:([^.]+)\.levels\.(\d+)\.([A-Za-z][A-Za-z0-9]*)", address)
    if not match:
        raise ValueError(f"Invalid sect address: {address!r}")
    sect_id, level_raw, field = match.groups()
    sect = next((entry for entry in sects if entry.get("id") == sect_id), None)
    level = None if sect is None else next(
        (entry for entry in sect.get("levels", []) if entry.get("level") == int(level_raw)), None
    )
    if level is None or field not in level:
        raise ValueError(f"Unknown sect level field in address: {address!r}")
    if set_value:
        level[field] = value
        return value
    return level[field]


def config_parameter(config: dict[str, Any], address: str, value: Any = None,
                     *, set_value: bool) -> Any:
    match = re.fullmatch(r"config:([A-Za-z][A-Za-z0-9]*)(?:\.(\d+))?", address)
    if not match:
        raise ValueError(f"Invalid config address: {address!r}")
    field, index_raw = match.groups()
    if field not in config:
        raise ValueError(f"Unknown battle config field in address: {address!r}")
    target = config[field]
    if index_raw is None:
        if not isinstance(target, (int, float)) or isinstance(target, bool):
            raise ValueError(f"Config field needs an index or is not numeric: {address!r}")
        if set_value:
            config[field] = value
            return value
        return target
    if not isinstance(target, list):
        raise ValueError(f"Config field is not an array: {address!r}")
    index = int(index_raw)
    if index >= len(target):
        raise ValueError(f"Config array index out of range: {address!r}")
    if not isinstance(target[index], (int, float)) or isinstance(target[index], bool):
        raise ValueError(f"Config array element is not numeric: {address!r}")
    if set_value:
        target[index] = value
        return value
    return target[index]


def parameter(items: list[dict[str, Any]], sects: list[dict[str, Any]], config: dict[str, Any], address: str,
              value: Any = None, *, set_value: bool = False) -> Any:
    if address.startswith("config:"):
        return config_parameter(config, address, value, set_value=set_value)
    if address.startswith("sect:"):
        return sect_parameter(sects, address, value, set_value=set_value)
    return item_parameter(items, address, value, set_value=set_value)


def load_patch(path: Path) -> dict[str, int | float]:
    patch = read_json(path)
    if not isinstance(patch, dict) or not all(
        isinstance(key, str) and isinstance(value, (int, float)) and not isinstance(value, bool)
        for key, value in patch.items()
    ):
        raise ValueError("Patch must be a JSON object mapping addresses to numbers")
    return patch


def patched_files(temp: Path, base_items: Path, base_sects: Path, config: dict[str, Any],
                  patch: dict[str, int | float]) -> tuple[Path, Path, Path]:
    items, sects = read_json(base_items), read_json(base_sects)
    for address, value in patch.items():
        parameter(items, sects, config, address, value, set_value=True)
    items_path, sects_path, config_path = temp / "items.json", temp / "sects.json", temp / "battle-config.json"
    write_json(items_path, items)
    write_json(sects_path, sects)
    # Burst objects are not tuning targets and BattleConfig.Burst is deliberately not
    # a general-purpose JSON DTO. Scalars and numeric arrays cover supported addresses.
    write_json(config_path, {key: entry for key, entry in config.items() if not isinstance(entry, dict)})
    return items_path, sects_path, config_path


def load_battle_config(classpath: str, override: Path | None) -> dict[str, Any]:
    with tempfile.TemporaryDirectory(prefix="onebag-config-") as raw_temp:
        dumped = Path(raw_temp) / "defaults.json"
        run_checked(["java", "-cp", classpath, MAIN_CLASS, "--dump-config", str(dumped)], ENGINE)
        config = read_json(dumped)
    if override is not None:
        supplied = read_json(override)
        if not isinstance(supplied, dict):
            raise ValueError("Battle config override must be a JSON object")
        unknown = sorted(set(supplied) - set(config))
        if unknown:
            raise ValueError("Unknown battle config field(s): " + ", ".join(unknown))
        config.update(supplied)
    return config


def matrix_command(args: argparse.Namespace, patch: dict[str, int | float] | None = None) -> None:
    classpath = ensure_classpath()
    tiers = parse_ints(args.tiers, "--tiers")
    tp_levels = parse_ints(args.tp, "--tp")
    items, sects = Path(args.items).resolve(), Path(args.sects).resolve()
    battle_config = Path(args.battle_config).resolve() if args.battle_config else None
    with tempfile.TemporaryDirectory(prefix="onebag-data-") as raw_temp:
        if patch is not None:
            config = load_battle_config(classpath, battle_config)
            items, sects, battle_config = patched_files(Path(raw_temp), items, sects, config, patch)
        labels, variants, rows = evaluate_matrix(
            classpath, Path(args.loadouts).resolve(), tiers, tp_levels,
            args.runs, items, sects, parallel=False, rarity=args.rarity, battle_config=battle_config,
            level=args.level,
        )
    print_table(labels, variants, rows, args.low, args.high)


def cli_match(classpath: str, a: Path, b: Path, runs: int, items: Path, sects: Path,
              battle_config: Path | None) -> float:
    if runs == 1:
        with tempfile.TemporaryDirectory(prefix="onebag-match-") as raw_temp:
            result = Path(raw_temp) / "result.json"
            command = [
                "java", "-cp", classpath, MAIN_CLASS, str(a), str(b), "--seed", "1",
                "--json", str(result), "--items", str(items), "--sects", str(sects),
            ]
            if battle_config is not None:
                command.extend(["--battle-config", str(battle_config)])
            run_checked(command, ENGINE)
            winner = read_json(result).get("winner")
            return 100.0 if winner == 0 else 0.0 if winner == 1 else 50.0
    command = [
        "java", "-cp", classpath, MAIN_CLASS, str(a), str(b), "--runs", str(runs),
        "--items", str(items), "--sects", str(sects),
    ]
    if battle_config is not None:
        command.extend(["--battle-config", str(battle_config)])
    output = run_checked(command, ENGINE).stdout
    percentages = re.findall(r"([0-9]+(?:\.[0-9]+)?)%", output)
    if len(percentages) < 3:
        raise RuntimeError("Could not parse battle result:\n" + output[-2000:])
    return float(percentages[0]) + float(percentages[2]) / 2


def rarity_step_command(args: argparse.Namespace) -> None:
    classpath = ensure_classpath()
    source = load_source_loadouts(Path(args.loadouts).resolve())
    items, sects = Path(args.items).resolve(), Path(args.sects).resolve()
    battle_config = Path(args.battle_config).resolve() if args.battle_config else None
    rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
    headers = ["Lương>Phàm", "Tinh>Lương", "Tuyệt>Tinh", "Thần Binh>Tuyệt", "Chí Tôn>Thần Binh",
               "Phàm T9 vs Lương T1"]
    rows: list[list[float]] = []
    with tempfile.TemporaryDirectory(prefix="onebag-rarity-") as raw_temp:
        temp = Path(raw_temp)
        for source_index, (filename, original) in enumerate(source):
            if args.level is not None:
                original = copy.deepcopy(original)
                original["level"] = args.level
            values = []
            for rarity_index in range(len(rarities) - 1):
                high, low = copy.deepcopy(original), copy.deepcopy(original)
                for placed in high["items"]:
                    placed["tier"], placed["rarity"] = args.tier, rarities[rarity_index + 1]
                for placed in low["items"]:
                    placed["tier"], placed["rarity"] = args.tier, rarities[rarity_index]
                high["tamPhapLevel"] = low["tamPhapLevel"] = args.tp
                high_path, low_path = temp / f"{source_index}-high.json", temp / f"{source_index}-low.json"
                write_json(high_path, high, indent=None)
                write_json(low_path, low, indent=None)
                values.append(cli_match(classpath, high_path, low_path, args.runs, items, sects, battle_config))
            uncommon_t1, common_t9 = copy.deepcopy(original), copy.deepcopy(original)
            for placed in uncommon_t1["items"]:
                placed["tier"], placed["rarity"] = 1, "uncommon"
            for placed in common_t9["items"]:
                placed["tier"], placed["rarity"] = 9, "common"
            uncommon_t1["tamPhapLevel"] = common_t9["tamPhapLevel"] = args.tp
            high_path, low_path = temp / f"{source_index}-u1.json", temp / f"{source_index}-c9.json"
            write_json(high_path, uncommon_t1, indent=None)
            write_json(low_path, common_t9, indent=None)
            values.append(cli_match(classpath, high_path, low_path, args.runs, items, sects, battle_config))
            rows.append(values)
    labels = [str(data.get("name") or Path(filename).stem) for filename, data in source]
    width = max(18, min(38, max(len(label) for label in labels)))
    print(f"{'Loadout':<{width}} " + " ".join(f"{header:>20}" for header in headers))
    print("-" * (width + 1 + 21 * len(headers)))
    for label, values in zip(labels, rows):
        print(f"{label[:width]:<{width}} " + " ".join(f"{value:19.1f}%" for value in values))
    averages = [sum(row[column] for row in rows) / len(rows) for column in range(len(headers))]
    print(f"{'Average':<{width}} " + " ".join(f"{value:19.1f}%" for value in averages))
    print("(draws count as half a win; the last column reports the Lương T1 side)")


def vs_command(args: argparse.Namespace) -> None:
    """Run every unmodified loadout in A against every unmodified loadout in B."""
    classpath = ensure_classpath()
    source_a = load_source_loadouts(Path(args.a).resolve())
    source_b = load_source_loadouts(Path(args.b).resolve())
    level_a = args.level_a if args.level_a is not None else args.level
    level_b = args.level_b if args.level_b is not None else args.level
    for source, level in ((source_a, level_a), (source_b, level_b)):
        if level is not None:
            for _, loadout in source:
                loadout["level"] = level
    items, sects = Path(args.items).resolve(), Path(args.sects).resolve()
    battle_config = Path(args.battle_config).resolve() if args.battle_config else None
    labels_a = [str(data.get("name") or Path(filename).stem) for filename, data in source_a]
    labels_b = [str(data.get("name") or Path(filename).stem) for filename, data in source_b]
    with tempfile.TemporaryDirectory(prefix="onebag-vs-") as raw_temp:
        temp = Path(raw_temp)
        paths_a, paths_b = [], []
        for side, source, output in (("a", source_a, paths_a), ("b", source_b, paths_b)):
            for index, (_, loadout) in enumerate(source):
                path = temp / f"{side}-{index}.json"
                # Deliberately preserve each placed item's tier and rarity. Unlike
                # matrix, vs compares the JSON loadouts exactly as authored.
                write_json(path, loadout, indent=None)
                output.append(path)
        rows = [[cli_match(classpath, a, b, args.runs, items, sects, battle_config)
                 for b in paths_b] for a in paths_a]
    row_width = max(18, min(38, max(len(label) for label in labels_a)))
    col_width = max(12, min(28, max(len(label) for label in labels_b)))
    print("{:<{}} ".format("A \\ B", row_width) + " ".join(f"{label[:col_width]:>{col_width}}" for label in labels_b))
    print("-" * (row_width + 1 + (col_width + 1) * len(labels_b)))
    for label, values in zip(labels_a, rows):
        print(f"{label[:row_width]:<{row_width}} " + " ".join(f"{value:>{col_width - 1}.1f}%" for value in values))
    print("(tỷ lệ thắng của A; hòa tính nửa trận thắng; giữ nguyên Trọng và phẩm trong từng JSON)")


def normalize_number(value: float, original: Any) -> int | float:
    rounded = round(value, 10)
    if isinstance(original, int) and float(rounded).is_integer():
        return int(rounded)
    return rounded


def tune_command(args: argparse.Namespace) -> None:
    config = read_json(Path(args.config).resolve())
    raw_params = config.get("parameters") if isinstance(config, dict) else None
    if not isinstance(raw_params, list) or not raw_params:
        raise ValueError("Tune config must contain a non-empty 'parameters' array")
    params = []
    for index, entry in enumerate(raw_params):
        if not isinstance(entry, dict) or not all(key in entry for key in ("address", "step", "min", "max")):
            raise ValueError(f"Parameter {index} needs address, step, min, and max")
        if entry["step"] <= 0 or entry["min"] > entry["max"]:
            raise ValueError(f"Invalid bounds/step for parameter {entry.get('address', index)!r}")
        params.append(entry)

    classpath = ensure_classpath()
    tiers, tp_levels = parse_ints(args.tiers, "--tiers"), parse_ints(args.tp, "--tp")
    base_items, base_sects = read_json(Path(args.items).resolve()), read_json(Path(args.sects).resolve())
    base_config = load_battle_config(
        classpath, Path(args.battle_config).resolve() if args.battle_config else None
    )
    values: dict[str, int | float] = {}
    originals: dict[str, int | float] = {}
    for entry in params:
        address = entry["address"]
        if address in values:
            raise ValueError(f"Duplicate parameter address: {address}")
        original = parameter(base_items, base_sects, base_config, address)
        start = entry.get("start", original)
        if not entry["min"] <= start <= entry["max"]:
            raise ValueError(f"Start value outside bounds for {address}")
        originals[address] = original
        values[address] = start

    evaluations = 0
    started = time.monotonic()

    def log(message: str) -> None:
        with PRINT_LOCK:
            print(message, file=sys.stderr, flush=True)

    def evaluate(candidate: dict[str, int | float]) -> float:
        nonlocal evaluations
        evaluations += 1
        with tempfile.TemporaryDirectory(prefix="onebag-tune-") as raw_temp:
            items_path, sects_path, config_path = patched_files(
                Path(raw_temp), Path(args.items).resolve(), Path(args.sects).resolve(),
                copy.deepcopy(base_config), candidate
            )
            _, _, rows = evaluate_matrix(
                classpath, Path(args.loadouts).resolve(), tiers, tp_levels,
                args.runs, items_path, sects_path, parallel=True, rarity=args.rarity,
                battle_config=config_path, level=args.level,
            )
        score = sum(
            max(0.0, value - args.high) ** 2 + args.w_low * max(0.0, args.low - value) ** 2
            for row in rows for value in row
        )
        log(f"eval {evaluations}: objective={score:.2f}")
        return score

    best = evaluate(values)
    log(f"start: objective={best:.2f}")
    for round_index in range(1, args.rounds + 1):
        improved = False
        for entry in params:
            address = entry["address"]
            for direction in (1, -1):
                while True:
                    raw = float(values[address]) + direction * float(entry["step"])
                    candidate_value = normalize_number(raw, originals[address])
                    if candidate_value < entry["min"] or candidate_value > entry["max"]:
                        break
                    trial = dict(values)
                    trial[address] = candidate_value
                    score = evaluate(trial)
                    if score < best - 1e-9:
                        values, best, improved = trial, score, True
                        log(f"round {round_index}: {address}={candidate_value} objective={best:.2f}")
                    else:
                        break
        log(f"round {round_index} done: objective={best:.2f}")
        if not improved:
            break
    output = Path(args.out).resolve()
    output.parent.mkdir(parents=True, exist_ok=True)
    write_json(output, values)
    log(f"wrote {output} ({evaluations} evaluations, {time.monotonic() - started:.1f}s)")
    print(json.dumps(values, ensure_ascii=False, indent=2))
    print(f"Objective: {best:.2f}")


def positive_int(raw: str) -> int:
    value = int(raw)
    if value < 1:
        raise argparse.ArgumentTypeError("must be at least 1")
    return value


def add_matrix_options(parser: argparse.ArgumentParser) -> None:
    parser.add_argument("--loadouts", default=str(SAMPLES), help="folder containing loadout JSON files")
    parser.add_argument("--tiers", default="1,3,6,9", help="comma-separated item tiers (default: %(default)s)")
    parser.add_argument("--tp", default="1", help="comma-separated Tâm Pháp levels (default: %(default)s)")
    parser.add_argument("--items", default=str(DEFAULT_ITEMS), help="base items JSON")
    parser.add_argument("--sects", default=str(DEFAULT_SECTS), help="base sects JSON")
    parser.add_argument("--battle-config", help="partial BattleConfig JSON override")
    parser.add_argument("--runs", type=positive_int, default=200, help="seeds per matchup (default: %(default)s)")
    parser.add_argument("--low", type=float, default=35.0, help="low average threshold")
    parser.add_argument("--high", type=float, default=65.0, help="high average threshold")
    parser.add_argument("--rarity", choices=("common", "uncommon", "rare", "epic", "legendary", "supreme"),
                        help="force every placed copy to this rarity")
    parser.add_argument("--level", type=positive_int, help="override hero level in every loadout")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    subparsers = parser.add_subparsers(dest="command", required=True)
    matrix = subparsers.add_parser("matrix", help="run average win-rate matrices")
    add_matrix_options(matrix)
    matrix.set_defaults(handler=lambda args: matrix_command(args))

    whatif = subparsers.add_parser("whatif", help="run matrices with a temporary JSON patch")
    add_matrix_options(whatif)
    whatif.add_argument("--patch", required=True, help="flat address-to-value JSON patch")
    whatif.set_defaults(handler=lambda args: matrix_command(args, load_patch(Path(args.patch).resolve())))

    tune = subparsers.add_parser("tune", help="coordinate-descent parameter tuning")
    add_matrix_options(tune)
    tune.add_argument("--config", required=True, help="parameter config JSON")
    tune.add_argument("--out", required=True, help="destination for the best flat patch JSON")
    tune.add_argument("--rounds", type=positive_int, default=6, help="maximum descent rounds")
    tune.add_argument("--w-low", type=float, default=0.3, help="penalty weight below --low")
    tune.set_defaults(handler=tune_command)

    rarity_step = subparsers.add_parser("rarity-step", help="measure each one-rarity advantage")
    rarity_step.add_argument("--loadouts", required=True, help="folder containing loadout JSON files")
    rarity_step.add_argument("--tier", type=positive_int, default=3, help="item tier for rarity steps")
    rarity_step.add_argument("--tp", type=positive_int, default=1, help="Tâm Pháp level")
    rarity_step.add_argument("--level", type=positive_int, help="override hero level in every loadout")
    rarity_step.add_argument("--runs", type=positive_int, required=True, help="seeds per matchup")
    rarity_step.add_argument("--items", default=str(DEFAULT_ITEMS), help="base items JSON")
    rarity_step.add_argument("--sects", default=str(DEFAULT_SECTS), help="base sects JSON")
    rarity_step.add_argument("--battle-config", help="partial BattleConfig JSON override")
    rarity_step.set_defaults(handler=rarity_step_command)

    versus = subparsers.add_parser("vs", help="run every authored loadout in A against every loadout in B")
    versus.add_argument("--a", required=True, help="folder containing side A loadout JSON files")
    versus.add_argument("--b", required=True, help="folder containing side B loadout JSON files")
    versus.add_argument("--runs", type=positive_int, required=True, help="seeds per matchup")
    versus.add_argument("--level", type=positive_int, help="override both sides' hero level")
    versus.add_argument("--level-a", type=positive_int, help="override side A hero level")
    versus.add_argument("--level-b", type=positive_int, help="override side B hero level")
    versus.add_argument("--items", default=str(DEFAULT_ITEMS), help="base items JSON")
    versus.add_argument("--sects", default=str(DEFAULT_SECTS), help="base sects JSON")
    versus.add_argument("--battle-config", help="partial BattleConfig JSON override")
    versus.set_defaults(handler=vs_command)
    return parser


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
    try:
        args = build_parser().parse_args()
        if hasattr(args, "low") and args.low > args.high:
            raise ValueError("--low cannot exceed --high")
        if ((hasattr(args, "tier") and args.tier > 9)
                or (hasattr(args, "tp") and isinstance(args.tp, int) and args.tp > 9)):
            raise ValueError("--tier and --tp must be in 1..9")
        args.handler(args)
        return 0
    except (ValueError, RuntimeError, OSError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
