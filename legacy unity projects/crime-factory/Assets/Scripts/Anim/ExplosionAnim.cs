using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAnim : AnimBehavior {
    public Sprite[] anim;
    public float timer;
    private PhysBox phys;

    private void OnEnable() {
        timer = Random.Range(0.2f, 0.6f);
    }

    private new void Start() {
        sprites = anim;
        period = 0.2f;
        phys = GetComponent<PhysBox>();
        base.Start();
    }

    private new void Update() {
        base.Update();
        if (Simulator.IsPaused) return;
        timer -= Time.deltaTime;
        if (timer > 0.2f) {
            phys.visualPosOffset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (timer > 0f) {
            phys.visualPosOffset = new Vector2(0f, 0f);
            float animRatio = timer / 0.2f;
            transform.localScale = new Vector3(animRatio, animRatio, 1f);
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
