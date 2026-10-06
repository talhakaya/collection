using UnityEngine;

namespace Collection.Story
{
    // A ramp that throws a bicycle into the air: ridden up the way its arrows point, the bicycle leaves the top
    // going at least `speed`, with `up` more upward than the slope alone would give it, and flies for a bit.
    //
    // This is on the trigger at the ramp's top end, turned as the ramp is: its own forward is the way the arrows
    // point. A bicycle coming the wrong way, or across, or with nobody on it, or too slowly, is just a bicycle on
    // a slope.
    [RequireComponent(typeof(Collider))]
    public class BoostRamp : MonoBehaviour
    {
        [Tooltip("The speed along the ground the bicycle leaves with, at least (m/s), and the speed upward (m/s).")]
        public float speed = 17f;
        public float up = 6f;
        [Tooltip("How far off the arrows' way the bicycle may be going (degrees), and how fast it must be going at least (m/s).")]
        public float within = 50f;
        public float least = 3f;

        float nextAt;

        void OnTriggerEnter(Collider other)
        {
            // (Each of the bicycle's parts comes through; the first is the one that counts.)
            if (Time.time < nextAt)
                return;
            Bicycle bicycle = other.GetComponentInParent<Bicycle>();
            if (bicycle == null || !bicycle.Ridden)
                return;

            Vector3 way = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 going = Vector3.ProjectOnPlane(bicycle.frame.linearVelocity, Vector3.up);
            if (going.magnitude < least || Vector3.Angle(going, way) > within)
                return;

            nextAt = Time.time + 1.5f;
            bicycle.Launch(way * Mathf.Max(speed, going.magnitude) + Vector3.up * up);
        }
    }
}
