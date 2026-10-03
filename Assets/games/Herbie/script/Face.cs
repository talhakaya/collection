using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Face : MonoBehaviour {
		public TalhaAnimation happy;
		public TalhaAnimation sad;
		public Herbie herbie;
		private bool isHappy;
		private TalhaAnimation current;
		private float period = 2f;
		private float maxAngle = 15f;
		private float maxScale = 1.5f;

		void Start ()
		{
			if (herbie != null)
			{
				isHappy = !herbie.isHappy;
			}
		}

		void Update ()
		{
			float ratio = (Game.time % (period * 2)) / period;
			float ratio2 = (ratio + 0.5f) % 2f;
			float ratio3 = (ratio + 1.5f) % 2f;
			float scaleX = 1f;
			float scaleY = 1f;
			float angle = 0f;
			if (ratio < 1f)
			{
				scaleX = 1f + (maxScale - 1f) * ratio;
			}
			else
			{
				scaleX = 1f + (maxScale - 1f) * (2 - ratio);
			}
			if (ratio3 < 1f)
			{
				angle = 2 * ratio3 * maxAngle - maxAngle;
			}
			else
			{
				angle = 2 * (2 - ratio3) * maxAngle - maxAngle;
			}
			if (ratio2 < 1f)
			{
				scaleY = 1f + (maxScale - 1f) * ratio2;
			}
			else
			{
				scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
			}
			transform.localScale = new Vector3(scaleX, scaleY, 1f);
			transform.eulerAngles = Vector3.forward * angle;
			if (herbie != null)
			{
				if (!herbie.isHappy && isHappy)
				{
					changeAnimation(sad);
				}
				else if (herbie.isHappy && !isHappy)
				{
					changeAnimation(happy);
				}
				isHappy = herbie.isHappy;
			}
		}

		void changeAnimation(TalhaAnimation newAnim)
		{
			if (current != null)
			{
				current.enabled = false;
			}
			newAnim.enabled = true;
			current = newAnim;
		}
	}
}
