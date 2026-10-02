using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyAnim : AnimBehavior
{
    private float timer;
    private MoneyPhysBox physBox;
    [HideInInspector] public int amount;
    [SerializeField] private AnimAmountPair[] anims;

    private void OnEnable() {
        timer = 0.2f;
        physBox = GetComponent<MoneyPhysBox>();
        physBox.timerJump = 2.5f;
        physBox.timerDie = Random.Range(15f, 20f);
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    public void SetAmount(int amount) {
        this.amount = amount;
        for (int i = 0, len = anims.Length; i < len; i++) {
            if (anims[i].amount == amount) {
                sprites = anims[i].anim;
                GetComponent<SpriteRenderer>().color = anims[i].color;
                break;
            }
        }
    }

    private new void Update() {
        base.Update();
        if (Simulator.IsPaused) return;
        if (timer > 0f) {
            timer -= Platformer.dt;
            if (timer <= 0f) physBox.input = new Vector2(0f, 0f);
            else physBox.input = physBox.inputFirst * Mathf.Max(1f, timer / 0.1f);
        }
        if (physBox.timerInvincibility > 0f) {
            physBox.timerInvincibility -= Platformer.dt;
        }
        physBox.timerJump -= Platformer.dt;
        if (physBox.timerJump <= 0f) physBox.timerJump = Random.Range(2.5f, 2.7f);
        physBox.timerDie -= Platformer.dt;
        if (physBox.timerDie <= 5f) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, physBox.timerDie % 0.2f >= 0.1f ? 1f : 0f);
        }
    }

    [System.Serializable]
    public class AnimAmountPair {
        public int amount;
        public Sprite[] anim;
        public Color color;
    }
}
