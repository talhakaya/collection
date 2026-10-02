using UnityEngine;
using System.Collections;

public class Talha : MonoBehaviour {

	public float speed;

	void Start ()
	{
		name = "Talha";

		GameObject tilePrefab = Resources.Load<GameObject> ("Tile");

		Vector2 rand1 = new Vector2 (Random.Range (-8, 8), Random.Range (-4, 4));
		Vector2 rand2 = new Vector2 (Random.Range (-8, 8), Random.Range (-4, 4));
		while (rand1.x == rand2.x || rand1.y == rand2.y)
		{
			rand2 = new Vector2 (Random.Range (-8, 8), Random.Range (-4, 4));
		}
		for (float i = -9; i < 9; i++)
		{
			for (float j = -5f; j < 5f; j++)
			{
				GameObject tile = Instantiate(tilePrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity) as GameObject;

				if (i == rand1.x && j == rand1.y)
				{
					tile.GetComponent<Tile>().blackTime = 3f;
				}
				else if (i == rand2.x && j == rand2.y)
				{
					tile.GetComponent<Tile>().blackTime = 4.5f;
				}
				else
				{
					tile.GetComponent<Tile>().blackTime = Random.Range(6f, 8f);
				}
			}
		}
	}

	void Update ()
	{
		if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
		{
			Vector3 force = Geometry.normalizeVector2(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), speed);
			rigidbody2D.AddForce(force * Game.dt);
		}
	}
}
