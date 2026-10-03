using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.Oracle
{
	// In the collection: rewritten for URP, which never calls OnPostRender. The original
	// read the screen back, changed every pixel in a loop and drew the result over the
	// screen. Here the camera renders into a texture, and a full-screen image behind all
	// other UI shows that texture through PassThru.shader, which does the same arithmetic.
	public class RenderGrayScale : MonoBehaviour {

		RenderTexture screenTexture;
		public Material mat;
		Material _mat;
	    public float greyScaleRatio;
	    public int mode = 0;
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
			if(screenTexture != null)
			{
				cam.targetTexture = null;
				screenTexture.Release();
				Destroy (screenTexture);
			}
			screenTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
			screenTexture.filterMode = FilterMode.Point;
			lastWidth = Screen.width;
			lastHeight = Screen.height;
			cam.targetTexture = screenTexture;
			image.texture = screenTexture;
		}

	    void Update()
	    {
	        if (TaloketoInputManager.GetButtonDown("Mode")) // In the collection: was the Space key
	        {
	            mode++;
	            mode = mode % 4;
	        }
			// In the collection: the Escape-quit is gone (the collection has its own exit).

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

			if(Screen.width != lastWidth || Screen.height != lastHeight)
			{
				CreateTexture();
			}
			_mat.SetFloat("_Ratio", greyScaleRatio);
			_mat.SetFloat("_Mode", mode);
			_mat.SetFloat("_Scanline", Random.Range(0, 4));
			_mat.SetFloat("_Seed", Random.Range(0f, 100f));
			_mat.SetFloat("_Rows", Screen.height);
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
