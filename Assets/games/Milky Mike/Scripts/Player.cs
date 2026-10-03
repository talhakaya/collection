using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.MilkyMike
{
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

	        horizontalInput = TaloketoInputManager.GetAxisRaw("Horizontal");
	        verticalInput = TaloketoInputManager.GetAxisRaw("Vertical");
	        interactInput = TaloketoInputManager.GetButton("Interact");
	        fireInput = TaloketoInputManager.GetButton("Fire");
	        fire2Input = TaloketoInputManager.GetButtonDown("Fire2");
	        weaponSelect = TaloketoInputManager.GetAxisRaw("WeaponSelect");
	        int newWeaponId = -1;
	        if (Game.keyDown(UnityEngine.InputSystem.Key.Digit1)) {
	            newWeaponId = 0;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit2)) {
	            newWeaponId = 1;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit3)) {
	            newWeaponId = 2;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit4)) {
	            newWeaponId = 3;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit5)) {
	            newWeaponId = 4;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit6)) {
	            newWeaponId = 5;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit7)) {
	            newWeaponId = 6;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit8)) {
	            newWeaponId = 7;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit9)) {
	            newWeaponId = 8;
	        }
	        else if (Game.keyDown(UnityEngine.InputSystem.Key.Digit0)) {
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
}
