using System.Collections;
using System.Collections.Generic;
using Collection.Controls;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // The hammer: the artifacts come together into one, and the player beats the character's round head into a
    // cube with it, a side at a time.
    //
    // It is seen on black, with nothing but the character and the hammer (the character creator's way of
    // showing the character: CameraMan.EnterStudio), from a camera standing off to one side and above. The hammer
    // is always on the right of the picture. The character is turned, between blows, so that the side to be hit
    // next faces the hammer; for the top of the head the hammer goes up over it and comes down, and the
    // character is turned to face the camera, so that its face is seen as it is hit. After the last blow, which
    // is to the face, the view goes in on the face from straight in front.
    //
    // The only thing the player does is press the button under "SMASH!" (the story's Interact). Each press is one
    // blow, and each blow flattens one side of the head (MorphSphere, the sphere that becomes a cube), in the
    // order: left, back, right, top, top again, front. The top is hit twice because a blow from above squashes
    // the head between the hammer and the neck: top and bottom both go halfway, and then flat.
    //
    // The face is `terror` while it waits for the next blow and `pain` as one lands.
    //
    // Play is for a Cutscene's onStart, in a conversation; the cutscene is one that goes on until told (it is
    // told here, when the last blow has landed). The head is left flat but is not kept in the save here: whoever
    // ends the level keeps it (DesertEnding), so a level left halfway starts again with a round head to flatten.
    //
    // The hammer is a placeholder made of plain shapes: a stick, a hard bit, and glue between them.
    public class HammerGame : MonoBehaviour
    {
        public StoryDirector director;

        [Header("The view")]
        [Tooltip("How far the camera stands from the head (m), and how far above level it looks down from (degrees).")]
        public float cameraDistance = 6.2f;
        [Range(0f, 80f)] public float cameraPitch = 14f;
        public float fieldOfView = 36f;
        [Tooltip("The camera's priority: over every other camera of the level.")]
        public int priority = 50;
        [Tooltip("How far round toward the camera the hammer is, from straight to the right of the picture (degrees): the side being hit is seen at that slant.")]
        [Range(0f, 60f)] public float slant = 28f;
        [Tooltip("For the blows from above the character faces the camera, turned this far toward the hammer's side (degrees).")]
        [Range(0f, 60f)] public float facesAside = 20f;
        [Tooltip("After the last blow the view goes in on the face, from straight in front: how near (m), how long it takes and how long it stays (s).")]
        public float faceDistance = 1.9f;
        public float faceTime = 0.9f;
        public float faceStays = 1.6f;

        [Header("The hammer")]
        public Material handleMaterial;
        public Material headMaterial;
        public Material glueMaterial;
        [Tooltip("How far from the head's side the hammer waits (m): beside it, and over it.")]
        public float waitsAt = 1.3f;
        public float waitsOver = 0.75f;
        [Tooltip("Seconds: the blow, the hammer staying where it landed, and its going back.")]
        public float blowTime = 0.08f;
        public float stayTime = 0.22f;
        public float backTime = 0.4f;
        [Tooltip("Seconds the character takes to turn its next side to the hammer.")]
        public float turnTime = 0.7f;
        [Tooltip("Seconds the artifacts take to come together.")]
        public float gatherTime = 1.4f;

        [Header("Faces and words")]
        public string terror = "terror";
        public string pain = "pain";
        public string word = "SMASH!";

        // A blow: the side of the head it flattens (MorphSphere's order: right, left, up, down, front, back),
        // another that goes with it (or -1), how flat they are after it (0 is flat), and where it comes from: the
        // side's own direction as the head has it, or above.
        struct Blow
        {
            public int side, also;
            public float leaves;
            public Vector3 from;
            public bool above;
        }

        static readonly Blow[] Blows =
        {
            new Blow { side = 1, also = -1, leaves = 0f, from = Vector3.left },
            new Blow { side = 5, also = -1, leaves = 0f, from = Vector3.back },
            new Blow { side = 0, also = -1, leaves = 0f, from = Vector3.right },
            new Blow { side = 2, also = 3, leaves = 0.5f, above = true },
            new Blow { side = 2, also = 3, leaves = 0f, above = true },
            new Blow { side = 4, also = -1, leaves = 0f, from = Vector3.forward },
        };

        // From the middle of a sphere of radius 1 to a side of the cube inside it.
        const float CubeHalf = 0.57735027f;
        // The hammer's head: this long along the blow, and this thick.
        const float HeadLength = 0.5f;
        const float HeadThick = 0.3f;

        static readonly int TiltShiftDisabledId = Shader.PropertyToID("_TiltShiftDisabled");

        public bool Playing { get; private set; }

        // A blow asked for by something other than the button (a test; a click, one day).
        bool asked;

        public void Smash()
        {
            asked = true;
        }

        // For a Cutscene's onStart.
        public void Play()
        {
            if (!Playing)
                StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            Playing = true;
            Transform player = director != null ? director.player : PlayerMovement.Current.transform;
            PlayerControl control = PlayerControl.Of(player);
            control.Take(this);
            TelevisionHead head = player.GetComponentInChildren<TelevisionHead>();
            MorphSphere sphere = head.GetComponent<MorphSphere>();
            ScreenFace face = player.GetComponentInChildren<ScreenFace>();
            head.keepFullSize = true;
            Quaternion facedBefore = player.rotation;
            float radius = head.FullRadius;

            // The view: from where the game's own camera is looking, round the character at the same bearing,
            // so that the picture does not swing about as it changes.
            Camera view = Camera.main;
            Vector3 ahead = view != null ? Vector3.ProjectOnPlane(view.transform.forward, Vector3.up) : -player.forward;
            ahead = ahead.sqrMagnitude > 0.001f ? ahead.normalized : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, ahead);
            Vector3 middle = head.transform.position;
            Vector3 looking = Quaternion.AngleAxis(cameraPitch, right) * ahead;
            Vector3 aim = middle + Vector3.up * 0.1f + right * 0.45f;

            var cameraObject = new GameObject("CM Hammer Camera");
            cameraObject.SetActive(false);
            var camera = cameraObject.AddComponent<CinemachineCamera>();
            LensSettings lens = camera.Lens;
            lens.FieldOfView = fieldOfView;
            camera.Lens = lens;
            // Over whatever camera the conversation has on (the television's wide one).
            camera.Priority = new PrioritySettings { Enabled = true, Value = priority };
            cameraObject.transform.SetPositionAndRotation(aim - looking * cameraDistance, Quaternion.LookRotation(looking, Vector3.up));
            Main.inst.camera.EnterStudio(cameraObject);

            // Where the hammer is: to the right of the picture, and round toward the camera a little.
            Vector3 side = (right * Mathf.Cos(slant * Mathf.Deg2Rad) - ahead * Mathf.Sin(slant * Mathf.Deg2Rad)).normalized;
            Quaternion fromTheSide = Quaternion.LookRotation(Vector3.Cross(side, Vector3.up), Vector3.up);
            Quaternion fromAbove = fromTheSide * Quaternion.Euler(0f, 0f, 90f);
            // Which way the character faces for a blow from above: at the camera, and a little to the hammer's
            // side.
            Vector3 atTheCamera = (-ahead * Mathf.Cos(facesAside * Mathf.Deg2Rad) + right * Mathf.Sin(facesAside * Mathf.Deg2Rad)).normalized;

            if (face != null)
                face.Show(terror, true);

            // The artifacts come together where the hammer will be, and are the hammer.
            Transform hammer = Build(player.gameObject.layer);
            hammer.gameObject.SetActive(false);
            Vector3 waiting = middle + side * (radius + HeadLength * 0.5f + waitsAt);
            yield return Gather(waiting, player.gameObject.layer);
            hammer.SetPositionAndRotation(waiting, fromTheSide);
            hammer.gameObject.SetActive(true);
            for (float t = 0f; t < 0.3f; t += Time.deltaTime)
            {
                // In with a little too much, and back.
                float shown = t / 0.3f;
                hammer.localScale = Vector3.one * (shown + Mathf.Sin(shown * Mathf.PI) * 0.25f);
                yield return null;
            }
            hammer.localScale = Vector3.one;

            TextMeshProUGUI words = MakeWords();
            Vector3 hammerWas = waiting;
            Quaternion hammerTurned = fromTheSide;

            int struck = 0;
            foreach (Blow blow in Blows)
            {
                Vector3 along = blow.above ? Vector3.up : side;
                Quaternion turned = blow.above ? fromAbove : fromTheSide;

                // The side to be hit turned to the hammer (for a blow from above, the face to the camera), and
                // the hammer to where it waits for this blow.
                float away = blow.above ? waitsOver : waitsAt;
                Vector3 now = Vector3.ProjectOnPlane(head.transform.TransformDirection(blow.above ? Vector3.forward : blow.from), Vector3.up);
                Quaternion faces = Quaternion.AngleAxis(Vector3.SignedAngle(now, blow.above ? atTheCamera : side, Vector3.up), Vector3.up) * player.rotation;
                Quaternion facedAt = player.rotation;
                float surface = radius * Mathf.Lerp(CubeHalf, 1f, sphere[blow.side]);
                for (float t = 0f; t < turnTime; t += Time.deltaTime)
                {
                    float done = Mathf.SmoothStep(0f, 1f, t / turnTime);
                    player.rotation = Quaternion.Slerp(facedAt, faces, done);
                    Vector3 to = head.transform.position + along * (surface + HeadLength * 0.5f + away);
                    hammer.SetPositionAndRotation(Vector3.Lerp(hammerWas, to, done), Quaternion.Slerp(hammerTurned, turned, done));
                    yield return null;
                }
                player.rotation = faces;

                // Waiting for the press: the hammer weighed in the hand, the words under it.
                words.gameObject.SetActive(true);
                float waited = 0f;
                asked = false;
                while (true)
                {
                    waited += Time.deltaTime;
                    Vector3 at = head.transform.position + along * (surface + HeadLength * 0.5f + away + Mathf.Sin(waited * 5f) * 0.07f);
                    hammer.SetPositionAndRotation(at, turned);
                    SetWords(words, waited);
                    // (Not the press that sent the last line of the conversation on its way.)
                    if (waited > 0.25f && (Main.inst.input.interactPressed || asked))
                        break;
                    yield return null;
                }
                words.gameObject.SetActive(false);

                // The blow: back a little, then in, faster and faster.
                Vector3 raised = hammer.position;
                float flatAt = radius * Mathf.Lerp(CubeHalf, 1f, blow.leaves);
                for (float t = 0f; t < blowTime + 0.12f; t += Time.deltaTime)
                {
                    Vector3 lands = head.transform.position + along * (flatAt + HeadLength * 0.5f);
                    if (t < 0.12f)
                    {
                        hammer.position = raised + along * (Mathf.Sin(t / 0.12f * Mathf.PI * 0.5f) * 0.35f);
                    }
                    else
                    {
                        float done = (t - 0.12f) / blowTime;
                        hammer.position = Vector3.Lerp(raised + along * 0.35f, lands, done * done);
                    }
                    yield return null;
                }

                // It lands: the side goes in under it, the face is one of pain, the body is knocked.
                if (face != null)
                    face.Show(pain, true);
                float before = sphere[blow.side];
                Vector3 stood = player.position;
                for (float t = 0f; t < stayTime; t += Time.deltaTime)
                {
                    float done = Mathf.Clamp01(t / 0.07f);
                    float flat = Mathf.Lerp(before, blow.leaves, done);
                    sphere[blow.side] = flat;
                    if (blow.also >= 0)
                        sphere[blow.also] = flat;
                    // Knocked away from the blow, and back.
                    float knock = Mathf.Sin(Mathf.Clamp01(t / stayTime) * Mathf.PI) * 0.09f;
                    player.position = stood - along * knock;
                    hammer.position = head.transform.position + along * (radius * Mathf.Lerp(CubeHalf, 1f, flat) + HeadLength * 0.5f);
                    yield return null;
                }
                sphere[blow.side] = blow.leaves;
                if (blow.also >= 0)
                    sphere[blow.also] = blow.leaves;
                player.position = stood;

                // And back to where it waits.
                surface = radius * Mathf.Lerp(CubeHalf, 1f, blow.leaves);
                Vector3 landed = hammer.position;
                for (float t = 0f; t < backTime; t += Time.deltaTime)
                {
                    Vector3 to = head.transform.position + along * (surface + HeadLength * 0.5f + away);
                    hammer.position = Vector3.Lerp(landed, to, Mathf.SmoothStep(0f, 1f, t / backTime));
                    yield return null;
                }
                // (After the last blow the face stays as the blow left it: the view is about to go in on it.)
                struck++;
                if (face != null && struck < Blows.Length)
                    face.Show(terror, true);
                hammerWas = hammer.position;
                hammerTurned = turned;
            }

            // Done. The hammer goes, and the view goes in on the face it has just hit, from straight in front
            // of it.
            Vector3 size = hammer.localScale;
            Transform seenFrom = cameraObject.transform;
            Vector3 seenAt = seenFrom.position;
            Quaternion seenTurned = seenFrom.rotation;
            for (float t = 0f; t < faceTime; t += Time.deltaTime)
            {
                float done = Mathf.SmoothStep(0f, 1f, t / faceTime);
                hammer.localScale = size * Mathf.Clamp01(1f - t / 0.35f);
                Vector3 front = head.transform.forward;
                Vector3 close = head.transform.position + front * (radius * CubeHalf + faceDistance);
                seenFrom.SetPositionAndRotation(Vector3.Lerp(seenAt, close, done), Quaternion.Slerp(seenTurned, Quaternion.LookRotation(-front, Vector3.up), done));
                yield return null;
            }
            Destroy(hammer.gameObject);
            Destroy(words.gameObject);
            yield return new WaitForSeconds(faceStays);
            if (face != null)
                face.ShowIdle();

            player.rotation = facedBefore;
            head.keepFullSize = false;
            Main.inst.camera.ExitStudio(cameraObject);
            Destroy(cameraObject, 1f);
            // The conversation this is in has the picture plain, without the blur at its top and bottom, which
            // leaving the studio has just put back.
            if (Main.inst.dialogue.IsTalking())
                Shader.SetGlobalFloat(TiltShiftDisabledId, 1f);
            control.Release(this);
            Playing = false;
            Cutscene.FinishRunning();
        }

        // The artifacts that are won fly to a place and are gone.
        IEnumerator Gather(Vector3 to, int layer)
        {
            var coming = new List<Transform>();
            var starts = new List<Vector3>();
            var sizes = new List<Vector3>();
            if (director != null)
            {
                foreach (Artifact artifact in director.artifacts)
                {
                    if (artifact == null || !artifact.isActiveAndEnabled || !artifact.Won)
                        continue;
                    // No longer following; and seen on black, where only the character's layer is.
                    artifact.enabled = false;
                    foreach (Transform part in artifact.GetComponentsInChildren<Transform>(true))
                        part.gameObject.layer = layer;
                    coming.Add(artifact.transform);
                    starts.Add(artifact.transform.position);
                    sizes.Add(artifact.transform.localScale);
                }
            }

            for (float t = 0f; t < gatherTime && coming.Count > 0; t += Time.deltaTime)
            {
                float done = t / gatherTime;
                for (int i = 0; i < coming.Count; i++)
                {
                    // Round each other as they close, each a third of a turn from the next.
                    float turn = done * 9f + i * Mathf.PI * 2f / coming.Count;
                    Vector3 round = (Vector3.up * Mathf.Sin(turn) + Vector3.right * Mathf.Cos(turn)) * ((1f - done) * 0.5f);
                    coming[i].position = Vector3.Lerp(starts[i], to, Mathf.SmoothStep(0f, 1f, done)) + round * Mathf.Sin(done * Mathf.PI);
                    coming[i].localScale = sizes[i] * Mathf.Lerp(1f, 0.4f, done * done);
                }
                yield return null;
            }
            foreach (Transform one in coming)
                one.gameObject.SetActive(false);
        }

        // The hammer, about the middle of its head. Its head lies along its own left-to-right line and strikes
        // with its left end; the handle hangs down from the head's middle.
        Transform Build(int layer)
        {
            var hammer = new GameObject("Hammer").transform;
            Part(hammer, PrimitiveType.Cube, headMaterial, Vector3.zero, Quaternion.identity, new Vector3(HeadLength, HeadThick, HeadThick));
            Part(hammer, PrimitiveType.Cylinder, handleMaterial, new Vector3(0f, -0.6f, 0f), Quaternion.identity, new Vector3(0.09f, 0.5f, 0.09f));
            Part(hammer, PrimitiveType.Sphere, glueMaterial, new Vector3(0f, -HeadThick * 0.5f, 0f), Quaternion.identity, new Vector3(0.26f, 0.18f, 0.26f));
            foreach (Transform part in hammer.GetComponentsInChildren<Transform>(true))
                part.gameObject.layer = layer;
            return hammer;
        }

        static void Part(Transform parent, PrimitiveType shape, Material material, Vector3 at, Quaternion turned, Vector3 size)
        {
            GameObject part = GameObject.CreatePrimitive(shape);
            Destroy(part.GetComponent<Collider>());
            part.transform.SetParent(parent, false);
            part.transform.localPosition = at;
            part.transform.localRotation = turned;
            part.transform.localScale = size;
            if (material != null)
                part.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        // "SMASH!" with its button, low in the middle of the screen: made from the prompt the story shows over
        // what can be talked to, so that it is in the same letters.
        TextMeshProUGUI MakeWords()
        {
            DialogueMan dialogue = Main.inst.dialogue;
            TextMeshProUGUI words = Instantiate(dialogue.textPrompt, dialogue.textPrompt.canvas.transform);
            words.name = "Hammer Words";
            if (words.TryGetComponent(out CanvasGroup group))
                group.alpha = 1f;
            words.spriteAsset = InputPrompts.SpriteAsset(true);
            words.alignment = TextAlignmentOptions.Center;
            words.fontSize = 72f;
            words.textWrappingMode = TextWrappingModes.NoWrap;
            RectTransform rect = words.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(900f, 140f);
            words.gameObject.SetActive(false);
            return words;
        }

        void SetWords(TextMeshProUGUI words, float waited)
        {
            // Formatted afresh each time: the button's picture is the one for whatever is being played with.
            string text = InputPrompts.Format(Main.inst.dialogue.promptButton + " " + word, out bool _);
            if (words.text != text)
                words.text = text;
            words.rectTransform.localScale = Vector3.one * (1f + 0.08f * Mathf.Sin(waited * 7f));
        }
    }
}
