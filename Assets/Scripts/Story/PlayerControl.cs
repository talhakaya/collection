using System.Collections.Generic;
using UnityEngine;

namespace Collection.Story
{
    // Who has the character at the moment, other than the player walking it about.
    //
    // Several things take the character over for a while: the roll, a fall (Ragdoll), a bicycle, a cutscene that
    // carries it (the land coming up), the character creator. Each used to switch the character's own movement
    // and its CharacterController off and back on by itself, and whichever finished first switched them on under
    // the others. Now each says so here (Take) and says when it is done (Release), and the character walks only
    // while nobody has it.
    //
    // Taking it is of the movement always (PlayerMovement), and of the body too unless asked otherwise: the
    // CharacterController, which is the character's collider and what dialogue triggers notice. The roll keeps
    // the body, moving it itself.
    //
    // It need not be put on the character by hand: Of gives the character's, adding one if there is none.
    [RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
    public class PlayerControl : MonoBehaviour
    {
        readonly List<Object> holders = new List<Object>();
        readonly List<bool> bodies = new List<bool>();
        CharacterController body;
        PlayerMovement walking;

        // Found when first needed, not in Awake: this may have been added to a character that is switched off.
        CharacterController controller => body != null ? body : body = GetComponent<CharacterController>();
        PlayerMovement movement => walking != null ? walking : walking = GetComponent<PlayerMovement>();

        // Nobody has it: the player is walking it.
        public bool Free => holders.Count == 0;

        // How high the character's own place is above the ground it stands on (m).
        public float Standing => controller.height * 0.5f - controller.center.y + controller.skinWidth;

        public static PlayerControl Of(Component player)
        {
            return player.TryGetComponent(out PlayerControl control) ? control : player.gameObject.AddComponent<PlayerControl>();
        }

        // `withBody`: the CharacterController is switched off too. Taking it again changes only that.
        public void Take(Object who, bool withBody = true)
        {
            int at = holders.IndexOf(who);
            if (at < 0)
            {
                holders.Add(who);
                bodies.Add(withBody);
            }
            else
            {
                bodies[at] = withBody;
            }
            Apply();
        }

        public void Release(Object who)
        {
            int at = holders.IndexOf(who);
            if (at < 0)
                return;
            holders.RemoveAt(at);
            bodies.RemoveAt(at);
            Apply();
        }

        // Puts the character somewhere. (A CharacterController that is on does not let its object be moved.)
        public void MoveTo(Vector3 place, Quaternion facing)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(place, facing);
            controller.enabled = !BodyTaken();
        }

        public void MoveTo(Vector3 place)
        {
            MoveTo(place, transform.rotation);
        }

        bool BodyTaken()
        {
            foreach (bool taken in bodies)
                if (taken)
                    return true;
            return false;
        }

        void Apply()
        {
            movement.enabled = holders.Count == 0;
            controller.enabled = !BodyTaken();
        }

        // Something that had the character and is gone (destroyed with it in hand) has let go.
        void Update()
        {
            bool gone = false;
            for (int i = holders.Count - 1; i >= 0; i--)
            {
                if (holders[i] != null)
                    continue;
                holders.RemoveAt(i);
                bodies.RemoveAt(i);
                gone = true;
            }
            if (gone)
                Apply();
        }
    }
}
