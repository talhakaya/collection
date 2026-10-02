using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimBehavior {
    public GameObject shield;
    public Sprite[] idle;
    public Sprite[] walk;
    public Sprite[] fall;
    public Sprite[] jump;
    public Sprite[] ceiling;
    public Sprite[] ceilingMove;
    public Sprite[] ladder;
    public Sprite[] ladderMove;
    public Sprite[] hanger;
    public Sprite[] wallClimb;
    public Sprite[] wallClimbMove;
    public Sprite[] fire;
    public Sprite[] fireCeiling;
    public Sprite[] fireWallClimb;
    public Sprite[] fireHanger;
    public Sprite[] fireLadder;
    private CharacterPhysBox phys;
    public float yScaleFactor;
    private float yScale;
    public float deathTimer;
    private Vector3 scaleLast;
    private Vector2 visualPosOffset;

    protected new void Start () {
        sprites = idle;
        phys = GetComponent<CharacterPhysBox>();
        visualPosOffset = phys.visualPosOffset;
        yScale = transform.localScale.y;
        yScaleFactor = 1f;
        base.Start();
    }

    private void OnEnable() {
        deathTimer = 0f;
    }

    protected new void Update () {
        base.Update();
        if (deathTimer > 0f) {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f) {
                transform.localScale = scaleLast;
                gameObject.SetActive(false);
            }
            else {
                transform.localScale = new Vector3(Random.Range(0.5f, 4f) * scaleLast.x, Random.Range(0.5f, 4f) * scaleLast.y, 1f);
            }
            return;
        }
        if (phys.isRight) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), yScale * yScaleFactor, transform.localScale.z);
        }
        else {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), yScale * yScaleFactor, transform.localScale.z);
        }
        scaleLast = transform.localScale;

        if (phys.onLadder) {
            if (phys.ladderActive.gameElement == PhysBox.GameElement.Ladder)
            {
                if (phys.fireTime > 0f) {
                    sprites = fireLadder;
                    period = 0.05f;
                    sounds = null;
                }
                else if (Mathf.Abs(phys.input.y) > 0.5f) {
                    sprites = ladderMove;
                    period = 0.1f;
                    sounds = new AudioClip[4] {
                        AudioPlayer.instance.clipStep0,
                        AudioPlayer.instance.clipStep1,
                        AudioPlayer.instance.clipStep0,
                        AudioPlayer.instance.clipStep1
                        };
                    soundTimings = new float[4] {
                        0.0f,
                        0.1f,
                        0.2f,
                        0.3f
                        };
                }
                else {
                    sprites = ladder;
                    period = 0.3f;
                    sounds = null;
                }
            }
            else if (phys.ladderActive.gameElement == PhysBox.GameElement.Hanger)
            {
                if (phys.fireTime > 0f) {
                    sprites = fireHanger;
                    period = 0.05f;
                    sounds = null;
                }
                else {
                    sprites = hanger;
                    period = 0.3f;
                    sounds = null;
                }
            }
            else if (phys.ladderActiveClimbable == ClimbableTile.L || phys.ladderActiveClimbable == ClimbableTile.R)
            {
                if (phys.fireTime > 0f) {
                    sprites = fireWallClimb;
                    period = 0.05f;
                    sounds = null;
                }
                else if (Mathf.Abs(phys.input.y) > 0.5f)
                {
                    sprites = wallClimbMove;
                    period = 0.1f;
                    sounds = new AudioClip[2] {
                        AudioPlayer.instance.clipStep0,
                        AudioPlayer.instance.clipStep1
                        };
                    soundTimings = new float[2] {
                        0.0f,
                        0.2f
                        };
                }
                else
                {
                    sprites = wallClimb;
                    period = 0.3f;
                    sounds = null;
                }
                if (phys.ladderActiveClimbable == ClimbableTile.L)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), yScale * yScaleFactor, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), yScale * yScaleFactor, transform.localScale.z);
                }
            }
            else if (phys.ladderActiveClimbable == ClimbableTile.D)
            {
                if (phys.fireTime > 0f) {
                    sprites = fireCeiling;
                    period = 0.05f;
                    sounds = null;
                }
                else if (Mathf.Abs(phys.input.x) > 0.5f)
                {
                    sprites = ceilingMove;
                    period = 0.15f;
                    sounds = new AudioClip[2] {
                        AudioPlayer.instance.clipStep0,
                        AudioPlayer.instance.clipStep1
                        };
                    soundTimings = new float[2] {
                        0.0f,
                        0.3f
                        };
                }
                else
                {
                    sprites = ceiling;
                    period = 0.3f;
                    sounds = null;
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
        else {
            if (phys.fireTime > 0f) {
                sprites = fire;
                period = 0.05f;
                sounds = null;
            }
            else if (phys.vel.y > 0.5f) {
                sprites = jump;
                period = Platformer.isMK ? 0.3f : 0.2f;
                sounds = null;
            }
            else if (phys.vel.y < -0.5f) {
                sprites = fall;
                period = 0.1f;
                sounds = null;
            }
            else if (Mathf.Abs(phys.input.x) > 0.5f) {
                sprites = walk;
                period = Platformer.isMK ? 0.075f : 0.15f;
                sounds = new AudioClip[2] {
                        AudioPlayer.instance.clipStep0,
                        AudioPlayer.instance.clipStep1
                        };
                soundTimings = new float[2] {
                        0.0f,
                        0.3f
                        };
            }
            else {
                sprites = idle;
                period = 0.3f;
                sounds = null;
            }
        }

        if (phys.invincibilityTime > 0f) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Random.Range(0f, 1f));
        }
        else {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }

        if (phys.fireTime > 0f) {
            phys.visualPosOffset = visualPosOffset + new Vector2(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f));
        }
        else {
            phys.visualPosOffset = visualPosOffset;
        }
    }

    public void ShowShield(bool doesPlayerHaveShield) {
        shield.SetActive(doesPlayerHaveShield);
    }
}
