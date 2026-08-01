using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class SpotlightManager : MonoBehaviour
	{
	    public bool rotate;
	    public bool xScale;
	    public bool yScale;
	    private float rotateTimer;
	    private float xTimer;
	    private float yTimer;
	    private const float rotatePeriod = 5f;
	    private const float xPeriod = 3f;
	    private const float yPeriod = 4f;

		void Start ()
	    {
		
		}
		
		void Update ()
	    {
	        if (rotate)
	        {
	            rotateTimer += Game.dt;
	            rotateTimer = rotateTimer % (2f * rotatePeriod);
	            float r = rotateTimer / rotatePeriod;
	            if (r >= 1f)
	            {
	                r = 2f - r;
	            }
	            transform.eulerAngles = Vector3.forward * r * 180f;
	        }
	        if (xScale)
	        {
	            xTimer += Game.dt;
	            xTimer = xTimer % (2f * xPeriod);
	            float r = xTimer / xPeriod;
	            if (r >= 1f)
	            {
	                r = 2f - r;
	            }
	            transform.localScale = new Vector3(1f - 2f * r, transform.localScale.y, transform.localScale.z);
	        }
	        if (yScale)
	        {
	            yTimer += Game.dt;
	            yTimer = yTimer % (2f * yPeriod);
	            float r = yTimer / yPeriod;
	            if (r >= 1f)
	            {
	                r = 2f - r;
	            }
	            transform.localScale = new Vector3(transform.localScale.x, 1f - 2f * r, transform.localScale.z);
	        }
		}
	}

}
