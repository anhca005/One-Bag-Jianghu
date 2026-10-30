"""Load test for the game server: N WebSocket clients run a fixed session and record round-trip times and bytes.

Each client: connect, hello, register a fresh account, then
  - PROFILE_GETS x profile_get (cheap read that returns the whole profile),
  - PVE_BATTLES x pve_battle (server simulates a battle and returns its event log),
  - one arena_find (rated match against a ghost or a bot).

Usage:
  py tools/net/ws_load_test.py --url ws://localhost:18765/ws --clients 1 10 25 50 --repeats 3 --out out.csv
"""

import argparse
import asyncio
import csv
import json
import statistics
import time
import uuid

import aiohttp

PROFILE_GETS = 20
PVE_BATTLES = 5
PASSWORD = "loadtest123"


class Client:
    def __init__(self, socket):
        self.socket = socket
        self.next_id = 0
        self.records = []

    async def request(self, message_type, payload):
        """Sends one request and waits for the reply with the same id; skips pushed messages (replyTo 0)."""
        self.next_id += 1
        envelope = json.dumps({"type": message_type, "id": self.next_id, "payload": payload})
        started = time.perf_counter()
        await self.socket.send_str(envelope)
        while True:
            message = await self.socket.receive(timeout=60)
            if message.type != aiohttp.WSMsgType.TEXT:
                raise RuntimeError("connection closed during " + message_type)

            reply = json.loads(message.data)
            if reply.get("replyTo") == self.next_id:
                break

        elapsed_ms = (time.perf_counter() - started) * 1000
        self.records.append({
            "type": message_type,
            "reply": reply["type"],
            "rtt_ms": elapsed_ms,
            "sent_bytes": len(envelope.encode("utf-8")),
            "received_bytes": len(message.data.encode("utf-8")),
        })
        return reply


async def run_client(session, url, run_tag, index):
    async with session.ws_connect(url) as socket:
        client = Client(socket)
        await client.request("hello", {"clientVersion": "loadtest", "heroName": "Bench"})
        username = ("lt_" + run_tag + "_" + str(index))[:20]
        await client.request("register", {"username": username, "password": PASSWORD, "heroName": "Bench"})
        for _ in range(PROFILE_GETS):
            await client.request("profile_get", {})

        for _ in range(PVE_BATTLES):
            await client.request("pve_battle", {"stageId": "hoi_1_01"})

        await client.request("arena_find", {})
        return client.records


async def run_level(url, clients):
    run_tag = uuid.uuid4().hex[:8]
    started = time.perf_counter()
    async with aiohttp.ClientSession() as session:
        tasks = [run_client(session, url, run_tag, i) for i in range(clients)]
        results = await asyncio.gather(*tasks, return_exceptions=True)

    duration = time.perf_counter() - started
    errors = [r for r in results if isinstance(r, Exception)]
    records = [record for r in results if not isinstance(r, Exception) for record in r]
    return records, errors, duration


def percentile(values, fraction):
    ordered = sorted(values)
    position = min(len(ordered) - 1, int(round(fraction * (len(ordered) - 1))))
    return ordered[position]


def summarize(records):
    rows = {}
    for message_type in sorted({r["type"] for r in records}):
        subset = [r for r in records if r["type"] == message_type]
        errors = sum(1 for r in subset if r["reply"] == "error")
        rtts = [r["rtt_ms"] for r in subset]
        rows[message_type] = {
            "count": len(subset),
            "errors": errors,
            "rtt_mean_ms": statistics.mean(rtts),
            "rtt_p95_ms": percentile(rtts, .95),
            "sent_bytes_mean": statistics.mean(r["sent_bytes"] for r in subset),
            "received_bytes_mean": statistics.mean(r["received_bytes"] for r in subset),
        }
    return rows


async def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--url", default="ws://localhost:18765/ws")
    parser.add_argument("--clients", type=int, nargs="+", default=[1, 10, 25, 50])
    parser.add_argument("--repeats", type=int, default=3)
    parser.add_argument("--out", default="loadtest.csv")
    args = parser.parse_args()

    fields = ["clients", "repeat", "type", "count", "errors", "rtt_mean_ms", "rtt_p95_ms",
              "sent_bytes_mean", "received_bytes_mean", "connection_errors", "duration_s"]
    with open(args.out, "w", newline="", encoding="utf-8") as handle:
        writer = csv.DictWriter(handle, fieldnames=fields)
        writer.writeheader()
        for clients in args.clients:
            for repeat in range(1, args.repeats + 1):
                records, errors, duration = await run_level(args.url, clients)
                for message_type, row in summarize(records).items():
                    writer.writerow({"clients": clients, "repeat": repeat, "type": message_type, **row,
                                     "connection_errors": len(errors), "duration_s": round(duration, 2)})
                print(f"clients={clients} repeat={repeat} requests={len(records)} "
                      f"connection_errors={len(errors)} duration={duration:.1f}s")
                for error in errors[:3]:
                    print("  error:", repr(error))


if __name__ == "__main__":
    asyncio.run(main())
