using UnityEngine;
using System.Collections;

public class RenderGrayScale : MonoBehaviour {

	Texture2D screenTexture;
	public Material mat;
	Material _mat;
    public float greyScaleRatio;
    public int mode = 0;
	
	int lastWidth;
	int lastHeight;

	void Awake()
	{
		_mat = new Material(mat);
		CreateTexture();
	}

	void CreateTexture()
	{
		if(screenTexture != null)
		{
			Destroy (screenTexture);
		}
		screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
		lastWidth = Screen.width;
		lastHeight = Screen.height;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mode++;
            mode = mode % 4;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (mode == 0)
        {
            greyScaleRatio = 0;
        }
        if (mode == 1)
        {
            greyScaleRatio = 1;
        }
        else if (mode == 2)
        {
            greyScaleRatio = 0;
        }
        else if (mode == 3)
        {
            greyScaleRatio += Time.deltaTime;
        }
    }
	
	void OnPostRender()
	{
		if(Screen.width != lastWidth || Screen.height != lastHeight)
		{
			CreateTexture();
		}
		screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

		Color32[] ca = screenTexture.GetPixels32();


        //for (int i = 0; i < ca.Length; i++)
        //{
        //    Color32 c = ca[i];
        //    float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
        //    byte grayByte = (byte) gray;
        //    c.r = grayByte;
        //    c.g = grayByte;
        //    c.b = grayByte;
        //    ca[i] = c;
        //}

        int scanline = Random.Range(0, 4);
        for (int i = 0; i < Screen.height; i++)
        {
            if (i % 4 == scanline && mode != 0)
            {
                float scanlineDegree = Random.Range(2f, 3f);
                for (int j = 0; j < Screen.width; j++)
                {
                    if (mode == 2)
                    {
                        greyScaleRatio = Random.Range(0f, 1f);
                    }
                    Color32 c = ca[i * Screen.width + j];
                    float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
                    gray = gray / scanlineDegree;
                    byte grayByte = (byte)gray;
                    c.r = grayByte;
                    c.g = grayByte;
                    c.b = grayByte;
                    ca[i * Screen.width + j] = c;
                }
            }
            else
            {
                for (int j = 0; j < Screen.width; j++)
                {
                    if (mode == 2)
                    {
                        greyScaleRatio = Random.Range(0f, 1f);
                    }
                    Color32 c = ca[i * Screen.width + j];
                    float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
                    byte grayByte = (byte)gray;
                    c.r = (byte)(grayByte * greyScaleRatio + (1 - greyScaleRatio) * c.r);
                    c.g = (byte)(grayByte * greyScaleRatio + (1 - greyScaleRatio) * c.g);
                    c.b = (byte)(grayByte * greyScaleRatio + (1 - greyScaleRatio) * c.b);
                    ca[i * Screen.width + j] = c;
                }
            }
		}

		screenTexture.SetPixels32(ca);
		screenTexture.Apply();
		_mat.mainTexture = screenTexture;

		// Draw a quad over the whole screen with the above shader
		GL.PushMatrix ();
		GL.LoadOrtho ();
		//for (var i = 0; i < mat.passCount; ++i) {
			_mat.SetPass(0);
			GL.Begin( GL.QUADS );
			GL.Vertex3( 0f, 0f, 0.1f );
			GL.Vertex3( 1f, 0f, 0.1f );
			GL.Vertex3( 1f, 1f, 0.1f );
			GL.Vertex3( 0f, 1f, 0.1f );
			GL.End();
		//}
		GL.PopMatrix ();
	}
}
