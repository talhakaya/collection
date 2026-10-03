using UnityEngine;
using System.Collections;

namespace Games.TalhasScreensaver
{
	public class WebcamOnMaterial : MonoBehaviour
	{
	    public static WebCamTexture CameraTexture;
	    public bool randomSpasm;
	    private Vector3 firstPos;
	    public Color color;
	    private const float colorfulAlpha = 0.01f;
	    private const float colorlessAlpha = 0.01f;

	    void Start()
	    {
	        Initialize();
	        firstPos = transform.position;
	    }

	    void Update()
	    {
	        if (randomSpasm)
	        {
	            transform.position = firstPos * Random.value;
	        }
	    }

	    // In the collection: the webcam used to stop with the application; now it has to be
	    // stopped when the toy is left.
	    void OnDestroy()
	    {
	        if (CameraTexture != null)
	        {
	            CameraTexture.Stop();
	            Destroy(CameraTexture);
	            CameraTexture = null;
	        }
	    }
	    public void Initialize()
	    {
	        if (CameraTexture == null)
	        {
	            Debug.Log("Initialize");
	            //GUITexture BackgroundTexture = gameObject.AddComponent<GUITexture>();
	            //BackgroundTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
	            //set up camera
	            WebCamDevice[] devices = WebCamTexture.devices;
	            Debug.Log("devices.Length:" + devices.Length);
	            string backCamName = "";
	            for (int i = 0; i < devices.Length; i++)
	            {
	                Debug.Log("Device:" + devices[i].name + "IS FRONT FACING:" + devices[i].isFrontFacing);

	                if (!devices[i].isFrontFacing)
	                {
	                    backCamName = devices[i].name;
	                }
	            }

	            CameraTexture = new WebCamTexture(backCamName, 1920, 1920, 30);
	            CameraTexture.filterMode = FilterMode.Point;
	            CameraTexture.Play();
	        }
	        //BackgroundTexture.texture = CameraTexture;
	        GetComponent<Renderer>().material.mainTexture = CameraTexture;
	        bool isWhite = (color.r == 1f) && (color.g == 1f) && (color.b == 1f);
	        GetComponent<Renderer>().material.SetColor("_TintColor", Game.gammaTint(new Color(color.r, color.g, color.b, isWhite?colorlessAlpha:colorfulAlpha)));
	    }
	}
}
