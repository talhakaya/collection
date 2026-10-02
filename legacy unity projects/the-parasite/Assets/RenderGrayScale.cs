using UnityEngine;
using System.Collections;

public class RenderGrayScale : MonoBehaviour
{

    public static float ratio;
    Texture2D screenTexture;
    public Material mat;
    Material _mat;

    int lastWidth;
    int lastHeight;

    void Awake()
    {
        _mat = new Material(mat);
        CreateTexture();
    }

    void CreateTexture()
    {
        if (screenTexture != null)
        {
            Destroy(screenTexture);
        }
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void OnPostRender()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            CreateTexture();
        }
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        Color32[] ca = screenTexture.GetPixels32();

        if (ratio == 0)
        {
            for (int i = 0; i < ca.Length; i++)
            {
                Color32 c = ca[i];
                float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
                byte grayByte = (byte)gray;
                c.r = grayByte;
                c.g = grayByte;
                c.b = grayByte;
                ca[i] = c;
            }
        }
        else if (ratio < 1)
        {
            for (int i = 0; i < ca.Length; i++)
            {
                Color32 c = ca[i];
                float gray = ((int)c.r + (int)c.g + (int)c.b) / 3.0f;
                byte grayByte = (byte)gray;
                c.r = (byte)(ratio * c.r + (1f - ratio) * grayByte);
                c.g = (byte)(ratio * c.g + (1f - ratio) * grayByte);
                c.b = (byte)(ratio * c.b + (1f - ratio) * grayByte);
                ca[i] = c;
            }
        }

        screenTexture.SetPixels32(ca);
        screenTexture.Apply();
        _mat.mainTexture = screenTexture;

        // Draw a quad over the whole screen with the above shader
        GL.PushMatrix();
        GL.LoadOrtho();
        //for (var i = 0; i < mat.passCount; ++i) {
        _mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Vertex3(0f, 0f, 0.1f);
        GL.Vertex3(1f, 0f, 0.1f);
        GL.Vertex3(1f, 1f, 0.1f);
        GL.Vertex3(0f, 1f, 0.1f);
        GL.End();
        //}
        GL.PopMatrix();
    }
}
