using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.MilkyMike
{
	public enum Tool {
	    None,
	    Bug,
	    Drone,
	    Jetpack,
	    Teleporter,
	    Pistol,
	    Uzi,
	    JumpPanel
	}

	public class Level : MonoBehaviour {
	    //camera
	    private static bool isLatestGroundYSet;
	    private static float latestGroundY;
	    private Vector3 camPos;
	    private Vector3 mousePos;
	    private Vector3 kickBackPos;
	    private Vector3 lookDownPos;
	    private float camOrtographicSize;
	    private const float MaxMouseVecLength = 3f;

	    public Tool[] tools;
	    public int moneyCollected;
	    public int moneyToFinish;
	    public PrefabCreator[] guardPrefabs;
	    public PrefabCreator[] doorPrefabs;
	    public GameObject[] enableToFinish;
	    public GameObject[] disableToFinish;
	    public Transform start;
	    public PrefabCreator end;
	    [HideInInspector] public FinishTrigger finish;
	    [HideInInspector] public Guard[] guards;
	    [HideInInspector] public Door[] doors;
	    private bool inited;
	    [HideInInspector] public bool isStealthy;
	    [HideInInspector] public float time;
	    public float timeLimit;
	    public bool hasChallenge;
	    [HideInInspector] public List<MoneyCollect> moneyCollects;
	    public NPC npcTalkAutomatically;
	    private const float CAM_ORTO_NPC = 2f;
	    private const float CAM_ORTO_ACTION = 10f;
	    public float milkChangeAtTheEnd;

	    void Start() {
	        if (!inited) {
	            inited = true;
	            Init();
	        }
	    }

	    void Init() {
	        isLatestGroundYSet = false;
	        mousePos = new Vector3(0f, 0f, 0f);
	        kickBackPos = new Vector3(0f, 0f, 0f);
	        lookDownPos = new Vector3(0f, 0f, 0f);
	        finish = end.objectCreated.GetComponent<FinishTrigger>();
	        guards = new Guard[guardPrefabs.Length];
	        for (int i = 0, len = guardPrefabs.Length; i < len; i++) {
	            guards[i] = guardPrefabs[i].objectCreated.GetComponent<Guard>();
	        }
	        doors = new Door[doorPrefabs.Length];
	        for (int i = 0, len = doorPrefabs.Length; i < len; i++) {
	            doors[i] = doorPrefabs[i].objectCreated.GetComponent<Door>();
	        }
	    }

	    public void ResetLevel() {
	        if (!inited) {
	            inited = true;
	            Init();
	        }
	        Player.instance.transform.position = start.position;
	        Player.person.tools = Game.instance.allTools;
	        Player.person.ResetState();
	        Player.person.tools = new GameObject[tools.Length];
	        for (int i = 0, len = tools.Length; i < len; i++) {
	            Player.person.tools[i] = Game.instance.allTools[(int)tools[i]];
	        }
	        Player.person.weaponUi.ResetState();
	        finish.level = this;
	        finish.ResetState();
	        moneyCollected = 0;
	        for (int i = 0, len = guards.Length; i < len; i++) {
	            guards[i].ResetState();
	            guards[i].person.ResetState();
	            guards[i].transform.localPosition = new Vector3(0f, 0f, 0f);
	        }
	        for (int i = 0, len = doors.Length; i < len; i++) {
	            doors[i].ResetState();
	        }
	        isStealthy = true;
	        time = 0f;
	        UiManager.instance.ResetState();
	        if (npcTalkAutomatically == null) {
	            Camera.main.transform.position = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Camera.main.transform.position.z);
	            Camera.main.orthographicSize = CAM_ORTO_ACTION;
	        }
	        else {
	            Vector3 npcPos = npcTalkAutomatically.transform.TransformPoint(npcTalkAutomatically.cameraOffset);
	            Camera.main.transform.position = new Vector3(npcPos.x, npcPos.y, Camera.main.transform.position.z);
	            Camera.main.orthographicSize = CAM_ORTO_NPC;
	            npcTalkAutomatically.Talk();
	        }
	        for (int i = 0, len = moneyCollects.Count; i < len; i++) {
	            moneyCollects[i].gameObject.SetActive(true);
	        }
	        camPos = Camera.main.transform.position;
	        camOrtographicSize = CAM_ORTO_ACTION;
	    }

		void Update () {
	        UpdateCamera();
	        if (UiManager.instance.CanPlayerMove()) {
	            time += Time.deltaTime;
	        }
	    }

	    public bool CanFinish() {
	        if (!inited) return false;
	        if (moneyCollected < moneyToFinish) return false;
	        foreach (GameObject g in enableToFinish) {
	            if (!g.activeSelf) return false;
	        }
	        foreach (GameObject g in disableToFinish) {
	            if (g.activeSelf) return false;
	        }
	        foreach (Guard g in guards) {
	            if (g.person.state == Person.State.Alive) return false;
	        }
	        return true;
	    }

	    void UpdateCamera() {
	        if (UiManager.instance.uiDialogue.isActive && UiManager.instance.uiDialogue.shouldZoom) {
	            Camera.main.orthographicSize += (CAM_ORTO_NPC - Camera.main.orthographicSize) * Game.dt * 4f;
	            Vector3 deltaPos = (UiManager.instance.uiDialogue.GetCameraPos() - Camera.main.transform.position) * Game.dt * 10f;
	            deltaPos.z = 0f;
	            Camera.main.transform.position += deltaPos;
	        }
	        else {
	            bool isPerson = Player.person.camAt.GetComponent<Person>() != null;
	            bool lookingDown = false;
	            if (isPerson) {
	                Person person = Player.person.camAt.GetComponent<Person>();
	                float yGoal = Player.person.camAt.position.y;
	                if (isLatestGroundYSet) {
	                    yGoal = Mathf.Min(Player.person.camAt.position.y, latestGroundY) + Camera.main.orthographicSize - 2f;
	                    if (yGoal < Player.person.camAt.position.y) {
	                        yGoal = Player.person.camAt.position.y;
	                    }
	                }
	                camPos += new Vector3(Mathf.Clamp(Player.person.camAt.position.x - camPos.x, -person.speedH, person.speedH) * 2f, (yGoal - camPos.y), 0f) * Game.dt * 4f;

	                lookingDown = Player.instance.verticalInput < 0f;
	            }
	            else {
	                camPos += new Vector3(Player.person.camAt.position.x - camPos.x, Player.person.camAt.position.y - camPos.y, 0f) * Game.dt * 4f;
	            }

	            camOrtographicSize = Mathf.Clamp(camOrtographicSize + TaloketoInputManager.GetAxis("Mouse ScrollWheel") * 5f, 2f, 15f);
	            Camera.main.orthographicSize += (camOrtographicSize - Camera.main.orthographicSize) * Game.dt * 4f;

	            Vector3 newMousePos = new Vector3(0f, 0f, 0f);
	            if (!UiManager.instance.uiDialogue.isActive && Player.person.camAt == Player.instance.transform && !lookingDown && (Player.instance.fireInput || Player.person.IsUsingGun())) {
	                newMousePos = MousePosition.get - Player.instance.transform.position;
	                if (Geometry.lengthOfVector3(newMousePos) > MaxMouseVecLength) {
	                    newMousePos = Geometry.normalizeVector3(newMousePos, MaxMouseVecLength);
	                }
	            }
	            mousePos += (newMousePos - mousePos) * Game.dt * 4f;
	            mousePos.z = 0f;

	            Vector3 newKickBackPos = new Vector3(0f, 0f, 0f);
	            if (Player.person.IsUsingGun()) {
	                Gun gun = Player.person.GetGun();
	                if (gun.kickBackTimer > 0f) {
	                    gun.kickBackTimer -= Game.dt;
	                    newKickBackPos = Geometry.createVector3(gun.kickBackAngle, 15f);
	                }
	            }
	            kickBackPos += (newKickBackPos - kickBackPos) * Game.dt * 4f;

	            if (!UiManager.instance.uiDialogue.isActive && lookingDown) {
	                lookDownPos += (new Vector3(0f, -1f * Camera.main.orthographicSize, 0f) - lookDownPos) * Game.dt * 4f;
	            }
	            else {
	                lookDownPos += (new Vector3(0f, 0f, 0f) - lookDownPos) * Game.dt * 4f;
	            }

	            Camera.main.transform.position = camPos + mousePos + lookDownPos + kickBackPos;
	        }
	    }

	    public static void SetLatestGroundY(float y) {
	        if (isLatestGroundYSet) {
	            if (y - latestGroundY > 4f || y - latestGroundY < 0f) {
	                latestGroundY = y;
	            }
	        }
	        else {
	            isLatestGroundYSet = true;
	            latestGroundY = y;
	        }
	    }

	    public bool HasAliveGuard() {
	        foreach (Guard g in guards) {
	            if (g.person.state == Person.State.Alive) return true;
	        }
	        return false;
	    }

	}
}
