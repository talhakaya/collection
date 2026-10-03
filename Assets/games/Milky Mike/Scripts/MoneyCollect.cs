using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class MoneyCollect : MonoBehaviour {

		void Start () {
			if (Game.currentLevel.moneyCollects == null) {
	            Game.currentLevel.moneyCollects = new List<MoneyCollect>();
	        }
	        Game.currentLevel.moneyCollects.Add(this);
	    }

	    private void Update() {
	        if (Geometry.lengthOfVector3(transform.position - Player.instance.transform.position) < 0.75f) {
	            gameObject.SetActive(false);
	            Game.currentLevel.moneyCollected += 100;
	        }
	    }
	}
}
