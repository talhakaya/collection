using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game2
{
    public class PlayerControl : MonoBehaviour
    {
        public static PlayerControl inst;

        public Actor actor;
        public Camera cam;
        public MapView mapView;
        public Cinemachine.CinemachineFreeLook camFreeLook;
        public Transform camDefaultLookAt;
        public Cinemachine.CinemachineFreeLook.Orbit[] camDefaultOrbits;
        public Transform camAimLookAt;
        public Cinemachine.CinemachineFreeLook.Orbit[] camAimOrbits;
        public float periodCamTransition;
        public GameObject uiCrosshair;
        public LayerMask camLookAtLayer;
        public List<HideSpot> hideSpots;
        public AnimationCurve lookGamepadCurve;

        private float timerCamTransition;
        private float hitDistance;
        private PlayerInputActions inputAction;
        private Vector2 moveInput;
        private Vector2 lookInputMouse;
        private Vector2 lookInputGamepadRaw;
        private float lookGamepadTimer;
        private const float lookGamepadTimeMax = 1f;
        private float crouchInput;
        private float fireInput;
        private float aimInput;
        private float jumpInput;
        private float mapViewInput;
        private Actor aimedEnemy;
        private Actor aimedEnemyOld;

        private void Awake()
        {
            inst = this;
        }

        private void Start()
        {
            MapView.inst.player = transform;
            hitDistance = 100f;
            inputAction = new PlayerInputActions();
            inputAction.PlayerControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            inputAction.PlayerControls.LookMouse.performed += ctx => {
                lookInputMouse = ctx.ReadValue<Vector2>();
                lookInputMouse *= 18f / Screen.height;
            };
            inputAction.PlayerControls.LookGamepad.performed += ctx => lookInputGamepadRaw = ctx.ReadValue<Vector2>();
            inputAction.PlayerControls.Crouch.performed += ctx => crouchInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Fire.performed += ctx => fireInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Aim.performed += ctx => aimInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Jump.performed += ctx => jumpInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.MapView.performed += ctx => mapViewInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Crouch.canceled += ctx => crouchInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Fire.canceled += ctx => fireInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Aim.canceled += ctx => aimInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.Jump.canceled += ctx => jumpInput = ctx.ReadValue<float>();
            inputAction.PlayerControls.MapView.canceled += ctx => mapViewInput = ctx.ReadValue<float>();
            inputAction.Enable();
        }

        private void Update()
        {
            bool inputMapView = mapViewInput > 0f;
            mapView.UpdateDt(inputMapView, Time.deltaTime);

            if (mapView.transitionTimer > 0f)
            {
                actor.Move(Vector3.zero, false, false, false, Vector3.zero);
                return;
            }

            float inputHor = moveInput.x;
            float inputVer = moveInput.y;
            bool inputFire = fireInput > 0f;
            bool inputAim = aimInput > 0f;
            bool inputCrouch = crouchInput > 0f;
            bool inputJump = jumpInput > 0f;
            if (inputAim)
            {
                camFreeLook.m_Follow = camAimLookAt;
                camFreeLook.m_LookAt = camAimLookAt;
                timerCamTransition = Mathf.Clamp(timerCamTransition + Time.deltaTime, 0f, periodCamTransition);
            }
            else
            {
                camFreeLook.m_Follow = camDefaultLookAt;
                camFreeLook.m_LookAt = camDefaultLookAt;
                timerCamTransition = Mathf.Clamp(timerCamTransition - Time.deltaTime, 0f, periodCamTransition);
            }
            float animRatioCamTransition = timerCamTransition / periodCamTransition;
            for (int i = 0; i < 3; i++)
            {
                camFreeLook.m_Orbits[i] = new Cinemachine.CinemachineFreeLook.Orbit(
                    Mathf.Lerp(camDefaultOrbits[i].m_Height, camAimOrbits[i].m_Height, animRatioCamTransition),
                    Mathf.Lerp(camDefaultOrbits[i].m_Radius, camAimOrbits[i].m_Radius, animRatioCamTransition));
            }

            Vector3 lookAt = GetCamLookAt();
            actor.Move(new Vector3(inputHor, inputJump ? 1f : 0f, inputVer), inputFire, inputAim, inputCrouch, lookAt);

            bool isHidden = IsHidden();
            foreach (Renderer r in actor.GetRenderers())
            {
                r.material.SetFloat("_TintAmount", isHidden ? 1 : 0);
            }
            if (lookInputGamepadRaw.magnitude > 0.1f) lookGamepadTimer = Mathf.Clamp(lookGamepadTimer + Time.deltaTime, 0f, lookGamepadTimeMax);
            else lookGamepadTimer = Mathf.Clamp(lookGamepadTimer - 2f * Time.deltaTime, 0f, lookGamepadTimeMax);

            Vector2 lookInputGamepad = lookInputGamepadRaw * lookGamepadCurve.Evaluate(lookGamepadTimer / lookGamepadTimeMax) * 0.5f;
            Vector2 lookInput = lookInputMouse + lookInputGamepad;
            camFreeLook.m_XAxis.m_InputAxisValue = lookInput.x;
            camFreeLook.m_YAxis.m_InputAxisValue = lookInput.y;

            if (aimedEnemy != aimedEnemyOld)
            {
                OutlineHandler.inst.Clear();
                if (aimedEnemy != null) OutlineHandler.inst.Show(aimedEnemy);
                aimedEnemyOld = aimedEnemy;
            }
        }

        private Vector3 GetCamLookAt()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            bool cast = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, camLookAtLayer, QueryTriggerInteraction.Ignore);
            if (cast)
            {
                float newhitDistance = Mathf.Max(5f, hitInfo.distance);
                hitDistance += (newhitDistance - hitDistance) * Time.deltaTime * 10f;
                aimedEnemy = hitInfo.transform.GetComponent<Actor>();
            }
            else aimedEnemy = null;
            return cam.transform.position + cam.transform.forward * hitDistance;
        }

        public bool IsHidden()
        {
            foreach (HideSpot hideSpot in hideSpots)
            {
                if (!hideSpot.needToCrouch || actor.IsCrouching()) return true;
            }
            return false;
        }
    }
}
