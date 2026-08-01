using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class GetObject : MonoBehaviour
	{


		void Start ()
	    {
		
		}
		
		void Update ()
	    {
		
		}

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        trigger(other);
	    }

	    void OnTriggerStay2D(Collider2D other)
	    {
	        trigger(other);
	    }

	    void trigger(Collider2D other)
	    {
	        if (PlaneManager.instance.open && other.gameObject.GetComponent<PlayerScript>() != null && PlayerScript.instance.resetTimer <= 0)
	        {
	            AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, CameraGame.instance.transform.position);
	            gameObject.SetActive(false);
	            //GetComponent<AudioSource>().Play();
	        }
	    }
	}

}
