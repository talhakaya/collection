using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.DevilMayCry
{
    public class Particle : MonoBehaviour
    {
        public string anim;

        public CinemachineImpulseSource cinemachineImpulseSource;

        private void OnEnable()
        {
            GetComponent<Animator>().Play(anim);
            if (cinemachineImpulseSource != null) cinemachineImpulseSource.GenerateImpulse();
        }
    }
}
