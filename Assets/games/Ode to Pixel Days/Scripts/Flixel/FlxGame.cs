using System;
using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// One thing the game draws: a GameObject with a renderer on it, owned by an FlxSprite,
	/// FlxText or FlxTilemap. Flixel objects are plain C# objects and repaint themselves
	/// from scratch every frame; this is what makes that look like retained Unity
	/// rendering - a thing presented this frame is shown, on top of everything presented
	/// before it, and a thing not presented is hidden.
	/// </summary>
	public class FlxRenderer
	{
		public GameObject gameObject;
		public Renderer renderer;
		public SpriteRenderer sprite;

		internal int drawnFrame = -1;

		public bool Alive
		{
			get { return gameObject != null; }
		}

		/// (screenX, screenY) is where the object's pivot goes, in game pixels from the
		/// top-left of the screen, Y down.
		public void Present(double screenX, double screenY, double scaleX, double scaleY, bool flipX, Color color)
		{
			Transform transform = gameObject.transform;
			transform.localPosition = new Vector3(
				(float)(screenX / FlxGame.PixelsPerUnit),
				(float)(-screenY / FlxGame.PixelsPerUnit),
				0f);
			transform.localScale = new Vector3((float)scaleX, (float)scaleY, 1f);

			if (sprite != null)
			{
				sprite.flipX = flipX;
				sprite.color = color;
			}

			renderer.sortingOrder = FlxGame.NextOrder();
			if (!renderer.enabled)
			{
				renderer.enabled = true;
			}

			drawnFrame = FlxGame.DrawFrame;
		}

		public void Destroy()
		{
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}

			gameObject = null;
			renderer = null;
			sprite = null;
		}
	}

	/// <summary>
	/// org.flixel.FlxGame: the loop.
	///
	/// Flixel advances the game in fixed steps - 16 ms each, however long a frame took -
	/// and the game is written against that: it counts steps for the jump boost, for
	/// fades, for every TTimer. So this accumulates real time and runs as many steps as
	/// fit, then draws once. The cap on the accumulator is Flixel's too: after a long
	/// hitch the game resumes rather than racing to catch up.
	///
	/// The game renders at its own resolution into a texture, which is then stretched to
	/// the window with hard pixels and bars at the sides. That is what keeps a sprite
	/// scaled up 16x on the same pixel grid as everything else, as it was in Flash.
	/// </summary>
	public class FlxGame : MonoBehaviour
	{
		public const float PixelsPerUnit = 100f;

		internal FlxState _state;
		internal FlxState _requestedState;

		private Func<FlxState> _iState;
		private bool _created;
		private double _step;
		private double _accumulator;
		private double _maxAccumulation;

		private static FlxGame instance;
		private static int drawOrder;
		private static int drawFrame;

		private Transform displayRoot;
		private Transform audioRoot;
		private Camera gameCamera;
		private RenderTexture target;
		private RawImage screen;
		private RectTransform screenRect;
		private readonly List<FlxRenderer> renderers = new List<FlxRenderer>();

		// In the collection: button prompts, drawn over the picture at the window's
		// resolution (see FlxPrompt).
		private InputPromptOverlay prompts;

		internal static int DrawFrame
		{
			get { return drawFrame; }
		}

		internal static int NextOrder()
		{
			return drawOrder++;
		}

		/// <summary>
		/// FlxGame's constructor. Zoom is not needed: it was how large Flash drew each game
		/// pixel, and here the picture is fitted to the window instead.
		/// </summary>
		protected void Init(int GameSizeX, int GameSizeY, Func<FlxState> InitialState, double Zoom = 1, uint GameFramerate = 60, uint FlashFramerate = 30)
		{
			instance = this;

			// Whole milliseconds, as in the source: 1000 / 60 is 16, not 16.67.
			_step = Math.Floor(1000.0 / GameFramerate);
			_maxAccumulation = Math.Floor(2000.0 / FlashFramerate) - 1;
			_accumulator = _step;

			FlxG.init(this, GameSizeX, GameSizeY);
			FlxG.elapsed = FlxG.timeScale * (_step / 1000.0);

			buildDisplay(GameSizeX, GameSizeY);

			_state = null;
			_iState = InitialState;
			_requestedState = _iState();
			_created = true;
		}

		private void buildDisplay(int width, int height)
		{
			displayRoot = new GameObject("Display").transform;
			displayRoot.SetParent(transform, false);

			audioRoot = new GameObject("Audio").transform;
			audioRoot.SetParent(transform, false);

			target = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			target.filterMode = FilterMode.Point;
			target.wrapMode = TextureWrapMode.Clamp;
			target.Create();

			// Looks at the rectangle (0,0)-(width,-height): screen pixel (x, y) is the world
			// point (x, -y) / PixelsPerUnit, so drawing never needs the camera to move.
			var cameraObject = new GameObject("Game Camera");
			cameraObject.transform.SetParent(transform, false);
			cameraObject.transform.localPosition = new Vector3(width / 2f / PixelsPerUnit, -height / 2f / PixelsPerUnit, -10f);
			gameCamera = cameraObject.AddComponent<Camera>();
			gameCamera.orthographic = true;
			gameCamera.orthographicSize = height / 2f / PixelsPerUnit;
			gameCamera.clearFlags = CameraClearFlags.SolidColor;
			gameCamera.backgroundColor = Color.black;
			gameCamera.nearClipPlane = 0.1f;
			gameCamera.farClipPlane = 100f;
			gameCamera.targetTexture = target;

			var canvasObject = new GameObject("Screen", typeof(Canvas));
			canvasObject.transform.SetParent(transform, false);
			Canvas canvas = canvasObject.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 0;

			var imageObject = new GameObject("Picture", typeof(RectTransform), typeof(RawImage));
			imageObject.transform.SetParent(canvasObject.transform, false);
			screen = imageObject.GetComponent<RawImage>();
			screen.texture = target;
			screen.raycastTarget = false;
			screenRect = (RectTransform)imageObject.transform;
			screenRect.anchorMin = screenRect.anchorMax = screenRect.pivot = new Vector2(0.5f, 0.5f);
			fitScreen();
			prompts = new InputPromptOverlay(screenRect, width, height);
		}

		/// The largest size that fits the window without changing the picture's shape.
		private void fitScreen()
		{
			float gameAspect = (float)FlxG.width / FlxG.height;
			float windowAspect = (float)Screen.width / Screen.height;
			Vector2 size = windowAspect > gameAspect
				? new Vector2(Screen.height * gameAspect, Screen.height)
				: new Vector2(Screen.width, Screen.width / gameAspect);

			float scaleFactor = screen.canvas != null ? screen.canvas.scaleFactor : 1f;
			size /= scaleFactor;
			if (screenRect.sizeDelta != size)
			{
				screenRect.sizeDelta = size;
			}
		}

		/// Where a point of the window is in game pixels, origin top-left.
		private Vector2 windowToGame(Vector2 windowPosition)
		{
			float gameAspect = (float)FlxG.width / FlxG.height;
			float windowAspect = (float)Screen.width / Screen.height;
			float pictureWidth = windowAspect > gameAspect ? Screen.height * gameAspect : Screen.width;
			float pictureHeight = windowAspect > gameAspect ? Screen.height : Screen.width / gameAspect;
			float left = (Screen.width - pictureWidth) * 0.5f;
			float bottom = (Screen.height - pictureHeight) * 0.5f;

			return new Vector2(
				(windowPosition.x - left) / pictureWidth * FlxG.width,
				(1f - (windowPosition.y - bottom) / pictureHeight) * FlxG.height);
		}

		internal static FlxRenderer NewRenderer(string name, Type extraComponent = null)
		{
			var flxRenderer = new FlxRenderer();
			flxRenderer.gameObject = extraComponent != null ? new GameObject(name, extraComponent) : new GameObject(name);
			flxRenderer.gameObject.transform.SetParent(instance.displayRoot, false);

			// A text field brings its own renderer once its component is added.
			if (extraComponent == null)
			{
				flxRenderer.sprite = flxRenderer.gameObject.AddComponent<SpriteRenderer>();
				flxRenderer.renderer = flxRenderer.sprite;
			}

			instance.renderers.Add(flxRenderer);
			return flxRenderer;
		}

		internal static InputPromptOverlay.Label NewPromptLabel()
		{
			return instance.prompts.NewLabel();
		}

		internal static AudioSource NewAudioSource()
		{
			var go = new GameObject("FlxSound");
			go.transform.SetParent(instance.audioRoot, false);
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 0f;
			return source;
		}

		private void Update()
		{
			if (!_created)
			{
				return;
			}

			FlxG.keys.poll();
			FlxG.mouse.poll(windowToGame(TaloketoInputManager.mousePosition), FlxG.camera);

			_accumulator += Time.unscaledDeltaTime * 1000.0;
			if (_accumulator > _maxAccumulation)
			{
				_accumulator = _maxAccumulation;
			}

			while (_accumulator >= _step)
			{
				step();
				_accumulator = _accumulator - _step;
			}

			draw();
		}

		/// FlxGame.step() and update() together.
		private void step()
		{
			if (_state != _requestedState)
			{
				switchState();
			}

			// FlxG.updateInput()
			FlxG.keys.update();
			FlxG.mouse.update();

			FlxG.elapsed = FlxG.timeScale * (_step / 1000.0);
			FlxTimerManager.update(); // FlxG.updatePlugins()
			_state.update();
			FlxG.updateCameras();
		}

		private void switchState()
		{
			// Basic reset stuff
			FlxG.resetCameras();
			FlxG.resetInput();
			FlxG.destroySounds();

			// Clear the timers
			FlxTimerManager.clear();

			// Destroy the old state (if there is an old state)
			if (_state != null)
			{
				_state.destroy();
			}

			// Anything the old state drew and did not clean up goes with it.
			for (int i = 0; i < renderers.Count; i++)
			{
				renderers[i].Destroy();
			}

			renderers.Clear();
			prompts.Clear();

			// Finally assign and create the new state
			_state = _requestedState;
			_state.create();
		}

		private void draw()
		{
			drawFrame++;
			drawOrder = 0;

			fitScreen();
			gameCamera.backgroundColor = toColor(FlxG.bgColor);
			prompts.BeginFrame();

			if (_state != null)
			{
				_state.draw();
			}

			prompts.EndFrame();

			// Whatever did not draw itself this frame is not on screen.
			for (int i = renderers.Count - 1; i >= 0; i--)
			{
				FlxRenderer flxRenderer = renderers[i];
				if (!flxRenderer.Alive)
				{
					renderers.RemoveAt(i);
					continue;
				}

				if (flxRenderer.renderer != null && flxRenderer.drawnFrame != drawFrame && flxRenderer.renderer.enabled)
				{
					flxRenderer.renderer.enabled = false;
				}
			}
		}

		private static Color toColor(uint argb)
		{
			return new Color(
				((argb >> 16) & 0xff) / 255f,
				((argb >> 8) & 0xff) / 255f,
				(argb & 0xff) / 255f,
				1f);
		}

		private void OnDestroy()
		{
			if (_state != null)
			{
				_state.destroy();
				_state = null;
			}

			FlxG.shutdown();
			FlxAssets.Clear();

			if (target != null)
			{
				if (gameCamera != null)
				{
					gameCamera.targetTexture = null;
				}

				target.Release();
				Destroy(target);
			}

			if (instance == this)
			{
				instance = null;
			}
		}
	}
}
