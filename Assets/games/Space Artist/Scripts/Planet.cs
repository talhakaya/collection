using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class Planet : MonoBehaviour
	{
	    public float maxDistance;
	    public float minDistance;
	    public float maxForce;
	    public float minForce;
	    public float maxDistanceForRotation;
	    public float rotateSpeed;
	    public TintScript areaTint;
	    private const float areaPeriod = 2.5f;
	    private float scaleMultiplier;

	    void Start()
	    {
	        scaleMultiplier = transform.lossyScale.x / 4f;
	        maxDistance *= scaleMultiplier;
	        minDistance *= scaleMultiplier;
	        maxForce *= scaleMultiplier;
	        minForce *= scaleMultiplier;
	        maxDistanceForRotation *= scaleMultiplier;
	        scaleAreaTint();
	    }

	    void Update()
	    {
	        scaleAreaTint();

	        if (PlaneManager.instance.open && PlayerScript.instance.resetTimer <= 0f)
	        {
	            gravity(PlayerScript.instance.body);
	        }
	    }

	    void scaleAreaTint()
	    {
	        float timer = Game.time;
	        timer = timer % (2f * areaPeriod);
	        float r = timer / areaPeriod;
	        if (r >= 1f)
	        {
	            r = 2f - r;
	        }
	        float areaScale = (scaleMultiplier * maxDistance * 2f + (-0.5f + r)) / transform.lossyScale.x;
	        areaTint.transform.localScale = new Vector3(areaScale, areaScale, 1f);
	    }

		public void gravity(Rigidbody2D body)
	    {
	        float distance = Geometry.lengthOfVector3(transform.position - body.transform.position);
	        float angle = Geometry.angleOfVector3(transform.position - body.transform.position);
	        if (distance < maxDistanceForRotation)
	        {
	            float angleToRotateTo = Geometry.differenceOfAnglesNegative(angle, body.transform.eulerAngles.z - 90f);

	            if (angleToRotateTo < -rotateSpeed * Game.dt)
	            {
	                body.transform.eulerAngles -= Vector3.forward * rotateSpeed * Game.dt;
	            }
	            else if (angleToRotateTo > rotateSpeed * Game.dt)
	            {
	                body.transform.eulerAngles += Vector3.forward * rotateSpeed * Game.dt;
	            }
	        }

	        if (distance >= minDistance && distance <= maxDistance)
	        {
	            body.AddForce(Geometry.normalizeVector3(transform.position - body.transform.position, (minForce + (maxForce - minForce) * ((maxDistance - distance - minDistance) / (maxDistance - minDistance))) * Game.dt));
	        }
	    }
	}

}
