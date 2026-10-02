using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class DeadHandler : MonoBehaviour
    {
        public static DeadHandler inst;

        private void Start()
        {
            inst = this;
        }

        public void Show(Actor actor)
        {
            foreach (Renderer renderer in actor.GetRenderers())
            {
                GameObject dead = Pools.Get(PoolType.DeadCube);
                dead.GetComponent<Renderer>().material.SetColor("_MainColor", renderer.material.GetColor("_MainColor"));
                dead.transform.position = renderer.transform.position;
                dead.transform.localScale = renderer.transform.lossyScale;
                dead.transform.eulerAngles = renderer.transform.eulerAngles;
                Rigidbody deadBody = dead.GetComponent<Rigidbody>();
                const float maxSpeed = 10f;
                const float maxAngularSpeed = 200f;
                deadBody.velocity = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed));
                deadBody.angularVelocity = new Vector3(Random.Range(-maxAngularSpeed, maxAngularSpeed), Random.Range(-maxAngularSpeed, maxAngularSpeed), Random.Range(-maxAngularSpeed, maxAngularSpeed));
            }
        }
    }
}
