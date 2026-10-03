using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class Explosion : MonoBehaviour
	{
	    public static GameObject prefab;
	    public float lineHeightChange = 0.2f;
	    public float lineRGBChange = 0.3f;
	    private TintScript tint;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
	        SpriteEffect.make(Effect.Blur, gameObject, false, true, transform);
	        LineManager.SrgbSplit += lineRGBChange;
	        LineManager.Sheight += lineHeightChange;
		}

		void Update ()
	    {
	        tint.effectDistance += Game.dt * 5f;
	        if (tint.effectDistance >= 2.5f)
	        {
	            Destroy(gameObject);
	        }
		}

	    public static void create(GameObject objectToExplode, float scale = 1f)
	    {
	        if (prefab == null)
	        {
	            prefab = Resources.Load("WhereIsHe/explosion") as GameObject;
	        }
	        GameObject go = Instantiate(prefab, objectToExplode.transform.position, objectToExplode.transform.rotation) as GameObject;
	        go.transform.parent = objectToExplode.transform.parent;
	        go.GetComponent<SpriteRenderer>().sprite = objectToExplode.GetComponent<SpriteRenderer>().sprite;
	        go.transform.localScale = objectToExplode.transform.localScale * scale;
	        go.GetComponent<TintScript>().selfColor = objectToExplode.GetComponent<SpriteRenderer>().color;
	    }
	}
}
