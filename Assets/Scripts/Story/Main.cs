using UnityEngine;

namespace Collection.Story
{
    // Entry point that holds references to the managers living on the same "Main" GameObject.
    // Runs first so Main.inst is set before any other script's Awake.
    [DefaultExecutionOrder(-1000)]
    public class Main : MonoBehaviour
    {
        public static Main inst { get; private set; }

        // "new" because Component has a deprecated built-in "camera" property with the same name.
        public new CameraMan camera;
        public InputMan input;
        public CharacterMan character;
        public DialogueMan dialogue;

        void Awake()
        {
            inst = this;
        }
    }
}
