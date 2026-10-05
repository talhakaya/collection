using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // The story's opening, in the story scene itself.
    //
    //   Begin (as the scene starts): nothing but the character, idling, in black, seen from the front by the intro
    //   camera. The world is switched off and the sea is in its place, lying just under the character's feet, but
    //   all of it is under the black.
    //
    //   RevealSea (a cutscene in the conversation): the black fades and the sea is there, rising around the
    //   character's legs as it appears, while the view moves to the sea camera - low over the water and wide,
    //   looking toward the sun, so that the sun's shine on the water and the character's reflection are in the
    //   picture.
    //
    //   End (when the conversation is over): back to the follow camera.
    //
    // The black is a quad drawn at the very back of the picture, after everything else (the Blackout shader): it
    // covers the sky and the water, which do not mark how far away they are, and leaves the character, who does.
    //
    // It happens once in a save slot (its NPCTrigger is onlyOnce). StoryDirector calls Begin when it has not
    // happened yet, and Skip when it has: the sea as the intro leaves it, with no black and no cameras.
    public class IntroSequence : MonoBehaviour
    {
        public Transform player;

        [Header("Opening shot")]
        [Tooltip("The Cinemachine camera for the opening shot. It is put in front of the character when the intro begins.")]
        public GameObject introCamera;
        [Tooltip("Where the camera stands, from the character's feet, in the character's own directions (z is in front).")]
        public Vector3 cameraOffset = new Vector3(0f, 1.3f, 6f);
        [Tooltip("The height above the character's feet that the camera looks at.")]
        public float lookAtHeight = 1.3f;
        [Tooltip("The quad with the Blackout material.")]
        public Renderer blackout;

        [Header("The sea")]
        [Tooltip("Switched off for the intro: the land.")]
        public List<GameObject> world = new List<GameObject>();
        [Tooltip("Switched on for the intro: the water, its floor to stand on, its mirror camera.")]
        public GameObject sea;
        [Tooltip("The water's surface, which rises as the sea appears.")]
        public Transform water;
        [Tooltip("How far up the character's legs the water comes, from the feet (m). Knee deep is about 0.5.")]
        public float waterDepth = 0.25f;
        [Tooltip("The Cinemachine camera the view moves to as the sea appears.")]
        public GameObject seaCamera;
        public Vector3 seaCameraOffset = new Vector3(0f, 0.9f, 7f);
        public float seaLookAtHeight = 1.25f;
        [Tooltip("The sun, turned for the sea: low and ahead of the sea camera, so that it shines on the water.")]
        public Light sun;
        [Tooltip("How high the sun is above the horizon (degrees), and how far round to the right of straight ahead of the sea camera.")]
        public float sunHeight = 13f;
        public float sunToTheRight = 18f;

        // The water starts this far under the feet: out of sight, and not yet round the legs.
        const float WaterUnderFeet = 0.02f;

        static readonly int ColorId = Shader.PropertyToID("_Color");
        static readonly int TiltShiftDisabledId = Shader.PropertyToID("_TiltShiftDisabled");

        MaterialPropertyBlock block;
        Coroutine fade;
        CinemachineBrain brain;
        CinemachineBlendDefinition usualBlend;
        bool slowBlend;
        float feetHeight;

        void OnDestroy()
        {
            Shader.SetGlobalFloat(TiltShiftDisabledId, 0f);
        }

        public void Begin()
        {
            ShowSea();
            SetWaterHeight(-WaterUnderFeet);

            SetBlack(1f);
            blackout.gameObject.SetActive(true);

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

        // The sea as it is after the intro, for a game that had its intro before. To be called with the character
        // where it stands in the scene, before anything moves it.
        public void Skip()
        {
            ShowSea();
            SetWaterHeight(waterDepth);
            blackout.gameObject.SetActive(false);
            introCamera.SetActive(false);
            if (seaCamera != null)
                seaCamera.SetActive(false);
        }

        // The land off, the sea on, the sun turned for it.
        void ShowSea()
        {
            foreach (GameObject part in world)
                if (part != null)
                    part.SetActive(false);
            if (sea != null)
                sea.SetActive(true);

            feetHeight = player.GetComponentInChildren<CharacterAppearance>().transform.position.y;

            // The sun beyond the character as the sea camera sees it, a little to one side: light on the water
            // between the two.
            if (sun != null)
            {
                float ahead = player.eulerAngles.y + 180f;
                sun.transform.rotation = Quaternion.Euler(sunHeight, ahead + sunToTheRight + 180f, 0f);
            }
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

        // For a Cutscene's onStart: the black fades, and the water rises, over this many seconds.
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
                float shown = Mathf.SmoothStep(0f, 1f, t / seconds);
                SetBlack(1f - shown);
                SetWaterHeight(Mathf.Lerp(-WaterUnderFeet, waterDepth, shown));
                yield return null;
            }
            Finish();
            fade = null;
        }

        // Everything as it is once the sea has appeared.
        void Finish()
        {
            SetBlack(0f);
            SetWaterHeight(waterDepth);
            blackout.gameObject.SetActive(false);
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
            Finish();
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

        // The height of the water's surface, from the character's feet.
        void SetWaterHeight(float aboveFeet)
        {
            if (water == null)
                return;
            Vector3 position = water.position;
            position.y = feetHeight + aboveFeet;
            water.position = position;
        }
    }
}
