using UnityEngine;

// In the collection: the Standard Assets edge detection worked through OnRenderImage,
// which URP never calls, and URP has no edge detection of its own. The component is kept
// (same file, so the scene's binding and its saved values survive) but draws nothing:
// the picture is shown without the outlines. See MIGRATION.md.
namespace Games.Screens.ImageEffects
{
    [RequireComponent(typeof (Camera))]
    public class EdgeDetection : MonoBehaviour
    {
        public enum EdgeDetectMode
        {
            TriangleDepthNormals = 0,
            RobertsCrossDepthNormals = 1,
            SobelDepth = 2,
            SobelDepthThin = 3,
            TriangleLuminance = 4,
        }

        public EdgeDetectMode mode = EdgeDetectMode.SobelDepthThin;
        public float sensitivityDepth = 1.0f;
        public float sensitivityNormals = 1.0f;
        public float lumThreshold = 0.2f;
        public float edgeExp = 1.0f;
        public float sampleDist = 1.0f;
        public float edgesOnly = 0.0f;
        public Color edgesOnlyBgColor = Color.white;
    }
}
