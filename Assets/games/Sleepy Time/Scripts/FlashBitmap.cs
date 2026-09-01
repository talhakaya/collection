using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// OpenFL's Bitmap: a display-list node that draws a BitmapData. It has its own x, y,
	/// scale and alpha, separate from the Sprite it hangs under - Citmap uses exactly that
	/// separation, offsetting the bitmap by the registration point while the Citmap itself
	/// carries the object's position.
	///
	/// Sprites are imported with a top-left pivot for the same reason: the registration
	/// point is a child offset in the source (bitmap.x = -centerX), not a pivot, and the
	/// same image is constructed with different centres in different places.
	/// </summary>
	public class FlashBitmap : FlashObject
	{
		private SpriteRenderer cachedRenderer;

		public static FlashBitmap New(Sprite bitmapData)
		{
			GameObject node = NewNode("Bitmap");
			node.AddComponent<SpriteRenderer>();

			FlashBitmap bitmap = node.AddComponent<FlashBitmap>();
			bitmap.bitmapData = bitmapData;
			return bitmap;
		}

		public SpriteRenderer Renderer
		{
			get
			{
				if (cachedRenderer == null)
				{
					cachedRenderer = GetComponent<SpriteRenderer>();
				}

				return cachedRenderer;
			}
		}

		/// Named after the Flash property it stands in for (bitmap.bitmapData = ...).
		public Sprite bitmapData
		{
			get { return Renderer.sprite; }
			set { Renderer.sprite = value; }
		}

		public override void ApplyRender(float worldAlpha, int sortingOrder)
		{
			SpriteRenderer renderer = Renderer;
			Color color = renderer.color;
			color.a = worldAlpha;
			renderer.color = color;
			renderer.sortingOrder = sortingOrder;
		}
	}
}
