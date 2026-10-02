using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class Weapon : MonoBehaviour
    {
        public PoolType bulletType;
        public float firePeriod = 0.3f;
        public Transform firePosition;

        private float fireTimer;

        private void Update()
        {
            if (fireTimer < firePeriod) fireTimer += Time.deltaTime;
        }

        public void TryFire(Vector3 lookAt, bool byPlayer)
        {
            if (fireTimer < firePeriod) return;
            fireTimer = 0f;
            Bullet bullet = Pools.Get(bulletType).GetComponent<Bullet>();
            bullet.transform.position = firePosition.position;
            bullet.transform.LookAt(lookAt);
            bullet.Shoot(byPlayer);
        }
    }
}
