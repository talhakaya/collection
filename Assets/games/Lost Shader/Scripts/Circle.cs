using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.LostShader
{
	public class Circle : MonoBehaviour
	{
	    private List<TintScript> circles;
	    private List<float> timers;
	    private List<float> angles;
	    private float period = 3f;
	    private float maxDistance = 5f;
	    private bool taken;
	    private TintScript tint;
	    private bool spawned;
	    public GameObject enableObject;
	    private float enabledTimer;

	    void OnEnable()
	    {
	        enabledTimer = 0f;
	    }

		void Start ()
	    {
	        enableObject.SetActive(false);
	        tint = GetComponent<TintScript>();
	        circles = new List<TintScript>();
	        timers = new List<float>();
	        angles = new List<float>();
	        foreach (Transform child in transform)
	        {
	            circles.Add(child.GetComponent<TintScript>());
	            timers.Add(period * Random.value);
	            angles.Add(360f * Random.value);
	            //child.GetComponent<TintScript>().selfColor = new Color(Random.value, Random.value, Random.value);
	            child.localScale *= Random.Range(0.5f, 1.5f);
	        }
		}

		void Update ()
	    {
	        if (enabledTimer < 3f)
	        {
	            enabledTimer += Game.dt;
	        }
	        for (int i = 0; i < circles.Count; i++)
	        {
	            timers[i] += Game.dt;
	            if (timers[i] >= period)
	            {
	                timers[i] = timers[i] % period;
	                angles[i] = 360f * Random.value;
	            }
	            circles[i].transform.localPosition = Geometry.createVector3(angles[i], Easing.SineEaseOut(timers[i], maxDistance, -maxDistance, period));
	        }

	        if (taken)
	        {
	            transform.localScale += Vector3.one * Game.dt * 10f;
	            if (transform.localScale.x > 10f)
	            {
	                tint.selfColor = new Color(1f, 1f, 1f, (30f - transform.localScale.x) * 0.1f);
	                for (int i = 0; i < circles.Count; i++)
	                {
	                    circles[i].selfColor = new Color(1f, 1f, 1f, tint.selfColor.a);
	                }

	                if (transform.localScale.x > 20f)
	                {
	                    if (!spawned)
	                    {
	                        spawned = true;
	                        CameraScript.PlanesUp();
	                        enableObject.SetActive(true);
	                    }
	                }

	                if (transform.localScale.x >= 30f)
	                {
	                    Destroy(gameObject);
	                }
	            }
	        }
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
	        if (other.tag == "Player" && enabledTimer >= 3f)//other.transform.lossyScale.x >= 0.3f && 
	        {
	            taken = true;
	            GetComponent<Scale>().enabled = false;
	        }
	    }
	}
}
