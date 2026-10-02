using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RectangleManager : MonoBehaviour
{
    private float timer;
    private List<GameObject> rectangles;
    public float period;
    public GameObject disableObject;

	void Start ()
    {
        rectangles = new List<GameObject>();
        foreach (Transform child in transform)
        {
            rectangles.Add(child.gameObject);
            if (child.GetComponent<TintScript>() != null)
            {
                child.GetComponent<TintScript>().selfColor = new Color(Random.value, Random.value, Random.value);
            }
            child.localScale = Vector3.one * 0.001f;
        }
	}
	
	void Update ()
    {
        timer += Game.dt;
        for (int i = 0; i < rectangles.Count; i++)
        {
            if (timer < i * period / rectangles.Count)
            {
                rectangles[i].transform.localScale = Vector3.one * 0.1f;
            }
            else if (timer < (i + 1) * period / rectangles.Count)
            {
                rectangles[i].transform.localScale = Vector3.one * Easing.CircEaseIn((timer - i * period / rectangles.Count), 0.1f, 0.9f, (period / rectangles.Count));
            }
            else
            {
                rectangles[i].transform.localScale = Vector3.one;
            }
            //rectangles[i].transform.localPosition = new Vector3(0f, 0f, i * 0.01f);
        }
        if (timer >= period)
        {
            if (disableObject != null)
            {
                disableObject.SetActive(false);
            }
        }
	}
}
