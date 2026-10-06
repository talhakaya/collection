using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Collection.Story
{
    // In the collection this is a development tool, not part of the game (see CharacterMan), and the head is not
    // among its settings: the head is the television.
    //
    // Character creation screen. The player is lifted to a "studio" high above the world (drawn alone on black) and
    // framed on the right of the screen, while the settings are listed on the left as "<  Hair: Mohawk  >" rows.
    // Dragging with the mouse (outside the panel) or the gamepad's right stick turns the character. Confirm puts the
    // player back and starts play.
    public class CharacterCreator : MonoBehaviour
    {
        [Tooltip("Row cloned for each setting (kept inactive).")]
        public CreatorRow rowTemplate;
        public Button confirmButton;
        [Tooltip("Shown while the creator is open.")]
        public GameObject panel;
        [Tooltip("Cinemachine camera framing the character; placed when the creator opens.")]
        public GameObject studioCamera;
        public float studioHeight = 1000f;
        [Tooltip("Studio camera position relative to the character's feet, in the character's space (x = the character's " +
                 "right, z = in front). A sideways offset puts the character to one side of the screen.")]
        public Vector3 cameraOffset = new Vector3(1.2f, 1.1f, 4.2f);
        [Tooltip("Turning speed (degrees/s) with the gamepad's right stick.")]
        public float rotateSpeed = 150f;
        [Tooltip("Degrees turned per pixel of mouse drag.")]
        public float dragSensitivity = 0.4f;

        CharacterAppearance appearance;
        Transform player;
        PlayerMovement movement;
        Vector3 returnPosition;
        Action onConfirm;
        bool open;

        public bool isOpen => open || opening;
        bool opening;
        bool dragging;
        GameObject lastSelected;
        readonly List<CreatorRow> rows = new List<CreatorRow>();

        void Awake()
        {
            rowTemplate.gameObject.SetActive(false);
            panel.SetActive(false);
            studioCamera.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        public void Open(CharacterAppearance target, Action confirmed)
        {
            appearance = target;
            onConfirm = confirmed;
            opening = true;
            StartCoroutine(OpenWhenReady());
        }

        IEnumerator OpenWhenReady()
        {
            movement = appearance.GetComponentInParent<PlayerMovement>(true);
            player = movement.transform;
            while (!player.gameObject.activeInHierarchy)
                yield return null;

            PlayerControl.Of(movement).Take(this);
            returnPosition = player.position;
            MovePlayer(new Vector3(returnPosition.x, studioHeight, returnPosition.z));

            // Face the camera and put the character on the right of the screen.
            Quaternion facing = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
            Vector3 feet = appearance.transform.position;
            studioCamera.transform.SetPositionAndRotation(feet + facing * cameraOffset, facing * Quaternion.Euler(0f, 180f, 0f));
            Main.inst.camera.EnterStudio(studioCamera);

            BuildRows();
            panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(rows[0].gameObject);
            open = true;
            opening = false;
        }

        void Update()
        {
            if (!open)
                return;

            KeepFocus();

            // Turning follows the direction of the drag or stick, as if pushing the character's front sideways:
            // right turns the front toward the right of the screen.
            float yaw = 0f;

            float stick = Main.inst.input.rotate;
            if (Mathf.Abs(stick) > 0.1f)
                yaw -= stick * rotateSpeed * Time.deltaTime;

            // Mouse: drag with the left button, starting anywhere outside the settings panel.
            var mouse = Mouse.current;
            if (mouse != null)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                    dragging = !EventSystem.current.IsPointerOverGameObject();
                else if (!mouse.leftButton.isPressed)
                    dragging = false;
                if (dragging)
                    yaw -= mouse.delta.ReadValue().x * dragSensitivity;
            }

            if (yaw != 0f)
                player.Rotate(0f, yaw, 0f, Space.World);
        }

        // Something in the panel is always selected, so a gamepad or keyboard can navigate at any time, even after a
        // mouse click elsewhere (e.g. clicking the Game view to focus it).
        void KeepFocus()
        {
            var eventSystem = EventSystem.current;
            GameObject selected = eventSystem.currentSelectedGameObject;
            if (selected != null && selected.activeInHierarchy && selected.transform.IsChildOf(panel.transform))
            {
                lastSelected = selected;
                return;
            }
            eventSystem.SetSelectedGameObject(lastSelected != null && lastSelected.activeInHierarchy ? lastSelected : rows[0].gameObject);
        }

        void Confirm()
        {
            if (!open)
                return;
            open = false;
            panel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            MovePlayer(returnPosition);
            PlayerControl.Of(movement).Release(this);
            Main.inst.camera.ExitStudio(studioCamera);
            onConfirm?.Invoke();
        }

        // Moves the player and tells Cinemachine, so following cameras jump along instead of swooping.
        void MovePlayer(Vector3 position)
        {
            Vector3 delta = position - player.position;
            player.position = position;
            CinemachineCore.OnTargetObjectWarped(player, delta);
        }

        // ------------------------------------------------------------------ rows

        void BuildRows()
        {
            foreach (var row in rows)
                Destroy(row.gameObject);
            rows.Clear();

            var a = appearance;
            var c = a.catalog;
            var skin = c.skinColors.Select(o => o.name).ToList();
            var cloth = c.clothColors.Select(o => o.name).ToList();

            AddFloat("Height", () => a.height, v => a.height = v, 0.75f, 1.3f, 0.02f);
            AddFloat("Fatness", () => a.fatness, v => a.fatness = v, 0.7f, 1.6f, 0.02f);
            AddChoice("Skin", () => a.skinColor, v => a.skinColor = v, skin, i => c.Skin(i));
            AddChoice("Top", () => a.topType, v => a.topType = v, Names(c.tops));
            AddChoice("Top Colour", () => a.shirtColor, v => a.shirtColor = v, cloth, i => c.Cloth(i));
            AddChoice("Jacket", () => a.jacketType, v => a.jacketType = v, Names(c.jackets));
            AddChoice("Jacket Colour", () => a.jacketColor, v => a.jacketColor = v, cloth, i => c.Cloth(i));
            AddChoice("Bottom", () => a.bottomType, v => a.bottomType = v, Names(c.bottoms));
            AddChoice("Pants Colour", () => a.pantsColor, v => a.pantsColor = v, cloth, i => c.Cloth(i));
            AddChoice("Shoes Colour", () => a.shoesColor, v => a.shoesColor = v, cloth, i => c.Cloth(i));

            // Up/down goes through the rows and on to the confirm button.
            for (int i = 0; i < rows.Count; i++)
            {
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnUp = i > 0 ? rows[i - 1] : null;
                nav.selectOnDown = i < rows.Count - 1 ? (Selectable)rows[i + 1] : confirmButton;
                rows[i].navigation = nav;
            }
            confirmButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = rows[rows.Count - 1] };
        }

        static List<string> Names(List<CharacterCatalog.ClothingOption> options) => options.Select(o => o.name).ToList();

        // A list choice; left/right cycle through the options (wrapping around).
        void AddChoice(string name, Func<int> get, Action<int> set, List<string> options, Func<int, Color> color = null)
        {
            AddRow(() => $"{name}: {(options.Count > 0 ? options[Mathf.Clamp(get(), 0, options.Count - 1)] : "-")}",
                color == null ? null : (Func<Color?>)(() => color(get())),
                direction =>
                {
                    int n = options.Count;
                    if (n == 0)
                        return;
                    set(((get() + direction) % n + n) % n);
                    appearance.Apply();
                });
        }

        // A number; left/right step it within its range.
        void AddFloat(string name, Func<float> get, Action<float> set, float min, float max, float step)
        {
            AddRow(() => $"{name}: {get():0.00}", null, direction =>
            {
                set(Mathf.Clamp(Mathf.Round((get() + direction * step) / step) * step, min, max));
                appearance.Apply();
            });
        }

        void AddRow(Func<string> describe, Func<Color?> swatchColor, Action<int> change)
        {
            var row = Instantiate(rowTemplate, rowTemplate.transform.parent);
            row.transform.SetSiblingIndex(rowTemplate.transform.GetSiblingIndex() + rows.Count + 1);
            row.gameObject.SetActive(true);
            row.Bind(describe, swatchColor, change);
            rows.Add(row);
        }
    }
}
