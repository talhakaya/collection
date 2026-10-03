using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class TitleScreenManager : MonoBehaviour {

		bool finished;
		bool started;
		public GameObject texts;

		void Start ()
		{
			finished = false;
		}

		void Update ()
		{
			if (Collection.Controls.PortHelpers.KeyDown(UnityEngine.InputSystem.Key.G))
			{
				CameraScript.instance.glitchEffect = !CameraScript.instance.glitchEffect;
			}
			if (!finished)
			{
				if (!started)
				{
					if (Collection.Controls.TaloketoInputManager.GetButtonDown("Start"))
					{
						started = true;
					}
				}
				else
				{
					if (GetComponent<Renderer>().material.color.a > 0f)
					{
						GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a - Time.deltaTime * 2f);
					}
					if (Collection.Controls.TaloketoInputManager.GetButtonDown("Start"))
					{
						finished = true;
					}
				}
			}
			else
			{
				if (GetComponent<Renderer>().material.color.a < 1f)
				{
					GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a + Time.deltaTime * 2f);
				}
				else
				{
					if (Collection.Controls.TaloketoInputManager.GetButtonDown("Start"))
					{
						GameManagerScript.changeLevel("Prologue");
					}
				}
			}
		}
	}
}
