# Porting a legacy Unity project into the collection

How Herbie (Unity 4.5.2) was brought in straight from its project folder, written as a
procedure for the next game. The earlier games came in as a `.unitypackage` exported
from their old Unity version plus an Input Manager JSON (`Tools > Collection > Import
Game Package...`); this is the route for when all there is is the old project folder.

The rule throughout: the game is legacy content to keep working, not to clean up. Every
change beyond what Unity 6 or the collection forces is marked in the code with a comment
starting `In the collection:`.

Herbie took one session. The order below is the order that worked; the pitfalls are in
the steps where they bit.

## 0. Look before touching anything

Work on a branch (`feature/<name>`), from the repository root.

- **Unity version and format.** Open `Assets/*.unity` in a hex/text viewer. Unity 4
  projects are binary and carry the version near the top (`4.5.2f1`). Unity 6 read
  Herbie's binary scene and prefabs without help. A text (`%YAML`) project is easier
  still.
- **Product name.** It is in `ProjectSettings/ProjectSettings.asset` (strings scan). Use
  it as the game name: it becomes the folder, the namespace (`Games.<Name>`, spaces
  removed), the action map name and the GameList entry, and all four must match.
- **Read every script.** They are short. Note: which input axes and buttons are used and
  how; `Application.Quit` / `LoadLevel`; `Resources.Load` names; statics that assume a
  fresh process; anything reading input inside physics callbacks.
- **Tags, layers, physics.** Strings-scan `TagManager.asset` for custom tags and layers
  (Herbie had none; a game that has them needs them added to the project, or its code
  changed). Note the gravity the game sets or expects.
- **Clashes.** Check that none of the game's `.meta` GUIDs exist in `Assets/` already,
  and that no file directly under its `Resources/` shares a name with one in another
  game's `Resources/`.

## 1. Copy the assets in, with their meta files

Copy `legacy unity projects/<game>/Assets/*` to `Assets/games/<Name>/`.

- **The `.meta` files are hidden files.** Copy with `-Force` (PowerShell) or they stay
  behind, every GUID changes, and every reference in every scene and prefab breaks.
  Count files before and after. Clear the hidden attribute afterwards.
- Move everything directly under `Resources/` into `Resources/<Name>/` (files and their
  metas together), so load names cannot collide across games.

Do not let Unity see the folder yet.

## 2. Scripts

    python "legacy unity projects/tools/legacy_scripts.py" "Assets/games/<Name>" "<Name>"

It rewrites legacy `Input` calls to `TaloketoInputManager`, the component shortcuts
Unity 5 removed (`rigidbody2D`, `collider2D`, `camera`, ...) to `GetComponent<>()`,
`Application.LoadLevel(0)` to a reload of the active scene, and wraps everything in
`namespace Games.<Name>`. Script GUIDs are untouched, so component bindings survive.
It prints what it changed and a `BY HAND:` list. By hand:

- **Quitting.** An Escape-quit is deleted (the collection has its own exit: Start+Select
  or Shift+Escape). Any other `Application.Quit()` becomes
  `GlobalInputManager.ReturnToMainMenu()`.
- **`Resources.Load("x")`** becomes `Resources.Load("<Name>/x")`.
- **Axis compared with exactly 1 or -1** (`GetAxisRaw("Horizontal") == 1`) becomes
  `> 0.5f` / `< -0.5f`: a stick rarely reads exactly 1.
- **`Input.GetAxis` smoothing.** The collection's `GetAxis` is the raw value. Where the
  game relied on the old easing (Herbie's fight slides him sideways), ease it locally
  with the Input Manager's own numbers from step 3 (`Fight.cs` has a ten-line `smooth`).
- **Button presses read in physics callbacks.** `GetButtonDown` inside
  `OnTriggerStay2D`/`OnCollisionStay2D`/`FixedUpdate` misses presses, because a press
  lasts one frame and physics runs at 50 Hz: about one in six at 60 fps, most at 144.
  Latch the press in `Update` and hold it until one physics step has seen it
  (`Herbie.cs`: `interactDown`/`interactSeen`). Also set the body's `sleepMode` to
  `NeverSleep`, since a sleeping body gets no `OnTriggerStay2D`.
- **Statics.** In the collection the process outlives the game, so statics a game left
  set (a mini-game flag, a timer, a tint) are still set at its next start. Reset them in
  the game's first `Awake`.
- **Float index maths** like `FloorToInt((time % (period * n)) / period)` can land on
  `n`. Clamp it. (It was a latent bug; it surfaces as an exception that pauses the
  editor.)

## 3. Input map

    python "legacy unity projects/tools/legacy_input_axes.py" "legacy unity projects/<game>/ProjectSettings/InputManager.asset"

prints the axes the game was written against, with alternates and smoothing values. (A
plain strings scan of that binary file drops one-letter keys, which is how WASD and Z
were nearly lost for Herbie.)

Add a map named `<Name>` to `Assets/Resources/Input/CollectionInput.inputactions`, one
action per legacy axis, same names:

- two-key axes: `Value`/`Axis` action, a `1DAxis` composite per key pair (main and alt);
- buttons: `Button` action, one binding per key;
- then the gamepad, which the originals never had: left stick and d-pad on the movement
  axes, `buttonSouth` on the main action, `start` on a menu/confirm action. Leave debug
  cheats on the keyboard only. Leave out `Quit`.

Edit the JSON with a script that appends to it (`json.load` with `OrderedDict`, keep the
file's line endings), not by hand; check `git diff --stat` shows insertions only.

## 4. Let Unity import, then re-save as text

Refresh/compile in the editor and fix compile errors until the console is clean. Then:

- Open the game's scene and check: no missing scripts, and the main object's public
  fields are all assigned.
- `AssetDatabase.ForceReserializeAssets(<every asset path under the folder>,
  ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)` rewrites the binary
  scene, prefabs and metas in the current text format. Do this with another scene open.
- Add the scene to Build Settings, and an entry to `Assets/Resources/Games/GameList.asset`
  (`gameName`, `entryScene`/`entryScenePath`, the game's gravity, `enableMouseEmulation`
  only for a mouse-only game).

Commit and push here: a checkpoint that compiles and opens.

## 5. Repair what the import dropped silently

    python "legacy unity projects/tools/legacy_sprite_ids.py" "legacy unity projects/<game>/Assets" "Assets/games/<Name>"
    python "legacy unity projects/tools/legacy_sprite_ids.py" "legacy unity projects/<game>/Assets" "Assets/games/<Name>" --apply

Images that were renamed in the old project have their sprite on file ID 21300002
instead of 21300000. Unity 6 drops references to them without any error: the object just
has no picture (Herbie's pacman and one star). The tool finds them in the legacy metas
and repoints the references.

- Editing the open scene's file makes Unity show a modal "modified externally" dialog
  that blocks everything, including MCP calls. Have another scene open, or click Reload.
- Then scan every component in the scene and in every prefab for broken references: a
  `SerializedProperty` of type `ObjectReference` whose `objectReferenceValue` is null but
  whose `objectReferenceInstanceIDValue` is not zero. Zero broken is the bar. Sprites
  were the only kind Herbie lost; audio clips, fonts, prefab links and script bindings
  all survived.

## 6. Play it through

Set `DISABLESTEAMWORKS` first. Being loadable is not being playable: every real bug in
Herbie was found by playing, none by inspection.

- Drive it with a virtual gamepad (`InputSystem.AddDevice<Gamepad>` +
  `QueueStateEvent`) from an `EditorApplication.update` hook that steps through a list
  ("go to X", "tap", "wait", "screenshot"), logging to a file. Screenshots with
  `ScreenCapture.CaptureScreenshot`. A virtual keyboard or mouse only works while the
  editor window is focused.
- Cover: the start screen, moving around, every interaction, every mini-game or
  sub-scene, every ending, the restart, and going back to the collection's menu and in
  again (`GlobalInputManager.ReturnToMainMenu()`, then load the scene).
- Check the keyboard bindings as well as the pad.
- The editor runs unfocused at hundreds of frames a second, which exposes
  frame-rate-dependent code. That is useful, not noise: a 144 Hz monitor has the same
  effect on players.
- Any exception pauses the editor ("Error Pause") and stalls the bot; read the console
  when a run stops making progress.
- Shortcuts are fine for reaching late states (setting a timer forward, moving the
  player onto a pickup), but say which ones were taken.

## 7. Finish

- On-screen prompts in text objects: name the gamepad's button while a pad is the device
  in use (Herbie's `Menu.cs`). Prompts drawn into the art are left alone and reported.
- Remove the virtual devices, clear the dynamic TMP font assets
  (`ClearFontAssetData(true)`, never `git checkout` them), reopen the main menu scene.
- Commit, push, give the review link. Report what was tested, what shortcuts the test
  took, every decision made on the owner's behalf, and anything that looked off but was
  left as imported.
- Add the game's entry to `MIGRATION.md`.

## What to expect from other games

Herbie was a kind case: one scene, 2D, no custom layers or tags, no plugins, no legacy
GUI. Things it did not exercise, to check for in step 0:

- `OnGUI`, `GUIText`, `GUITexture` (removed in Unity 2019): need rebuilding with UI or
  TextMesh.
- Custom tags, layers, sorting layers and the 2D collision matrix: live in project
  settings, not in the game's folder.
- Several scenes: `Application.LoadLevel(n)` / `LoadLevel("name")` must become scene
  paths, as in Space Artist.
- Shaders written for the built-in pipeline: the project is URP. (`Sprites/Default` and
  the legacy text shader worked; in a custom sprite shader the renderer's colour arrives
  as `_RendererColor`, not in the vertices.)
- Mouse-driven games: `enableMouseEmulation` in GameList, and `TaloketoInputManager`'s
  mouse calls.
- Plugins, `PlayerPrefs` keys shared between games, legacy particle systems and
  animations.
- The project's colour space is Linear; old projects were Gamma, so semi-transparent
  blends come out lighter (`Tools > Color Space` switches it for a comparison).

## Added by Lost Shader (Unity 5.3)

- **Script updater dialog.** The first compile can raise a modal "Script Updating
  Consent" (for `body.velocity` and the like, which `legacy_scripts.py` only rewrites
  after a `GetComponent`). It blocks MCP exactly like the reload dialog. Answer "Yes,
  just for these files".
- **Collision matrix and auto-sync.** Decode `m_LayerCollisionMatrix` at the end of the
  old `Physics2DSettings.asset` (one 32-bit row per layer) and compare with the layers
  the scene's colliders are on. Reproduce what matters with
  `Physics2D.IgnoreLayerCollision` in the game's first `Awake`, restored in `OnDestroy`.
  Do the same for `Physics2D.autoSyncTransforms = true` when the game moves bodies
  through their transforms; the collection has it off, Unity 5 had no such switch.
- **`lossyScale` on mirrored objects** is negative in Unity 6. A test like
  `lossyScale.x > 0.1f` on something that flips to face left needs `Mathf.Abs`.
- **Tint colours on legacy shaders in Linear space.** `Particles/Alpha Blended` doubles
  `_TintColor`; 0.5 grey meant "unchanged" in Gamma and means 0.43 in Linear. Anything
  that feeds its own output back (a camera with clear flags Depth/Nothing into a render
  texture shown on a plane) loses its trail. Convert the tint with
  `Mathf.LinearToGammaSpace` at start. The feedback loop itself works under URP.
- **`Input.anyKeyDown`** has no collection equivalent; walk `Keyboard.current.allKeys`
  and the pad's buttons (`Game.anyKeyDown` in Lost Shader).
- **Devices the game opens** (webcam, microphone) must be stopped in `OnDestroy`.
- **Test bots:** walking in a straight line gets stuck on scenery; place the player next
  to the goal instead and say so. Log on state changes only. Cap the frame rate at 60
  (`Application.targetFrameRate`) when judging per-frame effects.

## Added by Ode to Cactus (Unity 4.6, 3D, several scenes)

- **Lit 3D scenes are magenta under URP.** Every built-in lit shader (`Diffuse`,
  `Transparent/Diffuse`, the `Default-Diffuse` material on primitives) has no URP pass.
  `Assets/games/Ode to Cactus/collection/` has a shader and a light feeder that
  reproduce Unity 4's forward lighting; copy and rename them for the next lit game
  rather than switching to URP/Lit, which changes the look entirely. Renderers on the
  built-in default material have to be repointed in the scenes (an editor script: any
  `sharedMaterial` whose asset path starts with `Resources/unity_builtin`).
  `ShaderUtil.GetShaderMessages` tells why a new shader is still magenta.
- **Upgraded light intensities.** Unity rewrites a Unity 4 intensity `i` as
  `(2i)^(1/2.2)`; code that sets `Light.intensity` still uses the old numbers.
- **UnityScript (`.js`).** Translate to C#, keep the class and field names, and rename
  the `.js.meta` to `.cs.meta` so the GUID and the scene bindings survive. A function
  containing `yield` was started as a coroutine implicitly: `StartCoroutine` it.
- **Standard Assets, editor plugins, `Thumbs.db`, `.pdn`:** copy only what the scenes
  use. A scan for missing scripts and broken references after import shows if too much
  was left out.
- **Several scenes.** `Application.LoadLevel("name")` becomes `LoadScene` with the full
  path. A game that counts scenes in a static needs to know a fresh start from its own
  scene change (`Game.travelling` in Ode to Cactus). All scenes go into Build Settings;
  the GameList entry points at the first.
- **`Collision2D.contacts` is empty in `OnCollisionExit2D`.**
- **Mouse look:** bind `Mouse X`/`Mouse Y` to `<Mouse>/delta` with `scale(factor=0.1)`
  (the old axis sensitivity) and add the right stick in code, scaled by frame time.

## Added by Where Is He (Unity 5.2)

- **`Shader.Find` by an old name** returns null: the built-in shaders moved under
  `Legacy Shaders/`. Grep the scripts for `Shader.Find` in step 0. A material made from
  such a shader in code gets the default grey tint, which needs the Linear correction.
- **Named layers.** `LayerMask.NameToLayer("X")` is -1 unless the collection names that
  layer. Decode the old `TagManager.asset` (tags, then 32 length-prefixed layer names)
  and use the index as a constant; objects keep their layer index through the import.
- **Tags compared as strings** (`tag == "Ground"`) do not throw when the tag does not
  exist; only tags actually set on objects have to be added to the project.
- **`OnValidate` runs on `AddComponent` in play mode in the editor.** One that assumes a
  sibling component needs a null check.
- **Prompts that name ESC** as the way out are now wrong: name the collection's exit.

## Added by Casket Fucker (Unity 5.1)

- **`OnTriggerStay2D` for a collider moved by its transform** (no body of its own) is
  unreliable: rare while it moves, absent once the other body sleeps. A timer counted
  down in it runs many times too slowly. Test the overlap in `Update`
  (`Physics2D.SyncTransforms`, then `Collider2D.Distance(...).isOverlapped`), as
  `MoveToFind.cs` does. Have the bot log how long each state took and compare with
  the time the scene asks for.
- **Steer bots by collider bounds**, not transform positions: sprite pivots and
  collider offsets differ by whole units.
- A product name left at the template's default ("ArtisticGame") is no use; take the
  name from the title art.

## Added by Let's Never Do That Again (Unity 4.6)

- **`Application.LoadLevel(n)` with a counter**: put the old build order (strings in
  `EditorBuildSettings.asset`) in an array of scene names and load by path.
- **Any non-zero axis treated as full input** (`GetAxisRaw(...) != 0`, a vector
  normalised to a fixed speed, `> 0f` as a trigger): give the stick bindings the old
  joystick dead zone, `"processors": "AxisDeadzone(min=0.19)"`, or stick drift plays
  the game.
- **Gravity changed by the game** across its own scenes carries over fine; the
  collection applies the GameList gravity only when the game changes.

## Added by BloodSpace (Unity 4.5)

- **Sorting layers.** `grep m_SortingLayerID` in the re-saved scene and prefabs: any
  value other than 0 is a layer of the old project, and the renderer silently falls
  back to Default. Decode the names, order and unique IDs from the end of the old
  `TagManager.asset` and add them to the project's sorting layers with the same IDs
  (a `SerializedObject` on `ProjectSettings/TagManager.asset`, `m_SortingLayers`).
- **Bare shortcuts** (`x = audio;`, `joint.connectedBody = rigidbody2D;`) are not
  rewritten by `legacy_scripts.py`. They raise the Script Updating Consent dialog; fix
  them by hand and answer No.
- **`Screen.showCursor = false`** with a cursor sprite of the game's own:
  `GlobalInputManager.HideGameCursor()`.
- **`PlayerPrefs`** keys get the game's name as a prefix.
- **A force added every frame without `Time.deltaTime`** scales with the frame rate;
  multiply by `Time.deltaTime * 60f`.
- **Unreferenced assets.** Search the old scene and prefabs for each asset's GUID (in
  the binary files the bytes have their nibbles swapped) and leave out what nothing
  references, apart from `Resources/`.
- A test that writes a high score leaves it in the editor's `PlayerPrefs`; delete the
  keys afterwards.

## Added by Crime Factory (Unity 2019.2, text format, large)

- **Copy by reference closure** when the project is big: map GUID to path from the
  metas, start from the build scenes, the scripts and `Resources/`, and follow
  `guid:` references through every text asset and through the `.meta` of every file
  (model metas name their external materials). Dry-run first and look at the size per
  top folder. Skip a `TextMesh Pro` folder whose GUIDs the collection already has.
- **`PlayerPrefs.DeleteAll()`** must go: prefix the keys and delete them by name.
- **`Application.streamingAssetsPath`**: put the files under
  `Assets/StreamingAssets/<Name>/` and change the paths. Remove anything that writes
  there at quit.
- **Escape with a function of its own** (back to the game's menu): keep it, but ignore
  it while Shift or the pad's Select is held, or it fires together with the
  collection's exit chord.
- **uGUI menus with no first selection** are mouse-only. Add navigation in the menu
  script with the game's own actions rather than relying on `StandaloneInputModule`.
- **Built-in lit materials**: model materials are converted to URP/Lit by the importer;
  list what is left (`Material.shader.name` per `.mat`) and convert those by script
  (`_MainTex`/`_Color`/`_Glossiness` to `_BaseMap`/`_BaseColor`/`_Smoothness`).
- **Broken references that were already broken**: look the GUID up in the old project
  before trying to repair it.
- **Test bots must change levels the way the game does** (here: set the door's
  `nextLevel` and `restartCounter = 0`); calling the loader from an editor callback
  destroyed objects mid-step and threw.

## Added by To Everyone I'll Never Meet (Unity 5.1, first person)

- **Standard Assets scripts** already in a namespace: do not run `legacy_scripts.py`
  over them (it turns parameters named `camera` into `GetComponent<Camera>()`). Rename
  the namespace to the game's, and replace `CrossPlatformInputManager` with
  `TaloketoInputManager` by hand.
- **A `CharacterController` moved through its transform** stays where it was: set
  `Physics.autoSyncTransforms = true` while the game runs. Have the bot log the
  player's position at the start of each level; "always (0, y, 0)" is the sign.
- **`DontDestroyOnLoad` objects** outlive the game. Subscribe once to
  `SceneManager.sceneLoaded` and destroy them when the loaded scene is not the game's;
  restore changed physics settings in the same place.
- **3D gravity** is not covered by the GameList entry (that is 2D): a game that changes
  `Physics.gravity` sets it at start and puts it back on leaving.
- **Bots in first person:** steer with the right stick by the signed angle to the
  target and push the left stick once it is under 25 degrees.

## Added by The Parasite (Unity 5.0, mouse game with a screen effect)

- **GUID clashes** happen between games that grew out of one another. Give the incoming
  asset a new GUID in its meta; for a script, replace the old GUID's bytes in the
  binary scene before Unity sees it (each byte has its two hex digits swapped).
- **`OnPostRender` / `OnRenderImage`** are never called under URP. Render the camera
  into a `RenderTexture` and show it with a full-screen `RawImage` on an overlay canvas
  with a low sorting order; move the per-pixel work into the image's shader. Convert to
  gamma space in the shader when the old code did byte arithmetic. Release the texture
  and destroy the canvas in `OnDestroy`.
- **A prefab with missing scripts** makes `ForceReserializeAssets` log errors. Check
  whether the scripts were already missing in the old project and whether anything
  loads the prefab before deciding to leave it out.
- **Mouse-only games** need no action map: `enableMouseEmulation: 1` in GameList and
  the converter's `TaloketoInputManager.mousePosition` / `GetMouseButton` are enough.
- **Bots on an emulated pointer:** hold the stick until the game reports the wanted
  hover state for a few frames, then tap; do not wait for an exact position when the
  camera moves.

## Added by Troubled Football (Unity 4, mouse game with OnMouseUp buttons)

- **`OnMouseDown` / `OnMouseUp` / `OnMouseOver`** are only sent with the old Input
  Manager. Test the pointer against the object's collider in `Update` and call the
  handler from there (`GameManager.clickedOn` in Troubled Football).
- **`body.velocity` on a `Rigidbody2D` field** is `linearVelocity` in Unity 6; rename it
  by hand before the first import or the script updater asks.
- **Bot button presses must be timed in game time** (hold for 0.15 s), not counted in
  `EditorApplication.update` ticks, which run several times per frame.

## Added by Wall Stains (Unity 5.0, first person, image effects)

- **Standard Assets share GUIDs across projects.** After copying, give every `.meta`
  GUID that already exists elsewhere under `Assets/` a new one and replace it in the
  game's binary scenes and prefabs (nibble-swapped bytes) and text files, before Unity
  sees the folder. Reuse the already ported controller scripts under the new GUIDs.
- **`legacy_scripts.py` rewrites `other.collider`** on a `Collision` parameter to
  `other.GetComponent<Collider>()`, which does not compile; put it back.
- **Standard Assets image effects** (`Bloom`, `NoiseAndGrain`, ...): replace the script
  in place with a small component that keeps the serialized field names and builds a
  URP `Volume` at start; switch `renderPostProcessing` on for the camera.
- **`Legacy Shaders/Diffuse`** has no URP pass either: URP/Lit with smoothness 0 and
  highlights off. Save each material with `AssetDatabase.SaveAssetIfDirty` right away;
  a conversion followed by opening a scene was lost once.
- **Check the first screenshot for magenta** before judging anything else.

## Added by Childhood Nightmare (Unity 4, two scenes, first person)

- **A script reused from an earlier port may carry that game's own changes.** Diff the
  two legacy originals first (Ode to Cactus's `MouseLook` clamps the turn; the stock
  one does not). A bot that holds the stick for more than one full turn shows it.
- **`legacy_scripts.py` turns `Application.LoadLevel(0)` into a reload of the active
  scene**, which is wrong when the game has more than one scene: load the first scene
  by path.
- **A renderer on `Default-Diffuse` with no light in the scene** showed albedo times
  ambient; an unlit material in that shade is the closest thing.
