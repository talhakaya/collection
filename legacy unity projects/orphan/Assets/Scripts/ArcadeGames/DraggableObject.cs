using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {
	
	public static bool MouseHoldingAnObject;
	
	public bool hold;
	
	private Vector2 mouseOffset;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (!hold)
		{
			if (!MouseHoldingAnObject && Input.GetButtonDown("Fire"))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();
				
				if(Physics.Raycast(ray, out hit))
				{
					if (transform == hit.transform)
					{
						MouseHoldingAnObject = true;
						hold = true;
						mouseOffset = ray.origin - transform.position;
					}
				}
			}
		}
		if (hold)
		{
			if (Input.GetButtonUp("Fire"))
			{
				MouseHoldingAnObject = false;
				hold = false;
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				transform.position = new Vector3(ray.origin.x - mouseOffset.x, ray.origin.y - mouseOffset.y ,transform.position.z);
			}
		}
	}
}
