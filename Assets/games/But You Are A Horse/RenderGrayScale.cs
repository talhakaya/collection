using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.ButYouAreAHorse
{
	// In the collection: rewritten for URP, which never calls OnPostRender. The original
	// read the screen back, changed every pixel in a loop and drew the result over the
	// screen. Here the camera renders into a texture, and a full-screen image behind all
	// other UI shows that texture through PassThru.shader, which does the same arithmetic.
	public class RenderGrayScale : MonoBehaviour
	{

	    public float ratio;
	    public Material mat;
	    Material _mat;
	    RenderTexture screenTexture;
	    Camera cam;
	    GameObject screen;
	    RawImage image;

	    int lastWidth;
	    int lastHeight;

	    void Awake()
	    {
	        cam = GetComponent<Camera>();
	        _mat = new Material(mat);

	        screen = new GameObject("RenderGrayScale screen");
	        Canvas canvas = screen.AddComponent<Canvas>();
	        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
	        canvas.sortingOrder = -100;
	        GameObject child = new GameObject("image");
	        child.transform.SetParent(screen.transform, false);
	        image = child.AddComponent<RawImage>();
	        image.raycastTarget = false;
	        image.material = _mat;
	        RectTransform rect = image.rectTransform;
	        rect.anchorMin = Vector2.zero;
	        rect.anchorMax = Vector2.one;
	        rect.offsetMin = Vector2.zero;
	        rect.offsetMax = Vector2.zero;

	        CreateTexture();
	    }

	    void CreateTexture()
	    {
	        if (screenTexture != null)
	        {
	            cam.targetTexture = null;
	            screenTexture.Release();
	            Destroy(screenTexture);
	        }
	        screenTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
	        lastWidth = Screen.width;
	        lastHeight = Screen.height;
	        cam.targetTexture = screenTexture;
	        image.texture = screenTexture;
	    }

	    void Update()
	    {
	        if (Screen.width != lastWidth || Screen.height != lastHeight)
	        {
	            CreateTexture();
	        }
	        _mat.SetFloat("_Ratio", ratio);
	    }

	    void OnDestroy()
	    {
	        if (cam != null)
	        {
	            cam.targetTexture = null;
	        }
	        if (screenTexture != null)
	        {
	            screenTexture.Release();
	            Destroy(screenTexture);
	        }
	        if (screen != null)
	        {
	            Destroy(screen);
	        }
	    }
	}
}
