using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collection.Story
{
    // Arrows at the edges of the screen toward what the story is asking for, each shown only while the thing
    // itself is out of the picture:
    //
    //   the artifacts not won yet, once they are there to be won (the land is up);
    //
    //   the giant television, when it has been waiting to be found in the sea for `televisionAfter` seconds,
    //   and again once every artifact is won and it has something new to say;
    //
    //   the bicycle, while nobody is riding it.
    //
    // "Out of the picture" is about where it is, not about what is in front of it: something behind a hill but
    // within the screen's edges counts as seen.
    //
    // On a RectTransform that fills the story's canvas; the arrows are made here.
    public class QuestPointers : MonoBehaviour
    {
        public StoryDirector director;
        [Tooltip("Seconds the television waits in the sea before it is pointed to.")]
        public float televisionAfter = 30f;
        [Tooltip("The height on the television that is pointed to, and that counts as seeing it (m above its base).")]
        public float televisionHeight = 10f;

        [Tooltip("Pointed to while it is not being ridden. Empty: none.")]
        public Bicycle bicycle;

        [Header("Look")]
        [Tooltip("The arrows to the artifacts, to the television, and to the bicycle.")]
        public Color colour = new Color(1f, 0.95f, 0.8f, 0.95f);
        public Color televisionColour = new Color(0.75f, 0.9f, 1f, 0.95f);
        public Color bicycleColour = new Color(1f, 0.45f, 0.4f, 0.95f);
        [Tooltip("The arrow's size, and how far in from the screen's edge it sits (canvas units).")]
        public float size = 56f;
        public float inset = 70f;
        [Tooltip("How far inside the screen's edge something still counts as out of the picture, as a part of the screen.")]
        [Range(0f, 0.2f)] public float edge = 0.04f;
        [Tooltip("Seconds an arrow takes to fade in or out.")]
        public float fadeTime = 0.6f;

        // One arrow for each thing that can be pointed to, kept for as long as the scene: the artifacts in the
        // director's order, then the television, then the bicycle. Each fades in when it is wanted and out when
        // it is not.
        class Pointer
        {
            public Image image;
            public Color colour;
            public float shown;
            public bool wanted;
        }

        readonly List<Pointer> pointers = new List<Pointer>();
        RectTransform area;
        Sprite shape;
        float televisionWaited;

        void Awake()
        {
            area = (RectTransform)transform;
            shape = MakeArrow();
        }

        void LateUpdate()
        {
            foreach (Pointer pointer in pointers)
                pointer.wanted = false;

            Camera view = Camera.main;
            bool quiet = view == null || Main.inst.dialogue.IsTalking() || Collection.Controls.TaloketoInputManager.Blocked;

            if (!quiet && director != null)
            {
                int slot = 0;
                foreach (Artifact artifact in director.artifacts)
                {
                    if (artifact != null && artifact.isActiveAndEnabled && !artifact.Won)
                        Point(slot, view, artifact.transform.position, colour);
                    slot++;
                }

                // The television: there, and not met yet.
                NPCTrigger meeting = director.televisionTrigger;
                bool waiting = meeting != null && meeting.isActiveAndEnabled && director.television.activeInHierarchy;
                televisionWaited = waiting ? televisionWaited + Time.deltaTime : 0f;
                // And when it has something new to say: that trigger is only switched on then, and off once said.
                NPCTrigger again = director.allArtifactsTrigger;
                bool calling = again != null && again.isActiveAndEnabled && director.television.activeInHierarchy;
                if ((waiting && televisionWaited >= televisionAfter) || calling)
                    Point(slot, view, director.television.transform.position + Vector3.up * televisionHeight, televisionColour);
                slot++;

                if (bicycle != null && bicycle.isActiveAndEnabled && !bicycle.Ridden)
                    Point(slot, view, bicycle.frame.position + Vector3.up * 0.6f, bicycleColour);
            }

            // (A frame is never counted as longer than a twentieth of a second: the first after a scene has loaded
            // is as long as the loading, and would have every arrow there at once.)
            float step = fadeTime > 0f ? Mathf.Min(Time.unscaledDeltaTime, 0.05f) / fadeTime : 1f;
            float beat = 1f + 0.12f * Mathf.Sin(Time.unscaledTime * 5f);
            foreach (Pointer pointer in pointers)
            {
                pointer.shown = Mathf.MoveTowards(pointer.shown, pointer.wanted ? 1f : 0f, step);
                pointer.image.enabled = pointer.shown > 0f;
                Color faded = pointer.colour;
                faded.a *= Mathf.SmoothStep(0f, 1f, pointer.shown);
                pointer.image.color = faded;
                pointer.image.rectTransform.sizeDelta = new Vector2(size, size) * beat;
            }
        }

        // The arrow in `slot` toward `target`, if that is out of the picture. While it is in the picture the arrow
        // is not wanted, and stays where it last was as it fades.
        void Point(int slot, Camera view, Vector3 target, Color arrowColour)
        {
            Vector3 at = view.WorldToViewportPoint(target);
            bool seen = at.z > 0f && at.x > edge && at.x < 1f - edge && at.y > edge && at.y < 1f - edge;
            if (seen)
            {
                // In the picture: the arrow is not wanted, and as it fades it leaves the edge for the thing
                // itself.
                if (slot < pointers.Count && pointers[slot].shown > 0f)
                {
                    RectTransform fading = pointers[slot].image.rectTransform;
                    Vector2 size = area.rect.size;
                    Vector2 onIt = new Vector2((at.x - 0.5f) * size.x, (at.y - 0.5f) * size.y);
                    fading.anchoredPosition = Vector2.Lerp(fading.anchoredPosition, onIt, 1f - Mathf.Exp(-7f * Mathf.Min(Time.unscaledDeltaTime, 0.05f)));
                }
                return;
            }

            // The way to it from the middle of the screen. Behind the camera, what the camera gives is the way to
            // its opposite.
            Vector2 way = new Vector2(at.x - 0.5f, at.y - 0.5f);
            if (at.z < 0f)
                way = -way;
            Vector2 half = area.rect.size * 0.5f;
            way = new Vector2(way.x * half.x, way.y * half.y);
            if (way.sqrMagnitude < 0.0001f)
                way = Vector2.down;

            // Out along that way to the edge, less the inset.
            Vector2 reach = new Vector2(Mathf.Max(1f, half.x - inset), Mathf.Max(1f, half.y - inset));
            float scale = Mathf.Min(reach.x / Mathf.Max(0.0001f, Mathf.Abs(way.x)), reach.y / Mathf.Max(0.0001f, Mathf.Abs(way.y)));

            Pointer pointer = Slot(slot);
            pointer.wanted = true;
            pointer.colour = arrowColour;
            RectTransform rect = pointer.image.rectTransform;
            // Straight to the edge when it is new; back out to it, not a jump, when it was on its way in.
            Vector2 atEdge = way * scale;
            rect.anchoredPosition = pointer.shown <= 0f ? atEdge
                : Vector2.Lerp(rect.anchoredPosition, atEdge, 1f - Mathf.Exp(-12f * Mathf.Min(Time.unscaledDeltaTime, 0.05f)));
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(way.y, way.x) * Mathf.Rad2Deg - 90f);
        }

        Pointer Slot(int index)
        {
            while (pointers.Count <= index)
            {
                var go = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
                go.layer = gameObject.layer;
                go.transform.SetParent(transform, false);
                Image image = go.GetComponent<Image>();
                image.sprite = shape;
                image.raycastTarget = false;
                image.enabled = false;
                var outline = go.AddComponent<Shadow>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
                outline.effectDistance = new Vector2(2f, -2f);
                pointers.Add(new Pointer { image = image, colour = colour });
            }
            return pointers[index];
        }

        // An arrowhead pointing up: a triangle with a notch in its base.
        static Sprite MakeArrow()
        {
            const int n = 64;
            var texture = new Texture2D(n, n, TextureFormat.RGBA32, false) { name = "Quest Arrow", wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[n * n];
            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    float u = (x + 0.5f) / n - 0.5f, v = (y + 0.5f) / n;
                    // Inside the triangle from the base's corners to the tip, and above the notch.
                    float side = 0.46f * (1f - v) - Mathf.Abs(u);
                    float notch = v - (0.28f - Mathf.Abs(u) * 0.6f);
                    float inside = Mathf.Clamp01(Mathf.Min(side, notch) * n * 0.7f);
                    pixels[y * n + x] = new Color(1f, 1f, 1f, inside);
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
