using UnityEngine;

namespace Collection.Story
{
    // The dodge roll: Space or the pad's right button. The character goes head over heels in the direction it is
    // being steered (or the way it faces), a fixed distance in a fixed time, and breaks any barrel it rolls into.
    //
    // There is no roll among the character's animation clips; the roll is made here, on top of whatever the
    // Animator is playing, after it has posed the body each frame: the whole model is turned once round its hips
    // while the hips are brought down near the ground, and the body is tucked in (the back bent, the knees up to
    // the chest, the arms in).
    //
    // Now and then (fallChance) a roll goes wrong: somewhere in its middle half the character turns into a
    // ragdoll (Ragdoll) and has to get up again.
    [RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
    public class PlayerRoll : MonoBehaviour
    {
        [Tooltip("The character's Animator, on the model that is turned.")]
        public Animator animator;
        public Ragdoll ragdoll;

        [Header("The roll")]
        public float duration = 0.8f;
        [Tooltip("How far it goes (m).")]
        public float distance = 4f;
        [Tooltip("Seconds after one roll's end before the next can start.")]
        public float pause = 0.15f;
        [Tooltip("How high the hips are off the ground in the middle of the roll (m).")]
        public float hipHeight = 0.38f;

        [Header("Falling over")]
        [Range(0f, 1f)] public float fallChance = 0.2f;
        [Tooltip("The part of the roll in which the fall can come, from 0 (its start) to 1 (its end).")]
        public Vector2 fallWindow = new Vector2(0.25f, 0.75f);
        [Tooltip("How much of the roll's speed and turning the falling body keeps. At 1 it tumbles on for metres.")]
        [Range(0f, 1f)] public float fallCarry = 0.55f;

        public bool Rolling { get; private set; }

        CharacterController controller;
        PlayerMovement movement;
        PlayerControl control;
        Transform model;
        Vector3 modelPlace;
        Quaternion modelFacing;
        Vector3 direction;
        float time;
        float fallAt;
        float nextAt;
        float verticalVelocity;

        Transform hips, spine, chest, upperChest;
        Transform leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg;
        Transform leftUpperArm, rightUpperArm, leftLowerArm, rightLowerArm;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            movement = GetComponent<PlayerMovement>();
            control = PlayerControl.Of(this);
            model = animator.transform;

            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
            leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        }

        void Update()
        {
            if (!Rolling)
            {
                if (control.Free && controller.isGrounded && Time.time >= nextAt
                    && Main.inst.input.rollPressed)
                    Begin();
                return;
            }

            time += Time.deltaTime;

            // Fast off the mark and slowing toward the end, covering `distance` over the whole roll.
            float done = Mathf.Clamp01(time / duration);
            float speed = distance / duration * Mathf.Lerp(1.5f, 0.5f, done);
            verticalVelocity = controller.isGrounded ? -2f : verticalVelocity - 20f * Time.deltaTime;
            controller.Move((direction * speed + Vector3.up * verticalVelocity) * Time.deltaTime);

            if (time >= duration)
                End(direction * speed);
        }

        void Begin()
        {
            Vector2 stick = Main.inst.input.move;
            direction = transform.forward;
            Camera view = Camera.main;
            if (stick.magnitude > 0.2f && view != null)
            {
                Vector3 forward = Vector3.ProjectOnPlane(view.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(view.transform.right, Vector3.up).normalized;
                direction = (forward * stick.y + right * stick.x).normalized;
            }
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            modelPlace = model.localPosition;
            modelFacing = model.localRotation;
            time = 0f;
            verticalVelocity = -2f;
            fallAt = Random.value < fallChance ? Random.Range(fallWindow.x, fallWindow.y) * duration : -1f;
            // The movement, not the body: the roll moves that itself.
            control.Take(this, false);
            Rolling = true;
        }

        void End(Vector3 velocity)
        {
            Rolling = false;
            nextAt = Time.time + pause;
            model.localPosition = modelPlace;
            model.localRotation = modelFacing;
            movement.Velocity = velocity;
            control.Release(this);
        }

        // No roll for this long from now: for whatever hands the character back on the press of the roll button
        // (getting off the bicycle), which is not also a roll.
        public void Wait(float seconds)
        {
            nextAt = Mathf.Max(nextAt, Time.time + seconds);
        }

        // Rolling into a barrel breaks it.
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!Rolling)
                return;
            Barrel barrel = hit.collider.GetComponentInParent<Barrel>();
            if (barrel != null)
                barrel.Break(direction * (distance / duration), controller);
        }

        // After the Animator has posed the body for the frame.
        void LateUpdate()
        {
            if (!Rolling)
                return;

            float done = Mathf.Clamp01(time / duration);
            // How far into the tuck: quickly in, held, and out again in time to land on the feet.
            float tuck = Mathf.SmoothStep(0f, 1f, Mathf.Min(done / 0.2f, (1f - done) / 0.25f));
            // Once round, a little eased at both ends.
            float angle = 360f * Mathf.Lerp(done, Mathf.SmoothStep(0f, 1f, done), 0.5f);

            // The model back where it stands, then: down, so the hips come near the ground, and turned about the
            // hips.
            model.localPosition = modelPlace;
            model.localRotation = modelFacing;
            float hipsUp = hips.position.y - model.position.y;
            model.position += Vector3.down * (Mathf.Max(0f, hipsUp - hipHeight) * tuck);
            model.RotateAround(hips.position, model.right, angle);

            // The tuck, each part turned about the model's left-to-right line. Parents before children.
            Vector3 across = model.right;
            Bend(spine, across, 30f * tuck);
            Bend(chest, across, 30f * tuck);
            Bend(upperChest, across, 20f * tuck);
            Bend(leftUpperLeg, across, -115f * tuck);
            Bend(rightUpperLeg, across, -115f * tuck);
            Bend(leftLowerLeg, across, 125f * tuck);
            Bend(rightLowerLeg, across, 125f * tuck);
            Bend(leftUpperArm, across, -55f * tuck);
            Bend(rightUpperArm, across, -55f * tuck);
            Bend(leftLowerArm, across, -95f * tuck);
            Bend(rightLowerArm, across, -95f * tuck);

            // The roll that goes wrong: from this pose, at this speed, as a ragdoll.
            if (fallAt >= 0f && time >= fallAt && ragdoll != null)
            {
                float speed = distance / duration * Mathf.Lerp(1.5f, 0.5f, done);
                Rolling = false;
                nextAt = Time.time + pause;
                movement.Velocity = Vector3.zero;
                ragdoll.Fall(direction * (speed * fallCarry), across * (360f / duration * Mathf.Deg2Rad * fallCarry), modelPlace, modelFacing);
                // The fall has the character now.
                control.Release(this);
            }
        }

        static void Bend(Transform bone, Vector3 axis, float degrees)
        {
            if (bone != null)
                bone.rotation = Quaternion.AngleAxis(degrees, axis) * bone.rotation;
        }
    }
}
