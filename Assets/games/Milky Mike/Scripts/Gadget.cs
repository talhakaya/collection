using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Gadget : MonoBehaviour {
	    protected Vector2 input;
	    [HideInInspector] public bool interactInput;
	    public bool wearable;
	    public Sprite icon;
	    [HideInInspector] public Remote remote;
	    public bool canDie;

	    public virtual void SetInput(float horizontalInput, float verticalInput, bool interactInput) {
	        SetInput(horizontalInput, verticalInput);
	        this.interactInput = interactInput;
	    }

	    public virtual void SetInput(float horizontalInput, float verticalInput) {
	        input = new Vector2(horizontalInput, verticalInput);
	    }

	    public void GetShot(RaycastHit2D hit) {
	        if (canDie) {
	            ObjectPool.explosionPool.Create(new Vector3(transform.position.x, transform.position.y, -1));
	            gameObject.SetActive(false);
	        }
	    }
	}
}
