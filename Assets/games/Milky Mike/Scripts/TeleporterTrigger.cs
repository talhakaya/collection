using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class TeleporterTrigger : MonoBehaviour {
	    private Teleporter teleporter;

		void Start () {
	        teleporter = transform.parent.GetComponent<Teleporter>();
	    }

	    void OnTriggerEnter2D(Collider2D other) {
	        teleporter.Trigger(other);
	    }

	    void OnTriggerStay2D(Collider2D other) {
	        teleporter.Trigger(other);
	    }
	}
}
