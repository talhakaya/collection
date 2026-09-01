using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from Particle.as. Pooled - Scene builds 100 up front and cycles through them,
	/// so reset() is the constructor as far as gameplay is concerned.
	///
	/// Everything here integrates GameManager.dt, so it keeps working when the song clock is
	/// driven by the audio rather than by frame deltas.
	/// </summary>
	public class Particle : Citmap
	{
		public int timeCounter;
		public float rotatePerDt;
		public bool needsToBeKilled;
		public int lifeTime;
		public bool alive;
		public float accelerationY;
		public float accelerationX;

		public static Particle New()
		{
			Particle particle = NewNode("Particle").AddComponent<Particle>();
			particle.alive = false;
			particle.Init(SleepyAssets.GetSprite("img/particle.png"), 0.5f, 0.5f);
			particle.scaleX = particle.scaleY = 8f;
			return particle;
		}

		public override void Tick()
		{
			if (alive)
			{
				timeCounter += GameManager.dt;
				if (timeCounter >= lifeTime)
				{
					needsToBeKilled = true;
				}

				rotation += rotatePerDt * GameManager.dt;
				x += accelerationX * GameManager.dt / 50f;
				y += accelerationY * GameManager.dt / 50f;
				accelerationY += GameManager.dt / 15f;
				alpha = 0.5f + 0.5f * Random.value;
			}
		}

		public void reset(float x, float y)
		{
			this.x = x;
			this.y = y;
			lifeTime = 200 + FlashMath.round(300f * Random.value);
			rotatePerDt = -1f + 2f * Random.value;
			accelerationX = -5f + 10f * Random.value;
			accelerationY = -5f - 10f * Random.value;
			timeCounter = 0;
			alive = true;
			needsToBeKilled = false;
		}

		public void kill()
		{
			alive = false;
			needsToBeKilled = false;
		}
	}
}
