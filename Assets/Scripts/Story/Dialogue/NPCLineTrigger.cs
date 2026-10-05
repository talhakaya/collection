using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Collection.Story
{
    // Events for one line of an NPCTrigger's conversation, put next to it: lineIndex counts the lines and the
    // options of the conversation together, from 0, in the order they come up. For a set of options,
    // onOptionSelected has one event for each option, in the order they are written.
    public class NPCLineTrigger : MonoBehaviour
    {
        public int lineIndex;
        public UnityEvent onLineStart;
        public UnityEvent onLineEnd;
        public List<UnityEvent> onOptionSelected = new List<UnityEvent>();

        public void OnLineStart()
        {
            onLineStart?.Invoke();
        }

        public void OnLineEnd()
        {
            onLineEnd?.Invoke();
        }

        public void OnOptionSelected(int optionIndex)
        {
            if (optionIndex >= onOptionSelected.Count)
                return;
            onOptionSelected[optionIndex]?.Invoke();
        }
    }
}
