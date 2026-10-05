using UnityEngine;

namespace Collection.Story
{
    // Player movement from the Move action (WASD, arrows or left stick): walks normally, sprints while Sprint is held
    // (Left Shift / left stick press). Input is relative to the main camera's facing, flattened onto the ground plane.
    //
    // Velocity changes through acceleration while the body turns at a limited rate, so during a change of direction the
    // character briefly moves sideways or backwards relative to where it faces. The Animator gets that local velocity
    // (VelX/VelZ) and blends the directional walk/run/sprint clips, plus turn-in-place footwork when turning on the spot.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Matches the walk animations' stride so the feet don't slide (measured: 1.9 m/s).")]
        [SerializeField] float walkSpeed = 1.9f;
        [Tooltip("Matches the sprint animations' stride (measured: 5.72 m/s).")]
        [SerializeField] float sprintSpeed = 5.72f;
        [Tooltip("How quickly velocity changes when starting, stopping or changing direction (m/s²).")]
        [SerializeField] float acceleration = 10f;
        [Tooltip("How fast the body turns toward the input direction (degrees/s) while walking and while sprinting. " +
                 "Slower than velocity changes, so sidestep/backstep animations show during turns.")]
        [SerializeField] float walkTurnSpeed = 420f;
        [SerializeField] float sprintTurnSpeed = 300f;
        [Tooltip("Turning rate (degrees/s) at which the turn-in-place animation plays at full weight.")]
        [SerializeField] float turnAnimationRate = 180f;
        [SerializeField] float gravity = -20f;
        [Tooltip("Character model's Animator; gets VelX/VelZ (local velocity, m/s), Speed (m/s) and Turn (-1..1).")]
        [SerializeField] Animator animator;

        static readonly int VelXParam = Animator.StringToHash("VelX");
        static readonly int VelZParam = Animator.StringToHash("VelZ");
        static readonly int SpeedParam = Animator.StringToHash("Speed");
        static readonly int TurnParam = Animator.StringToHash("Turn");

        CharacterController controller;
        Vector3 horizontalVelocity;
        float verticalVelocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            InputMan input = Main.inst.input;
            Vector3 direction = CameraRelative(Vector2.ClampMagnitude(input.move, 1f));

            // Analog sticks walk or sprint proportionally to how far they're pushed.
            Vector3 targetVelocity = direction * (input.sprint ? sprintSpeed : walkSpeed);
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);

            float yawBefore = transform.eulerAngles.y;
            if (direction.sqrMagnitude > 0.001f)
            {
                float speed01 = Mathf.InverseLerp(walkSpeed, sprintSpeed, horizontalVelocity.magnitude);
                float turnSpeed = Mathf.Lerp(walkTurnSpeed, sprintTurnSpeed, speed01);
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
            }
            float turnRate = Mathf.DeltaAngle(yawBefore, transform.eulerAngles.y) / Mathf.Max(Time.deltaTime, 0.0001f);

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = horizontalVelocity;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            if (animator != null)
                UpdateAnimator(turnRate);
        }

        void UpdateAnimator(float turnRate)
        {
            // Actual movement (after collisions), in the body's local space: x = right, z = forward.
            Vector3 moved = controller.velocity;
            moved.y = 0f;
            Vector3 local = transform.InverseTransformDirection(moved);
            float dt = Time.deltaTime;
            animator.SetFloat(VelXParam, local.x, 0.1f, dt);
            animator.SetFloat(VelZParam, local.z, 0.1f, dt);
            animator.SetFloat(SpeedParam, moved.magnitude, 0.1f, dt);
            animator.SetFloat(TurnParam, Mathf.Clamp(turnRate / turnAnimationRate, -1f, 1f), 0.15f, dt);
        }

        static Vector3 CameraRelative(Vector2 input)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return new Vector3(input.x, 0f, input.y);

            Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
            return forward * input.y + right * input.x;
        }
    }
}
