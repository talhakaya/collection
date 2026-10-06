using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.UI
{
	/// <summary>
	/// A black screen saying "Loading", for scene changes that take long enough to be seen:
	/// into the story, from the story into a game and back.
	///
	/// Loading a scene holds the picture still until it is done, so whatever was on the
	/// screen at that moment is what is looked at meanwhile (the list of games behind the
	/// main menu, on choosing a story slot). This goes up first, has a frame to be drawn,
	/// and comes down once the new scene has drawn its first frame.
	/// </summary>
	public static class LoadingScreen
	{
		private static Runner runner;
		private static bool loading;

		public static bool Loading => loading;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			runner = null;
			loading = false;
		}

		/// Loads a scene under the loading screen. A second call while one is under way is
		/// ignored.
		public static void Load(string scenePath)
		{
			if (loading) return;

			if (runner == null)
			{
				runner = Build();
			}

			loading = true;
			runner.gameObject.SetActive(true);
			runner.StartCoroutine(runner.Load(scenePath));
		}

		private static Runner Build()
		{
			var go = new GameObject(nameof(LoadingScreen), typeof(Canvas), typeof(CanvasScaler));
			Object.DontDestroyOnLoad(go);

			Canvas canvas = go.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			// Over everything but the cursor.
			canvas.sortingOrder = short.MaxValue - 2;

			CanvasScaler scaler = go.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 1f;

			var black = new GameObject("Black", typeof(RectTransform), typeof(Image));
			black.transform.SetParent(go.transform, false);
			var blackRect = (RectTransform)black.transform;
			blackRect.anchorMin = Vector2.zero;
			blackRect.anchorMax = Vector2.one;
			blackRect.offsetMin = blackRect.offsetMax = Vector2.zero;
			black.GetComponent<Image>().color = Color.black;

			var label = new GameObject("Label", typeof(RectTransform));
			label.transform.SetParent(go.transform, false);
			TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
			text.text = "Loading";
			text.fontSize = 30f;
			text.color = Color.white;
			text.alignment = TextAlignmentOptions.Center;
			text.raycastTarget = false;
			var labelRect = (RectTransform)label.transform;
			labelRect.anchorMin = labelRect.anchorMax = new Vector2(0.5f, 0.5f);
			labelRect.sizeDelta = new Vector2(600f, 80f);

			return go.AddComponent<Runner>();
		}

		private class Runner : MonoBehaviour
		{
			public IEnumerator Load(string scenePath)
			{
				// A frame for the screen to be drawn before the picture stands still. (Frames,
				// not seconds: time may be stopped.)
				yield return null;
				yield return null;

				SceneManager.LoadScene(scenePath);

				// And the new scene's first frames under it: things are still finding their
				// places.
				yield return null;
				yield return null;
				yield return null;

				loading = false;
				gameObject.SetActive(false);
			}
		}
	}
}
