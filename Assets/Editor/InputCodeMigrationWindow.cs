using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
	/// <summary>
	/// Standalone UI over InputCodeMigrator - rewrites a game's Input.* call sites to
	/// TaloketoInputManager, matched against an already-generated action map (run
	/// "Import Input Manager JSON..." first). See InputCodeMigrator for the actual logic.
	/// </summary>
	public class InputCodeMigrationWindow : EditorWindow
	{
		private string gameFolder = "";
		private string gameName = "";
		private string targetAssetPath = "";
		private string statusMessage = "";
		private MessageType statusType = MessageType.None;

		[MenuItem("Tools/Collection/Migrate Input Call Sites...")]
		private static void ShowWindow()
		{
			var window = GetWindow<InputCodeMigrationWindow>(true, "Migrate Input Call Sites", true);
			window.targetAssetPath = InputMigrationWindow.DefaultTargetAssetPath;
			window.minSize = new Vector2(420, 200);
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Game Script Folder", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			gameFolder = EditorGUILayout.TextField(gameFolder);
			if (GUILayout.Button("Browse...", GUILayout.Width(80)))
			{
				string selected = EditorUtility.OpenFolderPanel("Select Game Folder", "Assets/games", "");
				if (!string.IsNullOrEmpty(selected))
				{
					gameFolder = ToProjectRelativePath(selected);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Name (Action Map)", EditorStyles.boldLabel);
			gameName = EditorGUILayout.TextField(gameName);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Actions Asset", EditorStyles.boldLabel);
			targetAssetPath = EditorGUILayout.TextField(targetAssetPath);

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(
				"Rewrites Input.GetAxis/GetAxisRaw/GetButton/GetButtonDown/GetButtonUp calls whose " +
				"name matches an action in the map to TaloketoInputManager. Names that don't match " +
				"(e.g. a Mac-only axis dropped during JSON import) become a literal 0f/false instead " +
				"of being left as a legacy call that would throw at runtime.",
				MessageType.Info);

			EditorGUILayout.Space();
			string validationError = Validate();
			if (!string.IsNullOrEmpty(validationError))
			{
				EditorGUILayout.HelpBox(validationError, MessageType.Warning);
			}

			GUI.enabled = string.IsNullOrEmpty(validationError);
			if (GUILayout.Button("Migrate Call Sites", GUILayout.Height(30)))
			{
				Migrate();
			}
			GUI.enabled = true;

			if (!string.IsNullOrEmpty(statusMessage))
			{
				EditorGUILayout.HelpBox(statusMessage, statusType);
			}
		}

		private string Validate()
		{
			if (string.IsNullOrEmpty(gameFolder) || !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), gameFolder)))
			{
				return "Choose the game's script folder.";
			}

			if (string.IsNullOrEmpty(gameName))
			{
				return "Enter the game name (must match the generated action map).";
			}

			if (string.IsNullOrEmpty(targetAssetPath) || !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), targetAssetPath)))
			{
				return "Actions asset not found.";
			}

			return null;
		}

		private void Migrate()
		{
			var log = new List<string>();
			try
			{
				int callSites = InputCodeMigrator.MigrateFolder(gameFolder, gameName, targetAssetPath, log);
				AssetDatabase.Refresh();
				string summary = $"Migrated {callSites} call site(s) in {gameFolder}.";
				statusMessage = summary + "\n\n" + string.Join("\n", log);
				statusType = MessageType.Info;
				Debug.Log($"[InputCodeMigrationWindow] {statusMessage}");
			}
			catch (Exception e)
			{
				statusMessage = $"Failed: {e.Message}";
				statusType = MessageType.Error;
				Debug.LogError($"[InputCodeMigrationWindow] {e}");
			}
		}

		private static string ToProjectRelativePath(string absolutePath)
		{
			string projectPath = Directory.GetCurrentDirectory().Replace('\\', '/');
			string full = absolutePath.Replace('\\', '/');
			return full.StartsWith(projectPath) ? full.Substring(projectPath.Length + 1) : full;
		}
	}
}
