using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunction : MonoBehaviour {
    public List<GameObject> toFunctions;
    [SerializeField] private List<PrefabCreator> toFunctionPrefabCreator;
    [SerializeField] private GameObject interactSign;
    private float canInteract;
    private float interacting;
    private float interactingOld;
    [SerializeField] private bool killPlayer;

    void Start () {
        if (killPlayer) {
            toFunctions.Add(Player.instance.gameObject);
        }
        foreach (PrefabCreator pc in toFunctionPrefabCreator) {
            toFunctions.Add(pc.objectCreated);
            if (pc.objectCreated.GetComponent<Door>() != null) {
                pc.objectCreated.GetComponent<Door>().canBeTriggered = false;
            }
        }
	}
	
	void Update () {
        if (interacting > 0f && interactingOld <= 0f) {
            foreach (GameObject toFunction in toFunctions) {
                if (toFunction.GetComponent<Door>() != null) {
                    Door d = toFunction.GetComponent<Door>();
                    if (d.state == Door.State.Closed) {
                        d.Open();
                    }
                    else if (d.state == Door.State.Open) {
                        d.Close();
                    }
                    else {
                        throw new System.NotImplementedException();
                    }
                }
                else if (toFunction.GetComponent<Person>() != null && toFunction.GetComponent<Person>().canDie) {
                    Person p = toFunction.GetComponent<Person>();
                    if (p.state == Person.State.Alive) {
                        p.Die();
                    }
                    else if (p.state == Person.State.Dead) {
                        p.Undie();
                    }
                    else {
                        throw new System.NotImplementedException();
                    }
                }
            }
        }
        interactSign.SetActive(canInteract > 0f && interacting <= 0f);
        interactingOld = interacting;
        interacting -= Game.dt;
        canInteract -= Game.dt;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Drone>() != null) {
            Drone g = other.GetComponent<Drone>();
            if (g.interactInput) {
                interacting = 0.1f;
                g.interactInput = false;
            }
            canInteract = 0.1f;
        }
        else if (other.GetComponent<Bug>() != null) {
            Bug g = other.GetComponent<Bug>();
            if (g.interactInput) {
                interacting = 0.1f;
                g.interactInput = false;
            }
            canInteract = 0.1f;
        }
        else if (other.GetComponent<Person>() != null) {
            if (other.GetComponent<Person>().interactInput) {
                interacting = 0.1f;
            }
            if (other.GetComponent<Player>() != null) {
                if (other.GetComponent<Player>().interactInput) {
                    interacting = 0.1f;
                }
                canInteract = 0.1f;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        OnTriggerEnter2D(other);
    }
}
