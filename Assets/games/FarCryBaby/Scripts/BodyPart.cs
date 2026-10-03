using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.FarCryBaby
{
    public class BodyPart : MonoBehaviour
    {
        public enum Part
        {
            Default,
            Limb,
            Head
        }

        public Part part;
    }
}
