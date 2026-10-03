using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

namespace Collection.EditorTools
{
	/// <summary>
	/// Helpers for bringing a legacy Unity project folder into the collection (see
	/// "legacy unity projects/PORTING.md"). They are called from scripts, not from a menu:
	/// Import re-saves a freshly copied game folder as text and reports what the import
	/// lost; ConvertMaterials moves built-in lit materials to URP; Smoke plays a scene for a
	/// few seconds with a virtual gamepad and records errors and screenshots.
	/// </summary>
	public static class LegacyPortTools
	{
		const string ShotFolder = "Temp/claude-shots";

		/// <summary>
		/// Re-serializes everything under root, scans its prefabs and the given scenes for
		/// missing scripts and broken references, adds the scenes to Build Settings and
		/// returns a summary. Scene paths are relative to root.
		/// </summary>
		public static string Import(string root, params string[] scenes)
		{
			var sb = new StringBuilder();
			string[] paths = AssetDatabase.FindAssets("", new[] { root }).Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();
			AssetDatabase.ForceReserializeAssets(paths, ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
			AssetDatabase.SaveAssets();

			int missing = 0, broken = 0;
			var tags = new SortedDictionary<string, int>();
			var layers = new SortedDictionary<int, int>();
			var shaders = new SortedDictionary<string, int>();
			var sorting = new SortedDictionary<int, int>();
			var scripts = new SortedDictionary<string, int>();

			Action<GameObject, string> scan = null;
			scan = (go, where) =>
			{
				Count(tags, go.tag);
				Count(layers, go.layer);
				foreach (Component c in go.GetComponents<Component>())
				{
					if (c == null)
					{
						missing++;
						sb.AppendLine("MISSING script on " + where + "/" + go.name);
						continue;
					}
					if (c is MonoBehaviour) Count(scripts, c.GetType().Name);
					var r = c as Renderer;
					if (r != null)
					{
						foreach (Material m in r.sharedMaterials)
						{
							string name = m == null ? "null" : m.shader.name;
							if (m != null && AssetDatabase.GetAssetPath(m).StartsWith("Resources/unity_builtin")) name += " (built-in " + m.name + ")";
							Count(shaders, name);
						}
						Count(sorting, r.sortingLayerID);
					}
					var so = new SerializedObject(c);
					SerializedProperty p = so.GetIterator();
					while (p.NextVisible(true))
					{
						if (p.propertyType == SerializedPropertyType.ObjectReference && p.objectReferenceValue == null && p.objectReferenceInstanceIDValue != 0)
						{
							broken++;
							sb.AppendLine("BROKEN " + where + "/" + go.name + "." + c.GetType().Name + "." + p.propertyPath);
						}
					}
				}
				foreach (Transform child in go.transform) scan(child.gameObject, where);
			};

			foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { root }))
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				if (prefab != null) scan(prefab, Path.GetFileName(path));
			}

			var buildScenes = EditorBuildSettings.scenes.ToList();
			string firstGuid = "";
			foreach (string rel in scenes)
			{
				string scenePath = root + "/" + rel;
				Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
				if (firstGuid == "") firstGuid = AssetDatabase.AssetPathToGUID(scenePath);
				sb.AppendLine("=== " + rel + " ambient=" + RenderSettings.ambientLight + " fog=" + RenderSettings.fog);
				foreach (GameObject go in scene.GetRootGameObjects())
				{
					scan(go, rel);
					sb.AppendLine("ROOT " + go.name + (go.activeSelf ? "" : " (off)") + " tag=" + go.tag + " L" + go.layer + " pos=" + go.transform.position + " : " + Describe(go) + " children=" + go.transform.childCount);
				}
				foreach (Camera cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					sb.AppendLine("CAMERA " + cam.name + " ortho=" + cam.orthographic + " size=" + cam.orthographicSize + " clear=" + cam.clearFlags + " bg=" + cam.backgroundColor + " target=" + (cam.targetTexture ? cam.targetTexture.name : "screen"));
				}
				foreach (AudioSource a in UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					sb.AppendLine("AUDIO " + a.name + " clip=" + (a.clip ? a.clip.name : "none") + " loop=" + a.loop + " awake=" + a.playOnAwake);
				}
				foreach (Light l in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					sb.AppendLine("LIGHT " + l.name + " " + l.type + " i=" + l.intensity);
				}
				if (!buildScenes.Any(s => s.path == scenePath)) buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
			}
			EditorBuildSettings.scenes = buildScenes.ToArray();

			foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { root }))
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var m = AssetDatabase.LoadAssetAtPath<Material>(path);
				if (m != null && path.EndsWith(".mat")) sb.AppendLine("MAT " + path.Substring(root.Length + 1) + " " + m.shader.name);
			}

			sb.AppendLine("scripts: " + Join(scripts));
			sb.AppendLine("tags: " + Join(tags));
			sb.AppendLine("layers: " + Join(layers));
			sb.AppendLine("shaders: " + Join(shaders));
			sb.AppendLine("sortingLayers: " + Join(sorting));

			EditorSceneManager.OpenScene("Assets/Scenes/mainmenu.unity");
			AssetDatabase.SaveAssets();
			Directory.CreateDirectory(ShotFolder);
			File.WriteAllText(ShotFolder + "/scene_dump.txt", sb.ToString());
			return "reserialized " + paths.Length + " missing " + missing + " broken " + broken + " buildScenes " + EditorBuildSettings.scenes.Length + " firstSceneGuid " + firstGuid;
		}

		/// <summary>
		/// Moves the folder's Standard and Legacy Diffuse materials to URP/Lit, and gives
		/// renderers in the given scenes that sit on Unity's built-in default materials a
		/// material of the game's own (lit, or unlit grey when unlitDefault is set).
		/// </summary>
		public static string ConvertMaterials(string root, bool unlitDefault, params string[] scenes)
		{
			var sb = new StringBuilder();
			Shader lit = Shader.Find("Universal Render Pipeline/Lit");
			foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { root }))
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var m = AssetDatabase.LoadAssetAtPath<Material>(path);
				if (m == null || !path.EndsWith(".mat")) continue;
				string old = m.shader.name;
				if (old != "Standard" && old != "Legacy Shaders/Diffuse" && old != "Legacy Shaders/Specular" && old != "Legacy Shaders/Bumped Diffuse") continue;
				Texture tex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
				Vector2 scale = m.HasProperty("_MainTex") ? m.GetTextureScale("_MainTex") : Vector2.one;
				Vector2 offset = m.HasProperty("_MainTex") ? m.GetTextureOffset("_MainTex") : Vector2.zero;
				Color col = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
				float gloss = old == "Standard" ? m.GetFloat("_Glossiness") : 0f;
				float metal = old == "Standard" ? m.GetFloat("_Metallic") : 0f;
				float mode = old == "Standard" ? m.GetFloat("_Mode") : 0f;
				m.shader = lit;
				m.SetTexture("_BaseMap", tex);
				m.SetTextureScale("_BaseMap", scale);
				m.SetTextureOffset("_BaseMap", offset);
				m.SetColor("_BaseColor", col);
				m.SetFloat("_Smoothness", gloss);
				m.SetFloat("_Metallic", metal);
				if (old != "Standard")
				{
					m.SetFloat("_SpecularHighlights", 0f);
					m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
					m.SetFloat("_EnvironmentReflections", 0f);
					m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
				}
				if (mode >= 2f)
				{
					m.SetFloat("_Surface", 1f);
					m.SetFloat("_SrcBlend", 5f);
					m.SetFloat("_DstBlend", 10f);
					m.SetFloat("_ZWrite", 0f);
					m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					m.SetOverrideTag("RenderType", "Transparent");
					m.renderQueue = 3000;
				}
				EditorUtility.SetDirty(m);
				AssetDatabase.SaveAssetIfDirty(m);
				sb.AppendLine(Path.GetFileName(path) + ": " + old + " -> URP/Lit col=" + col + " tex=" + (tex ? tex.name : "none"));
			}

			string defPath = root + "/DefaultMat.mat";
			Material def = AssetDatabase.LoadAssetAtPath<Material>(defPath);
			int repointed = 0;
			foreach (string rel in scenes)
			{
				Scene scene = EditorSceneManager.OpenScene(root + "/" + rel, OpenSceneMode.Single);
				int n = 0;
				foreach (Renderer r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					Material[] mats = r.sharedMaterials;
					bool changed = false;
					for (int i = 0; i < mats.Length; i++)
					{
						if (mats[i] == null || !AssetDatabase.GetAssetPath(mats[i]).StartsWith("Resources/unity_builtin")) continue;
						string sn = mats[i].shader.name;
						if (sn != "Standard" && sn != "Legacy Shaders/Diffuse") continue;
						if (def == null)
						{
							def = new Material(unlitDefault ? Shader.Find("Universal Render Pipeline/Unlit") : lit);
							def.SetColor("_BaseColor", unlitDefault ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white);
							AssetDatabase.CreateAsset(def, defPath);
						}
						mats[i] = def;
						changed = true;
						n++;
					}
					if (changed)
					{
						r.sharedMaterials = mats;
						EditorUtility.SetDirty(r);
					}
				}
				if (n > 0)
				{
					EditorSceneManager.MarkSceneDirty(scene);
					EditorSceneManager.SaveScene(scene);
					repointed += n;
				}
			}
			AssetDatabase.SaveAssets();
			EditorSceneManager.OpenScene("Assets/Scenes/mainmenu.unity");
			sb.AppendLine("repointed " + repointed + " renderers from built-in default materials");
			return sb.ToString();
		}

		/// <summary>
		/// Must be called in play mode. Loads the scene, feeds a virtual gamepad a fixed
		/// pattern (stick circles, taps of every face button, triggers) for the given time,
		/// takes three screenshots, then returns to the main menu. Everything is logged to
		/// Temp/claude-shots/smoke_{tag}.txt, ending with a line starting DONE.
		/// With taps set to false only the sticks move (for games where a button ends them).
		/// </summary>
		public static string Smoke(string scenePath, float seconds, string tag, bool taps)
		{
			if (!EditorApplication.isPlaying) return "not playing";
			Directory.CreateDirectory(ShotFolder);
			string logPath = ShotFolder + "/smoke_" + tag + ".txt";
			File.WriteAllText(logPath, "start " + scenePath + "\n");
			Action<string> log = s => File.AppendAllText(logPath, s + "\n");

			Application.targetFrameRate = 60;
			Application.runInBackground = true;
			Gamepad pad = InputSystem.AddDevice<Gamepad>();
			var errors = new List<string>();
			Application.LogCallback onLog = (condition, stack, type) =>
			{
				if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) && errors.Count < 6)
				{
					string firstStack = stack == null ? "" : stack.Split('\n').FirstOrDefault(l => l.Contains("Games.")) ?? "";
					string line = condition + " | " + firstStack.Trim();
					if (!errors.Contains(line)) errors.Add(line);
				}
			};
			Application.logMessageReceived += onLog;
			int errorCount = 0;
			Application.LogCallback counter = (c, s, type) => { if (type == LogType.Error || type == LogType.Exception) errorCount++; };
			Application.logMessageReceived += counter;

			string gameFolder = Path.GetDirectoryName(scenePath).Replace('\\', '/');
			string gameRoot = string.Join("/", scenePath.Split('/').Take(3).ToArray());
			SceneManager.LoadScene(scenePath);

			float t0 = -1f;
			int shot = 0;
			int step = 0;
			float leftAt = 0f;
			string lastScene = "";
			int frames = 0;
			float fpsStart = 0f;
			EditorApplication.CallbackFunction tick = null;
			Action finish = () =>
			{
				Application.logMessageReceived -= onLog;
				Application.logMessageReceived -= counter;
				EditorApplication.update -= tick;
				if (pad.added) InputSystem.RemoveDevice(pad);
				Time.timeScale = 1f;
			};
			tick = () =>
			{
				if (!EditorApplication.isPlaying)
				{
					finish();
					return;
				}
				try
				{
					Scene scene = SceneManager.GetActiveScene();
					if (step == 0)
					{
						if (!scene.path.StartsWith(gameRoot))
						{
							if (t0 > 0f)
							{
								log("left the game by itself after " + (Time.unscaledTime - t0).ToString("F1") + "s, now " + scene.path);
								step = 1;
								leftAt = Time.realtimeSinceStartup;
							}
							return;
						}
						if (t0 < 0f)
						{
							t0 = Time.unscaledTime;
							fpsStart = Time.realtimeSinceStartup;
							frames = Time.frameCount;
						}
						if (scene.name != lastScene)
						{
							lastScene = scene.name;
							log((Time.unscaledTime - t0).ToString("F1") + "s scene " + scene.name);
						}
						float t = Time.unscaledTime - t0;
						var st = new GamepadState();
						if (t > 1.5f)
						{
							st.leftStick = new Vector2(Mathf.Cos(t * 0.9f), Mathf.Sin(t * 0.9f));
							st.rightStick = new Vector2(Mathf.Sin(t * 0.5f), Mathf.Cos(t * 0.7f) * 0.5f);
							if (taps)
							{
								float c = t % 5f;
								uint buttons = 0;
								if (c > 0.2f && c < 0.4f) buttons |= 1u << (int)GamepadButton.South;
								if (c > 1.2f && c < 1.4f) buttons |= 1u << (int)GamepadButton.West;
								if (c > 2.2f && c < 2.4f) buttons |= 1u << (int)GamepadButton.East;
								if (c > 3.2f && c < 3.4f) buttons |= 1u << (int)GamepadButton.North;
								if (c > 4.0f && c < 4.2f) buttons |= 1u << (int)GamepadButton.RightShoulder;
								st.buttons = buttons;
								st.rightTrigger = (c > 2.6f && c < 3.0f) ? 1f : 0f;
							}
						}
						InputSystem.QueueStateEvent(pad, st);
						float[] shotTimes = { 1.2f, seconds * 0.5f, seconds - 0.5f };
						if (shot < 3 && t > shotTimes[shot])
						{
							ScreenCapture.CaptureScreenshot(ShotFolder + "/smoke_" + tag + "_" + shot + ".png");
							shot++;
						}
						if (t > seconds)
						{
							float fps = (Time.frameCount - frames) / Mathf.Max(0.01f, Time.realtimeSinceStartup - fpsStart);
							log("played " + seconds + "s, fps " + fps.ToString("F0") + ", scene at end " + scene.name);
							InputSystem.QueueStateEvent(pad, new GamepadState());
							Collection.Controls.GlobalInputManager.ReturnToMainMenu();
							step = 1;
							leftAt = Time.realtimeSinceStartup;
						}
					}
					else if (Time.realtimeSinceStartup - leftAt > 1.5f)
					{
						Scene scene2 = SceneManager.GetActiveScene();
						int leftovers = 0;
						var probe = new GameObject("probe");
						UnityEngine.Object.DontDestroyOnLoad(probe);
						foreach (GameObject go in probe.scene.GetRootGameObjects())
						{
							foreach (MonoBehaviour mb in go.GetComponentsInChildren<MonoBehaviour>(true))
							{
								if (mb != null && mb.GetType().Namespace != null && mb.GetType().Namespace.StartsWith("Games."))
								{
									leftovers++;
									log("LEFTOVER " + go.name + " (" + mb.GetType().FullName + ")");
									break;
								}
							}
						}
						UnityEngine.Object.Destroy(probe);
						int audio = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None).Count(a => a.isPlaying && a.clip != null && AssetDatabase.GetAssetPath(a.clip).StartsWith(gameRoot));
						foreach (string e in errors) log("ERROR " + e);
						log("DONE scene=" + scene2.path + " errors=" + errorCount + " leftovers=" + leftovers + " gameAudioStillPlaying=" + audio + " gravity2D=" + Physics2D.gravity + " timeScale=" + Time.timeScale + " cursorVisible=" + Cursor.visible);
						finish();
					}
				}
				catch (Exception e)
				{
					log("BOT EXCEPTION " + e);
					log("DONE (bot failed)");
					finish();
				}
			};
			EditorApplication.update += tick;
			return "smoke test started: " + logPath;
		}

		/// <summary>
		/// Runs Smoke for several scenes one after another (play mode). scenes, tags and
		/// seconds are parallel arrays; taps applies to all.
		/// </summary>
		public static string SmokeMany(string[] scenes, float[] seconds, string[] tags, bool taps)
		{
			if (!EditorApplication.isPlaying) return "not playing";
			int index = 0;
			string current = "";
			EditorApplication.CallbackFunction driver = null;
			driver = () =>
			{
				if (!EditorApplication.isPlaying)
				{
					EditorApplication.update -= driver;
					return;
				}
				if (current != "")
				{
					string text = File.Exists(current) ? File.ReadAllText(current) : "";
					if (!text.Contains("DONE scene=") && !text.Contains("DONE (bot failed)")) return;
					current = "";
				}
				if (index >= scenes.Length)
				{
					File.WriteAllText(ShotFolder + "/smoke_all_done.txt", "ALLDONE");
					EditorApplication.update -= driver;
					return;
				}
				Smoke(scenes[index], seconds[index], tags[index], taps);
				current = ShotFolder + "/smoke_" + tags[index] + ".txt";
				index++;
			};
			if (File.Exists(ShotFolder + "/smoke_all_done.txt")) File.Delete(ShotFolder + "/smoke_all_done.txt");
			EditorApplication.update += driver;
			return "started " + scenes.Length + " smoke tests";
		}

		static string Describe(GameObject go)
		{
			return string.Join(",", go.GetComponents<Component>().Select(c => c == null ? "MISSING" : c.GetType().Name).Where(n => n != "Transform").ToArray());
		}

		static void Count<T>(SortedDictionary<T, int> d, T key)
		{
			int n;
			d.TryGetValue(key, out n);
			d[key] = n + 1;
		}

		static string Join<T>(SortedDictionary<T, int> d)
		{
			return string.Join(", ", d.Select(k => k.Key + "=" + k.Value).ToArray());
		}
	}
}
