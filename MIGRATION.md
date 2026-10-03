# Migration Journal

Notes on what broke and what fixed it, per game, when bringing each one into the
`Assets/games/<Name>` collection layout. Kept so the next import doesn't have to
rediscover the same issues from scratch.

## chocolate

- Wrapped all script classes/enum in `namespace Games.Chocolate`. Script GUIDs were
  left untouched, so scene/prefab component bindings were unaffected.

## golfinity

Three unrelated bugs, found by actually importing and playing the game rather than
just checking that assets existed.

### 1. Missing `Resources` folder / localization crash

The original `golfinity.unitypackage` never included a `Resources` folder for the
game's own content (only `Assets/TextMesh Pro/Resources/...`, which is shared TMP
essentials, not golfinity's). `Local.cs`'s static constructor does:

```csharp
TextAsset textAsset = Resources.Load("localization") as TextAsset;
ParseLocalizationData(textAsset.text); // NRE: textAsset is null
```

Since this runs in a static constructor, the very first thing that touched `Local`
anywhere in the game (including `Game.Awake()`) crashed with a
`NullReferenceException` / `TypeInitializationException`, which is why the logo
scene appeared to "do nothing" and `main.unity` crashed immediately on play.

**Fix:** supplied a `localization.csv` and placed it at
`Assets/games/Golfinity/Resources/localization.csv` (later became `.txt` after a
later reimport - same content, different extension, Unity still imports it fine as
a `TextAsset`).

### 2. Entry scene picked the wrong scene

`golfinity.unitypackage` ships two scenes: `logo.unity` and `main.unity`. `logo.unity`
turned out to not be a real level — it's an isolated editing scene for the intro
popup component, which is also embedded directly inside `main.unity`
(`Canvas/logo`, a `LogoPopup`). The game-import tool's `RegisterScenesUnder`
registered scenes alphabetically, so Build Settings put `logo.unity` before
`main.unity`, and the main menu (which launches the first registered scene per
game folder) launched the inert logo scene instead of the real game.

**Fix:** `RegisterScenesUnder` now sorts a scene literally named `main` first when
present, so real gameplay launches instead of the empty logo scene.

### 3. Coverface font not rendering (text invisible / solid white)

The custom `Coverface SE FS SDF` font (used for most in-game text and level-number
labels) rendered as either fully invisible or a solid white block, depending on
the object - a `NullReferenceException`-free bug that took a long investigation to
pin down since every property inspected (atlas texture, material properties, shader
validity, mesh bounds, blend/surface settings) checked out as individually correct.
Also affected: the `Additive` shine sprite (see #4).

This coincided with Unity's Script/Shader API Updater dialogs (triggered by
importing content authored in an older Unity/TMP version) reserializing the TMP
font asset and its shaders - the underlying font atlas and shader ended up in a
half-upgraded state that looked fine in isolation but didn't render correctly at
runtime.

**Fix** (found empirically, not from a single root cause):
1. Reimported golfinity and its TextMesh Pro fonts fresh.
2. Selected `Coverface SE FS SDF` (the `TMP_Font Asset`) directly and used
   "Update Atlas Texture" to regenerate the atlas in place.
3. On the font's material, reassigned the shader to
   **`TextMeshPro/Distance Field (SDF)`** (it had drifted to a different/stale
   Distance Field shader variant during the reserialization above). This is what
   actually fixed rendering.

### 4. `Additive` shine effect rendering as a solid red/pink block

The "shine" sparkle overlay on stars (`uiStar.prefab` and `mapLevelUiStar.prefab`,
both using `Assets/games/Golfinity/Materials/Additive.mat` /
`Additive.shadergraph`) rendered as a solid red/pink shape instead of blending
additively and invisibly at alpha 0.

Ruled out during investigation: UI vs. SpriteRenderer render-path mismatch (Material
target was correctly `Sprite Unlit`), Blend Mode (checked and changed to
`Additive`), Surface Type / blend-factor floats on the compiled `.mat` (the graph
was never actually re-saved, per the `Additive*` unsaved-changes marker in the
Shader Graph tab), and the node graph itself (traced every edge of the raw
`.shadergraph` JSON - `Color` node is genuinely pure white, wiring to
`Split`/`Combine`/`Multiply`/`SpriteColor` is logically correct). None of it fixed
the visible tint even after saving.

**Fix:** replaced the Shader Graph asset with a small hand-written HLSL shader,
`Assets/games/Golfinity/Materials/AdditiveGlow.shader` (`Custom/AdditiveGlow`) -
samples `_MainTex`, multiplies by vertex color, premultiplies RGB by alpha under a
straight `Blend One One`, so alpha still fades the glow's intensity. Reassigned on
`Additive.mat`. Verified clean (no tint) against both light and dark backdrops in
Play mode before handing off.

## Tooling fixes made along the way

Found by actually running the import tool against real packages (golfinity,
specifically) rather than only synthetic test packages:

- Packages that pull in shared support assets alongside the game (TextMeshPro
  auto-importing `Assets/TextMesh Pro`) broke common-root detection in the import
  tool, which fell back to nearly moving `Assets/games` into a subfolder of
  itself. Fixed detection to look for the one new folder directly under
  `Assets/games`, and added a hard guard refusing any move where the destination
  nests inside the source.
- Build Settings registration ran after the script-namespacing step's
  `AssetDatabase.Refresh()`, which can trigger a recompile/domain reload mid-call
  and silently skip the rest of the import. Reordered so Build Settings is written
  first.

## herbie

The first game brought in from its raw Unity 4.5 project folder rather than a
`.unitypackage`. The procedure is in `legacy unity projects/PORTING.md`, with the tools
it uses in `legacy unity projects/tools/`. What broke:

- Five sprites (the four pacman frames and one star) lost their references: the images
  had been renamed in the old project, which leaves the sprite on file ID 21300002, and
  Unity 6 drops references to that silently. Repointed by `legacy_sprite_ids.py`.
- Interact did nothing most of the time: `GetButtonDown` is read in `OnTriggerStay2D`,
  a physics callback, so a one-frame press usually fell between steps. The press is now
  latched until a physics step has seen it (`Herbie.cs`).
- The fight mini-game relied on the old `Input.GetAxis` smoothing, which the
  collection's raw `GetAxis` does not have; eased locally in `Fight.cs`.
- `TalhaAnimation` and `TalhaColorChanger` could index one past the end from float
  rounding; clamped.
- The binary Input Manager was first read with a strings scan, which dropped the
  one-letter keys (WASD, Z). `legacy_input_axes.py` reads it properly.

## lost shader

Unity 5.3 project folder (Global Game Jam 2016), ported by `PORTING.md`. What broke:

- The echo over the whole screen was nearly gone. It is a camera that does not clear its
  render texture and draws the last frame back over itself through a particle shader
  with a grey tint of 0.5, which the shader doubles. In Linear colour space that tint
  reads as 0.21, so every pass darkened the picture and the trail died in three frames.
  The tint is converted at start (`CameraScript.gammaTint`), for the webcam plane too.
- The player froze for good the first time he faced left: the "has the world grown in"
  test reads `lossyScale.x`, negative on a mirrored transform in Unity 6. Now `Mathf.Abs`.
- The cereal mini-game never closed if the player kept feeding the kid while it slid
  out: `MiniGame.end()` restarted the slide on every call. It now ignores calls while
  ending. (A bug of the original, not of the port.)
- Two project settings the game depended on are set in `Game.Awake` and put back in
  `OnDestroy`: Default and Water layers do not collide (the mini-games sit on Water, on
  top of the world), and `Physics2D.autoSyncTransforms` is on, as it always was in 5.
- `Input.anyKeyDown` became `Game.anyKeyDown`, which also counts the gamepad's buttons.
- The webcam is stopped when the game is left; it used to stop with the application.
- The `Spoon` tag was added to the project. The intro's audio source `state0` has no
  clip, as in the old project. `trees.pdn` (a Paint.NET source file) was not copied.

## ode to cactus

Unity 4.6 project folder ("CloneJamCactus"), seven scenes, ported by `PORTING.md`.
What broke:

- The three lit 3D scenes (road, mondo, norr) drew solid magenta: the built-in
  `Diffuse` shaders do not exist under URP. They are drawn by
  `collection/LegacyDiffuse.shader`, which redoes Unity 4's lighting arithmetic (Lambert
  in gamma values, lights doubled, the old falloff) on light data uploaded by
  `collection/LegacyLighting.cs`, added to each scene by `Game.Awake`. `road.mat` and
  `tutorial.mat` use it, and the 25 renderers in mondo and norr that used the built-in
  `Default-Diffuse` material now use `collection/LegacyDiffuse.mat`. All lights are
  per-pixel now; Unity 4 did four per pixel and the rest per vertex.
- The first-person controller was UnityScript (`CharacterMotor.js`,
  `FPSInputController.js`), which Unity 6 does not compile. Both are translated line
  for line to C# under the same GUIDs and field names, so the scene's tuning survived.
- `Man.cs` threw on leaving a collision (`contacts[0]` of an ended collision is empty in
  Unity 6), which also broke its ground count. Ground colliders are remembered on enter.
- `Application.LoadLevel("name")` became scene paths, and the scene counter, clock and
  score start again whenever a scene is reached other than through `Game.nextLevel`.
- Input: W/Up to jump became `Up` (plus A on a pad), Z/Space became `Fire`, Return became
  `Submit` (Start on a pad). Mouse look kept, with the right stick added in `MouseLook`.
- `Screen.showCursor = false` became `GlobalInputManager.HideGameCursor()`.
- Not copied: the UnityVS plugin, `Thumbs.db`, `.pdn` sources, and the unused parts of
  Standard Assets (third-person controller, prototype character).

## where is he

Unity 5.2 project folder ("Collab2"), one scene, ported by `PORTING.md`. What broke:

- `Shader.Find("Particles/Additive")` finds nothing now (the shader is
  `Legacy Shaders/Particles/Additive`), so every line's `Start` threw. `LineManager`
  asks for the new name, and gives the material the tint that still doubles to 1 in
  Linear colour space, as in Lost Shader.
- The old project's `Ground` layer (index 8) is not named in the collection, so
  `LayerMask.NameToLayer("Ground")` returned -1. The objects keep the index; the three
  linecasts use `Game.GroundMask`.
- `PlatformerController` read `contacts[0]` of an ended collision (empty in Unity 6).
- `TintScript.OnValidate` threw in the editor when `SpriteEffect` added it at run time.
- `Resources.Load` names take the `WhereIsHe/` prefix. The `Bullet` tag was added to the
  project. Statics are reset in `Game.Awake`.
- Input: Z/W/Up became `Jump` (A on a pad), X became `Fire` (X or right trigger),
  Return became `Submit` (Start or A). "Press Enter" reads "Press A" on a pad, and
  "Press ESC to quit" names the collection's exit instead.
- `legacy_scripts.py` took a field named `audio` for the removed component shortcut;
  it now leaves a name alone when the script declares it.

## casket fucker

Unity 5.1 project folder ("ArtisticGame"), one scene of thirteen states, ported by
`PORTING.md`. No custom tags or layers, no `Resources`, no `Shader.Find`. What changed:

- `Input.anyKeyDown` (the whole game is "tap random keys") became `Game.anyKeyDown`:
  every key, the mouse buttons, and the pad's face buttons, shoulders, triggers and d-pad.
- The arrow-key states (`MoveToFind`) counted in `OnTriggerStay2D`. Unity 6 sends it
  only now and then while the object is moved by its transform (cutting the hole took
  half a minute instead of `moveTime` = 3 seconds) and never once the goal's body is
  asleep. `MoveToFind.Update` tests the overlap itself, and only on frames the player
  moves: the knife has to keep moving to count (the owner's call).
- The Escape-quit is gone; `Game.Awake` resets the clock; the four animation index
  calculations are clamped; `TintScript.OnValidate` has its null check.
- `PlatformerController` is in the folder but on no object. It was fixed up to compile
  (`Jump` action, remembered ground colliders) and nothing more.
- Input map: `Horizontal`, `Vertical` (arrows/WASD, left stick, d-pad), `Jump`.
- `UnityVS` and `Thumbs.db` were not copied.

## let's never do that again

Unity 4.6.1 project folder ("LetsNeverDoThatAgain", Nordic Game Jam 2015), five scenes
walked through by a static counter, ported by `PORTING.md`. No custom tags or layers, no
`Resources`, no `Shader.Find`. What changed:

- `Application.LoadLevel(sceneCount)` loaded by build index; `Scene0` now has the five
  scene names in that order and loads by path. `Game.Awake` starts the run again
  (`sceneCount`, clock) unless the scene was reached through `Scene0.nextLevel`.
- The game switches `Physics2D.gravity` off for its last two scenes. Nothing was needed
  for that: the collection sets a game's gravity when the game changes, not per scene.
- The Escape-quit is gone. Enter / keypad Enter still runs the game four times faster,
  on the keyboard only.
- "W to jump" reads "A to jump" while a gamepad is in use, and a `Jump` action
  (gamepad A only) jumps as well as up does.
- Movement is normalised from any non-zero axis, so the stick bindings carry the old
  Input Manager's 0.19 dead zone (`AxisDeadzone`); without it a drifting stick walks the
  penguin at full speed and jumps for him.
- The two animation index calculations are clamped.
- Input map: `Horizontal`, `Vertical` (arrows/WASD, left stick, d-pad), `Jump`.
- `UnityVS` and `Thumbs.db` were not copied.

## over-eating 101

Unity 4.5/4.6 project folder (`overeating-101`, product name and title "Over-Eating
101"), one scene, ported by `PORTING.md`. No custom tags or layers, no `Resources.Load`,
no `Shader.Find`. What changed:

- `Input.GetKeyDown(KeyCode.Return)` (next rule / start the round) became a `Submit`
  action: Enter, and the gamepad's A and Start. The game shows no prompt for it and
  still does not.
- The Escape-quit is gone. `Game.Awake` resets `ruleNo`, `score`, `totalScore` and the
  clock, so the lesson starts at Rule 0 each time the game is opened.
- The prefabs under `Resources/` moved to `Resources/OverEating101/` (they are
  referenced by field, never loaded by name).
- The two animation index calculations are clamped; the stick has the old 0.19 dead zone.
- Input map: `Horizontal` (body), `Vertical` (arms), `Submit`.
- The end screen has no way on, as in the original; the collection's exit leaves it.
- `UnityVS` and `Thumbs.db` were not copied.

## bloodspace

Unity 4.5.2 project folder (product name "BloodSpace", by Kayabros and Amon26, music by
Erdogan Cem Evin), one scene, a shoot-em-up with a mouse mode, ported by `PORTING.md`.
What changed:

- **Sorting layers.** The game draws in three sorting layers (Background, Gameplay,
  Default, in that order). They were added to the project's `TagManager.asset` with the
  old project's unique IDs (648055327 and 3753717039), ahead of Default, so the scene and
  prefabs resolve them unchanged. The tag `Enemy` already existed; the named physics
  layers were not used by any object.
- **Cursor.** `Screen.showCursor = false` became `GlobalInputManager.HideGameCursor()`:
  the game draws its own pointer.
- **Saved settings.** The five `PlayerPrefs` keys (`mouseMode`, `GameMode`, `highscore`,
  `musicOn`, `Won`) are prefixed `BloodSpace.`.
- **Input.** `Horizontal1`/`Vertical1` (the old joystick axes) are the left stick and
  d-pad, with the 0.19 dead zone; `Vertical1` is inverted in the binding because the code
  subtracts it. A is special attack and select, Start and RB select too. `KeyCode.C`
  (game mode) became a `GameMode` action with the gamepad's Y. M (mouse mode) and N
  (music) stay on the keyboard.
- The Escape quit and the F4 fullscreen switch are gone; the logo screen's line reads
  "ENTER to begin" or "A to begin" instead of "F4 for fullscreen, ENTER to begin".
- `EnemyHead` added its pull force once per frame whatever the frame rate; it is scaled
  to what it was at 60 frames a second.
- `Game.Awake` clears the statics a run leaves behind (score, dead octopus count, screen
  shake, `Shield.thereIs`, the weather tint). Prefabs load from `Resources/BloodSpace/`.
- Two bare component shortcuts (`audioSource = audio;`, `connectedBody = rigidbody2D;`)
  were fixed by hand; `legacy_scripts.py` only rewrites the ones used with a dot.
- Not copied: `UnityVS`-style leftovers, `Thumbs.db`, `PNG.rar`, and 37 images and sounds
  that no scene or prefab references (older art, two unused music tracks).

## crime factory

Unity 2019.2 project folder (product name "Crime Factory"), text format, two build
scenes (`menu`, `main`), a platformer with its own physics simulator, 84 level files and
3D backdrops. Ported by `PORTING.md`, with these differences from the Unity 4 games:

- **Only what is referenced was copied.** Starting from the two build scenes, the
  scripts and `Resources/` (without the `mk` variant), GUID references were followed
  through scenes, prefabs, materials and model metas: 378 files, 219 MB of the 290.
  Left out: DOTween (no script uses it), the project's own TextMesh Pro folder (the
  collection's has the same GUIDs), the `mk` dev scene and prefab set, unused asset-store
  packs, Standard Assets. 183 MB of what came in is the seven WAV music tracks.
- **Levels** moved from `StreamingAssets/Levels` to `StreamingAssets/CrimeFactory/Levels`
  (the first game to use StreamingAssets); `temp.txt` was not copied, and the
  `OnApplicationQuit` that wrote it is disabled.
- **Saves.** `PlayerPrefs.DeleteAll()` in "Start From Beginning" would have wiped every
  game's saves. The five keys are prefixed `CrimeFactory.` and deleted one by one.
- **Scenes** load by path. `Application.Quit` (menu Quit, Escape in the menu) returns
  to the collection's menu. Escape or Start in a level goes to the game's menu, unless
  Shift or Select is held (the collection's own exit chord).
- **Menu.** It could only be clicked with the mouse. `Menu.UpdateNavigation` adds
  up/down with a "> " marker and Jump/Enter to press, shown only once a key or the pad is
  used.
- **Controller detection** (`Platformer.isUsingController`, which swaps the tutorial
  signs) read `KeyCode.JoystickButton0-3`; it now compares the last-used device.
- **Materials.** Unity converted the 43 model materials to URP/Lit on import. The six
  others on built-in lit shaders (three Standard, three Legacy Diffuse variants) were
  converted by an editor script. `GrabPassInvert` is on an object that is never switched
  on and was left alone. 66 material slots inside three asset-store prefabs point at
  materials the old project no longer had; the backdrop prefabs override them.
- The tag `Climb` was added to the project.
- Input map: `Horizontal`, `Vertical` (arrows/WASD, stick with the old 0.5 dead zone,
  d-pad), `Jump` (Z, A), `Fire1` (X, B), `Fire2` (C, X), `Back` (R, Y), `FireAxis`
  (right trigger), `Cancel` (Escape, Start). The old Cancel also had joystick button 1,
  the same button as fire; it was not carried over.
- The level editor (L key) is still there, as in the original build.

## to everyone i'll never meet

Unity 5.1 project folder (product folder "StepIntoMyEyes", title on screen "to everyone
I'll never meet"), binary format, one scene reloaded for each of its twelve levels, 3D,
first person. Ported by `PORTING.md`:

- **Only what the scene references was copied** (108 files, 14 MB of 45): the game's own
  scripts, sprites and two songs, and from the Standard Assets the first-person
  controller, its mouse look, three utility classes and four footstep sounds. The rest of
  the Standard Assets (cameras, vehicles, CrossPlatformInput, RollerBall, ThirdPerson)
  and the unused 2D `PlatformerController.cs` stayed behind.
- **Standard Assets scripts** are in the game's own namespaces
  (`Games.ToEveryoneIllNeverMeet.FirstPerson` / `.Utility`) and read
  `TaloketoInputManager` instead of `CrossPlatformInputManager`. `legacy_scripts.py`
  must not be run over them: it rewrites parameters named `camera`.
- **Player placement.** `Game.Awake` puts the player at a random spot through its
  transform, and the last level lifts it the same way. With the collection's
  `Physics.autoSyncTransforms` off the character controller ignored both, so the player
  always started in the middle. The game switches it on and puts it back when another
  scene loads.
- **Music** is a `DontDestroyOnLoad` object. `Game` watches `sceneLoaded` and destroys it
  when the scene is not the game's; the same handler puts gravity back (the last level
  sets it to zero).
- **Fresh start.** `LevelPass.no`, `Game.time` and the drawn sentences are reset in
  `Game.Awake` unless the load is the game's own level change (`Game.travelling`).
- **End.** `Application.Quit()` after the last song is a return to the collection's menu.
  The countdown still reads "quitting in 10 ... goodbye forever".
- **Input:** arrows/WASD, left stick (dead zone 0.19) and d-pad move; mouse and right
  stick look; `Jump` (space, A) and `Run` (left Shift, left stick press, right trigger)
  are bound but the scene has jump speed 0 and run speed equal to walk speed. Movement
  is eased as the old `Input.GetAxis` did (3 per second).
- **Ground material** converted from Standard to URP/Lit; stars stay on
  `Unlit/Transparent`.

## the parasite

Unity 5.0 project folder (product name "TheParasite"), binary format, one scene
(`test.unity`), a conversation played with the mouse. Ported by `PORTING.md`:

- **Only what the scene references was copied** (plus the game's own scripts). Left out:
  `Resources/bg.prefab` (nothing loads it, and three scripts on it were already missing
  in the old project), `png/2.png`, `png/3.png`, the unused `PlatformerController.cs`
  and the UnityVS editor DLLs.
- **GUID clashes with Abused**, which grew out of this project: `RenderGrayScale.cs` and
  the scene had the same GUIDs. Both got new ones; the script's was patched into the
  binary scene before import (same length, nibble-swapped bytes).
- **The screen effect** (`RenderGrayScale`) read the screen back in `OnPostRender`,
  changed every pixel in a C# loop and drew it over the screen. URP never calls
  `OnPostRender`. Now the camera renders into a texture and a full-screen `RawImage`
  on an overlay canvas (sorting order -100, under the game's text) shows it through
  `PassThru.shader`, which does the same arithmetic on gamma-space bytes, including
  the byte wrap-around that makes the garbled colours for ratios below zero.
- **Mouse game:** `enableMouseEmulation` in GameList, no action map. The left stick
  moves the pointer and A clicks. The title still says "Play with mouse, select
  answers".
- **Statics** (`answerId`, the title texts, blinking, talking) are reset in `Game.Awake`;
  the Escape-quit is removed.

## troubled football

Unity 4 project folder (product name "KickVolley", title on screen "TROUBLED FOOTBALL"),
binary format, one scene, a mouse game. Ported by `PORTING.md`:

- Only what the scene references was copied (11 MB of 27); the `.bmp` and `.pdn` sources
  stayed behind.
- **Mouse game:** `enableMouseEmulation` in GameList. The left stick moves the pointer,
  A kicks, B restarts (it is the emulated right click).
- **`OnMouseUp`** is never sent with the new Input System. The four on-screen buttons
  ask `GameManager.clickedOn(gameObject)` in their `Update` (button released while the
  pointer is over their collider) and call their `OnMouseUp` themselves.
- **Keys** became actions: `Music` (M, gamepad Y), `Eyesore` (N, gamepad X), `Restart`
  (Space). The close button returns to the collection's menu; the Escape-quit is gone.
- Statics are reset in `GameManager.Awake`; `Rigidbody2D.velocity` is `linearVelocity`.

## wall stains

Unity 5.0 project folder, binary format, one scene, first person: light seven candles,
the wall falls open, walk out. Ported by `PORTING.md`:

- Only what the scene references was copied (1.2 MB of 37). The first-person controller
  is the one already ported for To Everyone I'll Never Meet, in this game's namespaces,
  with the jump and landing sounds this project had switched on; the input map is a copy
  of that game's (jump on Space / A is needed here, to get onto the table).
- **Shared GUIDs.** The Standard Assets files have the same GUIDs in every project that
  used them. `tools/legacy_reguid.py`
  gave this game's copies new ones and patched them into the binary scene and prefab.
- **Image effects.** `Bloom` and `NoiseAndGrain` worked through `OnRenderImage`, which
  URP never calls. The two scripts are replaced (same files, so the scene's values
  survive) by one that makes a URP volume with bloom and film grain at start. The
  image-effect shaders and the lens flare were not copied.
- **Materials.** Four `Standard` and five `Legacy Shaders/Diffuse` materials are URP/Lit
  now (the diffuse ones with highlights and reflections off); the one object on Unity's
  built-in default material has a `DefaultMat` of the game's own.
- `Application.Quit()` at the end returns to the collection's menu; statics
  (`LightStart.candles`, `Suicide.done`) are reset in `Game.Awake`.

## childhood nightmare

Unity 4 project folder (product name "Childhood Nightmare"), binary format, two scenes
(`first`, `scene1`): a title that blurs away on a click, then a first-person scene that
runs a timed 90-second sequence and returns to the title. Ported by `PORTING.md`:

- Only what the two scenes reference was copied (4.6 MB of 43); iTween was not used by
  any script and stayed behind.
- **Start** is the mouse button, and now also gamepad A or Space.
- **Mouse look** is the Unity 4 Standard Assets script as ported for Ode to Cactus
  (right stick added), under a new GUID, but with this project's own horizontal
  rotation: Ode to Cactus's copy clamps the turn to one full circle, this one did not.
- **Tag** `Ground` is added to the project (the jump resets on it).
- **Ground plane** was on Unity's built-in diffuse material, lit by the ambient colour
  only; it has an unlit grey `GroundMat` now. The shade is my estimate.
- Scene changes are by path; `TintScript.weatherColor` and the RGB-split constant are
  reset when the title starts.

## azer avm

Unity 4 project folder (product name "AzerAVM"), binary format, two scenes (`scene0`
title, `scene1` match): two players bounce on a platform and push each other off.
Ported by `PORTING.md`:

- **Keyboard** as before: A/D and W for the first player, arrows and Up for the second
  (`P1Horizontal`, `P1Jump`, `P2Horizontal`, `P2Jump` in the action map).
- **Gamepads** are read in `PlayerScript.padInput`, not through the map, because the
  map cannot tell two pads apart. Two pads: one each (left stick or d-pad, A). One pad:
  shared, left stick and LB/LT for the first player, right stick and RB/RT for the
  second.
- **Title:** `Input.anyKeyDown` became `PlayerScript.anyKeyDown()` (any key, the mouse
  button, or a face/shoulder/Start button). **Restart** was Return held; it is the
  `Restart` action now (Return, gamepad Start), ignored while the exit chord is held.
- **Bounce force** was added every frame without the frame time; it is scaled to 60
  frames a second. `OnCollisionExit2D` no longer reads contact points (Unity 6 gives
  none there).
- Scene changes are by path; the Escape-quit is gone.

## talha's screensaver

Unity 5.3 project folder `screensaver` (no product name set; the name is the owner's),
binary format, one scene, no input: a toy that feeds the webcam and a few particles
through a camera that never clears its render texture. A sibling of Lost Shader.
Ported by `PORTING.md`:

- **Tints** set from code (`_TintColor` on `Particles/Additive`) go through
  `Game.gammaTint`, for the Linear colour space reason described under Lost Shader.
- **The webcam** is stopped and released in `WebcamOnMaterial.OnDestroy`; it used to
  stop with the application.
- The Escape-quit is gone; `Game.time` starts over in `Awake`. No action map, no mouse
  emulation: there is nothing to press.

## hello fractals

Unity 4 project folder `hello fractals` (product name "HelloFractals"), binary format,
one scene: a 160 by 90 grid of sprites coloured by a random quadratic in x, y and time.
Ported by `PORTING.md`:

- **Keys became actions**, each with a gamepad button: `New` (Space, A; the mouse
  button still works too), `Fast` (Return, right trigger, held), `BlackWhite` (left
  Ctrl, Y), and the held function keys `FunctionQ` (Q, X), `FunctionW` (W, B),
  `FunctionE` (E), `FunctionR` (R, LB), `FunctionT` (T, RB).
- **The function keys are read once per frame** in `Game.Update`. Each of the 14400
  rectangles used to ask for five keys itself every frame.
- Statics (`time`, `trigon`, `blackWhite`) start over in `Game.Awake`; the Escape-quit
  is gone.

## incredible penis

Unity 5.0 project folder `incredible-penis` (product name "IncrediblePenis"), binary
format, ten build scenes: six full-screen videos alternating with four first-person
walks over a terrain, where the score is the distance walked. Ported by `PORTING.md`:

- **Videos.** `MovieTexture` no longer exists. `VideoPlay` is rewritten around
  `VideoPlayer` (material override on the same renderer, sound through the same
  `AudioSource`); the `.mp4` files import as `VideoClip`s, and the scenes' references
  were repointed in the text scenes (file ID 15200000 to 32900000, same GUIDs).
- **Scene order** was `LoadLevel(loadedLevel + 1)` over the build order; it is the
  `Levels.order` array now, loaded by path. After the last video the game used to quit;
  it returns to the collection's menu.
- **First-person controller** is the one ported for Wall Stains (walk, run on Shift /
  right trigger, jump on Space / A, right stick to look), under new GUIDs.
- The I+O+P score cheat stays on the keyboard. `YellowMaterial` and the objects on
  Unity's default material are URP/Lit. The terrain has no texture layers; URP draws it
  as a red checkerboard.
- 83 MB of the folder is the game's own video and audio.

## milky mike

Unity 5.6 project folder `kayaprot2` (product name "Kayabros Prototype 2"; the owner's
name for it is Milky Mike), binary format, one scene and 24 level prefabs: a stealth
platformer aimed with the mouse. Ported by `PORTING.md`:

- **Everything was copied** (7.7 MB) except three unused scripts: `UDPManager.cs`
  (opens UDP sockets), `Rotate3D.cs` and `SpriteButton.cs`. `Carryable.cs` never
  compiled against this version of `Level` and was removed with the one unused prefab
  that used it (`moneyCase`).
- **First level.** `Level0.prefab` and `Level0Mama.prefab` had the same GUID in the old
  project. The newer one (`Level0Mama`) is kept under that GUID; the older file is gone.
- **Aiming.** The game aims at the mouse. On a gamepad the right stick aims: the aim
  point sits three units from the player in the stick's direction until the mouse moves.
- **Actions:** move (arrows/WASD, left stick, d-pad), `Fire` (Space, left mouse, right
  trigger, X), `Fire2` (left Alt, right mouse, left trigger, B), `Interact` (Space, A),
  `WeaponSelect` (Q/E, LB/RB), `Restart` (R, Y), mouse wheel zoom. The number keys and
  the O/P level-skip keys stay on the keyboard. The Escape-quit is gone.
- **`legacy_scripts.py` is for Unity 4 and early 5 code.** Here it turned the valid
  `hit.collider` of raycast hits into `GetComponent<Collider>()`; that was put back.
  Unity's own script updater handled `velocity` (answered "just for these files").
- Tags `Bug` and `Drone` were added to the project (`Enemy`, `Wall` and `Door` existed).

## 120 pixels

Unity 4.5 project folder `120-pixels` (product name "ColorRGBA"), one scene: circles
grow, a grid of 180 tiles lights up, and a figure can be pushed around it. A sketch with
no goal. Ported with `tools/legacy_port.py`:

- Namespace `Games.Pixels120` (a namespace cannot start with a digit); the folder, map
  and GameList entry are "120 Pixels", Resources under `Resources/120Pixels/`.
- `Fast` (Space, gamepad A or right trigger) is the old "hold Space for ten times speed".
- `Circle`'s static counter is reset from `Game.Awake`.
