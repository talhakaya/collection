using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Collection.Story
{
    // Night in the grassy field. The sun goes down to a faint blue moon and the ambient light to a little of the
    // day's, tinted blue, while the sky is left as bright as it was: the ponds go on reflecting it, and stand out
    // against the dark land. The character carries a lantern - a light with no lantern to it - whose reach wavers
    // like a flame's. Over the screen, a drawn animation loops while it's dark (the overlay), each frame
    // growing and fading on top of the ones before.
    //
    // A story beat brings it: a Cutscene whose onStart calls Fall. It fades in over `fadeSeconds`, and the save
    // remembers it, so the field is night from then on whenever it is loaded. Day brings the day back, for a
    // story that turns out to want that.
    public class FieldNight : MonoBehaviour
    {
        const string Remembered = "field night";

        [Tooltip("The sun, which becomes the moon.")]
        public Light sun;
        [Tooltip("For now: the field opens at night. Untick once a story beat brings the night (Fall).")]
        public bool nightAtStart = true;
        [Tooltip("Seconds night takes to fall.")]
        public float fadeSeconds = 4f;
        [Tooltip("How much of the sun is left at night, as the moon. Kept very low: the grass takes a lot from even a faint sun.")]
        [Range(0f, 1f)] public float moonlight = 0.03f;
        [Tooltip("How much of the day's ambient light is left at night.")]
        [Range(0f, 1f)] public float darkness = 0.12f;
        [Tooltip("The moon's colour, for what was the sun.")]
        public Color moonColour = new Color(0.6f, 0.72f, 1f);
        [Tooltip("What the ambient light is multiplied by at night, as well as by `darkness`.")]
        public Color nightTint = new Color(0.7f, 0.82f, 1.25f);
        [Tooltip("How much of the sky the land still reflects at night. Even matte ground reflects a little, and in the dark that little turns it grey.")]
        [Range(0f, 1f)] public float landReflections = 0.1f;
        [Tooltip("How much of the bright sky everything but the terrains reflects at night - which is to say the water.")]
        public float skyReflections = 1f;

        [Header("Lantern")]
        public Color lanternColour = new Color(1f, 0.74f, 0.45f);
        public float lanternIntensity = 7f;
        [Tooltip("Height of the light above the character's feet (m).")]
        public float lanternHeight = 1.6f;
        [Tooltip("The light's reach wavers between these (m).")]
        public float minReach = 7.5f, maxReach = 10.5f;
        [Tooltip("How fast it wavers.")]
        public float flicker = 2.5f;

        [Header("Overlay")]
        [Tooltip("Drawn over the screen at night, one after another on a loop. Covers the screen, cropped rather than stretched.")]
        public Texture2D[] overlayFrames;
        [Tooltip("Frames a second. Each frame comes in as a layer of its own, on top of the ones before it.")]
        public float overlayFps = 6f;
        [Tooltip("Seconds a frame's layer takes to grow and fade away; the longer, the more of them overlap (frames a second times this is how many are on the screen at once).")]
        public float overlayLayerSeconds = 0.5f;
        [Tooltip("How big a layer has grown by the time it's gone (1 is not at all).")]
        public float overlayGrowth = 1.3f;
        [Tooltip("How strongly a new layer shows at full night.")]
        [Range(0f, 1f)] public float overlayOpacity = 0.6f;
        [Tooltip("Each layer is turned over at random, left to right, top to bottom, both or neither, so the loop does not look the same each time round.")]
        public bool overlayFlips = true;

        // Under the character creator, the dialogue and the screen fades.
        const int OverlayOrder = 5;

        // 0 for day, 1 for night, and between while it falls.
        float night;
        float target;

        float dayIntensity;
        Color dayColour;
        AmbientMode dayMode;
        float dayReflections;
        // The day's ambient light from above, from the side and from below, which night's is made from.
        Color daySky, dayEquator, dayGround;

        Light lantern;
        // The sky alone, for the water to go on reflecting at full strength while the land reflects next to
        // nothing: everything but the terrains is given it at night.
        ReflectionProbe sky;
        float seed;
        Canvas overlay;
        // Enough layers for every frame still fading; the oldest is reused for the next frame.
        RawImage[] overlayLayers;
        float[] overlayBorn;
        int overlayStep = -1;

        public bool IsNight => target >= 1f;

        void Awake()
        {
            if (sun != null)
            {
                dayIntensity = sun.intensity;
                dayColour = sun.color;
            }
            dayMode = RenderSettings.ambientMode;
            dayReflections = RenderSettings.reflectionIntensity;

            var directions = new[] { Vector3.up, Vector3.forward, Vector3.down };
            var colours = new Color[3];
            RenderSettings.ambientProbe.Evaluate(directions, colours);
            daySky = colours[0];
            dayEquator = colours[1];
            dayGround = colours[2];

            var held = new GameObject("Lantern");
            lantern = held.AddComponent<Light>();
            lantern.type = LightType.Point;
            lantern.color = lanternColour;
            lantern.shadows = LightShadows.None;
            lantern.range = maxReach;
            lantern.enabled = false;
            seed = Random.value * 100f;

            sky = new GameObject("Night Sky Reflection").AddComponent<ReflectionProbe>();
            sky.mode = ReflectionProbeMode.Realtime;
            sky.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            sky.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
            sky.clearFlags = ReflectionProbeClearFlags.Skybox;
            sky.cullingMask = 0;
            sky.size = Vector3.one * 2000f;
            sky.resolution = 128;
            sky.importance = 100;
            sky.enabled = false;

            if (overlayFrames != null && overlayFrames.Length > 0 && overlayFrames[0] != null)
            {
                overlay = new GameObject("Night Overlay").AddComponent<Canvas>();
                overlay.renderMode = RenderMode.ScreenSpaceOverlay;
                overlay.sortingOrder = OverlayOrder;
                overlay.enabled = false;
                int count = Mathf.CeilToInt(overlayFps * overlayLayerSeconds) + 1;
                overlayLayers = new RawImage[count];
                overlayBorn = new float[count];
                for (int i = 0; i < count; i++)
                {
                    var layer = new GameObject("Frame", typeof(RectTransform)).AddComponent<RawImage>();
                    layer.transform.SetParent(overlay.transform, false);
                    layer.raycastTarget = false;
                    layer.enabled = false;
                    var fit = layer.gameObject.AddComponent<AspectRatioFitter>();
                    fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                    fit.aspectRatio = (float)overlayFrames[0].width / overlayFrames[0].height;
                    overlayLayers[i] = layer;
                    overlayBorn[i] = float.NegativeInfinity;
                }
            }
        }

        void Start()
        {
            if (nightAtStart || StoryMemory.Is(Remembered, "yes"))
            {
                night = target = 1f;
                Apply();
            }
        }

        // Night falls, and stays.
        public void Fall()
        {
            target = 1f;
            StoryMemory.Set(Remembered, "yes");
        }

        public void Day()
        {
            target = 0f;
            StoryMemory.Set(Remembered, "no");
        }

        void Update()
        {
            if (night == target)
                return;
            night = Mathf.MoveTowards(night, target, fadeSeconds > 0f ? Time.deltaTime / fadeSeconds : 1f);
            Apply();
        }

        void Apply()
        {
            float t = Mathf.SmoothStep(0f, 1f, night);

            if (sun != null)
            {
                sun.intensity = Mathf.Lerp(dayIntensity, dayIntensity * moonlight, t);
                sun.color = Color.Lerp(dayColour, moonColour, t);
            }

            // The day's own ambient light, from the sky, until night starts to fall; then the same light from
            // above, the side and below, which can be turned down and tinted.
            if (night <= 0f)
            {
                RenderSettings.ambientMode = dayMode;
            }
            else
            {
                Color by = nightTint * darkness;
                RenderSettings.ambientMode = AmbientMode.Trilight;
                RenderSettings.ambientSkyColor = Color.Lerp(daySky, daySky * by, t);
                RenderSettings.ambientEquatorColor = Color.Lerp(dayEquator, dayEquator * by, t);
                RenderSettings.ambientGroundColor = Color.Lerp(dayGround, dayGround * by, t);
            }
            // The sky as the land reflects it, turned down; the water keeps it, from the probe.
            RenderSettings.reflectionIntensity = Mathf.Lerp(dayReflections, dayReflections * landReflections, t);
            bool dark = night > 0f;
            if (dark && !sky.enabled)
            {
                sky.enabled = true;
                sky.intensity = skyReflections;
                sky.RenderProbe();
            }
            sky.enabled = dark;
            foreach (Terrain terrain in Terrain.activeTerrains)
                terrain.reflectionProbeUsage = dark ? ReflectionProbeUsage.Off : ReflectionProbeUsage.BlendProbes;

            lantern.enabled = dark;

            if (overlay != null)
                overlay.enabled = dark;
        }

        // Each step of the animation puts its frame on top as a new layer. A layer comes in quickly, then grows
        // from the middle of the screen and fades away, so several frames drift outward at once.
        void AnimateOverlay()
        {
            int step = Mathf.FloorToInt(Time.time * overlayFps);
            if (step != overlayStep)
            {
                overlayStep = step;
                int newest = step % overlayLayers.Length;
                overlayLayers[newest].texture = overlayFrames[step % overlayFrames.Length];
                bool acrossFlipped = overlayFlips && Random.value < 0.5f, upFlipped = overlayFlips && Random.value < 0.5f;
                overlayLayers[newest].uvRect = new Rect(acrossFlipped ? 1f : 0f, upFlipped ? 1f : 0f, acrossFlipped ? -1f : 1f, upFlipped ? -1f : 1f);
                overlayLayers[newest].transform.SetAsLastSibling();
                overlayBorn[newest] = Time.time;
            }

            float strength = overlayOpacity * Mathf.SmoothStep(0f, 1f, night);
            for (int i = 0; i < overlayLayers.Length; i++)
            {
                float age = (Time.time - overlayBorn[i]) / Mathf.Max(overlayLayerSeconds, 0.01f);
                RawImage layer = overlayLayers[i];
                layer.enabled = age < 1f;
                if (!layer.enabled)
                    continue;
                float fadeIn = Mathf.Clamp01(age / 0.1f);
                float fadeOut = (1f - age) * (1f - age);
                layer.color = new Color(1f, 1f, 1f, strength * fadeIn * fadeOut);
                // Quick at first, slowing as it goes.
                layer.transform.localScale = Vector3.one * Mathf.Lerp(1f, overlayGrowth, 1f - fadeOut);
            }
        }

        void LateUpdate()
        {
            if (!lantern.enabled)
                return;
            if (overlay != null)
                AnimateOverlay();
            PlayerMovement player = PlayerMovement.Current;
            if (player != null)
                lantern.transform.position = player.transform.position + Vector3.up * lanternHeight;

            // A slow sway and a quicker flutter on top of it, so the reach never settles.
            float time = Time.time * flicker;
            float waver = Mathf.PerlinNoise(seed, time) * 0.7f + Mathf.PerlinNoise(seed + 31f, time * 3.1f) * 0.3f;
            lantern.range = Mathf.Lerp(minReach, maxReach, waver);
            lantern.intensity = lanternIntensity * Mathf.SmoothStep(0f, 1f, night) * Mathf.Lerp(0.85f, 1f, waver);
        }

        void OnDestroy()
        {
            if (lantern != null)
                Destroy(lantern.gameObject);
            if (sky != null)
                Destroy(sky.gameObject);
            if (overlay != null)
                Destroy(overlay.gameObject);
        }
    }
}
