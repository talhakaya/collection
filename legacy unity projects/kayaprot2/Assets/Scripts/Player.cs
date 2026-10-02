using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
    public static Player instance;
    public static Person person;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public bool interactInput;
    [HideInInspector] public bool fireInput;
    [HideInInspector] public bool fire2Input;
    [HideInInspector] public float weaponSelect;
    [SerializeField] private GameObject dialogueBubble;
    private float timerDialogueBubble;

    void Awake () {
        person = GetComponent<Person>();
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
	
	void Update () {
        if (timerDialogueBubble > 0f && person.state == Person.State.Alive && !UiManager.instance.uiDialogue.isActive) {
            timerDialogueBubble -= Time.deltaTime;
            dialogueBubble.SetActive(true);
        }
        else {
            dialogueBubble.SetActive(false);
        }

        if (person.state == Person.State.Dead) return;
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        interactInput = Input.GetButton("Interact");
        fireInput = Input.GetButton("Fire");
        fire2Input = Input.GetButtonDown("Fire2");
        weaponSelect = Input.GetAxisRaw("WeaponSelect");
        int newWeaponId = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            newWeaponId = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            newWeaponId = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            newWeaponId = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            newWeaponId = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            newWeaponId = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            newWeaponId = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            newWeaponId = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            newWeaponId = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            newWeaponId = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            newWeaponId = 9;
        }

        if (!UiManager.instance.CanPlayerMove()) {
            person.SetInput(0f, 0f, false);
        }
        else {
            person.SetInput(horizontalInput, verticalInput, interactInput, fireInput, fire2Input, weaponSelect, newWeaponId, Game.instance.mouse);
        }
    }

    public void GetShot(RaycastHit2D hit) {

    }

    public void SetDialogueBubbleActive() {
        timerDialogueBubble = 0.1f;
    }
}
