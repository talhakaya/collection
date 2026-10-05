using System;
using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace Collection.Story
{
    // Something the player can talk to or set off: one object that owns one conversation (a node of the Yarn
    // script) and the events around it. Yarn only supplies the lines and the options; everything that happens in
    // the scene hangs off the events here and on the NPCLineTriggers next to this component.
    //
    // It starts when the player presses Interact inside its trigger collider, or by itself on entering the collider
    // (playOnTrigger) or when the scene starts (playOnAwake). With no conversation it just fires onActivate.
    //
    // After the user's own NPCTrigger, without the parts this game has no use for yet (voice clips and subtitle
    // timing, the background channel, lights, cameras).
    public class NPCTrigger : MonoBehaviour
    {
        public string id;
        [Tooltip("Where the prompt is shown, usually above the head.")]
        public Transform uiPromptParent;
        [Tooltip("What the prompt says after the button, e.g. Talk.")]
        public string prompt = "Talk";
        public float waitAtStart;
        public bool playOnAwake;
        public bool playOnTrigger;
        [Tooltip("Happens once in a save slot, then this object is switched off for good.")]
        public bool onlyOnce;
        public bool disableOnConversationEnd;
        [Tooltip("The Yarn node to run.")]
        public string conversation;
        [Tooltip("The nodes to run on the next times, in order; the last one repeats.")]
        public List<string> nextConversations = new List<string>();
        public UnityEvent onActivate;
        public UnityEvent onConversationStart;
        public UnityEvent onConversationEnd;

        public static Action<NPCTrigger> OnNPCTriggerEnter;
        public static Action<NPCTrigger> OnNPCTriggerExit;

        int conversationEndCount;
        bool allConversationsHad;
        int lineIndex;
        NPCLineTrigger[] lineTriggers;

        void OnEnable()
        {
            lineTriggers = GetComponents<NPCLineTrigger>();
        }

        void Start()
        {
            if (onlyOnce && HasHappened())
            {
                gameObject.SetActive(false);
                return;
            }
            if (playOnAwake)
                TryStartDialogue();
        }

        void OnDisable()
        {
            OnNPCTriggerExit?.Invoke(this);
        }

        bool HasHappened()
        {
            return SaveManager.Slot.story.conversations.Contains(conversation);
        }

        public void TryStartDialogue()
        {
            if (!onlyOnce || !HasHappened())
                Main.inst.dialogue.StartDialogue(this);
        }

        public void OnDialogueStarted()
        {
            onConversationStart?.Invoke();
            if (!playOnTrigger)
                onActivate?.Invoke();
            lineIndex = 0;
        }

        public void OnDialogueComplete()
        {
            onConversationEnd?.Invoke();
            if (onlyOnce)
            {
                if (!HasHappened())
                {
                    SaveManager.Slot.story.conversations.Add(conversation);
                    SaveManager.MarkDirty();
                }
                gameObject.SetActive(false);
                return;
            }
            if (disableOnConversationEnd)
            {
                gameObject.SetActive(false);
                return;
            }
            if (nextConversations.Count > 0)
            {
                int next = Mathf.Clamp(conversationEndCount, 0, nextConversations.Count - 1);
                conversation = nextConversations[next];
            }
            conversationEndCount++;
            allConversationsHad = conversationEndCount > nextConversations.Count;
        }

        void OnTriggerEnter(Collider other)
        {
            DialogueActor actor = other.GetComponent<DialogueActor>();
            if (actor == null || !actor.canUse)
                return;
            OnNPCTriggerEnter?.Invoke(this);
            if (playOnTrigger)
            {
                if (!string.IsNullOrEmpty(conversation))
                    Main.inst.dialogue.StartDialogue(this);
                onActivate?.Invoke();
            }
        }

        void OnTriggerExit(Collider other)
        {
            DialogueActor actor = other.GetComponent<DialogueActor>();
            if (actor == null || !actor.canUse)
                return;
            OnNPCTriggerExit?.Invoke(this);
        }

        // Everything there is to say has been said at least once: the prompt is shown faintly.
        public bool ShouldShowPromptWithLowAlpha()
        {
            return allConversationsHad;
        }

        // Lines and options are counted together, from 0, in the order they come up.

        public void OnLineStart()
        {
            foreach (NPCLineTrigger trigger in lineTriggers)
                if (trigger.lineIndex == lineIndex)
                    trigger.OnLineStart();
        }

        public void OnLineEnd()
        {
            foreach (NPCLineTrigger trigger in lineTriggers)
                if (trigger.lineIndex == lineIndex)
                    trigger.OnLineEnd();
            lineIndex++;
        }

        public void OnOptionSelected(int optionIndex)
        {
            foreach (NPCLineTrigger trigger in lineTriggers)
                if (trigger.lineIndex == lineIndex)
                    trigger.OnOptionSelected(optionIndex);
            lineIndex++;
        }

        // For an event in the Inspector (an option's, say): keeps something in the save slot, written as
        // "name=value". StoryMemory.Get("name") reads it back later.
        public void Remember(string nameAndValue)
        {
            int at = nameAndValue.IndexOf('=');
            if (at <= 0)
            {
                Debug.LogError($"NPCTrigger.Remember wants \"name=value\", got \"{nameAndValue}\".", this);
                return;
            }
            StoryMemory.Set(nameAndValue.Substring(0, at).Trim(), nameAndValue.Substring(at + 1).Trim());
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0.6f, 0f, 0.2f);
            Gizmos.matrix = transform.localToWorldMatrix;
            if (TryGetComponent(out Collider col))
            {
                switch (col)
                {
                    case BoxCollider box:
                        Gizmos.DrawCube(box.center, box.size);
                        break;
                    case SphereCollider sphere:
                        Gizmos.DrawSphere(sphere.center, sphere.radius);
                        break;
                }
            }
        }
    }
}
