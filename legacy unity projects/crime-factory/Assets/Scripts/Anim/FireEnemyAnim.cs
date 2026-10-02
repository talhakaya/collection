using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemyAnim : AnimBehavior {
    private float timer;
    private PhysBox physBox;
    public Sprite[] anim;
    public bool isVertical;

    private new void Start() {
        physBox = GetComponent<PhysBox>();
        sprites = anim;
        period = 0.2f;
        base.Start();
    }
    
	void OnEnable () {
        timer = 2f;
    }
    
	private new void Update () {
        base.Update();
        if (Simulator.IsPaused) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            gameObject.SetActive(false);
        }
        else if (timer <= 1f) {
            if (isVertical) {
                transform.localScale = new Vector3(Mathf.Min(1f, timer), transform.localScale.y, 1f);
            }
            else {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Min(1f, timer), 1f);
            }
            physBox.visualPosOffset = new Vector2(0f, 0f);
        }
        else {
            physBox.visualPosOffset = new Vector2(isVertical ? Random.Range(-0.1f, 0.1f) : 0f, !isVertical ? Random.Range(-0.1f, 0.1f) : 0f);
        }
        physBox.enabled = timer > 0.5f;
	}
}
