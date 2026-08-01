using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.SpaceArtist
{
	public class PlaneManager : MonoBehaviour
	{
	    public static PlaneManager instance;
	    public int numberOfPlanes;
	    public bool open;
	    private bool openOld;
	    private float openTimer;
	    private const float openPeriod = 2f;
	    public List<Transform> planes;
	    public static float openColorAlpha;

		void Awake ()
	    {
	        instance = this;
	        planes = new List<Transform>();
	        float z = 0f;
		    foreach (Transform child in transform)
	        {
	            planes.Add(child);
	            child.localPosition = Vector3.forward * z;
	            z += 0.001f;
	        }
	        SetPlanesActive();
	        openColorAlpha = 1f;
		}
		
		void Update ()
	    {
		    if (openOld != open)
	        {
	            openOld = open;
	            openTimer = openPeriod;
	            SetPlanesActive();
	        }

	        if (openTimer > 0f)
	        {
	            if (open)
	            {
	                for (int i = 0; i < planes.Count && i < numberOfPlanes; i++)
	                {
	                    planes[i].localEulerAngles = Vector3.forward * (Easing.CircEaseOut(openPeriod - openTimer, 0f, i * 360f / numberOfPlanes, openPeriod));
	                }
	                openColorAlpha = openTimer / openPeriod;
	            }
	            else
	            {
	                for (int i = 0; i < planes.Count && i < numberOfPlanes; i++)
	                {
	                    planes[i].localEulerAngles = Vector3.forward * (Easing.CircEaseIn(openTimer, 0f, i * 360f / numberOfPlanes, openPeriod));
	                }
	                openColorAlpha = (openPeriod - openTimer) / openPeriod;
	            }
	            openTimer -= Game.dt;
	        }
	        else
	        {
	            if (open)
	            {
	                for (int i = 0; i < planes.Count && i < numberOfPlanes; i++)
	                {
	                    planes[i].localEulerAngles = Vector3.forward * i * 360f / numberOfPlanes;
	                }
	                openColorAlpha = 0f;
	            }
	            else
	            {
	                for (int i = 0; i < planes.Count && i < numberOfPlanes; i++)
	                {
	                    planes[i].localEulerAngles = Vector3.forward * 0f;
	                }
	                openColorAlpha = 1f;
	            }
	        }
		}

	    void SetPlanesActive()
	    {
	        for (int i = 0; i < planes.Count; i++)
	        {
	            planes[i].gameObject.SetActive(i < numberOfPlanes);
	        }
	    }

	    public static void set(bool isOpen, int noOfPlanes)
	    {
	        instance.open = isOpen;
	        instance.numberOfPlanes = noOfPlanes;
	    }

	    public static void ChangeTexture(Texture tex)
	    {
	        for (int i = 0; i < instance.planes.Count; i++)
	        {
	            instance.planes[i].GetComponentInChildren<Renderer>().material.mainTexture = tex;
	        }
	    }
	}

}
