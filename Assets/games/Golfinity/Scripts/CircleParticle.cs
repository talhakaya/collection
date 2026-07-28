using UnityEngine;
using System.Collections;

namespace Games.Golfinity
{
	public class CircleParticle : MonoBehaviour
	{
	    private float time;
	    private float speed;
	    private float period;
	    private int circleCount;
	    private bool gaveBirth;
	    private float birthTime;
	    private TintScript tint;
	    private SpriteRenderer sprite;

		void CallOnEnable ()
	    {
	        transform.localScale = Vector3.zero;
	        tint = GetComponent<TintScript>();
	        sprite = GetComponent<SpriteRenderer>();
	        tint.UpdateTint();
	        gaveBirth = false;
	        birthTime = 0.25f;
	        time = 0f;
		}
		
		void Update ()
	    {
	        transform.position = new Vector3(HoleTrigger.pos.x, HoleTrigger.pos.y, transform.position.z);

	        time += Game.dt;
	        transform.localScale = time * speed * Vector3.one;

	        if (!gaveBirth && time > period * birthTime)
	        {
	            gaveBirth = true;
	            if (circleCount > 0)
	            {
	                if (Game.circleHoleEffectOn)
	                {
	                    create(circleCount - 1, transform.position, speed, period);
	                }
	            }
	        }

		    if (time < period * 0.5f)
	        {
	            tint.changingColor = new Color(tint.changingColor.r, tint.changingColor.g, tint.changingColor.b, 1f);
	        }
	        else if (time < period)
	        {
	            tint.changingColor = new Color(tint.changingColor.r, tint.changingColor.g, tint.changingColor.b, (period - time) / (period * 0.5f));
	        }
	        else
	        {
	            gameObject.SetActive(false);
	        }
		}

	    public static void create(int noOfParticles, Vector3 pos, float speedFactor = 5f, float periodFactor = 0.6f)
	    {
	        GameObject go = Pool.circlePool.get(pos);
	        CircleParticle cp = go.GetComponent<CircleParticle>();
	        cp.circleCount = noOfParticles;
	        cp.speed = speedFactor;
	        cp.period = periodFactor;
	        cp.CallOnEnable();
	    }
	}

}
