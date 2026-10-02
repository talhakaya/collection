using UnityEngine;
using System.Collections;

public class RandomRotate : MonoBehaviour
{
    private float rotateSpeed;
    public float addToYRandomly;
    public float scaleRandomly;

	void Start ()
    {
        transform.eulerAngles += Vector3.up * Random.value * 360;
        rotateSpeed = Random.Range(60f, 180f);
        if (Random.value < 0.5f)
        {
            rotateSpeed *= -1;
        }
        transform.position += Vector3.up * addToYRandomly * Random.value;
        foreach (Transform child in transform)
        {
            child.transform.localScale *= 1f + Random.value * scaleRandomly;
        }
	}
	
	void Update ()
    {
        transform.eulerAngles += Vector3.up * rotateSpeed * Game.dt;
	}
}
