using UnityEngine;
using System.Collections;

namespace Games.SlimeFight
{
	public class DoubleSprite : MonoBehaviour {

		public static Color weatherColor = new Color (1f, 1f, 1f);

		public Color selfColor = new Color (1f, 1f, 1f, 1f);
		public SpriteRenderer sprite0;
		public SpriteRenderer sprite1;
		private float shakeConst;
		public static float ShakeConst = 0.03f;
		public static float MinShakeConst = 0.03f;
		public static float MaxShakeConst = 0.5f;

		void Start ()
		{

		}

		void Update ()
		{
			shakeConst = ShakeConst;
			sprite0.color = new Color(selfColor.r * weatherColor.r, selfColor.g * weatherColor.g, selfColor.b * weatherColor.b, selfColor.a);
			sprite1.color = new Color(selfColor.r * weatherColor.r, selfColor.g * weatherColor.g, selfColor.b * weatherColor.b, selfColor.a);
			if (!GameManager.nPressed)
			{
				sprite0.transform.localPosition = new Vector3(Random.Range(-shakeConst, shakeConst), Random.Range(-shakeConst, shakeConst), sprite0.transform.localPosition.z);
				sprite1.transform.localPosition = new Vector3(Random.Range(-shakeConst, shakeConst), Random.Range(-shakeConst, shakeConst), sprite1.transform.localPosition.z);
			}
			else
			{
				sprite0.transform.localPosition = new Vector3(0f, 0f, sprite0.transform.localPosition.z);
				sprite1.transform.localPosition = new Vector3(0f, 0f, sprite1.transform.localPosition.z);
			}
			if (transform.position.x < -GameManager.XMax)
			{
				transform.position = new Vector3(-GameManager.XMax, transform.position.y, transform.position.z);
			}
			else if (transform.position.x > GameManager.XMax)
			{
				transform.position = new Vector3(GameManager.XMax, transform.position.y, transform.position.z);
			}
			if (transform.position.y < -GameManager.YMax)
			{
				transform.position = new Vector3(transform.position.x, -GameManager.YMax, transform.position.z);
			}
			else if (transform.position.y > GameManager.YMax)
			{
				transform.position = new Vector3(transform.position.x, GameManager.YMax, transform.position.z);
			}
		}
	}
}
