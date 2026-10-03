using UnityEngine;
using System.Collections;

namespace Games.OverEating101
{
	public class TextTalha : MonoBehaviour {

		public string text;
		public TextMesh main;
		public TextMesh shadow;
		public TextMesh shadow2;
		public float minScale;

		// Use this for initialization
		void Start () {
			if (minScale == 0f)
			{
				minScale = 0.3f;
			}
		}

		// Update is called once per frame
		void Update () {
			main.text = text;
			shadow.text = text;
			shadow2.text = text;

			float ratio0 = (Game.time % 2f) / 2f;
			if (ratio0 >= 0.5f)
			{
				ratio0 = 1f - ratio0;
			}
			float ratio1 = (Game.time % 3f) / 3f;
			if (ratio1 >= 0.5f)
			{
				ratio1 = 1f - ratio1;
			}

			transform.localScale = new Vector3 (minScale + ratio0 * 0.1f, minScale + ratio1 * 0.1f, 1f);
		}
	}
}
