using UnityEngine;
using System.Collections;

public class EnemyFiring : MonoBehaviour {
	
	private float timeCounter;
	public float firePeriod;
	private float fireSpriteCounter;
	private float fireSpritePeriod = 0.3f;
	public float turningSpeed;
	public int firesPerShot;
	private EnemyScript enemyScript;
	private SpriteRenderer spriteRenderer;

	public Sprite notFiring;
	public Sprite firing;

	void Start ()
	{
		enemyScript = GetComponent<EnemyScript> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		enemyScript.hp = 200f;

		timeCounter = Random.Range (0f, firePeriod / 2);

	}

	void Update ()
	{
		if (PlayerScript.instance != null)
		{
			timeCounter += Time.deltaTime;
			float goalAngle = Mathf.Atan2(PlayerScript.instance.transform.position.y - transform.position.y, PlayerScript.instance.transform.position.x - transform.position.x);
			goalAngle = Geometry.mod (180 * goalAngle / Mathf.PI, 360);
			float currentAngle = Geometry.mod (transform.eulerAngles.z, 360);
			if ((goalAngle < 90 && currentAngle >= 270) || (currentAngle < 90 && goalAngle >= 270))
			{
				if (goalAngle > currentAngle)
				{
					transform.Rotate(-Vector3.forward * turningSpeed * Time.deltaTime);
				}
				else
				{
					transform.Rotate(Vector3.forward * turningSpeed * Time.deltaTime);
				}
			}
			else
			{
				if (goalAngle > currentAngle)
				{
					transform.Rotate(Vector3.forward * turningSpeed * Time.deltaTime);
				}
				else
				{
					transform.Rotate(-Vector3.forward * turningSpeed * Time.deltaTime);
				}
			}

			if (timeCounter > firePeriod && !enemyScript.frozen)
			{
				timeCounter = 0f;
				Fire();
			}
		}

		if (spriteRenderer.sprite == firing)
		{
			fireSpriteCounter += Time.deltaTime;
			if (fireSpriteCounter >= fireSpritePeriod)
			{
				fireSpriteCounter = 0f;
				spriteRenderer.sprite = notFiring;
			}
		}
	}

	void Fire()
	{
		spriteRenderer.sprite = firing;
		for (int i = 0; i < firesPerShot; i++)
		{
			Game.enemyFire.GetComponent<EnemyFire> ().direction = transform.eulerAngles.z - 90 + (i + 0.5f - firesPerShot / 2f) * 30f;
			Instantiate (Game.enemyFire, transform.position + Vector3.forward, Quaternion.identity);
		}
	}
}
