using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

namespace Collection.Story
{
    // What Yarn's lines and options are shown with: a name, a line, and a list of options. A line stays until
    // Interact is pressed; an option is chosen with up and down and Interact, or with the mouse.
    //
    // It tells the conversation's NPCTrigger when each line starts and ends and which option was picked, which is
    // where anything that should happen in the scene is hooked up.
    //
    // After the user's own DialogueUI, for Yarn Spinner 3 (a presenter with async methods, where version 2 had a
    // view with callbacks).
    public class DialogueUI : DialoguePresenterBase
    {
        public bool shouldLog;
        public CanvasGroup cg;
        public DialogueRunner dialogueRunner;
        public TextMeshProUGUI textName;
        public TextMeshProUGUI textLine;
        public List<TextMeshProUGUI> textOptions;
        public GameObject lineMenu;
        public GameObject optionsMenu;
        [Tooltip("Shown on the line's panel once the line can be moved on.")]
        public GameObject continueHint;

        [Tooltip("Space between two options (in the canvas's units).")]
        public float optionGap = 10f;

        public Color optionColour = new Color(1f, 1f, 1f, 0.08f);
        public Color optionSelectedColour = new Color(1f, 1f, 1f, 0.95f);
        public Color optionTextColour = new Color(0.92f, 0.92f, 0.92f);
        public Color optionSelectedTextColour = new Color(0.05f, 0.05f, 0.05f);

        [HideInInspector] public bool isActive;
        [HideInInspector] public NPCTrigger currentNpcTrigger;

        // A press does not count for this long after a line or the options come up (real seconds), so the press
        // that brought them up does not also dismiss them.
        const float InputDelay = 0.3f;

        int selectedOption;
        int verticalHeld;

        void Awake()
        {
            lineMenu.SetActive(false);
            optionsMenu.SetActive(false);
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            if (shouldLog) Debug.Log($"{name} DialogueStarted", this);
            isActive = true;
            textName.text = "";
            textLine.text = "";
            DialogueMan.OnDialogueStarted?.Invoke();
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            if (shouldLog) Debug.Log($"{name} DialogueComplete", this);
            lineMenu.SetActive(false);
            optionsMenu.SetActive(false);
            isActive = false;
            NPCTrigger trigger = currentNpcTrigger;
            currentNpcTrigger = null;
            if (trigger != null)
                trigger.OnDialogueComplete();
            DialogueMan.OnDialogueComplete?.Invoke();
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (shouldLog) Debug.Log($"{name} RunLine {line.TextID}", this);
            if (currentNpcTrigger != null)
                currentNpcTrigger.OnLineStart();

            optionsMenu.SetActive(false);
            lineMenu.SetActive(true);
            textName.text = line.CharacterName ?? "";
            string text = line.TextWithoutCharacterName.Text;
            text = text.Replace("<br>", "\n");
            text = text.Replace("<c>", ":");
            textLine.text = text;
            if (continueHint != null)
                continueHint.SetActive(false);

            float ready = Time.unscaledTime + InputDelay;
            while (this != null && !token.NextContentToken.IsCancellationRequested)
            {
                await YarnTask.Yield();
                if (Time.unscaledTime < ready)
                    continue;
                if (continueHint != null && !continueHint.activeSelf)
                    continueHint.SetActive(true);
                if (Main.inst.input.interactPressed || Clicked())
                    break;
            }

            if (this == null)
                return;
            if (continueHint != null)
                continueHint.SetActive(false);
            if (currentNpcTrigger != null)
                currentNpcTrigger.OnLineEnd();
        }

        public override async YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken token)
        {
            if (shouldLog) Debug.Log($"{name} RunOptions {dialogueOptions.Length}", this);
            if (continueHint != null)
                continueHint.SetActive(false);
            optionsMenu.SetActive(true);
            int count = Mathf.Min(dialogueOptions.Length, textOptions.Count);
            if (dialogueOptions.Length > textOptions.Count)
                Debug.LogWarning($"{name}: {dialogueOptions.Length} options, but only {textOptions.Count} places to show them.", this);
            for (int i = 0; i < textOptions.Count; i++)
            {
                var row = (RectTransform)textOptions[i].transform.parent;
                row.gameObject.SetActive(i < count);
                if (i >= count)
                    continue;
                textOptions[i].text = dialogueOptions[i].Line.TextWithoutCharacterName.Text;

                // The rows are stacked up from the bottom of the menu, so that however many options there are,
                // the last one sits just above the line.
                Vector2 position = row.anchoredPosition;
                position.y = (count - 1 - i) * (row.sizeDelta.y + optionGap);
                row.anchoredPosition = position;
            }

            selectedOption = 0;
            verticalHeld = Vertical();
            ShowSelection(count);

            int picked = -1;
            float ready = Time.unscaledTime + InputDelay;
            while (this != null && picked < 0 && !token.NextContentToken.IsCancellationRequested)
            {
                await YarnTask.Yield();

                // Up and down, one step for each push.
                int vertical = Vertical();
                if (vertical != verticalHeld)
                {
                    verticalHeld = vertical;
                    if (vertical != 0)
                        selectedOption = ((selectedOption - vertical) % count + count) % count;
                }

                // The mouse: the option under it is the selected one, and a click picks it.
                int under = OptionUnderMouse(count);
                if (under >= 0 && MouseMoved())
                    selectedOption = under;

                ShowSelection(count);
                if (Time.unscaledTime < ready)
                    continue;
                if (Main.inst.input.interactPressed)
                    picked = selectedOption;
                else if (under >= 0 && Clicked())
                    picked = under;
            }

            if (this == null || picked < 0)
                return null;

            optionsMenu.SetActive(false);
            if (currentNpcTrigger != null)
                currentNpcTrigger.OnOptionSelected(picked);
            return dialogueOptions[picked];
        }

        int Vertical()
        {
            float y = Main.inst.input.navigate.y;
            return y > 0.5f ? 1 : y < -0.5f ? -1 : 0;
        }

        void ShowSelection(int count)
        {
            for (int i = 0; i < count; i++)
            {
                bool selected = i == selectedOption;
                textOptions[i].color = selected ? optionSelectedTextColour : optionTextColour;
                if (textOptions[i].transform.parent.TryGetComponent(out Image back))
                    back.color = selected ? optionSelectedColour : optionColour;
            }
        }

        int OptionUnderMouse(int count)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return -1;
            Vector2 position = mouse.position.ReadValue();
            for (int i = 0; i < count; i++)
            {
                var row = (RectTransform)textOptions[i].transform.parent;
                if (RectTransformUtility.RectangleContainsScreenPoint(row, position, null))
                    return i;
            }
            return -1;
        }

        static bool MouseMoved()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.delta.ReadValue() != Vector2.zero;
        }

        static bool Clicked()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && !Collection.Controls.TaloketoInputManager.Blocked && mouse.leftButton.wasPressedThisFrame;
        }
    }
}
