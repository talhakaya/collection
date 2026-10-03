using UnityEngine;
using System.Collections;

namespace Games.SlimeFight
{
	public class SlimeDead : MonoBehaviour {

		// In the collection: this was loaded in the field initializer, which Unity no longer
		// allows for a MonoBehaviour; it is loaded on first use instead.
		private static GameObject _prefab;
		public static GameObject prefab
		{
			get
			{
				if (_prefab == null) _prefab = Resources.Load ("SlimeFight/SlimeDead") as GameObject;
				return _prefab;
			}
		}
		private float direction;
		private float speed;
		public AudioSource audioSource;

		void Start ()
		{
			direction = Random.Range(0f, 1f) * 360;
			GetComponent<DoubleSprite> ().selfColor = new Color (Random.Range (0.7f, 1f), Random.Range (0.7f, 1f), Random.Range (0.7f, 1f));
			transform.Rotate (Vector3.forward, direction);
			speed = Random.Range (1f, 2f);
			audioSource.pitch = Random.Range(0.7f, 1.3f);
			audioSource.Play ();
		}

		void Update ()
		{
			transform.position += Geometry.createVector3 (direction, speed * Time.deltaTime);
			speed -= Time.deltaTime;
			if (speed <= 0)
			{
				speed = 0;
			}
		}


		public static SlimeDead create(float size)
		{
			SlimeDead slimeDead = (Instantiate (prefab) as GameObject).GetComponent<SlimeDead> ();
			slimeDead.name = "SlimeDead";
			slimeDead.transform.localScale = new Vector3 (size, size, size);
			slimeDead.transform.parent = GameManager.game;

			return slimeDead;
		}
	}
}
