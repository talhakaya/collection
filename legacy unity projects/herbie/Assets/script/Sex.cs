using UnityEngine;
using System.Collections;

public class Sex : MonoBehaviour {

	public GameObject hammer;
	public GameObject nail;
	private bool pressing;
	private bool pressingOld = true;

	void Start ()
	{
		Game.miniGame = gameObject;
		Game.isThereMiniGame = true;
	}

	void Update ()
	{
		if (Input.GetButton("Interact") || Input.GetAxisRaw("Vertical") == -1)
		{
			pressing = true;
		}
		else
		{
			pressing = false;
		}

		if (pressing && !pressingOld)
		{
			nail.transform.position += Vector3.down * Random.Range(0.01f, 0.05f);
			hammer.transform.rotation = Quaternion.identity;
			Game.instance.aHammer.pitch = Random.Range(0.7f, 1.3f);
			Game.instance.aHammer.Play();
		}
		else if (!pressing && pressingOld)
		{
			hammer.transform.Rotate(Vector3.forward * (- 30));
		}

		if (nail.transform.position.y < -1.94f)
		{
			Herbie.instance.endSex();
			Game.endMiniGame();
		}

		hammer.transform.position = new Vector3(hammer.transform.position.x, nail.transform.position.y + 1.76f, hammer.transform.position.z);

		pressingOld = pressing;
	}
}
