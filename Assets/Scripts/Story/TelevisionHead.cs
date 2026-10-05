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
    //
    // Near the ground. The head is far bigger than a head, and when the body is down low (rolling, lying after a
    // fall) it would be half in the ground. So it gets smaller the nearer the head bone is to the ground under the
    // character, down to `smallest` of its size, and grows back as the character stands up.
    [ExecuteAlways]
    [DefaultExecutionOrder(200)]
    [RequireComponent(typeof(MorphSphere))]
    public class TelevisionHead : MonoBehaviour
    {
        // From the middle of the sphere to a side of the cube inside it, for a sphere of radius 1.
        const float CubeHalf = 0.57735027f;

        [Tooltip("Height of the head's lowest point above the head bone (m).")]
        public float neckGap = 0.02f;

        [Header("Smaller near the ground")]
        [Tooltip("The character's body, for where the ground under it is. Empty: the head never changes size.")]
        public CharacterController body;
        [Tooltip("The size it shrinks to, as a part of its own.")]
        [Range(0.1f, 1f)] public float smallest = 0.3f;
        [Tooltip("Head bone heights above the ground (m): at the first and below it is at its smallest, at the second and above its full size.")]
        public Vector2 shrinkHeights = new Vector2(0.45f, 1.15f);
        [Tooltip("How quickly it changes size. Higher is quicker.")]
        public float shrinkSpeed = 14f;

        MorphSphere sphere;
        Vector3 fullScale;
        float size = 1f;

        void OnEnable()
        {
            sphere = GetComponent<MorphSphere>();
            fullScale = transform.localScale;
            size = 1f;
            Sit();
        }

        void OnDisable()
        {
            if (Application.isPlaying)
                transform.localScale = fullScale;
        }

        void Start()
        {
            if (Application.isPlaying)
                Load();
        }

        // After everything that poses the body for the frame (the Animator, the roll, getting up).
        void LateUpdate()
        {
            if (Application.isPlaying && body != null && transform.parent != null)
            {
                float ground = body.transform.position.y + body.center.y - body.height * 0.5f;
                float height = transform.parent.position.y - ground;
                float wanted = Mathf.Lerp(smallest, 1f, Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(shrinkHeights.x, shrinkHeights.y, height)));
                size = Mathf.Lerp(size, wanted, 1f - Mathf.Exp(-shrinkSpeed * Time.deltaTime));
                transform.localScale = fullScale * size;
            }
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
