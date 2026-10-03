using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Star : MonoBehaviour {

		private TintScript tint;
		private Vector3 speed;
		private float MaxDist;
		private float turnSpeed;
		void Start ()
		{
			tint = GetComponent<TintScript> ();
			tint.selfColor = Game.color0 / 2;
			MaxDist = Random.Range (4f, 6f);
			turnSpeed = Random.Range (100f, 360f);
			speed = Geometry.createVector3 (Random.Range (0f, 360f), Random.Range (15f, 20f));
			float rand = Random.Range (0f, MaxDist);
			transform.position = speed * Random.Range (0f, 1f) + Vector3.forward * 7;
			tint.selfColor = new Color (tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, (1f - rand));
		}

		void Update ()
		{
			if (tint.selfColor.a <= 0)
			{
				transform.position = Vector3.forward * 7;
				speed = Geometry.createVector3 (Random.Range (0f, 360f), Random.Range (15f, 20f));
			}
			transform.position += speed * Game.dt;
			float distance = Geometry.lengthOfVector2(new Vector2(transform.position.x, transform.position.y));
			tint.selfColor = new Color (tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, (MaxDist - distance) / MaxDist);
			transform.Rotate (Vector3.forward * Game.dt * turnSpeed);
		}
	}
}
