using UnityEngine;

namespace Collection.Story
{
    // On each rigid body of a Bicycle: tells the bicycle when the part hits something.
    public class BicyclePart : MonoBehaviour
    {
        public Bicycle bicycle;

        void OnCollisionEnter(Collision collision)
        {
            if (bicycle != null)
                bicycle.Hit(collision);
        }
    }
}
