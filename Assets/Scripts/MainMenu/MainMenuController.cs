using System.Collections.Generic;
using Collection.Controls;
using Collection.Saving;
using Collection.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.MainMenu
{
	/// <summary>
	/// The main menu. Its first screen (Story mode, Just the games, Settings, Exit) and the
	/// story slots are screens of the collection's menus (Menus), drawn over this scene;
	/// what is in the scene itself is the list of games.
	///
	/// Placeholder: the story mode is not made yet, so choosing a slot leads to the same
	/// list of games, played into that slot.
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

		private GameObject lastSelected;
		private MenuScreen titleScreen;
		private UnityEngine.InputSystem.InputAction cancelAction;

		private struct GameEntry
		{
			public string displayName;
			public string scenePath;
		}

		private void Start()
		{
			buttonTemplate.SetActive(false);

			// Read at runtime rather than typed into the scene, so the label is always the
			// version this build was actually made with (see BuildScript's Version region).
			if (versionLabel != null)
			{
				versionLabel.text = "v" + Application.version;
			}

			GameObject firstButton = null;
			foreach (GameEntry entry in FindGameEntries())
			{
				GameObject button = CreateButton(entry.displayName, entry.scenePath);
				firstButton ??= button;
				if (inList && entry.scenePath == lastLaunched)
				{
					firstButton = button;
				}
			}

			var asset = Resources.Load<UnityEngine.InputSystem.InputActionAsset>("Input/CollectionInput");
			cancelAction = asset != null ? asset.FindActionMap("Global")?.FindAction("MenuCancel") : null;

			// Button navigation (Automatic, set on the template) and Submit/Cancel/Navigate
			// input (EventSystem's InputSystemUIInputModule) are already wired - the only
			// thing missing for gamepad/keyboard navigation to work at all is an initial
			// selection, since nothing is selected by default when a scene loads.
			if (firstButton != null && EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(firstButton);
			}

			lastSelected = firstButton;
			if (firstButton != null)
			{
				ScrollIntoView((RectTransform)firstButton.transform);
			}

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
			titleScreen = new MenuScreen { title = "Talovision", opaque = true };
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

		private void ShowList()
		{
			inList = true;
			Menus.CloseAll();
			if (titleLabel != null)
			{
				titleLabel.text = SaveManager.IsStoryMode
					? "Story mode, slot " + (SaveManager.StorySlotIndex + 1) + " (placeholder)"
					: "Just the games";
			}
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

			ShowList();
		}

		private static string PlayTime(float seconds)
		{
			int total = Mathf.FloorToInt(seconds);
			int hours = total / 3600;
			int minutes = total / 60 % 60;
			return hours > 0 ? hours + "h " + minutes.ToString("00") + "m" : minutes + "m " + (total % 60).ToString("00") + "s";
		}

		/// Keeps a game button selected so the pad can always drive the list.
		///
		/// Clicking away used to clear the selection outright and leave navigation with
		/// nothing to move from; the module's deselectOnBackgroundClick is off now, but
		/// selection can still be lost other ways (a click landing on a non-button, the
		/// selected object being disabled), and the result is the same dead list. Restoring
		/// the last game button covers all of them.
		private void Update()
		{
			// One of the screens is up: the list is not what is being used.
			if (Menus.IsOpen) return;

			// Back from the list is the first screen.
			if (cancelAction != null && cancelAction.WasPressedThisFrame())
			{
				ShowTitle();
				return;
			}

			EventSystem events = EventSystem.current;
			if (events == null) return;

			GameObject selected = events.currentSelectedGameObject;
			if (selected != null && selected.transform.parent == contentParent)
			{
				if (selected != lastSelected)
				{
					ScrollIntoView((RectTransform)selected.transform);
				}

				lastSelected = selected;
				return;
			}

			if (lastSelected != null && lastSelected.activeInHierarchy)
			{
				events.SetSelectedGameObject(lastSelected);
			}
		}

		/// Moves the list just far enough that the newly selected button is fully inside
		/// the viewport. Navigation moves the selection, not the scroll position, so once
		/// there are more games than fit on screen the pad would otherwise select buttons
		/// that cannot be seen.
		private void ScrollIntoView(RectTransform item)
		{
			ScrollRect scroll = contentParent.GetComponentInParent<ScrollRect>();
			if (scroll == null) return;

			RectTransform viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
			Canvas.ForceUpdateCanvases();
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, item);
			Rect view = viewport.rect;

			float shift = 0f;
			if (bounds.max.y > view.yMax)
			{
				shift = view.yMax - bounds.max.y;
			}
			else if (bounds.min.y < view.yMin)
			{
				shift = view.yMin - bounds.min.y;
			}

			if (shift != 0f)
			{
				scroll.StopMovement();
				scroll.content.anchoredPosition += new Vector2(0f, shift);
			}
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

		private GameObject CreateButton(string displayName, string scenePath)
		{
			GameObject buttonObject = Instantiate(buttonTemplate, contentParent);
			buttonObject.name = displayName;
			buttonObject.SetActive(true);

			Text label = buttonObject.GetComponentInChildren<Text>(true);
			if (label != null)
			{
				label.text = displayName;
			}

			Button button = buttonObject.GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				// Where it saves (a story slot, or the free-play area) was chosen on the way
				// to this list.
				lastLaunched = scenePath;
				SceneManager.LoadScene(scenePath);
			});

			return buttonObject;
		}
	}
}
