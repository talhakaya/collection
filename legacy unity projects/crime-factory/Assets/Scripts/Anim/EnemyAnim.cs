using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnim : AnimBehavior {
    public Sprite[] idle;
    public Sprite[] walk;
    public Sprite[] fire;
    public Sprite[] wait;
    private CharacterPhysBox phys;

    protected new void Start() {
        sprites = idle;
        phys = GetComponent<CharacterPhysBox>();
        base.Start();
    }

    protected new void Update() {
        base.Update();
        if (phys.invincibilityTime > 0f) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Random.Range(0f, 1f));
        }
        else {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }
        if (Simulator.IsPaused) return;
        if (phys.isRight) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (phys.fireTime > 0f && fire.Length > 0) {
            sprites = fire;
            period = 0.1f;
        }
        else if (phys.fireCoolingTime > 0f && wait.Length > 0) {
            sprites = wait;
            period = 0.3f;
        }
        else if (Mathf.Abs(phys.input.x) > 0.5f) {
            sprites = walk;
            period = 0.15f;
        }
        else {
            sprites = idle;
            period = 0.3f;
        }

        if (phys.fireTime > 0f) {
            phys.visualPosOffset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f) + 0.15f);
        }
        else {
            phys.visualPosOffset = new Vector2(0f, 0.15f);
        }
    }
}