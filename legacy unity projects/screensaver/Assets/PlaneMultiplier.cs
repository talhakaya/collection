using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneMultiplier : MonoBehaviour
{
    private float dist = 0;
    private const float PlaneZ = -0.1f;
    public GameObject planePrefab;
    private List<Transform> children;
    private const float period0 = 1.5f;
    private const float period1 = 1f;
    private const float alpha = 0.015f;
    private const float alpha1 = 0.04f;
	void Start ()
    {
        GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha1));
        Color[] colors = new Color[]
        {
            new Color(1f, 0f, 0f),
            new Color(0.5f, 0.5f, 0f),
            new Color(0f, 1f, 0f),
            new Color(0f, 0.5f, 0.5f),
            new Color(0f, 0f, 1f),
            new Color(0.5f, 0f, 0.5f)
        };
        children = new List<Transform>();
        for (int i = 0; i < 6; i++)
        {
            GameObject go = Instantiate(planePrefab, transform.position + Vector3.forward * PlaneZ, transform.rotation) as GameObject;
            go.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(colors[i].r, colors[i].g, colors[i].b, alpha));
            go.transform.parent = transform;
            children.Add(go.transform);
        }
	}
	
	void Update ()
    {
        float t0 = Game.time % (period0 * 2f);
        if (t0 >= period0)
        {
            t0 = 2f * period0 - t0;
        }
        t0 = t0 / period0;
        float t1 = Game.time % (period1 * 2f);
        if (t1 >= period1)
        {
            t1 = 2f * period1 - t1;
        }
        t1 = t1 / period0;
	    for (int i = 0; i < children.Count; i++)
        {
            children[i].position = transform.position + Vector3.forward * PlaneZ + Geometry.createVector3(i * 60f + 360f * t0, t1 * 0.5f);
        }
	}
}
