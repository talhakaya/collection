using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public class RenderGrayScale : MonoBehaviour {

		RenderTexture renderTexture;
	    public Camera cam;
	    public Renderer renderQuad;
	    Texture2D quadTexture;

	    public float greyScaleRatio;
	    public float noiseRatioSet;
	    private float noiseRatio;
	    public int screenHeight = 180;
		
		int lastWidth;
		int lastHeight;
	    public int visibleCircleRadius;
	    public float coneAngle;
	    public static RenderGrayScale instance;

		void Awake()
		{
			CreateTexture();
	        instance = this;
		}

		void CreateTexture()
		{
			if(renderTexture != null)
			{
				Destroy (renderTexture);
	        }
	        float screenRatio = 1f * Screen.width / Screen.height;
	        int width = Mathf.RoundToInt(screenHeight * screenRatio);
	        int height = screenHeight;
	        renderTexture = new RenderTexture(width, height, 32);
			lastWidth = width;
			lastHeight = height;

	        quadTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
	        quadTexture.filterMode = FilterMode.Point;

	        cam.targetTexture = renderTexture;

	        renderQuad.material.SetTexture("_BaseMap", quadTexture);
	        renderQuad.transform.localScale = new Vector3(9f * screenRatio, 9f, 1f);
	    }
		
		void Update()
		{
	        if (noiseRatio < noiseRatioSet)
	        {
	            noiseRatio += Game.dt * 10f;
	        }
	        else if (noiseRatio > noiseRatioSet)
	        {
	            noiseRatio -= Game.dt * 10f;
	        }
	        float screenRatio = 1f * Screen.width / Screen.height;
	        int width = Mathf.RoundToInt(screenHeight * screenRatio);
	        int height = screenHeight;
	        if (width != lastWidth || height != lastHeight)
			{
				CreateTexture();
			}

			Color32[] ca = GetPixelsFromRenderTexture();

	        for (int i = 0; i < height; i++)
	        {
	            float scanlineDegree = Random.Range(0.6f, 1.4f);
	            for (int j = 0; j < width; j++)
	            {
	                float greyScaleRatio2 = greyScaleRatio + Random.Range(-0.25f, 0.25f);
	                Color32 c = ca[i * width + j];
	                float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
	                float noise = Random.Range(-noiseRatio, noiseRatio);
	                gray = gray / scanlineDegree;
	                byte grayByte = (byte)gray;
	                c.r = (byte)(grayByte * greyScaleRatio2 + (1 - greyScaleRatio2) * c.r + noise);
	                c.g = (byte)(grayByte * greyScaleRatio2 + (1 - greyScaleRatio2) * c.g + noise);
	                c.b = (byte)(grayByte * greyScaleRatio2 + (1 - greyScaleRatio2) * c.b + noise);
	                ca[i * width + j] = c;
	            }
	        }

	        quadTexture.SetPixels32(ca);
	        quadTexture.Apply();
	    }

	    public Color32[] GetPixelsFromRenderTexture()
	    {
	        // 1. Store previous active RenderTexture
	        RenderTexture prevActive = RenderTexture.active;

	        // 2. Set current RenderTexture as active
	        RenderTexture.active = renderTexture;

	        // 3. Create a Texture2D matching dimensions and read pixels
	        quadTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
	        quadTexture.Apply();

	        // 4. Restore original active RenderTexture
	        RenderTexture.active = prevActive;

	        // 5. Get pixel array
	        Color32[] pixels = quadTexture.GetPixels32();

	        return pixels;
	    }
	}

}
