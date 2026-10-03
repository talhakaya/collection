using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.MilkyMike
{
	public class UiManager : MonoBehaviour {
	    public static UiManager instance;
	    public UiDialogue uiDialogue;
	    public UiScore uiScore;
	    public UiMilkCounter uiMilkCounter;
	    public UiMoneyCounter uiMoneyCounter;
	    public UiMilkEnd uiMilkEnd;
	    public Image imageRestart;

	    void Awake() {
	        instance = this;
	        ResetState();
	    }

	    public bool CanPlayerMove() {
	        return !uiDialogue.isActive && !uiScore.isActive;
	    }

	    public void ResetState() {
	        uiDialogue.isActive = false;
	        uiScore.isActive = false;
	        uiMilkEnd.isActive = false;
	        uiMoneyCounter.ResetTimer();
	    }
	}
}
