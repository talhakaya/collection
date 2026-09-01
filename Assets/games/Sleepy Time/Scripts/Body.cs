using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from Body.as: the figure, as a self-contained object that can be placed
	/// anywhere. Song 6 weaves five of them across the stage instead of showing one; every
	/// other song draws the same parts directly in Scene.updateGraphicsBody.
	///
	/// XOffset/YOffset are what make that possible - every position the Scene version writes
	/// in stage coordinates is written here relative to the Body's own origin.
	///
	/// It reads GameManager.getKeyDown() itself rather than being handed input, so the five
	/// bodies all respond to the same press.
	///
	/// The decompiler lost the target of six compound assignments in the shake blocks; the
	/// order of the counters gives them away - hand, then head, then penis.
	/// </summary>
	public class Body : FlashObject
	{
		public static float PenisHeightMax;

		public int shakingPenisCounter = 0;
		public int shakingHeadCounter = 0;
		public int shakingHandCounter = 0;
		public float shakeFactor = 2f;
		public float penisHeight = 36f;
		public Citmap penis;
		public bool oldKeyDown = false;
		public int headRotationTimeCurrent = 0;
		public float headRotationMax = 15f;
		public bool headRotatingToMax = false;
		public bool headRotating = true;
		public Citmap head;
		public Citmap hand;
		public float endOfSong;
		public float YOffset = Main.stageHeight;
		public float XOffset = Main.stageWidth / 2f;

		public static Body New(int id = 0, float endOfSong = 0f)
		{
			Body body = NewNode("Body").AddComponent<Body>();
			body.endOfSong = endOfSong;

			body.penis = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/penis.png"), 8, 72);
			body.hand = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/hand.png"), 64, 90);
			body.head = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/head.png"), 45, 90);
			body.penis.scaleX = body.penis.scaleY = body.hand.scaleX = body.hand.scaleY =
				body.head.scaleX = body.head.scaleY = 4f;

			body.head.x = 45f * body.head.scaleX - body.XOffset;
			body.hand.x = Main.stageWidth - body.XOffset;
			body.penis.x = Main.stageWidth - 32f * body.hand.scaleX - body.XOffset;
			body.hand.y = Main.stageHeight + 72f * body.hand.scaleY + body.shakeFactor - body.YOffset;

			body.addChild(body.penis);
			body.addChild(body.head);
			body.addChild(body.hand);
			return body;
		}

		public void Tick()
		{
			if (!oldKeyDown && GameManager.getKeyDown())
			{
				shakingPenisCounter = 500;
				shakingHeadCounter = 500;
				Actuate.stop(hand);
				Actuate.tween(hand, 1f,
					y: Main.stageHeight + penisHeight * hand.scaleY - YOffset,
					x: Main.stageWidth + 3f * hand.scaleX * (-1f + 2f * Random.value) - XOffset);
			}
			else if (oldKeyDown && !GameManager.getKeyDown())
			{
				Actuate.stop(hand);
				Actuate.tween(hand, 1f,
					y: Main.stageHeight + 72f * hand.scaleY - YOffset,
					x: Main.stageWidth + 3f * hand.scaleX * (-1f + 2f * Random.value) - XOffset);
			}

			oldKeyDown = GameManager.getKeyDown();

			penisHeight = Mathf.Max(0f, 36f * (endOfSong - GameManager.time) / endOfSong);
			penis.y = Main.stageHeight + penisHeight * penis.scaleY + shakeFactor - YOffset;
			head.y = Main.stageHeight + (36f - penisHeight) * penis.scaleY / 4f + shakeFactor - YOffset;
			head.x = 45f * head.scaleX - XOffset;
			penis.x = Main.stageWidth - 32f * penis.scaleX - XOffset;
			penis.bitmap.alpha = 0.4f + 0.6f * Random.value;
			head.bitmap.alpha = 0.4f + 0.6f * Random.value;

			if (headRotating)
			{
				if (headRotatingToMax)
				{
					headRotationTimeCurrent += GameManager.dt;
					if (headRotationTimeCurrent >= GameManager.rhythm / 2f)
					{
						headRotatingToMax = false;
					}
				}
				else
				{
					headRotationTimeCurrent -= GameManager.dt;
					if (headRotationTimeCurrent <= -GameManager.rhythm / 2f)
					{
						headRotatingToMax = true;
					}
				}

				head.rotation = headRotationMax * (headRotationTimeCurrent / (float)GameManager.rhythm / 2f);
			}

			if (shakingHandCounter > 0)
			{
				shakingHandCounter -= GameManager.dt;
				hand.x += -shakeFactor + 2f * shakeFactor * Random.value;
				hand.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}

			if (shakingHeadCounter > 0)
			{
				shakingHeadCounter -= GameManager.dt;
				head.x += -shakeFactor + 2f * shakeFactor * Random.value;
				head.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}

			if (shakingPenisCounter > 0)
			{
				shakingPenisCounter -= GameManager.dt;
				penis.x += -shakeFactor + 2f * shakeFactor * Random.value;
				penis.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}
		}
	}
}
