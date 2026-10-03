using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.KayabrosPrototype0
{
	public class Exit : MonoBehaviour {
	    private Light lightt;
	    private float lighttIntensity;
	    private float exitTimer;
	    private float exitPeriod = 2f;
	    private float caseTimer;
	    private float casePeriod = 2f;


	    void Start () {
	        lightt = GetComponent<Light>();
	        lighttIntensity = lightt.intensity;
	    }

	    void Update() {
	        if (Game.instance.moneyCase.activeSelf) {
	            lightt.color = new Color(1f, 0f, 1f);
	            lightt.intensity = 1f - caseTimer / casePeriod;
	        }
	        else {
	            lightt.color = new Color(0f, 1f, 0f);
	            lightt.intensity = 1f - exitTimer / exitPeriod;
	        }
	    }

	    void OnTriggerEnter2D(Collider2D other) {
	        if (other.CompareTag("Case")) {

	        }
	    }

	    void OnTriggerStay2D(Collider2D other) {
	        if (other.CompareTag("Player") && Game.instance.player.visible) {
	            if (!Game.instance.moneyCase.activeSelf) {
	                exitTimer += Time.fixedDeltaTime;
	                if (exitTimer >= exitPeriod) {
	                    Game.instance.player.gameObject.SetActive(false);
	                    Game.failed = true;
	                }
	            }
	        }
	        if (other.CompareTag("Case") && Game.instance.player.visible) {
	            caseTimer += Time.fixedDeltaTime;
	            if (caseTimer >= casePeriod) {
	                Game.instance.moneyCase.SetActive(false);
	            }
	        }
	    }

	    void OnTriggerExit2D(Collider2D other) {
	        if (other.CompareTag("Player")) {
	            exitTimer = 0f;
	        }
	        if (other.CompareTag("Case")) {
	            caseTimer = 0f;
	        }
	    }
	}
}
