using UnityEngine;
using System.Collections;

public class ArcadeCryingEye : MonoBehaviour {
	
	private float counter;
	private GameObject tear;
	
	public float tearPeriod;
	
	// Use this for initialization
	void Start () {
		tearPeriod = 2f;
		tear = Resources.Load ("Arcade/ArcadeCrying/Tear") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		if (counter >= tearPeriod)
		{
			counter = 0;
			tearPeriod -= tearPeriod / 20;
			GameObject tearNew = Instantiate(tear, transform.position, transform.rotation) as GameObject;
			tearNew.name = "Tear";
			tearNew.transform.parent = transform.parent;
		}
	}
}
