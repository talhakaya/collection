using UnityEngine;
using System.Collections;

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
		if (Input.GetKeyDown(KeyCode.G))
		{
			CameraScript.instance.glitchEffect = !CameraScript.instance.glitchEffect;
		}
		if (!finished)
		{
			if (!started)
			{
				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					started = true;
				}
			}
			else
			{
				if (renderer.material.color.a > 0f)
				{
					renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a - Time.deltaTime * 2f);
				}
				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					finished = true;
				}
			}
		}
		else
		{
			if (renderer.material.color.a < 1f)
			{
				renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a + Time.deltaTime * 2f);
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					GameManagerScript.changeLevel("Prologue");
				}
			}
		}
	}
}
