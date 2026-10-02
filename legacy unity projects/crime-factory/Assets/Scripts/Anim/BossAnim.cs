using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnim : MonoBehaviour {
    public List<PhysBox> leftAndRight;
    public List<PhysBox> rightAndLeft;
    public float periodHor = 8f;
    public float speedHor = 2f;
    public List<PhysBox> upAndDown;
    public List<PhysBox> downAndUp;
    public float periodVer = 4f;
    public float speedVer = 1f;
    [Space(40)]
    public List<PhysBox> leftAndRightP1;
    public List<PhysBox> rightAndLeftP1;
    public float periodHorP1 = 8f;
    public float speedHorP1 = 2f;
    public List<PhysBox> upAndDownP1;
    public List<PhysBox> downAndUpP1;
    public float periodVerP1 = 4f;
    public float speedVerP1 = 1f;
    [Space(40)]
    public List<PhysBox> leftAndRightP2;
    public List<PhysBox> rightAndLeftP2;
    public float periodHorP2 = 8f;
    public float speedHorP2 = 2f;
    public List<PhysBox> upAndDownP2;
    public List<PhysBox> downAndUpP2;
    public float periodVerP2 = 4f;
    public float speedVerP2 = 1f;
    [Space(40)]
    public List<PhysBox> leftAndRightH;
    public List<PhysBox> rightAndLeftH;
    public float periodHorH = 8f;
    public float speedHorH = 2f;
    public List<PhysBox> upAndDownH;
    public List<PhysBox> downAndUpH;
    public float periodVerH = 4f;
    public float speedVerH = 1f;
    [Space(40)]
    public float animTimerHor;
    public float animTimerVer;
    public Dictionary<PhysBox, Vector2> firstPos;
    public enum Phase {
        Phase1,
        Phase2
    }
    public Phase phase;
    public float hurtTimer;

    void Start () {
        firstPos = new Dictionary<PhysBox, Vector2>();
        
        foreach (PhysBox physBox in upAndDown) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in downAndUp) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in leftAndRight) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in rightAndLeft) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }

        foreach (PhysBox physBox in upAndDownP1) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in downAndUpP1) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in leftAndRightP1) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in rightAndLeftP1) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }

        foreach (PhysBox physBox in upAndDownP2) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in downAndUpP2) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in leftAndRightP2) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in rightAndLeftP2) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }

        foreach (PhysBox physBox in upAndDownH) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in downAndUpH) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in leftAndRightH) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
        foreach (PhysBox physBox in rightAndLeftH) {
            if (!firstPos.ContainsKey(physBox)) firstPos.Add(physBox, physBox.transform.localPosition);
        }
    }

    void Update() {
        UpdateAnim(periodHor, speedHor, periodVer, speedVer, upAndDown, downAndUp, leftAndRight, rightAndLeft);
        if (hurtTimer > 0f) {
            hurtTimer -= Platformer.dt;
            if (hurtTimer <= 0f) {
                animTimerHor = 0f;
                animTimerVer = 0f;
            }
            UpdateAnim(periodHorH, speedHorH, periodVerH, speedVerH, upAndDownH, downAndUpH, leftAndRightH, rightAndLeftH);
        }
        else {
            switch (phase) {
                case Phase.Phase1:
                    UpdateAnim(periodHorP1, speedHorP1, periodVerP1, speedVerP1, upAndDownP1, downAndUpP1, leftAndRightP1, rightAndLeftP1);
                    break;
                case Phase.Phase2:
                    UpdateAnim(periodHorP2, speedHorP2, periodVerP2, speedVerP2, upAndDownP2, downAndUpP2, leftAndRightP2, rightAndLeftP2);
                    break;
                default:
                    break;
            }
        }
    }

    void UpdateAnim(float pHor, float sHor, float pVer, float sVer, List<PhysBox> ud, List<PhysBox> du, List<PhysBox> lr, List<PhysBox> rl) {
        if (Simulator.IsPaused) return;
        animTimerVer += Time.deltaTime;
        animTimerHor += Time.deltaTime;
        float animRatioVer = (animTimerVer % pVer) / pVer;
        float animRatioHor = (animTimerHor % pHor) / pHor;
        const float MAX = 0.125f;
        if (animRatioVer < 0.5f) {
            foreach (PhysBox physBox in ud) {
                if (physBox.transform.localPosition.y >= firstPos[physBox].y + sVer * pVer * MAX) {
                    physBox.vel = new Vector2(physBox.vel.x, 0f);
                }
                else {
                    physBox.vel = new Vector2(physBox.vel.x, sVer);
                }
            }
            foreach (PhysBox physBox in du) {
                if (physBox.transform.localPosition.y <= firstPos[physBox].y - sVer * pVer * MAX) {
                    physBox.vel = new Vector2(physBox.vel.x, 0f);
                }
                else {
                    physBox.vel = new Vector2(physBox.vel.x, -sVer);
                }
            }
        }
        else {
            foreach (PhysBox physBox in ud) {
                if (physBox.transform.localPosition.y <= firstPos[physBox].y - sVer * pVer * MAX) {
                    physBox.vel = new Vector2(physBox.vel.x, 0f);
                }
                else {
                    physBox.vel = new Vector2(physBox.vel.x, -sVer);
                }
            }
            foreach (PhysBox physBox in du) {
                if (physBox.transform.localPosition.y >= firstPos[physBox].y + sVer * pVer * MAX) {
                    physBox.vel = new Vector2(physBox.vel.x, 0f);
                }
                else {
                    physBox.vel = new Vector2(physBox.vel.x, sVer);
                }
            }
        }

        if (animRatioHor < 0.5f) {
            foreach (PhysBox physBox in lr) {
                if (physBox.transform.localPosition.x >= firstPos[physBox].x + sHor * pHor * MAX) {
                    physBox.vel = new Vector2(0f, physBox.vel.y);
                }
                else {
                    physBox.vel = new Vector2(sHor, physBox.vel.y);
                }
            }
            foreach (PhysBox physBox in rl) {
                if (physBox.transform.localPosition.x <= firstPos[physBox].x - sHor * pHor * MAX) {
                    physBox.vel = new Vector2(0f, physBox.vel.y);
                }
                else {
                    physBox.vel = new Vector2(-sHor, physBox.vel.y);
                }
            }
        }
        else {
            foreach (PhysBox physBox in lr) {
                if (physBox.transform.localPosition.x <= firstPos[physBox].x - sHor * pHor * MAX) {
                    physBox.vel = new Vector2(0f, physBox.vel.y);
                }
                else {
                    physBox.vel = new Vector2(-sHor, physBox.vel.y);
                }
            }
            foreach (PhysBox physBox in rl) {
                if (physBox.transform.localPosition.x >= firstPos[physBox].x + sHor * pHor * MAX) {
                    physBox.vel = new Vector2(0f, physBox.vel.y);
                }
                else {
                    physBox.vel = new Vector2(sHor, physBox.vel.y);
                }
            }
        }
    }
}
