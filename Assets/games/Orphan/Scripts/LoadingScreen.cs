using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class LoadingScreen : MonoBehaviour
	{
		private TextTalha text;
		private float textAlpha;

		public bool ImDonePleaseKillMe;
		public string levelName;

		void Awake ()
		{
			Collection.Controls.PortHelpers.KeepWithinGame(transform.gameObject); // In the collection: was DontDestroyOnLoad
		}

		void Start ()
		{
			ImDonePleaseKillMe = false;
			transform.localScale = transform.localScale / CameraScript.instance.cameraTk2d.ZoomFactor;
			GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0f);
			GameObject textTalha = TextTalha.create(transform.position - Vector3.forward, "Loading", 1, 10, Color.white, 5, 0.1f, new Vector2(0.10f, -0.10f), true);
			textTalha.transform.parent = transform;
			text = textTalha.GetComponent<TextTalha>();
			textAlpha = text.color.a;
		}

		void Update ()
		{
			if (!ImDonePleaseKillMe)
			{
				if (GetComponent<Renderer>().material.color.a < 1f)
				{
					GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, GetComponent<Renderer>().material.color.a + 0.5f * Time.deltaTime);
					text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha * GetComponent<Renderer>().material.color.a - 0.5f);
				}
				else
				{
					transform.parent = null;
					UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Orphan/Scenes/" + levelName + ".unity"); // In the collection: was Application.LoadLevel(levelName)
					ImDonePleaseKillMe = true;
				}
			}
			else
			{
				transform.position = CameraScript.instance.transform.position + Vector3.forward;

				if (GetComponent<Renderer>().material.color.a > 0f)
				{
					GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, GetComponent<Renderer>().material.color.a - 0.5f * Time.deltaTime);
				}
				else
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
