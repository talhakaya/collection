using UnityEngine;
using System.Collections;

public class Dither : MonoBehaviour
{
    public Color color;
    private TintScript tint;
    public float maxAlpha = 0.15f;
    public bool slightlyDifferentMaxAlpha = true;
    public float maxDistance = 2f;
    public float minDistance = 0.3f;

	void Start()
    {
        color.a = 0f;
        tint = GetComponent<TintScript>();
        tint.selfColor = color;
        if (slightlyDifferentMaxAlpha)
        {
            maxAlpha *= Random.Range(0f, 2f);
        }
	}
	
	void Update()
    {
        float d = Mathf.Abs(transform.position.x - MousePosition.x) + Mathf.Abs(transform.position.y - MousePosition.y);
        if (d > maxDistance)
        {
            color.a = maxAlpha;
            tint.selfColor = color;
        }
        else if (minDistance >= d)
        {
            color.a = 0f;
            tint.selfColor = color;
        }
        else
        {
            color.a = maxAlpha * (d - minDistance) / (maxDistance - minDistance);
            tint.selfColor = color;
        }
	}
}
