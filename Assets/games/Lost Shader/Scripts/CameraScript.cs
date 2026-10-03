using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
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
	        gammaTint(planes.GetComponent<Renderer>().material);
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

	    // In the collection: the project is in Linear colour space, the game was made in Gamma.
	    // The echo plane's grey tint of 0.5 is doubled by its shader to draw the last frame back
	    // at full strength; read as a linear colour it came to 0.43, and the echo died in three
	    // frames. The tint is handed over so that the shader sees the numbers it was given.
	    public static void gammaTint(Material material)
	    {
	        Color tint = material.GetColor("_TintColor");
	        material.SetColor("_TintColor", new Color(Mathf.LinearToGammaSpace(tint.r), Mathf.LinearToGammaSpace(tint.g), Mathf.LinearToGammaSpace(tint.b), tint.a));
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
}
