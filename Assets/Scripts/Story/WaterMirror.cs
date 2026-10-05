using UnityEngine;
using UnityEngine.Rendering;

namespace Collection.Story
{
    // The camera that draws what the water reflects, for the Simple Water Shader (IgniteCoders). The shader shows a
    // picture taken by a second camera mirrored under the water; this is that camera's script, in place of the
    // asset's WaterReflection, which mirrored the main camera and left it at that. What this does besides:
    //
    //   - Cuts the picture off at the water. The mirrored camera is under the surface looking up, so without that
    //     it also draws whatever is below the water (the sea floor, the far shore's underside) into the reflection.
    //   - Follows the main camera's lens and shape every frame, not once, so a change of camera (Cinemachine
    //     switching between them) or of the window keeps the reflection lined up.
    //   - Is placed at the last moment, as its own picture is about to be drawn, so it uses where the main camera
    //     really is this frame and not where it was before Cinemachine moved it.
    //   - Draws into a texture the size of the screen (times `resolution`), made here and handed to the water's
    //     renderers, where the asset used one fixed 1024 texture.
    //   - Leaves the water itself and anything on the layers not ticked in `reflects` out of the reflection.
    //
    // The picture is flipped left to right compared with a true mirror image, as the asset's was: the shader
    // un-flips it when it reads it.
    [RequireComponent(typeof(Camera))]
    public class WaterMirror : MonoBehaviour
    {
        [Tooltip("The water's surface: its position and its up are the mirror.")]
        public Transform plane;
        [Tooltip("The water renderers that show the reflection.")]
        public Renderer[] water;
        [Tooltip("The layers that show up in the reflection.")]
        public LayerMask reflects = ~0;
        [Tooltip("The reflection's size as a part of the screen's.")]
        [Range(0.25f, 1f)] public float resolution = 0.75f;
        [Tooltip("How far above the surface the cut is (m). A little above avoids a bright line where things meet the water.")]
        public float clipOffset = 0.02f;

        // The shader graph's name for its Reflection Texture.
        static readonly int ReflectionTextureId = Shader.PropertyToID("Texture2D_28de85506601443d82b6148f21ccc69c");

        Camera mirror;
        RenderTexture texture;
        MaterialPropertyBlock block;

        void OnEnable()
        {
            mirror = GetComponent<Camera>();
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            UpdateTexture();
        }

        void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            mirror.targetTexture = null;
            if (texture != null)
            {
                texture.Release();
                Destroy(texture);
                texture = null;
            }
        }

        void LateUpdate()
        {
            UpdateTexture();
        }

        void UpdateTexture()
        {
            int width = Mathf.Max(16, Mathf.RoundToInt(Screen.width * resolution));
            int height = Mathf.Max(16, Mathf.RoundToInt(Screen.height * resolution));
            if (texture != null && texture.width == width && texture.height == height)
                return;

            if (texture != null)
            {
                mirror.targetTexture = null;
                texture.Release();
                Destroy(texture);
            }
            texture = new RenderTexture(width, height, 24, RenderTextureFormat.DefaultHDR) { name = "Water Reflection" };
            mirror.targetTexture = texture;

            block ??= new MaterialPropertyBlock();
            foreach (Renderer renderer in water)
            {
                if (renderer == null)
                    continue;
                renderer.GetPropertyBlock(block);
                block.SetTexture(ReflectionTextureId, texture);
                renderer.SetPropertyBlock(block);
            }
        }

        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == mirror)
                Place();
        }

        void Place()
        {
            Camera main = Camera.main;
            if (main == null || plane == null)
                return;

            // The same lens, drawn before the main camera so the picture is this frame's.
            mirror.fieldOfView = main.fieldOfView;
            mirror.orthographic = main.orthographic;
            mirror.orthographicSize = main.orthographicSize;
            mirror.nearClipPlane = main.nearClipPlane;
            mirror.farClipPlane = main.farClipPlane;
            mirror.aspect = main.aspect;
            mirror.depth = main.depth - 1f;
            mirror.cullingMask = reflects.value;

            // The main camera's place and directions, in the plane's own space, with up and down swapped.
            Transform from = main.transform;
            Vector3 position = plane.InverseTransformPoint(from.position);
            Vector3 forward = plane.InverseTransformDirection(from.forward);
            Vector3 up = plane.InverseTransformDirection(from.up);
            position.y = -position.y;
            forward.y = -forward.y;
            up.y = -up.y;
            position = plane.TransformPoint(position);
            forward = plane.TransformDirection(forward);
            up = plane.TransformDirection(up);
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(forward, up));

            // Nothing under the surface: the near side of what is drawn is the water itself.
            Vector3 normal = plane.up;
            Matrix4x4 toCamera = mirror.worldToCameraMatrix;
            Vector3 point = toCamera.MultiplyPoint(plane.position + normal * clipOffset);
            Vector3 direction = toCamera.MultiplyVector(normal).normalized;
            var clip = new Vector4(direction.x, direction.y, direction.z, -Vector3.Dot(point, direction));
            mirror.ResetProjectionMatrix();
            mirror.projectionMatrix = mirror.CalculateObliqueMatrix(clip);
        }
    }
}
