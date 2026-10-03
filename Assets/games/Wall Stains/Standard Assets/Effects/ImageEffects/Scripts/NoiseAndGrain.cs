using UnityEngine;

// In the collection: only the saved values are kept; Bloom.cs turns them into URP's film
// grain (see the comment there).
namespace Games.WallStains.ImageEffects
{
    [RequireComponent(typeof (Camera))]
    public class NoiseAndGrain : MonoBehaviour
    {
        public float intensityMultiplier = 0.25f;
        public float generalIntensity = 0.5f;
        public float blackIntensity = 1.0f;
        public float whiteIntensity = 1.0f;
        public float midGrey = 0.2f;
        public float softness = 0.0f;
        public bool monochrome = false;

        private void Start()
        {
        }
    }
}
