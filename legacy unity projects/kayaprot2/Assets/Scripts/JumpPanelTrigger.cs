using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPanelTrigger : MonoBehaviour {
    private JumpPanel jumpPanel;

    void Start() {
        jumpPanel = transform.parent.GetComponent<JumpPanel>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        jumpPanel.Trigger(other);
    }

    void OnTriggerStay2D(Collider2D other) {
        jumpPanel.Trigger(other);
    }
}
