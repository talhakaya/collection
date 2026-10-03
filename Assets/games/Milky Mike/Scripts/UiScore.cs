using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Collection.Controls;

namespace Games.MilkyMike
{
	public class UiScore : MonoBehaviour {
	    private CanvasGroup canvasGroup;
	    [SerializeField] private Image[] ticks;
	    [SerializeField] private Text[] textTicks;
	    [SerializeField] private Text textMoneyCollected;
	    [SerializeField] private Text textMoneyTotal;
	    [SerializeField] private Sprite spriteTickFull;
	    [SerializeField] private Sprite spriteTickEmpty;
	    [SerializeField] private GameObject parentContinue;
	    private int moneyCollected;
	    private int moneyTotal;
	    private bool[] boolTicks;
	    private float timer;
	    private const float period = 7f;
	    public bool isActive;

	    void Start () {
	        canvasGroup = GetComponent<CanvasGroup>();
	        canvasGroup.alpha = 0f;
	        timer = period;
	    }

		void Update () {
	        if (isActive) {
	            canvasGroup.alpha = 1f;
	            if (timer < period) {
	                timer += (Player.instance.fireInput ? 4f : 1f) * Time.deltaTime;
	                float ratio = timer / period;
	                float ratioCalc = 0.05f;

	                textMoneyCollected.enabled = ratio > ratioCalc;
	                ratioCalc += 0.05f;
	                if (ratio < ratioCalc) {
	                    textMoneyCollected.text = string.Format("MONEY GAINED: {0}", 0);
	                }
	                else if (ratio < ratioCalc + 0.15f) {
	                    textMoneyCollected.text = string.Format("MONEY GAINED: {0}", Mathf.RoundToInt((ratio - ratioCalc) / 0.15f * moneyCollected));
	                }
	                else {
	                    textMoneyCollected.text = string.Format("MONEY GAINED: {0}", moneyCollected);
	                }
	                ratioCalc += 0.25f;
	                textMoneyTotal.enabled = ratio > ratioCalc;
	                if (ratio < ratioCalc) {
	                    textMoneyTotal.text = string.Format("TOTAL MONEY: {0}", moneyTotal);
	                }
	                else if (ratio < ratioCalc + 0.15f) {
	                    textMoneyTotal.text = string.Format("TOTAL MONEY: {0}", moneyTotal + Mathf.RoundToInt((ratio - ratioCalc) / 0.15f * moneyCollected));
	                }
	                else {
	                    textMoneyTotal.text = string.Format("TOTAL MONEY: {0}", moneyTotal + moneyCollected);
	                }
	                ratioCalc += 0.25f;
	                for (int i = 0, len = ticks.Length; i < len; i++) {
	                    textTicks[i].enabled = ratio > ratioCalc;
	                    ticks[i].enabled = ratio > ratioCalc + 0.05f;
	                    ratioCalc += 0.13f;
	                }
	                parentContinue.SetActive(ratio > ratioCalc);
	            }
	            else {
	                if (Player.instance.fireInput) {
	                    Game.instance.NextLevel();
	                }
	                else if (TaloketoInputManager.GetButtonDown("Restart")) {
	                    Game.instance.ResetLevel();
	                }
	            }
	        }
	        else {
	            canvasGroup.alpha = 0f;
	        }
	    }

	    public void StartScoring(int moneyCollected, int moneyTotal, bool tickMoney, bool tickStealth, bool tickTime) {
	        textMoneyCollected.enabled = false;
	        textMoneyTotal.enabled = false;
	        for (int i = 0, len = ticks.Length; i < len; i++) {
	            ticks[i].enabled = false;
	            textTicks[i].enabled = false;
	        }
	        parentContinue.SetActive(false);
	        this.moneyCollected = moneyCollected;
	        this.moneyTotal = moneyTotal;
	        boolTicks = new bool[3] { tickMoney, tickStealth, tickTime };
	        for (int i = 0, len = ticks.Length; i < len; i++) {
	            ticks[i].sprite = boolTicks[i] ? spriteTickFull : spriteTickEmpty;
	        }
	        timer = 0f;
	        isActive = true;
	    }
	}
}
