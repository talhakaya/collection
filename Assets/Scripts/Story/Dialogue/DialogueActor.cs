using System.Collections.Generic;
using UnityEngine;

namespace Collection.Story
{
    // Someone who can be in a conversation. canUse marks the one who sets NPCTriggers off by walking into them and
    // pressing Interact: the player. actorNames are the names this one speaks under in the Yarn script.
    public class DialogueActor : MonoBehaviour
    {
        public bool canUse;
        public List<string> actorNames = new List<string>();

        public bool IsActorName(string actorName)
        {
            return actorNames.Contains(actorName);
        }
    }
}
