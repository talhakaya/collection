using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Collection.Story
{
    // The story's input: the "Story" map of the collection's input actions (Resources/Input/CollectionInput).
    // Every binding works at the same time, so keyboard and gamepad can be used interchangeably without switching.
    // Nothing is read while one of the collection's menus is up (the pause screen).
    public class InputMan : MonoBehaviour
    {
        const string AssetResourcePath = "Input/CollectionInput";
        const string MapName = "Story";

        [Tooltip("A toggled sprint (gamepad L3) switches off when the move input drops below this.")]
        public float sprintToggleStopThreshold = 0.2f;

        public Vector2 move => Blocked ? Vector2.zero : moveAction.ReadValue<Vector2>();
        // Hold Left Shift, RB or RT, or click the left stick (L3) to toggle sprint on until you stop moving.
        public bool sprint => !Blocked && (sprintAction.IsPressed() || sprintToggled);
        // -1..1: turns the character in the character creator (gamepad right stick; the mouse drags instead).
        public float rotate => Blocked ? 0f : rotateAction.ReadValue<float>();

        static bool Blocked => TaloketoInputManager.Blocked;

        InputActionMap map;
        InputAction moveAction;
        InputAction sprintAction;
        InputAction sprintToggleAction;
        InputAction rotateAction;
        bool sprintToggled;

        void Awake()
        {
            InputActionAsset asset = Resources.Load<InputActionAsset>(AssetResourcePath);
            map = asset != null ? asset.FindActionMap(MapName) : null;
            if (map == null)
            {
                Debug.LogError($"InputMan: no '{MapName}' action map in Resources/{AssetResourcePath}.");
                enabled = false;
                return;
            }

            moveAction = map.FindAction("Move");
            sprintAction = map.FindAction("Sprint");
            sprintToggleAction = map.FindAction("SprintToggle");
            rotateAction = map.FindAction("Rotate");
        }

        void OnEnable()
        {
            map?.Enable();
        }

        void OnDisable()
        {
            map?.Disable();
        }

        void Update()
        {
            if (Blocked)
                return;
            if (sprintToggleAction.WasPressedThisFrame())
                sprintToggled = !sprintToggled;
            if (move.magnitude < sprintToggleStopThreshold)
                sprintToggled = false;
        }
    }
}
