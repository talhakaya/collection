using UnityEngine;
using System.Collections;

namespace Games.OverEating101
{
	public class Food : MonoBehaviour {

		public bool eaten = false;
		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			if (transform.position.y < -10f)
			{
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other)
		{
			float l = Geometry.lengthOfVector2 (other.relativeVelocity);
			if (l > 0.1f)
			{
				Game.instance.aHit.volume = l;
				Game.instance.aHit.pitch = Random.Range(0.5f, 1.5f);
				Game.instance.aHit.Play();
			}
		}

		void OnCollisionStay2D(Collision2D other)
		{
			if (other.gameObject.name == "arm")
			{
				transform.position += Vector3.up * Game.dt;
			}
		}
	}
}
