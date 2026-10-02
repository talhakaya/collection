using UnityEngine;
using System.Collections;

public class ArcadeFinishingHandler : MonoBehaviour {
	
	public int effectId;
	public ArcadeGameManager arcadeGame;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (arcadeGame != null)
		{
			if (arcadeGame.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Won
				|| arcadeGame.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Lost
				|| arcadeGame.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Tie)
			{
				Effect(effectId, arcadeGame);
			}
		}
	}
	
	public static void Effect(int id, ArcadeGameManager game)
	{
		switch (id)
		{
			case 1: // Day 1 nurse medicine won
				if (game.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Won)
				{
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(DialogueCollection.parameterDialogueCharacter, 8);
				}
				else if (game.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Lost)
				{
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(DialogueCollection.parameterDialogueCharacter, 14);
				}
				
				break;
			/*
			case :
				
				break;*/
			default:
				break;
		}
	}
	
	public static void Create(int id, ArcadeGameManager game)
	{
		GameObject finishingHandler = new GameObject("ArcadeFinishingHandler");
		Instantiate(finishingHandler);
		ArcadeFinishingHandler script = finishingHandler.AddComponent<ArcadeFinishingHandler>();
		script.arcadeGame = game;
		script.effectId = id;
	}
}
