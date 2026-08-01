using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
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

			foreach (GameEntry entry in FindGameEntries())
			{
				CreateButton(entry.displayName, entry.scenePath);
			}
		}

		private static IEnumerable<GameEntry> FindGameEntries()
		{
			var seenFolders = new HashSet<string>();

			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);
				string folderName = GameContext.FromScenePath(path);
				if (folderName == null)
				{
					continue;
				}

				if (!seenFolders.Add(folderName))
				{
					continue;
				}

				yield return new GameEntry { displayName = Capitalize(folderName), scenePath = path };
			}
		}

		private static string Capitalize(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			return char.ToUpperInvariant(value[0]) + value.Substring(1);
		}

		private void CreateButton(string displayName, string scenePath)
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
		}
	}
}
