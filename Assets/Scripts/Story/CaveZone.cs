using UnityEngine;

namespace Collection.Story
{
    // Trigger volume covering a cave interior. Tells CameraMan when the player enters or leaves.
    [RequireComponent(typeof(BoxCollider))]
    public class CaveZone : MonoBehaviour
    {
        void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerMovement>() != null)
                Main.inst.camera.EnterCave();
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerMovement>() != null)
                Main.inst.camera.ExitCave();
        }
    }
}
