using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPhysBox : DynamicPhysBox
{
    [HideInInspector] public int amount;
    [HideInInspector] public float timerInvincibility;
    [HideInInspector] public float timerJump;
    [HideInInspector] public float timerDie;
    [HideInInspector] public Vector2 inputFirst;

    public void SetAmount(int amount) {
        this.amount = amount;
        GetComponent<MoneyAnim>().SetAmount(amount);
    }
}
