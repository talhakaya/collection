using UnityEngine;
using UnityEngine.Events;

namespace Collection.Story
{
    // Something a conversation stops for: the Yarn script says <<cutscene name>> on a line of its own, the dialogue's
    // text goes away, onStart fires, `duration` seconds pass, onEnd fires, and the conversation carries on with its
    // next line. With a duration of zero it is just a place in the script where something happens (a camera
    // switched on, say).
    //
    // Unlike an NPCLineTrigger, which counts lines, this is found by name, so it stays in the right place whichever
    // way the conversation branched before it.
    public class Cutscene : MonoBehaviour
    {
        [Tooltip("The name after <<cutscene in the Yarn script.")]
        public string id;
        [Tooltip("Seconds the conversation waits.")]
        public float duration;
        public UnityEvent onStart;
        public UnityEvent onEnd;

        public static Cutscene Find(string id)
        {
            foreach (Cutscene cutscene in FindObjectsByType<Cutscene>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (cutscene.id == id)
                    return cutscene;
            return null;
        }
    }
}
