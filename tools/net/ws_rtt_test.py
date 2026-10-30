"""Guest-only RTT test: connect, hello, then sequential battle_request. Writes nothing to the server DB."""
import argparse
import asyncio
import csv
import json
import statistics
import time

import aiohttp


async def client(url, loadout, repeats, rows, label, clients):
    async with aiohttp.ClientSession() as session:
        started = time.perf_counter()
        async with session.ws_connect(url) as ws:
            connect_ms = (time.perf_counter() - started) * 1000
            rows.append([label, clients, "connect", connect_ms, True])
            message_id = 1
            for kind in ["hello"] + ["battle_request"] * repeats:
                payload = {} if kind == "hello" else {"mode": "pve", "loadout": loadout}
                sent = time.perf_counter()
                await ws.send_str(json.dumps({"id": message_id, "type": kind, "payload": payload}))
                reply = json.loads((await ws.receive(timeout=30)).data)
                rtt = (time.perf_counter() - sent) * 1000
                ok = reply.get("type") in ("hello_ok", "battle_result")
                rows.append([label, clients, kind, rtt, ok])
                message_id += 1


async def run(url, label, client_counts, repeats, loadout, rows):
    for clients in client_counts:
        started = time.perf_counter()
        await asyncio.gather(*(client(url, loadout, repeats, rows, label, clients) for _ in range(clients)))
        elapsed = time.perf_counter() - started
        done = [r for r in rows if r[0] == label and r[1] == clients and r[2] == "battle_request"]
        rtts = sorted(r[3] for r in done)
        p95 = rtts[int(len(rtts) * .95) - 1]
        errors = sum(1 for r in done if not r[4])
        print(f"{label} clients={clients} n={len(done)} err={errors} mean={statistics.mean(rtts):.1f} "
              f"p50={statistics.median(rtts):.1f} p95={p95:.1f} req/s={len(done) / elapsed:.1f}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--url", required=True)
    parser.add_argument("--label", required=True)
    parser.add_argument("--clients", type=int, nargs="+", default=[1, 5, 10])
    parser.add_argument("--repeats", type=int, default=20)
    parser.add_argument("--loadout", required=True)
    parser.add_argument("--out", required=True)
    args = parser.parse_args()
    with open(args.loadout, encoding="utf8") as handle:
        loadout = json.load(handle)
    rows = []
    asyncio.run(run(args.url, args.label, args.clients, args.repeats, loadout, rows))
    with open(args.out, "w", newline="", encoding="utf8") as handle:
        writer = csv.writer(handle)
        writer.writerow(["label", "clients", "kind", "rtt_ms", "ok"])
        writer.writerows(rows)


main()
