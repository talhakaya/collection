using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.FarCryBaby
{
    public class MapView : MonoBehaviour
    {
        public static MapView inst;

        [HideInInspector] public float transitionTimer;
        [HideInInspector] public Transform player;
        [HideInInspector] public List<Transform> enemies;
        public float transitionPeriod;
        public Camera cam;
        public Unity.Cinemachine.CinemachineBrain cinemachineBrain;
        public Unity.Cinemachine.CinemachineFreeLook camFreeLook;
        public Unity.Cinemachine.CinemachineVirtualCamera camMapView;
        public float positionY;
        public AnimationCurve curveY;
        public Color playerIconColor;
        public Color enemyIconColor;
        public int cloudCount;
        public List<Sprite> cloudSprites;

        private List<SpriteRenderer> clouds;
        private Image playerIcon;
        private List<Image> enemyIcons;

        private const float iconAppearAnimRatio = 0.7f;

        private void Awake()
        {
            inst = this;
            enemies = new List<Transform>();
            enemyIcons = new List<Image>();
        }

        private void Start()
        {
            clouds = new List<SpriteRenderer>();
            for (int i = 0; i < cloudCount; i++)
            {
                GameObject newCloud = new GameObject($"Cloud{i}");
                clouds.Add(newCloud.AddComponent<SpriteRenderer>());
                newCloud.transform.position = new Vector3(Random.Range(-50, 50), Random.Range(30, 60), Random.Range(-50, 50));
                newCloud.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                clouds[i].sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];
                clouds[i].color = new Color(1f, 1f, 1f, 0f);
            }
            enemyIcons = new List<Image>();
        }

        public void UpdateDt(bool inputMapView, float deltaTime)
        {
            if (inputMapView)
            {
                transitionTimer = Mathf.Min(transitionPeriod, transitionTimer + deltaTime);
                camFreeLook.Priority = 10;
                camMapView.Priority = 11;
            }
            else
            {
                transitionTimer = Mathf.Max(0f, transitionTimer - deltaTime * transitionPeriod / cinemachineBrain.DefaultBlend.Time);
                camFreeLook.Priority = 11;
                camMapView.Priority = 10;
            }
            float animRatio = curveY.Evaluate(transitionTimer / transitionPeriod);
            float y = positionY * animRatio;
            transform.position = new Vector3(0f, y, 0f);
            foreach (SpriteRenderer cloud in clouds)
            {
                cloud.color = new Color(1f, 1f, 1f, 0.3f * animRatio * cloud.transform.position.y / positionY);
                cloud.transform.position -= Vector3.forward * deltaTime * 10f;
                if (cloud.transform.position.z <= -50f)
                {
                    cloud.transform.position = new Vector3(Random.Range(-50, 50), cloud.transform.position.y, cloud.transform.position.z + 100f);
                }
            }
            if (animRatio >= iconAppearAnimRatio)
            {
                float iconAlpha = (animRatio - iconAppearAnimRatio) / (1f - iconAppearAnimRatio);
                if (player != null)
                {
                    if (playerIcon == null) playerIcon = GetMapIcon(playerIconColor);
                    SetIconAlpha(playerIcon, iconAlpha);
                    SetIconPosition(playerIcon, player);
                }
                else
                {
                    if (playerIcon != null)
                    {
                        playerIcon.gameObject.SetActive(false);
                        playerIcon = null;
                    }
                }

                if (enemies.Count == enemyIcons.Count)
                {
                    for (int i = 0, len = enemies.Count; i < len; i++)
                    {
                        SetIconAlpha(enemyIcons[i], iconAlpha);
                        SetIconPosition(enemyIcons[i], enemies[i]);
                    }
                }
                else
                {
                    if (enemyIcons.Count > 0)
                    {
                        for (int i = 0, len = enemyIcons.Count; i < len; i++)
                        {
                            enemyIcons[i].gameObject.SetActive(false);
                        }
                        enemyIcons.Clear();
                    }
                    for (int i = 0, len = enemies.Count; i < len; i++)
                    {
                        enemyIcons.Add(GetMapIcon(enemyIconColor));
                        SetIconAlpha(enemyIcons[i], iconAlpha);
                        SetIconPosition(enemyIcons[i], enemies[i]);
                    }
                }
            }
            else
            {
                if (playerIcon != null)
                {
                    playerIcon.gameObject.SetActive(false);
                    playerIcon = null;
                }
                if (enemyIcons.Count > 0)
                {
                    for (int i = 0, len = enemyIcons.Count; i < len; i++)
                    {
                        enemyIcons[i].gameObject.SetActive(false);
                    }
                    enemyIcons.Clear();
                }
            }
        }

        private Image GetMapIcon(Color c)
        {
            Image i = Pools.Get(PoolType.MapIcon).GetComponent<Image>();
            i.color = c;
            return i;
        }

        private void SetIconPosition(Image i, Transform o)
        {
            i.rectTransform.position = cam.WorldToScreenPoint(o.position);
        }

        private void SetIconAlpha(Image i, float alpha)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, alpha);
        }
    }
}
