using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remote : MonoBehaviour {
    public Gadget gadget;
    [HideInInspector] public bool isGadgetOut;
    private Vector2 input;
    private bool interactInput;
    private bool fireInput;
    public Vector3 gadgetScale;
    [HideInInspector] public Person person;

    void Start () {
        gadget.remote = this;
        if (gadgetScale.x == 0f) {
            gadgetScale = gadget.transform.localScale;
        }
        ResetState();
    }
	
	void Update () {
        if (!isGadgetOut) {
            if (interactInput) {
                SendGadget();
            }
        }
        else if (fireInput) {
            gadget.SetInput(input.x, input.y, interactInput);
        }
    }

    public void ResetState() {
        if (gadgetScale.x == 0f) {
            gadgetScale = gadget.transform.localScale;
        }
        gadget.gameObject.SetActive(true);
        Collider2D[] cols = gadget.GetComponents<Collider2D>();
        foreach (Collider2D col in cols) {
            col.enabled = false;
        }
        if (!gadget.wearable) {
            gadget.GetComponent<Rigidbody2D>().isKinematic = true;
        }
        gadget.transform.parent = transform;
        gadget.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        gadget.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        gadget.enabled = false;
        if (gadget.GetComponent<Person>() != null) {
            gadget.GetComponent<Person>().enabled = false;
        }
        if (gadget.GetComponent<LineRenderer>() != null) {
            gadget.GetComponent<LineRenderer>().enabled = false;
        }
        if (gadget.GetComponent<Teleporter>() != null) {
            gadget.GetComponent<Teleporter>().telPoint.gameObject.SetActive(false);
            gadget.GetComponent<Teleporter>().trigger.gameObject.SetActive(false);
        }
        if (gadget.GetComponent<JumpPanel>() != null) {
            gadget.GetComponent<JumpPanel>().direction.gameObject.SetActive(false);
            gadget.GetComponent<JumpPanel>().trigger.gameObject.SetActive(false);
        }
        if (gadget.GetComponent<Rigidbody2D>() != null) {
            gadget.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            gadget.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        isGadgetOut = false;
    }

    public void SendGadget() {
        Collider2D[] cols = gadget.GetComponents<Collider2D>();
        foreach (Collider2D col in cols) {
            col.enabled = true;
        }
        if (!gadget.wearable) {
            gadget.GetComponent<Rigidbody2D>().isKinematic = false;
            gadget.transform.parent = null;
            gadget.transform.localScale = gadgetScale;
            if (gadget.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.FreezeRotation) {
                gadget.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
        else {
            gadget.transform.parent = Player.person.bodyTransform;
            gadget.transform.localScale = gadgetScale;
            gadget.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            gadget.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        gadget.enabled = true;
        if (gadget.GetComponent<Person>() != null) {
            gadget.GetComponent<Person>().enabled = true;
        }
        if (gadget.GetComponent<LineRenderer>() != null) {
            gadget.GetComponent<LineRenderer>().enabled = true;
        }
        if (gadget.GetComponent<Teleporter>() != null) {
            gadget.GetComponent<Teleporter>().telPoint.gameObject.SetActive(true);
            gadget.GetComponent<Teleporter>().trigger.gameObject.SetActive(true);
            gadget.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        if (gadget.GetComponent<JumpPanel>() != null) {
            gadget.GetComponent<JumpPanel>().direction.gameObject.SetActive(true);
            gadget.GetComponent<JumpPanel>().trigger.gameObject.SetActive(true);
            gadget.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        isGadgetOut = true;
    }

    public void SetInput(Person person, float horizontalInput, float verticalInput, bool interactInput, bool fireInput) {
        this.person = person;
        input = new Vector2(horizontalInput, verticalInput);
        this.interactInput = interactInput;
        this.fireInput = fireInput;
    }
}
