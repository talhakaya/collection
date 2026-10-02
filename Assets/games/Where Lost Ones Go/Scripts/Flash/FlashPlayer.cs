using System;
using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// What the Flash Player did for the game: run it at 30 frames a second on a 640x360
	/// stage, with the mouse.
	///
	/// Each frame, in order: a click, if there was one, goes to the listeners; the
	/// ENTER_FRAME listeners run in the order they were added; timers and tweens move on by
	/// one frame's time; then the display list is drawn. Everything runs on frame time - a
	/// thirtieth of a second per frame - so the piece takes the same time however fast the
	/// machine is, and a slow frame catches up rather than stretching it.
	///
	/// The stage is rendered at its own size into a texture, then shown as large as fits
	/// the window with hard pixels: the game zooms twenty times into a photo, and was asked
	/// to look pixelated doing it, not smoothed.
	///
	/// The mouse is whatever TaloketoInputManager says it is, so the collection's mouse
	/// emulation drives it from a gamepad.
	/// </summary>
	public class FlashPlayer : MonoBehaviour
	{
		public const int StageWidth = 640;
		public const int StageHeight = 360;
		public const float FrameRate = 30f;
		public const float PixelsPerUnit = 100f;

		public static FlashStage stage;
		public static DisplayObjectContainer root;

		private static readonly List<Action> enterFrameListeners = new List<Action>();
		private static FlashPlayer instance;

		private FlashRenderer renderer;
		private RenderTexture target;
		private Camera stageCamera;
		private RawImage screen;
		private RectTransform screenRect;
		private double accumulator;
		private bool clickPending;

		internal static Transform DisplayRoot
		{
			get { return instance.renderer.root; }
		}

		internal static AudioSource NewAudioSource()
		{
			var go = new GameObject("Sound");
			go.transform.SetParent(instance.transform, false);
			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 0f;
			return source;
		}

		internal static void AddEnterFrame(Action listener)
		{
			if (!enterFrameListeners.Contains(listener))
			{
				enterFrameListeners.Add(listener);
			}
		}

		internal static void RemoveEnterFrame(Action listener)
		{
			enterFrameListeners.Remove(listener);
		}

		/// Starts the movie with its document class.
		protected void Run(Func<DisplayObjectContainer> documentClass)
		{
			instance = this;
			enterFrameListeners.Clear();
			FlashTimer.Reset();
			TweenLite.Reset();

			buildDisplay();
			GlobalInputManager.ClearGameCursor();

			stage = new FlashStage();
			pollMouse();
			root = documentClass();
		}

		private void buildDisplay()
		{
			target = new RenderTexture(StageWidth, StageHeight, 0, RenderTextureFormat.ARGB32);
			target.filterMode = FilterMode.Point;
			target.Create();

			var cameraObject = new GameObject("Stage Camera");
			cameraObject.transform.SetParent(transform, false);
			cameraObject.transform.localPosition = new Vector3(StageWidth / 2f / PixelsPerUnit, -StageHeight / 2f / PixelsPerUnit, -10f);
			stageCamera = cameraObject.AddComponent<Camera>();
			stageCamera.orthographic = true;
			stageCamera.orthographicSize = StageHeight / 2f / PixelsPerUnit;
			stageCamera.clearFlags = CameraClearFlags.SolidColor;
			stageCamera.backgroundColor = Color.white;
			stageCamera.nearClipPlane = 0.1f;
			stageCamera.farClipPlane = 100f;
			stageCamera.targetTexture = target;

			var displayObject = new GameObject("Display");
			displayObject.transform.SetParent(transform, false);
			renderer = new FlashRenderer(displayObject.transform);

			var canvasObject = new GameObject("Screen", typeof(Canvas));
			canvasObject.transform.SetParent(transform, false);
			Canvas canvas = canvasObject.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;

			var imageObject = new GameObject("Picture", typeof(RectTransform), typeof(RawImage));
			imageObject.transform.SetParent(canvasObject.transform, false);
			screen = imageObject.GetComponent<RawImage>();
			screen.texture = target;
			screen.raycastTarget = false;
			screenRect = (RectTransform)imageObject.transform;
			screenRect.anchorMin = screenRect.anchorMax = screenRect.pivot = new Vector2(0.5f, 0.5f);
		}

		private void Update()
		{
			pollMouse();
			if (TaloketoInputManager.GetMouseButtonUp(0))
			{
				clickPending = true;
			}

			double frame = 1.0 / FrameRate;
			accumulator += Time.unscaledDeltaTime;
			if (accumulator > frame * 4)
			{
				accumulator = frame * 4;
			}

			while (accumulator >= frame)
			{
				accumulator -= frame;
				step(frame);
			}

			fitScreen();
			renderer.Render(root);
		}

		private void step(double frame)
		{
			if (clickPending)
			{
				clickPending = false;
				dispatchClick();
			}

			foreach (Action listener in enterFrameListeners.ToArray())
			{
				// A listener removed earlier in this frame does not run.
				if (enterFrameListeners.Contains(listener))
				{
					listener();
				}
			}

			FlashTimer.Advance(frame);
			TweenLite.Advance(frame);
		}

		/// <summary>
		/// A click reaches the click listeners. In this game there is one, on the
		/// foreground, which covers the whole stage; the things that are ever drawn above
		/// it (scorpions, figures) only exist between the two clicks the game waits for, so
		/// they never stand in the way of one.
		/// </summary>
		private void dispatchClick()
		{
			var all = new List<DisplayObject>();
			collect(root, all);
			foreach (DisplayObject o in all)
			{
				o.dispatchClick();
			}
		}

		private static void collect(DisplayObject o, List<DisplayObject> list)
		{
			list.Add(o);
			var container = o as DisplayObjectContainer;
			if (container != null)
			{
				foreach (DisplayObject child in container.children)
				{
					collect(child, list);
				}
			}
		}

		private void pollMouse()
		{
			Vector2 position = windowToStage(TaloketoInputManager.mousePosition);
			stage.mouseX = position.x;
			stage.mouseY = position.y;
		}

		private Rect pictureRect()
		{
			float aspect = (float)StageWidth / StageHeight;
			float windowAspect = (float)Screen.width / Screen.height;
			float w = windowAspect > aspect ? Screen.height * aspect : Screen.width;
			float h = windowAspect > aspect ? Screen.height : Screen.width / aspect;
			return new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
		}

		private Vector2 windowToStage(Vector2 windowPosition)
		{
			Rect r = pictureRect();
			return new Vector2(
				(windowPosition.x - r.x) / r.width * StageWidth,
				(1f - (windowPosition.y - r.y) / r.height) * StageHeight);
		}

		private void fitScreen()
		{
			Rect r = pictureRect();
			float scaleFactor = screen.canvas != null ? screen.canvas.scaleFactor : 1f;
			var size = new Vector2(r.width, r.height) / scaleFactor;
			if (screenRect.sizeDelta != size)
			{
				screenRect.sizeDelta = size;
			}
		}

		private void OnDestroy()
		{
			enterFrameListeners.Clear();
			FlashTimer.Reset();
			TweenLite.Reset();
			FlashSound.StopAll();
			FlashAssets.Clear();
			root = null;
			stage = null;

			if (target != null)
			{
				stageCamera.targetTexture = null;
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
