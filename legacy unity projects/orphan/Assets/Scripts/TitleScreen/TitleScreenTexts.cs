using UnityEngine;
using System.Collections;

public class TitleScreenTexts : MonoBehaviour {
	
	public bool firstText;

	// Use this for initialization
	void Start () {
		if (firstText)
		{
			TextTalha.create(Vector3.up * 10f, "Orphan", 3, 5, Color.white, 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * 2f, "A Game By", 1, 5, new Color(0.7f, 0.7f, 0.7f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * 0f, "Kayabros", 1.5f, 5, new Color(1f, 0.5f, 0.5f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-4f), "Music By", 1, 5, new Color(0.7f, 0.7f, 0.7f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-6f), "Radio for the Daydreamers", 1.25f, 5, new Color(0.5f, 0.5f, 1f, 1f), 5f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-8f), "Midnight Moodswings", 1.25f, 5, new Color(0.5f, 0.5f, 1f, 1f), 5f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
		}
		else
		{
			TextTalha.create(Vector3.up * 10f, "Orphan", 3, 5, Color.white, 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * 2f, "A Game By", 1, 5, new Color(0.7f, 0.7f, 0.7f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * 0f, "Kayabros", 1.5f, 5, new Color(1f, 0.5f, 0.5f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-4f), "Music By", 1, 5, new Color(0.7f, 0.7f, 0.7f, 1f), 10f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-6f), "Radio for the Daydreamers", 1.25f, 5, new Color(0.5f, 0.5f, 1f, 1f), 5f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
			TextTalha.create(Vector3.up * (-8f), "Midnight Moodswings", 1.25f, 5, new Color(0.5f, 0.5f, 1f, 1f), 5f, 0.1f, new Vector2(0.1f, -0.1f), true).transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
