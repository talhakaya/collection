using UnityEngine;

namespace Collection.Story
{
    // The desert's sand glittering: little stars that come and go on the ground around the character, each at a
    // place of its own for a moment. They are drawn as they are, not lit, so they show the same on any screen.
    //
    // They go by the camera, as glints in sand do, not by the clock: they come and twinkle as the camera moves
    // and turns, faster the faster it does, and hold still while it does. A metre the camera moves is
    // `perMetre` seconds of them, and a degree it turns `perDegree`.
    //
    // Only on dry land: a place lower than `above` (under the sea, or with the land not risen yet) has none.
    [RequireComponent(typeof(ParticleSystem))]
    public class SandSparkles : MonoBehaviour
    {
        [Tooltip("The sparkle's material (an added-on, unlit star).")]
        public Material sparkle;
        [Tooltip("How far round the character they come (m): across, and front to back.")]
        public Vector2 area = new Vector2(22f, 14f);
        [Tooltip("How many come in a second's worth of the camera's moving.")]
        public float perSecond = 45f;
        [Tooltip("How long one lasts (s of the camera's moving): it grows in and shrinks away over this.")]
        public Vector2 lasts = new Vector2(0.25f, 0.5f);
        [Tooltip("Seconds' worth for each metre the camera moves: walking pace is about 4 m a second.")]
        public float perMetre = 0.25f;
        [Tooltip("Seconds' worth for each degree the camera turns.")]
        public float perDegree = 0.03f;
        [Tooltip("The most one frame can be worth (s): a jump of the camera to somewhere else is not a burst.")]
        public float mostAtOnce = 0.1f;
        [Tooltip("How big one is at its brightest (m).")]
        public Vector2 size = new Vector2(0.12f, 0.24f);
        public Color colour = new Color(1f, 0.97f, 0.85f, 1f);
        [Tooltip("No sparkles where the ground is lower than this (m): under the sea.")]
        public float above = 0.1f;
        [Tooltip("How high over the ground they are (m), so the ground does not cut them.")]
        public float lift = 0.05f;

        ParticleSystem system;
        float due;
        Transform seenFrom;
        Vector3 lastPlace;
        Quaternion lastTurn;

        void Awake()
        {
            system = GetComponent<ParticleSystem>();
            var main = system.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;
            main.loop = true;
            main.maxParticles = 200;
            main.startSpeed = 0f;
            main.gravityModifier = 0f;
            var emission = system.emission;
            emission.rateOverTime = 0f;
            var shape = system.shape;
            shape.enabled = false;

            // Grows in fast and goes out a little slower: a twinkle.
            var sizeOver = system.sizeOverLifetime;
            sizeOver.enabled = true;
            var curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));
            sizeOver.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var renderer = GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sharedMaterial = sparkle;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            // Moved on by hand, by the camera's moving (Simulate), not by the clock.
            system.Play();
            system.Pause();
        }

        // After the camera has been moved for the frame.
        void LateUpdate()
        {
            float step = CameraMoved();
            if (step <= 0f)
                return;
            system.Simulate(step, true, false, false);

            PlayerMovement player = PlayerMovement.Current;
            if (player == null)
                return;

            Vector3 around = player.transform.position;
            due += perSecond * step;
            while (due >= 1f)
            {
                due -= 1f;
                Vector3 at = around + new Vector3(Random.Range(-0.5f, 0.5f) * area.x, 0f, Random.Range(-0.5f, 0.5f) * area.y);
                if (!Ground(at, out float height) || height < above)
                    continue;
                at.y = height + lift;

                var emit = new ParticleSystem.EmitParams
                {
                    position = at,
                    startLifetime = Random.Range(lasts.x, lasts.y),
                    startSize = Random.Range(size.x, size.y),
                    startColor = colour,
                    rotation = Random.Range(0f, 45f),
                };
                system.Emit(emit, 1);
            }
            // (Emitting sets it playing, by the clock, again.)
            system.Pause();
        }

        // How far the camera has moved and turned since the last frame, as seconds' worth of sparkling.
        float CameraMoved()
        {
            Camera camera = Camera.main;
            if (camera == null)
                return 0f;

            Transform now = camera.transform;
            if (now != seenFrom)
            {
                seenFrom = now;
                lastPlace = now.position;
                lastTurn = now.rotation;
                return 0f;
            }

            float metres = Vector3.Distance(now.position, lastPlace);
            float degrees = Quaternion.Angle(now.rotation, lastTurn);
            lastPlace = now.position;
            lastTurn = now.rotation;
            return Mathf.Min(metres * perMetre + degrees * perDegree, mostAtOnce);
        }

        // The height of the terrain under a place, if there is one.
        static bool Ground(Vector3 at, out float height)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Vector3 corner = terrain.transform.position;
                Vector3 size = terrain.terrainData.size;
                if (at.x < corner.x || at.z < corner.z || at.x > corner.x + size.x || at.z > corner.z + size.z)
                    continue;
                height = corner.y + terrain.SampleHeight(at);
                return true;
            }
            height = 0f;
            return false;
        }
    }
}
