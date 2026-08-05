using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Abused
{
	public class Player : MonoBehaviour
	{
	    public float speed;
	    public SpriteRenderer flashLight;
	    public float flashLightRatio;

		void Start ()
	    {
		
		}
		
		void Update ()
	    {
	        if (new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")) != Vector2.zero)
	        {
	            GetComponent<AudioSource>().volume = 0.6f;
	        }
	        else
	        {
	            GetComponent<AudioSource>().volume = 0f;
	        }
	        GetComponent<Rigidbody2D>().AddForce(new Vector2(TaloketoInputManager.GetAxis("Horizontal"), TaloketoInputManager.GetAxis("Vertical")) * speed * Game.dt);
	        flashLight.color = new Color(1f, 1f, 1f, flashLightRatio + Random.Range(0f, 0.4f));
	        float deltaX = MousePosition.x - transform.position.x;
	        float deltaY = MousePosition.y - transform.position.y;
	        transform.eulerAngles = Vector3.forward * Mathf.Atan2(deltaY, deltaX) * 180f / Mathf.PI;
		}
	}

}
