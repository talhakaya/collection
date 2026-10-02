using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
    private TintScript tint;
	void Start ()
    {
        tint = GetComponent<TintScript>();
	}
	
	void Update ()
    {
	    if (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) != Vector2.zero || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))
        {
            tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Game.dt / 3f);
            if (tint.selfColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
	}
}
