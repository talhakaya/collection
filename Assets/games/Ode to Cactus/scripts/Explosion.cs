using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class Explosion : MonoBehaviour {

	    public float scale;
	    private float timeCounter;
	    private float lag;
	    public Sprite[] sprites;
	    public float period = 0.2f;
	    private int i = 0;
	    private SpriteRenderer spriteRenderer;

		void Start ()
	    {
	        transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	        transform.localScale = Vector3.zero;
	        spriteRenderer = GetComponent<SpriteRenderer>();
	        spriteRenderer.sprite = sprites[0];
	        if (scale > 4f)
	        {
	            GetComponent<AudioSource>().pitch = 1.5f - scale / 10f;
	            GetComponent<AudioSource>().volume = scale / 10f;
	            GetComponent<AudioSource>().Play();
	        }
		}

		void Update ()
	    {
	        if (lag > 0f)
	        {
	            transform.localScale = Vector3.zero;
	            lag -= Game.dt;
	        }
	        else
	        {
	            timeCounter += Game.dt * 10f / scale;
	            transform.localScale = Vector3.one * (scale * Random.Range(2.8f, 3f));
	            i = Mathf.FloorToInt((timeCounter % (period * sprites.Length)) / period);
	            spriteRenderer.sprite = sprites[i];

	            if (timeCounter >= 0.98f)
	            {
	                Destroy(gameObject);
	            }
	        }
		}

	    public static void create(Vector3 pos, float scale)
	    {
	        GameObject exp = Instantiate(Game.instance.explosion, pos, Quaternion.identity) as GameObject;
	        Explosion e = exp.GetComponent<Explosion>();
	        e.scale = scale;

	        for (int i = 5; i < scale * 10f; i++)
	        {
	            GameObject exp2 = Instantiate(Game.instance.explosion, pos + new Vector3(Random.Range(-scale, scale), Random.Range(-scale, scale), 0f) / 5f, Quaternion.identity) as GameObject;
	            Explosion e2 = exp2.GetComponent<Explosion>();
	            e2.scale = Random.Range(1f, scale / 2f);
	            e2.lag = Random.Range(0f, scale / 20f);
	        }
	    }
	}
}
