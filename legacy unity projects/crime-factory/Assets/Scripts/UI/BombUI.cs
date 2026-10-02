using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour {
    public bool isMK;
    public static int numBombs;
    private RectTransform rect;
    
	void Start () {
        rect = GetComponent<RectTransform>();
    }
	
	void Update () {
        if (isMK) {
            rect.sizeDelta = new Vector2(90f * numBombs, rect.sizeDelta.y);
        }
        else {
            rect.sizeDelta = new Vector2(100f * numBombs, rect.sizeDelta.y);
        }
	}
}
