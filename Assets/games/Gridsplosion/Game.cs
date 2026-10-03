using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Collection.Controls;

namespace Games.Gridsplosion
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
	    public static Vector3 shadowVector;
	    public GameObject tile;
	    public static List<List<Tile>> tiles;
	    public static List<ExplosionData> explosionData;
	    public Transform tilesTransform;
	    public static float inputHorizontal;
	    public static float inputVertical;

		private Transform aimFrom;

		void Start ()
		{
	        shadowVector = Vector3.down + Vector3.left;
	        if (tile != null)
	        {
	            tiles = new List<List<Tile>>();
	            for (float j = -8.5f; j <= 8.5f; j++)
	            {
	                tiles.Add(new List<Tile>());
	                for (float i = -15.5f; i <= 15.5f; i++)
	                {
	                    Tile t = (Instantiate(tile, new Vector3(i, j, 10f), Quaternion.identity) as GameObject).GetComponent<Tile>();
	                    tiles[Mathf.RoundToInt(j + 8.5f)].Add(t);
	                    t.transform.SetParent(tilesTransform);
	                }
	            }
	        }
	        explosionData = new List<ExplosionData>();
		}

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

			dt = Time.deltaTime;
			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			// In the collection: with a gamepad the right stick aims where the mouse did.
			if (aimFrom == null)
			{
				Player found = FindFirstObjectByType<Player>();
				if (found != null) aimFrom = found.transform;
			}
			if (aimFrom != null)
			{
				MousePosition.get = Collection.Controls.PortHelpers.AimPoint(MousePosition.get, aimFrom.position, 3f);
			}
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0) || TaloketoInputManager.GetButton("Fire"); // In the collection: a pad button fires too
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        inputHorizontal = TaloketoInputManager.GetAxis("Horizontal");
	        inputVertical = TaloketoInputManager.GetAxis("Vertical");

	        explosionCalculations();
	        tileCalculations();
		}

	    private static void explosionCalculations()
	    {
	        for (int k = 0; k < explosionData.Count; k++)
	        {
	            if (explosionData[k].time < time - explosionData[k].period)
	            {
	                explosionData.RemoveAt(k);
	            }
	        }
	    }

	    public static void explosion(ExplosionData data)
	    {
	        explosionData.Add(data);
	    }

	    private static void tileCalculations()
	    {
	        for (int i = 0; i < tiles.Count; i++)
	        {
	            for (int j = 0; j < tiles[i].Count; j++)
	            {
	                Tile t = tiles[i][j];
	                t.r = 0f;
	                t.g = 0f;
	                t.b = 0f;

	                float val = -Mathf.Pow(t.x, 2f) + t.y + 10f * time;
	                float val2 = t.x + Mathf.Pow(t.y, 2f) - 10f * time;
	                //t.r += Mathf.Abs(val * 0.01f) % 1f;
	                t.g += Mathf.Abs(val * 0.012f) % 0.6f;
	                t.b += Mathf.Abs(val2 * 0.014f) % 0.6f;

	                for (int k = 0; k < explosionData.Count; k++)
	                {
	                    Color explosionColor = explosionData[k].r(t.transform.position);
	                    t.r += explosionColor.r;
	                    t.g += explosionColor.g;
	                    t.b += explosionColor.b;
	                }

	                t.updateColor();
	            }
	        }
	    }
	}
}
