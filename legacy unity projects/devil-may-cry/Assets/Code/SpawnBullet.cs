using UnityEngine;

namespace Game2
{
    public class SpawnBullet : MonoBehaviour
    {
        private Actor actor;

        private void OnEnable()
        {
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
            GameObject bullet = Pool.Bullet.Get();
            bullet.GetComponent<HitTrigger>().SetActor(actor);
            bullet.transform.position = transform.position;
            Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
            Vector2 dir = (Main.inst.player.trans.position + new Vector3(0f, 2f, 0f)) - transform.position;
            body.velocity = dir.normalized * 10f;
        }
    }
}
