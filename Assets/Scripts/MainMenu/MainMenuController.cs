using System.Collections.Generic;
using Collection.Controls;
using Collection.Saving;
using Collection.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.MainMenu
{
	/// <summary>
	/// The main menu. Its first screen (Story mode, Just the games, Settings, Exit), the list
	/// of games and the story slots are all screens of the collection's menus (Menus), drawn
	/// over this scene.
	///
	/// Choosing a story slot loads the story scene, played into that slot.
	/// </summary>
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private RectTransform contentParent;
		[SerializeField] private GameObject buttonTemplate;
		[SerializeField] private Text versionLabel;
		[SerializeField] private Text titleLabel;

		// Kept over scene loads, so that leaving a game comes back to the list it was
		// started from, on that game, rather than to the first screen.
		private static bool inList;
		private static string lastLaunched;

		private MenuScreen titleScreen;

		private struct GameEntry
		{
			public string displayName;
			public string scenePath;
		}

		private void Start()
		{
			// The scene's own list of games is not used any more: the list is a screen of the
			// collection's menus like the others (ShowList). Hidden, so nothing of it shows.
			buttonTemplate.SetActive(false);
			ScrollRect oldList = contentParent.GetComponentInParent<ScrollRect>(true);
			if (oldList != null) oldList.gameObject.SetActive(false);
			if (titleLabel != null) titleLabel.gameObject.SetActive(false);
			if (versionLabel != null) versionLabel.gameObject.SetActive(false);

			if (inList)
			{
				ShowList();
			}
			else
			{
				ShowTitle();
			}
		}

		// ---- The screens over the list ---------------------------------------------------

		private void ShowTitle()
		{
			inList = false;
			titleScreen = new MenuScreen { title = "Talovision", titleDances = true, opaque = true };
			titleScreen.Button("Story mode", ShowSlots);
			titleScreen.Button("Just the games", () =>
			{
				SaveManager.PlayFree();
				ShowList();
			});
			titleScreen.Button("Settings", SettingsScreen.Open);
			titleScreen.Button("Exit", Quit);
			titleScreen.cancel = () => { };
			titleScreen.footer = () => "v" + Application.version;
			Menus.CloseAll();
			Menus.Push(titleScreen);
		}

		/// The games, one button each, in GameList's order, scrolled through. It opens on the game
		/// last played from it, so leaving a game comes back to where it was.
		private void ShowList()
		{
			inList = true;
			var screen = new MenuScreen { title = "Just the games", opaque = true, scrolls = true };
			foreach (GameEntry entry in FindGameEntries())
			{
				string scenePath = entry.scenePath;
				if (scenePath == lastLaunched) screen.selected = screen.items.Count;
				screen.Button(entry.displayName, () =>
				{
					// Where it saves (a story slot, or the free-play area) was chosen on the way
					// to this list.
					lastLaunched = scenePath;
					Menus.CloseAll();
					SceneManager.LoadScene(scenePath);
				});
			}

			int games = screen.items.Count;
			screen.Button("Back", ShowTitle);
			screen.cancel = ShowTitle;
			screen.footer = () => screen.selected < games ? (screen.selected + 1) + " / " + games : "";
			Menus.CloseAll();
			Menus.Push(screen);
		}

		private static void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		private void ShowSlots()
		{
			var screen = new MenuScreen { title = "Story mode", opaque = true };
			for (int i = 0; i < SaveData.SlotCount; i++)
			{
				int slot = i;
				MenuItem item = screen.Button(null, () =>
				{
					if (SaveManager.Data.slots[slot].started) ShowSlot(slot);
					else StartStory(slot);
				});
				item.liveLabel = () =>
				{
					SaveSlotData data = SaveManager.Data.slots[slot];
					return "Slot " + (slot + 1) + "    " + (data.started ? PlayTime(data.timePlayed) + " played" : "Empty: new game");
				};
			}

			screen.Button("Back", Menus.Pop);
			Menus.Push(screen);
		}

		/// A slot with a game in it: carry on, or throw it away for a new one.
		private void ShowSlot(int slot)
		{
			var screen = new MenuScreen { title = "Slot " + (slot + 1), opaque = true };
			screen.body = () => PlayTime(SaveManager.Data.slots[slot].timePlayed) + " played";
			screen.Button("Continue", () => StartStory(slot));
			screen.Button("Delete and start a new game", () => ConfirmDelete(slot));
			screen.Button("Back", Menus.Pop);
			Menus.Push(screen);
		}

		/// Asked once, opening on "No": deleting a slot cannot be undone.
		private void ConfirmDelete(int slot)
		{
			string played = PlayTime(SaveManager.Data.slots[slot].timePlayed);
			var screen = new MenuScreen { title = "Delete slot " + (slot + 1) + "?", opaque = true };
			screen.body = () => "Everything in this slot will be lost (" + played + " of play) and a new game started in it. This cannot be undone.";
			screen.Button("No, keep it", Menus.Pop);
			screen.Button("Yes, delete it", () =>
			{
				SaveManager.DeleteSlot(slot);
				StartStory(slot);
			});
			Menus.Push(screen);
		}

		private void StartStory(int slot)
		{
			SaveManager.PlayStory(slot);
			if (!SaveManager.Slot.started)
			{
				SaveManager.Slot.started = true;
				SaveManager.Save();
			}

			// Leaving the story comes back to the first screen, not to the list of games.
			// The loading screen goes up before the menus come down, so the list of games
			// behind them is never what is looked at while the story loads.
			inList = false;
			Collection.Story.StoryLevels.Load();
			Menus.CloseAll();
		}

		private static string PlayTime(float seconds)
		{
			int total = Mathf.FloorToInt(seconds);
			int hours = total / 3600;
			int minutes = total / 60 % 60;
			return hours > 0 ? hours + "h " + minutes.ToString("00") + "m" : minutes + "m " + (total % 60).ToString("00") + "s";
		}

		private static IEnumerable<GameEntry> FindGameEntries()
		{
			var seenFolders = new HashSet<string>();
			GameList gameList = Resources.Load<GameList>("Games/GameList");

			// GameList's entry order is authoritative for menu order - reorder entries in
			// the Inspector to reorder the menu, rather than depending on Build Settings/
			// import order.
			if (gameList != null)
			{
				foreach (GameList.Entry entry in gameList.entries)
				{
					if (string.IsNullOrEmpty(entry.gameName) || !seenFolders.Add(entry.gameName))
					{
						continue;
					}

					// Hidden games are left out. They are in seenFolders by now, so the safety
					// net below does not bring them back.
					if (entry.hidden)
					{
						continue;
					}

					string scenePath = !string.IsNullOrEmpty(entry.entryScenePath)
						? entry.entryScenePath
						: FindFirstRegisteredScene(entry.gameName);

					if (scenePath == null)
					{
						continue;
					}

					yield return new GameEntry { displayName = Capitalize(entry.gameName), scenePath = scenePath };
				}
			}

			// Safety net: a game with a registered scene but no GameList entry yet (or an
			// entry with no scene resolvable) still shows up, just appended after the
			// explicitly-ordered ones rather than silently missing from the menu.
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);
				string folderName = GameContext.FromScenePath(path);
				if (folderName == null || !seenFolders.Add(folderName))
				{
					continue;
				}

				yield return new GameEntry { displayName = Capitalize(folderName), scenePath = path };
			}
		}

		private static string FindFirstRegisteredScene(string gameName)
		{
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);
				if (string.Equals(GameContext.FromScenePath(path), gameName, System.StringComparison.OrdinalIgnoreCase))
				{
					return path;
				}
			}

			return null;
		}

		private static string Capitalize(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			return char.ToUpperInvariant(value[0]) + value.Substring(1);
		}
	}
}
