using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class Particle : MonoBehaviour
	{
	    public int recursionCount = 0;
	    public float scale = 1f;
	    private float timer;
	    private const float period = 2f;
	    private const float moveAmount = 0.5f;
	    private TintScript tint;
	    
		void Awake ()
	    {
	        tint = GetComponent<TintScript>();
		}

		void OnEnable ()
	    {
	        transform.localScale = new Vector3(0.1f, 1f, 1f) * scale;
	        tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 1f);
	        timer = 0f;
		}
		
		void Update ()
	    {
	        timer += Game.dt;
	        transform.position += -transform.right * Game.dt * moveAmount / period;
	        if (timer <= period * 0.5f)
	        {
	            transform.localScale = new Vector3(timer / (period * 0.5f), 1f, 1f) * scale;
	        }
	        else if (timer < period)
	        {
	            transform.localScale = new Vector3(1f, 1f, 1f) * scale;
	            tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, (period - timer) / (period * 0.5f));
	        }
	        else
	        {
	            if (recursionCount > 0)
	            {
	                int noOfParticles = Random.Range(2, 6);
	                for (int i = 0; i < noOfParticles; i++)
	                {
	                    ParticlePool.get(transform.position, transform.eulerAngles.z + Random.Range(-60f, 60f), recursionCount - 1, scale * 0.5f);
	                }
	            }
	            gameObject.SetActive(false);
	        }
		}
	}

}
