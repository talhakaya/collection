using UnityEngine;
using System.Collections;

public class PlaneRandomColorSprite : MonoBehaviour
{
    private Material mat;
    public Texture[] textures;

	void Start ()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = Game.colors[Random.Range(0, Game.colors.Length)];
        if (textures.Length > 0)
        {
            mat.mainTexture = textures[Random.Range(0, textures.Length)];
        }
	}
	
	void Update ()
    {
	    
	}
}
