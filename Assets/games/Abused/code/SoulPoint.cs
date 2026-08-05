using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public class SoulPoint : MonoBehaviour
	{
	    private bool pickedUp;
	    private TintScript tint;
	    private float scaleTweenMax;
	    private float speed;
	    private float randRotCounter;
	    private Vector3 direction;
	    private GameObject player;
	    public static int pickedSoul;
	    public static int totalSoul;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
	        scaleTweenMax = Random.Range(1f, 3f);
	        totalSoul++;
		}
		
		void Update ()
	    {
	        randRotCounter -= Game.dt;
	        if (randRotCounter <= 0f)
	        {
	            randRotCounter = 1f;
	            transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	        }
	        transform.localScale = Vector3.one * 2 * (1 + ((Game.time % scaleTweenMax < scaleTweenMax / 2) ? Game.time % scaleTweenMax : scaleTweenMax - (Game.time % scaleTweenMax)));

	        if (pickedUp)
	        {
	            
	            if (speed <= 0f)
	            {
	                speed = Random.Range(0.9f, 1.1f);
	                direction = (player.transform.position + Geometry.createVector3(Random.Range(0f, 360f), 50f)) - transform.position;
	            }
	            else
	            {
	                speed -= Game.dt;
	                transform.position += direction * speed * Game.dt;
	            }
	            transform.position = new Vector3(transform.position.x, transform.position.y, -6f);
	        }
		}

	    void OnTriggerStay2D(Collider2D other)
	    {
	        if (other.name.Contains("Player") && !pickedUp)
	        {
	            pickedSoul++;
	            pickedUp = true;
	            transform.position = new Vector3(transform.position.x, transform.position.y, -6f);
	            player = other.gameObject;
	            SpriteEffect.make(Effect.Blur, gameObject, false, true, other.transform);
	            GetComponent<AudioSource>().pitch = Random.Range(0.5f, 0.8f);
	            GetComponent<AudioSource>().Play();
	        }
	    }
	}

}
