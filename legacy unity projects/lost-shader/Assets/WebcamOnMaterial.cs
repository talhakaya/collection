using UnityEngine;
using System.Collections;

public class WebcamOnMaterial : MonoBehaviour
{
    public static WebcamOnMaterial instance;
    public static WebCamTexture CameraTexture;
    public float activeCount;
    private Vector3 firstPos;

    void Start()
    {
        instance = this;
        Initialize();
        firstPos = transform.position;
    }

    void Update()
    {
        if (activeCount > 0)
        {
            activeCount -= Game.dt;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
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

            CameraTexture = new WebCamTexture(backCamName, 10000, 10000, 30);
            CameraTexture.Play();
        }
        //BackgroundTexture.texture = CameraTexture;
        GetComponent<Renderer>().material.mainTexture = CameraTexture;
    }
}
