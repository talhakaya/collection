using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbablePhysBox : ScalablePhysBox {
    private static GameObject prefabWallClimb;
    public bool isClimbableL;
    public bool isClimbableD;
    public bool isClimbableR;
    public Dictionary<string, SpriteRenderer> climbableDict;
    public static Sprite[] spriteClimbables;
    public Dictionary<string, int> spriteClimbableDict = new Dictionary<string, int>();

    public void SetClimbable() {
        int scaleX = 1;
        int scaleY = 1;
        if (GetComponent<ScalablePhysBox>() != null) {
            ScalablePhysBox scalable = GetComponent<ScalablePhysBox>();
            scaleX = scalable.scaleX;
            scaleY = scalable.scaleY;
        }
        if (prefabWallClimb == null) {
            string mk = LevelEditor.isMKStatic ? "mk/" : "";
            prefabWallClimb = Resources.Load<GameObject>(mk + "wallClimb");
        }
        if (climbableDict == null) climbableDict = new Dictionary<string, SpriteRenderer>();
        climbableDict.Clear();
        foreach (Transform child in transform) {
            if (child.CompareTag("Climb")) Destroy(child.gameObject);
        }
        for (int i = 0; i < scaleX; i++) {
            if (isClimbableD) {
                GameObject go = Instantiate(prefabWallClimb, transform);
                go.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                go.transform.localScale = new Vector3(1f / Mathf.Max(1f, transform.localScale.y), 1f / Mathf.Max(1f, transform.localScale.x), 1f);
                go.transform.position = transform.position + new Vector3(i - (scaleX - 1) * 0.5f, -(scaleY - 1) * 0.5f, -0.1f);
                climbableDict.Add(string.Format("{0},{1}d", i, scaleY - 1), go.GetComponent<SpriteRenderer>());
            }
        }
        for (int j = 0; j < scaleY; j++) {
            if (isClimbableL) {
                GameObject go = Instantiate(prefabWallClimb, transform);
                go.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                go.transform.localScale = new Vector3(1f / Mathf.Max(1f, transform.localScale.x), 1f / Mathf.Max(1f, transform.localScale.y), 1f);
                go.transform.position = transform.position + new Vector3(-(scaleX - 1) * 0.5f, -j + (scaleY - 1) * 0.5f, -0.1f);
                climbableDict.Add(string.Format("{0},{1}l", 0, j), go.GetComponent<SpriteRenderer>());
            }
            if (isClimbableR) {
                GameObject go = Instantiate(prefabWallClimb, transform);
                go.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                go.transform.localScale = new Vector3(1f / Mathf.Max(1f, transform.localScale.x), 1f / Mathf.Max(1f, transform.localScale.y), 1f);
                go.transform.position = transform.position + new Vector3((scaleX - 1) * 0.5f, -j + (scaleY - 1) * 0.5f, -0.1f);
                climbableDict.Add(string.Format("{0},{1}r", scaleX - 1, j), go.GetComponent<SpriteRenderer>());
            }
        }
    }

    public Vector2 GetClimbPlayerPosition(ClimbableTile c, PhysBox player) {
        Vector2 res;
        switch (c) {
            case ClimbableTile.L:
                res = new Vector2(rect.x - rect.w * 0.5f - player.rect.w * 0.5f, rect.y);
                break;
            case ClimbableTile.D:
                res = new Vector2(rect.x, rect.y - rect.h * 0.5f - player.rect.h * 0.5f);
                break;
            case ClimbableTile.R:
                res = new Vector2(rect.x + rect.w * 0.5f + player.rect.w * 0.5f, rect.y);
                break;
            default:
                throw new System.EntryPointNotFoundException();
        }
        return res;
    }

    protected override List<Property> GetPropertyTypes() {
        List<Property> res = base.GetPropertyTypes();
        for (int i = 0; i < scaleX; i++) {
            for (int j = 0; j < scaleY; j++) {
                if (!spriteClimbableDict.ContainsKey(string.Format("{0},{1}", i, j))) {
                    spriteClimbableDict.Add(string.Format("{0},{1}", i, j), 0);
                }
                PropertyInt p = Property.GetCopy(PROP_SPRITE_CLIMBABLE) as PropertyInt;
                p.iDelta = i;
                p.jDelta = j;
                p.pMaxValue = spriteClimbables.Length - 1;
                res.Add(p);
            }
        }
        return res;
    }

    protected override void UpdateProperties() {
        base.UpdateProperties();
        for (int i = 0; i < scaleX; i++) {
            for (int j = 0; j < scaleY; j++) {
                if (!spriteClimbableDict.ContainsKey(string.Format("{0},{1}", i, j))) {
                    spriteClimbableDict.Add(string.Format("{0},{1}", i, j), 0);
                    PropertyInt p = Property.GetCopy(PROP_SPRITE_CLIMBABLE) as PropertyInt;
                    p.iDelta = i;
                    p.jDelta = j;
                    p.pMaxValue = spriteClimbables.Length - 1;
                    properties.Add(p);
                }
            }
        }
    }

    public override void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
        base.SetProperty(name, val, iDelta, jDelta);
        if (name == PROP_SPRITE_CLIMBABLE) {
            if (val >= spriteClimbables.Length) {
                Debug.LogWarningFormat("There are {0} sprites and you are trying to access {1}", spriteClimbables.Length, val);
                return;
            }
            spriteClimbableDict[string.Format("{0},{1}", iDelta, jDelta)] = val;
            if (climbableDict.ContainsKey(string.Format("{0},{1}d", iDelta, jDelta))) {
                climbableDict[string.Format("{0},{1}d", iDelta, jDelta)].sprite = spriteClimbables[val];
            }
            if (climbableDict.ContainsKey(string.Format("{0},{1}l", iDelta, jDelta))) {
                climbableDict[string.Format("{0},{1}l", iDelta, jDelta)].sprite = spriteClimbables[val];
            }
            if (climbableDict.ContainsKey(string.Format("{0},{1}r", iDelta, jDelta))) {
                climbableDict[string.Format("{0},{1}r", iDelta, jDelta)].sprite = spriteClimbables[val];
            }
            return;
        }
    }
}
