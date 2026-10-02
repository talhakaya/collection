using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour {
    private float timer;
    private bool checkHear;
    private const float period = 0.1f;
    private const float maxAlpha = 0.15f;
    private Color color = new Color(1f, 1f, 0.3f, maxAlpha);
    private Vector3 scale;
    private SpriteRenderer spriteRenderer;
    public const float Gun = 15f;
    public const float BulletHit = 8f;
    public const float JumpHitGround = 4f;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() {
        checkHear = true;
        timer = 0f;
    }

    void Update() {
        timer += Game.dt;

        if (timer > period * 2f) {
            gameObject.SetActive(false);
        }
        else {
            if (timer > period) {
                float ratio = (2f * period - timer) / period;
                transform.localScale = scale * ratio;
                spriteRenderer.color = new Color(color.r, color.g, color.b, ratio * maxAlpha);
            }
            else {
                scale = transform.localScale;
                spriteRenderer.color = new Color(color.r, color.g, color.b, timer / period * maxAlpha);
            }

            if (checkHear && Game.currentLevel != null) {
                checkHear = false;
                foreach (Guard g in Game.currentLevel.guards) {
                    if (g.isActiveAndEnabled && g.person.state == Person.State.Alive && personCanHear(g.person)) {
                        g.Hear(transform.position);
                    }
                }
            }
        }
    }

    bool personCanHear(Person p) {
        foreach (Tilt t in p.tilts) {
            if (Geometry.lengthOfVector3(t.transform.position - transform.position) < scale.x) {
                return true;
            }
        }
        return false;
    }

    public static void Create(Vector3 position, Vector3 scale) {
        if (Game.currentLevel != null && !Game.currentLevel.HasAliveGuard()) return;
        ObjectPool.noisePool.get(position, scale);
    }
}
