using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Rotate3D : MonoBehaviour
	{
	    public float rotationMultiplier = 30f;
	    private Vector3 mousePosOld;
	    public bool limitY;
	    public float maxY;

		void Start ()
	    {
	        mousePosOld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	    }
		
		void Update ()
	    {
	        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        if (Input.GetMouseButton(0))
	        {
	            //Vector3 deltaRotationVec = rotationMultiplier * (mousePos - mousePosOld);
	            Vector3 deltaRotationVec = rotationMultiplier * new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
	            //transform.eulerAngles += new Vector3(deltaRotationVec.y, 0f, deltaRotationVec.x);
	            transform.Rotate(new Vector3(deltaRotationVec.y, -deltaRotationVec.x, 0f), Space.World);
	            if (limitY)
	            {
	                if (transform.eulerAngles.x > maxY && transform.eulerAngles.x < 360f - maxY)
	                {
	                    if (transform.eulerAngles.x < 180f)
	                    {
	                        transform.eulerAngles = new Vector3(maxY, transform.eulerAngles.y, transform.eulerAngles.z);
	                    }
	                    else
	                    {
	                        transform.eulerAngles = new Vector3(360f - maxY, transform.eulerAngles.y, transform.eulerAngles.z);
	                    }
	                }
	                if (transform.eulerAngles.z > maxY && transform.eulerAngles.z < 360f - maxY)
	                {
	                    if (transform.eulerAngles.z < 180f)
	                    {
	                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, maxY);
	                    }
	                    else
	                    {
	                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 360f - maxY);
	                    }
	                }
	            }
	        }
	        mousePosOld = mousePos;
	    }
	}

}
