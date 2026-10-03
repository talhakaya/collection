using UnityEngine;
using System.Collections;

namespace Games.OverEating101
{
	public class FoodCheck : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.name == "food" && !other.GetComponent<Food>().eaten)
			{
				other.GetComponent<Food>().eaten = true;
				Game.instance.transform.Rotate(Vector3.forward * Random.Range(-20f, 20f));
				Camera.main.orthographicSize -= Random.Range(0.5f, 1f);
				Game.score++;
				Game.instance.aEat.clip = Game.instance.eatClips[Random.Range (0, Game.instance.eatClips.Length)];
				Game.instance.aEat.Play ();
			}
		}
	}
}
