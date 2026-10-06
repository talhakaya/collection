using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

namespace Collection.Story
{
    // Camera-related settings and view switching: the outdoor view, cave interiors, and the character creator studio.
    public class CameraMan : MonoBehaviour
    {
        // Tilt-shift blur material, referenced here so its values can be tweaked while playing.
        public Material tiltShiftBlur;

        [Header("Cave view")]
        public Camera mainCamera;
        [Tooltip("Closer Cinemachine camera, enabled while the player is inside a cave.")]
        public GameObject caveCamera;
        [Tooltip("Layers drawn while inside a cave. Everything else is left black.")]
        public LayerMask caveCullingMask;
        [Tooltip("Turned off inside caves so the interior is lit only by cave lights.")]
        public Light sun;
        public Color caveAmbient = new Color(0.06f, 0.05f, 0.05f);
        public CanvasGroup screenFade;
        public float fadeDuration = 0.2f;

        [Header("Character creator studio")]
        [Tooltip("Layers drawn in the character creator: just the player on black.")]
        public LayerMask studioCullingMask;

        static readonly int TiltShiftDisabledId = Shader.PropertyToID("_TiltShiftDisabled");

        int caveZoneCount;
        bool wantInside;
        bool viewInside;
        int outdoorCullingMask;
        CameraClearFlags outdoorClearFlags;
        Color outdoorBackground;
        SphericalHarmonicsL2 outdoorAmbient;
        Coroutine transition;
        CinemachineBrain brain;
        CinemachineBlendDefinition normalBlend;

        void Awake()
        {
            outdoorCullingMask = mainCamera.cullingMask;
            outdoorClearFlags = mainCamera.clearFlags;
            outdoorBackground = mainCamera.backgroundColor;
            brain = mainCamera.GetComponent<CinemachineBrain>();
            normalBlend = brain.DefaultBlend;
            caveCamera.SetActive(false);
            screenFade.alpha = 0f;
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        void Start()
        {
            // The ambient probe comes from the scene's lighting data, which is only guaranteed to be ready by Start.
            outdoorAmbient = RenderSettings.ambientProbe;
        }

        void OnDestroy()
        {
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        // Character creator: only the player on black, no tilt-shift blur, seen through the given studio camera.
        // Switches cut instantly, since the character is moved far away for the studio.
        public void EnterStudio(GameObject studioCamera)
        {
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            mainCamera.cullingMask = studioCullingMask.value;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
            Shader.SetGlobalFloat(TiltShiftDisabledId, 1f);
            studioCamera.SetActive(true);
        }

        public void ExitStudio(GameObject studioCamera)
        {
            studioCamera.SetActive(false);
            mainCamera.cullingMask = outdoorCullingMask;
            mainCamera.clearFlags = outdoorClearFlags;
            mainCamera.backgroundColor = outdoorBackground;
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
            StartCoroutine(RestoreBlendAfterCut());
        }

        // The next change of camera, made this frame, is a cut and not a move from the one before: for a scene
        // that starts on a camera of its own. (The game's follow camera is live from the scene's first moment,
        // and anything switched on after it would be moved to from there.)
        public void Cut()
        {
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            StartCoroutine(RestoreBlendAfterCut());
        }

        IEnumerator RestoreBlendAfterCut()
        {
            // Let the brain make the cut back to the follow camera first.
            yield return null;
            yield return null;
            brain.DefaultBlend = normalBlend;
        }

        public void EnterCave()
        {
            caveZoneCount++;
            UpdateView();
        }

        public void ExitCave()
        {
            caveZoneCount = Mathf.Max(0, caveZoneCount - 1);
            UpdateView();
        }

        void UpdateView()
        {
            bool inside = caveZoneCount > 0;
            if (inside == wantInside)
                return;
            wantInside = inside;

            if (transition != null)
                StopCoroutine(transition);
            transition = StartCoroutine(FadeThroughBlack(inside));
        }

        IEnumerator FadeThroughBlack(bool inside)
        {
            yield return Fade(1f);
            ApplyView(inside);
            yield return Fade(0f);
            transition = null;
        }

        IEnumerator Fade(float target)
        {
            while (!Mathf.Approximately(screenFade.alpha, target))
            {
                screenFade.alpha = Mathf.MoveTowards(screenFade.alpha, target, Time.unscaledDeltaTime / fadeDuration);
                yield return null;
            }
        }

        void ApplyView(bool inside)
        {
            if (inside == viewInside)
                return;
            viewInside = inside;

            mainCamera.cullingMask = inside ? caveCullingMask.value : outdoorCullingMask;
            mainCamera.clearFlags = inside ? CameraClearFlags.SolidColor : outdoorClearFlags;
            mainCamera.backgroundColor = inside ? Color.black : outdoorBackground;
            sun.enabled = !inside;
            caveCamera.SetActive(inside);

            if (inside)
            {
                var ambient = new SphericalHarmonicsL2();
                ambient.AddAmbientLight(caveAmbient);
                RenderSettings.ambientProbe = ambient;
            }
            else
            {
                RenderSettings.ambientProbe = outdoorAmbient;
            }
        }
    }
}
