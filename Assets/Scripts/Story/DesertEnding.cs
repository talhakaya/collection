using System.Collections;
using Collection.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // How the desert ends, in the television's last conversation there. The conversation stops for each of
    // these in turn (each is a Cutscene's onStart; the hammer between the first two is HammerGame's):
    //
    //   Jigsaw: the artifacts are put together. For now a screen that says it has been done.
    //
    //   ShowBox: a cardboard box appears on the ground by the character, open at the top, its flaps out, and
    //   the view comes in close to it. The box is exactly the size of the character's head, which by now is a
    //   cube.
    //
    //   IntoTheBox: the character is lifted, turned upside down as stiff as a doll, and let down head first
    //   into the box - slowly and evenly, a perfect fit - and once the head is in, the rest drops after it and
    //   the picture goes black. Then the head, as the hammer left it, is kept in the save: the level is over.
    //
    // The conversation's end, after that, is what goes on to the next level (StoryDirector.NextLevel).
    //
    // The box is a placeholder built here of flat squares with one drawing on all of them.
    public class DesertEnding : MonoBehaviour
    {
        public StoryDirector director;

        [Header("The box")]
        [Tooltip("With the drawing of one side of the box. Shown from both sides.")]
        public Material boxMaterial;
        [Tooltip("How far the flaps lean out from straight up (degrees), and how long they are, as a part of the box's side.")]
        public float flapAngle = 20f;
        public float flapLength = 0.5f;
        [Tooltip("How far from the character it appears (m), toward the television.")]
        public float boxAhead = 2.2f;
        [Tooltip("How much wider than the head it is inside (m). The less, the better the fit.")]
        public float room = 0.012f;

        [Header("The view")]
        [Tooltip("Where the camera stands, from the middle between the character and the box: to the side, up, and back from the box (m).")]
        public Vector3 cameraFrom = new Vector3(5.6f, 2f, -0.8f);
        public float lookAtHeight = 1f;
        [Tooltip("The camera's priority: over every other camera of the level.")]
        public int priority = 50;

        [Header("Into the box")]
        [Tooltip("Seconds to be lifted and turned over, and how far above the box the head hangs then (m).")]
        public float liftTime = 1.6f;
        public float hangsAbove = 0.5f;
        [Tooltip("Seconds for the head to go in.")]
        public float fitTime = 5f;
        [Tooltip("Seconds for the rest to go after it, and for the black.")]
        public float dropTime = 0.8f;

        // From the middle of a sphere of radius 1 to a side of the cube inside it.
        const float CubeHalf = 0.57735027f;

        Transform box;
        float boxSide;
        GameObject boxCamera;

        // For a Cutscene's onStart; the cutscene goes on until this is answered.
        public void Jigsaw()
        {
            var screen = new MenuScreen { title = "Jigsaw complete!" };
            screen.body = () => "(The jigsaw puzzle of the artifacts goes here.)";
            System.Action on = () =>
            {
                Menus.CloseAll();
                Cutscene.FinishRunning();
            };
            screen.Button("Continue", on);
            screen.cancel = on;
            Menus.Push(screen);
        }

        // For a Cutscene's onStart.
        public void ShowBox()
        {
            if (box != null)
                return;
            Transform player = director.player;
            TelevisionHead head = player.GetComponentInChildren<TelevisionHead>();
            boxSide = head.FullRadius * CubeHalf * 2f + room;

            // Toward the television, if the level has one; otherwise the way the character faces.
            Vector3 toward = player.forward;
            if (director.desert != null && director.desert.television != null)
                toward = director.desert.television.transform.position - player.position;
            toward = Vector3.ProjectOnPlane(toward, Vector3.up);
            toward = toward.sqrMagnitude > 0.001f ? toward.normalized : Vector3.forward;

            Vector3 place = player.position + toward * boxAhead;
            float ground;
            place.y = Ground.Under(place + Vector3.up * 3f, 30f, ~0, out ground, player) ? ground : Ground.Land(place);
            box = Build(boxSide);
            box.SetPositionAndRotation(place, Quaternion.LookRotation(toward, Vector3.up));
            StartCoroutine(Appear());

            // The view: close, from the side, with the character and the box both in it.
            Vector3 between = (player.position + place) * 0.5f;
            between.y = place.y;
            Vector3 across = Vector3.Cross(Vector3.up, toward);
            Vector3 from = between + across * cameraFrom.x + Vector3.up * cameraFrom.y + toward * cameraFrom.z;
            Vector3 target = between + Vector3.up * lookAtHeight;
            boxCamera = new GameObject("CM Box Camera");
            boxCamera.SetActive(false);
            // Over whatever camera the conversation has on (the television's wide one).
            boxCamera.AddComponent<CinemachineCamera>().Priority = new PrioritySettings { Enabled = true, Value = priority };
            boxCamera.transform.SetPositionAndRotation(from, Quaternion.LookRotation(target - from, Vector3.up));
            boxCamera.SetActive(true);
        }

        IEnumerator Appear()
        {
            for (float t = 0f; t < 0.4f; t += Time.deltaTime)
            {
                float shown = t / 0.4f;
                box.localScale = Vector3.one * (shown + Mathf.Sin(shown * Mathf.PI) * 0.2f);
                yield return null;
            }
            box.localScale = Vector3.one;
        }

        // For a Cutscene's onStart; the cutscene goes on until this is done.
        public void IntoTheBox()
        {
            if (box == null)
                ShowBox();
            StartCoroutine(GoIn());
        }

        IEnumerator GoIn()
        {
            Transform player = director.player;
            PlayerControl control = PlayerControl.Of(player);
            control.Take(this);
            TelevisionHead head = player.GetComponentInChildren<TelevisionHead>();
            head.keepFullSize = true;
            // Not animated: as it stands at this moment, all the way.
            Animator animator = player.GetComponentInChildren<Animator>();
            animator.speed = 0f;
            yield return null;

            // Upside down, and with the head's sides square to the box's. The head is turned a little on the
            // body by the pose it is held in; the body is turned to make up for that.
            Quaternion headOnBody = Quaternion.Inverse(player.rotation) * head.transform.rotation;
            Quaternion stood = player.rotation;
            Quaternion over = box.rotation * Quaternion.Euler(180f, 0f, 0f) * Quaternion.Inverse(headOnBody);
            Vector3 headStart = head.transform.position;
            float half = boxSide * 0.5f;
            Vector3 above = box.position + Vector3.up * (boxSide + half + hangsAbove);
            Vector3 inside = box.position + Vector3.up * (half + 0.004f);

            // Lifted and turned over.
            for (float t = 0f; t < liftTime; t += Time.deltaTime)
            {
                float done = Mathf.SmoothStep(0f, 1f, t / liftTime);
                Hold(player, head, Quaternion.Slerp(stood, over, done), Vector3.Lerp(headStart, above, done) + Vector3.up * (Mathf.Sin(done * Mathf.PI) * 0.6f));
                yield return null;
            }

            // In: evenly, with no ease to it.
            for (float t = 0f; t < fitTime; t += Time.deltaTime)
            {
                Hold(player, head, over, Vector3.Lerp(above, inside, t / fitTime));
                yield return null;
            }
            Hold(player, head, over, inside);
            yield return new WaitForSeconds(0.35f);

            // And the rest after it, as the picture goes black.
            CanvasGroup black = Main.inst.camera.screenFade;
            for (float t = 0f; t < dropTime; t += Time.deltaTime)
            {
                float done = t / dropTime;
                Hold(player, head, over, inside + Vector3.down * (done * done * 9f));
                if (black != null)
                    black.alpha = Mathf.Clamp01(done * 1.4f);
                yield return null;
            }
            if (black != null)
                black.alpha = 1f;

            // The level is over: the head is the character's from now on.
            head.Save();
            Cutscene.FinishRunning();
        }

        // The character turned a way, and put so that its head's middle is at a place.
        static void Hold(Transform player, TelevisionHead head, Quaternion turned, Vector3 headAt)
        {
            player.rotation = turned;
            player.position += headAt - head.transform.position;
        }

        // The box, standing on its own origin: four sides, a bottom, and a flap leaning out from the top of each
        // side.
        Transform Build(float side)
        {
            var made = new GameObject("Box").transform;
            float half = side * 0.5f;
            Flat(made, "Bottom", new Vector3(0f, 0.004f, 0f), Quaternion.Euler(90f, 0f, 0f), new Vector2(side, side));
            for (int i = 0; i < 4; i++)
            {
                // Each side's own directions: out of the box is its backward.
                Quaternion round = Quaternion.Euler(0f, i * 90f, 0f);
                Flat(made, "Side " + i, round * new Vector3(0f, half, -half), round, new Vector2(side, side));

                var hinge = new GameObject("Flap " + i).transform;
                hinge.SetParent(made, false);
                hinge.localPosition = round * new Vector3(0f, side, -half);
                hinge.localRotation = round * Quaternion.Euler(-flapAngle, 0f, 0f);
                Flat(hinge, "Flap", new Vector3(0f, side * flapLength * 0.5f, 0f), Quaternion.identity, new Vector2(side, side * flapLength));
            }
            return made;
        }

        void Flat(Transform parent, string name, Vector3 at, Quaternion turned, Vector2 size)
        {
            GameObject flat = GameObject.CreatePrimitive(PrimitiveType.Quad);
            flat.name = name;
            Destroy(flat.GetComponent<Collider>());
            flat.transform.SetParent(parent, false);
            flat.transform.localPosition = at;
            flat.transform.localRotation = turned;
            flat.transform.localScale = new Vector3(size.x, size.y, 1f);
            var drawn = flat.GetComponent<MeshRenderer>();
            if (boxMaterial != null)
                drawn.sharedMaterial = boxMaterial;
            drawn.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
