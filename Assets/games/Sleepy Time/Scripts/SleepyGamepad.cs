using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.SleepyTime
{
	/// <summary>
	/// Lets a gamepad reach the buttons, which are otherwise mouse-only.
	///
	/// The rule that keeps this unambiguous: selection is live only on a screen that has
	/// buttons of its own *and* consumes no gameplay input. That means the menu, and the
	/// score table once its buttons exist - never during a song, and never while paused. So
	/// South is either "play" or "confirm", and never both at once.
	///
	/// The persistent Pause and Menu buttons are deliberately unreachable this way: they hang
	/// off GameManager rather than off a screen, so they are never candidates. That is what
	/// leaves gameplay screens with nothing selectable at all. Start pauses, and the
	/// collection's own Select+Start still exits.
	///
	/// Confirming calls the same buttonPressHandler the mouse calls, so there is one path
	/// through every button in the game.
	/// </summary>
	public static class SleepyGamepad
	{
		/// Stick deflection needed to count as a direction.
		private const float StepThreshold = 0.5f;
		private const float StepRepeatDelay = 0.4f;
		private const float StepRepeatRate = 0.18f;

		private static Button selected;
		private static bool usingGamepad;
		private static Vector2 heldDirection;
		private static float nextStepTime;

		private static readonly List<Button> candidates = new List<Button>();

		public static bool UsingGamepad
		{
			get { return usingGamepad; }
		}

		public static Button Selected
		{
			get { return selected; }
		}

		public static void reset()
		{
			selected = null;
			usingGamepad = false;
			heldDirection = Vector2.zero;
			nextStepTime = 0f;
			candidates.Clear();
		}

		public static void update(GameManager game)
		{
			UpdateActiveDevice();

			FlashObject screen = ActiveScreen(game);
			CollectCandidates(screen);

			if (!usingGamepad || candidates.Count == 0)
			{
				selected = null;
				return;
			}

			if (selected == null || !candidates.Contains(selected))
			{
				selected = candidates[0];
			}

			Navigate();

			// The highlight is the mouse's own hover feedback, so a selected button looks
			// exactly like a hovered one.
			selected.mouseOverHandler();

			if (TaloketoInputManager.GetButtonDown("Press"))
			{
				// Held South carries over from the score screen's count-up speed-up, but that
				// is a hold and this is an edge, so releasing and pressing again is required.
				game.buttonPressHandler(selected);
			}
		}

		/// <summary>
		/// Which screen owns the selection right now, or null when nothing is selectable.
		/// The score table wins over the menu because it is the newer screen while the old one
		/// is still sliding away.
		/// </summary>
		private static FlashObject ActiveScreen(GameManager game)
		{
			if (GameManager.paused || game.sceneManager != null)
			{
				return null;
			}

			if (game.scoreTable != null && game.scoreTable.starsCreated)
			{
				return game.scoreTable;
			}

			return game.menu;
		}

		private static void CollectCandidates(FlashObject screen)
		{
			candidates.Clear();
			if (screen == null)
			{
				return;
			}

			for (int i = 0; i < Button.buttons.Count; i++)
			{
				Button button = Button.buttons[i];
				if (button != null && button.Parent == screen)
				{
					candidates.Add(button);
				}
			}
		}

		private static void Navigate()
		{
			Vector2 move = TaloketoInputManager.GetVector2("Navigate");
			Vector2 direction = Vector2.zero;

			// One axis at a time, whichever is pushed further, so a diagonal doesn't move twice.
			if (Mathf.Abs(move.x) >= Mathf.Abs(move.y))
			{
				if (Mathf.Abs(move.x) > StepThreshold)
				{
					direction = new Vector2(Mathf.Sign(move.x), 0f);
				}
			}
			else if (Mathf.Abs(move.y) > StepThreshold)
			{
				direction = new Vector2(0f, Mathf.Sign(move.y));
			}

			if (direction == Vector2.zero)
			{
				heldDirection = Vector2.zero;
				return;
			}

			// A fresh push steps immediately; holding repeats after a delay.
			if (direction != heldDirection)
			{
				heldDirection = direction;
				nextStepTime = Time.unscaledTime + StepRepeatDelay;
				Step(direction);
				return;
			}

			if (Time.unscaledTime >= nextStepTime)
			{
				nextStepTime = Time.unscaledTime + StepRepeatRate;
				Step(direction);
			}
		}

		/// <summary>
		/// Spatial rather than an index walk, because the menu is not a list: the song buttons
		/// sit in a row with Fullscreen and Tutorial above them and Twitter and Soundtrack
		/// below. Picks whatever lies furthest in the pushed direction while staying closest to
		/// straight ahead, which is what makes left/right on the score screen move between
		/// Retry and Next Level without knowing anything about them.
		/// </summary>
		private static void Step(Vector2 direction)
		{
			Button best = null;
			float bestScore = float.MaxValue;

			for (int i = 0; i < candidates.Count; i++)
			{
				Button candidate = candidates[i];
				if (candidate == selected)
				{
					continue;
				}

				// Flash y grows downward, so flip it to make "up" positive here.
				Vector2 delta = new Vector2(candidate.x - selected.x, selected.y - candidate.y);
				float along = Vector2.Dot(delta, direction);
				if (along <= 1f)
				{
					continue;
				}

				float across = Mathf.Abs(delta.x * direction.y - delta.y * direction.x);
				float score = along + across * 2f;
				if (score < bestScore)
				{
					bestScore = score;
					best = candidate;
				}
			}

			if (best != null)
			{
				selected = best;
			}
		}

		/// <summary>
		/// A connected gamepad isn't necessarily the thing driving the game - it might be
		/// sitting there while the player uses the mouse, and a highlight would be misleading.
		/// Compares each device's own timestamp for its last real input, so whichever was
		/// touched most recently wins. Same approach as GolfinityGamepad.
		/// </summary>
		private static void UpdateActiveDevice()
		{
			Gamepad pad = Gamepad.current;
			if (pad == null)
			{
				usingGamepad = false;
				return;
			}

			double pointerTime = 0.0;
			if (Mouse.current != null)
			{
				pointerTime = System.Math.Max(pointerTime, Mouse.current.lastUpdateTime);
			}

			if (Keyboard.current != null)
			{
				pointerTime = System.Math.Max(pointerTime, Keyboard.current.lastUpdateTime);
			}

			if (pad.lastUpdateTime > pointerTime)
			{
				usingGamepad = true;
			}
			else if (pointerTime > pad.lastUpdateTime)
			{
				usingGamepad = false;
			}
			// Equal - neither touched yet this session - keeps whatever it already was.
		}
	}
}
