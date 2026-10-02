using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager instance;
	public static float oldZoomFactor = 1f;
	
	private enum State
	{
		NotStarted,
		Started,
		NodeAppearing,
		NodeAppeared,
		NodeFinished,
		Finished
	}
	private State state;
	private float textPeriod;
	private float textPeriodCount;
	private string textTemp;
	private float numberTemp;
	private const float ALPHA = 0.85f;
	private Renderer meshRenderer;
	private static float zoomSpeed = 0.2f;
	
	public int dialogueTreeId;
	public DialogueNode currentNode;
	public List<TextTalha> texts;
	
	void Awake ()
	{
		instance = this;
	}
	
	void Start ()
	{
		foreach (Transform child in transform)
		{
			if (child.gameObject.name == "DialogueBoard")
			{
				meshRenderer = child.gameObject.renderer;
				break;
			}
		}
		gameObject.name = "DialogueBox";
		transform.parent = CameraScript.instance.transform;
		transform.localPosition = new Vector3(0f, -20f, 2f);
		
		meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0f);
		state = State.NotStarted;
		textPeriod = 0.03f;
		textPeriodCount = 0f;
		CameraScript.zoomInOut(1.5f, zoomSpeed);
	}
	
	void Update ()
	{
		if (state == State.NotStarted)
		{
			if (meshRenderer.material.color.a < ALPHA)
			{
				meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, meshRenderer.material.color.a + 1.5f * Time.deltaTime);
			}
			else
			{
				state++;
			}
		}
		else if (state == State.Started)
		{
			currentNode = DialogueCollection.getDialogueTree(dialogueTreeId);
			readNode(currentNode);
			state++;
			texts[2].size = 1.2f;
			texts[3].size = 1.2f;
			texts[4].size = 1.2f;
		}
		else if (state == State.NodeAppearing)
		{
			textPeriodCount += Time.deltaTime;
			if (textPeriodCount >= textPeriod)
			{
				textPeriodCount = 0f;
				if (textTemp.Length > texts[1].text.Length)
				{
					texts[1].text = textTemp.Substring(0, texts[1].text.Length + 1);
				}
			}
			
			if (Input.GetButtonDown("Fire"))
			{
				texts[1].text = textTemp;
			}
			
			
			if (texts[1].text == textTemp)
			{
				state++;
				numberTemp = 0f;
			}
		}
		else if (state == State.NodeAppeared)
		{
			if (currentNode.type == DialogueNodeType.QuestionNodeWithAnswers)
			{
				if (numberTemp < 1f)
				{
					numberTemp += Time.deltaTime;
				}
				else if (numberTemp > 1f)
				{
					numberTemp = 1f;
				}
				else
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Vector3 mousePosition = ray.origin + (ray.direction * 10f);
					texts[2].size = 1.2f;
					texts[3].size = 1.2f;
					texts[4].size = 1.2f;
					TextTalha temp = null;
					int whichAnswer = -1;
					if (mousePosition.y < transform.position.y + 1.25f && mousePosition.y > transform.position.y - 6.25f)
					{
						if (mousePosition.y < transform.position.y - 3.75f)
						{
							temp = texts[4];
							whichAnswer = 2;
						}
						else if (mousePosition.y < transform.position.y - 1.25f)
						{
							temp = texts[3];
							whichAnswer = 1;
						}
						else
						{
							temp = texts[2];
							whichAnswer = 0;
						}
					}
					
					if (temp != null)
					{
						temp.size = 1.4f;
						
						if (Input.GetButtonUp("Fire"))
						{
							if (currentNode.children.Count > whichAnswer)
							{
								currentNode = DialogueCollection.getDialogueNode(currentNode.children[whichAnswer]);
								for (int i = 2; i < 5; i++)
								{
									texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0f);
								}
								state++;
							}
						}
					}
				}
				if (numberTemp <= ALPHA)
				{
					for (int i = 2; i < 5; i++)
					{
						texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, numberTemp);
					}
				}
			}
			
			if (currentNode.type == DialogueNodeType.QuestionNodeWithQuestion)
			{
				if (Input.GetButtonDown("Fire"))
				{
					state++;
				}
			}
		}
		else if (state == State.NodeFinished)
		{
			DialogueCollection.specialEffect(currentNode.id);
			if (currentNode.children.Count > 0)
			{
				currentNode = DialogueCollection.getDialogueNode(currentNode.children[0]);
				readNode(currentNode);
				state = State.NodeAppearing;
			}
			else
			{
				readNode(null);
				state++;
			}
		}
		else if (state == State.Finished)
		{
			if (meshRenderer.material.color.a > 0f)
			{
				meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, meshRenderer.material.color.a - 1.5f * Time.deltaTime);
			}
			else
			{
				CameraScript.zoomInOut(oldZoomFactor, zoomSpeed * 2f);
				Destroy(gameObject);
			}
		}
	}
	
	private void readNode(DialogueNode node)
	{
		foreach (TextTalha item in texts)
		{
			item.text = "";
		}
		
		if (node != null)
		{
			texts[0].text = DialogueCollection.getCharacterName(node.character);
			texts[1].text = "";
			textTemp = node.text;
			if (node.type == DialogueNodeType.QuestionNodeWithAnswers)
			{
				for (int i = 0; i < node.children.Count; i++)
				{
					texts[2 + i].color = new Color(0.5f, 0.5f, 1f, 0f);
					texts[2 + i].text = DialogueCollection.getDialogueNode (node.children[i]).text;
				}
			}
		}
	}
	
	public static void createNewDialogue(int id)
	{
		if (instance == null)
		{
			if (CameraScript.instance.cameraTk2d.ZoomFactor != 1f)
			{
				DialoguePreparer prep = CameraScript.instance.gameObject.AddComponent<DialoguePreparer>();
				prep.dialogueTreeId = id;
				if (CameraScript.instance.zoomFactor != 0)
				{
					oldZoomFactor = CameraScript.instance.zoomFactor;
				}
				else
				{
					oldZoomFactor = CameraScript.instance.cameraTk2d.ZoomFactor;
				}
				CameraScript.zoomInOut(1f, zoomSpeed);
			}
			else
			{
				GameObject box = Resources.Load ("DialogueBox") as GameObject;
				box = Instantiate(box) as GameObject;
				DialogueManager dialogue = box.GetComponent<DialogueManager>();
				dialogue.dialogueTreeId = id;
			}
		}
	}
}