using UnityEngine;
using System.Collections;

namespace Games.HelloFractals
{
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
	        if (Game.keyQ)
	        {
	            n = Game.currentFunction(x, y);

	        }
	        else if (Game.keyW)
	        {
	            n = Game.currentFunction2(x, y);
	        }
	        else if (Game.keyE)
	        {
	            n = Game.currentFunction2(x, y);
	        }
	        else if (Game.keyR)
	        {
	            n = Game.semiRandFunction2(x, y);
	        }
	        else if (Game.keyT)
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
}
