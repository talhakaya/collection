using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class ColorPicker : MonoBehaviour
	{
	    public int color;
	    public float alpha = 1f;

	    void OnValidate()
	    {
	        Color c = Game.colors[color];
	        c = new Color(c.r, c.g, c.b, alpha);
	        if (GetComponent<SpriteRenderer>() != null)
	        {
	            GetComponent<SpriteRenderer>().color = c;
	        }
	        if (GetComponent<TintScript>() != null)
	        {
	            GetComponent<TintScript>().selfColor = c;
	        }
	    }
	}

}
