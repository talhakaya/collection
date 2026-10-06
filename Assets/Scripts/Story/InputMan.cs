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

        // While a conversation is up (talking, set by DialogueMan) the character stands still: the same keys and
        // stick choose between dialogue options instead (navigate).
        [HideInInspector] public bool talking;
        // The same for something that is not a conversation: a chest being opened, an artifact about to speak.
        // The character is not steered, but is still its own to stand on the ground (unlike one that has been
        // taken over: PlayerControl). Each Hold is ended by a LetGo; while any is on, nothing is read.
        public bool held => holds > 0;
        int holds;

        public void Hold()
        {
            holds++;
        }

        public void LetGo()
        {
            holds = Mathf.Max(0, holds - 1);
        }

        public Vector2 move => Blocked || talking || held ? Vector2.zero : moveAction.ReadValue<Vector2>();
        public Vector2 navigate => Blocked ? Vector2.zero : moveAction.ReadValue<Vector2>();
        // Hold Left Shift, RB or RT, or click the left stick (L3) to toggle sprint on until you stop moving.
        public bool sprint => !Blocked && !talking && !held && (sprintAction.IsPressed() || sprintToggled);
        // E, Enter or the pad's bottom button: talks to someone, moves a conversation on, picks an option.
        public bool interactPressed => !Blocked && interactAction.WasPressedThisFrame();
        // Space or the pad's right button: the dodge roll.
        public bool rollPressed => !Blocked && !talking && !held && rollAction.WasPressedThisFrame();
        // Anything at all that counts as wanting to get going again (for getting up after a fall).
        public bool anyPressed => !Blocked && !talking && !held && (moveAction.ReadValue<Vector2>().magnitude > 0.3f
            || interactAction.WasPressedThisFrame() || rollAction.WasPressedThisFrame());
        // -1..1: turns the character in the character creator (gamepad right stick; the mouse drags instead).
        public float rotate => Blocked ? 0f : rotateAction.ReadValue<float>();

        static bool Blocked => TaloketoInputManager.Blocked;

        InputActionMap map;
        InputAction moveAction;
        InputAction sprintAction;
        InputAction sprintToggleAction;
        InputAction rotateAction;
        InputAction interactAction;
        InputAction rollAction;
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
            interactAction = map.FindAction("Interact");
            rollAction = map.FindAction("Roll");
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
