using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;

namespace Games.GarbagePeople
{
	/// <summary>
	/// A Phaser 3 scene, for what the game's scenes use: the display list and the factories
	/// that add to it (add, physics.add), tweens, the clock, the camera, the keyboard,
	/// sounds, the game-wide animations, and starting another scene.
	///
	/// As in Phaser, a scene object lives for the whole game: starting it again runs
	/// create() again on the same object.
	/// </summary>
	public abstract class Scene
	{
		public readonly string key;
		internal readonly List<GameObject> displayList = new List<GameObject>();

		public readonly Factory add;
		public readonly PhysicsPlugin physics;
		public readonly TweenManager tweens = new TweenManager();
		public readonly Clock time = new Clock();
		public readonly CameraManager cameras = new CameraManager();
		public readonly PhaserInput input = new PhaserInput();
		public readonly SoundPlugin sound = new SoundPlugin();
		public readonly AnimationManager anims = new AnimationManager();
		public readonly ScenePlugin scene;

		protected Scene(string key)
		{
			this.key = key;
			add = new Factory(this);
			physics = new PhysicsPlugin(this);
			scene = new ScenePlugin();
		}

		public virtual void create()
		{
		}

		public virtual void update()
		{
		}

		/// Shutting down: everything the scene made goes, but sounds play on.
		internal void shutdown()
		{
			foreach (GameObject o in displayList.ToArray())
			{
				o.destroy();
			}

			displayList.Clear();
			tweens.clear();
			time.clear();
			input.clear();
			cameras.main = new Camera();
		}

		internal T addObject<T>(T o) where T : GameObject
		{
			displayList.Add(o);
			return o;
		}
	}

	public class Factory
	{
		private readonly Scene scene;

		internal Factory(Scene scene)
		{
			this.scene = scene;
		}

		public Sprite image(double x, double y, string key)
		{
			return scene.addObject(new Sprite(scene, x, y, key));
		}

		public Sprite sprite(double x, double y, string key)
		{
			return scene.addObject(new Sprite(scene, x, y, key));
		}

		public Text text(double x, double y, string text, TextStyle style = null)
		{
			return scene.addObject(new Text(scene, x, y, text, style));
		}
	}

	/// <summary>
	/// The Arcade physics the game uses: sprites with a velocity, moved by it. The game
	/// turns on collision with the world's edges but never gives a body collideWorldBounds,
	/// so nothing ever collides.
	/// </summary>
	public class PhysicsPlugin
	{
		private readonly Scene scene;
		public readonly PhysicsWorld world = new PhysicsWorld();
		public readonly PhysicsFactory add;

		internal PhysicsPlugin(Scene scene)
		{
			this.scene = scene;
			add = new PhysicsFactory(scene);
		}

		internal void step(double seconds)
		{
			foreach (GameObject o in scene.displayList)
			{
				var s = o as Sprite;
				if (s != null && s.hasBody)
				{
					s.x += s.velocityX * seconds;
					s.y += s.velocityY * seconds;
				}
			}
		}
	}

	public class PhysicsWorld
	{
		public void setBoundsCollision(bool left, bool right, bool up, bool down)
		{
		}
	}

	public class PhysicsFactory
	{
		private readonly Scene scene;

		internal PhysicsFactory(Scene scene)
		{
			this.scene = scene;
		}

		public Sprite sprite(double x, double y, string key)
		{
			Sprite s = scene.addObject(new Sprite(scene, x, y, key));
			s.hasBody = true;
			return s;
		}
	}

	public class CameraManager
	{
		public Camera main = new Camera();
	}

	public class AnimationManager
	{
		/// Animations belong to the game; a name defined already keeps its first definition.
		public void create(AnimationConfig config)
		{
			if (!SpriteAnimations.Defined.ContainsKey(config.key))
			{
				SpriteAnimations.Defined[config.key] = config;
			}
		}
	}

	public class ScenePlugin
	{
		/// Stops this scene and starts another, from the next step.
		public void start(string key)
		{
			PhaserGame.Start(key);
		}
	}

	/// <summary>
	/// The game (Phaser.Game): a 1280x720 canvas, the scenes, and the loop.
	///
	/// Each step - sixty a second, as on the 60 Hz screen the game ran on - polls the keys,
	/// moves tweens, timers, physics bodies and animations on, then calls the scene's
	/// update, in Phaser's order. The game counts frames for its hit rate, so it runs on
	/// steps rather than on Unity's frame rate.
	///
	/// The art is drawn at full resolution and smooth: the camera draws straight to the
	/// screen, as large as the 16:9 picture fits, with bars where it does not.
	/// </summary>
	public class PhaserGame : MonoBehaviour
	{
		public const int WIDTH = 1280;
		public const int HEIGHT = 720;
		public const float PixelsPerUnit = 100f;
		private const double StepMs = 1000.0 / 60.0;

		private static PhaserGame instance;
		private static string pending;

		private readonly Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
		private Scene active;
		private UnityEngine.Camera unityCamera;
		private Transform displayRoot;
		private Transform audioRoot;
		private double accumulator;

		internal static Transform DisplayRoot
		{
			get { return instance.displayRoot; }
		}

		internal static Vector3 ToUnity(double screenX, double screenY)
		{
			return new Vector3((float)(screenX / PixelsPerUnit), (float)(-screenY / PixelsPerUnit), 0f);
		}

		internal static AudioSource NewAudioSource(string name)
		{
			var go = new UnityEngine.GameObject(name);
			go.transform.SetParent(instance.audioRoot, false);
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 0f;
			return source;
		}

		internal static void Start(string key)
		{
			pending = key;
		}

		protected void Run(Scene[] sceneList)
		{
			instance = this;
			SpriteAnimations.Defined.Clear();
			GlobalInputManager.HideGameCursor();

			displayRoot = new UnityEngine.GameObject("Display").transform;
			displayRoot.SetParent(transform, false);
			audioRoot = new UnityEngine.GameObject("Audio").transform;
			audioRoot.SetParent(transform, false);

			var cameraObject = new UnityEngine.GameObject("Phaser Camera");
			cameraObject.transform.SetParent(transform, false);
			cameraObject.transform.localPosition = new Vector3(WIDTH / 2f / PixelsPerUnit, -HEIGHT / 2f / PixelsPerUnit, -10f);
			unityCamera = cameraObject.AddComponent<UnityEngine.Camera>();
			unityCamera.orthographic = true;
			unityCamera.orthographicSize = HEIGHT / 2f / PixelsPerUnit;
			unityCamera.clearFlags = CameraClearFlags.SolidColor;
			unityCamera.backgroundColor = Color.black;
			unityCamera.nearClipPlane = 0.1f;
			unityCamera.farClipPlane = 100f;
			unityCamera.depth = 10;

			foreach (Scene s in sceneList)
			{
				scenes[s.key] = s;
			}

			// The first scene in the list starts.
			pending = sceneList[0].key;
		}

		private void Update()
		{
			fitViewport();

			accumulator += Time.unscaledDeltaTime * 1000.0;
			if (accumulator > StepMs * 4)
			{
				accumulator = StepMs * 4;
			}

			while (accumulator >= StepMs)
			{
				accumulator -= StepMs;
				step();
			}

			render();
			Sound.Tidy();
		}

		private void step()
		{
			if (pending != null)
			{
				string key = pending;
				pending = null;
				if (active != null)
				{
					active.shutdown();
				}

				active = scenes[key];
				active.create();
			}

			if (active == null)
			{
				return;
			}

			active.input.poll();
			active.tweens.update(StepMs);
			active.time.update(StepMs);
			active.physics.step(StepMs / 1000.0);
			foreach (GameObject o in active.displayList.ToArray())
			{
				o.preUpdate(StepMs);
			}

			active.update();
		}

		private void render()
		{
			if (active == null)
			{
				return;
			}

			Camera camera = active.cameras.main;
			camera.preRender();
			for (int i = 0; i < active.displayList.Count; i++)
			{
				active.displayList[i].render(i * 2, camera);
			}
		}

		/// The 16:9 picture, as large as fits, centred.
		private void fitViewport()
		{
			float aspect = (float)WIDTH / HEIGHT;
			float windowAspect = (float)Screen.width / Screen.height;
			Rect rect = windowAspect > aspect
				? new Rect((1f - aspect / windowAspect) / 2f, 0f, aspect / windowAspect, 1f)
				: new Rect(0f, (1f - windowAspect / aspect) / 2f, 1f, windowAspect / aspect);
			if (unityCamera.rect != rect)
			{
				unityCamera.rect = rect;
			}
		}

		private void OnDestroy()
		{
			Sound.StopAll();
			SpriteAnimations.Defined.Clear();
			if (instance == this)
			{
				instance = null;
			}
		}
	}

	/// The game's images and sounds, by their Phaser keys; see AssetTable.
	public static class PhaserAssets
	{
		private const string Root = "GarbagePeople/";

		private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
		private static readonly Dictionary<Texture2D, UnityEngine.Sprite> sprites = new Dictionary<Texture2D, UnityEngine.Sprite>();
		private static readonly Dictionary<KeyValuePair<Texture2D, BlendModes>, Material> materials = new Dictionary<KeyValuePair<Texture2D, BlendModes>, Material>();

		public static Texture2D GetTexture(string key)
		{
			Texture2D texture;
			if (textures.TryGetValue(key, out texture) && texture != null)
			{
				return texture;
			}

			string path;
			if (!AssetTable.Images.TryGetValue(key, out path))
			{
				Debug.LogError("GarbagePeople: no image called '" + key + "'.");
				return null;
			}

			texture = Resources.Load<Texture2D>(Root + path);
			textures[key] = texture;
			return texture;
		}

		internal static UnityEngine.Sprite GetSprite(Texture2D texture)
		{
			UnityEngine.Sprite sprite;
			if (!sprites.TryGetValue(texture, out sprite) || sprite == null)
			{
				sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PhaserGame.PixelsPerUnit);
				sprites[texture] = sprite;
			}

			return sprite;
		}

		public static AudioClip GetSound(string key)
		{
			string path;
			if (!AssetTable.Sounds.TryGetValue(key, out path))
			{
				Debug.LogError("GarbagePeople: no sound called '" + key + "'.");
				return null;
			}

			return Resources.Load<AudioClip>(Root + path);
		}

		/// <summary>
		/// The material to draw a texture with, one per texture and blend mode. With one
		/// material shared by all sprites, Unity drew some sprites with another sprite's
		/// texture (everything after the sky in the first level's city, in the sky's colours).
		/// </summary>
		internal static Material Material(Texture2D texture, BlendModes mode)
		{
			var key = new KeyValuePair<Texture2D, BlendModes>(texture, mode);
			Material material;
			if (!materials.TryGetValue(key, out material) || material == null)
			{
				Shader shader = mode == BlendModes.ADD
					? Resources.Load<Shader>(Root + "AdditiveSprite")
					: Shader.Find("Sprites/Default");
				material = new Material(shader) { mainTexture = texture };
				materials[key] = material;
			}

			return material;
		}
	}
}
