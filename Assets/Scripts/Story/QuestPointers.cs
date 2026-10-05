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

        readonly List<Image> arrows = new List<Image>();
        readonly List<Vector3> targets = new List<Vector3>();
        readonly List<Color> colours = new List<Color>();
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
            targets.Clear();
            colours.Clear();
            Camera view = Camera.main;
            bool quiet = view == null || Main.inst.dialogue.IsTalking() || Collection.Controls.TaloketoInputManager.Blocked;

            if (!quiet && director != null)
            {
                foreach (Artifact artifact in director.artifacts)
                    if (artifact != null && artifact.isActiveAndEnabled && !artifact.Won)
                        Add(artifact.transform.position, colour);

                // The television: there, and not met yet.
                NPCTrigger meeting = director.televisionTrigger;
                bool waiting = meeting != null && meeting.isActiveAndEnabled && director.television.activeInHierarchy;
                televisionWaited = waiting ? televisionWaited + Time.deltaTime : 0f;
                // And when it has something new to say: that trigger is only switched on then, and off once said.
                NPCTrigger again = director.allArtifactsTrigger;
                bool calling = again != null && again.isActiveAndEnabled && director.television.activeInHierarchy;
                if ((waiting && televisionWaited >= televisionAfter) || calling)
                    Add(director.television.transform.position + Vector3.up * televisionHeight, televisionColour);

                if (bicycle != null && bicycle.isActiveAndEnabled && !bicycle.Ridden)
                    Add(bicycle.frame.position + Vector3.up * 0.6f, bicycleColour);
            }

            int shown = 0;
            for (int t = 0; t < targets.Count; t++)
            {
                Vector3 target = targets[t];
                Vector3 at = view.WorldToViewportPoint(target);
                bool seen = at.z > 0f && at.x > edge && at.x < 1f - edge && at.y > edge && at.y < 1f - edge;
                if (seen)
                    continue;

                // The way to it from the middle of the screen. Behind the camera, what the camera gives is the
                // way to its opposite.
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

                Image arrow = Arrow(shown++);
                arrow.color = colours[t];
                RectTransform rect = arrow.rectTransform;
                rect.anchoredPosition = way * scale;
                rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(way.y, way.x) * Mathf.Rad2Deg - 90f);
                float beat = 1f + 0.12f * Mathf.Sin(Time.unscaledTime * 5f);
                rect.sizeDelta = new Vector2(size, size) * beat;
            }

            for (int i = 0; i < arrows.Count; i++)
                arrows[i].enabled = i < shown;
        }

        void Add(Vector3 target, Color arrowColour)
        {
            targets.Add(target);
            colours.Add(arrowColour);
        }

        Image Arrow(int index)
        {
            while (arrows.Count <= index)
            {
                var go = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
                go.layer = gameObject.layer;
                go.transform.SetParent(transform, false);
                Image image = go.GetComponent<Image>();
                image.sprite = shape;
                image.color = colour;
                image.raycastTarget = false;
                var outline = go.AddComponent<Shadow>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
                outline.effectDistance = new Vector2(2f, -2f);
                arrows.Add(image);
            }
            return arrows[index];
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
