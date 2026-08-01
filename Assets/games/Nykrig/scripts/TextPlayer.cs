using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.Nykrig
{
	public class TextPlayer : MonoBehaviour {
	    public GameObject player;
	    public Text[] texts;
		
		void Update () {
	        bool enabled = Game.instance.twoPlayers && player.activeSelf;
	        for (int i = 0, len = texts.Length; i < len; i++) {
	            texts[i].enabled = enabled;
	        }
	        if (enabled) {
	            texts[0].transform.position = Camera.main.WorldToScreenPoint(player.transform.position);
	        }
	    }
	}

}
