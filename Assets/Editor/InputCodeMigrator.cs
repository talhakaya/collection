using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Collection.EditorTools
{
	/// <summary>
	/// Rewrites legacy UnityEngine.Input axis/button call sites to TaloketoInputManager,
	/// the New Input System shim (see Collection.Controls.TaloketoInputManager). Reusable
	/// core for both InputCodeMigrationWindow and the combined import pipeline in
	/// GameImportWindow.
	///
	/// Only Input.GetAxis/GetAxisRaw/GetButton/GetButtonDown/GetButtonUp calls with a string
	/// literal argument are touched - Input.GetKey(KeyCode...), Input.mousePosition,
	/// Input.GetMouseButton etc. are a different API shape and out of scope.
	/// </summary>
	public static class InputCodeMigrator
	{
		private const string UsingLine = "using Collection.Controls;";

		// Longest-prefix-conflicting alternatives first (GetAxisRaw before GetAxis,
		// GetButtonDown/Up before GetButton) so the regex engine can't short-match.
		private static readonly Regex CallSitePattern = new Regex(
			"Input\\.(GetAxisRaw|GetAxis|GetButtonDown|GetButtonUp|GetButton)\\(\\s*\"([^\"]*)\"\\s*\\)",
			RegexOptions.Compiled);

		/// <summary>
		/// Rewrites every .cs file under folderPath. Resolved names (present as an action in
		/// the gameName action map) become TaloketoInputManager calls; anything else becomes
		/// a literal default (0f / false) so a dropped legacy axis (e.g. a Mac-only trigger
		/// workaround skipped during JSON import) can't reintroduce a "not setup" runtime
		/// exception by falling through to the untouched legacy Input Manager. Returns the
		/// number of call sites rewritten; appends notes to log.
		/// </summary>
		public static int MigrateFolder(string folderPath, string gameName, string targetAssetPath, List<string> log)
		{
			string assetFullPath = Path.Combine(Directory.GetCurrentDirectory(), targetAssetPath);
			var asset = InputActionAsset.FromJson(File.ReadAllText(assetFullPath));
			InputActionMap map = asset.FindActionMap(gameName);
			if (map == null)
			{
				log.Add($"No action map named '{gameName}' in {targetAssetPath} - nothing to resolve against.");
				return 0;
			}

			var validNames = new HashSet<string>(map.actions.Select(a => a.name));

			string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
			if (!Directory.Exists(absoluteFolder))
			{
				log.Add($"Folder '{folderPath}' does not exist.");
				return 0;
			}

			int totalCallSites = 0;
			int filesChanged = 0;

			foreach (string scriptFile in Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories))
			{
				string original = File.ReadAllText(scriptFile);
				int fileCallSites = 0;
				bool addedManagerCall = false;

				string rewritten = CallSitePattern.Replace(original, match =>
				{
					string methodName = match.Groups[1].Value;
					string argName = match.Groups[2].Value;
					bool isAxis = methodName == "GetAxis" || methodName == "GetAxisRaw";

					if (validNames.Contains(argName))
					{
						fileCallSites++;
						addedManagerCall = true;
						return $"TaloketoInputManager.{methodName}(\"{argName}\")";
					}

					fileCallSites++;
					log.Add($"{Path.GetFileName(scriptFile)}: '{argName}' isn't in map '{gameName}' - " +
					        $"replaced with a literal {(isAxis ? "0f" : "false")}.");
					return isAxis ? "0f" : "false";
				});

				if (fileCallSites == 0)
				{
					continue;
				}

				if (addedManagerCall && !Regex.IsMatch(rewritten, @"^\s*using\s+Collection\.Controls\s*;", RegexOptions.Multiline))
				{
					rewritten = InsertUsing(rewritten);
				}

				File.WriteAllText(scriptFile, rewritten);
				filesChanged++;
				totalCallSites += fileCallSites;
			}

			log.Add($"Rewrote {totalCallSites} call site(s) across {filesChanged} file(s).");

			return totalCallSites;
		}

		private static string InsertUsing(string contents)
		{
			string[] lines = contents.Replace("\r\n", "\n").Split('\n');

			int lastUsingLine = -1;
			for (int i = 0; i < lines.Length; i++)
			{
				string trimmed = lines[i].Trim();
				if (trimmed.StartsWith("using ") && trimmed.EndsWith(";"))
				{
					lastUsingLine = i;
				}
				else if (trimmed.Length > 0 && !trimmed.StartsWith("//"))
				{
					break;
				}
			}

			var result = new System.Text.StringBuilder();
			for (int i = 0; i < lines.Length; i++)
			{
				result.Append(lines[i]);
				if (i < lines.Length - 1)
				{
					result.Append('\n');
				}

				if (i == lastUsingLine)
				{
					result.Append(UsingLine).Append('\n');
				}
			}

			if (lastUsingLine < 0)
			{
				return UsingLine + "\n" + result;
			}

			return result.ToString();
		}
	}
}
