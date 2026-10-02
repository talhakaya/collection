using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextShadowAnim : MonoBehaviour {
    private Text text;
    private Shadow shadow;
    private float sLength;
    private float sAngle;
    [SerializeField] private bool makeColorful;

    void Start () {
        text = GetComponent<Text>();
        shadow = GetComponent<Shadow>();
        sLength = Geometry.lengthOfVector2(shadow.effectDistance);
        sAngle = Geometry.angleOfVector2(shadow.effectDistance);
    }
	
	void Update () {
        const float p = 1f;
        float animRatio = (Game.time % p) / p;
        float angle = sAngle + animRatio * 360f;
        float length = sLength * Easing.Linear(Game.Rhythm(p), 1f, -0.6f, 1f);
        shadow.effectDistance = Geometry.createVector2(angle, length);

        if (makeColorful) {
            shadow.effectColor = TalhaColorChanger.GetColor(Game.colors, new Color(1f, 1f, 1f));
        }
    }
}
