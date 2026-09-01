using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// One star on the score screen or beside a song in the menu. Ported from ScoreStar.as.
	///
	/// Earned stars pop in first and hold still; unearned ones come in half a second later
	/// and blink, so the row fills in visibly rather than all at once. The pop is
	/// pow(t, 1.5), which starts slow and snaps.
	/// </summary>
	public class ScoreStar : Citmap
	{
		public int timeCounter = 0;
		public float maxScale = 0.9f;
		public int delay = 0;

		public static ScoreStar New(bool full = false)
		{
			ScoreStar star = NewNode("ScoreStar").AddComponent<ScoreStar>();

			if (full)
			{
				star.Init(SleepyAssets.GetSprite("img/star_full.png"), 62, 57);
				star.delay = FlashMath.round(Random.value * 500f);
			}
			else
			{
				star.Init(SleepyAssets.GetSprite("img/star_empty.png"), 62, 57);
				star.delay = 500 + FlashMath.round(Random.value * 500f);
				star.blinking = true;
			}

			star.scaleX = star.scaleY = 0f;
			return star;
		}

		public override void Tick()
		{
			base.Tick();

			timeCounter += GameManager.dt;
			if (timeCounter >= delay)
			{
				if (timeCounter < delay + 1000)
				{
					scaleX = scaleY = Mathf.Pow((timeCounter - delay) / 1000f, 1.5f) * maxScale;
				}
				else
				{
					scaleX = scaleY = maxScale;
				}
			}
		}
	}
}
