using UnityEngine;
using System.Collections;

public class ZSorter : MonoBehaviour
{
    public float offset;

	void Start ()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, offset + transform.localPosition.y * 0.1f);
	}
}
