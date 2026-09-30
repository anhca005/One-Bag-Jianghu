# Battle replay data layer

`BattleLogParser.Parse(json)` converts a server JSON log into typed replay data.
`BattleLog` holds battle metadata, item references, and events.
`BattleEvent` preserves optional event fields and the original type string.
`BattleEventType.Unknown` safely represents event types added by the server later.
`BattleTimeline` stable-sorts events by timestamp while preserving source order.
`CollectEvents(from, to, buffer)` appends events in the `(from, to]` window.
Reuse the caller-owned buffer to avoid playback allocations.
`StateAt(side, time)` returns state after all events at or before that time.
Timeline checkpoints keep state queries from rescanning the full log.
`FighterState` includes core stats, build-up meters, and stun expiry.

```csharp
BattleLog log = BattleLogParser.Parse(json);
BattleTimeline timeline = new BattleTimeline(log);
timeline.CollectEvents(previousMs, currentMs, eventBuffer);
FighterState left = timeline.StateAt(0, currentMs);
```

## Replay demo

1. Create an empty scene.
2. Create an empty GameObject.
3. Add `ReplayDemoBootstrap`.
4. Enter Play mode.

The bootstrap creates the Canvas and EventSystem, then loads
`Resources/Replays/replay_sample.json`. A different battle log can be assigned to
the component's **Log** field. Playback supports pause, x1/x2/x4 speed, seeking,
restart, item status tags, floating combat text, and a recent-event feed.

## Attack VFX

Replay VFX use the replay clock, so pausing freezes animations and x2/x4 playback
accelerates them. Seeking and restarting clear all active effects without replaying
past effects. Effect objects are pooled by `ReplayVfxLayer`.

Weapon `TRIGGER` events launch an effect from the weapon's bag rectangle to the
opposing fighter. `DAMAGE` events show the `hit` effect on their target, preserving
the impact timing supplied by the server. `BURST` and `PARRY` effects are centered
on the event side's fighter panel.

The icon library supplies VFX sprites using these file names (without `.png`):

- Travelling: `<weaponClass>_basic` and `<weaponClass>_ultimate`. The source art is
  landscape, 1536x1024, and faces left to right. Current classes are `fist`,
  `staff`, `sword`, `blade`, `hidden`, `claw`, `axe`, `brush`, `flute`, and
  `zither`; future class names work without code changes.
- Centered, square: `burst_burn`, `burst_poison`, `burst_bleed`, `parry`, and `hit`.

Place PNGs anywhere below `Assets/_Game/Textures2D/` and rebuild the Item Icon
Library. Missing sprites use runtime-generated colored shapes, so VFX remain visible
before final art is installed.
