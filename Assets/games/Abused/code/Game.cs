using UnityEngine;
using System.Collections;
using Collection.Controls;

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

		[Tooltip("How far the right stick has to be pushed before it aims the character. Below this the stick is ignored and the character keeps facing the way it was.")]
		public float aimStickThreshold = 0.3f;
		[Tooltip("How far from the player the stick's aim point sits, in world units. The character only needs the direction - this is what the camera leans towards, standing in for where a mouse would be held. 0 keeps the camera centred on the player.")]
		public float aimStickDistance = 60f;
		[Tooltip("How long the camera takes to follow when the stick's aim direction changes: it covers about two thirds of the way in this many seconds. Only the camera is smoothed - the character still snaps to the stick. 0 turns it off. No effect when aiming with the mouse.")]
		public float aimStickCameraSmoothTime = 0.3f;

		private Transform player;
		private bool aimingWithStick;
		private Vector3 stickAimDirection = Vector3.right;
		private Vector3 stickCameraOffset;
		private Vector3 lastMouseScreenPosition;

		void Start ()
		{
			Player playerScript = FindFirstObjectByType<Player>();
			player = playerScript != null ? playerScript.transform : null;
			lastMouseScreenPosition = TaloketoInputManager.mousePosition;
		}
		
		void Update ()
		{
	        RenderGrayScale.instance.noiseRatioSet = SoulPoint.pickedSoul * 25f / SoulPoint.totalSoul;
			dt = Time.deltaTime;
			time += dt;

	        Vector2 normalizedMouse = new Vector2(
				TaloketoInputManager.mousePosition.x / Screen.width,
				TaloketoInputManager.mousePosition.y / Screen.height
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

			// Gamepad aiming. Everything that aims in this game - the character's facing, the
			// camera's lean, the sprite effects - reads MousePosition, so the right stick only
			// has to stand in for it: while the stick is what's aiming, the "mouse" is a point
			// aimStickDistance from the player in the stick's direction. No smoothing; the
			// character snaps to the stick.
			//
			// Releasing the stick keeps the last direction rather than dropping back to the
			// real mouse, which would be sitting wherever it was last left and would swing the
			// character round as it walks. The mouse takes over again when it actually moves.
			Vector2 stick = new Vector2(TaloketoInputManager.GetAxisRaw("Aim Horizontal"), TaloketoInputManager.GetAxisRaw("Aim Vertical"));
			Vector3 mouseScreenPosition = TaloketoInputManager.mousePosition;
			if (stick.magnitude >= aimStickThreshold)
			{
				if (!aimingWithStick && player != null)
				{
					// Taking over from the mouse: the camera's lean starts from where the
					// mouse had it, so it eases across rather than jumping.
					Vector3 fromMouse = MousePosition.get - player.position;
					fromMouse.z = 0f;
					stickCameraOffset = Vector3.ClampMagnitude(fromMouse, aimStickDistance);
				}

				aimingWithStick = true;
				stickAimDirection = stick.normalized;
			}
			else if (mouseScreenPosition != lastMouseScreenPosition)
			{
				aimingWithStick = false;
			}
			lastMouseScreenPosition = mouseScreenPosition;

			if (aimingWithStick && player != null)
			{
				// Depth is left as the mouse path set it: only x and y are ever aimed at.
				float depth = MousePosition.get.z;
				Vector3 aimOffset = stickAimDirection * Mathf.Max(aimStickDistance, 0.01f);
				Vector3 aimPoint = player.position + aimOffset;

				// A stick flicks from one direction to another instantly, where a mouse has to
				// travel there - so the camera, which follows the aim point, lurched on every
				// flick. Only the camera's copy of the point is eased; the facing stays
				// instant. Flipping right round takes the lean back through the player, so
				// the camera re-centres and then leans the other way.
				//
				// This relies on who reads what: cameraScript reads MousePosition.get, while
				// Player and SpriteEffect read MousePosition.x / .y.
				float blend = aimStickCameraSmoothTime > 0f ? 1f - Mathf.Exp(-dt / aimStickCameraSmoothTime) : 1f;
				stickCameraOffset = Vector3.Lerp(stickCameraOffset, aimOffset, blend);

				MousePosition.get = player.position + stickCameraOffset;
				MousePosition.get.z = depth;
				MousePosition.x = aimPoint.x;
				MousePosition.y = aimPoint.y;
			}
			else
			{
				MousePosition.x = MousePosition.get.x;
				MousePosition.y = MousePosition.get.y;
			}

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

			// Escape used to quit the application here. The collection binds its own exit
			// combination (Select+Start, or Shift+Escape), so a bare Escape quitting is both
			// redundant and a way to lose the whole collection by accident.
		}
	}

}
