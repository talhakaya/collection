using UnityEngine;
using System.Collections;

public class Fight : MonoBehaviour {
	
	public GameObject herbie;
	public GameObject fist;
	public GameObject crosshair;
	private Vector3 herbiePosition;
	private Vector3 fistScale;
	private float time = 0f;
	private bool decided;
	private bool fistTurningBack;
	private bool isRight;
	private float dt;
	private float dtMultiplier = 1f;


	void Start ()
	{
		Game.miniGame = gameObject;
		Game.isThereMiniGame = true;
		herbiePosition = herbie.transform.position;
		fistScale = fist.transform.localScale;
	}

	void Update ()
	{
		dt = Game.dt * dtMultiplier;
		time += dt;

		herbie.transform.position = herbiePosition + Vector3.right * Input.GetAxis("Horizontal") * 2f + Vector3.up * Input.GetAxis("Vertical") * 1.5f;
		crosshair.transform.Rotate(Vector3.forward * 100 * dt);

		if (time < 1f)
		{
			fist.transform.position = new Vector3(fist.transform.position.x, 10, fist.transform.position.z);
			crosshair.transform.position = new Vector3(crosshair.transform.position.x, 10, crosshair.transform.position.z);
			decided = false;
			fistTurningBack = false;
		}
		else if (time < 3f)
		{
			if (!decided)
			{
				decided = true;
				dtMultiplier += 0.2f;
				if (Random.Range(0f, 1f) < 0.5f)
				{
					isRight = true;
					fist.transform.localScale = fistScale;
					crosshair.transform.position = new Vector3(2f, Random.Range(2f, -1f), crosshair.transform.position.z);
					fist.transform.position = new Vector3(8f, crosshair.transform.position.y, fist.transform.position.z);
				}
				else
				{
					isRight = false;
					fist.transform.localScale = new Vector3(-fistScale.x, fistScale.y, fistScale.z);
					crosshair.transform.position = new Vector3(-2f, Random.Range(2f, -1f), crosshair.transform.position.z);
					fist.transform.position = new Vector3(-8f, crosshair.transform.position.y, fist.transform.position.z);
				}
			}
			if (isRight)
			{
				fist.transform.position -= Vector3.right * 4f * dt;
			}
			else
			{
				fist.transform.position += Vector3.right * 4f * dt;
			}
			if (!fistTurningBack && time > 1.6f)
			{
				fistTurningBack = true;
				Game.instance.aFist.pitch = 0.75f + dtMultiplier / 4f;
				Game.instance.aFist.Play ();
			}
		}
		else if (time < 5f)
		{
			if (isRight)
			{
				fist.transform.position += Vector3.right * 4f * dt;
			}
			else
			{
				fist.transform.position -= Vector3.right * 4f * dt;
			}
		}
		else
		{
			time -= 5f;
		}
	}
}
