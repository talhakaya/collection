using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {
    public bool done;
    public bool disableOnFinish;

    public void ResetState() {
        done = false;
        gameObject.SetActive(true);
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(0f, 0f);
        body.angularVelocity = 0f;
    }
}
