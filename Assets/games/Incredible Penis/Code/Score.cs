using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.IncrediblePenis
{
	public class Score : MonoBehaviour {

		public Text scoreText;
		public int scoreToNextLevel;
		private float score;
		private Vector3 pos;

		// In the collection: the I+O+P cheat stays on the keyboard only.
		static bool cheat()
		{
			var k = UnityEngine.InputSystem.Keyboard.current;
			return k != null && k.iKey.isPressed && k.oKey.isPressed && k.pKey.isPressed;
		}

		void Start ()
		{
			pos = transform.position;
		}

		void Update ()
		{
			float x = Mathf.Abs(transform.position.x - pos.x);
			float y = Mathf.Abs(transform.position.z - pos.z);
			score += Mathf.Sqrt(x * x + y * y) * (cheat()? 100f : 1f);
			scoreText.text = "SCORE: " + Mathf.Round (score) + " / " + scoreToNextLevel;
			pos = transform.position;
			if (score > scoreToNextLevel)
			{
				Levels.next(); // In the collection: was LoadLevel(loadedLevel + 1), or Quit after the last
				enabled = false;
			}
		}
	}
}
