using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.LostShader
{
	public class FaceLookAround : MonoBehaviour {

	    public float maxSpeed;
	    public float acceleration;
	    private float speed;
	    private Vector3 localPos;
		void Start ()
	    {
	        localPos = transform.localPosition;
		}

		void Update () {
	        Vector2 dir = new Vector2(TaloketoInputManager.GetAxis("Horizontal"), TaloketoInputManager.GetAxis("Vertical"));
	        if (TaloketoInputManager.GetMouseButton(0))
	        {
	            dir = Camera.main.ScreenToWorldPoint(TaloketoInputManager.mousePosition) - transform.position;
	        }
	        if (dir.x != 0f || dir.y != 0f)
	        {
	            speed = acceleration;
	        }
	        else
	        {
	            speed = 0f;
	            //speed -= Game.dt * acceleration;
	            //if (speed < 0f)
	            //{
	            //}
	            transform.localPosition += (localPos - transform.localPosition) * Game.dt;
	        }
	        transform.localPosition += Geometry.normalizeVector3(dir, speed);
	        if (Geometry.lengthOfVector3(localPos - transform.localPosition) > maxSpeed)
	        {
	            transform.localPosition = localPos + Geometry.normalizeVector3(transform.localPosition - localPos, maxSpeed);
	        }
		}
	}
}
