using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Orphan
{
	public enum DialogueNodeType
	{
		QuestionNodeWithAnswers,
		AnswerNode,
		QuestionNodeWithQuestion,
	}

	public enum DialogueCharacter
	{
		None,
		Player,
		OldMan,
		Roommate,
		Nurse,
		Principal,
		QuestionMark,
		Mother,
		Father,
		Smoker,
		Ghost
	}

	public class DialogueNode
	{
		public int id;
		public DialogueNodeType type;
		public string text;
		public List<int> children;
		public DialogueCharacter character;

		public DialogueNode(int _id, DialogueNodeType _type, string _text, List<int> _children, DialogueCharacter _character)
		{
			id = _id;
			type = _type;
			text = _text;
			children = _children;
			character = _character;
		}

		public override string ToString()
		{
			string r = "id:" + id + " type: " + type + " " + text + " " + character;
			r = r + "(";
			for (int i = 0; i < children.Count; i++)
			{
				r = r + " " + children[i];  
			}
			r = r + " )";
			return r;
		}
	}
}
