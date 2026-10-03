using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// In the collection: the Standard Assets image effects work through OnRenderImage, which
// URP never calls. This keeps the component (same file, so the scene's binding and its
// saved values survive) and hands the two values that matter to URP's own bloom, on a
// volume made at start.
namespace Games.WallStains.ImageEffects
{
    [RequireComponent(typeof (Camera))]
    public class Bloom : MonoBehaviour
    {
        public float bloomIntensity = 0.5f;
        public float bloomThreshold = 0.5f;
        public Color bloomThresholdColor = Color.white;
        public int bloomBlurIterations = 2;
        public float sepBlurSpread = 2.5f;

        private GameObject volumeObject;
        private VolumeProfile profile;

        private void Start()
        {
            UniversalAdditionalCameraData data = GetComponent<Camera>().GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;

            volumeObject = new GameObject("Bloom volume");
            Volume volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 10f;
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            UnityEngine.Rendering.Universal.Bloom bloom = profile.Add<UnityEngine.Rendering.Universal.Bloom>(true);
            bloom.threshold.value = bloomThreshold;
            bloom.intensity.value = bloomIntensity;
            bloom.scatter.value = 0.7f;
            bloom.tint.value = bloomThresholdColor;

            NoiseAndGrain grain = GetComponent<NoiseAndGrain>();
            if (grain != null && grain.enabled)
            {
                FilmGrain film = profile.Add<FilmGrain>(true);
                film.type.value = FilmGrainLookup.Medium1;
                film.intensity.value = Mathf.Clamp01(grain.intensityMultiplier * grain.generalIntensity * 0.6f);
                film.response.value = 0.8f;
            }
        }

        private void OnDestroy()
        {
            if (volumeObject != null)
            {
                Destroy(volumeObject);
            }
            if (profile != null)
            {
                Destroy(profile);
            }
        }
    }
}
