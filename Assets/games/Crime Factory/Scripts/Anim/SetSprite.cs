using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class SetSprite : MonoBehaviour {
	    private HoldablePhysBox physBox;
	    private SpriteRenderer sr;
	    private Sprite ifDefault;
	    public Sprite ifAlsoPlatform;

	    void Start () {
	        physBox = GetComponent<HoldablePhysBox>();
	        sr = GetComponent<SpriteRenderer>();
	        ifDefault = sr.sprite;
	    }

		void Update () {
	        if (physBox.isAlsoPlatform && ifAlsoPlatform != null) {
	            sr.sprite = ifAlsoPlatform;
	        }
	        else {
	            sr.sprite = ifDefault;
	        }
		}
	}
}
