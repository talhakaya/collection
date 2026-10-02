using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoneyUI : MonoBehaviour {
    public static MoneyUI instance;
    public TextMeshProUGUI text;
    public CanvasGroup cg;
    public float cgTimer;
    public List<int> moneyValues;
    public List<float> moneyTimers;
    private Salesperson salesperson;
    private const float STAY_TIME = 4f;
    private const float FADEOUT_TIME = 2f;

    void Awake()
    {
        instance = this;
    }
    
    void Update() {
        text.text = "";
        if (moneyValues.Count > 0) {
            for (int i = 0, len = moneyValues.Count; i < len; i++) {
                string aStart = "";
                string aEnd = "";
                if (moneyTimers[i] >= STAY_TIME) {
                    string a = "00";
                    aStart = "<color=#ffffff" + a + ">";
                    aEnd = "</color>";
                }
                else if (moneyTimers[i] >= STAY_TIME - FADEOUT_TIME) {
                    float alpha = (STAY_TIME - moneyTimers[i]) / FADEOUT_TIME;
                    string a = Mathf.FloorToInt(alpha * 256f).ToString("X2");
                    aStart = "<color=#ffffff" + a + ">";
                    aEnd = "</color>";
                }
                text.text += aStart + (moneyValues[i] > 0 ? "+" : "") + moneyValues[i].ToString() + aEnd + "\n";
                moneyTimers[i] += Time.deltaTime;
            }
            if (moneyTimers[0] >= STAY_TIME) {
                moneyValues.RemoveAt(0);
                moneyTimers.RemoveAt(0);
            }
        }
        else {
            if (cgTimer >= STAY_TIME - FADEOUT_TIME) {
                cg.alpha = (STAY_TIME - cgTimer) / FADEOUT_TIME;
            }
            else {
                cg.alpha = 1f;
            }
            cgTimer += Time.deltaTime;
            if (salesperson != null && salesperson.state != Salesperson.State.None) {
                cgTimer = 0f;
            }
        }
        text.text += "$ " + Platformer.money.ToString();
    }

    public void ShowSalesperson(Salesperson salesperson) {
        this.salesperson = salesperson;
    }

    public void Add(int amount) {
        if (moneyValues.Count > 9) {
            moneyValues.RemoveAt(0);
            moneyTimers.RemoveAt(0);
        }
        moneyValues.Add(amount);
        moneyTimers.Add(0f);
        cgTimer = 0f;
        cg.alpha = 1f;
    }
}
