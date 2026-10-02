using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelAnim : AnimBehavior {
    private CharacterPhysBox phys;
    private Color color;

    protected new void Start() {
        phys = GetComponent<CharacterPhysBox>();
        base.Start();
        color = sr.color;
        sprites = new Sprite[1] { sr.sprite };
    }

    protected new void Update() {
        base.Update();
        if (phys.invincibilityTime > 0f) {
            sr.color = new Color(color.r + phys.invincibilityTime, color.g + phys.invincibilityTime, color.b + phys.invincibilityTime, Random.Range(0f, 1f));
        }
        else {
            sr.color = new Color(color.r, color.g, color.b, 1f);
        }
    }
}