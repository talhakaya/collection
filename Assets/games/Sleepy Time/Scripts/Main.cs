using Collection.Controls;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// What survives of Main.as: the stage dimensions the rest of the game measures itself
	/// against, and the stage's mouse position.
	///
	/// The original read the dimensions off the live Flash stage, but SleepyTime.swf's header
	/// declares 800x450 and the source assumes it anyway - enterFrameHandler divides by
	/// literal 800/450, and GriddyBackground hardcodes (800 - 45 * 9) / 2 - so they're
	/// constants here.
	///
	/// Kept under the original names so call sites transcribe verbatim: Main.stageWidth,
	/// Main.STAGE.mouseX.
	/// </summary>
	public static class Main
	{
		public const int stageWidth = 800;
		public const int stageHeight = 450;

		public static readonly Stage STAGE = new Stage();

		/// <summary>
		/// The pointer in stage pixels - origin top-left, Y down - which is the space
		/// Button.isColliding does its arithmetic in.
		///
		/// Goes through the camera's viewport rather than scaling the screen directly,
		/// because SleepyStage letterboxes: at anything but 16:9 the stage does not fill the
		/// window, and only the viewport knows where its edges are. Coordinates outside
		/// 0..stageWidth/Height mean the pointer is on a bar, not on the stage.
		/// </summary>
		public class Stage
		{
			public float mouseX
			{
				get { return Viewport().x * stageWidth; }
			}

			public float mouseY
			{
				get { return (1f - Viewport().y) * stageHeight; }
			}

			private static Vector3 Viewport()
			{
				SleepyStage stage = SleepyStage.Instance;
				Camera camera = stage == null ? null : stage.stageCamera;
				if (camera == null)
				{
					return Vector3.zero;
				}

				return camera.ScreenToViewportPoint(TaloketoInputManager.mousePosition);
			}
		}
	}
}
