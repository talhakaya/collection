using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // The story's opening, in the story scene itself.
    //
    //   Begin (as the scene starts): nothing but the character, idling, in black, seen from the front by the intro
    //   camera. The world is switched off and the sea is in its place, but all of it is under the black.
    //
    //   RevealSea (a cutscene in the conversation): the black fades and the sea is there, while the view moves to
    //   the sea camera - further back and from a little above, so that the water under the character, and the
    //   character's reflection in it, are in the picture. (From the intro camera, level with the chest, the
    //   reflection is below the bottom of the screen.)
    //
    //   End (when the conversation is over): back to the follow camera.
    //
    // The black is a quad drawn over everything late in the see-through queue (the Blackout shader). The character
    // has to show on top of it, so while the black is up the character's materials are copies set to be drawn later
    // still; when the black has gone they are put back.
    //
    // Not saved yet: this happens every time the scene starts.
    public class IntroSequence : MonoBehaviour
    {
        [Tooltip("The character: everything under it is kept on top of the black.")]
        public Transform player;
        [Tooltip("The Cinemachine camera for the opening shot. It is put in front of the character when the intro begins.")]
        public GameObject introCamera;
        [Tooltip("Where the intro camera stands, from the character's feet, in the character's own directions (z is in front).")]
        public Vector3 cameraOffset = new Vector3(0f, 1.3f, 6f);
        [Tooltip("The height above the character's feet that the camera looks at.")]
        public float lookAtHeight = 1.3f;
        [Tooltip("The Cinemachine camera the view moves to as the sea appears.")]
        public GameObject seaCamera;
        public Vector3 seaCameraOffset = new Vector3(0f, 2.6f, 9.5f);
        public float seaLookAtHeight = 0.5f;
        [Tooltip("The quad with the Blackout material.")]
        public Renderer blackout;
        [Tooltip("Switched off for the intro: the land.")]
        public List<GameObject> world = new List<GameObject>();
        [Tooltip("Switched on for the intro: the water, its floor to stand on, its mirror camera.")]
        public GameObject sea;

        // Later than the black (Transparent+900).
        const int QueueOverBlackout = 3950;

        static readonly int ColorId = Shader.PropertyToID("_Color");
        static readonly int TiltShiftDisabledId = Shader.PropertyToID("_TiltShiftDisabled");

        readonly List<Renderer> raised = new List<Renderer>();
        readonly List<Material[]> originals = new List<Material[]>();
        MaterialPropertyBlock block;
        Coroutine fade;
        CinemachineBrain brain;
        CinemachineBlendDefinition usualBlend;
        bool slowBlend;

        void Start()
        {
            Begin();
        }

        void OnDestroy()
        {
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        public void Begin()
        {
            foreach (GameObject part in world)
                if (part != null)
                    part.SetActive(false);
            if (sea != null)
                sea.SetActive(true);

            SetBlack(1f);
            blackout.gameObject.SetActive(true);
            RaiseCharacter();

            // A plain shot from the front; the blur at the top and bottom of the screen is for the game's own view.
            Shader.SetGlobalFloat(TiltShiftDisabledId, 1f);
            Aim(introCamera, cameraOffset, lookAtHeight);
            if (seaCamera != null)
            {
                Aim(seaCamera, seaCameraOffset, seaLookAtHeight);
                seaCamera.SetActive(false);
            }
            introCamera.SetActive(true);
        }

        // Stands a camera in front of the character, looking at a height above its feet.
        void Aim(GameObject camera, Vector3 offset, float height)
        {
            Quaternion facing = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
            Vector3 feet = player.GetComponentInChildren<CharacterAppearance>().transform.position;
            Vector3 from = feet + facing * offset;
            Vector3 target = feet + Vector3.up * height;
            camera.transform.SetPositionAndRotation(from, Quaternion.LookRotation(target - from, Vector3.up));
        }

        // For a Cutscene's onStart: the black fades over this many seconds.
        public void RevealSea(float seconds)
        {
            if (fade != null)
                StopCoroutine(fade);
            fade = StartCoroutine(FadeBlack(seconds));

            // The brain blends from the intro camera to this one, being the one switched on last. The move takes
            // as long as the fade, where the game's own camera changes are quick.
            if (seaCamera != null)
            {
                if (brain == null)
                    brain = Camera.main.GetComponent<CinemachineBrain>();
                if (brain != null && !slowBlend)
                {
                    usualBlend = brain.DefaultBlend;
                    slowBlend = true;
                    brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, seconds);
                }
                seaCamera.SetActive(true);
                introCamera.SetActive(false);
            }
        }

        IEnumerator FadeBlack(float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                // Eased, so that it neither starts nor ends abruptly.
                SetBlack(1f - Mathf.SmoothStep(0f, 1f, t / seconds));
                yield return null;
            }
            SetBlack(0f);
            blackout.gameObject.SetActive(false);
            LowerCharacter();
            RestoreBlend();
            fade = null;
        }

        void RestoreBlend()
        {
            if (slowBlend && brain != null)
                brain.DefaultBlend = usualBlend;
            slowBlend = false;
        }

        // For the conversation's end.
        public void End()
        {
            if (fade != null)
            {
                StopCoroutine(fade);
                fade = null;
            }
            blackout.gameObject.SetActive(false);
            LowerCharacter();
            RestoreBlend();
            introCamera.SetActive(false);
            if (seaCamera != null)
                seaCamera.SetActive(false);
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        void SetBlack(float alpha)
        {
            block ??= new MaterialPropertyBlock();
            blackout.GetPropertyBlock(block);
            block.SetColor(ColorId, new Color(0f, 0f, 0f, alpha));
            blackout.SetPropertyBlock(block);
        }

        void RaiseCharacter()
        {
            if (raised.Count > 0)
                return;
            foreach (Renderer renderer in player.GetComponentsInChildren<Renderer>(true))
            {
                raised.Add(renderer);
                originals.Add(renderer.sharedMaterials);

                // Copies: the queue is the material's, and the same materials are on everyone else.
                Material[] copies = renderer.materials;
                foreach (Material copy in copies)
                    copy.renderQueue = QueueOverBlackout;
                renderer.materials = copies;
            }
        }

        void LowerCharacter()
        {
            for (int i = 0; i < raised.Count; i++)
            {
                Renderer renderer = raised[i];
                if (renderer == null)
                    continue;
                Material[] copies = renderer.sharedMaterials;
                renderer.sharedMaterials = originals[i];
                foreach (Material copy in copies)
                    if (copy != null && System.Array.IndexOf(originals[i], copy) < 0)
                        Destroy(copy);
            }
            raised.Clear();
            originals.Clear();
        }
    }
}
