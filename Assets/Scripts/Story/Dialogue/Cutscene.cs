using UnityEngine;
using UnityEngine.Events;

namespace Collection.Story
{
    // Something a conversation stops for: the Yarn script says <<cutscene name>> on a line of its own, the dialogue's
    // text goes away, onStart fires, `duration` seconds pass, onEnd fires, and the conversation carries on with its
    // next line. With a duration of zero it is just a place in the script where something happens (a camera
    // switched on, say).
    //
    // One that is `untilFinished` has no set length: it goes on until whatever it started calls Finish (a
    // minigame played to its end, a screen answered).
    //
    // Unlike an NPCLineTrigger, which counts lines, this is found by name, so it stays in the right place whichever
    // way the conversation branched before it.
    public class Cutscene : MonoBehaviour
    {
        [Tooltip("The name after <<cutscene in the Yarn script.")]
        public string id;
        [Tooltip("Seconds the conversation waits.")]
        public float duration;
        [Tooltip("Ticked: it goes on until something calls Finish, and `duration` is not used.")]
        public bool untilFinished;
        public UnityEvent onStart;
        public UnityEvent onEnd;

        // The one the conversation is stopped for at the moment (or was last).
        public static Cutscene Running { get; private set; }

        public bool Finished { get; private set; }

        // For DialogueMan, as it starts.
        public void Begin()
        {
            Finished = false;
            Running = this;
        }

        public void Finish()
        {
            Finished = true;
        }

        // For whatever a cutscene started, which need not know which cutscene that was.
        public static void FinishRunning()
        {
            if (Running != null)
                Running.Finish();
        }

        public static Cutscene Find(string id)
        {
            foreach (Cutscene cutscene in FindObjectsByType<Cutscene>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (cutscene.id == id)
                    return cutscene;
            return null;
        }
    }
}
