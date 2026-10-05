using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Collection.Controls
{
	public enum InputScheme
	{
		KeyboardMouse,
		Gamepad
	}

	public enum GamepadKind
	{
		/// Unknown make: face buttons are drawn as a diamond with the right one blinking.
		Generic,
		Xbox,
		/// PS4 and PS5 pads look the same in prompts.
		PlayStation,
		Switch
	}

	/// <summary>
	/// One glyph of the input sheet, by sprite name. Some are two sprites swapped on a timer:
	/// a face button on an unknown pad (empty diamond / the button lit) and mouse buttons
	/// (plain mouse / the button lit).
	/// </summary>
	public struct InputGlyph
	{
		public string name;
		public string alternate;

		public InputGlyph(string name, string alternate = null)
		{
			this.name = name;
			this.alternate = alternate;
		}

		public bool Animated => alternate != null;
		public string Current => alternate != null && InputPrompts.Blink ? alternate : name;
	}

	/// <summary>
	/// The collection's button prompts: which device the player is using right now, and which
	/// glyph of the shared sheet (Assets/Textures/Input) stands for a control on it. Every
	/// game's prompts go through here, so they all look the same and all follow the device.
	///
	/// A prompt names what it is for rather than a button. In text that is a token in braces:
	///   {Jump}                                    an action of the current game's map
	///   {&lt;Keyboard&gt;/space|&lt;Gamepad&gt;/buttonSouth}   controls, for input that is not an action
	/// Format() turns the tokens into TextMesh Pro sprite tags; InputPromptText does that for
	/// a text and keeps it current, InputPromptSprite is the same for a single sprite. Games
	/// that draw their own text can ask Resolve() for the glyphs and GetSprite() for the art.
	///
	/// GlobalInputManager calls Tick() once a frame; nothing else needs setting up.
	/// </summary>
	public static class InputPrompts
	{
		private const string SpriteAssetPath = "Input/InputGlyphs";
		private const string ShadowSpriteAssetPath = "Input/InputGlyphsShadow";

		// How long each of an animated glyph's two sprites is shown.
		private const float BlinkSeconds = 0.3f;

		// A stick has to be pushed this far to count as the player using the pad. Well above
		// what a worn stick reports at rest, which must not flip the prompts.
		private const float StickThreshold = 0.3f;

		// A key has to be seen down for this many frames in a row before the prompts go back
		// to the keyboard, so a stray key event does not take them away from a pad.
		private const int KeyboardFramesToSwitch = 3;

		private static InputScheme rawScheme = InputScheme.KeyboardMouse;
		private static GamepadKind rawPadKind = GamepadKind.Generic;
		private static InputScheme? forcedScheme;
		private static GamepadKind? forcedPadKind;
		private static Gamepad activePad;
		private static int keyboardFrames;
		private static bool keyboardMouseEverUsed;

		private static TMP_SpriteAsset spriteAsset;
		private static TMP_SpriteAsset shadowSpriteAsset;
		private static Dictionary<string, Sprite> sprites;
		private static Dictionary<string, Sprite> shadowSprites;

		private static readonly List<InputGlyph> scratchGlyphs = new List<InputGlyph>();
		private static readonly List<string> scratchPaths = new List<string>();

		/// What the prompts should show.
		public static InputScheme Scheme => forcedScheme ?? rawScheme;
		public static GamepadKind PadKind => forcedPadKind ?? rawPadKind;

		/// What the player is really holding, whatever the prompts were told to show.
		public static InputScheme RawScheme => rawScheme;

		/// Whether the mouse has genuinely been moved or clicked this session.
		public static bool MouseEverUsed { get; private set; }

		/// Which of an animated glyph's two sprites is up. Unscaled, so prompts keep
		/// animating while a game has time stopped.
		public static bool Blink => (int)(Time.realtimeSinceStartup / BlinkSeconds) % 2 == 1;

		/// Raised when Scheme or PadKind changes.
		public static event Action Changed;

		/// <summary>
		/// Makes the prompts show a given device whatever is plugged in, for looking at them
		/// without owning every pad. Null goes back to following the player.
		/// </summary>
		public static void Force(InputScheme? scheme, GamepadKind? padKind)
		{
			InputScheme oldScheme = Scheme;
			GamepadKind oldKind = PadKind;
			forcedScheme = scheme;
			forcedPadKind = padKind;
			if (oldScheme != Scheme || oldKind != PadKind)
			{
				Changed?.Invoke();
			}
		}

		// ---- Which device ----------------------------------------------------------------

		/// <summary>
		/// A device takes over when the player really uses it - a press, or a stick pushed
		/// well past rest - not when it merely reports something.
		///
		/// The exception is before the keyboard and mouse have been used at all: then a
		/// connected pad wins outright. On a pad-first machine (a Steam Deck, a console-style
		/// setup) that is what makes the prompts, and the cursor, right from the first frame.
		/// </summary>
		public static void Tick()
		{
			Mouse mouse = Mouse.current;
			bool mouseActive = mouse != null && MouseShowsActivity(mouse);
			if (mouseActive)
			{
				MouseEverUsed = true;
			}

			Keyboard keyboard = Keyboard.current;
			keyboardFrames = keyboard != null && keyboard.anyKey.isPressed ? keyboardFrames + 1 : 0;
			bool keyboardActive = keyboardFrames >= KeyboardFramesToSwitch;
			if (mouseActive || keyboardActive)
			{
				keyboardMouseEverUsed = true;
			}

			Gamepad pressedPad = null;
			var pads = Gamepad.all;
			for (int i = 0; i < pads.Count; i++)
			{
				if (PadShowsActivity(pads[i]))
				{
					pressedPad = pads[i];
					break;
				}
			}

			InputScheme scheme = rawScheme;
			if (pressedPad != null)
			{
				scheme = InputScheme.Gamepad;
				activePad = pressedPad;
			}
			else if (pads.Count == 0)
			{
				scheme = InputScheme.KeyboardMouse;
			}
			else if (mouseActive || keyboardActive)
			{
				scheme = InputScheme.KeyboardMouse;
			}
			else if (!keyboardMouseEverUsed)
			{
				scheme = InputScheme.Gamepad;
			}

			if (activePad == null || !activePad.added)
			{
				activePad = Gamepad.current;
			}

			GamepadKind kind = activePad != null ? KindOf(activePad) : rawPadKind;
			if (scheme == rawScheme && kind == rawPadKind)
			{
				return;
			}

			InputScheme oldScheme = Scheme;
			GamepadKind oldKind = PadKind;
			rawScheme = scheme;
			rawPadKind = kind;
			if (oldScheme != Scheme || oldKind != PadKind)
			{
				Changed?.Invoke();
			}
		}

		/// Real use of the mouse this frame - movement, a button or the wheel - as opposed to
		/// the device merely having reported a state.
		private static bool MouseShowsActivity(Mouse mouse)
		{
			return mouse.delta.ReadValue() != Vector2.zero
				|| mouse.scroll.ReadValue() != Vector2.zero
				|| mouse.leftButton.isPressed
				|| mouse.rightButton.isPressed
				|| mouse.middleButton.isPressed;
		}

		private static bool PadShowsActivity(Gamepad pad)
		{
			return pad.buttonSouth.isPressed
				|| pad.buttonEast.isPressed
				|| pad.buttonWest.isPressed
				|| pad.buttonNorth.isPressed
				|| pad.leftShoulder.isPressed
				|| pad.rightShoulder.isPressed
				|| pad.leftTrigger.isPressed
				|| pad.rightTrigger.isPressed
				|| pad.startButton.isPressed
				|| pad.selectButton.isPressed
				|| pad.leftStickButton.isPressed
				|| pad.rightStickButton.isPressed
				|| pad.dpad.ReadValue() != Vector2.zero
				|| pad.leftStick.ReadValue().sqrMagnitude > StickThreshold * StickThreshold
				|| pad.rightStick.ReadValue().sqrMagnitude > StickThreshold * StickThreshold;
		}

		/// On Steam with Steam Input on, every pad arrives as an Xbox one, so that is what
		/// this reports there whatever the player is holding.
		private static GamepadKind KindOf(Gamepad pad)
		{
			if (pad is DualShockGamepad) return GamepadKind.PlayStation;
			if (pad is XInputController) return GamepadKind.Xbox;

			// The Switch pad's class only exists on some platforms, so it is told by name.
			string id = (pad.layout + " " + pad.name + " " + pad.description.product).ToLowerInvariant();
			if (id.Contains("switch") || id.Contains("pro controller") || id.Contains("joy-con")) return GamepadKind.Switch;
			if (id.Contains("dualshock") || id.Contains("dualsense")) return GamepadKind.PlayStation;
			if (id.Contains("xinput") || id.Contains("xbox")) return GamepadKind.Xbox;
			return GamepadKind.Generic;
		}

		// ---- Control to glyph ------------------------------------------------------------

		/// <summary>
		/// The glyph for a control path such as "&lt;Gamepad&gt;/buttonSouth" or
		/// "&lt;Keyboard&gt;/space". False when the sheet has nothing for it.
		///
		/// Pad face buttons go by position: an action on the south button shows B on a Switch
		/// pad, the button that is actually there.
		/// </summary>
		public static bool TryGetGlyph(string controlPath, GamepadKind kind, out InputGlyph glyph)
		{
			glyph = default;
			if (!SplitPath(controlPath, out string device, out string control))
			{
				return false;
			}

			string name = null;
			switch (device)
			{
				case "Gamepad":
					return TryGetGamepadGlyph(control, kind, out glyph);
				case "Keyboard":
					name = KeyboardGlyphName(control);
					break;
				case "Mouse":
					switch (control)
					{
						case "leftButton": glyph = new InputGlyph("mouse", "mouse_left"); return true;
						case "rightButton": glyph = new InputGlyph("mouse", "mouse_right"); return true;
						case "middleButton": name = "mouse_middle"; break;
						default: name = control.StartsWith("scroll") ? "mouse_middle" : "mouse"; break;
					}
					break;
			}

			if (name == null)
			{
				return false;
			}

			glyph = new InputGlyph(name);
			return true;
		}

		private static bool TryGetGamepadGlyph(string control, GamepadKind kind, out InputGlyph glyph)
		{
			glyph = default;
			string name = null;
			switch (control)
			{
				case "buttonSouth": glyph = Face(kind, "south", "pad_a", "ps_cross", "pad_b"); return true;
				case "buttonEast": glyph = Face(kind, "east", "pad_b", "ps_circle", "pad_a"); return true;
				case "buttonWest": glyph = Face(kind, "west", "pad_x", "ps_square", "pad_y"); return true;
				case "buttonNorth": glyph = Face(kind, "north", "pad_y", "ps_triangle", "pad_x"); return true;
				case "leftShoulder": name = ByKind(kind, "xbox_lb", "ps_l1", "switch_l"); break;
				case "rightShoulder": name = ByKind(kind, "xbox_rb", "ps_r1", "switch_r"); break;
				case "leftTrigger": name = ByKind(kind, "xbox_lt", "ps_l2", "switch_zl"); break;
				case "rightTrigger": name = ByKind(kind, "xbox_rt", "ps_r2", "switch_zr"); break;
				case "start": name = kind == GamepadKind.Switch ? "switch_plus" : "pad_start"; break;
				case "select": name = kind == GamepadKind.Switch ? "switch_minus" : "pad_select"; break;
				case "leftStickPress": name = "pad_stick_left_press"; break;
				case "rightStickPress": name = "pad_stick_right_press"; break;
				case "dpad/up": name = "pad_dpad_up"; break;
				case "dpad/down": name = "pad_dpad_down"; break;
				case "dpad/left": name = "pad_dpad_left"; break;
				case "dpad/right": name = "pad_dpad_right"; break;
				default:
					if (control.StartsWith("leftStick")) name = "pad_stick_left";
					else if (control.StartsWith("rightStick")) name = "pad_stick_right";
					else if (control.StartsWith("dpad")) name = "pad_dpad";
					break;
			}

			if (name == null)
			{
				return false;
			}

			glyph = new InputGlyph(name);
			return true;
		}

		private static InputGlyph Face(GamepadKind kind, string position, string xbox, string playStation, string nintendo)
		{
			switch (kind)
			{
				case GamepadKind.Xbox: return new InputGlyph(xbox);
				case GamepadKind.PlayStation: return new InputGlyph(playStation);
				case GamepadKind.Switch: return new InputGlyph(nintendo);
				default: return new InputGlyph("pad_face_empty", "pad_face_" + position);
			}
		}

		private static string ByKind(GamepadKind kind, string xbox, string playStation, string nintendo)
		{
			switch (kind)
			{
				case GamepadKind.PlayStation: return playStation;
				case GamepadKind.Switch: return nintendo;
				default: return xbox;
			}
		}

		private static string KeyboardGlyphName(string control)
		{
			if (control.Length == 1 && control[0] >= 'a' && control[0] <= 'z')
			{
				return "key_" + control;
			}

			switch (control)
			{
				case "space": return "key_space";
				case "enter":
				case "numpadEnter": return "key_enter";
				case "escape": return "key_esc";
				case "tab": return "key_tab";
				case "backspace": return "key_backspace";
				case "pageUp": return "key_pageup";
				case "pageDown": return "key_pagedown";
				case "upArrow": return "key_up";
				case "downArrow": return "key_down";
				case "leftArrow": return "key_left";
				case "rightArrow": return "key_right";
				case "ctrl":
				case "leftCtrl":
				case "rightCtrl": return "key_ctrl";
				case "shift":
				case "leftShift":
				case "rightShift": return "key_shift";
				case "alt":
				case "leftAlt":
				case "rightAlt": return "key_alt";
				default: return null;
			}
		}

		/// "&lt;Gamepad&gt;/dpad/up" into "Gamepad" and "dpad/up".
		private static bool SplitPath(string path, out string device, out string control)
		{
			device = control = null;
			if (string.IsNullOrEmpty(path) || path[0] != '<')
			{
				return false;
			}

			int close = path.IndexOf('>');
			if (close < 0 || close + 2 > path.Length)
			{
				return false;
			}

			device = path.Substring(1, close - 1);
			control = path.Substring(close + 2);
			return true;
		}

		private static bool PathIsFor(string path, InputScheme scheme)
		{
			return scheme == InputScheme.Gamepad
				? path.StartsWith("<Gamepad>")
				: path.StartsWith("<Keyboard>") || path.StartsWith("<Mouse>");
		}

		// ---- Token to glyphs -------------------------------------------------------------

		/// <summary>
		/// The glyphs for a token (see the class summary for what a token is) on the device
		/// the prompts are showing. Appends to results; false when there is nothing to show,
		/// which for an action means the current game has no action of that name.
		///
		/// When the token has nothing for the current device - a keyboard-only action while a
		/// pad is held - it shows what it does have, which is still the truth. The one
		/// translation is the mouse in a game with mouse emulation on: with a pad, a mouse
		/// button shows the pad button that clicks it.
		/// </summary>
		public static bool Resolve(string token, List<InputGlyph> results)
		{
			if (string.IsNullOrEmpty(token))
			{
				return false;
			}

			token = token.Trim();
			if (token.IndexOf('<') >= 0)
			{
				scratchPaths.Clear();
				foreach (string path in token.Split('|'))
				{
					scratchPaths.Add(path.Trim());
				}

				return AddGlyphsForPaths(scratchPaths, results);
			}

			InputAction action = TaloketoInputManager.GetAction(token);
			if (action == null)
			{
				return false;
			}

			// A composite (WASD, a stick's four directions) is one prompt; so is a plain
			// binding. The first that has something for the current device is the one shown.
			var bindings = action.bindings;
			int fallbackStart = -1, fallbackEnd = -1;
			for (int i = 0; i < bindings.Count;)
			{
				int end = i + 1;
				if (bindings[i].isComposite)
				{
					while (end < bindings.Count && bindings[end].isPartOfComposite) end++;
				}

				scratchPaths.Clear();
				bool anyForScheme = false;
				for (int j = i; j < end; j++)
				{
					string path = bindings[j].effectivePath;
					if (bindings[j].isComposite || string.IsNullOrEmpty(path)) continue;
					scratchPaths.Add(path);
					anyForScheme |= PathIsFor(path, Scheme);
				}

				if (anyForScheme)
				{
					return AddGlyphsForPaths(scratchPaths, results);
				}

				if (fallbackStart < 0 && scratchPaths.Count > 0)
				{
					fallbackStart = i;
					fallbackEnd = end;
				}

				i = end;
			}

			if (fallbackStart < 0)
			{
				return false;
			}

			scratchPaths.Clear();
			for (int j = fallbackStart; j < fallbackEnd; j++)
			{
				string path = bindings[j].effectivePath;
				if (!bindings[j].isComposite && !string.IsNullOrEmpty(path)) scratchPaths.Add(path);
			}

			return AddGlyphsForPaths(scratchPaths, results);
		}

		private static bool AddGlyphsForPaths(List<string> paths, List<InputGlyph> results)
		{
			InputScheme scheme = Scheme;
			bool anyForScheme = false;
			for (int i = 0; i < paths.Count; i++)
			{
				anyForScheme |= PathIsFor(paths[i], scheme);
			}

			int before = results.Count;
			int keys = 0;
			bool w = false, a = false, s = false, d = false, up = false, down = false, left = false, right = false;
			int dpadDirections = 0;
			for (int i = 0; i < paths.Count; i++)
			{
				string path = paths[i];
				if (anyForScheme)
				{
					if (!PathIsFor(path, scheme)) continue;
				}
				else if (scheme == InputScheme.Gamepad && GlobalInputManager.MouseEmulationEnabled && path.StartsWith("<Mouse>"))
				{
					path = GlobalInputManager.EmulatingGamepadPath(path) ?? path;
				}

				if (!TryGetGlyph(path, PadKind, out InputGlyph glyph)) continue;

				switch (path)
				{
					case "<Keyboard>/w": w = true; break;
					case "<Keyboard>/a": a = true; break;
					case "<Keyboard>/s": s = true; break;
					case "<Keyboard>/d": d = true; break;
					case "<Keyboard>/upArrow": up = true; break;
					case "<Keyboard>/downArrow": down = true; break;
					case "<Keyboard>/leftArrow": left = true; break;
					case "<Keyboard>/rightArrow": right = true; break;
				}

				if (path.StartsWith("<Keyboard>")) keys++;
				if (path.StartsWith("<Gamepad>/dpad/")) dpadDirections++;

				bool duplicate = false;
				for (int j = before; j < results.Count; j++)
				{
					duplicate |= results[j].name == glyph.name && results[j].alternate == glyph.alternate;
				}

				if (!duplicate) results.Add(glyph);
			}

			// Four keys that make a cluster are one glyph, and so is a whole d-pad.
			int added = results.Count - before;
			if (added == 4 && keys == 4 && w && a && s && d)
			{
				results.RemoveRange(before, added);
				results.Add(new InputGlyph("key_wasd"));
			}
			else if (added == 4 && keys == 4 && up && down && left && right)
			{
				results.RemoveRange(before, added);
				results.Add(new InputGlyph("key_arrows"));
			}
			else if (added == 4 && dpadDirections == 4)
			{
				results.RemoveRange(before, added);
				results.Add(new InputGlyph("pad_dpad"));
			}

			return results.Count > before;
		}

		// ---- Text ------------------------------------------------------------------------

		/// <summary>
		/// The text with every {token} replaced by TextMesh Pro sprite tags for the current
		/// device. A token that resolves to nothing is left as written. The text showing it
		/// needs SpriteAsset() as its sprite asset; InputPromptText sees to that.
		///
		/// animated says whether any glyph is one that swaps sprites, in which case the text
		/// has to be formatted again whenever Blink flips.
		/// </summary>
		public static string Format(string template, out bool animated)
		{
			animated = false;
			if (string.IsNullOrEmpty(template) || template.IndexOf('{') < 0)
			{
				return template;
			}

			var sb = new StringBuilder(template.Length + 32);
			int at = 0;
			while (at < template.Length)
			{
				int open = template.IndexOf('{', at);
				int close = open < 0 ? -1 : template.IndexOf('}', open + 1);
				if (close < 0)
				{
					sb.Append(template, at, template.Length - at);
					break;
				}

				sb.Append(template, at, open - at);
				scratchGlyphs.Clear();
				if (Resolve(template.Substring(open + 1, close - open - 1), scratchGlyphs))
				{
					for (int i = 0; i < scratchGlyphs.Count; i++)
					{
						animated |= scratchGlyphs[i].Animated;
						sb.Append("<sprite name=\"").Append(scratchGlyphs[i].Current).Append("\" tint=1>");
					}
				}
				else
				{
					sb.Append(template, open, close - open + 1);
				}

				at = close + 1;
			}

			return sb.ToString();
		}

		// ---- Art -------------------------------------------------------------------------

		/// <summary>
		/// The glyphs as a TextMesh Pro sprite asset. The plain one is white, to be tinted by
		/// the text's colour; the shadowed one has a black drop shadow under each glyph, for
		/// text over a busy picture. Built from the sheets by Collection > Build Input Glyph
		/// Assets.
		/// </summary>
		public static TMP_SpriteAsset SpriteAsset(bool shadowed = false)
		{
			if (shadowed)
			{
				if (shadowSpriteAsset == null) shadowSpriteAsset = Resources.Load<TMP_SpriteAsset>(ShadowSpriteAssetPath);
				return shadowSpriteAsset;
			}

			if (spriteAsset == null) spriteAsset = Resources.Load<TMP_SpriteAsset>(SpriteAssetPath);
			return spriteAsset;
		}

		/// A glyph's sprite, for anything that is not TextMesh Pro text.
		public static Sprite GetSprite(string glyphName, bool shadowed = false)
		{
			Dictionary<string, Sprite> table = shadowed ? shadowSprites : sprites;
			if (table == null)
			{
				table = new Dictionary<string, Sprite>();
				TMP_SpriteAsset asset = SpriteAsset(shadowed);
				if (asset != null)
				{
					foreach (TMP_SpriteCharacter character in asset.spriteCharacterTable)
					{
						if (character.glyph is TMP_SpriteGlyph glyph && glyph.sprite != null)
						{
							table[character.name] = glyph.sprite;
						}
					}
				}

				if (shadowed) shadowSprites = table;
				else sprites = table;
			}

			table.TryGetValue(glyphName, out Sprite sprite);
			return sprite;
		}
	}
}
