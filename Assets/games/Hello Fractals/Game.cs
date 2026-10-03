using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.HelloFractals
{
	public class Game : MonoBehaviour {

		public static float time;
		public static float dt;
		public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
		public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
	    public GameObject rectanglePrefab;
	    private static float[] rands;
	    public static bool trigon = false;
	    public static bool blackWhite = false;

	    public static bool keyQ, keyW, keyE, keyR, keyT;

		// In the collection: the process outlives the toy, so the statics start over here.
		void Awake ()
		{
			time = 0f;
			inputOld = false;
			trigon = false;
			blackWhite = false;
		}

		void Start ()
		{
			for (float i = -79.5f; i < 80.5f; i++)
	        {
	            for (float j = -44.5f; j < 45.5f; j++)
	            {
	                GameObject r = Instantiate(rectanglePrefab, new Vector3(i * 0.1f, j * 0.1f, 0f), Quaternion.identity) as GameObject;
	            }
	        }
	        randomizeInt();
	        Debug.Log(rands);
		}

	    void randomize()
	    {
	        rands = new float[] { Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f) };
	        while (rands[5] == 0)
	        {
	            rands[5] = Random.Range(-10, 10);
	        }
	    }

	    void randomizeInt()
	    {
	        rands = new float[] { Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10) };
	        while (rands[5] == 0)
	        {
	            rands[5] = Random.Range(-10, 10);
	        }
	    }

		void Update ()
	    {
	        // In the collection: the Escape-quit is gone (the collection has its own exit).

	        // In the collection: each of the 14400 rectangles used to ask for five keys every
	        // frame. The keys are read once here and the rectangles look at the answer.
	        keyQ = TaloketoInputManager.GetButton("FunctionQ");
	        keyW = TaloketoInputManager.GetButton("FunctionW");
	        keyE = TaloketoInputManager.GetButton("FunctionE");
	        keyR = TaloketoInputManager.GetButton("FunctionR");
	        keyT = TaloketoInputManager.GetButton("FunctionT");

	        dt = Time.deltaTime;
	        if (TaloketoInputManager.GetButton("Fast"))
	        {
	            time += 100 * dt;
	        }
	        else
	        {
	            time += 10 * dt;
	        }

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

	        input = TaloketoInputManager.GetMouseButton(0) || TaloketoInputManager.GetButton("New");
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

	        if (inputDown)
	        {
	             time = 0f;
	             randomizeInt();
	             trigon = !trigon;
	        }

	        if (TaloketoInputManager.GetButtonDown("BlackWhite"))
	        {
	            blackWhite = !blackWhite;
	        }

			inputOld = input;
		}

	    public static float currentFunction(float x, float y)
	    {
	        return x * x + y * y + time * time;
	    }

	    public static float currentFunction2(float x, float y)
	    {
	        return 10 * Mathf.Sin(x) * x + 10 * Mathf.Cos(y) * y + time * time;
	    }

	    public static Color getColor0(float f)
	    {
	        float f2 = Mathf.Abs(f);
	        if (f < 0)
	        {
	            return new Color((f2 % 10000f) / 10000f, (f2 % 10000f) / 10000f, (f2 % 10000f) / 10000f);
	        }
	        return new Color((f2 % 1000f) / 1000f, (f2 % 1000f) / 1000f, (f2 % 1000f) / 1000f);
	    }

	    public static Color getColor1(float f)
	    {
	        return new Color((f % 1000f) / 1000f, (f % 100f) / 100f, (f % 10f) / 10f);
	    }

	    public static Color getColor2(float f)
	    {
	        float f2 = Mathf.Abs(f);
	        return new Color((f2 % 1000f) / 1000f, (f2 % 100f) / 100f, (f2 % 10f) / 10f);
	    }

	    public static Color getColor3(float f)
	    {
	        float f2 = Mathf.Abs(f);
	        if (f < 0)
	        {
	            return new Color((f2 % 1000f) / 1000f, (f2 % 100f) / 100f, (f2 % 10000f) / 10000f);
	        }
	        return new Color((f2 % 10000f) / 10000f, (f2 % 1000f) / 1000f, (f2 % 100f) / 100f);
	    }

	    public static Color getColor4(float f)
	    {
	        float f2 = Mathf.Abs(f);
	        if (f < 0)
	        {
	            return new Color((f2 % 10000f) / 10000f, (f2 % 1000f) / 1000f, (f2 % 100000f) / 100000f);
	        }
	        return new Color((f2 % 100000f) / 100000f, (f2 % 10000f) / 10000f, (f2 % 1000f) / 1000f);
	    }

	    public static Color getColor5(float f)
	    {
	        float f2 = Mathf.Abs(f);
	        if (f < 0)
	        {
	            return new Color((f2 % 100000f) / 100000f, (f2 % 10000f) / 10000f, (f2 % 1000000f) / 1000000f);
	        }
	        return new Color((f2 % 1000000f) / 1000000f, (f2 % 100000f) / 100000f, (f2 % 10000f) / 10000f);
	    }

	    public static float randFunction(float x, float y)
	    {
	        return rands[0] * x * x + rands[1] * x * y + rands[2] * y * y + rands[3] * x * time + rands[4] * y * time + rands[5] * time * time + rands[6] * y + rands[7] * x + rands[8] * time + rands[9];
	    }

	    public static float semiRandFunction(float x, float y)
	    {
	        return rands[0] * x * x + rands[2] * y * y + rands[5] * time * time;
	    }

	    public static float semiRandFunction2(float x, float y)
	    {
	        return rands[0] * 10 * Mathf.Sin(x) * x + rands[2] * 10 * Mathf.Cos(y) * y + rands[5] * time * time;
	    }
	}
}
