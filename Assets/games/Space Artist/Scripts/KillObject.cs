using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class KillObject : MonoBehaviour
	{
	    public bool moving;
	    private Vector3 pos0;
	    private Vector3 pos1;
	    public float timer;
	    public float period = 2f;

	    void Start()
	    {
	        if (moving)
	        {
	            pos0 = transform.position;
	            foreach (Transform child in transform)
	            {
	                pos1 = child.position;
	            }
	        }
	    }

	    void Update()
	    {
	        if (moving)
	        {
	            timer += Game.dt;
	            timer = timer % (period * 2f);
	            float t = timer;
	            if (timer >= period)
	            {
	                t = period * 2f - timer;
	            }
	            //float x = Easing.Linear(t, pos0.x, pos1.x - pos0.x, period);
	            //float y = Easing.Linear(t, pos0.y, pos1.y - pos0.y, period);
	            //float z = Easing.Linear(t, pos0.z, pos1.z - pos0.z, period);
	            transform.position = (pos0 * t + pos1 * (period -t)) / period;
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
	        if (PlaneManager.instance.open && other.gameObject.GetComponent<PlayerScript>() != null && PlayerScript.instance.resetTimer <= 0)
	        {
	            LevelManager.instance.resetLevel();
	            GetComponent<AudioSource>().Play();
	            CameraGame.pixelGlitch = 0.5f;
	        }
	    }
	}

}
