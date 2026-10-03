using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class KidEatCereal : MonoBehaviour
	{
	    Sprite firstSprite;
	    public SpriteRenderer mouth;
	    private float timer;

	    void Start()
	    {
	        firstSprite = mouth.sprite;
	    }

	    void Update()
	    {
	        if (timer > 0f)
	        {
	            timer -= Game.dt;
	            mouth.GetComponent<TalhaAnimation>().enabled = true;
	        }
	        else
	        {
	            mouth.GetComponent<TalhaAnimation>().enabled = false;
	            mouth.sprite = firstSprite;
	        }
	    }

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        if (other.tag == "Spoon")
	        {
	            timer = 2f;
	            other.GetComponent<Spoon>().eatCereal();
	        }
	    }

	    void OnTriggerStay2D(Collider2D other)
	    {
	        if (other.tag == "Spoon")
	        {
	            timer = 2f;
	            other.GetComponent<Spoon>().eatCereal();
	        }
	    }
	}
}
