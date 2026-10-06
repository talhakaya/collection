using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collection.Story
{
    // Meeting the giant television on the sea, and the land coming up out of the water.
    //
    //   Begin (when its conversation starts, on walking up to it): the view pulls far back, behind the character,
    //   to take all of the television in.
    //
    //   RaiseLand (a cutscene in the conversation): the land, which the intro switched off, comes up from under
    //   the water while the sea sinks a little, and as the land settles the sea drains away out of sight: the
    //   desert has none. The
    //   character rides up on whatever ground comes up under its feet. The television, which stands in the sea,
    //   goes down with the sea.
    //
    //   As the land comes up the light turns to the desert's: the sun goes yellow and the desert's own look (a
    //   post-processing volume: colour grading, contrast, bloom) comes in over the sea's.
    //
    //   End (when the conversation is over): back to the follow camera, on land.
    //
    // It happens once in a save slot (its NPCTrigger is onlyOnce). For a game in which it has happened,
    // StoryDirector calls Skip: the land up and the sea down, as RaiseLand leaves them.
    public class TelevisionEncounter : MonoBehaviour
    {
        public Transform player;
        [Tooltip("The television itself: what the camera looks at, and what goes down with the sea.")]
        public Transform television;

        [Header("The view")]
        [Tooltip("The Cinemachine camera for the wide shot. It is placed when the conversation begins.")]
        public GameObject wideCamera;
        [Tooltip("How far behind the character the camera stands, on the line from the television through the character (m).")]
        public float cameraBack = 22f;
        public float cameraHeight = 9f;
        [Tooltip("The height on the television that the camera looks at (m above its base).")]
        public float lookAtHeight = 9f;

        [Header("The land coming up")]
        [Tooltip("The land's objects, switched off by the intro.")]
        public List<GameObject> world = new List<GameObject>();
        public Terrain terrain;
        [Tooltip("How far under its own place the land starts (m): deep enough that none of it shows through the water.")]
        public float landDepth = 9f;
        [Tooltip("The water's surface and the floor under it.")]
        public Transform water;
        public Transform floor;
        [Tooltip("How far the sea sinks (m): far enough to be out of sight for good. The desert has no sea.")]
        public float seaDrop = 40f;
        [Tooltip("How far it sinks while the land is still coming up, staying round it (m); the rest of the way it goes in the last part of the rise. The floor under it, which the character stands on, goes down only this far.")]
        public float seaSettle = 0.85f;
        [Tooltip("The part of the rise after which the sea drains away (0 to 1).")]
        [Range(0f, 1f)] public float drainFrom = 0.6f;
        [Tooltip("How far the television goes down (m): onto the bed of the bay it stood in.")]
        public float televisionDrop = 2.4f;

        [Header("The desert's light")]
        public Light sun;
        [Tooltip("The sun's colour once the land is up.")]
        public Color sunColour = new Color(1f, 0.84f, 0.5f);
        [Tooltip("The desert's look, a global volume over the scene's own. Its weight goes from 0 to 1 as the land comes up.")]
        public UnityEngine.Rendering.Volume desertLook;

        static readonly int TiltShiftDisabledId = Shader.PropertyToID("_TiltShiftDisabled");

        readonly List<Vector3> places = new List<Vector3>();
        Coroutine rising;
        Color sunBefore;
        bool landUp;

        // The sea, once it has drained: gone, with its floor and the camera that drew its reflections.
        void SeaGone()
        {
            landUp = true;
            if (water != null && water.parent != null)
                water.parent.gameObject.SetActive(false);
        }

        // How far the light is the desert's: 0 the sea's, 1 the desert's.
        void SetDesert(float amount)
        {
            if (sun != null)
                sun.color = Color.Lerp(sunBefore, sunColour, amount);
            if (desertLook != null)
                desertLook.weight = amount;
        }

        void Awake()
        {
            if (sun != null)
                sunBefore = sun.color;
            SetDesert(0f);

            // Where the land belongs, before anything moves it.
            foreach (GameObject part in world)
                places.Add(part != null ? part.transform.position : Vector3.zero);
        }

        public void Begin()
        {
            Vector3 away = player.position - television.position;
            away.y = 0f;
            away = away.sqrMagnitude > 0.01f ? away.normalized : Vector3.forward;
            Vector3 from = player.position + away * cameraBack + Vector3.up * cameraHeight;
            Vector3 target = television.position + Vector3.up * lookAtHeight;
            wideCamera.transform.SetPositionAndRotation(from, Quaternion.LookRotation(target - from, Vector3.up));

            // A plain wide shot: the blur at the top and bottom of the screen would be across the television.
            Shader.SetGlobalFloat(TiltShiftDisabledId, 1f);
            wideCamera.SetActive(true);
        }

        // The land and the sea as they are after RaiseLand. To be called after IntroSequence.Skip, which puts the
        // sea where it is before.
        public void Skip()
        {
            TakeStart();
            foreach (GameObject part in world)
                if (part != null)
                    part.SetActive(true);
            Show(1f);
            SeaGone();
        }

        // Where the sea, its floor and the television are before any of this. Taken once: everything after is
        // counted from there.
        bool startKnown;
        float waterStart, floorStart, televisionStart;

        void TakeStart()
        {
            if (startKnown)
                return;
            startKnown = true;
            waterStart = water.position.y;
            floorStart = floor.position.y;
            televisionStart = television.position.y;
        }

        // The land, the sea, the television and the light with this much of the rise behind them (0 to 1). The
        // one description of it: the rise goes through it from 0 to 1, and a game in which it has happened is
        // shown it at 1.
        void Show(float done)
        {
            float eased = Mathf.SmoothStep(0f, 1f, done);

            for (int i = 0; i < world.Count; i++)
                if (world[i] != null)
                    world[i].transform.position = places[i] + Vector3.down * (landDepth * (1f - eased));

            SetDesert(eased);
            // The sea stays round the land as it comes up, a little lower, and drains away at the end.
            float drop = seaSettle * eased;
            float drained = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(drainFrom, 1f, done));
            SetHeight(water, waterStart - drop - (seaDrop - seaSettle) * drained);
            SetHeight(floor, floorStart - drop);
            SetHeight(television, televisionStart - televisionDrop * eased);
        }

        // The top of the sea's floor, where it is at the moment.
        float FloorTop()
        {
            return floor.position.y + floor.GetComponent<BoxCollider>().size.y * 0.5f * floor.lossyScale.y;
        }

        // The ground at a place: the higher of the sea's floor and the land, if the land is there.
        public float GroundHeight(Vector3 at)
        {
            float land = Ground.Land(at);
            return landUp ? land : Mathf.Max(FloorTop(), land);
        }

        // For a Cutscene's onStart.
        public void RaiseLand(float seconds)
        {
            if (rising == null)
                rising = StartCoroutine(Rise(seconds));
        }

        IEnumerator Rise(float seconds)
        {
            // The character is carried, not walking, while the ground moves.
            PlayerControl control = PlayerControl.Of(player);
            control.Take(this);
            var ragdoll = player.GetComponent<Ragdoll>();

            TakeStart();
            float standing = player.position.y - FloorTop();

            Show(0f);
            foreach (GameObject part in world)
                if (part != null)
                    part.SetActive(true);

            for (float t = 0f; ; t += Time.deltaTime)
            {
                float done = seconds > 0f ? Mathf.Clamp01(t / seconds) : 1f;
                Show(done);

                // The higher of the sea's floor and the land, where the character is.
                // (Not one that is lying after a fall: that one is where its body is, on whatever is under it.)
                float ground = Mathf.Max(FloorTop(), Ground.Land(player.position));
                if (ragdoll == null || !ragdoll.Down)
                    SetHeight(player, ground + standing);

                if (done >= 1f)
                    break;
                yield return null;
            }

            control.Release(this);
            SeaGone();
            rising = null;
        }

        static void SetHeight(Transform what, float y)
        {
            Vector3 position = what.position;
            position.y = y;
            what.position = position;
        }

        // For the conversation's end.
        public void End()
        {
            wideCamera.SetActive(false);
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        void OnDestroy()
        {
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }
    }
}
