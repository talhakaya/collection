using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Seizure
{
    public class BleedingTexture : MonoBehaviour
    {
        public Renderer quad;
        public Camera cam;
        public Camera camRt;
        public int height = 360;

        void Start()
        {
            OnScreenResolutionChanged();
        }

        private void OnScreenResolutionChanged()
        {
            int width = height * Screen.width / Screen.height;
            RenderTexture rt = new RenderTexture(width, height, 32);
            camRt.targetTexture = rt;
            quad.material.SetTexture("_BaseMap", rt);
        }

        private void Update()
        {
            float size = cam.orthographicSize * 2;
            quad.transform.localScale = new Vector3(size * Screen.width / Screen.height, size, 1f);
            camRt.orthographicSize = cam.orthographicSize;
        }
    }
}
