using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class ShipFire : MonoBehaviour {

	    public float speed;
	    private float direction;

		void Start ()
	    {
	        speed += Random.Range(-5f, 5f);
	        direction = Random.Range(-5f, 5f);
		}

		void Update ()
	    {
	        transform.position += Geometry.createVector3(90 + direction, speed * Game.dt);

	        if (transform.position.y > 5f)
	        {
	            Destroy(gameObject);
	        }
		}
	}
}
