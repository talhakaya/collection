using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Games.PenisCloner
{
	/// <summary>
	/// Penis Cloner: a PuzzleScript game, run from its original source text.
	///
	/// The game was a single HTML page - PuzzleScript's engine plus the game's source. The
	/// source is kept as it was (Resources/PenisCloner/source.txt) and read at start: the
	/// objects and their pictures, the legend, the collision layers, the levels and the
	/// messages all come from it. What is not read from it is the logic - one rule and two
	/// win conditions - which is carried out by hand in runRules and checkWin, doing what
	/// the engine did with them.
	///
	/// The rest follows the engine in the HTML export, with its names: the title screen,
	/// message screens and their short pauses, a turn (start the players moving, run the
	/// rules, move what can move, check for a win), undo and restart, held keys repeating,
	/// and the level reached being remembered.
	///
	/// PuzzleScript draws everything on a grid of cells - the level's tiles, or 34 x 13
	/// characters of text - scaled to the largest whole number of screen pixels that fits.
	/// So does this: the picture is a small texture, one texel per PuzzleScript pixel.
	///
	/// Added for the collection: gamepad controls, and the key hints on screen naming the
	/// pad's buttons instead of the keyboard's while a pad is in use.
	/// </summary>
	public class PenisCloner : MonoBehaviour
	{
		private const string SourceResourcePath = "PenisCloner/source";
		private const string SaveKey = "PenisCloner.curlevel";

		// The logic below is written for exactly these, as they appear in the source.
		private const string TheRule = "[ > player | create | ] -> [ player | create | player ]";
		private const string WinCondition1 = "all target on player";
		private const string WinCondition2 = "all player on target";

		// Engine: the keys it listens for, as the directions it gives them.
		private const int KeyUp = 0;
		private const int KeyLeft = 1;
		private const int KeyDown = 2;
		private const int KeyRight = 3;
		private const int KeyAction = 4;
		private const int KeyUndo = 5;
		private const int KeyRestart = 6;
		private const int KeyEscape = 7;

		private static readonly string[] ActionNames = { "UP", "LEFT", "DOWN", "RIGHT", "ACTION", "UNDO", "RESTART", "ESCAPE" };

		// Engine: dirMasksDelta, by input direction.
		private static readonly int[] DeltaX = { 0, -1, 0, 1 };
		private static readonly int[] DeltaY = { -1, 0, 1, 0 };

		#region Engine state

		private PuzzleScriptSource state;
		private PuzzleScriptObject player;
		private PuzzleScriptObject create;
		private PuzzleScriptObject target;

		private int curlevel;
		private int levelWidth;
		private int levelHeight;
		private int[] levelObjects;

		/// For each tile and layer: 0, or the input direction plus one that the object
		/// there is trying to move in.
		private int[] movements;

		private readonly List<int[]> backups = new List<int[]>();
		private int[] restartTarget;

		private bool textMode = true;
		private bool titleScreen = true;
		private int titleMode;
		private int titleSelection;
		private bool titleSelected;
		private bool quittingTitleScreen;
		private bool quittingMessageScreen;
		private bool messageselected;
		private bool winning;
		private string[] titleImage;

		private const double deltatime = 17;
		private const double repeatinterval = 150;
		private double timer;
		private double keyRepeatTimer;
		private int keyRepeatIndex;
		private readonly List<int> keybuffer = new List<int>();

		#endregion

		private const int titleWidth = 34;
		private const int titleHeight = 13;

		private static readonly string[] messagecontainer_template =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..........X to continue...........",
			"..................................",
			"..................................",
		};

		private static readonly string[] titletemplate_firstgo =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..........#.start game.#..........",
			"..................................",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		private static readonly string[] titletemplate_select0 =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"...........#.new game.#...........",
			"..................................",
			".............continue.............",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		private static readonly string[] titletemplate_select1 =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			".............new game.............",
			"..................................",
			"...........#.continue.#...........",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		private static readonly string[] titletemplate_firstgo_selected =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"###########.start game.###########",
			"..................................",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		private static readonly string[] titletemplate_select0_selected =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"############.new game.############",
			"..................................",
			".............continue.............",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		private static readonly string[] titletemplate_select1_selected =
		{
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			"..................................",
			".............new game.............",
			"..................................",
			"############.continue.############",
			"..................................",
			".arrow keys to move...............",
			".X to action......................",
			".Z to undo, R to restart..........",
			"..................................",
		};

		// In the collection: the hint lines named their keys in the engine's own letters
		// ("arrow keys to move", "X to action"). The keys are the collection's glyphs now,
		// for the device in use, drawn over the picture where the words stood (the picture
		// itself is far too small for them); these are the lines with room left for them.
		private const string MoveHint = "....to move.......................";
		private const string ActionHint = "....to action.....................";
		private const string UndoHint = "....to undo,....to restart........";
		private const string ContinueHint = "............to continue...........";
		private const string BlankLine = "..................................";
		private const string MoveToken = "{<Keyboard>/upArrow|<Keyboard>/downArrow|<Keyboard>/leftArrow|<Keyboard>/rightArrow|<Gamepad>/dpad}";

		// Where each glyph goes, in character cells from the top-left of the text screen.
		private struct PromptSpot
		{
			public float column;
			public float row;
			public string token;
			public float keyHeight;
		}

		private readonly List<PromptSpot> promptSpots = new List<PromptSpot>();
		private readonly List<InputPromptOverlay.Label> promptLabels = new List<InputPromptOverlay.Label>();
		private InputPromptOverlay prompts;

		private void addPromptSpot(float column, float row, string token, float keyHeight = 1.1f)
		{
			promptSpots.Add(new PromptSpot { column = column, row = row, token = token, keyHeight = keyHeight });
		}

		private void drawPrompts()
		{
			prompts.BeginFrame();
			if (textMode)
			{
				for (int i = 0; i < promptSpots.Count; i++)
				{
					if (i >= promptLabels.Count)
					{
						promptLabels.Add(prompts.NewLabel());
					}

					promptLabels[i].Show(promptSpots[i].token, promptSpots[i].column, promptSpots[i].row, promptSpots[i].keyHeight, state.fgcolor, false);
				}
			}

			prompts.EndFrame();
		}

		private bool usingGamepad;
		private readonly bool[] keyDown = new bool[ActionNames.Length];
		private double accumulator;

		private Texture2D picture;
		private RawImage screen;
		private RectTransform screenRect;
		private bool dirty;

		#region Unity

		private void Awake()
		{
			TextAsset source = Resources.Load<TextAsset>(SourceResourcePath);
			state = new PuzzleScriptSource(source.text);
			player = state.Find("player");
			create = state.Find("create");
			target = state.Find("target");

			if (state.rules.Count != 1 || state.rules[0] != TheRule
				|| state.winConditions.Count != 2 || state.winConditions[0] != WinCondition1 || state.winConditions[1] != WinCondition2)
			{
				Debug.LogWarning("PenisCloner: the rules or win conditions in the source have changed. They are not read from it - PenisCloner.cs implements the original ones by hand.");
			}

			buildDisplay();
			GlobalInputManager.HideGameCursor();

			usingGamepad = Gamepad.current != null
				&& (Keyboard.current == null || Gamepad.current.lastUpdateTime > Keyboard.current.lastUpdateTime);

			// Engine: curlevel = localStorage[document.URL], then setGameState(["restart"]).
			curlevel = PlayerPrefs.GetInt(SaveKey, 0);
			if (curlevel < 0 || curlevel >= state.levels.Count)
			{
				curlevel = 0;
			}

			goToTitleScreen();
			titleSelected = false;
			redraw();
		}

		private void Update()
		{
			pollKeys();

			// The engine runs update() from a 17 ms interval timer.
			accumulator += Time.unscaledDeltaTime * 1000.0;
			if (accumulator > 250)
			{
				accumulator = 250;
			}

			while (accumulator >= deltatime)
			{
				accumulator -= deltatime;
				update();
			}

			if (dirty)
			{
				paint();
			}

			canvasResize();
			drawPrompts();
		}

		private void OnDestroy()
		{
			if (picture != null)
			{
				Destroy(picture);
			}
		}

		#endregion

		#region Input

		/// Turns the actions into the engine's key-down and key-up events, and notices
		/// which kind of device they are coming from.
		private void pollKeys()
		{
			for (int key = 0; key < ActionNames.Length; key++)
			{
				InputAction action = TaloketoInputManager.GetAction(ActionNames[key]);
				bool down = action != null && action.IsPressed();
				if (down == keyDown[key])
				{
					continue;
				}

				keyDown[key] = down;
				if (down)
				{
					bool gamepad = action.activeControl != null && action.activeControl.device is Gamepad;
					if (gamepad != usingGamepad)
					{
						usingGamepad = gamepad;
						refreshHints();
					}

					onKeyDown(key);
				}
				else
				{
					onKeyUp(key);
				}
			}
		}

		private void onKeyDown(int keyCode)
		{
			if (keybuffer.IndexOf(keyCode) >= 0)
			{
				return;
			}

			// JavaScript's splice takes a negative index as counted from the end.
			int index = keyRepeatIndex < 0 ? Math.Max(keybuffer.Count + keyRepeatIndex, 0) : Math.Min(keyRepeatIndex, keybuffer.Count);
			keybuffer.Insert(index, keyCode);
			keyRepeatTimer = 0;
			checkKey(keyCode, true);
		}

		private void onKeyUp(int keyCode)
		{
			int index = keybuffer.IndexOf(keyCode);
			if (index >= 0)
			{
				keybuffer.RemoveAt(index);
				if (keyRepeatIndex >= index)
				{
					keyRepeatIndex--;
				}
			}
		}

		private void checkKey(int keyCode, bool justPressed)
		{
			if (winning)
			{
				return;
			}

			int inputdir = -1;
			switch (keyCode)
			{
				case KeyLeft:
				case KeyUp:
				case KeyRight:
				case KeyDown:
				case KeyAction:
					inputdir = keyCode;
					break;
				case KeyUndo:
					if (textMode == false)
					{
						DoUndo();
						redraw();
						return;
					}

					break;
				case KeyRestart:
					if (textMode == false && justPressed)
					{
						DoRestart();
						redraw();
						return;
					}

					break;
				case KeyEscape:
					if (titleScreen == false)
					{
						goToTitleScreen();
						redraw();
						return;
					}

					break;
			}

			if (textMode)
			{
				if (titleScreen)
				{
					if (titleMode == 0)
					{
						if (inputdir == 4 && justPressed)
						{
							if (titleSelected == false)
							{
								titleSelected = true;
								messageselected = false;
								timer = 0;
								quittingTitleScreen = true;
								generateTitleScreen();
								redraw();
							}
						}
					}
					else
					{
						if (inputdir == 4 && justPressed)
						{
							if (titleSelected == false)
							{
								titleSelected = true;
								messageselected = false;
								timer = 0;
								quittingTitleScreen = true;
								generateTitleScreen();
								redraw();
							}
						}
						else if (inputdir == 0 || inputdir == 2)
						{
							titleSelection = 1 - titleSelection;
							generateTitleScreen();
							redraw();
						}
					}
				}
				else
				{
					if (inputdir == 4 && justPressed)
					{
						if (messageselected == false)
						{
							messageselected = true;
							timer = 0;
							quittingMessageScreen = true;
							titleScreen = false;
							drawMessageScreen();
						}
					}
				}
			}
			else
			{
				if (inputdir >= 0)
				{
					if (processInput(inputdir))
					{
						redraw();
					}
				}
			}
		}

		/// The engine's update(): the pauses after choosing something, and key repeat.
		private void update()
		{
			timer += deltatime;

			if (quittingTitleScreen)
			{
				if (timer / 1000 > 0.3)
				{
					quittingTitleScreen = false;
					nextLevel();
				}
			}

			if (quittingMessageScreen)
			{
				if (timer / 1000 > 0.15)
				{
					quittingMessageScreen = false;
					nextLevel();
				}
			}

			if (winning)
			{
				if (timer / 1000 > 0.5)
				{
					winning = false;
					nextLevel();
				}
			}

			if (keybuffer.Count > 0)
			{
				keyRepeatTimer += deltatime;
				double ticklength = repeatinterval / Math.Sqrt(keybuffer.Count);
				if (keyRepeatTimer > ticklength)
				{
					keyRepeatTimer = 0;
					keyRepeatIndex = (keyRepeatIndex + 1) % keybuffer.Count;
					int key = keybuffer[keyRepeatIndex];
					checkKey(key, false);
				}
			}
		}

		#endregion

		#region Screens

		private void generateTitleScreen()
		{
			titleMode = curlevel > 0 ? 1 : 0;

			string title = "PuzzleScript Game";
			string value;
			if (state.metadata.TryGetValue("title", out value))
			{
				title = value;
			}

			string[] template;
			if (titleMode == 0)
			{
				template = titleSelected ? titletemplate_firstgo_selected : titletemplate_firstgo;
			}
			else if (titleSelection == 0)
			{
				template = titleSelected ? titletemplate_select0_selected : titletemplate_select0;
			}
			else
			{
				template = titleSelected ? titletemplate_select1_selected : titletemplate_select1;
			}

			titleImage = (string[])template.Clone();
			// A line apart rather than on three lines running, as the engine had them: the
			// glyphs are taller than its letters and would touch.
			titleImage[8] = MoveHint;
			titleImage[9] = BlankLine;
			titleImage[10] = ActionHint;
			titleImage[11] = BlankLine;
			titleImage[12] = UndoHint;
			promptSpots.Clear();
			addPromptSpot(2f, 8.6f, MoveToken, 1.5f);
			addPromptSpot(2f, 10.6f, "{ACTION}");
			addPromptSpot(2f, 12.6f, "{UNDO}");
			addPromptSpot(14f, 12.6f, "{RESTART}");

			for (int i = 0; i < titleImage.Length; i++)
			{
				titleImage[i] = titleImage[i].Replace('.', ' ');
			}

			int width = titleImage[0].Length;
			List<string> titlelines = wordwrap(title, width);
			for (int i = 0; i < titlelines.Count; i++)
			{
				string titleline = titlelines[i];
				int lmargin = (width - titleline.Length) / 2;
				string row = titleImage[1 + i];
				titleImage[1 + i] = row.Substring(0, lmargin) + titleline + slice(row, lmargin + titleline.Length);
			}

			if (state.metadata.TryGetValue("author", out value))
			{
				string attribution = "by " + value;
				List<string> attributionsplit = wordwrap(attribution, width);
				for (int i = 0; i < attributionsplit.Count; i++)
				{
					string line = attributionsplit[i];
					string row = titleImage[3 + i];
					titleImage[3 + i] = row.Substring(0, width - line.Length - 1) + line + row[row.Length - 1];
				}
			}
		}

		/// The engine's wordwrap: lines of up to width characters, broken at spaces.
		private static List<string> wordwrap(string str, int width)
		{
			var lines = new List<string>();
			string regex = ".{1," + width + "}(\\s|$)|.{" + width + "}|.+$";
			foreach (Match match in Regex.Matches(str, regex))
			{
				lines.Add(match.Value);
			}

			return lines;
		}

		/// JavaScript's slice(start): empty when start is past the end. A wrapped line can be
		/// one character wider than the screen - the space it was broken at - and then
		/// there is nothing left of the row to put after it.
		private static string slice(string str, int start)
		{
			return start >= str.Length ? "" : str.Substring(start);
		}

		private void drawMessageScreen()
		{
			titleMode = 0;
			textMode = true;
			titleImage = (string[])messagecontainer_template.Clone();
			titleImage[10] = ContinueHint;
			promptSpots.Clear();
			if (!quittingMessageScreen)
			{
				addPromptSpot(10.5f, 10.6f, "{ACTION}");
			}

			for (int i = 0; i < titleImage.Length; i++)
			{
				titleImage[i] = titleImage[i].Replace('.', ' ');
			}

			int width = titleImage[0].Length;
			string message = state.levels[curlevel].message.Trim();
			if (usingGamepad)
			{
				// Not in the engine: the game's own messages name the restart key.
				message = message.Replace("R to restart", "Y to restart");
			}

			List<string> splitMessage = wordwrap(message, width);
			for (int i = 0; i < splitMessage.Count; i++)
			{
				string m = splitMessage[i];
				int row = 5 - (splitMessage.Count / 2) + i;
				int lmargin = (width - m.Length) / 2;
				string rowtext = titleImage[row];
				titleImage[row] = rowtext.Substring(0, lmargin) + m + slice(rowtext, lmargin + m.Length);
			}

			if (quittingMessageScreen)
			{
				titleImage[10] = titleImage[9];
			}

			redraw();
		}

		/// The device in use has changed: rewrite the hints on whatever text is showing.
		private void refreshHints()
		{
			if (!textMode)
			{
				return;
			}

			if (titleScreen)
			{
				generateTitleScreen();
				redraw();
			}
			else
			{
				drawMessageScreen();
			}
		}

		private void goToTitleScreen()
		{
			titleScreen = true;
			textMode = true;
			titleSelection = curlevel > 0 ? 1 : 0;
			generateTitleScreen();
		}

		private void nextLevel()
		{
			keybufferClear();
			if (titleScreen)
			{
				if (titleSelection == 0)
				{
					// new game
					curlevel = 0;
				}

				loadLevelFromState(curlevel);
			}
			else
			{
				if (curlevel < state.levels.Count - 1)
				{
					curlevel++;
					textMode = false;
					titleScreen = false;
					quittingMessageScreen = false;
					messageselected = false;
					loadLevelFromState(curlevel);
				}
				else
				{
					curlevel = 0;
					goToTitleScreen();
				}
			}

			// Engine: localStorage[document.URL] = curlevel.
			PlayerPrefs.SetInt(SaveKey, curlevel);
			PlayerPrefs.Save();

			redraw();
		}

		/// Engine: keybuffer = []. In the browser a key still held came straight back with
		/// the keyboard's own auto-repeat, as a fresh press. Here it stays forgotten until it
		/// is let go and pressed again, so a held button does not skip the next message.
		private void keybufferClear()
		{
			keybuffer.Clear();
		}

		private void loadLevelFromState(int levelindex)
		{
			PuzzleScriptLevel leveldat = state.levels[levelindex];
			curlevel = levelindex;

			titleScreen = false;
			titleMode = curlevel > 0 ? 1 : 0;
			titleSelection = curlevel > 0 ? 1 : 0;
			titleSelected = false;

			if (leveldat.message == null)
			{
				titleMode = 0;
				textMode = false;
				levelWidth = leveldat.width;
				levelHeight = leveldat.height;
				levelObjects = (int[])leveldat.objects.Clone();
				movements = new int[levelObjects.Length];
				backups.Clear();
				restartTarget = backupLevel();
			}
			else
			{
				drawMessageScreen();
			}
		}

		#endregion

		#region Turns

		private int[] backupLevel()
		{
			return (int[])levelObjects.Clone();
		}

		private void restoreLevel(int[] lev)
		{
			levelObjects = (int[])lev.Clone();
			Array.Clear(movements, 0, movements.Length);
		}

		private void DoRestart()
		{
			backups.Add(backupLevel());
			restoreLevel(restartTarget);
		}

		private void DoUndo()
		{
			if (backups.Count > 0)
			{
				int[] tobackup = backups[backups.Count - 1];
				restoreLevel(tobackup);
				backups.RemoveAt(backups.Count - 1);
			}
		}

		private int objectAt(int tile, int layer)
		{
			return levelObjects[tile * state.layerCount + layer];
		}

		private bool has(int tile, PuzzleScriptObject obj)
		{
			return levelObjects[tile * state.layerCount + obj.layer] == obj.id + 1;
		}

		/// One turn. Returns whether anything changed; a turn that changes nothing is not
		/// remembered for undo.
		private bool processInput(int dir)
		{
			// The action key gives every player an "action" movement, which no rule here
			// looks for and which moves nothing: the turn would change nothing.
			if (dir == KeyAction)
			{
				checkWin();
				return false;
			}

			int[] bak = backupLevel();

			// startMovement: every player sets off in the direction pressed.
			int tiles = levelWidth * levelHeight;
			for (int tile = 0; tile < tiles; tile++)
			{
				if (has(tile, player))
				{
					movements[tile * state.layerCount + player.layer] = dir + 1;
				}
			}

			runRules(dir);
			resolveMovements();

			bool modified = false;
			for (int i = 0; i < levelObjects.Length; i++)
			{
				if (levelObjects[i] != bak[i])
				{
					backups.Add(bak);
					modified = true;
					break;
				}
			}

			checkWin();
			return modified;
		}

		/// <summary>
		/// The game's one rule:
		///
		///     [ > Player | Create | ] -> [ Player | Create | Player ]
		///
		/// A player moving into a cloning machine, with any cell at all beyond it: the
		/// player stays where it is, and the cell beyond gets a player.
		///
		/// PuzzleScript tries a rule in all four directions, but "> Player" only matches a
		/// player moving along the direction being tried - so only the direction pressed
		/// can match. What the engine does to each of the three cells, as compiled:
		///
		///   1. Player stays, and loses its movement.
		///   2. Unchanged.
		///   3. Whatever was in the player's collision layer is replaced by a player. That
		///      layer is shared with walls, so cloning into a wall removes the wall. If a
		///      player was already there nothing changes - and since the rule says nothing
		///      about movement in this cell, that player keeps the movement it had, and
		///      may walk off leaving no clone behind.
		///
		/// The engine finds every match first and then applies them, and repeats until
		/// nothing changes. Applying a match takes away the movement it matched on, so the
		/// second time round there is nothing left to find.
		/// </summary>
		private void runRules(int dir)
		{
			int dx = DeltaX[dir];
			int dy = DeltaY[dir];
			int layers = state.layerCount;
			var matches = new List<int>();

			bool changed = true;
			while (changed)
			{
				changed = false;
				matches.Clear();

				for (int x = 0; x < levelWidth; x++)
				{
					for (int y = 0; y < levelHeight; y++)
					{
						// All three cells have to be inside the level.
						int x3 = x + 2 * dx;
						int y3 = y + 2 * dy;
						if (x3 < 0 || x3 >= levelWidth || y3 < 0 || y3 >= levelHeight)
						{
							continue;
						}

						int cell1 = x * levelHeight + y;
						int cell2 = (x + dx) * levelHeight + (y + dy);
						if (has(cell1, player) && movements[cell1 * layers + player.layer] == dir + 1 && has(cell2, create))
						{
							matches.Add(cell1);
						}
					}
				}

				foreach (int cell1 in matches)
				{
					int cell3 = cell1 + 2 * (dx * levelHeight + dy);

					movements[cell1 * layers + player.layer] = 0;
					changed = true;

					if (!has(cell3, player))
					{
						levelObjects[cell3 * layers + player.layer] = player.id + 1;
					}
				}
			}
		}

		/// Engine: resolveMovements. Each moving object steps into the next tile if its
		/// collision layer is free there; the sweep repeats until nothing more can move, so
		/// a row of players all moving the same way moves together. Then every movement is
		/// dropped, whether it happened or not.
		private void resolveMovements()
		{
			int layers = state.layerCount;
			int tiles = levelWidth * levelHeight;

			bool moved = true;
			while (moved)
			{
				moved = false;
				for (int tile = 0; tile < tiles; tile++)
				{
					for (int layer = 0; layer < layers; layer++)
					{
						int movement = movements[tile * layers + layer];
						if (movement == 0)
						{
							continue;
						}

						int tx = tile / levelHeight + DeltaX[movement - 1];
						int ty = tile % levelHeight + DeltaY[movement - 1];
						if (tx < 0 || tx >= levelWidth || ty < 0 || ty >= levelHeight)
						{
							continue;
						}

						int targetTile = tx * levelHeight + ty;
						if (objectAt(targetTile, layer) != 0)
						{
							continue;
						}

						levelObjects[targetTile * layers + layer] = levelObjects[tile * layers + layer];
						levelObjects[tile * layers + layer] = 0;
						movements[tile * layers + layer] = 0;
						moved = true;
					}
				}
			}

			Array.Clear(movements, 0, movements.Length);
		}

		/// The game's win conditions: "All Target On Player" and "All Player On Target".
		private void checkWin()
		{
			int tiles = levelWidth * levelHeight;
			for (int tile = 0; tile < tiles; tile++)
			{
				if (has(tile, target) != has(tile, player))
				{
					return;
				}
			}

			// DoWin
			if (winning)
			{
				return;
			}

			winning = true;
			timer = 0;
		}

		#endregion

		#region Drawing

		private void buildDisplay()
		{
			var canvasObject = new GameObject("Screen", typeof(Canvas));
			canvasObject.transform.SetParent(transform, false);
			Canvas canvas = canvasObject.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 0;

			// The page behind the picture: the game's background colour, edge to edge.
			var backdropObject = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
			backdropObject.transform.SetParent(canvasObject.transform, false);
			var backdropRect = (RectTransform)backdropObject.transform;
			backdropRect.anchorMin = Vector2.zero;
			backdropRect.anchorMax = Vector2.one;
			backdropRect.offsetMin = backdropRect.offsetMax = Vector2.zero;
			Image backdrop = backdropObject.GetComponent<Image>();
			backdrop.color = state.bgcolor;
			backdrop.raycastTarget = false;

			var imageObject = new GameObject("Picture", typeof(RectTransform), typeof(RawImage));
			imageObject.transform.SetParent(canvasObject.transform, false);
			screen = imageObject.GetComponent<RawImage>();
			screen.raycastTarget = false;
			screenRect = (RectTransform)imageObject.transform;
			screenRect.anchorMin = screenRect.anchorMax = screenRect.pivot = new Vector2(0f, 1f);
			prompts = new InputPromptOverlay(screenRect, titleWidth, titleHeight);
		}

		private void redraw()
		{
			dirty = true;
		}

		/// <summary>
		/// Engine: canvasResize. A cell is as large as fits the window, rounded down to a
		/// whole number of screen pixels per PuzzleScript pixel - five to a level tile, six
		/// to a character (five and a gap) - and the grid is centred.
		/// </summary>
		private void canvasResize()
		{
			if (picture == null)
			{
				return;
			}

			int screenwidth = textMode ? titleWidth : levelWidth;
			int screenheight = textMode ? titleHeight : levelHeight;
			int w = textMode ? 6 : 5;

			float scaleFactor = screen.canvas != null ? screen.canvas.scaleFactor : 1f;
			int cellwidth = w * (int)((float)Screen.width / screenwidth / w);
			int cellheight = w * (int)((float)Screen.height / screenheight / w);
			int cell = Math.Min(cellwidth, cellheight);

			int xoffset = (Screen.width - cell * screenwidth) / 2;
			int yoffset = (Screen.height - cell * screenheight) / 2;

			var size = new Vector2(cell * screenwidth, cell * screenheight) / scaleFactor;
			var position = new Vector2(xoffset, -yoffset) / scaleFactor;
			if (screenRect.sizeDelta != size)
			{
				screenRect.sizeDelta = size;
			}

			if (screenRect.anchoredPosition != position)
			{
				screenRect.anchoredPosition = position;
			}
		}

		/// Engine: redraw, into a texture of PuzzleScript pixels rather than a canvas.
		private void paint()
		{
			dirty = false;

			int cellSize = textMode ? 6 : 5;
			int columns = textMode ? titleWidth : levelWidth;
			int rows = textMode ? titleHeight : levelHeight;
			int width = columns * cellSize;
			int height = rows * cellSize;

			if (picture == null || picture.width != width || picture.height != height)
			{
				if (picture != null)
				{
					Destroy(picture);
				}

				picture = new Texture2D(width, height, TextureFormat.RGBA32, false);
				picture.filterMode = FilterMode.Point;
				picture.wrapMode = TextureWrapMode.Clamp;
				screen.texture = picture;
			}

			var pixels = new Color32[width * height];
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = state.bgcolor;
			}

			if (textMode)
			{
				for (int j = 0; j < titleHeight; j++)
				{
					for (int i = 0; i < titleWidth; i++)
					{
						string glyph;
						if (!PuzzleScriptFont.Glyphs.TryGetValue(titleImage[j][i], out glyph))
						{
							continue;
						}

						for (int p = 0; p < 25; p++)
						{
							if (glyph[p] == '1')
							{
								plot(pixels, width, height, i * 6 + p % 5, j * 6 + p / 5, state.fgcolor);
							}
						}
					}
				}
			}
			else
			{
				for (int x = 0; x < levelWidth; x++)
				{
					for (int y = 0; y < levelHeight; y++)
					{
						int tile = x * levelHeight + y;

						// In id order, which is layer order: the last layer ends up on top.
						for (int layer = 0; layer < state.layerCount; layer++)
						{
							int id = objectAt(tile, layer) - 1;
							if (id < 0)
							{
								continue;
							}

							PuzzleScriptObject obj = objectById(id);
							for (int p = 0; p < 25; p++)
							{
								int colorIndex = obj.pixels[p];
								if (colorIndex >= 0)
								{
									plot(pixels, width, height, x * 5 + p % 5, y * 5 + p / 5, obj.colors[colorIndex]);
								}
							}
						}
					}
				}
			}

			picture.SetPixels32(pixels);
			picture.Apply();
		}

		/// Textures count rows from the bottom; PuzzleScript counts from the top.
		private static void plot(Color32[] pixels, int width, int height, int x, int y, Color32 color)
		{
			pixels[(height - 1 - y) * width + x] = color;
		}

		private PuzzleScriptObject objectById(int id)
		{
			for (int i = 0; i < state.objects.Count; i++)
			{
				if (state.objects[i].id == id)
				{
					return state.objects[i];
				}
			}

			return null;
		}

		#endregion
	}
}
