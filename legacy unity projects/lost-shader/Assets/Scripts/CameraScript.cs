using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public static GameObject followObject;
    private bool moving;
    public float minDistance;
    public float maxDistance;
    private static CameraScript instance;
    public Transform planes;
    public static bool inMiniGame;
    public SpriteRenderer black;

	void Start ()
    {
        instance = this;
	}
	
	void Update ()
    {
	    if (followObject != null)
        {
            Vector3 deltaPos = followObject.transform.position - transform.position;
            deltaPos.z = 0f;
            float distance = Geometry.lengthOfVector3(deltaPos);
            if (moving)
            {
                if (distance <= minDistance)
                {
                    moving = false;
                }
                transform.position += deltaPos * Game.dt;
            }
            else
            {
                if (distance >= maxDistance)
                {
                    moving = true;
                }
            }
        }

        if (black.color.a > 0f && black.color.a < 1f)
        {
            black.color = new Color(0f, 0f, 0f, black.color.a + Game.dt);
        }
	}

    public static void PlanesUp()
    {
        instance.black.color = new Color(0f, 0f, 0f, Game.dt);
        //instance.planes.localPosition = Vector3.forward * 7f;
        //instance.planes.gameObject.SetActive(false);
        WebcamOnMaterial.instance.activeCount = 1f;
        inMiniGame = true;
    }

    public static void PlanesDown()
    {
        instance.black.color = new Color(0f, 0f, 0f, 0f);
        //instance.planes.localPosition = Vector3.forward * 21f;
        //instance.planes.gameObject.SetActive(true);
        inMiniGame = false;
    }
}
