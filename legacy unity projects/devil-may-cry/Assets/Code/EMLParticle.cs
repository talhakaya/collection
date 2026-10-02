using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class EMLParticle : PooledRigidbody
    {
        public void Init(Vector2 pos, Vector2 dir)
        {
            Init();
            body.position = pos;
            Vector2 vel = dir * 10f;
            vel += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            body.velocity = vel;
        }
    }
}
