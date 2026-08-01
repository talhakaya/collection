using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.SpaceArtist
{
	public class TintTextAlphaToPlanes : MonoBehaviour
	{
	    public bool invert;
	    private Text text;

	    void Start()
	    {
	        text = GetComponent<Text>();
	    }

	    void Update()
	    {
	        text.color = new Color(text.color.r, text.color.g, text.color.b, invert ? (1f - PlaneManager.openColorAlpha) : PlaneManager.openColorAlpha);
	    }
	}

}
