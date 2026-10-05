using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;

namespace Collection.UI
{
	/// <summary>
	/// The settings screen, reached from the main menu and from the pause screen.
	///
	/// What it shows is a copy of the saved settings. The volumes are heard as they are
	/// moved; the window only changes on Apply, which is also what saves. Back without
	/// Apply puts the saved volumes back.
	/// </summary>
	public static class SettingsScreen
	{
		public static void Open()
		{
			Settings saved = SaveManager.Settings;
			var shown = new Settings();
			Copy(saved, shown);

			List<Vector2Int> resolutions = Resolutions();

			var screen = new MenuScreen { title = "Settings" };

			screen.Slider("Master volume", () => shown.masterVolume, value =>
			{
				shown.masterVolume = value;
				CollectionSettings.ApplyAudio(shown);
			});

			screen.Slider("Music volume", () => shown.musicVolume, value =>
			{
				shown.musicVolume = value;
				CollectionSettings.ApplyAudio(shown);
			});

			screen.Choice("Fullscreen", () => shown.fullscreen ? "On" : "Off", by => shown.fullscreen = !shown.fullscreen);

			screen.Choice("Resolution",
				() => shown.resolutionWidth <= 0 ? "Display's own" : shown.resolutionWidth + " x " + shown.resolutionHeight,
				by =>
				{
					int at = resolutions.IndexOf(new Vector2Int(shown.resolutionWidth, shown.resolutionHeight));
					if (at < 0) at = 0;
					at = ((at + by) % resolutions.Count + resolutions.Count) % resolutions.Count;
					shown.resolutionWidth = resolutions[at].x;
					shown.resolutionHeight = resolutions[at].y;
				});

			string applied = "";
			screen.Button("Apply", () =>
			{
				Copy(shown, saved);
				SaveManager.MarkSettingsDirty();
				CollectionSettings.Apply();
				applied = "Saved.";
			});

			screen.Button("Defaults", () =>
			{
				Copy(new Settings(), shown);
				CollectionSettings.ApplyAudio(shown);
				applied = "These are the defaults. Apply to keep them.";
			});

			System.Action back = () =>
			{
				CollectionSettings.ApplyAudio(saved);
				Menus.Pop();
			};

			screen.Button("Back", back);
			screen.cancel = back;
			screen.footer = () => Same(shown, saved) ? applied : "Not applied yet.";

			Menus.Push(screen);
		}

		/// The display's own resolution (0 x 0) first, then every size the display offers,
		/// smallest first.
		private static List<Vector2Int> Resolutions()
		{
			var list = new List<Vector2Int> { Vector2Int.zero };
			foreach (Resolution resolution in Screen.resolutions)
			{
				var size = new Vector2Int(resolution.width, resolution.height);
				if (size.y >= 480 && !list.Contains(size))
				{
					list.Add(size);
				}
			}

			return list;
		}

		private static void Copy(Settings from, Settings to)
		{
			to.masterVolume = from.masterVolume;
			to.musicVolume = from.musicVolume;
			to.fullscreen = from.fullscreen;
			to.resolutionWidth = from.resolutionWidth;
			to.resolutionHeight = from.resolutionHeight;
		}

		private static bool Same(Settings a, Settings b)
		{
			return Mathf.Approximately(a.masterVolume, b.masterVolume) && Mathf.Approximately(a.musicVolume, b.musicVolume)
				&& a.fullscreen == b.fullscreen && a.resolutionWidth == b.resolutionWidth && a.resolutionHeight == b.resolutionHeight;
		}
	}
}
