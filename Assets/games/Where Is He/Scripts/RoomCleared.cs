using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class RoomCleared : MonoBehaviour
	{
	    private TintScript tint;
	    private static GameObject prefab;
	    public GameObject[] enableWhenFinished;
	    public AudioClip[] audio;
	    public bool disablePlayer;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
	        tint.effectDistance = 4f;
	        tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 0f);
	        GetComponent<SpriteRenderer>().color = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 0f);
	        SpriteEffect.make(Effect.Blur, gameObject);
	    }

		void Update ()
	    {
	        if (tint.effectDistance > 0f)
	        {
	            tint.effectDistance -= Game.dt * 4f;
	            if (tint.effectDistance > 0f)
	            {
	                tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 1f - tint.effectDistance * 0.25f);
	            }
	            else
	            {
	                tint.effectDistance = 0f;
	                tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 1f);
	                SpriteEffect.destroy(Effect.Blur, gameObject);
	                Game.screenShakeMedium();
	                for (int i = 0; i < enableWhenFinished.Length; i++)
	                {
	                    enableWhenFinished[i].SetActive(true);
	                }
	                for (int i = 0; i < audio.Length; i++)
	                {
	                    AudioSource.PlayClipAtPoint(audio[i], Camera.main.transform.position);
	                }
	                if (disablePlayer)
	                {
	                    // In the collection: this is the game's end ("the end" on the screen, the player
	                    // stopped), which in the story mode is what wins its artifact, after a moment to
	                    // read it.
	                    Collection.Story.StoryGames.Finish(2.5f);
	                    PlayerScript.instance.GetComponent<PlatformerController>().sprite.GetComponent<Animator>().speed = 0f;
	                    PlayerScript.instance.GetComponent<Collider2D>().enabled = false;
	                    PlayerScript.instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
	                    PlayerScript.instance.GetComponent<Rigidbody2D>().isKinematic = true;
	                    PlayerScript.instance.GetComponent<PlatformerController>().enabled = false;
	                    PlayerScript.instance.enabled = false;
	                }
	            }
	        }
	    }

	    public static void create()
	    {
	        if (prefab == null)
	        {
	            prefab = Resources.Load("WhereIsHe/roomCleared") as GameObject;
	        }
	        GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
	        go.transform.parent = WorldWander.currentRoom;
	        go.transform.localPosition = Vector3.forward * 3f;

	    }
	}
}
