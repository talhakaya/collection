using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class EnemyAttackTrigger : MonoBehaviour
    {
        private Enemy parent;

        private void Awake()
        {
            Transform t = transform.parent;
            while (t != null)
            {
                if (t.GetComponent<Enemy>() != null)
                {
                    parent = t.GetComponent<Enemy>();
                    break;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnTrigger(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            OnTrigger(collision);
        }

        private void OnTrigger(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                parent.OnAttackTrigger();
        }
    }
}
