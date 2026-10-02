using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMilkCounter : MonoBehaviour {
    public const float MaxMilkAmount = 2.5f;
    [SerializeField] private Image imageMilk;
    [SerializeField] private Image imageMilkFill;
    [SerializeField] private Text textMilk;
    [SerializeField] private CanvasGroup canvasGroup;
    private float timer;
    private const float PeriodAlpha = 0.5f;
    private const float PeriodText = 2f;
    private const float PeriodTotal = 5f;
    private float amountOld;
    private float amountNew;

    private void Start() {
        timer = PeriodTotal;
    }

    private void Update() {
        if (timer < PeriodTotal) {
            if (timer < PeriodAlpha) {
                canvasGroup.alpha = timer / PeriodAlpha;
                SetMilkNow(amountOld);
            }
            else if (timer < PeriodAlpha + PeriodText) {
                canvasGroup.alpha = 1f;
                SetMilkNow(Easing.SineEaseOut(timer - PeriodAlpha, amountOld, amountNew - amountOld, PeriodText));
            }
            else if (timer < PeriodTotal - PeriodAlpha) {
                canvasGroup.alpha = 1f;
                SetMilkNow(amountNew);
            }
            else {
                canvasGroup.alpha = (PeriodTotal - timer) / PeriodAlpha;
                SetMilkNow(amountNew);
            }
            timer += Time.deltaTime;
        }
        else {
            canvasGroup.alpha = 0f;
        }
    }

    public void SetMilk(float amount, float amountOld) {
        this.amountOld = amountOld;
        amountNew = amount;
        timer = 0f;
        SetMilkNow(amountOld);
    }

    private void SetMilkNow(float amount) {
        imageMilkFill.fillAmount = amount / MaxMilkAmount;
        textMilk.text = string.Format("{0}L", amount.ToString("F1"));
    }

    public bool IsActive { get { return timer < PeriodTotal; } }
}
