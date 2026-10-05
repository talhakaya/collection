using UnityEngine;

namespace Collection.Story
{
    // Fake cape physics: hinged segments flare back as the character moves, sway a little sideways when strafing or
    // turning, and settle with a little overshoot, plus a slight idle breeze. Cheap and stable (no physics engine):
    // each hinge is a damped spring toward a target angle, and lower segments react more slowly, so the cape trails.
    // The top of the cape takes the flare while the tail sags back toward hanging, as if pulled down by gravity.
    public class CapeSway : MonoBehaviour
    {
        [Tooltip("Hinges from top to bottom; each is the parent of the next.")]
        public Transform[] segments;
        [Tooltip("How far back the cape angles when standing still (degrees).")]
        public float restPitch = 6f;
        [Tooltip("Extra backward flare per m/s of forward speed (degrees).")]
        public float pitchPerSpeed = 10f;
        public float maxPitch = 75f;
        [Tooltip("How much the tail sags back toward hanging down (0 = the whole cape flares as one plane, " +
                 "1 = the very end hangs straight down).")]
        [Range(0f, 1f)] public float tailDroop = 0.65f;
        [Tooltip("How the sag spreads along the cape: higher keeps more of the cape flared and bends mostly near the end.")]
        public float droopCurve = 1.5f;
        [Tooltip("Sideways sway per m/s of sideways speed (degrees).")]
        public float rollPerSpeed = 3f;
        [Tooltip("Sideways sway per degree/s of turning.")]
        public float rollPerTurn = 0.02f;
        public float maxRoll = 12f;
        public float stiffness = 70f;
        public float damping = 9f;
        [Tooltip("How much slower each lower segment reacts than the one above (0..1).")]
        [Range(0f, 0.8f)] public float lag = 0.3f;
        [Tooltip("Gentle idle movement (degrees).")]
        public float breeze = 1.5f;

        [Header("Trail")]
        [Tooltip("Length multiplier at full speed (2 = twice as long), for an exaggerated trail.")]
        public float maxStretch = 2f;
        [Tooltip("Speed (m/s) at which the cape reaches its full stretch.")]
        public float stretchAtSpeed = 5.7f;
        [Tooltip("How quickly the length follows the speed (higher = faster).")]
        public float stretchResponse = 3f;

        Transform body;
        Vector3 lastPosition;
        float lastYaw;
        Vector3 localVelocity;
        float turnRate;
        float lean;
        float stretch = 1f;
        float[] pitch, pitchVelocity, roll, rollVelocity;
        Quaternion[] restRotations;
        Vector3[] restHingePositions;
        Transform[] cloths;
        Vector3[] restClothPositions, restClothScales;

        void OnEnable()
        {
            var appearance = GetComponentInParent<CharacterAppearance>();
            body = appearance != null ? appearance.transform : transform.root;
            lastPosition = transform.position;
            lastYaw = body.eulerAngles.y;
            int n = segments.Length;
            pitch = new float[n];
            pitchVelocity = new float[n];
            roll = new float[n];
            rollVelocity = new float[n];
            restRotations = new Quaternion[n];
            restHingePositions = new Vector3[n];
            cloths = new Transform[n];
            restClothPositions = new Vector3[n];
            restClothScales = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                restRotations[i] = segments[i].localRotation;
                restHingePositions[i] = segments[i].localPosition;
                // Each hinge's cloth is its first child with a renderer.
                foreach (Transform child in segments[i])
                {
                    if (child.GetComponent<Renderer>() == null)
                        continue;
                    cloths[i] = child;
                    restClothPositions[i] = child.localPosition;
                    restClothScales[i] = child.localScale;
                    break;
                }
                pitch[i] = HingeTarget(i, restPitch);
            }
            stretch = 1f;
        }

        void LateUpdate()
        {
            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            if (dt <= 0f)
                return;

            // Movement of the cape's attachment point, in the character's frame (x = right, z = forward).
            Vector3 velocity = (transform.position - lastPosition) / dt;
            lastPosition = transform.position;
            float blend = 1f - Mathf.Exp(-10f * dt);
            localVelocity = Vector3.Lerp(localVelocity, body.InverseTransformDirection(velocity), blend);
            float yaw = body.eulerAngles.y;
            turnRate = Mathf.Lerp(turnRate, Mathf.DeltaAngle(lastYaw, yaw) / dt, blend);
            lastYaw = yaw;

            // Moving forward flares the cape back; moving backwards lets it hang closer.
            float forward = localVelocity.z;
            float flare = Mathf.Clamp(restPitch + Mathf.Max(0f, forward) * pitchPerSpeed + Mathf.Min(0f, forward) * 2f, 0f, maxPitch);
            // Strafing right or turning right swings the bottom to the left (negative roll), and vice versa.
            float sway = Mathf.Clamp(-localVelocity.x * rollPerSpeed - turnRate * rollPerTurn, -maxRoll, maxRoll);

            // The cape hangs from the chest, which leans forward when running. Measure that lean (how far the
            // attachment's "down" already points backward) so the cape's angles are taken from true vertical.
            Vector3 down = body.InverseTransformDirection(-transform.up);
            float currentLean = Mathf.Atan2(-down.z, -down.y) * Mathf.Rad2Deg;
            lean = Mathf.Lerp(lean, currentLean, blend);

            // Stretch the cape along its length with speed, easing in and out, for a long trail when running.
            float speed = new Vector2(localVelocity.x, localVelocity.z).magnitude;
            float targetStretch = Mathf.Lerp(1f, maxStretch, Mathf.Clamp01(speed / stretchAtSpeed));
            stretch = Mathf.Lerp(stretch, targetStretch, 1f - Mathf.Exp(-stretchResponse * dt));

            float time = Time.time;
            float k = stiffness;
            for (int i = 0; i < segments.Length; i++)
            {
                // Lengthen along the hanging direction (local y): move lower hinges down and stretch each cloth.
                if (i > 0)
                    segments[i].localPosition = new Vector3(restHingePositions[i].x, restHingePositions[i].y * stretch, restHingePositions[i].z);
                if (cloths[i] != null)
                {
                    cloths[i].localPosition = new Vector3(restClothPositions[i].x, restClothPositions[i].y * stretch, restClothPositions[i].z);
                    cloths[i].localScale = new Vector3(restClothScales[i].x, restClothScales[i].y * stretch, restClothScales[i].z);
                }

                float idle = Mathf.Sin(time * 1.3f + i * 0.9f) * breeze;
                float target = HingeTarget(i, flare) - (i == 0 ? lean : 0f);
                Spring(ref pitch[i], ref pitchVelocity[i], target + idle, k, dt);
                Spring(ref roll[i], ref rollVelocity[i], sway / segments.Length, k, dt);
                segments[i].localRotation = restRotations[i] * Quaternion.Euler(pitch[i], 0f, roll[i]);
                k *= 1f - lag;
            }
        }

        void Spring(ref float angle, ref float speed, float target, float k, float dt)
        {
            speed += ((target - angle) * k - speed * damping) * dt;
            angle += speed * dt;
        }

        // Hinge angles are relative to the segment above, so each hinge bends by the change in the cape's angle
        // (from vertical) between neighbouring segments.
        float HingeTarget(int i, float flare)
        {
            return SegmentAngle(i, flare) - (i > 0 ? SegmentAngle(i - 1, flare) : 0f);
        }

        // Angle of segment i from hanging straight down: the top segment takes the full flare, and lower segments sag
        // back toward vertical, most strongly near the end.
        float SegmentAngle(int i, float flare)
        {
            float along = segments.Length > 1 ? (float)i / (segments.Length - 1) : 0f;
            return flare * (1f - tailDroop * Mathf.Pow(along, droopCurve));
        }
    }
}
