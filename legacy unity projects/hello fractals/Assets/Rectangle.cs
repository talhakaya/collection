using UnityEngine;
using System.Collections;

public class Rectangle : MonoBehaviour {

    private float x;
    private float y;
    private TintScript tint;

	void Start ()
    {
        tint = GetComponent<TintScript>();
        x = transform.position.x / 0.1f;
        y = transform.position.y / 0.1f;
	}

	void Update ()
    {
        float n = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            n = Game.currentFunction(x, y);

        }
        else if (Input.GetKey(KeyCode.W))
        {
            n = Game.currentFunction2(x, y);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            n = Game.currentFunction2(x, y);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            n = Game.semiRandFunction2(x, y);
        }
        else if (Input.GetKey(KeyCode.T))
        {
            n = Game.semiRandFunction(x, y);
        }
        else
        {
            n = Game.randFunction(x, y);
        }

        if (Game.blackWhite)
        {
            tint.selfColor = Game.getColor0(n);
        }
        else
        {
            tint.selfColor = Game.getColor5(n);
        }
	}
}
