using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public class cameraScript : MonoBehaviour {

	    public Transform focus;
	    public float cameraSpeed;

		void Start ()
	    {
		
		}
		
		void Update ()
	    {
	        if (focus != null)
	        {
	            Vector3 focusPos = focus.position * 0.67f + MousePosition.get * 0.33f;
	            if (Geometry.lengthOfVector3(MousePosition.get - focus.position) > 300)
	            {
	                focusPos = transform.position;
	            }
	            transform.position += (focusPos - transform.position) * cameraSpeed * Game.dt;

	            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
	        }
		    
		}
	}

}
