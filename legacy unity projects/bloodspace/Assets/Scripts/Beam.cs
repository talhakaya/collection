using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

	private float deathTime = 10f;
	private float timeCounter;
	public float firstScale = 1f;
	public float lifeTime;

	void Start ()
	{
		gameObject.name = "Beam";
		transform.parent = PlayerScript.instance.transform;

		SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
		SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
	
	}

	void Update ()
	{
		timeCounter += Time.deltaTime;
		if (timeCounter >= lifeTime)
		{
			if (timeCounter > deathTime)
			{
				Destroy(gameObject);
			}

			transform.localScale = new Vector3(0f, 1f, 1f);
		}
		else
		{
			transform.localScale = new Vector3(firstScale * (lifeTime - timeCounter), 1f, 1f);
		}
	}
	
	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Enemy")
		{
			coll.gameObject.GetComponent<EnemyScript>().hp -= 1000f * Time.deltaTime * transform.localScale.x;
		}
	}
}
