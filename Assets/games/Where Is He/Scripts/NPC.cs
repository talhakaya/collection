using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class NPC : MonoBehaviour
	{
	    public string talkText;
	    public AudioClip audio;

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        if (other.tag == "Player")
	        {
	            Game.instance.startTalking(talkText, GetComponent<SpriteRenderer>());
	            AudioSource.PlayClipAtPoint(audio, Camera.main.transform.position);
	        }
	    }

	    void OnTriggerExit2D(Collider2D other)
	    {
	        if (other.tag == "Player")
	        {
	            Game.instance.stopTalking();
	        }
	    }
	}
}
