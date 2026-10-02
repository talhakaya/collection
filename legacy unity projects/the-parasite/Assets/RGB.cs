using UnityEngine;
using System.Collections;

public class RGB : MonoBehaviour {

	void Start ()
    {
        SpriteEffect.make(Effect.RGBSplit, gameObject, false, true, Camera.main.transform);
	}
}
