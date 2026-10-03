using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class ScaledUp : MonoBehaviour
	{
	    public GameObject enableObject;
	    public GameObject destroyObject;
	    public float chanceOfCam;
		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
		    if (transform.lossyScale.x >= 1f && transform.lossyScale.y >= 1f)
	        {
	            if (enableObject != null)
	            {
	                enableObject.SetActive(true);
	            }
	            if (destroyObject != null)
	            {
	                Destroy(destroyObject);
	            }
	            if (Random.value < chanceOfCam)
	            {
	                WebcamOnMaterial.instance.activeCount = 1f;
	            }
	            enabled = false;
	        }
		}
	}
}
