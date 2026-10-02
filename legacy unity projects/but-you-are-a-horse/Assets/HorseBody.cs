using UnityEngine;
using System.Collections;

public class HorseBody : MonoBehaviour {

    private PlatformerController horse;

	void Start ()
    {
        horse = transform.parent. GetComponent<PlatformerController>();
	}
	
	void Update ()
    {
	
	}

    void resetHitAnim()
    {
        horse.hitTimeCounter = 0f;
    }
}
