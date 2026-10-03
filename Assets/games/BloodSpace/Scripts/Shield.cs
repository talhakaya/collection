using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class Shield : MonoBehaviour {

		public static int thereIs = 0;

		public float lifeTime;

		private TintScript tint;
		private float maxAlpha;
		private float timeCounter = 0f;

		void Start ()
		{
			gameObject.name = "Shield";
			transform.parent = PlayerScript.instance.transform;
			transform.localPosition = Vector3.forward * (-0.1f);
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			tint = GetComponent<TintScript> ();
			maxAlpha = tint.selfColor.a;
			thereIs++;
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;
			if (timeCounter < lifeTime)
			{
				tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, (lifeTime - timeCounter) * maxAlpha / lifeTime);
			}
			else
			{
				thereIs--;
				Destroy(gameObject);
			}
		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			if (coll.gameObject.name == "Enemy")
			{
				coll.gameObject.GetComponent<EnemyScript>().hp -= 100f;
			}
		}
	}
}
