using UnityEngine;
using System.Collections;

namespace Games.Abused
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

		public Camera cam;

		void Start ()
		{
			
		}
		
		void Update ()
		{
	        RenderGrayScale.instance.noiseRatioSet = SoulPoint.pickedSoul * 25f / SoulPoint.totalSoul;
			dt = Time.deltaTime;
			time += dt;

	        Vector2 normalizedMouse = new Vector2(
				Input.mousePosition.x / Screen.width,
				Input.mousePosition.y / Screen.height
			);

	        // 2. Scale normalized coordinates to RenderTexture dimensions
	        RenderTexture rt = cam.targetTexture;
	        Vector3 rtScreenPos = new Vector3(
	            normalizedMouse.x * rt.width,
	            normalizedMouse.y * rt.height,
	            10f // Distance in front of camWithRenderTexture
	        );

	        // 3. Convert directly to world point
	        MousePosition.get = cam.ScreenToWorldPoint(rtScreenPos);
	        MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = Input.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        if (Input.GetKey(KeyCode.Escape))
	        {
	            Application.Quit();
	        }
		}
	}

}
