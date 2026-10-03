using UnityEngine;
using System.Collections;

namespace Games.LetsNeverDoThatAgain
{
	public class RandomSprite : MonoBehaviour {

	    private float rotateSpeed;

		void Start ()
	    {
	        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)));
	        transform.position = new Vector3(Random.Range(-6.4f, 6.4f), Random.Range(-3.6f, 3.6f), transform.position.z);
	        rotateSpeed = Random.Range(-360f, 360f);
	        transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		}

	    void Update()
	    {
	        transform.Rotate(Vector3.forward * rotateSpeed * Game.dt);
	    }
	}
}
