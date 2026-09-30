# Popup system

Create a popup by deriving from `BasePopup` or `BasePopup<TData>`.
Add a public static `Build(GameObject root)` method that builds and assigns its UI.
`Build` is editor-only in usage; runtime always loads the generated prefab.
Run **One Bag Jianghu > Build Popup Prefabs** after adding or changing a popup.
Show it with `PopupManager.Show<MyPopup>(data)`.

Lifecycle:

`Awake → Setup → Show → OnEnable → animation → OnShown`

`Close → animation → Hide → OnHidden → Closed → OnDisable`

Override `OnEnable` and `OnDisable` for subscriptions and always call `base`.
Use `Refresh` to redraw from current `Data`.
Use `CloseWithResult` when callers need a value from the `Closed` event.

Scene UI uses sorting orders 0–99.
The manager canvas starts at 100.
Stack level `i` uses backdrop `100 + 2i` and popup `101 + 2i`.
Loading remains above popups at sorting order 1000.
