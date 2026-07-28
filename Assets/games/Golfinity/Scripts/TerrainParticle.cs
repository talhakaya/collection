using UnityEngine;
using System.Collections;

namespace Games.Golfinity
{
	public class TerrainParticle : MonoBehaviour
	{
	    private Vector3 randomScale;
	    private float direction;
	    private const float gravity = 10f;
	    private float time;
	    private float period;
	    private float scaleFactor;
	    private float speedFactor;
	    private Vector3 velocity;

		void CallOnEnable (bool inMud)
	    {
	        randomScale = new Vector3(1f, Random.Range(0.2f, 0.4f), 1f) * Random.Range(1f, 2f) * scaleFactor;
	        transform.localScale = Vector3.zero;
	        time = 0f;
	        period = Random.Range(0.2f, 0.5f) * Mathf.Min(5f, speedFactor) / 5f;
	        velocity = Geometry.createVector3(direction, Random.Range(1f, 2f) * speedFactor);// +Vector3.up * gravity;
	        TintScript tint = GetComponent<TintScript>();
	        tint.typeOfSprite = inMud ? TintScript.Type.Mud : TintScript.Type.Terrain;
	        tint.UpdateTint();
		}

		void Update ()
	    {
	        time += Game.dt;
	        if (time < period * 0.1f)
	        {
	            transform.localScale = randomScale * time / (period * 0.1f);
	        }
	        else if (time < period * 0.7f)
	        {
	            transform.localScale = randomScale;
	            velocity += Vector3.down * gravity * Game.dt;
	        }
	        else if (time < period)
	        {
	            transform.localScale = randomScale * (period - time) / (period * 0.3f);
	            velocity += Vector3.down * gravity * Game.dt;
	        }
	        else
	        {
	            gameObject.SetActive(false);
	        }

	        transform.position += velocity * Game.dt;
	        transform.eulerAngles = Vector3.forward * Geometry.angleOfVector3(velocity);
		}

	    public static void create(int noOfParticles, Vector3 pos, float direction, float scale, float speed, float randomAngle = 40f, bool inMud = false)
	    {
	        for (int i = 0; i < noOfParticles; i++)
	        {
	            GameObject go = Pool.terrainPool.get(pos);
	            TerrainParticle tp = go.GetComponent<TerrainParticle>();
	            tp.direction = direction + Random.Range(-randomAngle, randomAngle);
	            tp.scaleFactor = scale;
	            tp.speedFactor = speed;
	            tp.CallOnEnable(inMud);
	        }
	    }
	}

}
