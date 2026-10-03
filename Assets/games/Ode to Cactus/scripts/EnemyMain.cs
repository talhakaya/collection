using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class EnemyMain : MonoBehaviour {

	    public float speed;
	    private float hSpeed;

		void Start ()
	    {
	        speed += Random.Range(-1f, 1f);
	        if (transform.position.x < 0f)
	        {
	            hSpeed = Random.Range(-speed / 6f, speed / 2f);
	        }
	        else
	        {
	            hSpeed = Random.Range(-speed / 2f, speed / 6f);
	        }
		}

		void Update ()
	    {
	        transform.position += Vector3.down * speed * Game.dt;
	        transform.position += Vector3.right * hSpeed * Game.dt;

	        if (transform.position.y < -10f)
	        {
	            Destroy(gameObject);
	        }
		}
	}
}
