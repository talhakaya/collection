using UnityEngine;
using System.Collections;

public class ArcadeCryingTissue : MonoBehaviour {
	
	public int tearCount;

	private tk2dSprite sprite;
	private DraggableObject drag;
	private int tissueState;
	private const int maxTissueState = 5;
	
	void Start ()
	{
		sprite = gameObject.GetComponent<tk2dSprite>();
		drag = gameObject.GetComponent<DraggableObject>();
		tearCount = 0;
		tissueState = 1;
		transform.Rotate(Vector3.forward * Random.Range(-10, 10));
	}
	
	void Update ()
	{
		float tearRatio = tearCount / 20f;
		for (int i = 2; i <= maxTissueState; i++)
		{
			if (i < maxTissueState)
			{
				if (tissueState == (i - 1) && tearRatio > (i - 1) * 0.5f / (maxTissueState - 1))
				{
					tissueState++;
					sprite.SetSprite("tissue" + tissueState);
				}
				if (tissueState < i)
				{
					break;
				}
			}
			else if (tearRatio > 0.5f)
			{
				tearRatio = 0.5f;
				Destroy (rigidbody);
				sprite.SetSprite("tissueDone");
			}
		}
		
		if (drag.hold)
		{
			sprite.color = new Color(1f - tearRatio, 1f - tearRatio, 1f, 1f);
		}
		else
		{
			sprite.color = new Color(1f - tearRatio, 1f - tearRatio, 1f, 0.5f);
		}
	}
}
