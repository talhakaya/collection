using UnityEngine;
using Collection.Controls;
using System.Collections;

namespace Games.WhereIsHe
{
	public class PlayerScript : MonoBehaviour
	{
	    public static PlayerScript instance;
	    private Weapon weapon;
	    private PlatformerController platformerController;
	    private float weaponTimer;
	    public GameObject[] sprites;

		void Awake ()
	    {
	        instance = this;
	        platformerController = GetComponent<PlatformerController>();
		}

		public void ForceUpdate ()
	    {
		    if (weapon != null)
	        {
	            if (weaponTimer < weapon.period)
	            {
	                weaponTimer += Game.dt;
	            }
	            else if (TaloketoInputManager.GetButtonDown("Fire"))
	            {
	                weaponTimer = 0f;
	                float angle = platformerController.sprite.transform.eulerAngles.z;
	                if (platformerController.sprite.transform.localScale.x < 0f)
	                {
	                    angle += 180f;
	                }
	                weapon.shoot(angle);
	            }
	        }
		}

	    public void getWeapon(Weapon w)
	    {
	        dropWeapon();
	        weapon = w;
	        w.pickedUp = true;
	        w.GetComponent<Tilt>().enabled = false;
	        w.transform.parent = platformerController.sprite.transform;
	        w.transform.localScale = Vector3.one;
	        w.transform.localEulerAngles = Vector3.zero;
	        w.transform.localPosition = Vector3.forward;
	        //w.transform.localScale = new Vector3(Mathf.Abs(w.transform.localScale.x), w.transform.localScale.y, w.transform.localScale.z);
	    }

	    public void dropWeapon()
	    {
	        if (weapon != null)
	        {
	            weapon.pickedUp = false;
	            weapon.transform.localPosition = Vector3.up;
	            weapon.transform.parent = WorldWander.currentRoom;
	            weapon.transform.localScale = new Vector3(Mathf.Abs(weapon.transform.localScale.x), weapon.transform.localScale.y, weapon.transform.localScale.z);
	            weapon.GetComponent<Tilt>().enabled = true;
	            weapon = null;
	        }
	    }
	}
}
