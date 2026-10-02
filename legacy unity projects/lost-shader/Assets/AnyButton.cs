using UnityEngine;
using System.Collections;

public class AnyButton : MonoBehaviour
{
    public int maxButton = 20;
    private int count;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            count++;
        }
	    if (count < maxButton)
        {
            transform.localScale = new Vector3(12.8f * count / maxButton, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(12.8f, transform.localScale.y, transform.localScale.z);
            if (transform.parent.GetComponent<MiniGame>().enabledTimer == 0f)
            {
                transform.parent.GetComponent<MiniGame>().end();
            }
        }
	}
}
