using UnityEngine;
using System.Collections;

public class RoommateScript : BasicDialogueCharacter {

	/*public int dialogueId;
	public bool repeatDialogue;
	public float sizeOfShadow;
	public float locationOfShadow;*/
	
	void Start ()
	{
		ShadowScript.createShadow(transform, Vector2.one * sizeOfShadow, Vector2.up * (-1) * locationOfShadow);
		if (GameManagerScript.gameSaveState == GameSaveState.PlayedDay1)
		{
			dialogueId = 11;
		}
		DialogueStarter.createDialogueStarter(transform, dialogueId, new Vector2(10, 3), repeatDialogue);
	}
	
	void Update ()
	{
		
	}
}
