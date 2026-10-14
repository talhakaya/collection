using UnityEngine;
using UnityEngine.UI;

namespace Collection.Poster
{
    // For the game's poster: lines radiating out from a silhouette, evenly spaced and all the same width, out to the
    // edges of the screen. Shown on a RawImage covering the screen, with the Radiating Rings material; play the
    // scene to see it (and to record it).
    //
    // The silhouette is either a drawing or a camera's picture:
    //
    //   `outline`: a drawing of the silhouette's outline, opaque on transparent. Filled in once on starting: what
    //   cannot be reached from the top left corner without crossing the line is inside. It is fitted to the
    //   screen's height and stood on the bottom edge.
    //
    //   `maskCamera`: everything it sees over its background colour is the silhouette - a 3D character, say -
    //   every frame, so it can move. It is kept looking exactly as the main camera does.
    //
    // Every frame, then, each pixel of the screen is given the nearest pixel of the silhouette (JumpFlood.shader),
    // and the rings shader draws a line wherever the distance to it is a whole number of spacings.
    public class RadiatingOutline : MonoBehaviour
    {
        [Tooltip("A drawing of the outline, opaque on transparent. Needs Read/Write ticked in its import settings.")]
        public Texture2D outline;
        [Tooltip("Or: a camera whose picture, over its background colour, is the silhouette.")]
        public Camera maskCamera;
        [Tooltip("The RawImage covering the screen, with the Radiating Rings material.")]
        public RawImage output;
        public Shader jumpFlood;
        [Tooltip("How fast the rings go out, in pixels a second at 1080 high.")]
        public float speed = 40f;

        Material flood;
        Texture2D filled;
        RenderTexture mask, a, b;
        Material rings, look;

        static readonly int MaskRect = Shader.PropertyToID("_MaskRect");
        static readonly int ScreenPixels = Shader.PropertyToID("_ScreenPixels");
        static readonly int KeyColour = Shader.PropertyToID("_KeyColour");
        static readonly int UseKey = Shader.PropertyToID("_UseKey");
        static readonly int Step = Shader.PropertyToID("_Step");
        static readonly int Offset = Shader.PropertyToID("_Offset");

        void Start()
        {
            flood = new Material(jumpFlood);
            // Its own copy, so the offset moving each frame is not saved into the material asset. The asset's
            // settings are copied over every frame, so changing them while playing shows at once.
            look = output.material;
            rings = new Material(look);
            output.material = rings;
            if (outline != null)
                filled = Fill(outline);
            if (maskCamera != null)
                maskCamera.enabled = false;
        }

        void LateUpdate()
        {
            int width = Screen.width, height = Screen.height;
            Fit(ref a, width, height, RenderTextureFormat.RGFloat);
            Fit(ref b, width, height, RenderTextureFormat.RGFloat);

            // The silhouette, into `a`.
            flood.SetVector(ScreenPixels, new Vector4(width, height, 0, 0));
            if (maskCamera != null)
            {
                Fit(ref mask, width, height, RenderTextureFormat.ARGB32);
                Camera main = Camera.main;
                if (main != null && main != maskCamera)
                {
                    maskCamera.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
                    maskCamera.fieldOfView = main.fieldOfView;
                    maskCamera.orthographic = main.orthographic;
                    maskCamera.orthographicSize = main.orthographicSize;
                    maskCamera.nearClipPlane = main.nearClipPlane;
                    maskCamera.farClipPlane = main.farClipPlane;
                }
                maskCamera.clearFlags = CameraClearFlags.SolidColor;
                maskCamera.targetTexture = mask;
                maskCamera.Render();
                flood.SetVector(MaskRect, new Vector4(0, 0, 1, 1));
                flood.SetColor(KeyColour, maskCamera.backgroundColor);
                flood.SetFloat(UseKey, 1);
                Graphics.Blit(mask, a, flood, 0);
            }
            else if (filled != null)
            {
                // As tall as the screen, in the middle, standing on the bottom edge.
                float across = (float)filled.width / filled.height * height / width;
                flood.SetVector(MaskRect, new Vector4(0.5f - across * 0.5f, 0, across, 1));
                flood.SetFloat(UseKey, 0);
                Graphics.Blit(filled, a, flood, 0);
            }
            else
            {
                return;
            }

            // Steps halving from half the screen down to a pixel, back and forth between `a` and `b`.
            int step = Mathf.NextPowerOfTwo(Mathf.Max(width, height)) / 2;
            RenderTexture from = a, to = b;
            for (; step >= 1; step /= 2)
            {
                flood.SetFloat(Step, step);
                Graphics.Blit(from, to, flood, 1);
                (from, to) = (to, from);
            }

            output.texture = from;
            rings.CopyPropertiesFromMaterial(look);
            rings.SetFloat(Offset, Time.time * speed);
        }

        static void Fit(ref RenderTexture texture, int width, int height, RenderTextureFormat format)
        {
            if (texture != null && texture.width == width && texture.height == height)
                return;
            if (texture != null)
                texture.Release();
            texture = new RenderTexture(width, height, format == RenderTextureFormat.ARGB32 ? 24 : 0, format)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            texture.Create();
        }

        // The outline drawing with its inside filled: opaque wherever the line is or that the top left corner
        // cannot reach without crossing it.
        static Texture2D Fill(Texture2D drawing)
        {
            int w = drawing.width, h = drawing.height;
            Color32[] pixels = drawing.GetPixels32();
            var outside = new bool[w * h];
            var open = new System.Collections.Generic.Stack<int>();
            // Row 0 is the bottom; the top left corner is the first of the top row.
            int corner = (h - 1) * w;
            if (pixels[corner].a <= 127)
            {
                outside[corner] = true;
                open.Push(corner);
            }
            while (open.Count > 0)
            {
                int at = open.Pop();
                int x = at % w, y = at / w;
                Visit(x - 1, y);
                Visit(x + 1, y);
                Visit(x, y - 1);
                Visit(x, y + 1);
            }

            void Visit(int x, int y)
            {
                if (x < 0 || y < 0 || x >= w || y >= h)
                    return;
                int at = y * w + x;
                if (outside[at] || pixels[at].a > 127)
                    return;
                outside[at] = true;
                open.Push(at);
            }

            var result = new Color32[w * h];
            for (int i = 0; i < result.Length; i++)
                result[i] = outside[i] ? new Color32(0, 0, 0, 0) : new Color32(255, 255, 255, 255);
            var texture = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            texture.SetPixels32(result);
            texture.Apply();
            return texture;
        }

        void OnDestroy()
        {
            foreach (RenderTexture texture in new[] { mask, a, b })
                if (texture != null)
                    texture.Release();
        }
    }
}
