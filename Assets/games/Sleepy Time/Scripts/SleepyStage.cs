using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// The Flash stage: a fixed 800x450 virtual surface with its origin at the top-left,
	/// plus the render pass that gives the display list Flash's compositing rules back.
	///
	/// The root sits at the stage's top-left corner in world space, so a child at Flash
	/// (0, 0) lands there and an orthographic camera centred on the origin frames exactly
	/// the stage. Nothing in the game reads the real screen size - GameManager.enterFrameHandler
	/// computed a scale factor from it and then threw it away by setting both axes back to 1 -
	/// so letterboxing is the camera's job, not the game's.
	///
	/// Once per frame, after everything has moved, the tree is walked depth-first to do the
	/// two things Unity does not do for us:
	///   - alpha inheritance: Flash multiplied a node's alpha by all of its ancestors',
	///     SpriteRenderer does not.
	///   - paint order: Flash drew in display-list order, so child order becomes sorting
	///     order. addChild appending to the end is what puts a new child on top.
	/// </summary>
	[DisallowMultipleComponent]
	public class SleepyStage : MonoBehaviour
	{
		public static SleepyStage Instance { get; private set; }

		/// The display-list root. Everything the game adds hangs under this.
		public FlashObject Root { get; private set; }

		[Tooltip("Framed to the 800x450 stage on Awake. Leave empty to use Camera.main.")]
		public Camera stageCamera;

		private int letterboxedWidth;
		private int letterboxedHeight;

		private void Awake()
		{
			Instance = this;

			SleepyAssets.Preload();

			// The corner offset lives here rather than on the root, because the root is a
			// FlashObject and a FlashObject owns its transform: its Awake applies its own
			// (x, y) - (0, 0) for the root - and would overwrite anything set from outside.
			// The stage is not a display-list node, so it is the right place to sit.
			transform.localPosition = new Vector3(
				-Main.stageWidth / 2f / FlashObject.PixelsPerUnit,
				Main.stageHeight / 2f / FlashObject.PixelsPerUnit,
				0f);

			GameObject root = new GameObject("Root");
			root.transform.SetParent(transform, false);
			Root = root.AddComponent<FlashObject>();

			if (stageCamera == null)
			{
				stageCamera = Camera.main;
			}

			if (stageCamera != null)
			{
				stageCamera.orthographic = true;
				stageCamera.orthographicSize = Main.stageHeight / 2f / FlashObject.PixelsPerUnit;
				ApplyLetterbox();
			}
		}

		/// <summary>
		/// Fits the 800x450 stage to the window without distorting it, adding bars on
		/// whichever pair of edges has the slack.
		///
		/// An orthographic camera fixes only the vertical extent and lets the horizontal
		/// follow the window, so without this a narrow window silently crops the sides of
		/// the stage and a wide one reveals empty space beside it. Squeezing the viewport to
		/// the stage's own aspect is what makes 800x450 mean 800x450 at any window size.
		///
		/// The original had no answer here - Flash was NO_SCALE, TOP_LEFT, and
		/// enterFrameHandler computed a scale factor from the live stage size and then threw
		/// it away by setting both axes back to 1. It only ever looked right because the web
		/// embed was exactly 800x450.
		/// </summary>
		private void ApplyLetterbox()
		{
			float stageAspect = (float)Main.stageWidth / Main.stageHeight;
			float windowAspect = (float)Screen.width / Screen.height;
			float scale = windowAspect / stageAspect;

			stageCamera.rect = scale < 1f
				? new Rect(0f, (1f - scale) / 2f, 1f, scale)
				: new Rect((1f - 1f / scale) / 2f, 0f, 1f / scale, 1f);

			letterboxedWidth = Screen.width;
			letterboxedHeight = Screen.height;
		}

		/// Main.init(): the stage exists, so make the game and add it. Deferred to Start so
		/// GameManager's construction can rely on every Awake in the scene having run.
		private void Start()
		{
			Root.addChild(GameManager.New());
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		private void LateUpdate()
		{
			if (stageCamera != null && (Screen.width != letterboxedWidth || Screen.height != letterboxedHeight))
			{
				ApplyLetterbox();
			}

			int sortingOrder = 0;
			Composite(Root.transform, 1f, ref sortingOrder);
		}

		private static void Composite(Transform node, float inheritedAlpha, ref int sortingOrder)
		{
			int childCount = node.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = node.GetChild(i);
				if (!child.gameObject.activeSelf)
				{
					continue;
				}

				FlashObject flashObject = child.GetComponent<FlashObject>();
				float alpha = flashObject == null ? inheritedAlpha : inheritedAlpha * flashObject.alpha;

				if (flashObject != null)
				{
					flashObject.ApplyRender(alpha, sortingOrder);
				}

				sortingOrder++;
				Composite(child, alpha, ref sortingOrder);
			}
		}
	}
}
