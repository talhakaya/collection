using UnityEngine;
using System.Collections;

public class NurseScript : BasicDialogueCharacter {

	
	void Start ()
	{
		ShadowScript.createShadow(transform, Vector2.one * sizeOfShadow, Vector2.up * (-1) * locationOfShadow);
		DialogueStarter.createDialogueStarter(transform, dialogueId, new Vector2(10, 3), repeatDialogue);
	}
	
	void Update ()
	{
		
	}
}
