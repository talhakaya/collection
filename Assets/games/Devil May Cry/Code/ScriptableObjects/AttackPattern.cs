using UnityEngine;

namespace Games.DevilMayCry
{
    [CreateAssetMenu(fileName = "Data", menuName = "AttackPattern", order = 1)]
    public class AttackPattern : ScriptableObject
    {
        public Attack[] attacks;

        [System.Serializable]
        public class Attack
        {
            public Button button = Button.ATTACK0;
            public bool mode;
            public Actor.State state = Actor.State.Attack0;
            public float period = 0.25f;
            public float damage = 1f;
            public int squirt;
        }
    }
}
