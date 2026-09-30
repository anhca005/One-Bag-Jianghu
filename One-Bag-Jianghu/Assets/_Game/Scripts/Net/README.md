# Game server connection

Start the Java server with the command documented under **“Chạy game server”** in `../server/README.md`, then run the Unity game. The client connects when the player presses **“Vào Giang Hồ”**. If the server is unavailable, gameplay remains available with the bundled sample replay.

The default endpoint is `ws://localhost:8765/ws`.

Override it persistently from code or the Unity Console with:

```csharp
PlayerPrefs.SetString(ServerConfig.PlayerPrefsKey, "ws://host:port/ws");
PlayerPrefs.Save();
```

Override it for one standalone-player launch with:

```text
One-Bag-Jianghu.exe -server ws://host:port/ws
```

The `-server` command-line value takes precedence over PlayerPrefs.
