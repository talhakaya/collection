using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public int x;
    public int y;
    private TintScript tint;
    public float r;
    public float g;
    public float b;

	void Start ()
    {
        tint = GetComponent<TintScript>();
        GetComponent<Tilt>().period *= Random.Range(0.2f, 5f);
        x = Mathf.RoundToInt(transform.position.x + 15.5f);
        y = Mathf.RoundToInt(transform.position.y + 8.5f);
        name = "tile" + x + "_" + y;
	}
	
	void Update ()
    {
	    
	}

    public void updateColor()
    {
        float surplusR = (r > 1f) ? (r - 1f) : 0f;
        float surplusG = (g > 1f) ? (g - 1f) : 0f;
        float surplusB = (b > 1f) ? (b - 1f) : 0f;
        r = Mathf.Max(0f, Mathf.Min(1f, r - surplusG - surplusB));
        g = Mathf.Max(0f, Mathf.Min(1f, g + 0.4f - surplusR - surplusB));
        b = Mathf.Max(0f, Mathf.Min(1f, b + 0.4f - surplusR - surplusG));
        //saturate
        tint.selfColor = new Color(r, g, b);
    }
}
