using UnityEngine;
using System.Collections;

public class DialoguePreparer : MonoBehaviour {
	
	public static DialoguePreparer instance;
	public int dialogueTreeId;
	
	void Start ()
	{
		instance = this;
	}
	
	void Update ()
	{
		if (CameraScript.instance.cameraTk2d.ZoomFactor == 1f)
		{
			DialogueManager.createNewDialogue(dialogueTreeId);
			Destroy(this);
		}
	}
}
