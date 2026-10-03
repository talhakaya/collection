using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.FarCryBaby
{
    public class Bullet : MonoBehaviour
    {
        public LineRenderer line;
        public LayerMask playerBodyLayer;
        public LayerMask enemyBodyLayer;
        public float periodFadeOut = 0.5f;
        public Color color;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= periodFadeOut) gameObject.SetActive(false);
            else
            {
                Color c = new Color(color.r, color.g, color.b, (periodFadeOut - timer) / periodFadeOut);
                line.startColor = c;
                line.endColor = c;
            }
        }

        public void Shoot(bool byPlayer)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            bool cast = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, byPlayer ? enemyBodyLayer : playerBodyLayer, QueryTriggerInteraction.Ignore);
            Vector3 endPos;
            if (cast)
            {
                hitInfo.transform.GetComponent<Actor>().GetHit(hitInfo.collider.GetComponent<BodyPart>());
                endPos = transform.position + transform.forward * hitInfo.distance;
            }
            else
            {
                endPos = transform.position + transform.forward * 100f;
            }
            timer = 0f;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, endPos);
            line.startColor = color;
            line.endColor = color;
        }
    }
}
