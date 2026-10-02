using UnityEngine;
using System.Collections;

public class ArcadeGameStarter : MonoBehaviour {

	public static GameObject prefab;
	public GameArcadeKind arcadeId;
	public Vector2 scale;
	public bool repeatGame;
	
	private ArcadeGameStarterState arcadeGameStarterState;
	
	
	private enum ArcadeGameStarterState
	{
		NotSpawned,
		Spawned,
		Ended
	}
	
	public static GameObject createArcadeGameStarter(Transform parent, GameArcadeKind _arcadeId, Vector2 _colliderScale, bool _repeatGame)
	{
		GameObject g = Instantiate(prefab, parent.position, parent.rotation) as GameObject;
		g.transform.parent = parent;
		g.name = "DialogueStarter";
		ArcadeGameStarter gArcadeGameStarter = g.GetComponent<ArcadeGameStarter>();
		gArcadeGameStarter.scale = _colliderScale;
		gArcadeGameStarter.repeatGame = _repeatGame;
		gArcadeGameStarter.arcadeId = _arcadeId;
		
		return g;
	}
	
	void Start ()
	{
		prefab = Resources.Load("ArcadeGameStarter") as GameObject;
		arcadeGameStarterState = ArcadeGameStarterState.NotSpawned;
		transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
	}
	
	void Update ()
	{
		
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if (arcadeGameStarterState == ArcadeGameStarterState.NotSpawned)
		{
			arcadeGameStarterState = ArcadeGameStarterState.Spawned;
			ArcadeGameManager.createNewArcadeGame(arcadeId);
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if (repeatGame && arcadeGameStarterState == ArcadeGameStarterState.Spawned)
		{
			arcadeGameStarterState = ArcadeGameStarterState.NotSpawned;
		}
		else
		{
			arcadeGameStarterState = ArcadeGameStarterState.Ended;
			Destroy(gameObject);
		}
	}
}
