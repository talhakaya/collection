using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class HoleTrigger : MonoBehaviour
{
    public static Vector3 pos;
    private bool triggered;
    private float time;
    private float normalZoom = 18f;
    private float closeZoom = 5f;

	void Start ()
    {
        pos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (triggered)
        {
            time += Game.dt;
            if (Game.circleHoleEffectOn)
            {
                if (time < 0.8f)
                {
                    Camera.main.orthographicSize = Easing.SineEaseOut(time, normalZoom, closeZoom - normalZoom, 0.8f);
                }
                else
                {
                    Camera.main.orthographicSize = Easing.SineEaseIn(time - 0.8f, closeZoom, normalZoom - closeZoom, 0.2f);
                }
            }
            if (time > 1f)
            {
                if (Game.circleHoleEffectOn)
                {
                    Camera.main.orthographicSize = normalZoom;
                }
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
                if (!Game.instance.isMenu)
                {
                    Game.noOfHoles++;
                    PlayerPrefs.SetInt("noOfHoles", Game.noOfHoles);
                    Analytics.CustomEvent("getHole", new Dictionary<string, object>
                    {
                        { "noOfHoles", Game.noOfHoles },
                        { "noOfStrokes", Game.noOfStrokes },
                        { "outlineOn", OutlineSprite.isOn },
                        { "reverseShooting", Game.reverseShooting },
                        { "holesOnWalls", Game.holesOnWalls },
                        { "soundOn", Game.soundOn },
                        { "musicOn", Game.musicOn },
                        { "terrainEffectOn", Game.terrainEffectOn },
                        { "circleHoleEffectOn", Game.circleHoleEffectOn }
                    });
                    
                }
                else
                {
                    Analytics.CustomEvent("getHoleInMenu", new Dictionary<string, object>
                    {
                        { "noOfHoles", Game.noOfHoles },
                        { "noOfStrokes", Game.noOfStrokes },
                        { "outlineOn", OutlineSprite.isOn },
                        { "reverseShooting", Game.reverseShooting },
                        { "holesOnWalls", Game.holesOnWalls },
                        { "soundOn", Game.soundOn },
                        { "musicOn", Game.musicOn },
                        { "terrainEffectOn", Game.terrainEffectOn },
                        { "circleHoleEffectOn", Game.circleHoleEffectOn }
                    });
                }
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        trigger(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        trigger(other);
    }

    void trigger(Collider2D other)
    {
        if (other.gameObject == GolfBall.instance.gameObject && !triggered)
        {
            triggered = true;
            if (Game.circleHoleEffectOn)
            {
                CircleParticle.create(5, transform.position - Vector3.forward * 2f);
            }
            if (Game.soundOn)
            {
                Game.instance.audioSource.pitch = 0.9f + Random.value * 0.2f;
                Game.instance.audioSource.Play();
            }
        }
    }
}
