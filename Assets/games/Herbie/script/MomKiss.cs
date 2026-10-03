using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class MomKiss : MonoBehaviour {

		public static MomKiss instance;
		public GameObject lips;
		public GameObject mother;
		bool kissed = false;
		private float period = 2f;
	//	private float maxAngle = 15f;
		private float maxScale = 1.5f;

		void Start ()
		{
			Game.miniGame = gameObject;
			Game.isThereMiniGame = true;
			instance = this;
			Game.momLove++;
		}

		void Update ()
		{
			if (!kissed)
			{
				float ratio = (Game.time % (period * 2)) / period;
				float ratio2 = (ratio + 0.5f) % 2f;
	//			float ratio3 = (ratio + 1.5f) % 2f;
				float scaleX = 1f;
				float scaleY = 1f;
	//			float angle = 0f;
				if (ratio < 1f)
				{
					scaleX = 1f + (maxScale - 1f) * ratio;
				}
				else
				{
					scaleX = 1f + (maxScale - 1f) * (2 - ratio);
				}
	//			if (ratio3 < 1f)
	//			{
	//				angle = 2 * ratio3 * maxAngle - maxAngle;
	//			}
	//			else
	//			{
	//				angle = 2 * (2 - ratio3) * maxAngle - maxAngle;
	//			}
				if (ratio2 < 1f)
				{
					scaleY = 1f + (maxScale - 1f) * ratio2;
				}
				else
				{
					scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
				}
				lips.transform.localScale = 10 * new Vector3(scaleX, scaleY, 1f);
	//			lips.transform.eulerAngles = Vector3.forward * angle;
				if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0 || TaloketoInputManager.GetAxisRaw("Vertical") != 0)
				{
					Vector3 force = Geometry.normalizeVector2(new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")), 1000);
					lips.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
				}
			}
			else
			{
				lips.transform.localScale += 100 * Vector3.up * Game.dt;
				if (lips.transform.localScale.y > 100)
				{
					Game.endMiniGame();
				}
			}
		}

		public void kiss()
		{
			kissed = true;
		}
	}
}
