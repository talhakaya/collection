using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public class Person : MonoBehaviour {

	    private Transform player;

		void Start ()
	    {
	        transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
	        GetComponent<AudioSource>().volume = 0f;
		}
		
		void Update ()
	    {
		    if (player != null)
	        {
	            transform.eulerAngles = Vector3.forward * Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x) * 180 / Mathf.PI;
	        }
		}

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        if (other.name.Contains("Player"))
	        {
	            player = other.transform;
	            SpriteEffect.make(Effect.RGBSplit, gameObject, false, true, other.transform);
	            GetComponent<AudioSource>().volume = 0.1f;
	        }
	    }

	    void OnTriggerExit2D(Collider2D other)
	    {
	        if (other.name.Contains("Player"))
	        {
	            player = null;
	            SpriteEffect.destroy(Effect.RGBSplit, gameObject);
	            GetComponent<AudioSource>().volume = 0f;
	        }
	    }
	}

}
