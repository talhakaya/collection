using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerAnim : MonoBehaviour {
    private HoldablePhysBox physBox;
    private SpriteRenderer sr;
    private Color colorFirst;
    public Color colorHoldable;

    void Start () {
        physBox = GetComponent<HoldablePhysBox>();
        sr = GetComponent<SpriteRenderer>();
        colorFirst = sr.color;
    }
	
	void Update () {
        sr.color = physBox.isHoldableHanger ? colorHoldable : colorFirst;
    }
}
