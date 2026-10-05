using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Collection.EditorTools
{
	/// <summary>
	/// Looks through games' scenes for things that a 16:9 screen shows and a 16:10 one (a
	/// Steam Deck) does not: Unity keeps a camera's height, so a narrower screen loses the
	/// sides. For each scene's cameras it lists the renderers that are wholly on screen at
	/// 16:9 with their middle off screen at 16:10 (GONE) or their edge (CUT).
	///
	/// A guide for choosing which games get GameList's aspect set, not a verdict: it sees
	/// scenes as they are saved, not what a game spawns or where its camera goes.
	/// </summary>
	public static class AspectAudit
	{
		private const float Wide = 16f / 9f;
		private const float Narrow = 16f / 10f;

		public static string Run(int firstGame, int gameCount)
		{
			var report = new StringBuilder();
			string[] games = Directory.GetDirectories("Assets/games");
			for (int g = firstGame; g < games.Length && g < firstGame + gameCount; g++)
			{
				string game = Path.GetFileName(games[g]);
				report.Append("## ").Append(game).Append('\n');
				foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { games[g].Replace('\\', '/') }))
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
					string found = Scene();
					if (found.Contains("GONE") || found.Contains("CUT"))
					{
						report.Append(Path.GetFileNameWithoutExtension(path)).Append(": ").Append(found).Append('\n');
					}
				}
			}

			return report.ToString();
		}

		private static string Scene()
		{
			var line = new StringBuilder();
			foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
			{
				if (!camera.enabled || camera.targetTexture != null) continue;

				float before = camera.aspect;
				camera.aspect = Wide;

				// How much of the 16:9 width a 16:10 screen still shows, around the middle.
				float kept = Narrow / Wide;
				float low = 0.5f - kept * 0.5f, high = 0.5f + kept * 0.5f;

				var cut = new List<string>();
				var gone = new List<string>();
				foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
				{
					if (!renderer.enabled || (camera.cullingMask & (1 << renderer.gameObject.layer)) == 0) continue;

					Bounds bounds = renderer.bounds;
					float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
					bool inFront = false;
					for (int i = 0; i < 8; i++)
					{
						Vector3 corner = bounds.center + Vector3.Scale(bounds.extents,
							new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
						Vector3 view = camera.WorldToViewportPoint(corner);
						if (view.z <= 0f) continue;
						inFront = true;
						minX = Mathf.Min(minX, view.x);
						maxX = Mathf.Max(maxX, view.x);
						minY = Mathf.Min(minY, view.y);
						maxY = Mathf.Max(maxY, view.y);
					}

					// On screen at 16:9 with its middle in view, and not as wide as a background.
					float middle = (minX + maxX) * 0.5f;
					if (!inFront || maxX < 0f || minX > 1f || maxY < 0f || minY > 1f) continue;
					if (middle < 0f || middle > 1f || maxX - minX > 0.5f) continue;

					if (middle < low || middle > high) gone.Add(renderer.name);
					else if (minX < low || maxX > high) cut.Add(renderer.name);
				}

				camera.aspect = before;
				camera.ResetAspect();

				line.Append(camera.orthographic ? "[ortho " : "[persp ").Append(camera.name).Append("] ");
				if (gone.Count > 0) line.Append("GONE ").Append(gone.Count).Append(": ").Append(Names(gone)).Append(' ');
				if (cut.Count > 0) line.Append("CUT ").Append(cut.Count).Append(": ").Append(Names(cut)).Append(' ');
			}

			return line.ToString();
		}

		private static string Names(List<string> names)
		{
			var unique = new List<string>();
			foreach (string name in names)
			{
				if (!unique.Contains(name)) unique.Add(name);
				if (unique.Count == 6) break;
			}

			return string.Join(", ", unique);
		}
	}
}
