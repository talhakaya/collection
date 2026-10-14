using Collection.Poster;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Collection.Story
{
    // A blow's impact: lines going out from the silhouette of whatever the main camera sees (the poster's
    // RadiatingOutline, with the Radiating Rings shader), drawn behind everything on the screen.
    //
    // Each Hit sends a front out from the silhouette that reaches past the edges of the screen in `rise` seconds,
    // the lines showing behind it; then they fade away over `fall`. Nothing jumps: the lines come in behind the
    // front, and go out slowly. Hits closer together than that are each a front of their own, and a pixel shows
    // the strongest of those that have reached it - so a new one only ever brings lines up, sweeping out over the
    // older ones as they fade. Four are kept; a fifth takes the place of the oldest.
    //
    // While no lines show, nothing is worked out.
    [DefaultExecutionOrder(9000)]
    public class ImpactRings : MonoBehaviour
    {
        [Tooltip("Seconds a hit's lines take to reach the edges of the screen.")]
        public float rise = 0.6f;
        [Tooltip("Seconds they then take to fade away.")]
        public float fall = 4.4f;
        [Tooltip("How soft the edge of a hit's front is (pixels at 1080 high): the lines come in through it.")]
        public float softness = 160f;

        const int Kept = 4;
        readonly float[] hits = { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity };
        RadiatingOutline outline;
        Vector4 fronts, strengths;

        static readonly int ImpactsId = Shader.PropertyToID("_Impacts");
        static readonly int FrontsId = Shader.PropertyToID("_ImpactFronts");
        static readonly int StrengthsId = Shader.PropertyToID("_ImpactStrengths");
        static readonly int SoftnessId = Shader.PropertyToID("_ImpactSoftness");

        // Whether any lines show.
        public bool Showing { get; private set; }

        // A full screen of it, behind every other canvas, with its own camera for the silhouette.
        public static ImpactRings Make(Material look, Shader jumpFlood)
        {
            var root = new GameObject("Impact Rings");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -10;

            var image = new GameObject("Lines", typeof(RectTransform)).AddComponent<RawImage>();
            var rect = image.rectTransform;
            rect.SetParent(root.transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            image.material = look;
            image.raycastTarget = false;
            image.enabled = false;

            var mask = new GameObject("Impact Silhouette Camera").AddComponent<Camera>();
            mask.transform.SetParent(root.transform, false);
            mask.clearFlags = CameraClearFlags.SolidColor;
            mask.backgroundColor = new Color(1f, 0f, 1f, 1f);
            mask.enabled = false;
            var data = mask.gameObject.AddComponent<UniversalAdditionalCameraData>();
            data.renderPostProcessing = false;
            data.renderShadows = false;
            data.antialiasing = AntialiasingMode.None;

            var outline = root.AddComponent<RadiatingOutline>();
            outline.maskCamera = mask;
            outline.output = image;
            outline.jumpFlood = jumpFlood;
            outline.resolution = 0.5f;
            outline.showing = false;

            var rings = root.AddComponent<ImpactRings>();
            rings.outline = outline;
            outline.Drawing += rings.SetUp;
            return rings;
        }

        public void Hit()
        {
            int oldest = 0;
            for (int i = 1; i < Kept; i++)
                if (hits[i] < hits[oldest])
                    oldest = i;
            hits[oldest] = Time.time;
        }

        void Update()
        {
            // Past the farthest corner of the screen from anywhere on it, at 1080 high, and the soft edge behind.
            float across = 1080f * Mathf.Sqrt(1f + Mathf.Pow((float)Screen.width / Mathf.Max(1, Screen.height), 2f));

            Showing = false;
            for (int i = 0; i < Kept; i++)
            {
                float since = Time.time - hits[i];
                float front = 0f, strength = 0f;
                if (since >= 0f && since < rise + fall)
                {
                    // Fast at first, slowing as it nears the edges.
                    float reach = Mathf.Clamp01(since / rise);
                    front = (across + softness) * (1f - Mathf.Pow(1f - reach, 3f));
                    strength = since < rise ? 1f : 1f - Mathf.SmoothStep(0f, 1f, (since - rise) / fall);
                    Showing |= strength > 0f;
                }
                fronts[i] = front;
                strengths[i] = strength;
            }
            outline.showing = Showing;
        }

        void SetUp(Material material)
        {
            material.SetFloat(ImpactsId, 1f);
            material.SetFloat(SoftnessId, softness);
            material.SetVector(FrontsId, fronts);
            material.SetVector(StrengthsId, strengths);
        }
    }
}
