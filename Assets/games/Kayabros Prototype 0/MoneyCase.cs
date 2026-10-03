using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.KayabrosPrototype0
{
	public class MoneyCase : MonoBehaviour {
	    private Rigidbody2D body;
	    public Rigidbody cube;
	    public bool isHolding;

	    void Start () {
	        body = GetComponent<Rigidbody2D>();
	    }

	    void Update() {
	        if (isHolding) {
	            if (!Game.instance.player.visible || Game.instance.player.slot != 0 ||TaloketoInputManager.GetButtonUp("Fire1")) {
	                isHolding = false;
	                if (transform.parent != null) {
	                    transform.SetParent(null);

	                }
	            }
	        }
	    }

	    void FixedUpdate() {
	        cube.position = new Vector3(body.position.x, body.position.y, isHolding ? -2f : cube.position.z);
	        if (cube.isKinematic != isHolding) {
	            cube.isKinematic = isHolding;
	        }
	    }

	    void OnTriggerStay2D(Collider2D other) {
	        if (!isHolding && other.CompareTag("Player") && Game.instance.player.slot == 0 && TaloketoInputManager.GetButton("Fire1")) {
	            isHolding = true;
	            Game.instance.player.visible = true;
	            cube.angularVelocity = new Vector3(Random.Range(-300f, 300f), Random.Range(-300f, 300f), Random.Range(-300f, 300f));
	            transform.SetParent(Game.instance.player.transform);
	        }
	    }
	}
}
