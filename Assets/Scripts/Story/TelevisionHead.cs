using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // The main character's head: the morphing sphere, sat on the head bone, with its shape kept in the save.
    //
    // Sitting. The head rests on the neck by its lowest point, and how far that is below the middle depends on the
    // bottom side: a flat cube side is nearer the middle than the bottom of the sphere. So the height above the head
    // bone follows the bottom side's value, and the head neither sinks into the shoulders nor floats as it changes.
    //
    // Saving. The six side values are part of the story save (SaveManager.Slot.story.headSides). They are read when
    // the scene starts, so a loaded game has the head it was saved with; a new game has the sphere. Whatever changes
    // the head in play calls Save() when the change should be kept. Sliders moved by hand while playing are not
    // saved unless that is called (the component's context menu has it).
    [ExecuteAlways]
    [RequireComponent(typeof(MorphSphere))]
    public class TelevisionHead : MonoBehaviour
    {
        // From the middle of the sphere to a side of the cube inside it, for a sphere of radius 1.
        const float CubeHalf = 0.57735027f;

        [Tooltip("Height of the head's lowest point above the head bone (m).")]
        public float neckGap = 0.02f;

        MorphSphere sphere;

        void OnEnable()
        {
            sphere = GetComponent<MorphSphere>();
            Sit();
        }

        void Start()
        {
            if (Application.isPlaying)
                Load();
        }

        void LateUpdate()
        {
            Sit();
        }

        // Puts the head's lowest point on the neck.
        void Sit()
        {
            if (sphere == null)
                return;
            float reach = Mathf.Lerp(CubeHalf, 1f, sphere.down) * transform.localScale.y;
            Vector3 position = transform.localPosition;
            position.y = neckGap + reach;
            transform.localPosition = position;
        }

        // The head as it is in the slot being played.
        public void Load()
        {
            List<float> saved = SaveManager.Slot.story.headSides;
            if (saved == null || saved.Count != 6)
                return;
            for (int side = 0; side < 6; side++)
                sphere[side] = Mathf.Clamp01(saved[side]);
            Sit();
        }

        // Keeps the head as it is now in the slot being played.
        [ContextMenu("Save Head To Slot")]
        public void Save()
        {
            if (!Application.isPlaying)
                return;
            List<float> saved = SaveManager.Slot.story.headSides;
            saved.Clear();
            for (int side = 0; side < 6; side++)
                saved.Add(sphere[side]);
            SaveManager.MarkDirty();
        }
    }
}
