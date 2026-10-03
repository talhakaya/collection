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
