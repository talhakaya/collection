using UnityEngine;
using System.Collections;

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
		i = Mathf.FloorToInt((time % (period * textures.Length)) / period);
        GetComponent<MeshRenderer>().material.mainTexture = textures[i];
	}
}
