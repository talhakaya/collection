using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Collection.EditorTools
{
	/// <summary>
	/// Rewrites legacy UnityEngine.Input call sites to TaloketoInputManager, the New Input
	/// System shim (see Collection.Controls.TaloketoInputManager). Reusable core for both
	/// InputCodeMigrationWindow and the combined import pipeline in GameImportWindow.
	///
	/// Two kinds of call site are handled:
	///
	/// Input.GetAxis/GetAxisRaw/GetButton/GetButtonDown/GetButtonUp with a string literal
	/// argument, resolved against the game's action map.
	///
	/// Input.mousePosition and Input.GetMouseButton/Down/Up, which are the same for every
	/// game and need no map. Going through the shim is what lets the collection substitute
	/// the gamepad-emulated pointer for the real mouse without the game knowing.
	///
	/// Input.GetKey(KeyCode...), Input.touches and the rest are a different API shape and out
	/// of scope.
	/// </summary>
	public static class InputCodeMigrator
	{
		private const string UsingLine = "using Collection.Controls;";

		// Longest-prefix-conflicting alternatives first (GetAxisRaw before GetAxis,
		// GetButtonDown/Up before GetButton) so the regex engine can't short-match.
		private static readonly Regex CallSitePattern = new Regex(
			"Input\\.(GetAxisRaw|GetAxis|GetButtonDown|GetButtonUp|GetButton)\\(\\s*\"([^\"]*)\"\\s*\\)",
			RegexOptions.Compiled);

		// The lookbehind keeps this off anything that merely ends in "Input" - including
		// TaloketoInputManager's own members, so running the tool twice changes nothing. Only
		// the member name is matched, not the argument list: the shim's signatures are the
		// legacy ones, so whatever was passed stays as written.
		private static readonly Regex MouseCallSitePattern = new Regex(
			@"(?<![\w.])(?:UnityEngine\.)?Input\.(mousePosition|GetMouseButtonDown|GetMouseButtonUp|GetMouseButton)\b",
			RegexOptions.Compiled);

		/// <summary>
		/// Rewrites every .cs file under folderPath. Resolved names (present as an action in
		/// the gameName action map) become TaloketoInputManager calls; anything else becomes
		/// a literal default (0f / false) so a dropped legacy axis (e.g. a Mac-only trigger
		/// workaround skipped during JSON import) can't reintroduce a "not setup" runtime
		/// exception by falling through to the untouched legacy Input Manager. Mouse call
		/// sites are rewritten whether or not the game has an action map. Returns the number
		/// of call sites rewritten; appends notes to log.
		/// </summary>
		public static int MigrateFolder(string folderPath, string gameName, string targetAssetPath, List<string> log)
		{
			string assetFullPath = Path.Combine(Directory.GetCurrentDirectory(), targetAssetPath);
			var asset = InputActionAsset.FromJson(File.ReadAllText(assetFullPath));
			InputActionMap map = asset.FindActionMap(gameName);

			// Without a map there is nothing to resolve axis and button names against, and
			// turning them all into literals would gut the game's input - so those are left
			// alone. The mouse rewrites don't depend on it.
			HashSet<string> validNames = null;
			if (map == null)
			{
				log.Add($"No action map named '{gameName}' in {targetAssetPath} - axis and button calls left untouched.");
			}
			else
			{
				validNames = new HashSet<string>(map.actions.Select(a => a.name));
			}

			string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
			if (!Directory.Exists(absoluteFolder))
			{
				log.Add($"Folder '{folderPath}' does not exist.");
				return 0;
			}

			int totalCallSites = 0;
			int totalMouseCallSites = 0;
			int filesChanged = 0;

			foreach (string scriptFile in Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories))
			{
				string original = File.ReadAllText(scriptFile);
				int fileCallSites = 0;
				int fileMouseCallSites = 0;
				bool addedManagerCall = false;

				string rewritten = original;

				if (validNames != null)
				{
					rewritten = CallSitePattern.Replace(rewritten, match =>
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
				}

				rewritten = MouseCallSitePattern.Replace(rewritten, match =>
				{
					fileMouseCallSites++;
					addedManagerCall = true;
					return $"TaloketoInputManager.{match.Groups[1].Value}";
				});

				if (fileCallSites == 0 && fileMouseCallSites == 0)
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
				totalMouseCallSites += fileMouseCallSites;
			}

			log.Add($"Rewrote {totalCallSites} axis/button and {totalMouseCallSites} mouse call site(s) across {filesChanged} file(s).");

			return totalCallSites + totalMouseCallSites;
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
