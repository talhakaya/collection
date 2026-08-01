using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.MainMenu
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private RectTransform contentParent;
		[SerializeField] private GameObject buttonTemplate;

		private struct GameEntry
		{
			public string displayName;
			public string scenePath;
		}

		private void Start()
		{
			buttonTemplate.SetActive(false);

			GameObject firstButton = null;
			foreach (GameEntry entry in FindGameEntries())
			{
				GameObject button = CreateButton(entry.displayName, entry.scenePath);
				firstButton ??= button;
			}

			// Button navigation (Automatic, set on the template) and Submit/Cancel/Navigate
			// input (EventSystem's InputSystemUIInputModule) are already wired - the only
			// thing missing for gamepad/keyboard navigation to work at all is an initial
			// selection, since nothing is selected by default when a scene loads.
			if (firstButton != null && EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(firstButton);
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
			button.onClick.AddListener(() => SceneManager.LoadScene(scenePath));

			return buttonObject;
		}
	}
}
