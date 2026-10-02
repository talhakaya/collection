using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.system.input.Keyboard: FlxG.keys.
	///
	/// The game asks for keys by Flixel's names - FlxG.keys.LEFT, justPressed("SPACE") -
	/// and those names are the actions in this game's map in CollectionInput. So the
	/// gamepad is not a second code path: A is simply another binding on UP, the d-pad on
	/// LEFT and RIGHT, X on SPACE.
	///
	/// Flixel tracked each key as a small state machine fed by key events (2 just pressed,
	/// 1 held, 0 up, -1 just released) and advanced it once per game step, so "just
	/// pressed" lasts exactly one step however many steps a frame takes. That machine is
	/// kept; only its input differs - the key events are synthesised by comparing each
	/// action to its state the frame before.
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
			"SPACE", "ENTER", "ESCAPE", "M", "N", "E",
		};

		private readonly Dictionary<string, Key> keys = new Dictionary<string, Key>();
		private readonly List<Key> list = new List<Key>();

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
		public bool ENTER { get { return keys["ENTER"].down; } }
		public bool ESCAPE { get { return keys["ESCAPE"].down; } }

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

		/// Once per rendered frame: turn changes in the actions into Flixel's key events.
		/// A name with no action in the map simply never goes down.
		internal void poll()
		{
			for (int i = 0; i < list.Count; i++)
			{
				Key key = list[i];
				InputAction action = TaloketoInputManager.GetAction(key.name);
				bool down = action != null && action.IsPressed();

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

			bool isDown = TaloketoInputManager.GetMouseButton(0);
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
