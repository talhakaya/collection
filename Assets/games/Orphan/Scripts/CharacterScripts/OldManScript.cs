using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class OldManScript : MonoBehaviour {

		public int dialogueId;
		public bool repeatDialogue;

		void Start ()
		{
			ShadowScript.createShadow(transform, Vector2.one * 7f, Vector2.up * (-2));
			DialogueStarter.createDialogueStarter(transform, dialogueId, new Vector2(10, 3), repeatDialogue);
		}

		void Update ()
		{

		}
	}
}
