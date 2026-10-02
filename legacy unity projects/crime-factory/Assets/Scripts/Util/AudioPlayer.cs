using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
    public static AudioPlayer instance;

    private int poolCounter;
    public AudioSource[] audioSources;
    public AudioClip clipStep0;
    public AudioClip clipStep1;
    public AudioClip clipPickup;
    public AudioClip clipBulletHit;
    public AudioClip clipEnemyDie;
    public AudioClip clipPlayerDie;
    public AudioClip clipBossDie0;
    public AudioClip clipBossDie1;
    public AudioClip clipDoor;
    public AudioClip clipEnemyShoot;
    public AudioClip clipEnemyLaser;
    public AudioClip clipEnemyLaserBuild;
    public AudioClip clipPlayerJump;
    public AudioClip clipPlayerShoot;
    public AudioClip clipPlayerHitGround;
    public AudioClip clipExplosion;
    public AudioClip clipCoin;
    public AudioClip clipComedyDrum;
    public AudioClip clipSpendMoney;

    void Awake() {
        instance = this;
    }

    public void Play(AudioClip clip) {
        audioSources[poolCounter].clip = clip;
        audioSources[poolCounter].Play();

        poolCounter++;
        if (poolCounter >= audioSources.Length) {
            poolCounter = 0;
        }
    }
}
