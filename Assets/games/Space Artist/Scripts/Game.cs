using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
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
	    public static int level;

	    public Transform mouseLines;
	    public Transform mouseLine0;
	    public Transform mouseLine1;
	    public Transform mouseSquare;

		void Start ()
		{
	        level = 0;
	        shadowVector = Vector3.down + Vector3.left;
		}
		
		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
	        {
	            Application.Quit();
	        }
			
			dt = Time.deltaTime;
			time += dt;

	        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hit;
	        if (PlaneManager.instance.open && Physics.Raycast(ray, out hit) && hit.transform.tag == "Plane")
	        {
	            input = Input.GetMouseButton(0);
	            MousePosition.get = (Game.level == 26 ? PlayerScript.instance.transform.position : transform.position) + (Game.level == 26 ? 1.4f : 1f) * hit.transform.parent.InverseTransformPoint(mousePos) - hit.transform.localPosition;
	            MousePosition.get.z = 0f;
	            MousePosition.x = MousePosition.get.x;
	            MousePosition.y = MousePosition.get.y;
	            mouseLines.gameObject.SetActive(true);
	            if (Mathf.Abs(mousePos.x) > Mathf.Abs(mousePos.y))
	            {
	                mouseLine0.eulerAngles = Vector3.zero;
	                mouseLine0.localScale = new Vector3((MousePosition.x - mouseLines.position.x) * 100f, 10f, 1f);
	                mouseLine1.localScale = new Vector3((MousePosition.y - mouseLines.position.y) * 100f / mouseLine0.localScale.y, 10f / mouseLine0.localScale.x, 1f);
	            }
	            else
	            {
	                mouseLine0.eulerAngles = Vector3.forward * 90f;
	                mouseLine0.localScale = new Vector3((MousePosition.y - mouseLines.position.y) * 100f, 10f, 1f);
	                mouseLine1.localScale = new Vector3(-(MousePosition.x - mouseLines.position.x) * 100f / mouseLine0.localScale.y, 10f / mouseLine0.localScale.x, 1f);
	            }
	            mouseSquare.transform.position = MousePosition.get;
	        }
	        else
	        {
	            MousePosition.get = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	            MousePosition.get.z = 0f;
	            MousePosition.x = MousePosition.get.x;
	            MousePosition.y = MousePosition.get.y;
	            input = false;
	            mouseLines.gameObject.SetActive(false);
	        }

			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}
	}

}
