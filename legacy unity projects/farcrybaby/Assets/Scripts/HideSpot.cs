using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class HideSpot : MonoBehaviour
    {
        public bool needToCrouch;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == PlayerControl.inst.gameObject) PlayerControl.inst.hideSpots.Add(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == PlayerControl.inst.gameObject) PlayerControl.inst.hideSpots.Remove(this);
        }
    }
}
