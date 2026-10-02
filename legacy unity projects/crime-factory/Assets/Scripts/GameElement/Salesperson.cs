using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salesperson : MonoBehaviour {
    public enum State {
        None,
        Selling,
        NotEnoughMoney,
        OutOfStock
    }
    public enum Product {
        FireRate,
        Shield
    }
    public Product product;
    public int stock;
    public int price;
    [HideInInspector] public State state;

    void Start()
    {
        state = State.None;
    }

    public void StartSelling() {
        if (state == State.None) {
            if (stock <= 0) {
                state = State.OutOfStock;
            }
            else if (Platformer.money < price) {
                state = State.NotEnoughMoney;
            }
            else {
                state = State.Selling;
                TutorialManager.instance.ShowSaleUI(this);
            }
            MoneyUI.instance.ShowSalesperson(this);
        }
    }

    public void Buy() {
        Platformer.money -= price;
        stock -= 1;
        MoneyUI.instance.Add(-price);
        state = State.None;
        switch (product) {
            case Product.FireRate:
                Platformer.numFireRateUpgrades++;
                break;
            case Product.Shield:
                Platformer.numShields++;
                break;
            default:
                throw new System.NotImplementedException();
        }
        AudioPlayer.instance.Play(AudioPlayer.instance.clipSpendMoney);
    }

    public void Next() {
        state = State.None;
    }
}
