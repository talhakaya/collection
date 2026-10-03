using UnityEngine;
using System.Collections;

namespace Games.WallStains
{
	public class TextureChange : MonoBehaviour {

	    public Texture[] textures;
	    public float period = 0.2f;
	    private float time = 0f;
	    private int i = 0;

		void Start ()
	    {

		}

		void Update ()
	    {
	        time += Time.deltaTime;
			// In the collection: the float maths can land on textures.Length, so it is clamped.
			i = Mathf.Min(Mathf.FloorToInt((time % (period * textures.Length)) / period), textures.Length - 1);
	        GetComponent<MeshRenderer>().material.mainTexture = textures[i];
		}
	}
}
