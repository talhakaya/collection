using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class Cursor : MonoBehaviour {

		private SpriteRenderer spriteRenderer;

		void Start ()
		{
			// In the collection: was Screen.showCursor = false. The collection draws the pointer,
			// so it is told to stop while this game draws its own.
			GlobalInputManager.HideGameCursor();
			spriteRenderer = GetComponent<SpriteRenderer> ();
			Game.mouseMode = false;
		}

		// In the collection: the M key that switched mouse control on and off is gone. The
		// ship follows the mouse from the moment the mouse is moved, and goes back
		// to the keys or the gamepad from the moment one of those steers it.
		private const float MouseMoveToTakeOver = 2f;

		private static bool steeredByKeysOrPad()
		{
			return TaloketoInputManager.GetAxis("Horizontal0") != 0f || TaloketoInputManager.GetAxis("Vertical0") != 0f
				|| Mathf.Abs(TaloketoInputManager.GetAxis("Horizontal1")) > 0.3f || Mathf.Abs(TaloketoInputManager.GetAxis("Vertical1")) > 0.3f;
		}

		private static bool mouseUsed()
		{
			UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
			return mouse != null && mouse.delta.ReadValue().sqrMagnitude > MouseMoveToTakeOver * MouseMoveToTakeOver;
		}

		void Update ()
		{
			transform.position = MousePosition.get ();
			if (steeredByKeysOrPad())
			{
				Game.mouseMode = false;
			}
			else if (mouseUsed())
			{
				Game.mouseMode = true;
			}

			float alpha = 1f;
			if (Game.mouseMode)
			{
				if (PlayerScript.instance != null)
				{
					float distance = Geometry.lengthOfVector3 (new Vector2(transform.position.x - PlayerScript.instance.position.x, transform.position.y - PlayerScript.instance.position.y));
					if (distance < 1f)
					{
						alpha = distance;
					}
				}
			}
			else if (PlayerScript.instance != null)
			{
				alpha = 0f;
			}

			spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
		}
	}
}
