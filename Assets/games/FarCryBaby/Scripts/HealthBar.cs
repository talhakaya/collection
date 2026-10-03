using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.FarCryBaby
{
    public class HealthBar : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Image red;
        public Image yellow;
        public float maxDist;
        public AnimationCurve distScaleCurve;

        private RectTransform rectTransform;
        private float animTimer;
        private float realRatio;
        private Color yellowColor;
        private Vector3 worldPos;
        private bool updatedWorldPos;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            realRatio = 1f;
            yellowColor = yellow.color;
            SetHealth(realRatio);
        }

        private void Update()
        {
            if (animTimer > 0f)
            {
                animTimer -= Time.deltaTime;
                if (animTimer > 0.3f) yellow.color = Color.white;
                else if (animTimer > 0.2f) yellow.color = Color.Lerp(yellowColor, Color.white, (animTimer - 0.2f) / 0.1f);
                else yellow.color = yellowColor;
            }
            else if (yellow.transform.localScale.x > 0f)
            {
                yellow.transform.localScale = new Vector3(Mathf.Clamp01(yellow.transform.localScale.x - 2f * Time.deltaTime), 1f, 1f);
            }
        }

        private void LateUpdate()
        {
            if (updatedWorldPos) updatedWorldPos = false;
            else SetTransform();
        }

        public void SetHealth(float ratio)
        {
            canvasGroup.alpha = ratio >= 0.99f ? 0f : 1f;
            float deltaRatio = Mathf.Max(0f, realRatio - ratio);
            realRatio = ratio;
            red.transform.localScale = new Vector3(realRatio, 1f, 1f);
            yellow.rectTransform.anchoredPosition = new Vector2(yellow.rectTransform.sizeDelta.x * ratio, 0f);
            yellow.transform.localScale += new Vector3(deltaRatio, 0f, 0f);
            animTimer = 0.4f;
        }

        public void SetWorldPos(Vector3 worldPos)
        {
            this.worldPos = worldPos;
            updatedWorldPos = true;
            SetTransform();
        }

        private void SetTransform()
        {
            rectTransform.position = PlayerControl.inst.cam.WorldToScreenPoint(worldPos);
            float dist = (PlayerControl.inst.cam.transform.position - worldPos).magnitude;
            rectTransform.localScale = Vector3.one * distScaleCurve.Evaluate(Mathf.Clamp01(dist / maxDist));
        }
    }
}
