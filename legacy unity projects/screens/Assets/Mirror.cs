using UnityEngine;
using System.Collections;

public class Mirror : MonoBehaviour {
    private RenderTexture renderTexture;
    public Renderer renderer;
    public Camera camera;

	void Start () {
        renderTexture = new RenderTexture(640, 360, 24);
        camera.targetTexture = renderTexture;
        renderer.material.mainTexture = renderTexture;
    }
	
	void Update () {
	
	}
}
