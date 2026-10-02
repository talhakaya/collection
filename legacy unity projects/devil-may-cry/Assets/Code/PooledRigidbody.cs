using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PooledRigidbody : MonoBehaviour
    {
        protected Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        protected void Init()
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            transform.eulerAngles = Vector3.zero;
        }
    }
}
