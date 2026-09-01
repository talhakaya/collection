using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from Citmap.as, the base every visual thing in the game extends: a display-list
	/// node with a bitmap child offset by the registration point, plus two decorations that
	/// give the whole game its nervous, on-the-beat feel - a random per-frame alpha (blink)
	/// and a sway of +/-rotationMax over one beat (rotate).
	///
	/// centerX/centerY hold the *negated* constructor arguments, as in the source, because
	/// that's the value the bitmap offset needs and subclasses read the field.
	///
	/// The update() of the original is Tick() here: Unity dispatches any method literally
	/// named Update on a MonoBehaviour, and this one is called explicitly, in order, from the
	/// chain GameManager drives - two updates a frame in the wrong order would be a very
	/// quiet bug.
	/// </summary>
	public class Citmap : FlashObject
	{
		public FlashBitmap bitmap;
		public float centerX;
		public float centerY;
		public bool blinking = false;
		public bool rotating = true;
		public float rotationMax = 15f;
		public int rotationTimeCurrent = 0;
		public bool rotatingToMax = false;

		public static Citmap New(Sprite bitmapData, float centerX = 0f, float centerY = 0f)
		{
			Citmap citmap = NewNode("Citmap").AddComponent<Citmap>();
			citmap.Init(bitmapData, centerX, centerY);
			return citmap;
		}

		/// The base constructor - subclass factories call this where the source called super().
		protected void Init(Sprite bitmapData, float centerX = 0f, float centerY = 0f)
		{
			bitmap = FlashBitmap.New(bitmapData);
			addChild(bitmap);
			this.centerX = -centerX;
			this.centerY = -centerY;
			bitmap.x = this.centerX;
			bitmap.y = this.centerY;
		}

		public virtual void Tick()
		{
			if (blinking)
			{
				Blink();
			}

			if (rotating)
			{
				Rotate();
			}
		}

		public void Rotate()
		{
			if (rotatingToMax)
			{
				rotationTimeCurrent += GameManager.dt;
				if (rotationTimeCurrent >= GameManager.rhythm)
				{
					rotatingToMax = false;
				}
			}
			else
			{
				rotationTimeCurrent -= GameManager.dt;
				if (rotationTimeCurrent <= -GameManager.rhythm)
				{
					rotatingToMax = true;
				}
			}

			rotation = rotationMax * (rotationTimeCurrent / (float)GameManager.rhythm);
		}

		public void Blink()
		{
			bitmap.alpha = GameManager.blink(bitmap.alpha, 0.4f, 0.6f);
		}
	}
}
