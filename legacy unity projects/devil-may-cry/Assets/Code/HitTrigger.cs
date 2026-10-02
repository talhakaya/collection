using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class HitTrigger : MonoBehaviour
    {
        public bool shouldDisableOnHit;

        private static int layerEnemy = -1;
        private static int layerEnemyHit = -1;
        private static int layerPlayer = -1;
        private static int layerPlayerHit = -1;
        private bool isEnemy;
        private Actor actor;
        private float id;

        void Awake()
        {
            if (layerEnemy == -1) layerEnemy = LayerMask.NameToLayer("Enemy");
            if (layerEnemyHit == -1) layerEnemyHit = LayerMask.NameToLayer("EnemyHit");
            if (layerPlayer == -1) layerPlayer = LayerMask.NameToLayer("Player");
            if (layerPlayerHit == -1) layerPlayerHit = LayerMask.NameToLayer("PlayerHit");
            isEnemy = (gameObject.layer == layerEnemyHit);
            if (!isEnemy && gameObject.layer != layerPlayerHit)
                Debug.LogError($"HitTrigger layer doesn't make sense! {name} of parent {(transform.parent == null ? "null" : transform.parent.name)}");
            else if (isEnemy && transform.parent != null && transform.parent.gameObject.layer != layerEnemy)
                Debug.LogError($"HitTrigger layer is wrong! {name} of parent {(transform.parent == null ? "null" : transform.parent.name)}");
            else if (!isEnemy && transform.parent != null && transform.parent.gameObject.layer != layerPlayer)
                Debug.LogError($"HitTrigger layer is wrong! {name} of parent {(transform.parent == null ? "null" : transform.parent.name)}");
            Transform t = transform;
            while (actor == null && t != null)
            {
                if (t.GetComponent<Actor>() != null)
                {
                    actor = t.GetComponent<Actor>();
                    break;
                }
                t = t.parent;
            }
        }

        private void OnEnable()
        {
            id = Random.value;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool shouldHit = (collision.gameObject.layer == layerPlayer && isEnemy);
            if (!shouldHit)
            {
                if (collision.gameObject.layer == layerEnemy && !isEnemy)
                    shouldHit = true;
            }
            if (shouldHit)
            {
                Transform t = collision.transform;
                while (t != null)
                {
                    if (t.GetComponent<Actor>() != null)
                    {
                        actor.Glow();
                        t.GetComponent<Actor>().GetHitBy(actor, id, transform.position);
                        if (shouldDisableOnHit) gameObject.SetActive(false);
                        break;
                    }
                    t = t.parent;
                }
            }
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;
        }
    }
}
