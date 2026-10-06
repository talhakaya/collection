using System.Collections;
using Collection.Saving;
using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // Coming to the grassy field, the first time: out of the box the desert ended in, and out of the sky.
    //
    //   The character falls from `height` metres as a ragdoll, turning over as it comes, lands, and lies
    //   there. It cannot get up.
    //
    //   The fall is not followed down by the camera, which from that height would show the whole level and its
    //   edges. The view waits where the character will land, from further off than the game's own (`wide`
    //   times as far), and the character falls into it. While the character lies there the view comes in to
    //   the game's own, taking exactly the `liesFor` seconds to do it.
    //
    //   After those `liesFor` seconds someone comes over from the top right of the picture (`helper`, a character of
    //   the level kept switched off until now), stops by the character and speaks (the conversation of
    //   `helperTrigger`).
    //
    //   After that the character can get up, as after any fall: on the next thing the player presses. When it
    //   does, the helper walks off the way it came and is gone.
    //
    // It happens once in a save slot: the helper's NPCTrigger is onlyOnce, and with that conversation had, the
    // level starts as any other does. Left in the middle, it starts over from the fall.
    //
    // Starts after StoryDirector, which has by then put the character where the slot has it; the fall is from
    // above that place.
    public class FieldArrival : MonoBehaviour
    {
        public Transform player;
        [Tooltip("Who comes over: a character of the level, with an Animator (the Locomotion controller).")]
        public Animator helper;
        [Tooltip("What the helper says, onlyOnce. On an object of its own under the helper: an NPCTrigger that is onlyOnce switches its object off once it has been had.")]
        public NPCTrigger helperTrigger;

        [Tooltip("How high the fall is from (m).")]
        public float height = 50f;
        [Tooltip("How far it is tipped over as the fall starts, at most (degrees), and how fast it turns on the way down (radians/s, about).")]
        public float tipped = 35f;
        public float turns = 1.3f;
        [Tooltip("How much further off than the game's own camera the view of the landing is.")]
        public float wide = 1.5f;
        [Tooltip("Seconds lying on the ground before the helper sets out: the view comes in to the game's own in that time.")]
        public float liesFor = 10f;
        [Tooltip("How far off the helper starts (m), how fast it comes (m/s), and how near it stops (m).")]
        public float comesFrom = 20f;
        public float speed = 3.4f;
        public float stopsAt = 1.9f;

        static readonly int VelZParam = Animator.StringToHash("VelZ");
        static readonly int SpeedParam = Animator.StringToHash("Speed");

        void Start()
        {
            helper.gameObject.SetActive(false);
            if (SaveManager.Slot.story.conversations.Contains(helperTrigger.conversation))
                return;
            StartCoroutine(Arrive());
        }

        IEnumerator Arrive()
        {
            PlayerControl control = PlayerControl.Of(player);
            Ragdoll ragdoll = player.GetComponent<Ragdoll>();

            // The view: where the game's own camera would stand for a character on the ground here, but further
            // off, and staying there.
            Vector3 from = player.position;
            GameObject waiting = null;
            Vector3 offset = Vector3.zero;
            CinemachineCamera own = FollowCamera();
            if (own != null)
            {
                offset = own.GetComponent<CinemachineFollow>().FollowOffset;
                waiting = new GameObject("CM Arrival Camera");
                waiting.SetActive(false);
                var camera = waiting.AddComponent<CinemachineCamera>();
                camera.Lens = own.Lens;
                camera.Priority = new PrioritySettings { Enabled = true, Value = 50 };
                waiting.transform.SetPositionAndRotation(from + offset * wide, own.transform.rotation);
                // Straight to it: the level opens on this view.
                Main.inst.camera.Cut();
                waiting.SetActive(true);
            }

            // Up, and falling: tipped over a little, and turning. (The cameras are told the character has been
            // moved, not flown there.)
            Vector3 up = from;
            up.y = Ground.Land(from) + height;
            control.MoveTo(up);
            CinemachineCore.OnTargetObjectWarped(player, up - from);
            Transform model = player.GetComponentInChildren<Animator>().transform;
            Vector3 modelPlace = model.localPosition;
            Quaternion modelFacing = model.localRotation;
            Vector3 about = Random.onUnitSphere;
            about.y *= 0.35f;
            about = about.sqrMagnitude > 0.001f ? about.normalized : Vector3.right;
            model.rotation = Quaternion.AngleAxis(Random.Range(tipped * 0.5f, tipped), Vector3.Cross(Vector3.up, about)) * model.rotation;
            ragdoll.held = true;
            ragdoll.Fall(Vector3.zero, about * turns, modelPlace, modelFacing);

            // Down: near the ground and all but still. (Or after long enough, whatever it has landed on.) Kept
            // turning on the way, about a line that wanders.
            float fallen = 0f;
            Vector3 was = player.position;
            float still = 0f;
            while (fallen < 30f && still < 0.6f)
            {
                yield return new WaitForFixedUpdate();
                float step = Time.fixedDeltaTime;
                fallen += step;
                bool low = player.position.y - Ground.Land(player.position) < 3f;
                bool slow = (player.position - was).magnitude < 1.5f * step;
                still = low && slow ? still + step : 0f;
                was = player.position;
                // (As much as the parts' own damping takes away, to keep the turning about steady.)
                if (!low)
                    ragdoll.Tumble(Quaternion.AngleAxis(fallen * 40f, Vector3.up) * about * (turns * 1.5f));
            }

            // Lying there, as the view comes in to the game's own.
            Vector3 far = waiting != null ? waiting.transform.position : Vector3.zero;
            for (float t = 0f; t < liesFor; t += Time.deltaTime)
            {
                if (waiting != null)
                    waiting.transform.position = Vector3.Lerp(far, player.position + offset, Mathf.SmoothStep(0f, 1f, t / liesFor));
                yield return null;
            }
            if (waiting != null)
            {
                waiting.SetActive(false);
                Destroy(waiting, 1f);
            }

            // The helper, from the top right of the picture.
            Camera view = Camera.main;
            Vector3 way = view != null
                ? Vector3.ProjectOnPlane(view.transform.forward + view.transform.right, Vector3.up)
                : new Vector3(1f, 0f, 1f);
            way = way.sqrMagnitude > 0.001f ? way.normalized : Vector3.forward;
            Vector3 home = OnTheLand(player.position + way * comesFrom);
            Transform body = helper.transform;
            body.SetPositionAndRotation(home, Quaternion.LookRotation(-way, Vector3.up));
            helper.gameObject.SetActive(true);
            yield return Walk(body, () => player.position, stopsAt);

            // What it has to say.
            helperTrigger.TryStartDialogue();
            yield return null;
            while (Main.inst.dialogue.IsTalking())
                yield return null;

            // Now the character can get up; and when it does, the helper is off.
            ragdoll.held = false;
            while (ragdoll.Down)
                yield return null;
            yield return Walk(body, () => home, 0.3f);
            helper.gameObject.SetActive(false);
        }

        // The game's own camera: the one that is on and follows the character.
        CinemachineCamera FollowCamera()
        {
            foreach (CinemachineCamera camera in FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None))
                if (camera.Follow == player && camera.GetComponent<CinemachineFollow>() != null)
                    return camera;
            return null;
        }

        // Walks someone over the land to within `near` metres of a place (which may move).
        IEnumerator Walk(Transform body, System.Func<Vector3> to, float near)
        {
            while (true)
            {
                Vector3 off = Vector3.ProjectOnPlane(to() - body.position, Vector3.up);
                if (off.magnitude <= near)
                    break;
                Vector3 step = off.normalized * Mathf.Min(speed * Time.deltaTime, off.magnitude);
                body.rotation = Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(off.normalized, Vector3.up), 420f * Time.deltaTime);
                body.position = OnTheLand(body.position + step);
                helper.SetFloat(VelZParam, speed, 0.1f, Time.deltaTime);
                helper.SetFloat(SpeedParam, speed, 0.1f, Time.deltaTime);
                yield return null;
            }

            // To a stop.
            for (float t = 0f; t < 0.4f; t += Time.deltaTime)
            {
                helper.SetFloat(VelZParam, 0f, 0.1f, Time.deltaTime);
                helper.SetFloat(SpeedParam, 0f, 0.1f, Time.deltaTime);
                yield return null;
            }
        }

        static Vector3 OnTheLand(Vector3 at)
        {
            float land = Ground.Land(at);
            if (land > -1000f)
                at.y = land;
            return at;
        }
    }
}
