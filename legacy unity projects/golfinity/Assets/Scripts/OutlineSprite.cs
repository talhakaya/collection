using UnityEngine;
using System.Collections;

public class OutlineSprite : MonoBehaviour
{
    public enum Outline { Off, x4, x8 };
    public static Outline isOn;
    private GameObject go;
    private int noOfSprites = 4;
    private GameObject[] gos;
    private bool inited;
    private const float outlineWidth = 0.15f;
    public bool shouldBeUpdated;

    void Start()
    {
        Init();
    }

    public void Init(bool force = false)
    {
        if (!inited || force)
        {
            inited = true;
            if (isOn != Outline.Off)
            {
                if (isOn == Outline.x4)
                {
                    noOfSprites = 4;
                }
                else
                {
                    noOfSprites = 8;
                }
                gos = new GameObject[noOfSprites];
                for (int i = 0; i < noOfSprites; i++)
                {
                    go = new GameObject("Outline");
                    go.transform.parent = transform;
                    go.transform.localScale = Vector3.one;
                    go.transform.position = transform.position + Vector3.forward * 5f + Geometry.createVector3(i * 360f / noOfSprites, outlineWidth);
                    go.transform.localEulerAngles = Vector3.zero;
                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = GetComponent<SpriteRenderer>().sprite;
                    sr.color = Color.black;
                    gos[i] = go;
                }
            }
        }
    }
	
	void Update ()
    {
        if (isOn != Outline.Off)
        {
            if (gos == null)
            {
                Init(true);
            }
            else if ((isOn == Outline.x4 && noOfSprites != 4) || (isOn == Outline.x8 && noOfSprites != 8))
            {
                for (int i = 0; i < noOfSprites; i++)
                {
                    Destroy(gos[i]);
                }
                gos = null;
                Init(true);
            }
            else
            {
                if (shouldBeUpdated)
                {
                    for (int i = 0; i < noOfSprites; i++)
                    {
                        gos[i].transform.localScale = Vector3.one;
                        gos[i].transform.position = transform.position + Vector3.forward * 5f + Geometry.createVector3(45f + i * 360f / noOfSprites, outlineWidth);
                        gos[i].transform.localEulerAngles = Vector3.zero;
                    }
                }
            }
        }
        else
        {
            if (gos != null)
            {
                for (int i = 0; i < noOfSprites; i++)
                {
                    Destroy(gos[i]);
                }
                gos = null;
            }
        }
	}
}
