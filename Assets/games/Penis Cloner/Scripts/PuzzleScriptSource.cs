using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Games.PenisCloner
{
	/// One entry of the OBJECTS section: a name, and a 5x5 picture in up to ten colours.
	public class PuzzleScriptObject
	{
		public string name;
		public int id;
		public int layer;
		public Color32[] colors;

		/// 25 colour indices, row by row from the top; -1 is see-through.
		public int[] pixels;
	}

	/// One entry of the LEVELS section: either a message or a grid.
	public class PuzzleScriptLevel
	{
		public string message;
		public int width;
		public int height;

		/// For each tile and collision layer, the id of the object there plus one, or 0.
		/// Tiles are numbered down the columns - tile = x * height + y - as in the engine.
		public int[] objects;
	}

	/// <summary>
	/// Reads the game's PuzzleScript source: the settings at the top, OBJECTS, LEGEND,
	/// COLLISIONLAYERS and LEVELS.
	///
	/// RULES and WINCONDITIONS are not interpreted - the game has one rule and two
	/// conditions, and PenisCloner.cs carries them out by hand. Their text is kept so that
	/// it can check the file still says what it implements.
	///
	/// This covers what Penis Cloner's source uses, not the whole language: no synonyms or
	/// "or" groups in the legend, no sounds, no palettes other than the default.
	/// </summary>
	public class PuzzleScriptSource
	{
		public readonly Dictionary<string, string> metadata = new Dictionary<string, string>();
		public readonly List<PuzzleScriptObject> objects = new List<PuzzleScriptObject>();
		public readonly List<PuzzleScriptLevel> levels = new List<PuzzleScriptLevel>();
		public readonly List<string> rules = new List<string>();
		public readonly List<string> winConditions = new List<string>();
		public int layerCount;
		public Color32 bgcolor = new Color32(0, 0, 0, 255);
		public Color32 fgcolor = new Color32(255, 255, 255, 255);

		private readonly Dictionary<string, PuzzleScriptObject> objectsByName = new Dictionary<string, PuzzleScriptObject>();
		private readonly Dictionary<char, string[]> legend = new Dictionary<char, string[]>();
		private int nextId;

		// The colour names of PuzzleScript's default palette ("arnecolors").
		private static readonly Dictionary<string, string> Palette = new Dictionary<string, string>
		{
			{ "black", "#000000" }, { "white", "#FFFFFF" }, { "grey", "#9d9d9d" }, { "darkgrey", "#697175" },
			{ "lightgrey", "#cccccc" }, { "gray", "#9d9d9d" }, { "darkgray", "#697175" }, { "lightgray", "#cccccc" },
			{ "red", "#be2633" }, { "darkred", "#732930" }, { "lightred", "#e06f8b" }, { "brown", "#a46422" },
			{ "darkbrown", "#493c2b" }, { "lightbrown", "#eeb62f" }, { "orange", "#eb8931" }, { "yellow", "#f7e26b" },
			{ "green", "#44891a" }, { "darkgreen", "#2f484e" }, { "lightgreen", "#a3ce27" }, { "blue", "#1d57f7" },
			{ "lightblue", "#B2DCEF" }, { "darkblue", "#1B2632" }, { "purple", "#342a97" }, { "pink", "#de65e2" },
		};

		public PuzzleScriptObject Find(string name)
		{
			PuzzleScriptObject found;
			objectsByName.TryGetValue(name.ToLowerInvariant(), out found);
			return found;
		}

		public PuzzleScriptSource(string text)
		{
			string[] lines = StripComments(text).Replace("\r", "").Split('\n');
			string section = "";
			var block = new List<string>();
			var layers = new List<string[]>();

			for (int i = 0; i <= lines.Length; i++)
			{
				string line = i < lines.Length ? lines[i].Trim() : "";
				bool divider = line.Length > 0 && line.Trim('=').Length == 0;
				string upper = line.ToUpperInvariant();
				bool header = upper == "OBJECTS" || upper == "LEGEND" || upper == "SOUNDS" || upper == "COLLISIONLAYERS"
					|| upper == "RULES" || upper == "WINCONDITIONS" || upper == "LEVELS";

				// Objects and level grids are blocks of lines, ended by a blank one.
				if (line.Length == 0 || divider || header || (section == "LEVELS" && IsMessage(line)))
				{
					if (block.Count > 0)
					{
						if (section == "OBJECTS")
						{
							AddObject(block);
						}
						else if (section == "LEVELS")
						{
							AddLevel(block, layers);
						}

						block.Clear();
					}

					if (header)
					{
						section = upper;
					}
					else if (section == "LEVELS" && IsMessage(line))
					{
						levels.Add(new PuzzleScriptLevel { message = line.Substring("message".Length).Trim() });
					}

					continue;
				}

				switch (section)
				{
					case "":
						int space = line.IndexOf(' ');
						string key = (space < 0 ? line : line.Substring(0, space)).ToLowerInvariant();
						metadata[key] = space < 0 ? "" : line.Substring(space + 1).Trim();
						break;
					case "OBJECTS":
					case "LEVELS":
						block.Add(line);
						break;
					case "LEGEND":
						AddLegend(line);
						break;
					case "COLLISIONLAYERS":
						string[] names = line.ToLowerInvariant().Split(',');
						for (int n = 0; n < names.Length; n++)
						{
							names[n] = names[n].Trim();
						}

						layers.Add(names);
						AssignLayer(names, layers.Count - 1);
						break;
					case "RULES":
						rules.Add(Normalise(line));
						break;
					case "WINCONDITIONS":
						winConditions.Add(Normalise(line));
						break;
				}
			}

			layerCount = layers.Count;

			string color;
			if (metadata.TryGetValue("background_color", out color))
			{
				bgcolor = ParseColor(color);
			}

			if (metadata.TryGetValue("text_color", out color))
			{
				fgcolor = ParseColor(color);
			}
		}

		private static bool IsMessage(string line)
		{
			return line.StartsWith("message", StringComparison.OrdinalIgnoreCase)
				&& (line.Length == 7 || char.IsWhiteSpace(line[7]));
		}

		/// Lower case, single spaces: how a rule reads once layout is taken out of it.
		private static string Normalise(string line)
		{
			return string.Join(" ", line.ToLowerInvariant().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
		}

		/// PuzzleScript comments are in parentheses, and nest.
		private static string StripComments(string text)
		{
			var result = new StringBuilder(text.Length);
			int depth = 0;
			foreach (char c in text)
			{
				if (c == '(')
				{
					depth++;
				}
				else if (c == ')' && depth > 0)
				{
					depth--;
				}
				else if (depth == 0)
				{
					result.Append(c);
				}
			}

			return result.ToString();
		}

		/// A name, a line of colours, and optionally five rows of five: '.' for see-through
		/// or a digit choosing one of the colours. With no rows the object is a solid tile
		/// of its first colour.
		private void AddObject(List<string> block)
		{
			var obj = new PuzzleScriptObject();
			obj.name = block[0].Split(' ')[0].ToLowerInvariant();

			string[] colorNames = block[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			obj.colors = new Color32[colorNames.Length];
			for (int i = 0; i < colorNames.Length; i++)
			{
				obj.colors[i] = ParseColor(colorNames[i]);
			}

			obj.pixels = new int[25];
			for (int row = 0; row < 5; row++)
			{
				for (int col = 0; col < 5; col++)
				{
					if (block.Count < 7)
					{
						obj.pixels[row * 5 + col] = 0;
						continue;
					}

					char c = block[2 + row][col];
					obj.pixels[row * 5 + col] = c == '.' ? -1 : c - '0';
				}
			}

			obj.layer = -1;
			objects.Add(obj);
			objectsByName[obj.name] = obj;
		}

		/// "P = Player" or "R = Player and Target".
		private void AddLegend(string line)
		{
			int equals = line.IndexOf('=');
			char glyph = line.Substring(0, equals).Trim()[0];
			string[] names = line.Substring(equals + 1).ToLowerInvariant().Split(new[] { " and " }, StringSplitOptions.None);
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = names[i].Trim();
			}

			legend[glyph] = names;
		}

		/// Ids go in the order the collision layers list the objects, bottom layer first -
		/// which is also the order things are drawn in, so a later layer is drawn on top.
		private void AssignLayer(string[] names, int layer)
		{
			foreach (string name in names)
			{
				PuzzleScriptObject obj = Find(name);
				if (obj == null)
				{
					Debug.LogError("PenisCloner: collision layer names an unknown object \"" + name + "\".");
					continue;
				}

				obj.layer = layer;
				obj.id = nextId++;
			}
		}

		private void AddLevel(List<string> block, List<string[]> layers)
		{
			var level = new PuzzleScriptLevel();
			level.width = block[0].Length;
			level.height = block.Count;
			level.objects = new int[level.width * level.height * layers.Count];

			// Every tile has the background, whatever else the legend puts there.
			PuzzleScriptObject background = Find("background");

			for (int y = 0; y < level.height; y++)
			{
				for (int x = 0; x < level.width; x++)
				{
					int tile = x * level.height + y;
					if (background != null)
					{
						level.objects[tile * layers.Count + background.layer] = background.id + 1;
					}

					string[] names;
					if (!legend.TryGetValue(block[y][x], out names))
					{
						Debug.LogError("PenisCloner: level uses a character that is not in the legend: \"" + block[y][x] + "\".");
						continue;
					}

					foreach (string name in names)
					{
						PuzzleScriptObject obj = Find(name);
						level.objects[tile * layers.Count + obj.layer] = obj.id + 1;
					}
				}
			}

			levels.Add(level);
		}

		private static Color32 ParseColor(string text)
		{
			text = text.Trim().ToLowerInvariant();
			string hex;
			if (!Palette.TryGetValue(text, out hex))
			{
				hex = text;
			}

			hex = hex.TrimStart('#');
			if (hex.Length == 3)
			{
				hex = "" + hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
			}

			return new Color32(
				Convert.ToByte(hex.Substring(0, 2), 16),
				Convert.ToByte(hex.Substring(2, 2), 16),
				Convert.ToByte(hex.Substring(4, 2), 16),
				255);
		}
	}
}
