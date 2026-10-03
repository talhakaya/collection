using UnityEngine;

namespace Games.DevilMayCry
{
    [CreateAssetMenu(fileName = "Data", menuName = "PhysicsParams", order = 1)]
    public class PhysicsParams : ScriptableObject
    {
        public float gravityAcceleration;
        public float horAcceleration;
        public float horFriction;
        public int maxNumJumps;
        public float jumpInitial;
        public float jumpAcceleration;
        public Vector2 groundMin;
        public Vector2 groundMax;
        public Vector2 velocityMax;
        public int inputCacheLength;
        public float timeCancelAttackPattern;
        public float timeDodge;
        public float timeDodgeCooloff;
        public float speedDodge;
    }
}
