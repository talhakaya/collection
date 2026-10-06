using UnityEngine;

namespace Collection.Story
{
    // A bicycle to ride.
    //
    // It is four rigid bodies held together by hinges: the frame; the fork, which turns in the frame (the
    // steering); the front wheel, which spins in the fork; the back wheel, which spins in the frame and is the
    // one that is driven. The wheels roll on the ground by their own friction, so the bicycle goes where its
    // front wheel points and leans, skids and bounces as the ground has it. What is not left to the physics is
    // staying up: while someone is riding, the frame is turned about its own length toward upright (leaned into
    // the bend it is taking), which is the rider's balance.
    //
    // Riding. The controls are the walking ones: the stick says which way to go, as seen by the camera, not
    // "left" and "right" of the bicycle. The handlebars turn toward that way, more sharply the slower the
    // bicycle is going, and the bicycle comes round as it rolls. Speed: slow while it has far to turn (it brakes
    // for a sharp change of mind), and faster and faster the longer it is kept going the same way. Roll (Space, pad B) gets off.
    //
    // Getting on (the NPCTrigger on the frame, onActivate -> Mount): the bicycle stands up where it is, facing
    // the way the character faces, and the character is sat on it. The character's own movement is switched off;
    // it is put on the saddle every frame and posed there, in code, over whatever its Animator plays: bent
    // forward, arms out to the handlebars, legs going round with the pedals at the bicycle's speed.
    //
    // Crashing: hitting something hard, or going over too far, throws the rider off as a ragdoll (Ragdoll) and
    // sends the bicycle flying.
    //
    // Until it is first ridden it stands where it was put, not simulated.
    public class Bicycle : MonoBehaviour
    {
        [Header("Parts")]
        public Rigidbody frame;
        public Rigidbody fork;
        public Rigidbody frontWheel;
        public Rigidbody backWheel;
        [Tooltip("The fork's hinge in the frame.")]
        public HingeJoint steering;
        [Tooltip("The back wheel's hinge in the frame.")]
        public HingeJoint drive;
        [Tooltip("Where the rider's hips go.")]
        public Transform saddle;
        [Tooltip("Turned with the pedalling.")]
        public Transform crank;
        [Tooltip("The middle of the handlebars, on the fork: where the rider's hands go, `gripHalfWidth` to either side. Empty: the hands are left where the pose has them.")]
        public Transform grips;
        public float gripHalfWidth = 0.22f;
        [Tooltip("Stands in for the rider's body while riding, so that it is the rider that hits things.")]
        public Collider riderBody;
        [Tooltip("The trigger for getting on.")]
        public NPCTrigger mount;
        public float wheelRadius = 0.34f;

        [Header("Steering")]
        [Tooltip("The furthest the handlebars turn (degrees).")]
        public float steerSlow = 42f;
        [Tooltip("The hardest bend it takes (m/s² sideways): the faster it goes, the less the handlebars turn, to keep within this. More, and it skids round.")]
        public float grip = 5f;
        [Tooltip("How much of the angle to the way wanted the handlebars take.")]
        public float steerSharpness = 0.6f;
        [Tooltip("How much the handlebars come back for the frame's own turning (degrees per degree/s).")]
        public float steerEasing = 0.22f;
        [Tooltip("How fast the handlebars turn (degrees/s): slow, and at top speed.")]
        public float steerRate = 160f;
        public float steerRateFast = 14f;
        [Tooltip("A turn of the frame itself toward the way wanted (degrees/s² at a right angle off), so that it comes round even from standing.")]
        public float turnHelp = 140f;
        [Tooltip("The speed by which that help is gone (m/s).")]
        public float turnHelpBelow = 4f;

        [Header("Speed (m/s)")]
        [Tooltip("While it has far to turn.")]
        public float turningSpeed = 2.5f;
        [Tooltip("Going straight: at first, and after `buildTime` seconds of going the same way.")]
        public float startSpeed = 4.5f;
        public float topSpeed = 13f;
        public float buildTime = 6f;
        [Tooltip("Degrees off the way wanted within which it counts as going the same way, and beyond which as turning.")]
        public float straightWithin = 18f;
        public float turningBeyond = 70f;
        [Tooltip("How hard the back wheel is driven.")]
        public float driveForce = 90f;

        [Header("Balance")]
        public float balance = 420f;
        public float balanceDamping = 36f;
        [Tooltip("The most it leans into a bend (degrees).")]
        public float mostLean = 34f;

        [Header("Crashing")]
        [Tooltip("Hitting something at this speed or more is a crash (m/s).")]
        public float crashSpeed = 4f;
        [Tooltip("Over this far from upright is a crash (degrees).")]
        public float crashLean = 62f;
        [Tooltip("How hard the bicycle is thrown (m/s).")]
        public float crashThrow = 5f;

        [Header("The rider")]
        [Tooltip("The rider's weight on the frame (kg).")]
        public float riderMass = 45f;
        [Tooltip("Turns of the pedals for one of the wheel.")]
        public float gear = 0.45f;

        // The way to go, set by something other than the player's stick (a cutscene, a test). Zero: the stick.
        [System.NonSerialized] public Vector3 overrideDirection;
        // Set to have the ride written down, a line a physics step, for tuning.
        [System.NonSerialized] public System.Text.StringBuilder log;

        public bool Ridden => rider != null;
        public float Speed => Vector3.Dot(frame.linearVelocity, frame.transform.forward);

        Rigidbody[] bodies;
        Vector3[] restPlaces;
        Quaternion[] restTurns;
        float frameMass;
        Vector3 wheelInFork;
        Vector3 gripsInFrame;
        Transform frontWheelLook;
        float frontWheelTurn;
        bool simulated;

        Transform rider;
        Animator riderAnimator;
        PlayerMovement riderMovement;
        PlayerControl riderControl;
        PlayerRoll riderRoll;
        Ragdoll riderRagdoll;
        Vector3 riderModelPlace;
        Quaternion riderModelFacing;
        float straightFor;

        // The parts of the rider held still against the frame, parents before children, and how each is turned
        // as seen from the frame. Null until the pose has been made.
        static readonly HumanBodyBones[] Held =
        {
            HumanBodyBones.Hips, HumanBodyBones.Spine, HumanBodyBones.Chest, HumanBodyBones.UpperChest,
            HumanBodyBones.LeftShoulder, HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm, HumanBodyBones.RightUpperArm,
            HumanBodyBones.LeftLowerArm, HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftHand, HumanBodyBones.RightHand,
        };
        Quaternion[] heldTurns;
        float leanNow;
        float steerNow;
        float pedals;
        float mountedAt;

        void Awake()
        {
            bodies = new[] { frame, fork, frontWheel, backWheel };
            restPlaces = new Vector3[bodies.Length];
            restTurns = new Quaternion[bodies.Length];
            for (int i = 0; i < bodies.Length; i++)
            {
                // Where each part is, as seen from the frame, with the bicycle as it was made: upright, straight.
                restPlaces[i] = frame.transform.InverseTransformPoint(bodies[i].transform.position);
                restTurns[i] = Quaternion.Inverse(frame.transform.rotation) * bodies[i].transform.rotation;
                bodies[i].maxAngularVelocity = 120f;
                // Light parts with next to no resistance to being turned shake in their joints; each is given
                // at least a fair one.
                Vector3 inertia = bodies[i].inertiaTensor;
                float least = bodies[i].mass * 0.06f;
                bodies[i].inertiaTensor = new Vector3(Mathf.Max(inertia.x, least), Mathf.Max(inertia.y, least), Mathf.Max(inertia.z, least));
                bodies[i].solverIterations = 30;
                bodies[i].solverVelocityIterations = 8;
                // Standing where it was put, and going with whatever moves it there (the land coming up), until
                // it is first ridden.
                bodies[i].isKinematic = true;
                bodies[i].interpolation = RigidbodyInterpolation.None;
            }

            // The parts do not get in one another's way.
            Collider[] all = GetComponentsInChildren<Collider>(true);
            foreach (Collider a in all)
                foreach (Collider b in all)
                    if (a != b && !a.isTrigger && !b.isTrigger)
                        Physics.IgnoreCollision(a, b);

            wheelInFork = fork.transform.InverseTransformPoint(frontWheel.transform.position);
            if (grips != null)
                gripsInFrame = frame.transform.InverseTransformPoint(grips.position);

            // What is seen of the front wheel is carried by the fork, exactly where it belongs in it, and only
            // turned as the wheel's rigid body turns: under a hard bend the body itself gives a little in its
            // hinge, and a wheel seen leaving its fork looks broken.
            frontWheelLook = new GameObject("Front Wheel Look").transform;
            frontWheelLook.SetParent(fork.transform, false);
            frontWheelLook.localPosition = wheelInFork;
            frontWheelLook.localRotation = Quaternion.identity;
            for (int i = frontWheel.transform.childCount - 1; i >= 0; i--)
                frontWheel.transform.GetChild(i).SetParent(frontWheelLook, false);
            frameMass = frame.mass;
            frame.centerOfMass = new Vector3(0f, 0.45f, 0f);
            if (riderBody != null)
                riderBody.enabled = false;
        }

        // ---- Getting on and off -------------------------------------------------------------------------------

        // For the NPCTrigger's onActivate.
        public void Mount()
        {
            if (Ridden)
                return;
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player == null)
                return;
            // Not one that something else has: rolling, lying after a fall, in a cutscene.
            PlayerControl control = PlayerControl.Of(player);
            if (!control.Free)
                return;
            // Nor one still getting up from a fall: its body is on its way from lying to standing, and the pose
            // it is given on the bicycle is made from the body as it is at that moment.
            Ragdoll fallen = player.GetComponent<Ragdoll>();
            if (fallen != null && fallen.GettingUp)
                return;

            rider = player.transform;
            riderMovement = player;
            riderControl = control;
            riderRoll = rider.GetComponent<PlayerRoll>();
            riderRagdoll = rider.GetComponent<Ragdoll>();
            riderAnimator = rider.GetComponentInChildren<Animator>();
            riderModelPlace = riderAnimator.transform.localPosition;
            riderModelFacing = riderAnimator.transform.localRotation;

            // Up on its wheels where it is, facing the way the rider faces.
            Vector3 place = frame.position;
            place.y = GroundUnder(place, rider.position.y - 1f) + 0.02f;
            Stand(place, Quaternion.Euler(0f, rider.eulerAngles.y, 0f));

            riderControl.Take(this);
            if (mount != null)
                mount.gameObject.SetActive(false);
            if (riderBody != null)
                riderBody.enabled = true;
            frame.mass = frameMass + riderMass;
            straightFor = 0f;
            heldTurns = null;
            mountedAt = Time.time;
        }

        // The height of the ground under a place: the first thing below it, the bicycle and its rider not
        // counting (Ground). `otherwise` when there is nothing.
        float GroundUnder(Vector3 place, float otherwise)
        {
            float ground;
            Transform who = riderMovement != null ? riderMovement.transform : null;
            return Ground.Under(place + Vector3.up * 0.7f, 40f, ~0, out ground, transform, who) ? ground : otherwise;
        }

        // Every part where it belongs for a bicycle standing at `place`, turned `facing`, and still.
        void Stand(Vector3 place, Quaternion facing)
        {
            simulated = true;
            for (int i = 0; i < bodies.Length; i++)
            {
                Rigidbody body = bodies[i];
                body.isKinematic = false;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                Vector3 at = place + facing * restPlaces[i];
                Quaternion turn = facing * restTurns[i];
                body.transform.SetPositionAndRotation(at, turn);
                body.position = at;
                body.rotation = turn;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            steerNow = 0f;
            SetSteering(0f);
            SetDrive(0f, false);
        }

        // The rider off the bicycle, which is no longer balanced. `mountAgain`: the trigger for getting on is back.
        void Release()
        {
            rider = null;
            frame.mass = frameMass;
            if (riderBody != null)
                riderBody.enabled = false;
            SetDrive(0f, false);
            SetSteering(0f);
            if (mount != null)
                mount.gameObject.SetActive(true);
        }

        void Dismount()
        {
            Vector3 beside = frame.position - frame.transform.right * 0.7f;
            Vector3 velocity = frame.linearVelocity;
            riderAnimator.transform.localPosition = riderModelPlace;
            riderAnimator.transform.localRotation = riderModelFacing;
            Release();

            beside.y = GroundUnder(beside, beside.y - 1f);
            beside.y += riderControl.Standing;
            riderControl.MoveTo(beside, Quaternion.Euler(0f, frame.transform.eulerAngles.y, 0f));
            riderMovement.Velocity = velocity;
            // The press that got the rider off is the roll's button, and is not a roll as well.
            if (riderRoll != null)
                riderRoll.Wait(0.25f);
            riderControl.Release(this);
        }

        // Called by the parts when they hit something (BicyclePart).
        public void Hit(Collision collision)
        {
            if (!Ridden || Time.time < mountedAt + 0.5f)
                return;
            // The ground under the wheels is not something hit, however hard the landing.
            Vector3 normal = collision.GetContact(0).normal;
            if (normal.y > 0.6f)
                return;
            if (collision.relativeVelocity.magnitude >= crashSpeed)
                Crash();
        }

        void Crash()
        {
            Ragdoll ragdoll = riderRagdoll;
            Vector3 velocity = frame.linearVelocity;
            Vector3 modelPlace = riderModelPlace;
            Quaternion modelFacing = riderModelFacing;
            Vector3 across = frame.transform.right;
            Release();

            // The rider on over the handlebars; the bicycle up and away, tumbling.
            if (ragdoll != null)
                ragdoll.Fall(velocity * 0.8f + Vector3.up * 2.5f, across * 3f, modelPlace, modelFacing);
            // The fall has the rider now (or, with no ragdoll to fall as, the player again).
            riderControl.Release(this);
            Vector3 away = (Random.insideUnitSphere + Vector3.up * 1.5f).normalized * crashThrow;
            foreach (Rigidbody body in bodies)
            {
                body.linearVelocity += away;
                body.angularVelocity += Random.insideUnitSphere * 6f;
            }
        }

        // ---- Riding -------------------------------------------------------------------------------------------

        void Update()
        {
            if (Ridden && Time.time > mountedAt + 0.3f && Main.inst.input.rollPressed)
                Dismount();
        }

        void FixedUpdate()
        {
            if (!simulated || !Ridden)
                return;

            Transform body = frame.transform;
            Vector3 forward = Vector3.ProjectOnPlane(body.forward, Vector3.up).normalized;
            float speed = Speed;

            // The way wanted, along the ground.
            Vector3 wanted = overrideDirection;
            if (wanted == Vector3.zero)
            {
                Vector2 stick = Main.inst.input.move;
                if (stick.magnitude > 0.2f)
                    wanted = PlayerMovement.CameraRelative(stick);
            }
            wanted.y = 0f;
            bool going = wanted.sqrMagnitude > 0.01f;
            float off = going ? Vector3.SignedAngle(forward, wanted.normalized, Vector3.up) : 0f;

            // Handlebars: toward the way wanted, less far the faster it goes.
            const float wheelbase = 1.05f;
            float most = Mathf.Min(steerSlow, Mathf.Atan(grip * wheelbase / Mathf.Max(0.5f, speed * speed)) * Mathf.Rad2Deg);
            // Less the faster the frame is already coming round, so that it straightens up in time and does not
            // swing past.
            float turning = frame.angularVelocity.y * Mathf.Rad2Deg;
            // And the handlebars themselves only turn so fast, so they do not flick from side to side with every
            // twitch of the frame.
            float steer = Mathf.Clamp(off * steerSharpness - turning * steerEasing, -most, most);
            // Slower still at speed: the front wheel grips, and a bend taken before the frame has leaned into it
            // throws the bicycle over outward. (Which is how it falls when it is turned too hard, too fast.)
            float rate = Mathf.Lerp(steerRate, steerRateFast, Mathf.InverseLerp(2f, topSpeed, Mathf.Abs(speed)));
            steerNow = Mathf.MoveTowards(steerNow, steer, rate * Time.fixedDeltaTime);
            steer = steerNow;
            SetSteering(steer);

            // Speed: slow while there is far to turn; from startSpeed up to topSpeed the longer it goes one way.
            if (going && Mathf.Abs(off) < straightWithin)
                straightFor += Time.fixedDeltaTime;
            else
                straightFor = Mathf.Max(0f, straightFor - Time.fixedDeltaTime * (going ? 3f : 1.5f));
            float straight = Mathf.Lerp(startSpeed, topSpeed, Mathf.Clamp01(straightFor / buildTime));
            float target = Mathf.Lerp(straight, turningSpeed, Mathf.InverseLerp(straightWithin, turningBeyond, Mathf.Abs(off)));
            // Faster than that, it brakes: the wheel is held to the speed, not left to roll.
            SetDrive(target, going, speed > target + 0.7f);

            // A turn of the frame itself, so that it comes round from standing and in tight places.
            // Only while slow: at speed the front wheel does it.
            float help = 1f - Mathf.InverseLerp(1f, turnHelpBelow, Mathf.Abs(speed));
            if (going && help > 0f)
            {
                float turn = Mathf.Clamp(off / 90f, -1f, 1f) * turnHelp * Mathf.Deg2Rad;
                frame.AddTorque(Vector3.up * ((turn - frame.angularVelocity.y * 3f) * help), ForceMode.Acceleration);
            }

            // Balance: about the frame's own length, toward upright leaned into the bend being taken.
            // The lean a bend of this tightness at this speed asks for, from the handlebars, not from how the
            // frame happens to be swinging.
            float bend = speed * speed * Mathf.Tan(steer * Mathf.Deg2Rad) / (wheelbase * 9.81f);
            leanNow = Mathf.Lerp(leanNow, Mathf.Clamp(Mathf.Atan(bend) * Mathf.Rad2Deg, -mostLean, mostLean), 1f - Mathf.Exp(-10f * Time.fixedDeltaTime));
            float lean = leanNow;
            Vector3 along = body.forward;
            Vector3 upWanted = Quaternion.AngleAxis(-lean, along) * Vector3.ProjectOnPlane(Vector3.up, along).normalized;
            float tilt = Vector3.SignedAngle(body.up, upWanted, along);
            float rolling = Vector3.Dot(frame.angularVelocity, along);
            frame.AddTorque(along * (tilt * Mathf.Deg2Rad * balance - rolling * balanceDamping), ForceMode.Acceleration);

            if (log != null && log.Length < 6000)
                log.Append(Time.time.ToString("f2")).Append(" yaw ").Append(body.eulerAngles.y.ToString("f0")).Append(" off ").Append(off.ToString("f0"))
                    .Append(" steer ").Append(steer.ToString("f0")).Append(" v ").Append(speed.ToString("f1")).Append(" tilt ").Append(tilt.ToString("f0"))
                    .Append(" loose ").Append((Vector3.Distance(frontWheel.position, fork.transform.TransformPoint(wheelInFork)) * 100f).ToString("f1")).Append(';');

            if (Mathf.Abs(tilt) > crashLean || Vector3.Angle(body.up, Vector3.up) > 80f)
                Crash();
        }

        void SetSteering(float degrees)
        {
            // Held at the angle, not sprung toward it: the fork is light and a spring has it swinging. The hinge
            // is given no room but a degree round where it should be.
            JointLimits limits = steering.limits;
            limits.min = degrees - 0.5f;
            limits.max = degrees + 0.5f;
            limits.bounciness = 0f;
            steering.limits = limits;
            steering.useLimits = true;
            JointSpring spring = steering.spring;
            spring.targetPosition = degrees;
            steering.spring = spring;
        }

        // The back wheel driven to roll at `speed` m/s, or left to roll by itself.
        void SetDrive(float speed, bool on, bool brake = false)
        {
            JointMotor motor = drive.motor;
            motor.targetVelocity = speed / wheelRadius * Mathf.Rad2Deg;
            motor.force = driveForce;
            motor.freeSpin = !brake;
            drive.motor = motor;
            drive.useMotor = on;
        }

        // ---- The rider's place and pose -----------------------------------------------------------------------

        // After the rider's Animator has posed it for the frame.
        void LateUpdate()
        {
            pedals += Speed / wheelRadius * gear * Time.deltaTime;
            if (crank != null)
                crank.localRotation = Quaternion.Euler(pedals * Mathf.Rad2Deg, 0f, 0f);
            frontWheelTurn += Vector3.Dot(frontWheel.angularVelocity, fork.transform.right) * Mathf.Rad2Deg * Time.deltaTime;
            frontWheelLook.localRotation = Quaternion.Euler(frontWheelTurn, 0f, 0f);

            if (!Ridden)
                return;

            Transform body = frame.transform;
            Transform hips = riderAnimator.GetBoneTransform(HumanBodyBones.Hips);
            riderAnimator.transform.localPosition = riderModelPlace;
            riderAnimator.transform.localRotation = riderModelFacing;
            rider.rotation = body.rotation;
            rider.position += saddle.position - hips.position;

            // Bent forward to the handlebars, arms out to them. The pose is made once, on getting on, from
            // whatever the Animator had the body doing at that moment, and then held as it is against the frame:
            // the Animator goes on playing its idle, and hands that swayed with it would not be holding
            // anything.
            Vector3 across = body.right;
            if (heldTurns == null)
            {
                Bend(HumanBodyBones.Spine, across, 22f);
                Bend(HumanBodyBones.Chest, across, 14f);
                Bend(HumanBodyBones.LeftUpperArm, across, -52f);
                Bend(HumanBodyBones.RightUpperArm, across, -52f);
                Bend(HumanBodyBones.LeftLowerArm, across, -12f);
                Bend(HumanBodyBones.RightLowerArm, across, -12f);
                // And each hand brought to its end of the handlebars, as they are with the bicycle going straight.
                if (grips != null)
                {
                    Vector3 middle = body.TransformPoint(gripsInFrame);
                    Reach(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand, middle - across * gripHalfWidth, across);
                    Reach(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, middle + across * gripHalfWidth, across);
                }
                heldTurns = new Quaternion[Held.Length];
                for (int i = 0; i < Held.Length; i++)
                {
                    Transform bone = riderAnimator.GetBoneTransform(Held[i]);
                    heldTurns[i] = bone != null ? Quaternion.Inverse(body.rotation) * bone.rotation : Quaternion.identity;
                }
            }
            else
            {
                for (int i = 0; i < Held.Length; i++)
                {
                    Transform bone = riderAnimator.GetBoneTransform(Held[i]);
                    if (bone != null)
                        bone.rotation = body.rotation * heldTurns[i];
                }
                // Turning the hips has moved nothing but what hangs from them; they are still on the saddle.
            }
            Bend(HumanBodyBones.Head, across, -24f);

            // The legs going round, half a turn apart: the knee is most bent when the thigh is highest.
            Leg(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, across, pedals);
            Leg(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, across, pedals + Mathf.PI);
        }

        // Turns an arm at the shoulder and the elbow so that the hand is at `target` (or as near as the arm is
        // long): first the elbow, to make the hand as far from the shoulder as the target is, then the whole arm
        // round the shoulder to point at it.
        void Reach(HumanBodyBones upperArm, HumanBodyBones lowerArm, HumanBodyBones hand, Vector3 target, Vector3 across)
        {
            Transform shoulder = riderAnimator.GetBoneTransform(upperArm);
            Transform elbow = riderAnimator.GetBoneTransform(lowerArm);
            Transform wrist = riderAnimator.GetBoneTransform(hand);
            if (shoulder == null || elbow == null || wrist == null)
                return;

            float upper = Vector3.Distance(shoulder.position, elbow.position);
            float lower = Vector3.Distance(elbow.position, wrist.position);
            float far = Mathf.Clamp(Vector3.Distance(shoulder.position, target), Mathf.Abs(upper - lower) + 0.01f, upper + lower - 0.01f);

            Vector3 toShoulder = shoulder.position - elbow.position;
            Vector3 toWrist = wrist.position - elbow.position;
            Vector3 hinge = Vector3.Cross(toShoulder, toWrist);
            if (hinge.sqrMagnitude < 0.000001f)
                hinge = across;
            float bendNow = Vector3.Angle(toShoulder, toWrist);
            float bendWanted = Mathf.Acos(Mathf.Clamp((upper * upper + lower * lower - far * far) / (2f * upper * lower), -1f, 1f)) * Mathf.Rad2Deg;
            elbow.rotation = Quaternion.AngleAxis(bendWanted - bendNow, hinge.normalized) * elbow.rotation;

            shoulder.rotation = Quaternion.FromToRotation(wrist.position - shoulder.position, target - shoulder.position) * shoulder.rotation;
        }

        void Leg(HumanBodyBones upper, HumanBodyBones lower, Vector3 across, float turn)
        {
            float up = Mathf.Cos(turn);
            Bend(upper, across, -(58f + 24f * up));
            Bend(lower, across, 78f + 30f * up);
        }

        void Bend(HumanBodyBones which, Vector3 axis, float degrees)
        {
            Transform bone = riderAnimator.GetBoneTransform(which);
            if (bone != null)
                bone.rotation = Quaternion.AngleAxis(degrees, axis) * bone.rotation;
        }
    }
}
