using UnityEngine;

namespace Collection.Story
{
    // A volume covering a cave's inside. Tells CameraMan when the character goes in and comes out.
    //
    // It looks at where the character is each frame, not at the physics engine's reports of entering and leaving:
    // those come from the character's own collider, which is switched off while the character is a ragdoll or is
    // riding something, and a cave gone into or out of in that state was never noticed (the view then stayed the
    // cave's, dark, out in the open).
    [RequireComponent(typeof(BoxCollider))]
    public class CaveZone : MonoBehaviour
    {
        BoxCollider box;
        bool inside;

        void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        void Awake()
        {
            box = GetComponent<BoxCollider>();
        }

        void Update()
        {
            PlayerMovement movement = PlayerMovement.Current;
            if (movement == null)
                return;
            Transform player = movement.transform;

            // In the box's own space, where it is a plain box about its centre.
            Vector3 at = transform.InverseTransformPoint(player.position) - box.center;
            Vector3 half = box.size * 0.5f;
            bool now = Mathf.Abs(at.x) <= half.x && Mathf.Abs(at.y) <= half.y && Mathf.Abs(at.z) <= half.z;
            if (now == inside)
                return;
            inside = now;
            if (inside)
                Main.inst.camera.EnterCave();
            else
                Main.inst.camera.ExitCave();
        }

        void OnDisable()
        {
            if (inside && Main.inst != null && Main.inst.camera != null)
                Main.inst.camera.ExitCave();
            inside = false;
        }
    }
}
