using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.MilkyMike
{
	public class UiMilkEnd : MonoBehaviour {
	    [SerializeField] private CanvasGroup canvasGroup;
	    [SerializeField] private UiMilkCounter uiMilkCounter;
	    [SerializeField] private Image imageContinue;
	    public bool isActive;

	    void Start () {

		}

		void Update () {
	        if (isActive) {
	            canvasGroup.alpha = 1f;
	            imageContinue.enabled = !uiMilkCounter.IsActive;
	            if (!uiMilkCounter.IsActive && Player.instance.fireInput) {
	                Game.instance.NextLevel();
	            }
	        }
	        else {
	            canvasGroup.alpha = 0f;
	        }
	    }

	    public void SetMilk(float amount, float amountOld) {
	        if (!isActive) return;
	        uiMilkCounter.SetMilk(amount, amountOld);
	    }
	}
}
