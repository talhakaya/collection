using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Collection.EditorTools
{
	/// <summary>
	/// One-click player builds, each into its own folder under Builds/ (which is gitignored),
	/// and the upload of the Steam build through SteamPipe.
	///
	/// Also callable without the Editor window, for scripting a build:
	///   Unity.exe -batchmode -quit -projectPath . -executeMethod Collection.EditorTools.BuildScript.Build_Win_Steam
	/// In batch mode a failed build or upload exits with a non-zero code.
	/// </summary>
	public static class BuildScript
	{
		public const string DISABLESTEAMWORKS = "DISABLESTEAMWORKS";

		private const string ApplicationName = "Talovision";

		private const string SteamAccount = "taloketo";
		private const string SteamAppId = "5049170";
		private const string SteamBuildFolder = "Steam/Win";

		// steamcmd is Valve's binary: it updates itself and keeps the cached login token next
		// to it, so it stays inside Builds/ and out of git. The scripts that say what to
		// upload are ours and are tracked, in SteamPipe/ at the project root. The app build
		// script names the depot script, and that is what points at Builds/Steam/Win.
		private const string SteamCmdFolder = "Builds/ContentBuilder/builder";
		private const string SteamAppBuildScript = "SteamPipe/app_build_" + SteamAppId + ".vdf";

		#region Steam

		[MenuItem("File/Build Script/Enable Steamworks")]
		public static void Enable_Steam()
		{
			ExcludeDefine(EditorUserBuildSettings.selectedBuildTargetGroup, DISABLESTEAMWORKS);
		}

		[MenuItem("File/Build Script/Disable Steamworks")]
		public static void Disable_Steam()
		{
			IncludeDefine(EditorUserBuildSettings.selectedBuildTargetGroup, DISABLESTEAMWORKS);
		}

		// Steamworks is only ever enabled for the duration of a Steam build, and switched
		// straight back off afterwards. Left enabled, the next play-mode session in the Editor
		// would connect to Steam as if it were the game running, which confuses the Steam
		// client. To test Steam features in the Editor on purpose, use Enable Steamworks.

		[MenuItem("File/Build Script/Win Steam")]
		public static void Build_Win_Steam()
		{
			Enable_Steam();
			bool built = Build_Win(SteamBuildFolder);
			Disable_Steam();

			ExitIfBatchModeFailed(built);
		}

		/// <summary>
		/// The one path that produces a new version: bumps the build number, builds with it
		/// baked in, and uploads. The bump has to come first - the version is compiled into the
		/// player - so a failed build puts the old number back rather than burning one.
		/// </summary>
		[MenuItem("File/Build Script/Win Steam and Upload")]
		public static void Build_Win_Steam_And_Upload()
		{
			string previousVersion = PlayerSettings.bundleVersion;
			if (!ScenesAreSaved() || !BumpBuildNumber())
			{
				ExitIfBatchModeFailed(false);
				return;
			}

			Enable_Steam();
			bool built = Build_Win(SteamBuildFolder);
			Disable_Steam();

			if (!built)
			{
				SetVersion(previousVersion);
				Debug.Log($"Version left at {previousVersion} - nothing was built.");
				ExitIfBatchModeFailed(false);
				return;
			}

			Upload_Win_Steam();
		}

		/// <summary>
		/// Uploads whatever is in Builds/Steam/Win as a new build of the app.
		///
		/// No password is passed or stored: steamcmd logs in with the token it cached in its
		/// own config folder the last time this account logged in on this machine. When there
		/// is none, or it has expired, steamcmd asks for the password and Steam Guard code -
		/// which is why this opens a console window rather than running hidden.
		///
		/// Uploads the folder as it is, so the version is whatever that build was made with:
		/// this does not bump it. For a new version use Win Steam and Upload.
		///
		/// Where the build goes live is the app build script's "setlive" field - the "beta"
		/// branch, which is what test machines opt into. SteamPipe will not set the default
		/// branch live by itself; promoting a build there is done from the Builds page in
		/// Steamworks.
		/// </summary>
		[MenuItem("File/Build Script/Upload Win Steam")]
		public static void Upload_Win_Steam()
		{
			string steamCmd = Path.GetFullPath($"{SteamCmdFolder}/steamcmd.exe");
			string appBuildScript = Path.GetFullPath(SteamAppBuildScript);
			string player = Path.GetFullPath($"Builds/{SteamBuildFolder}/{ApplicationName}.exe");

			string missing = !File.Exists(player) ? $"no build at {player} - run Win Steam first"
				: !File.Exists(steamCmd) ? $"steamcmd not found at {steamCmd}"
				: !File.Exists(appBuildScript) ? $"app build script not found at {appBuildScript}"
				: null;
			if (missing != null)
			{
				Debug.LogError($"Steam upload not started: {missing}.");
				if (Application.isBatchMode) EditorApplication.Exit(1);
				return;
			}

			// Again here, not only after a build: the folder may have been filled by a build
			// made by hand from the Build window, which leaves them in place.
			RemoveDoNotShipFolders($"Builds/{SteamBuildFolder}");

			string arguments = $"+login {SteamAccount} +run_app_build \"{appBuildScript}\" +quit";
			string buildsPage = $"https://partner.steamgames.com/apps/builds/{SteamAppId}";

			if (Application.isBatchMode)
			{
				// Already in a console, and the caller wants the result: run it in place and
				// pass steamcmd's exit code on.
				var inPlace = Process.Start(new ProcessStartInfo(steamCmd, arguments)
				{
					WorkingDirectory = Path.GetDirectoryName(steamCmd),
					UseShellExecute = false,
				});
				inPlace.WaitForExit();
				if (inPlace.ExitCode != 0) EditorApplication.Exit(inPlace.ExitCode);
				return;
			}

			// "& pause" keeps the window open after steamcmd quits, so the result can be read.
			Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"\"{steamCmd}\" {arguments} & pause\"")
			{
				WorkingDirectory = Path.GetDirectoryName(steamCmd),
				UseShellExecute = true,
			});

			Debug.Log($"Steam upload started in a console window - check it for the result. " +
				$"A finished upload goes live on the branch the app build script's \"setlive\" names (beta). " +
				$"Promote it to the default branch from: {buildsPage}");
		}

		#endregion

		#region Version
		// MAJOR.MINOR.BUILD, kept in Unity's own version field (Player Settings > Version),
		// which is what Application.version returns in the player and what the main menu
		// shows.
		//
		//   BUILD  goes up by one on every Win Steam and Upload, and nowhere else, so each
		//          number is exactly one uploaded build.
		//   MINOR  is bumped by hand when something meaningful lands. Resets BUILD.
		//   MAJOR  is the release: 0.x until then, 1.0.0 on the day. Resets the rest.
		//
		// The number lives in ProjectSettings.asset, which is tracked - so a bump is a change
		// to commit and push (as "v0.2.1"), or the next machine to build carries on from a
		// stale number.

		[MenuItem("File/Build Script/Version/Bump Minor")]
		public static void Bump_Minor()
		{
			if (!TryParseVersion(PlayerSettings.bundleVersion, out int major, out int minor, out _)) return;
			SetVersion($"{major}.{minor + 1}.0");
		}

		[MenuItem("File/Build Script/Version/Bump Major")]
		public static void Bump_Major()
		{
			if (!TryParseVersion(PlayerSettings.bundleVersion, out int major, out _, out _)) return;
			SetVersion($"{major + 1}.0.0");
		}

		private static bool BumpBuildNumber()
		{
			if (!TryParseVersion(PlayerSettings.bundleVersion, out int major, out int minor, out int build)) return false;
			SetVersion($"{major}.{minor}.{build + 1}");
			return true;
		}

		private static void SetVersion(string version)
		{
			PlayerSettings.bundleVersion = version;
			// Written out now rather than whenever the Editor next saves, so the change is on
			// disk to be committed - and survives if the Editor is closed straight after.
			AssetDatabase.SaveAssets();
			Debug.Log($"Version is now {version}");
		}

		/// Missing parts count as 0, so the "0.2" this project started with reads as 0.2.0.
		private static bool TryParseVersion(string version, out int major, out int minor, out int build)
		{
			major = minor = build = 0;
			string[] parts = (version ?? "").Split('.');
			bool ok = parts.Length >= 1 && parts.Length <= 3
				&& int.TryParse(parts[0], out major)
				&& (parts.Length < 2 || int.TryParse(parts[1], out minor))
				&& (parts.Length < 3 || int.TryParse(parts[2], out build));
			if (!ok)
			{
				Debug.LogError($"Version '{version}' is not MAJOR.MINOR.BUILD - fix it in Player Settings > Version.");
			}

			return ok;
		}

		#endregion

		#region Standalone

		[MenuItem("File/Build Script/Win Standalone")]
		public static void Build_Win_Standalone()
		{
			Disable_Steam();
			ExitIfBatchModeFailed(Build_Win("Standalone/Win"));
		}

		public static bool Build_Win(string FolderName)
		{
			string outputFolder = $"Builds/{FolderName}";
			const string applicationFilename = ApplicationName + ".exe";
			const BuildTarget target = BuildTarget.StandaloneWindows64;
			const BuildTargetGroup group = BuildTargetGroup.Standalone;

			return Build(target, group, outputFolder, applicationFilename);
		}

		#endregion

		#region Others

		/// <summary>
		/// Whether every open scene is saved. BuildPlayer checks this too, but its way of
		/// saying so is a modal "Scene(s) Have Been Modified" dialog in the middle of the
		/// build - which stalls a scripted build indefinitely with nothing in the log.
		/// Refusing up front, before anything has been bumped or switched, is the same answer
		/// without the stall.
		/// </summary>
		private static bool ScenesAreSaved()
		{
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				if (scene.isDirty)
				{
					Debug.LogError($"Build not started: '{scene.name}' has unsaved changes. Save or discard them first.");
					return false;
				}
			}

			return true;
		}

		private static bool Build(BuildTarget target, BuildTargetGroup group, string outputFolder, string applicationFile)
		{
			if (!ScenesAreSaved())
			{
				return false;
			}

			EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);

			Debug.Log("Running build script");

			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = GetSceneList(),
				locationPathName = $"{outputFolder}/{applicationFile}",
				target = target,
				targetGroup = group,
				options = BuildOptions.None
			};

			var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
			var summary = report.summary;

			switch (summary.result)
			{
				case BuildResult.Succeeded:
					Debug.Log($"Build succeeded: {summary.totalSize} bytes, in {outputFolder}");
					break;
				case BuildResult.Cancelled:
					Debug.Log("Build cancelled");
					break;
				default:
					Debug.LogError($"Build failed, total errors: {summary.totalErrors}");
					break;
			}

			RemoveDoNotShipFolders(outputFolder);

			return summary.result == BuildResult.Succeeded;
		}

		/// From the command line the exit code is the only thing a calling script can check.
		/// Called by the menu entry points rather than from Build itself, so they get to put
		/// things back (the Steamworks define, a bumped version) before the Editor quits.
		private static void ExitIfBatchModeFailed(bool succeeded)
		{
			if (Application.isBatchMode && !succeeded)
			{
				EditorApplication.Exit(1);
			}
		}

		/// <summary>
		/// Deletes the folders Unity writes next to the player that are not part of the game:
		/// Burst's debug symbols (..._BurstDebugInformation_DoNotShip) and, on IL2CPP, the
		/// symbol backup (..._BackUpThisFolder_ButDontShipItWithYourGame). SteamPipe uploads
		/// everything in the folder, so anything left here ends up on players' machines.
		///
		/// Matched by Unity's own naming rather than by exact folder name, so it keeps working
		/// if the product is renamed or another such folder appears.
		/// </summary>
		private static void RemoveDoNotShipFolders(string outputFolder)
		{
			if (!Directory.Exists(outputFolder))
			{
				return;
			}

			foreach (string dir in Directory.GetDirectories(outputFolder))
			{
				string name = Path.GetFileName(dir);
				if (name.EndsWith("_DoNotShip") || name.EndsWith("_ButDontShipItWithYourGame"))
				{
					Directory.Delete(dir, true);
					Debug.Log($"Removed {name} from {outputFolder}");
				}
			}
		}

		/// Only the scenes ticked in Build Settings. A disabled entry is still listed there,
		/// but passing it to BuildPlayer would build it in anyway.
		private static string[] GetSceneList()
		{
			var paths = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes)
			{
				if (scene.enabled)
				{
					paths.Add(scene.path);
				}
			}
			return paths.ToArray();
		}

		private static void ExcludeDefine(BuildTargetGroup targetGroup, string define)
		{
			NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			var list = PlayerSettings.GetScriptingDefineSymbols(target).Split(';').ToList();
			if (!list.Contains(define))
			{
				return;
			}

			list.Remove(define);
			PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", list));
			AssetDatabase.SaveAssets();
		}

		private static void IncludeDefine(BuildTargetGroup targetGroup, string define)
		{
			NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			var list = PlayerSettings.GetScriptingDefineSymbols(target).Split(';').Where(s => s.Length > 0).ToList();
			if (list.Contains(define))
			{
				return;
			}

			list.Add(define);
			PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", list));
			// Written out now, like the version: both live in ProjectSettings.asset, and when
			// the version bump is committed straight after an upload the file must not still
			// be showing the define that the Steam build temporarily removed.
			AssetDatabase.SaveAssets();
		}

		#endregion
	}
}
