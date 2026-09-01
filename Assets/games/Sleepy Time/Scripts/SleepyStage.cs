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

		private void Awake()
		{
			Instance = this;

			SleepyAssets.Preload();

			GameObject root = new GameObject("Root");
			root.transform.SetParent(transform, false);
			root.transform.localPosition = new Vector3(
				-Main.stageWidth / 2f / FlashObject.PixelsPerUnit,
				Main.stageHeight / 2f / FlashObject.PixelsPerUnit,
				0f);
			Root = root.AddComponent<FlashObject>();

			if (stageCamera == null)
			{
				stageCamera = Camera.main;
			}

			if (stageCamera != null)
			{
				stageCamera.orthographic = true;
				stageCamera.orthographicSize = Main.stageHeight / 2f / FlashObject.PixelsPerUnit;
			}
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
