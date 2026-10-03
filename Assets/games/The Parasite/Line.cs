using UnityEngine;
using System.Collections;

namespace Games.TheParasite
{
	public class Line : MonoBehaviour {

	    private TintScript tint;
	    private float speed;
	    //public static float direction;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
	        Reset();
	        transform.localPosition = new Vector3(Random.Range(-30f, 40f), transform.localPosition.y, transform.localPosition.z);
		}

		void Update ()
	    {
	        transform.position += Vector3.left * speed * Game.dt;
		    if (transform.localPosition.x < -40f)
	        {
	            Reset();
	        }
		}

	    void Reset()
	    {
	        speed = Random.Range(20f, 30f);
	        transform.localScale = new Vector3(Random.Range(30f, 100f), Random.Range(3f, 10f), 1f);
	        transform.localPosition = new Vector3(Random.Range(30f, 40f), Random.Range(-5f, 5f), 16f);
	        Color rand = Game.colors[Random.Range(0, 5)];
	        tint.selfColor = new Color(rand.r, rand.g, rand.b, 0.5f);
	    }
	}
}
