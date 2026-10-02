using UnityEngine;
using System.Collections;

public class DialogueStarter : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load("DialogueStarter") as GameObject;
	public int dialogueId;
	public Vector2 scale;
	public bool repeatDialogue;
	
	private DialogueStarterState dialogueStarterState;
	
	
	private enum DialogueStarterState
	{
		NotSpawned,
		Spawned,
		Ended
	}
	
	public static GameObject createDialogueStarter(Transform parent, int _dialogueId, Vector2 _colliderScale, bool _repeatDialogue)
	{
		GameObject g = Instantiate(prefab, parent.position, parent.rotation) as GameObject;
		g.transform.parent = parent;
		g.name = "DialogueStarter";
		DialogueStarter gDialogueStarter = g.GetComponent<DialogueStarter>();
		gDialogueStarter.scale = _colliderScale;
		gDialogueStarter.repeatDialogue = _repeatDialogue;
		gDialogueStarter.dialogueId = _dialogueId;
		
		return g;
	}
	
	void Start ()
	{
		dialogueStarterState = DialogueStarterState.NotSpawned;
		transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
	}
	
	void Update ()
	{
		
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.name == "Player")
		{
			if (dialogueStarterState == DialogueStarterState.NotSpawned)
			{
				dialogueStarterState = DialogueStarterState.Spawned;
				DialogueManager.createNewDialogue(dialogueId);
				DialogueCollection.parameterDialogueCharacter = transform.parent;
			}
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.name == "Player")
		{
			if (repeatDialogue && dialogueStarterState == DialogueStarterState.Spawned)
			{
				dialogueStarterState = DialogueStarterState.NotSpawned;
			}
			else
			{
				dialogueStarterState = DialogueStarterState.Ended;
				Destroy(gameObject);
			}
		}
	}
	
	public static void FindDialogueStarterAndChangeDialogueId(Transform parent, int id)
	{
		DialogueStarter starter = null;
		foreach (Transform child in parent)
		{
			starter = child.GetComponent<DialogueStarter>();
			if (starter != null)
			{
				break;
			}
		}
		if (starter != null)
		{
			starter.dialogueId = id;
		}
	}
}
