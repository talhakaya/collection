using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class NPC : DialogueData {
	    public bool startWithPlayer;
	    public bool talkOnlyOnce;
	    public bool shouldZoom;
	    private bool talked;
	    public string[] dialogue;
	    private float talkableTimer;

		void Start () {

		}

		void Update () {
			if (talkableTimer > 0f) {
	            talkableTimer -= Time.deltaTime;
	            if (UiManager.instance.CanPlayerMove() && (!talkOnlyOnce || !talked)) {
	                Player.instance.SetDialogueBubbleActive();
	                if (Player.instance.fire2Input) {
	                    Talk();
	                }
	            }
	        }
		}

	    private void OnTriggerEnter2D(Collider2D collision) {
	        if (collision.CompareTag("Player")) {
	            talkableTimer = 0.1f;
	        }
	    }

	    private void OnTriggerStay2D(Collider2D collision) {
	        OnTriggerEnter2D(collision);
	    }

	    private GameObject[] GetPeople(int numDialogue) {
	        GameObject[] people = new GameObject[numDialogue];
	        for (int i = 0; i < numDialogue; i++) {
	            people[i] = ((startWithPlayer && i % 2 == 0) || (!startWithPlayer && i % 2 == 1)) ? Player.instance.gameObject : gameObject;
	        }
	        return people;
	    }

	    public void Talk() {
	        if (talkOnlyOnce) {
	            if (talked) return;
	            talked = true;
	        }
	        UiManager.instance.uiDialogue.shouldZoom = shouldZoom;
	        UiManager.instance.uiDialogue.StartDialogue(GetPeople(dialogue.Length), dialogue);
	    }
	}
}
