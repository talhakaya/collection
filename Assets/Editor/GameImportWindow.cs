using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
	[InitializeOnLoad]
	public class GameImportWindow : EditorWindow
	{
		private const string GamesRootFolder = "Assets/games";

		// Import is asynchronous and, when the package contains scripts, triggers a
		// domain reload before it completes - which wipes any runtime "+=" subscription.
		// Callbacks are instead registered once per domain load via [InitializeOnLoad],
		// and pending state survives the reload in SessionState rather than static fields.
		private const string SessionKeyPendingGameName = "Collection.GameImportWindow.PendingGameName";
		private const string SessionKeyBeforeSnapshot = "Collection.GameImportWindow.BeforeSnapshot";

		private string packagePath = "";
		private string gameName = "";
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

			string validationError = Validate();
			if (!string.IsNullOrEmpty(validationError))
			{
				EditorGUILayout.HelpBox(validationError, MessageType.Warning);
			}

			GUI.enabled = string.IsNullOrEmpty(validationError);
			if (GUILayout.Button("Import", GUILayout.Height(30)))
			{
				BeginImport(packagePath, gameName);
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

			if (string.IsNullOrEmpty(gameName))
			{
				return "Enter a game name.";
			}

			if (!Regex.IsMatch(gameName, "^[A-Za-z][A-Za-z0-9]*$"))
			{
				return "Game name must start with a letter and contain only letters and digits.";
			}

			if (AssetDatabase.IsValidFolder($"{GamesRootFolder}/{gameName}"))
			{
				return $"'{GamesRootFolder}/{gameName}' already exists.";
			}

			return null;
		}

		private void BeginImport(string path, string name)
		{
			SessionState.SetString(SessionKeyPendingGameName, name);
			SessionState.SetString(SessionKeyBeforeSnapshot, string.Join("\n", SnapshotAssetPaths()));

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

			HashSet<string> before = LoadAndClearPendingState();

			try
			{
				FinishImport(pendingGameName, before);
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

		private static HashSet<string> LoadAndClearPendingState()
		{
			string beforeRaw = SessionState.GetString(SessionKeyBeforeSnapshot, "");
			SessionState.EraseString(SessionKeyPendingGameName);
			SessionState.EraseString(SessionKeyBeforeSnapshot);

			return new HashSet<string>(beforeRaw.Split('\n').Where(s => s.Length > 0));
		}

		private static HashSet<string> SnapshotAssetPaths()
		{
			return new HashSet<string>(AssetDatabase.GetAllAssetPaths()
				.Where(p => p.StartsWith("Assets/") && !p.EndsWith(".meta")));
		}

		private static void FinishImport(string gameName, HashSet<string> before)
		{
			AssetDatabase.Refresh();

			HashSet<string> after = SnapshotAssetPaths();
			after.ExceptWith(before);
			List<string> newPaths = after.ToList();

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
			string commonRoot = FindCommonFolder(newPaths);

			if (commonRoot != "Assets")
			{
				string error = AssetDatabase.MoveAsset(commonRoot, destinationFolder);
				if (!string.IsNullOrEmpty(error))
				{
					throw new Exception($"Could not move '{commonRoot}' to '{destinationFolder}': {error}");
				}
			}
			else
			{
				AssetDatabase.CreateFolder(GamesRootFolder, gameName);
				var topLevelItems = newPaths
					.Select(p => p.Substring("Assets/".Length).Split('/')[0])
					.Distinct();

				foreach (string item in topLevelItems)
				{
					string source = $"Assets/{item}";
					string destination = $"{destinationFolder}/{item}";
					string error = AssetDatabase.MoveAsset(source, destination);
					if (!string.IsNullOrEmpty(error))
					{
						throw new Exception($"Could not move '{source}' to '{destination}': {error}");
					}
				}
			}

			AssetDatabase.Refresh();

			NamespaceScriptsUnder(destinationFolder, gameName);
			RegisterScenesUnder(destinationFolder);

			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog("Import Game", $"Imported '{gameName}' into {destinationFolder}.", "OK");
		}

		private static string FindCommonFolder(List<string> paths)
		{
			string[] commonSegments = null;

			foreach (string path in paths)
			{
				// A folder is itself a candidate common root, so its own segments are used
				// rather than its parent's - otherwise a single imported top-level folder
				// looks like it "diverges" from its own children and the root collapses to Assets.
				string[] segments = AssetDatabase.IsValidFolder(path)
					? path.Split('/')
					: path.Split('/').Take(path.Split('/').Length - 1).ToArray();
				string[] dirSegments = segments;

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

			string[] scriptFiles = Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories);
			foreach (string scriptFile in scriptFiles)
			{
				string contents = File.ReadAllText(scriptFile);
				string wrapped = WrapInNamespace(contents, $"Games.{gameName}");
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

			foreach (string guid in sceneGuids)
			{
				string scenePath = AssetDatabase.GUIDToAssetPath(guid);
				if (existingPaths.Add(scenePath))
				{
					scenes.Add(new EditorBuildSettingsScene(scenePath, true));
				}
			}

			EditorBuildSettings.scenes = scenes.ToArray();
		}
	}
}
