using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class BasicDialogueCharacter : MonoBehaviour {

		public int dialogueId;
		public bool repeatDialogue;
		public float sizeOfShadow;
		public float locationOfShadow;

		void Start ()
		{
			ShadowScript.createShadow(transform, Vector2.one * sizeOfShadow, Vector2.up * (-1) * locationOfShadow);
			DialogueStarter.createDialogueStarter(transform, dialogueId, new Vector2(10, 3), repeatDialogue);
		}
	}
}
