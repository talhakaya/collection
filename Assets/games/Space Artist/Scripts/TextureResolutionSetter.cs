using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class TextureResolutionSetter : MonoBehaviour
	{
	    private Camera cam;
	    private int texWidth;
	    public bool pixelGlitch;
	    private bool pixelGlitchOld;
	    public Renderer onlyRendererToChange;
	    public int minPixelGlitchWidth = 16;
	    public int maxPixelGlitchWidth = 48;
	    public float pixelGlitchPeriod = 0.02f;
	    private float pixelGlitchTimer;
	    private Vector3 firstPos;

	    void Start()
	    {
	        cam = GetComponent<Camera>();
	        texWidth = cam.targetTexture.width;
	        firstPos = transform.position;
	    }

	    void Update()
	    {
	        if (pixelGlitch)
	        {
	            transform.position = firstPos + Geometry.createVector3(Random.value * 360f, Random.value * 0.1f);
	        }
	        else
	        {
	            transform.position = firstPos;
	        }

	        //if (pixelGlitch != pixelGlitchOld)
	        //{
	        //    pixelGlitchOld = pixelGlitch;
	        //    if (!pixelGlitch)
	        //    {
	        //        changeTexture(texWidth);
	        //    }
	        //}
	        //else
	        //{
	        //    if (pixelGlitch)
	        //    {
	        //        pixelGlitchTimer += Game.dt;
	        //        if (pixelGlitchTimer > pixelGlitchPeriod)
	        //        {
	        //            pixelGlitchTimer = 0f;
	        //            changeTexture(Random.Range(minPixelGlitchWidth, maxPixelGlitchWidth));
	        //        }
	        //    }
	        //}
	    }

	    void changeTexture(int resolution)
	    {
	        RenderTexture renderTexture = cam.targetTexture;
	        //cam.targetTexture = null;
	        cam.targetTexture = new RenderTexture(resolution, resolution, 24);
	        renderTexture.Release();
	        cam.Render();
	        cam.targetTexture.filterMode = FilterMode.Point;
	        if (onlyRendererToChange != null)
	        {
	            onlyRendererToChange.material.mainTexture = cam.targetTexture;
	        }
	        else
	        {
	            PlaneManager.ChangeTexture(cam.targetTexture);
	        }
	    }
	}

}
