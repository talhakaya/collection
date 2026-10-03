using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Jetpack : Gadget {
	    public float jumpForceCont;
	    public float fuel;
	    public ObjectPool flames;
	    public Transform flamePoint;
	    private float flameTimer;
	    private const float flamePeriod = 0.01f;
	    public Bar bar;
	    private float fuelMax;

	    void Start () {
	        fuelMax = fuel;
	    }

		void Update () {
	        if (Player.instance.verticalInput > 0f/* && fuel > 0f*/) {
	            remote.person.jumpForceCont = jumpForceCont;
	            //fuel -= Game.dt;
	            flameTimer += Game.dt;
	            if (flameTimer > flamePeriod) {
	                flameTimer = 0f;
	                flames.get(flamePoint.position);
	            }
	        }
	        else {
	            remote.person.jumpForceCont = remote.person.normalJumpForceCont;
	        }
	        //bar.animRatio = fuel / fuelMax;
		}
	}
}
