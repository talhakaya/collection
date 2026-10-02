using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    private LineManager line;
    private bool opened;
    private float scaleX;
	
    void Start ()
    {
        line = GetComponent<LineManager>();
        scaleX = transform.localScale.x;
        line.filterColor = new Color(1f, 0.4f, 0.4f, 1f);
	}
	
	void Update ()
    {
	    if (opened)
        {
            line.rgbSplitOffset += Game.dt * 10f;
            transform.localScale -= Vector3.right * scaleX * Game.dt;
            if (transform.localScale.x <= 0f)
            {
                Destroy(gameObject);
            }
        }
	}

    public void open()
    {
        opened = true;
        line.filterColor = new Color(0.4f, 1f, 0.4f, 1f);
    }
}
