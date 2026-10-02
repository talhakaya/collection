using UnityEngine;
using System.Collections;

public class ShadowScript : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load("Shadow") as GameObject;
	public tk2dSprite sprite;
	public Vector2 scale;
	public Vector2 position;
	public bool flicker;
	public bool dynamicScale;
	public bool dynamicPosition;
	public float period;
	public float periodCounter;
	
	public static GameObject createShadow(Transform parent, Vector2 _scale, Vector2 _position)
	{
		GameObject g = Instantiate(prefab, parent.position, parent.rotation) as GameObject;
		g.transform.parent = parent;
		g.name = "Shadow";
		ShadowScript gShadowScript = g.GetComponent<ShadowScript>();
		gShadowScript.scale = _scale;
		gShadowScript.position = _position;
		
		return g;
	}
	
	void Start ()
	{
		transform.localPosition = new Vector3(position.x, position.y, 0.1f);
		transform.localScale = new Vector3(scale.x, scale.y, 1f);
		period = 0.2f;
		periodCounter = 0f;
		sprite = gameObject.GetComponent<tk2dSprite>();
		flicker = dynamicPosition = dynamicScale = true;
	}
	
	void Update ()
	{
		if (flicker || dynamicScale || dynamicPosition)
		{
			periodCounter += Time.deltaTime;
			if (periodCounter >= period)
			{
				periodCounter = 0f;
				if (flicker)
				{
					sprite.color = new Color(1f, 1f, 1f, Random.Range (0.7f, 1.2f));
				}
				if (dynamicScale)
				{
					transform.localScale = new Vector3(scale.x + Random.Range(-0.1f, 0.1f), scale.y + Random.Range(-0.1f, 0.1f), 1f);
				}
				if (dynamicPosition)
				{
					transform.localPosition = new Vector3(position.x + Random.Range(-0.1f, 0.1f), position.y + Random.Range(-0.1f, 0.1f), 0.1f);
				}
			}
			
		}
	}
}
