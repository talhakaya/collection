using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class Menu : MonoBehaviour {

		public GameObject text;
		private float period = 2f;
		private float maxScale = 1.1f;
		private float firstScale = 0.3f;
		private float lastTime = 0f;

		void Start ()
		{
			Game.miniGame = gameObject;
			Game.isThereMiniGame = true;

			foreach (TextMesh mesh in GetComponentsInChildren<TextMesh>(true))
			{
				if (mesh.text == KeyboardPrompt)
				{
					prompts.Add(mesh);
				}
			}
		}

		// In the collection: the "Press Enter" line names the gamepad's button while a
		// gamepad is the device in use. (The tutorial and "Enter to Restart" are drawn into
		// the art and stay as they are.)
		private const string KeyboardPrompt = "Press Enter";
		private const string GamepadPrompt = "Press A";
		private readonly System.Collections.Generic.List<TextMesh> prompts = new System.Collections.Generic.List<TextMesh>();

		private static bool usingGamepad()
		{
			var pad = UnityEngine.InputSystem.Gamepad.current;
			var keyboard = UnityEngine.InputSystem.Keyboard.current;
			return pad != null && (keyboard == null || pad.lastUpdateTime > keyboard.lastUpdateTime);
		}

		void Update () {
			string prompt = usingGamepad() ? GamepadPrompt : KeyboardPrompt;
			foreach (TextMesh mesh in prompts)
			{
				if (mesh.text != prompt)
				{
					mesh.text = prompt;
				}
			}

			float ratio = (Game.time % (period * 2)) / period;
			float ratio2 = (ratio + 0.5f) % 2f;
			float scaleX = 1f;
			float scaleY = 1f;
			if (ratio < 1f)
			{
				scaleX = 1f + (maxScale - 1f) * ratio;
			}
			else
			{
				scaleX = 1f + (maxScale - 1f) * (2 - ratio);
			}
			if (ratio2 < 1f)
			{
				scaleY = 1f + (maxScale - 1f) * ratio2;
			}
			else
			{
				scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
			}
			text.transform.localScale = new Vector3(firstScale * scaleX / transform.localScale.x, firstScale * scaleY / transform.localScale.y, 1f / transform.localScale.z);
			if (TaloketoInputManager.GetButtonDown("Interact") || TaloketoInputManager.GetButtonDown("Select"))
			{
				Game.time = 0f;
				Game.endMiniGame();
			}
			float soundTime = Game.time % 0.5f;
			if (lastTime < 0.25f && soundTime >= 0.25f)
			{
				Game.instance.aBush.pitch = Random.Range(0.5f, 1f);
				Game.instance.aBush.Play ();
			}
			lastTime = soundTime;
		}
	}
}
