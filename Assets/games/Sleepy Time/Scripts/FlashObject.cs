using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// One node of the OpenFL display list, which is all any visual thing in Sleepy Time
	/// ever was: a parent-relative position in stage pixels, a scale, a rotation, and an
	/// alpha that multiplies down the tree.
	///
	/// Flash's frame is Unity's mirrored through the Y axis: origin top-left, Y down,
	/// rotation clockwise-positive. So localPosition = (x, -y) / PPU and z rotation is
	/// negated. Mirroring is a conjugation of the transform group, so it survives nesting -
	/// as long as every level of the tree is a FlashObject, parents compose exactly as they
	/// did in the original.
	///
	/// Never express the Y flip as a negative scale: it would mirror every descendant
	/// sprite's artwork along with the coordinates.
	///
	/// Property names are the Flash ones (lowercase x, y, scaleX, alpha) so ported code
	/// reads like the source it came from.
	/// </summary>
	public class FlashObject : MonoBehaviour
	{
		public const float PixelsPerUnit = 100f;

		/// <summary>
		/// Multiplied into the rendered colour by SleepyStage's render pass, together with
		/// every ancestor's alpha - Flash composited nested alphas, Unity's SpriteRenderer
		/// does not. Sleepy Time relies on this: Scene.updateGraphicsBody writes
		/// penis.bitmap.alpha while the Citmap above it animates its own alpha, and the two
		/// have to multiply.
		/// </summary>
		public float alpha = 1f;

		// Serialized only so the Flash-space values are readable in the Inspector while
		// debugging - the transform they drive is derived, and shows the mirrored numbers.
		[SerializeField] private float positionX;
		[SerializeField] private float positionY;
		[SerializeField] private float scaleXValue = 1f;
		[SerializeField] private float scaleYValue = 1f;
		[SerializeField] private float rotationValue;

		private Transform cachedParentTransform;
		private FlashObject cachedParent;

		public float x
		{
			get { return positionX; }
			set { positionX = value; ApplyPosition(); }
		}

		public float y
		{
			get { return positionY; }
			set { positionY = value; ApplyPosition(); }
		}

		public float scaleX
		{
			get { return scaleXValue; }
			set { scaleXValue = value; ApplyScale(); }
		}

		public float scaleY
		{
			get { return scaleYValue; }
			set { scaleYValue = value; ApplyScale(); }
		}

		/// Degrees, clockwise-positive, as in Flash.
		public float rotation
		{
			get { return rotationValue; }
			set
			{
				rotationValue = value;
				transform.localRotation = Quaternion.Euler(0f, 0f, -value);
			}
		}

		/// <summary>
		/// The FlashObject this one is a child of, or null at the root. Cached against the
		/// Transform it was resolved from, so re-parenting is picked up without a
		/// GetComponent on every access.
		/// </summary>
		public FlashObject Parent
		{
			get
			{
				Transform parent = transform.parent;
				if (parent != cachedParentTransform)
				{
					cachedParentTransform = parent;
					cachedParent = parent == null ? null : parent.GetComponent<FlashObject>();
				}

				return cachedParent;
			}
		}

		/// <summary>
		/// Flash's addChild: appends to the end of the child list, which is also the top of
		/// the paint order (SleepyStage walks the tree depth-first to assign sorting order).
		/// Re-adding a child that's already here moves it to the top, as in Flash.
		/// </summary>
		public T addChild<T>(T child) where T : FlashObject
		{
			if (child == null)
			{
				return null;
			}

			child.transform.SetParent(transform, false);
			child.transform.SetAsLastSibling();
			child.gameObject.SetActive(true);
			return child;
		}

		/// <summary>
		/// Flash's removeChild: the object stays alive but stops being drawn and stops
		/// belonging to anyone. Deactivating is what makes "no longer in the display list"
		/// mean "no longer rendered" here, since an orphaned GameObject would otherwise keep
		/// drawing (outside the tree SleepyStage walks, so at sorting order 0).
		/// </summary>
		public void removeChild(FlashObject child)
		{
			if (child == null || child.transform.parent != transform)
			{
				return;
			}

			child.transform.SetParent(null, false);
			child.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called once per frame by SleepyStage's render pass, in display-list order, with
		/// the product of every ancestor alpha and this one's. Overridden by the nodes that
		/// actually draw something.
		/// </summary>
		public virtual void ApplyRender(float worldAlpha, int sortingOrder)
		{
		}

		protected virtual void Awake()
		{
			ApplyPosition();
			ApplyScale();
			rotation = rotationValue;
		}

		/// <summary>
		/// Starts inactive, because a Flash display object draws nothing until it is added to
		/// the display list, and addChild is what activates it. Scene's hundred-particle pool
		/// is built up front and only added when a particle is fired - left active they would
		/// all sit visible at the stage origin.
		/// </summary>
		protected static GameObject NewNode(string name)
		{
			GameObject node = new GameObject(name);
			node.SetActive(false);
			node.transform.localPosition = Vector3.zero;
			return node;
		}

		private void ApplyPosition()
		{
			transform.localPosition = new Vector3(positionX / PixelsPerUnit, -positionY / PixelsPerUnit, 0f);
		}

		private void ApplyScale()
		{
			transform.localScale = new Vector3(scaleXValue, scaleYValue, 1f);
		}
	}
}
