using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderData : MonoBehaviour {
    public Color color;
    public Vector2[] points;

    void Start() {
        if (color.a == 0f) color = new Color(1f, 0f, 1f);
    }
}
