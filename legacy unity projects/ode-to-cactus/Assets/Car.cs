using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    private bool isOn;
    private float timeCounter;
	
	void Update ()
    {
	    if (isOn)
        {
            timeCounter += Game.dt;
            if (timeCounter > 1f)
            {
                Game.nextLevel();
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        isOn = true;
        other.GetComponent<Man>().movementEnabled = false;
        other.GetComponent<TalhaAnimation>().enabled = false;
        other.GetComponent<SpriteRenderer>().enabled = false;
        other.GetComponent<TintScript>().enabled = false;
        other.GetComponent<TalhaColorChanger>().enabled = false;
        other.rigidbody2D.velocity = Vector2.zero;
        GetComponent<AudioSource>().Play();
    }
}
