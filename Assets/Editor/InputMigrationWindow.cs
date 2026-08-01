using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Collection.EditorTools
{
	/// <summary>
	/// Converts a legacy Input Manager m_Axes export (see nykrig.json for the shape -
	/// a raw SerializedProperty walk, not EditorJsonUtility) into a New Input System
	/// action map, added to a shared .inputactions asset alongside the always-on
	/// "Global" map (see GlobalInputManager). One map per game; this tool never
	/// touches "Global".
	/// </summary>
	public class InputMigrationWindow : EditorWindow
	{
		private const string DefaultAssetPath = "Assets/Resources/Input/CollectionInput.inputactions";
		private const string GlobalMapName = "Global";

		// Old Input Manager's joystick axis indices aren't a portable standard - they're
		// just "whatever slot this physical stick/trigger happened to read at" on whatever
		// platform the project was last authored for. These defaults were reverse-engineered
		// from this project's own asset-pack lineage (see nykrig.json / MIGRATION.md) and are
		// meant to be reviewed/edited per import, not trusted blindly.
		private const string DefaultJoystickAxisMap =
			"X axis=<Gamepad>/leftStick/x\n" +
			"Y axis=<Gamepad>/leftStick/y:invert\n" +
			"4th axis (Joysticks)=<Gamepad>/rightStick/x\n" +
			"5th axis (Joysticks)=<Gamepad>/rightStick/y:invert\n" +
			"6th axis (Joysticks)=<Gamepad>/rightTrigger\n" +
			"10th axis (Joysticks)=<Gamepad>/rightTrigger\n";

		private string jsonPath = "";
		private string gameName = "";
		private string targetAssetPath = DefaultAssetPath;
		private string skipNamePatterns = "Mac";
		private string joystickAxisMapText = DefaultJoystickAxisMap;
		private string statusMessage = "";
		private MessageType statusType = MessageType.None;

		[MenuItem("Tools/Collection/Import Input Manager JSON...")]
		private static void ShowWindow()
		{
			var window = GetWindow<InputMigrationWindow>(true, "Import Input Manager JSON", true);
			window.minSize = new Vector2(480, 480);
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Legacy Input Manager JSON (m_Axes export)", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			jsonPath = EditorGUILayout.TextField(jsonPath);
			if (GUILayout.Button("Browse...", GUILayout.Width(80)))
			{
				string selected = EditorUtility.OpenFilePanel("Select Exported m_Axes JSON", "", "json");
				if (!string.IsNullOrEmpty(selected))
				{
					jsonPath = selected;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Name (Action Map)", EditorStyles.boldLabel);
			gameName = EditorGUILayout.TextField(gameName);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Target Actions Asset", EditorStyles.boldLabel);
			targetAssetPath = EditorGUILayout.TextField(targetAssetPath);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Skip Entries Containing (comma-separated)", EditorStyles.boldLabel);
			skipNamePatterns = EditorGUILayout.TextField(skipNamePatterns);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Joystick Axis -> Control Path Map", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(
				"One per line: '<axis label>=<control path>[:invert]'. Old Input Manager axis " +
				"indices aren't portable across projects/platforms - review before generating.",
				MessageType.Info);
			joystickAxisMapText = EditorGUILayout.TextArea(joystickAxisMapText, GUILayout.Height(110));

			EditorGUILayout.Space();
			string validationError = Validate();
			if (!string.IsNullOrEmpty(validationError))
			{
				EditorGUILayout.HelpBox(validationError, MessageType.Warning);
			}

			GUI.enabled = string.IsNullOrEmpty(validationError);
			if (GUILayout.Button("Generate Action Map", GUILayout.Height(30)))
			{
				Generate();
			}
			GUI.enabled = true;

			if (!string.IsNullOrEmpty(statusMessage))
			{
				EditorGUILayout.HelpBox(statusMessage, statusType);
			}
		}

		private string Validate()
		{
			if (string.IsNullOrEmpty(jsonPath))
			{
				return "Choose the exported JSON file.";
			}

			if (!File.Exists(jsonPath))
			{
				return "JSON file not found.";
			}

			if (string.IsNullOrEmpty(gameName))
			{
				return "Enter a game name for the action map.";
			}

			if (gameName == GlobalMapName)
			{
				return $"'{GlobalMapName}' is reserved for the always-on cross-game map.";
			}

			if (!Regex.IsMatch(gameName, "^[A-Za-z][A-Za-z0-9]*$"))
			{
				return "Game name must start with a letter and contain only letters and digits.";
			}

			if (string.IsNullOrEmpty(targetAssetPath) ||
			    !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), targetAssetPath)))
			{
				return "Target .inputactions asset not found.";
			}

			return null;
		}

		private void Generate()
		{
			var log = new List<string>();
			try
			{
				string jsonText = File.ReadAllText(jsonPath);
				int pos = 0;
				object root = ParseJsonValue(jsonText, ref pos);
				List<AxisEntry> entries = ExtractAxisEntries(root as Dictionary<string, object>);

				string[] skipPatterns = skipNamePatterns.Split(',')
					.Select(s => s.Trim())
					.Where(s => s.Length > 0)
					.ToArray();

				List<ParsedAction> groups = GroupEntries(entries, skipPatterns, log);
				Dictionary<string, (string path, bool invert)> axisMap = ParseAxisMap(joystickAxisMapText);

				string assetFullPath = Path.Combine(Directory.GetCurrentDirectory(), targetAssetPath);
				var asset = InputActionAsset.FromJson(File.ReadAllText(assetFullPath));

				BuildActionMap(asset, gameName, groups, axisMap, log);

				File.WriteAllText(assetFullPath, asset.ToJson());
				AssetDatabase.ImportAsset(targetAssetPath, ImportAssetOptions.ForceUpdate);

				string summary = $"Generated action map '{gameName}' with {groups.Count} action(s) in {targetAssetPath}.";
				statusMessage = log.Count > 0 ? summary + "\n\n" + string.Join("\n", log) : summary;
				statusType = log.Count > 0 ? MessageType.Warning : MessageType.Info;
				Debug.Log($"[InputMigrationWindow] {statusMessage}");
			}
			catch (Exception e)
			{
				statusMessage = $"Failed: {e.Message}";
				statusType = MessageType.Error;
				Debug.LogError($"[InputMigrationWindow] {e}");
			}
		}

		// ---- Legacy axis entry model -------------------------------------------------

		private class AxisEntry
		{
			public string name;
			public string negativeButton;
			public string positiveButton;
			public string altNegativeButton;
			public string altPositiveButton;
			public string type; // "Key or Mouse Button" | "Mouse Movement" | "Joystick Axis"
			public string axis; // e.g. "X axis", "4th axis (Joysticks)"
			public bool invert;
		}

		private enum ActionKind
		{
			Button,
			Axis
		}

		private class ParsedAction
		{
			public string name;
			public ActionKind kind;
			public readonly List<AxisEntry> members = new List<AxisEntry>();
		}

		private static List<AxisEntry> ExtractAxisEntries(Dictionary<string, object> root)
		{
			if (root == null || !root.TryGetValue("children", out object childrenObj) || !(childrenObj is List<object> children) || children.Count == 0)
			{
				throw new Exception("Doesn't look like an m_Axes export (missing top-level 'children').");
			}

			var arrayNode = children[0] as Dictionary<string, object>;
			if (arrayNode == null || !arrayNode.TryGetValue("children", out object arrayChildrenObj) || !(arrayChildrenObj is List<object> arrayChildren))
			{
				throw new Exception("Doesn't look like an m_Axes export (missing 'Array' children).");
			}

			var entries = new List<AxisEntry>();
			foreach (object childObj in arrayChildren)
			{
				if (!(childObj is Dictionary<string, object> child) || GetString(child, "name") != "data")
				{
					continue;
				}

				if (!(child["children"] is List<object> fields))
				{
					continue;
				}

				var map = new Dictionary<string, object>();
				foreach (object fieldObj in fields)
				{
					var field = fieldObj as Dictionary<string, object>;
					if (field == null || !field.TryGetValue("name", out object nameObj))
					{
						continue;
					}

					map[(string)nameObj] = field.TryGetValue("val", out object val) ? val : null;
				}

				entries.Add(new AxisEntry
				{
					name = GetString(map, "m_Name"),
					negativeButton = GetString(map, "negativeButton"),
					positiveButton = GetString(map, "positiveButton"),
					altNegativeButton = GetString(map, "altNegativeButton"),
					altPositiveButton = GetString(map, "altPositiveButton"),
					type = StripEnumPrefix(GetString(map, "type")),
					axis = StripEnumPrefix(GetString(map, "axis")),
					invert = GetBool(map, "invert"),
				});
			}

			return entries;
		}

		private static string GetString(Dictionary<string, object> map, string key)
		{
			return map.TryGetValue(key, out object v) && v is string s ? s : "";
		}

		private static bool GetBool(Dictionary<string, object> map, string key)
		{
			return map.TryGetValue(key, out object v) && v is bool b && b;
		}

		private static string StripEnumPrefix(string value)
		{
			const string prefix = "Enum:";
			return value.StartsWith(prefix) ? value.Substring(prefix.Length) : value;
		}

		// ---- Grouping / classification ------------------------------------------------

		private static List<ParsedAction> GroupEntries(List<AxisEntry> entries, string[] skipPatterns, List<string> log)
		{
			var order = new List<string>();
			var groups = new Dictionary<string, ParsedAction>();

			foreach (AxisEntry e in entries)
			{
				if (skipPatterns.Any(p => e.name.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
				{
					log.Add($"Skipped '{e.name}' (matches a skip pattern).");
					continue;
				}

				if (!groups.TryGetValue(e.name, out ParsedAction group))
				{
					group = new ParsedAction { name = e.name, kind = ClassifyKind(e) };
					groups[e.name] = group;
					order.Add(e.name);
				}

				group.members.Add(e);
			}

			return order.Select(n => groups[n]).ToList();
		}

		private static ActionKind ClassifyKind(AxisEntry e)
		{
			if (e.type == "Mouse Movement" || e.type == "Joystick Axis")
			{
				return ActionKind.Axis;
			}

			// "Key or Mouse Button" covers keyboard keys, mouse buttons AND joystick/gamepad
			// buttons alike in the old Input Manager - the device is encoded in the string
			// value itself (e.g. "joystick button 3"), not the type field.
			bool isTwoKeyAxis = !string.IsNullOrEmpty(e.negativeButton) && !string.IsNullOrEmpty(e.positiveButton);
			return isTwoKeyAxis ? ActionKind.Axis : ActionKind.Button;
		}

		// ---- Action map construction ---------------------------------------------------

		private static void BuildActionMap(InputActionAsset asset, string mapName, List<ParsedAction> groups,
			Dictionary<string, (string path, bool invert)> axisMap, List<string> log)
		{
			InputActionMap existing = asset.FindActionMap(mapName);
			if (existing != null)
			{
				asset.RemoveActionMap(existing);
			}

			InputActionMap map = asset.AddActionMap(mapName);

			foreach (ParsedAction group in groups)
			{
				if (group.kind == ActionKind.Button)
				{
					InputAction action = map.AddAction(group.name, InputActionType.Button);
					foreach (AxisEntry member in group.members)
					{
						AddButtonBinding(action, member.positiveButton, log, group.name);
						AddButtonBinding(action, member.altPositiveButton, log, group.name);
						AddButtonBinding(action, member.negativeButton, log, group.name);
						AddButtonBinding(action, member.altNegativeButton, log, group.name);
					}
				}
				else
				{
					InputAction action = map.AddAction(group.name, InputActionType.Value, expectedControlLayout: "Axis");
					foreach (AxisEntry member in group.members)
					{
						AddAxisMember(action, member, axisMap, log, group.name);
					}
				}
			}
		}

		private static void AddButtonBinding(InputAction action, string legacyName, List<string> log, string actionName)
		{
			if (string.IsNullOrEmpty(legacyName))
			{
				return;
			}

			string path = ResolveButtonPath(legacyName);
			if (path == null)
			{
				log.Add($"'{actionName}': couldn't resolve button '{legacyName}'.");
				return;
			}

			// Duplicate legacy names (e.g. "return" and "enter" both meaning Return) can
			// resolve to the same control path - keep the generated map free of redundant
			// bindings to the same control.
			if (action.bindings.Any(b => b.path == path))
			{
				return;
			}

			action.AddBinding(path);
		}

		private static void AddAxisMember(InputAction action, AxisEntry member,
			Dictionary<string, (string path, bool invert)> axisMap, List<string> log, string actionName)
		{
			if (member.type == "Mouse Movement")
			{
				string path = ResolveMouseAxisPath(actionName, member.axis);
				if (path != null)
				{
					action.AddBinding(path);
				}
				else
				{
					log.Add($"'{actionName}': couldn't resolve mouse axis '{member.axis}'.");
				}

				return;
			}

			if (member.type == "Joystick Axis")
			{
				if (axisMap.TryGetValue(member.axis, out (string path, bool invert) mapped))
				{
					InputActionSetupExtensions.BindingSyntax binding = action.AddBinding(mapped.path);
					if (mapped.invert || member.invert)
					{
						binding.WithProcessor("invert");
					}
				}
				else
				{
					log.Add($"'{actionName}': no mapping for joystick axis '{member.axis}' - add one in the " +
					        "Joystick Axis Map field and re-run.");
				}

				return;
			}

			// "Key or Mouse Button" two-key axis (e.g. Horizontal: left/right, alt a/d).
			if (!string.IsNullOrEmpty(member.negativeButton) && !string.IsNullOrEmpty(member.positiveButton))
			{
				AddAxisComposite(action, member.negativeButton, member.positiveButton, log, actionName);
			}

			if (!string.IsNullOrEmpty(member.altNegativeButton) && !string.IsNullOrEmpty(member.altPositiveButton))
			{
				AddAxisComposite(action, member.altNegativeButton, member.altPositiveButton, log, actionName);
			}
		}

		private static void AddAxisComposite(InputAction action, string negativeLegacy, string positiveLegacy, List<string> log, string actionName)
		{
			string negPath = ResolveButtonPath(negativeLegacy);
			string posPath = ResolveButtonPath(positiveLegacy);
			if (negPath == null || posPath == null)
			{
				log.Add($"'{actionName}': couldn't resolve axis pair '{negativeLegacy}'/'{positiveLegacy}'.");
				return;
			}

			action.AddCompositeBinding("1DAxis")
				.With("Negative", negPath)
				.With("Positive", posPath);
		}

		// ---- Legacy control name -> New Input System path ------------------------------

		private static readonly Dictionary<string, string> KeyboardKeyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{ "left", "leftArrow" }, { "right", "rightArrow" }, { "up", "upArrow" }, { "down", "downArrow" },
			{ "left ctrl", "leftCtrl" }, { "right ctrl", "rightCtrl" },
			{ "left shift", "leftShift" }, { "right shift", "rightShift" },
			{ "left alt", "leftAlt" }, { "right alt", "rightAlt" },
			{ "return", "enter" }, { "enter", "enter" }, { "escape", "escape" }, { "space", "space" },
			{ "backspace", "backspace" }, { "tab", "tab" },
			{ "page up", "pageUp" }, { "page down", "pageDown" },
		};

		// Conventional Xbox-controller-via-old-Input-Manager button ordering on Windows:
		// A,B,X,Y,LB,RB,Back,Start,LeftStickClick,RightStickClick.
		private static readonly string[] GamepadButtonByIndex =
		{
			"buttonSouth", "buttonEast", "buttonWest", "buttonNorth",
			"leftShoulder", "rightShoulder", "select", "start",
			"leftStickPress", "rightStickPress",
		};

		private static string ResolveButtonPath(string legacyName)
		{
			if (string.IsNullOrEmpty(legacyName))
			{
				return null;
			}

			if (legacyName.StartsWith("mouse ", StringComparison.OrdinalIgnoreCase))
			{
				switch (legacyName.Substring("mouse ".Length).Trim())
				{
					case "0": return "<Mouse>/leftButton";
					case "1": return "<Mouse>/rightButton";
					case "2": return "<Mouse>/middleButton";
					default: return null;
				}
			}

			if (legacyName.StartsWith("joystick button ", StringComparison.OrdinalIgnoreCase))
			{
				string n = legacyName.Substring("joystick button ".Length).Trim();
				if (int.TryParse(n, out int index) && index >= 0 && index < GamepadButtonByIndex.Length)
				{
					return "<Gamepad>/" + GamepadButtonByIndex[index];
				}

				return null;
			}

			if (KeyboardKeyMap.TryGetValue(legacyName, out string mapped))
			{
				return "<Keyboard>/" + mapped;
			}

			// Single letters/digits: old Input Manager names them the same as the new
			// system's <Keyboard> control names (a-z, 0-9).
			if (Regex.IsMatch(legacyName, "^[a-zA-Z0-9]$"))
			{
				return "<Keyboard>/" + legacyName.ToLowerInvariant();
			}

			return null;
		}

		private static string ResolveMouseAxisPath(string actionName, string axisLabel)
		{
			if (string.Equals(actionName, "Mouse ScrollWheel", StringComparison.OrdinalIgnoreCase))
			{
				return "<Mouse>/scroll/y";
			}

			switch (axisLabel)
			{
				case "X axis": return "<Mouse>/delta/x";
				case "Y axis": return "<Mouse>/delta/y";
				default: return null;
			}
		}

		private static Dictionary<string, (string path, bool invert)> ParseAxisMap(string text)
		{
			var result = new Dictionary<string, (string, bool)>();
			foreach (string rawLine in text.Split('\n'))
			{
				string line = rawLine.Trim();
				if (line.Length == 0 || !line.Contains("="))
				{
					continue;
				}

				int eq = line.IndexOf('=');
				string label = line.Substring(0, eq).Trim();
				string rest = line.Substring(eq + 1).Trim();

				bool invert = false;
				const string invertSuffix = ":invert";
				if (rest.EndsWith(invertSuffix, StringComparison.OrdinalIgnoreCase))
				{
					invert = true;
					rest = rest.Substring(0, rest.Length - invertSuffix.Length).Trim();
				}

				result[label] = (rest, invert);
			}

			return result;
		}

		// ---- Minimal JSON parser --------------------------------------------------------
		// Unity's JsonUtility can't handle this export's irregular/dynamic schema (it needs
		// fixed C# types), and no JSON library is otherwise available in this project, so
		// this parses into a plain object graph: Dictionary<string,object> for objects,
		// List<object> for arrays, and string/double/bool/null for leaves.

		private static object ParseJsonValue(string s, ref int i)
		{
			SkipWhitespace(s, ref i);
			char c = s[i];
			if (c == '{') return ParseObject(s, ref i);
			if (c == '[') return ParseArray(s, ref i);
			if (c == '"') return ParseString(s, ref i);
			if (c == 't') { i += 4; return true; }
			if (c == 'f') { i += 5; return false; }
			if (c == 'n') { i += 4; return null; }
			return ParseNumber(s, ref i);
		}

		private static Dictionary<string, object> ParseObject(string s, ref int i)
		{
			var dict = new Dictionary<string, object>();
			i++; // {
			SkipWhitespace(s, ref i);
			if (s[i] == '}')
			{
				i++;
				return dict;
			}

			while (true)
			{
				SkipWhitespace(s, ref i);
				string key = ParseString(s, ref i);
				SkipWhitespace(s, ref i);
				i++; // :
				object value = ParseJsonValue(s, ref i);
				dict[key] = value;
				SkipWhitespace(s, ref i);
				if (s[i] == ',')
				{
					i++;
					continue;
				}

				i++; // }
				break;
			}

			return dict;
		}

		private static List<object> ParseArray(string s, ref int i)
		{
			var list = new List<object>();
			i++; // [
			SkipWhitespace(s, ref i);
			if (s[i] == ']')
			{
				i++;
				return list;
			}

			while (true)
			{
				list.Add(ParseJsonValue(s, ref i));
				SkipWhitespace(s, ref i);
				if (s[i] == ',')
				{
					i++;
					continue;
				}

				i++; // ]
				break;
			}

			return list;
		}

		private static string ParseString(string s, ref int i)
		{
			i++; // opening quote
			var sb = new StringBuilder();
			while (s[i] != '"')
			{
				if (s[i] == '\\')
				{
					i++;
					char esc = s[i];
					switch (esc)
					{
						case 'n': sb.Append('\n'); break;
						case 't': sb.Append('\t'); break;
						case 'r': sb.Append('\r'); break;
						case '"': sb.Append('"'); break;
						case '\\': sb.Append('\\'); break;
						case '/': sb.Append('/'); break;
						case 'u':
							string hex = s.Substring(i + 1, 4);
							sb.Append((char)Convert.ToInt32(hex, 16));
							i += 4;
							break;
						default: sb.Append(esc); break;
					}

					i++;
				}
				else
				{
					sb.Append(s[i]);
					i++;
				}
			}

			i++; // closing quote
			return sb.ToString();
		}

		private static double ParseNumber(string s, ref int i)
		{
			int start = i;
			while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '-' || s[i] == '+' || s[i] == '.' || s[i] == 'e' || s[i] == 'E'))
			{
				i++;
			}

			return double.Parse(s.Substring(start, i - start), CultureInfo.InvariantCulture);
		}

		private static void SkipWhitespace(string s, ref int i)
		{
			while (i < s.Length && char.IsWhiteSpace(s[i]))
			{
				i++;
			}
		}
	}
}
