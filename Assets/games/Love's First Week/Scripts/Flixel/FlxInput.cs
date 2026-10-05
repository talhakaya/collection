using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.system.input.Keyboard: FlxG.keys.
	///
	/// The game asks for keys by Flixel's names - FlxG.keys.LEFT, justPressed("ENTER").
	/// On the keyboard those names are the actions in this game's map in CollectionInput,
	/// bound to the keys they always were: arrows and K for Naz, WASD and Space for Talha
	/// when two play, either set for whoever is being controlled when one does.
	///
	/// Gamepads are read here directly rather than through the action map, because who a
	/// pad controls depends on the game: with one player every pad drives the same keys,
	/// with two the first pad is Naz's keys and the second is Talha's. An action binding
	/// cannot tell one pad from another.
	///
	/// Flixel tracked each key as a small state machine fed by key events (2 just pressed,
	/// 1 held, 0 up, -1 just released) and advanced it once per game step, so "just
	/// pressed" lasts exactly one step however many steps a frame takes. That machine is
	/// kept; the key events are synthesised by comparing each key to its state the frame
	/// before.
	/// </summary>
	public class FlxKeyboard
	{
		private class Key
		{
			public string name;
			public bool down;
			public int current;
			public int last;
		}

		private static readonly string[] Names =
		{
			"LEFT", "RIGHT", "UP", "DOWN", "A", "D", "W", "S",
			"SPACE", "K", "ENTER", "R", "E", "F",

			// Not Flixel keys: the port's own, for opening the game's menu and working
			// its buttons without a mouse.
			"ESCAPE", "MENU_UP", "MENU_DOWN", "MENU_SELECT",
		};

		private const float StickThreshold = 0.5f;

		private readonly Dictionary<string, Key> keys = new Dictionary<string, Key>();
		private readonly List<Key> list = new List<Key>();

		/// Not in Flixel: set by the level, so the pads know whose keys they are.
		public bool twoPlayers;

		/// Not in Flixel: while the game's menu is open the game's own keys read as up,
		/// so that choosing a button with the pad does not also walk and jump.
		public bool gameKeysBlocked;

		public FlxKeyboard()
		{
			foreach (string name in Names)
			{
				var key = new Key { name = name };
				keys[name] = key;
				list.Add(key);
			}
		}

		public bool LEFT { get { return keys["LEFT"].down; } }
		public bool RIGHT { get { return keys["RIGHT"].down; } }
		public bool UP { get { return keys["UP"].down; } }
		public bool DOWN { get { return keys["DOWN"].down; } }
		public bool A { get { return keys["A"].down; } }
		public bool D { get { return keys["D"].down; } }
		public bool W { get { return keys["W"].down; } }
		public bool S { get { return keys["S"].down; } }
		public bool SPACE { get { return keys["SPACE"].down; } }
		public bool K { get { return keys["K"].down; } }
		public bool ENTER { get { return keys["ENTER"].down; } }

		public bool pressed(string Key)
		{
			Key key;
			return keys.TryGetValue(Key, out key) && key.down;
		}

		public bool justPressed(string Key)
		{
			Key key;
			return keys.TryGetValue(Key, out key) && key.current == 2;
		}

		public bool justReleased(string Key)
		{
			Key key;
			return keys.TryGetValue(Key, out key) && key.current == -1;
		}

		/// Once per rendered frame: turn changes in the keys into Flixel's key events.
		internal void poll()
		{
			for (int i = 0; i < list.Count; i++)
			{
				Key key = list[i];
				bool menuKey = key.name == "ESCAPE" || key.name.StartsWith("MENU_");
				bool down = false;
				if (menuKey || !gameKeysBlocked)
				{
					InputAction action = TaloketoInputManager.GetAction(key.name);
					down = (action != null && action.IsPressed()) || padDown(key.name);
				}

				if (down && !key.down)
				{
					// handleKeyDown
					key.current = key.current > 0 ? 1 : 2;
					key.down = true;
				}
				else if (!down && key.down)
				{
					// handleKeyUp
					key.current = key.current > 0 ? -1 : 0;
					key.down = false;
				}
			}
		}

		/// <summary>
		/// What the pads add to a key.
		///
		/// One player: d-pad or stick to walk, A to jump (d-pad up too), X to kick, Y to
		/// swap who is controlled. Two players: the same walk, jump and kick, on the first
		/// pad for Naz (the arrow keys and K) and on the second for Talha (WASD and Space).
		/// On any pad: d-pad down or B moves dialogue on, Select restarts the level, Start
		/// opens the menu.
		/// </summary>
		private bool padDown(string name)
		{
			var pads = Gamepad.all;
			for (int p = 0; p < pads.Count; p++)
			{
				Gamepad pad = pads[p];
				bool left = pad.dpad.left.isPressed || pad.leftStick.ReadValue().x < -StickThreshold;
				bool right = pad.dpad.right.isPressed || pad.leftStick.ReadValue().x > StickThreshold;
				bool up = pad.dpad.up.isPressed || pad.leftStick.ReadValue().y > StickThreshold;
				bool down = pad.dpad.down.isPressed || pad.leftStick.ReadValue().y < -StickThreshold;
				bool jump = pad.buttonSouth.isPressed || pad.dpad.up.isPressed;
				bool kick = pad.buttonWest.isPressed;

				// Naz's keys in a two-player game, everyone's in a one-player game.
				bool first = !twoPlayers || p == 0;
				bool second = twoPlayers && p == 1;

				switch (name)
				{
					case "LEFT": if (first && left) return true; break;
					case "RIGHT": if (first && right) return true; break;
					case "UP": if (first && jump) return true; break;
					case "K": if (twoPlayers && first && kick) return true; break;
					case "A": if (second && left) return true; break;
					case "D": if (second && right) return true; break;
					case "W": if (second && jump) return true; break;
					case "SPACE": if ((second || !twoPlayers) && kick) return true; break;
					case "DOWN": if (pad.dpad.down.isPressed || pad.buttonEast.isPressed) return true; break;
					case "ENTER": if (pad.buttonNorth.isPressed) return true; break;
					case "R": if (pad.selectButton.isPressed) return true; break;
					// In the collection: Start was here, for the game's own menu. Start is the
					// collection's pause screen now; the menu is on Backspace and its button.
					case "MENU_UP": if (up) return true; break;
					case "MENU_DOWN": if (down) return true; break;
					case "MENU_SELECT": if (pad.buttonSouth.isPressed || pad.buttonWest.isPressed) return true; break;
				}
			}

			return false;
		}

		/// Once per game step: Input.update().
		internal void update()
		{
			for (int i = 0; i < list.Count; i++)
			{
				Key key = list[i];
				if (key.last == -1 && key.current == -1)
				{
					key.current = 0;
				}
				else if (key.last == 2 && key.current == 2)
				{
					key.current = 1;
				}

				key.last = key.current;
			}
		}

		/// On a change of state. A key still held then counts as freshly pressed on the
		/// next poll - in Flash the same happened on the next key-repeat event.
		internal void reset()
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].down = false;
				list[i].current = 0;
				list[i].last = 0;
			}
		}
	}

	/// <summary>
	/// org.flixel.system.input.Mouse: FlxG.mouse.
	///
	/// Flixel drew its own cursor sprite; show and hide controlled that. The collection
	/// draws the cursor now, so they are passed on to it - and what a game sets there is
	/// undone automatically when the game is left.
	/// </summary>
	public class FlxMouse
	{
		public double x;
		public double y;

		/// Flixel: whether its cursor sprite is showing, which is what makes buttons
		/// respond to the mouse. Here: whether the mouse is in use - it has moved or
		/// clicked since anything last said otherwise - so a mouse left lying over a
		/// button does not hold the highlight while the menu is worked from a pad.
		public bool visible;

		private Vector2 lastPosition;
		private bool positionKnown;
		private int current;
		private int last;
		private bool down;

		public void show(string Graphic = null, double Scale = 1, int XOffset = 0, int YOffset = 0)
		{
			GlobalInputManager.ClearGameCursor();
		}

		public void hide()
		{
			GlobalInputManager.HideGameCursor();
		}

		public void load(string Graphic = null, double Scale = 1, int XOffset = 0, int YOffset = 0)
		{
		}

		public void unload()
		{
		}

		public bool pressed()
		{
			return current > 0;
		}

		public bool justPressed()
		{
			return current == 2;
		}

		public bool justReleased()
		{
			return current == -1;
		}

		internal void poll(Vector2 gamePosition, FlxCamera camera)
		{
			x = gamePosition.x + (camera != null ? camera.scroll.x : 0);
			y = gamePosition.y + (camera != null ? camera.scroll.y : 0);

			if (!positionKnown)
			{
				positionKnown = true;
				lastPosition = gamePosition;
			}
			else if ((gamePosition - lastPosition).sqrMagnitude > 0.01f)
			{
				lastPosition = gamePosition;
				visible = true;
			}

			bool isDown = TaloketoInputManager.GetMouseButton(0);
			if (isDown)
			{
				visible = true;
			}

			if (isDown && !down)
			{
				current = current > 0 ? 1 : 2;
				down = true;
			}
			else if (!isDown && down)
			{
				current = current > 0 ? -1 : 0;
				down = false;
			}
		}

		internal void update()
		{
			if (last == -1 && current == -1)
			{
				current = 0;
			}
			else if (last == 2 && current == 2)
			{
				current = 1;
			}

			last = current;
		}

		internal void reset()
		{
			down = false;
			current = 0;
			last = 0;
		}
	}
}
