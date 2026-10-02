using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMoneyCounter : MonoBehaviour {
    public Text textMoney;
    public Text textMoneyAdd;
    private float addTimer = AddPeriod;
    private const float AddPeriod = 3f;

    void Update() {
        if (Game.currentLevel.hasChallenge) {
            textMoney.text = string.Format("MONEY GAINED: {0}$", Game.currentLevel.moneyCollected);
        }
        else {
            textMoney.text = string.Format("MONEY: {0}$", Game.instance.moneyTotal);
        }

        if (addTimer < AddPeriod) {
            textMoneyAdd.rectTransform.anchoredPosition = new Vector2(0f, 24f + addTimer * 12f);
            if (addTimer < AddPeriod - 1f) {
                textMoneyAdd.color = new Color(textMoneyAdd.color.r, textMoneyAdd.color.g, textMoneyAdd.color.b, 1f);
            }
            else {
                textMoneyAdd.color = new Color(textMoneyAdd.color.r, textMoneyAdd.color.g, textMoneyAdd.color.b, AddPeriod - addTimer);
            }
            addTimer += Time.deltaTime;
        }
        else {
            textMoneyAdd.color = new Color(textMoneyAdd.color.r, textMoneyAdd.color.g, textMoneyAdd.color.b, 0f);
        }
    }

    public void AddMoney(int amount) {
        addTimer = 0f;
        if (Game.currentLevel.hasChallenge) {
            textMoneyAdd.text = string.Format("<color=#00000000>MONEY GAINED: </color>{0}$", amount);
        }
        else {
            textMoneyAdd.text = string.Format("<color=#00000000>MONEY: </color>{0}$", amount);
        }
    }

    public void ResetTimer() {
        addTimer = AddPeriod;
    }
}
