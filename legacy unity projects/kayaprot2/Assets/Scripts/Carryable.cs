using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : MonoBehaviour {
    [HideInInspector] public Rigidbody2D body;
	void Awake() {
        body = GetComponent<Rigidbody2D>();
        Transform p = transform.parent;
        while (p != null) {
            if (p.GetComponent<Level>() != null) {
                p.GetComponent<Level>().carryables.Add(this);
                break;
            }
            p = p.parent;
        }
    }
}
