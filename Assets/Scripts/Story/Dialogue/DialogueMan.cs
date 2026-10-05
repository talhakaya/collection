using System;
using System.Collections;
using System.Collections.Generic;
using Collection.Controls;
using TMPro;
using UnityEngine;

namespace Collection.Story
{
    // Conversations: knows which NPCTriggers the player is standing in, shows the prompt over the nearest one,
    // starts its conversation on Interact, and keeps the player still while one is going.
    //
    // After the user's own DialogueMan, with only the NPCTrigger part of it.
    public class DialogueMan : MonoBehaviour
    {
        [Header("Yarn")]
        public DialogueUI dialogueUIMain;

        [Header("Prompt over what can be talked to")]
        public CanvasGroup uiPrompt;
        public TextMeshProUGUI textPrompt;
        [Tooltip("The button shown before the prompt's words.")]
        public string promptButton = "{<Keyboard>/e|<Gamepad>/buttonSouth}";

        public static Action OnDialogueStarted;
        public static Action OnDialogueComplete;

        readonly List<NPCTrigger> currentNpcTriggers = new List<NPCTrigger>();
        Transform player;
        string promptShown;
        float canInteractAt;

        void OnEnable()
        {
            NPCTrigger.OnNPCTriggerEnter += OnNPCTriggerEnter;
            NPCTrigger.OnNPCTriggerExit += OnNPCTriggerExit;
            InputPrompts.Changed += OnPromptsChanged;
        }

        void OnDisable()
        {
            NPCTrigger.OnNPCTriggerEnter -= OnNPCTriggerEnter;
            NPCTrigger.OnNPCTriggerExit -= OnNPCTriggerExit;
            InputPrompts.Changed -= OnPromptsChanged;
        }

        void Start()
        {
            player = FindFirstObjectByType<PlayerMovement>().transform;

            // <<cutscene name>> in the Yarn script: see Cutscene.
            dialogueUIMain.dialogueRunner.AddCommandHandler("cutscene", new Func<string, IEnumerator>(RunCutscene));
            textPrompt.spriteAsset = InputPrompts.SpriteAsset(true);
            uiPrompt.gameObject.SetActive(false);
        }

        void OnPromptsChanged()
        {
            promptShown = null;
        }

        void OnNPCTriggerEnter(NPCTrigger trigger)
        {
            if (!currentNpcTriggers.Contains(trigger))
                currentNpcTriggers.Add(trigger);
        }

        void OnNPCTriggerExit(NPCTrigger trigger)
        {
            currentNpcTriggers.Remove(trigger);
        }

        IEnumerator RunCutscene(string id)
        {
            Cutscene cutscene = Cutscene.Find(id);
            if (cutscene == null)
            {
                Debug.LogError($"The Yarn script asks for <<cutscene {id}>>, but no Cutscene in the scene has that id.");
                yield break;
            }

            dialogueUIMain.HideLines();
            cutscene.onStart?.Invoke();
            if (cutscene.duration > 0f)
                yield return new WaitForSeconds(cutscene.duration);
            cutscene.onEnd?.Invoke();
        }

        public bool IsTalking()
        {
            return dialogueUIMain.isActive;
        }

        void LateUpdate()
        {
            bool talking = IsTalking();
            Main.inst.input.talking = talking;
            if (talking)
            {
                // A moment after the last line before anything can be talked to again, so the press that ended
                // the conversation does not start it over.
                canInteractAt = Time.unscaledTime + 0.3f;
                uiPrompt.gameObject.SetActive(false);
                return;
            }

            NPCTrigger trigger = GetMostSuitableNPCTrigger();
            if (trigger == null || TaloketoInputManager.Blocked)
            {
                uiPrompt.gameObject.SetActive(false);
                return;
            }

            uiPrompt.gameObject.SetActive(true);
            uiPrompt.alpha = trigger.ShouldShowPromptWithLowAlpha() ? 0.45f : 1f;
            Transform at = trigger.uiPromptParent != null ? trigger.uiPromptParent : trigger.transform;
            uiPrompt.transform.position = Camera.main.WorldToScreenPoint(at.position);

            string prompt = InputPrompts.Format(promptButton + " " + trigger.prompt, out bool _);
            if (prompt != promptShown)
            {
                promptShown = prompt;
                textPrompt.text = prompt;
            }

            if (Time.unscaledTime >= canInteractAt && Main.inst.input.interactPressed)
                HandleInteractRequest(trigger);
        }

        void HandleInteractRequest(NPCTrigger trigger)
        {
            if (string.IsNullOrEmpty(trigger.conversation))
                trigger.onActivate?.Invoke();
            else
                StartDialogue(trigger);
        }

        // The nearest of the triggers the player is standing in.
        NPCTrigger GetMostSuitableNPCTrigger()
        {
            NPCTrigger mostSuitable = null;
            float nearest = float.MaxValue;
            for (int i = currentNpcTriggers.Count - 1; i >= 0; i--)
            {
                NPCTrigger trigger = currentNpcTriggers[i];
                if (trigger == null || !trigger.isActiveAndEnabled)
                {
                    currentNpcTriggers.RemoveAt(i);
                    continue;
                }
                // One that starts by itself on walking in has no prompt.
                if (trigger.playOnTrigger)
                    continue;
                float distance = (trigger.transform.position - player.position).sqrMagnitude;
                if (distance < nearest)
                {
                    nearest = distance;
                    mostSuitable = trigger;
                }
            }
            return mostSuitable;
        }

        public void StartDialogue(NPCTrigger npcTrigger)
        {
            DialogueUI dialogueUI = dialogueUIMain;
            if (dialogueUI.isActive)
                return;

            void ActuallyStartDialogue()
            {
                dialogueUI.currentNpcTrigger = npcTrigger;
                dialogueUI.dialogueRunner.StartDialogue(npcTrigger.conversation);
                npcTrigger.OnDialogueStarted();
            }

            IEnumerator Do()
            {
                yield return new WaitForSeconds(npcTrigger.waitAtStart);
                ActuallyStartDialogue();
            }

            // Counts as talking from now, so nothing else starts during the wait.
            dialogueUI.isActive = true;
            if (npcTrigger.waitAtStart > 0f)
                StartCoroutine(Do());
            else
                ActuallyStartDialogue();
        }
    }
}
