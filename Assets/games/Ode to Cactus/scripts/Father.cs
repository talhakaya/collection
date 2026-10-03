using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class Father : MonoBehaviour {

	    public Light[] lights;
	    public float waitForNextLevel;
	    private bool isOn;

		void Start ()
	    {
	        for (int i = 0; i < lights.Length; i++)
	        {
	            lights[i].gameObject.SetActive(false);
	        }
		}

		void Update ()
	    {
		    if (isOn)
	        {
	            for (int i = 0; i < lights.Length; i++)
	            {
	                lights[i].intensity = Random.Range(0.2f, 1f);
	            }
	            waitForNextLevel -= Game.dt;
	            if (waitForNextLevel < 0)
	            {
	                Game.nextLevel();
	            }
	        }
		}

	    void OnTriggerEnter(Collider other)
	    {
	        if (other.name.Contains("Person") && !isOn)
	        {
	            isOn = true;
	            GetComponent<AudioSource>().Play();
	            for (int i = 0; i < lights.Length; i++)
	            {
	                lights[i].gameObject.SetActive(true);
	            }
	        }
	    }
	}
}
