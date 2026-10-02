using UnityEngine;
using System.Collections;

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
        birthTime = 0.3f;
        time = 0f;
	}
	
	void Update ()
    {
        transform.position = new Vector3(HoleTrigger.pos.x, HoleTrigger.pos.y, transform.position.z);

        time += speed * Game.dt;
        transform.localScale = time * Vector3.one;

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

	    if (time < period * 0.75f)
        {
            
        }
        else if (time < period)
        {
            transform.localScale += speed * Game.dt * Vector3.one;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (period - time) / (period * 0.25f));
        }
        else
        {
            gameObject.SetActive(false);
        }
	}

    public static void create(int noOfParticles, Vector3 pos, float speedFactor = 2f, float periodFactor = 2f)
    {
        GameObject go = ObjectPool.circlePool.get(pos);
        CircleParticle cp = go.GetComponent<CircleParticle>();
        cp.circleCount = noOfParticles;
        cp.speed = speedFactor;
        cp.period = periodFactor;
        cp.CallOnEnable();
    }
}
