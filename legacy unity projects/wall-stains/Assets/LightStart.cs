using UnityEngine;
using System.Collections;

public class LightStart : MonoBehaviour {

    public static int candles;
    private bool sound;

	void Start ()
    {
	    foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
	}
	
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.name.Contains("Player"))
        {
            if (!sound)
            {
                sound = true;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
                GetComponent<Rigidbody>().isKinematic = true;
                candles++;
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
