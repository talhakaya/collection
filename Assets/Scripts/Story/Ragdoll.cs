using System.Collections.Generic;
using UnityEngine;

namespace Collection.Story
{
    // The character as a ragdoll: for falling over and getting up again.
    //
    //   Fall: the Animator and the character's own movement stop, and the body's parts (hips, back, head, upper
    //   and lower arms and legs) become rigid bodies joined at the joints, starting from the pose and the speed
    //   the character has. The character's own object keeps up with the hips along the ground, so the camera and
    //   everything else that follows the character follows the fall.
    //
    //   Down, the television head keeps its full size (it has a collider that size, so it lies on the ground and
    //   not in it) and shows `emote`.
    //
    //   Getting up: after `downFor` seconds, on moving or pressing something. The Animator takes over again and
    //   the body goes from where it lay to the animated pose over `getUpTime` seconds. (There is no getting-up
    //   animation; the body simply rights itself.)
    //
    // The rigid bodies, colliders and joints are made here the first time they are needed, from the Animator's
    // humanoid bones, so the character needs nothing set up by hand and its look (CharacterAppearance) can change.
    [RequireComponent(typeof(CharacterController))]
    public class Ragdoll : MonoBehaviour
    {
        public Animator animator;
        [Tooltip("Seconds on the ground before it can get up.")]
        public float downFor = 5f;
        [Tooltip("Seconds from lying to standing.")]
        public float getUpTime = 0.6f;
        [Tooltip("The face shown while down.")]
        public string emote = "scared";
        [Tooltip("What the body lands on.")]
        public LayerMask ground = 1;

        public bool Down { get; private set; }
        public bool GettingUp => rising > 0f;

        class Part
        {
            public Transform bone;
            public Rigidbody body;
            public Collider collider;
            public Vector3 lyingPosition;
            public Quaternion lyingRotation;
        }

        readonly List<Part> parts = new List<Part>();
        CharacterController controller;
        PlayerMovement movement;
        PlayerRoll roll;
        Transform model;
        Transform hips;
        Vector3 modelPlace;
        Quaternion modelFacing;
        float downTime;
        float rising;
        Quaternion[] animated;
        PhysicsMaterial rough;
        TelevisionHead head;
        ScreenFace face;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            movement = GetComponent<PlayerMovement>();
            roll = GetComponent<PlayerRoll>();
            model = animator.transform;
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            head = GetComponentInChildren<TelevisionHead>();
            face = GetComponentInChildren<ScreenFace>();
        }

        // `modelPlace` and `modelFacing`: where the model belongs under the character when standing, for a fall
        // that starts with the model moved (the roll).
        public void Fall(Vector3 velocity, Vector3 spin, Vector3 modelPlace, Quaternion modelFacing)
        {
            if (Down)
                return;
            if (parts.Count == 0)
                Build();

            this.modelPlace = modelPlace;
            this.modelFacing = modelFacing;
            Down = true;
            downTime = 0f;
            rising = 0f;

            if (head != null)
                head.keepFullSize = true;
            if (face != null && !string.IsNullOrEmpty(emote))
                face.Show(emote, true);

            animator.enabled = false;
            movement.enabled = false;
            if (roll != null)
                roll.enabled = false;
            controller.enabled = false;

            Vector3 middle = hips.position;
            foreach (Part part in parts)
            {
                part.collider.enabled = true;
                part.body.isKinematic = false;
                // Turning about the hips as it was, so each part's speed is the whole's plus its share of the turn.
                part.body.linearVelocity = velocity + Vector3.Cross(spin, part.body.worldCenterOfMass - middle);
                part.body.angularVelocity = spin;
            }
        }

        public void Fall()
        {
            Fall(controller.velocity, Vector3.zero, model.localPosition, model.localRotation);
        }

        void Update()
        {
            if (!Down)
                return;
            downTime += Time.deltaTime;
            if (downTime >= downFor && Main.inst.input.anyPressed)
                GetUp();
        }

        void GetUp()
        {
            Down = false;

            foreach (Part part in parts)
            {
                part.body.isKinematic = true;
                part.collider.enabled = false;
                part.lyingPosition = part.bone.position;
                part.lyingRotation = part.bone.rotation;
            }

            // The character stands where the hips are, on the ground under them, facing as it did.
            Vector3 place = hips.position;
            float feet = place.y - 1f;
            if (Physics.Raycast(place + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 20f, ground, QueryTriggerInteraction.Ignore))
                feet = hit.point.y;
            place.y = feet + controller.height * 0.5f - controller.center.y + controller.skinWidth;
            transform.position = place;
            model.localPosition = modelPlace;
            model.localRotation = modelFacing;

            animator.enabled = true;
            controller.enabled = true;
            movement.Velocity = Vector3.zero;
            movement.enabled = true;
            if (roll != null)
                roll.enabled = true;
            rising = getUpTime;
            if (face != null)
                face.ShowIdle();
        }

        void LateUpdate()
        {
            if (Down)
            {
                // The character's own object along with the hips, without moving the body, which hangs under it.
                Vector3 place = hips.position;
                Quaternion turn = hips.rotation;
                Vector3 to = new Vector3(place.x, transform.position.y, place.z);
                if ((to - transform.position).sqrMagnitude > 0.0001f)
                {
                    transform.position = to;
                    hips.SetPositionAndRotation(place, turn);
                }
                return;
            }

            if (rising <= 0f)
            {
                if (head != null)
                    head.keepFullSize = false;
                return;
            }

            // From where each part lay to where the Animator has it this frame. The animated places are read
            // first, all of them, because moving a part moves the parts under it.
            rising -= Time.deltaTime;
            float lying = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(rising / getUpTime));
            if (animated == null || animated.Length != parts.Count)
                animated = new Quaternion[parts.Count];
            for (int i = 0; i < parts.Count; i++)
                animated[i] = parts[i].bone.rotation;
            Vector3 hipsAnimated = hips.position;

            for (int i = 0; i < parts.Count; i++)
            {
                Part part = parts[i];
                if (part.bone == hips)
                    part.bone.position = Vector3.Lerp(hipsAnimated, part.lyingPosition, lying);
                part.bone.rotation = Quaternion.Slerp(animated[i], part.lyingRotation, lying);
            }
        }

        // ---- Making the ragdoll ---------------------------------------------------------------------------------

        void Build()
        {
            // Parents before children: the order they are moved in when getting up.
            Part hipsPart = Add(HumanBodyBones.Hips, null, HumanBodyBones.Spine, 0.13f, 12f);
            Part back = Add(Has(HumanBodyBones.Chest) ? HumanBodyBones.Chest : HumanBodyBones.Spine, hipsPart, HumanBodyBones.Head, 0.13f, 12f);
            Add(HumanBodyBones.Head, back, HumanBodyBones.Head, 0.14f, 4f);
            foreach (bool left in new[] { true, false })
            {
                Part upperArm = Add(left ? HumanBodyBones.LeftUpperArm : HumanBodyBones.RightUpperArm, back,
                    left ? HumanBodyBones.LeftLowerArm : HumanBodyBones.RightLowerArm, 0.055f, 2f);
                Add(left ? HumanBodyBones.LeftLowerArm : HumanBodyBones.RightLowerArm, upperArm,
                    left ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand, 0.05f, 1.5f);
                Part upperLeg = Add(left ? HumanBodyBones.LeftUpperLeg : HumanBodyBones.RightUpperLeg, hipsPart,
                    left ? HumanBodyBones.LeftLowerLeg : HumanBodyBones.RightLowerLeg, 0.075f, 5f);
                Add(left ? HumanBodyBones.LeftLowerLeg : HumanBodyBones.RightLowerLeg, upperLeg,
                    left ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot, 0.06f, 3f);
            }
        }

        bool Has(HumanBodyBones bone)
        {
            return animator.GetBoneTransform(bone) != null;
        }

        // A part: a capsule from the bone to `toward` (a ball when that is the bone itself), joined to `parent`.
        Part Add(HumanBodyBones which, Part parent, HumanBodyBones toward, float radius, float mass)
        {
            Transform bone = animator.GetBoneTransform(which);
            Transform end = animator.GetBoneTransform(toward);
            if (bone == null)
                return parent;

            // Sizes are given in metres; the collider's own are in the bone's, which may be scaled.
            float scale = Mathf.Max(0.0001f, Mathf.Abs(bone.lossyScale.x));
            Collider collider;
            if (end == null || end == bone)
            {
                var ball = bone.gameObject.AddComponent<SphereCollider>();
                ball.radius = radius / scale;
                ball.center = bone.InverseTransformVector(Vector3.up * radius * 0.6f);
                // The head is the television: a ball the size it is (a sphere of radius 1, scaled), where it is.
                if (head != null && head.transform.parent == bone)
                {
                    ball.radius = head.FullRadius / scale;
                    ball.center = head.transform.localPosition;
                }
                collider = ball;
            }
            else
            {
                Vector3 along = bone.InverseTransformPoint(end.position);
                var capsule = bone.gameObject.AddComponent<CapsuleCollider>();
                Vector3 size = new Vector3(Mathf.Abs(along.x), Mathf.Abs(along.y), Mathf.Abs(along.z));
                capsule.direction = size.x > size.y && size.x > size.z ? 0 : size.y > size.z ? 1 : 2;
                capsule.center = along * 0.5f;
                capsule.radius = radius / scale;
                capsule.height = along.magnitude + capsule.radius;
                collider = capsule;
            }
            collider.enabled = false;
            // Rough, so the body comes to rest where it falls and does not slide on.
            if (rough == null)
                rough = new PhysicsMaterial("Ragdoll") { dynamicFriction = 0.9f, staticFriction = 0.9f, frictionCombine = PhysicsMaterialCombine.Maximum };
            collider.sharedMaterial = rough;

            var body = bone.gameObject.AddComponent<Rigidbody>();
            body.mass = mass;
            body.isKinematic = true;
            body.linearDamping = 0.5f;
            body.angularDamping = 1.5f;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (parent != null)
            {
                var joint = bone.gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = parent.body;
                joint.enableProjection = true;
                joint.lowTwistLimit = new SoftJointLimit { limit = -35f };
                joint.highTwistLimit = new SoftJointLimit { limit = 35f };
                joint.swing1Limit = new SoftJointLimit { limit = 50f };
                joint.swing2Limit = new SoftJointLimit { limit = 50f };
            }

            var part = new Part { bone = bone, body = body, collider = collider };
            parts.Add(part);
            return part;
        }
    }
}
