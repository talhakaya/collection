using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Collection.Controls;
using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
	[InitializeOnLoad]
	public class GameImportWindow : EditorWindow
	{
		private const string GamesRootFolder = "Assets/games";

		// Packages that use TextMeshPro auto-pull "Assets/TextMesh Pro" (TMP Essential
		// Resources) alongside the game's own content. That folder is shared, project-wide
		// infrastructure - it must stay put, not get swept into the game's own folder.
		private static readonly string[] SharedSupportFolders = { "Assets/TextMesh Pro" };

		// Import is asynchronous and, when the package contains scripts, triggers a
		// domain reload before it completes - which wipes any runtime "+=" subscription.
		// Callbacks are instead registered once per domain load via [InitializeOnLoad],
		// and pending state survives the reload in SessionState rather than static fields.
		private const string SessionKeyPendingGameName = "Collection.GameImportWindow.PendingGameName";
		private const string SessionKeyBeforeSnapshot = "Collection.GameImportWindow.BeforeSnapshot";
		private const string SessionKeyPendingPackagePath = "Collection.GameImportWindow.PendingPackagePath";
		private const string SessionKeyPendingInputJsonPath = "Collection.GameImportWindow.PendingInputJsonPath";

		private string packagePath = "";
		private string gameName = "";
		private string inputJsonPathOverride = "";
		private string statusMessage = "";
		private MessageType statusType = MessageType.None;

		static GameImportWindow()
		{
			AssetDatabase.importPackageCompleted += OnImportCompleted;
			AssetDatabase.importPackageFailed += OnImportFailed;
			AssetDatabase.importPackageCancelled += OnImportCancelled;
		}

		[MenuItem("Tools/Collection/Import Game Package...")]
		private static void ShowWindow()
		{
			var window = GetWindow<GameImportWindow>(true, "Import Game Package", true);
			window.minSize = new Vector2(420, 140);
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Unitypackage", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			packagePath = EditorGUILayout.TextField(packagePath);
			if (GUILayout.Button("Browse...", GUILayout.Width(80)))
			{
				string selected = EditorUtility.OpenFilePanel("Select Game Package", "", "unitypackage");
				if (!string.IsNullOrEmpty(selected))
				{
					packagePath = selected;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Game Name", EditorStyles.boldLabel);
			gameName = EditorGUILayout.TextField(gameName);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Input Manager JSON (optional)", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			inputJsonPathOverride = EditorGUILayout.TextField(inputJsonPathOverride);
			if (GUILayout.Button("Browse...", GUILayout.Width(80)))
			{
				string selected = EditorUtility.OpenFilePanel("Select Input Manager JSON", "", "json");
				if (!string.IsNullOrEmpty(selected))
				{
					inputJsonPathOverride = selected;
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.HelpBox(
				"If set (or if a same-named .json sits next to the package), the action map is " +
				"generated and the game's own Input.* call sites are migrated to TaloketoInputManager " +
				"automatically, right after the package import.",
				MessageType.Info);

			EditorGUILayout.Space();

			string validationError = Validate();
			if (!string.IsNullOrEmpty(validationError))
			{
				EditorGUILayout.HelpBox(validationError, MessageType.Warning);
			}

			GUI.enabled = string.IsNullOrEmpty(validationError);
			if (GUILayout.Button("Import", GUILayout.Height(30)))
			{
				BeginImport(packagePath, gameName.Trim(), inputJsonPathOverride);
			}
			GUI.enabled = true;

			if (!string.IsNullOrEmpty(statusMessage))
			{
				EditorGUILayout.HelpBox(statusMessage, statusType);
			}
		}

		private string Validate()
		{
			if (string.IsNullOrEmpty(packagePath))
			{
				return "Choose a .unitypackage file.";
			}

			if (!File.Exists(packagePath))
			{
				return "Package file not found.";
			}

			string trimmedName = gameName.Trim();
			if (string.IsNullOrEmpty(trimmedName))
			{
				return "Enter a game name.";
			}

			if (!Regex.IsMatch(trimmedName, "^[A-Za-z][A-Za-z0-9]*( [A-Za-z0-9]+)*$"))
			{
				return "Game name must start with a letter, contain only letters/digits, and use single spaces between words.";
			}

			if (AssetDatabase.IsValidFolder($"{GamesRootFolder}/{trimmedName}"))
			{
				return $"'{GamesRootFolder}/{trimmedName}' already exists.";
			}

			if (!string.IsNullOrEmpty(inputJsonPathOverride) && !File.Exists(inputJsonPathOverride))
			{
				return "Input Manager JSON path doesn't exist.";
			}

			return null;
		}

		private void BeginImport(string path, string name, string jsonPathOverride)
		{
			SessionState.SetString(SessionKeyPendingGameName, name);
			SessionState.SetString(SessionKeyBeforeSnapshot, string.Join("\n", SnapshotAssetPaths()));
			SessionState.SetString(SessionKeyPendingPackagePath, path);
			SessionState.SetString(SessionKeyPendingInputJsonPath, jsonPathOverride ?? "");

			AssetDatabase.ImportPackage(path, false);
			Close();
		}

		private static void OnImportCompleted(string packageName)
		{
			string pendingGameName = SessionState.GetString(SessionKeyPendingGameName, "");
			if (string.IsNullOrEmpty(pendingGameName))
			{
				return;
			}

			(HashSet<string> before, string packagePath, string jsonPathOverride) = LoadAndClearPendingState();

			try
			{
				FinishImport(pendingGameName, before, packagePath, jsonPathOverride);
			}
			catch (Exception e)
			{
				Debug.LogError($"Game import failed: {e}");
				EditorUtility.DisplayDialog("Import Game", $"Import failed:\n{e.Message}", "OK");
			}
		}

		private static void OnImportFailed(string packageName, string errorMessage)
		{
			if (string.IsNullOrEmpty(SessionState.GetString(SessionKeyPendingGameName, "")))
			{
				return;
			}

			LoadAndClearPendingState();
			Debug.LogError($"Package import failed: {errorMessage}");
			EditorUtility.DisplayDialog("Import Game", $"Package import failed:\n{errorMessage}", "OK");
		}

		private static void OnImportCancelled(string packageName)
		{
			if (string.IsNullOrEmpty(SessionState.GetString(SessionKeyPendingGameName, "")))
			{
				return;
			}

			LoadAndClearPendingState();
		}

		private static (HashSet<string> before, string packagePath, string jsonPathOverride) LoadAndClearPendingState()
		{
			string beforeRaw = SessionState.GetString(SessionKeyBeforeSnapshot, "");
			string packagePath = SessionState.GetString(SessionKeyPendingPackagePath, "");
			string jsonPathOverride = SessionState.GetString(SessionKeyPendingInputJsonPath, "");
			SessionState.EraseString(SessionKeyPendingGameName);
			SessionState.EraseString(SessionKeyBeforeSnapshot);
			SessionState.EraseString(SessionKeyPendingPackagePath);
			SessionState.EraseString(SessionKeyPendingInputJsonPath);

			var before = new HashSet<string>(beforeRaw.Split('\n').Where(s => s.Length > 0));
			return (before, packagePath, jsonPathOverride);
		}

		private static HashSet<string> SnapshotAssetPaths()
		{
			return new HashSet<string>(AssetDatabase.GetAllAssetPaths()
				.Where(p => p.StartsWith("Assets/") && !p.EndsWith(".meta")));
		}

		private static void FinishImport(string gameName, HashSet<string> before, string packagePath, string jsonPathOverride)
		{
			AssetDatabase.Refresh();

			HashSet<string> after = SnapshotAssetPaths();
			List<string> newPaths = after.Except(before).ToList();

			if (newPaths.Count == 0)
			{
				EditorUtility.DisplayDialog("Import Game", "No new files were imported by this package.", "OK");
				return;
			}

			if (!AssetDatabase.IsValidFolder(GamesRootFolder))
			{
				AssetDatabase.CreateFolder("Assets", "games");
			}

			string destinationFolder = $"{GamesRootFolder}/{gameName}";

			// Primary case: the package already scopes itself under Assets/games/<OriginalName>
			// (how chocolate/golfinity/nykrig were all authored) - detected as the one new
			// direct child of the games folder, regardless of whatever else the package
			// pulled in elsewhere (e.g. TextMesh Pro essentials).
			string sourceFolder = FindNewDirectChild(GamesRootFolder, before, after)
				?? FindCommonFolder(newPaths.Where(p => !IsSharedSupportPath(p)).ToList());

			if (sourceFolder != null && sourceFolder != "Assets")
			{
				if (sourceFolder != destinationFolder)
				{
					SafeMoveAsset(sourceFolder, destinationFolder);
				}
			}
			else
			{
				MoveScatteredTopLevelItems(before, newPaths, destinationFolder);
			}

			AssetDatabase.Refresh();

			RegisterScenesUnder(destinationFolder);
			EnsureGameListEntry(gameName, destinationFolder);

			// Optional pass 2/3: an explicit JSON path from the window, or (if left blank) a
			// legacy Input Manager export sitting next to the package (same filename, .json
			// instead of .unitypackage - see InputMigrationWindow). If found, generate its
			// action map and rewrite the game's own Input.* call sites to use it, before the
			// namespace rewrite below. Both operate purely on file contents/the shared
			// .inputactions asset, not on the game's namespace, so ordering relative to
			// NamespaceScriptsUnder doesn't matter functionally - they run first so
			// NamespaceScriptsUnder (which can trigger a script recompile / domain reload
			// that tears down this call stack) stays the single last script-touching step,
			// same risk as before this feature existed.
			var extraLog = new List<string>();
			string jsonPath = !string.IsNullOrEmpty(jsonPathOverride) ? jsonPathOverride : Path.ChangeExtension(packagePath, ".json");
			if (File.Exists(jsonPath))
			{
				try
				{
					int actionCount = InputMigrationWindow.GenerateActionMap(jsonPath, gameName,
						InputMigrationWindow.DefaultTargetAssetPath, InputMigrationWindow.DefaultSkipNamePatterns,
						InputMigrationWindow.DefaultAxisMapText, extraLog);
					extraLog.Add($"Generated action map '{gameName}' with {actionCount} action(s) from {Path.GetFileName(jsonPath)}.");

					int callSites = InputCodeMigrator.MigrateFolder(destinationFolder, gameName,
						InputMigrationWindow.DefaultTargetAssetPath, extraLog);
					extraLog.Add($"Migrated {callSites} input call site(s) in {destinationFolder}.");
				}
				catch (Exception e)
				{
					extraLog.Add($"Input migration failed: {e.Message}");
					Debug.LogError($"[GameImportWindow] Input migration failed: {e}");
				}
			}

			// Build Settings must be updated before the namespace rewrite below: rewriting
			// dozens of .cs files triggers a script recompile, which can synchronously tear
			// down this call stack via a domain reload - anything after that point may never
			// run. Nothing here after this point is safety-critical.
			NamespaceScriptsUnder(destinationFolder, gameName);

			AssetDatabase.Refresh();

			string message = $"Imported '{gameName}' into {destinationFolder}.";
			if (extraLog.Count > 0)
			{
				message += "\n\n" + string.Join("\n", extraLog);
			}

			EditorUtility.DisplayDialog("Import Game", message, "OK");
		}

		/// Finds the single new path that is a direct child of parentFolder (existed after
		/// import but not before). Returns null if there isn't exactly one such child.
		private static string FindNewDirectChild(string parentFolder, HashSet<string> before, HashSet<string> after)
		{
			string prefix = parentFolder + "/";
			bool IsDirectChild(string p) => p.StartsWith(prefix) && !p.Substring(prefix.Length).Contains("/");

			List<string> newDirectChildren = after
				.Where(p => IsDirectChild(p) && !before.Contains(p))
				.ToList();

			return newDirectChildren.Count == 1 ? newDirectChildren[0] : null;
		}

		private static bool IsSharedSupportPath(string path)
		{
			foreach (string shared in SharedSupportFolders)
			{
				if (path == shared || path.StartsWith(shared + "/"))
				{
					return true;
				}
			}

			return false;
		}

		/// Moves an asset, refusing outright if destination is nested inside source - moving a
		/// folder into a subfolder of itself is undefined/destructive at the filesystem level
		/// (this is exactly how a prior version of this tool corrupted Assets/games).
		private static void SafeMoveAsset(string source, string destination)
		{
			if (destination == source || destination.StartsWith(source + "/"))
			{
				throw new Exception($"Refusing to move '{source}' into its own descendant '{destination}'.");
			}

			string error = AssetDatabase.MoveAsset(source, destination);
			if (!string.IsNullOrEmpty(error))
			{
				throw new Exception($"Could not move '{source}' to '{destination}': {error}");
			}
		}

		private static void MoveScatteredTopLevelItems(HashSet<string> before, List<string> newPaths, string destinationFolder)
		{
			if (!AssetDatabase.IsValidFolder(destinationFolder))
			{
				AssetDatabase.CreateFolder(GamesRootFolder, destinationFolder.Substring(GamesRootFolder.Length + 1));
			}

			// Only genuinely new top-level items are candidates - this is what keeps a
			// pre-existing folder (like Assets/games itself) from being swept in just
			// because some of its new children happen to start with that prefix.
			var topLevelItems = newPaths
				.Where(p => !IsSharedSupportPath(p))
				.Select(p => p.Substring("Assets/".Length).Split('/')[0])
				.Distinct()
				.Where(item => !before.Contains($"Assets/{item}"));

			foreach (string item in topLevelItems)
			{
				string source = $"Assets/{item}";
				string destination = $"{destinationFolder}/{item}";
				SafeMoveAsset(source, destination);
			}
		}

		private static string FindCommonFolder(List<string> paths)
		{
			string[] commonSegments = null;

			foreach (string path in paths)
			{
				// A folder is itself a candidate common root, so its own segments are used
				// rather than its parent's - otherwise a single imported top-level folder
				// looks like it "diverges" from its own children and the root collapses to Assets.
				string[] dirSegments = AssetDatabase.IsValidFolder(path)
					? path.Split('/')
					: path.Split('/').Take(path.Split('/').Length - 1).ToArray();

				if (commonSegments == null)
				{
					commonSegments = dirSegments;
					continue;
				}

				int matchLength = 0;
				int maxLength = Math.Min(commonSegments.Length, dirSegments.Length);
				while (matchLength < maxLength && commonSegments[matchLength] == dirSegments[matchLength])
				{
					matchLength++;
				}

				commonSegments = commonSegments.Take(matchLength).ToArray();
			}

			if (commonSegments == null || commonSegments.Length == 0)
			{
				return "Assets";
			}

			return string.Join("/", commonSegments);
		}

		private static void NamespaceScriptsUnder(string folder, string gameName)
		{
			string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
			if (!Directory.Exists(absoluteFolder))
			{
				return;
			}

			// C# namespaces can't contain spaces - "Space Artist" becomes Games.SpaceArtist.
			// Everything else (folder path, action map name, GameList entry, menu display)
			// keeps the game name as typed, spaces included.
			string namespaceSegment = gameName.Replace(" ", "");

			string[] scriptFiles = Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories);
			foreach (string scriptFile in scriptFiles)
			{
				string contents = File.ReadAllText(scriptFile);
				string wrapped = WrapInNamespace(contents, $"Games.{namespaceSegment}");
				if (wrapped != null)
				{
					File.WriteAllText(scriptFile, wrapped);
				}
			}
		}

		/// Wraps top-level type declarations in a namespace block. Returns null if the
		/// file already declares a top-level namespace (left untouched to avoid double-nesting).
		private static string WrapInNamespace(string contents, string namespaceName)
		{
			if (Regex.IsMatch(contents, @"^\s*namespace\s+\S", RegexOptions.Multiline))
			{
				return null;
			}

			string[] lines = contents.Replace("\r\n", "\n").Split('\n');

			int bodyStart = 0;
			for (int i = 0; i < lines.Length; i++)
			{
				string trimmed = lines[i].Trim();
				if (trimmed.Length == 0 || trimmed.StartsWith("using ") || trimmed.StartsWith("//"))
				{
					bodyStart = i + 1;
					continue;
				}
				break;
			}

			var result = new System.Text.StringBuilder();
			for (int i = 0; i < bodyStart; i++)
			{
				result.Append(lines[i]).Append('\n');
			}

			bool lastPreambleLineBlank = bodyStart > 0 && lines[bodyStart - 1].Trim().Length == 0;
			if (!lastPreambleLineBlank)
			{
				result.Append('\n');
			}
			result.Append("namespace ").Append(namespaceName).Append('\n');
			result.Append("{\n");

			for (int i = bodyStart; i < lines.Length; i++)
			{
				if (lines[i].Length > 0)
				{
					result.Append('\t').Append(lines[i]);
				}
				result.Append('\n');
			}

			result.Append("}\n");

			return result.ToString();
		}

		private static void RegisterScenesUnder(string folder)
		{
			string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { folder });
			if (sceneGuids.Length == 0)
			{
				return;
			}

			var scenes = EditorBuildSettings.scenes.ToList();
			var existingPaths = new HashSet<string>(scenes.Select(s => s.path));

			// A game can have auxiliary scenes (e.g. an isolated editing scene for a popup
			// prefab) alongside its real entry point. The main menu now prefers GameList's
			// explicit entryScenePath (see EnsureGameListEntry), but this ordering is still
			// the fallback for games without one, so a scene literally named "main" is put
			// first when present rather than left to whatever alphabetical order FindAssets
			// happens to return.
			List<string> newScenePaths = sceneGuids
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(existingPaths.Add)
				.OrderByDescending(p => string.Equals(Path.GetFileNameWithoutExtension(p), "main", StringComparison.OrdinalIgnoreCase))
				.ToList();

			foreach (string scenePath in newScenePaths)
			{
				scenes.Add(new EditorBuildSettingsScene(scenePath, true));
			}

			EditorBuildSettings.scenes = scenes.ToArray();
		}

		private const string GameListFolder = "Assets/Resources/Games";
		private const string GameListAssetPath = GameListFolder + "/GameList.asset";

		/// Adds a blank entry for gameName to the shared GameList asset if one doesn't
		/// already exist (creating the asset itself on first use). Fields beyond gameName
		/// are left for manual editing afterward - description empty, gravity snapshotting
		/// whatever Physics2D.gravity currently is (so an untouched entry behaves the same
		/// as no override), entryScenePath guessed from folder's scenes (a scene named
		/// "main" preferred, otherwise whatever's found first) as a starting point to
		/// correct by hand if it's wrong, rather than re-guessed at runtime every time.
		private static void EnsureGameListEntry(string gameName, string destinationFolder)
		{
			GameList gameList = AssetDatabase.LoadAssetAtPath<GameList>(GameListAssetPath);
			if (gameList == null)
			{
				if (!AssetDatabase.IsValidFolder(GameListFolder))
				{
					if (!AssetDatabase.IsValidFolder("Assets/Resources"))
					{
						AssetDatabase.CreateFolder("Assets", "Resources");
					}

					AssetDatabase.CreateFolder("Assets/Resources", "Games");
				}

				gameList = ScriptableObject.CreateInstance<GameList>();
				AssetDatabase.CreateAsset(gameList, GameListAssetPath);
			}

			if (gameList.TryGetEntry(gameName, out _))
			{
				return;
			}

			var entry = new GameList.Entry
			{
				gameName = gameName,
				description = "",
				gravity = Physics2D.gravity,
				entryScenePath = GuessEntryScene(destinationFolder),
			};

			gameList.entries = gameList.entries.Append(entry).ToArray();
			EditorUtility.SetDirty(gameList);
			AssetDatabase.SaveAssets();
		}

		private static string GuessEntryScene(string folder)
		{
			string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { folder });
			if (sceneGuids.Length == 0)
			{
				return "";
			}

			return sceneGuids
				.Select(AssetDatabase.GUIDToAssetPath)
				.OrderByDescending(p => string.Equals(Path.GetFileNameWithoutExtension(p), "main", StringComparison.OrdinalIgnoreCase))
				.First();
		}
	}
}
