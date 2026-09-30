# Bag grid
`ItemCatalog` and `SectCatalog` read the server-compatible JSON in `Resources/Data`.
`GridRect` implements fit, overlap, and edge-adjacency rules.
`BagGrid` owns placed instances, movement, tiers, neighbours, and change notifications.
`LoadoutSerializer` preserves optional sect and Tâm Pháp level metadata without coupling it to the grid.
`ItemIconLibrary` maps item ids to sprites and safely falls back when the library is missing.
Use `One Bag Jianghu/Rebuild Icon Library` after adding PNGs under `Textures2D`.
The icon importer configures those PNGs as single, transparent, non-mipmapped sprites.
`BagGridView`, `ItemTrayView`, and `ItemView` render and edit the 5x4 loadout.
The sect dropdown marks compatible weapons with a gold `TP`; its level stepper is limited to 1..9.
Save and Load use `Application.persistentDataPath/loadout.json`, including sect metadata.
Drop a placed item on the tray to remove it; right-click it to cycle tiers.
Use `One Bag Jianghu/Sync Data From Server` to copy `items.json` and `sects.json`.
The sync source is the sibling `server/battle-engine/src/main/resources/data` folder.
`BagDemoBootstrap` creates all required legacy UGUI objects at runtime.
Known limit: items cannot rotate.
