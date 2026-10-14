using Collection.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collection.UI
{
    /// <summary>
    /// The collection's own text - the menus, the story - in the handwritten font, or in the
    /// usual one (Liberation Sans, TextMesh Pro's own) when the settings' Font Type is Normal.
    ///
    /// Every text in either of the two is put into the one chosen: the scene's as it loads,
    /// and any other as TextMesh Pro next draws it (a line of dialogue, a prompt made from a
    /// prefab). The games' own scenes (Assets/games) are left as they are, and so is a text
    /// with KeepFont on it.
    /// </summary>
    public static class CollectionFonts
    {
        private const string HandwrittenPath = "Fonts/Handwriting SDF";

        private static bool started;
        private static TMP_FontAsset handwritten;
        private static TMP_FontAsset normal;
        private static bool useNormal;

        public static TMP_FontAsset Handwritten
        {
            get
            {
                Start();
                return handwritten;
            }
        }

        /// The font the collection's text is in now.
        public static TMP_FontAsset Current
        {
            get
            {
                Start();
                return useNormal || handwritten == null ? normal : handwritten;
            }
        }

        /// Handwritten (false) or normal (true), for every text there is and every one to come.
        public static void Use(bool normalFont)
        {
            Start();
            if (normalFont == useNormal) return;

            useNormal = normalFont;
            foreach (TMP_Text text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Fix(text);
            }
        }

        private static void Start()
        {
            if (!started)
            {
                started = true;
                handwritten = Resources.Load<TMP_FontAsset>(HandwrittenPath);
                normal = TMP_Settings.defaultFontAsset;
                if (handwritten == null) Debug.LogError($"CollectionFonts: no font at Resources/{HandwrittenPath}.");
                useNormal = SaveManager.Settings.normalFont;

                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            if (runner == null && Application.isPlaying)
            {
                runner = new GameObject(nameof(CollectionFonts)).AddComponent<Runner>();
                Object.DontDestroyOnLoad(runner.gameObject);
            }
        }

        private static Runner runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Start();
        }

        // Before the scene's first picture, so its text is never seen in the other font.
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
                {
                    Fix(text);
                }
            }
        }

        // Not changed there and then: that is in the middle of TextMesh Pro drawing it, and a font
        // changed then is drawn with the other's material, in pieces. After the frame's scripts.
        private static void OnTextChanged(Object changed)
        {
            if (changed is TMP_Text text && Wrong(text)) waiting.Add(text);
        }

        private static readonly System.Collections.Generic.HashSet<TMP_Text> waiting = new System.Collections.Generic.HashSet<TMP_Text>();

        private class Runner : MonoBehaviour
        {
            private void LateUpdate()
            {
                if (waiting.Count == 0) return;

                foreach (TMP_Text text in waiting)
                {
                    if (text != null) Fix(text);
                }

                waiting.Clear();
            }
        }

        private static void Fix(TMP_Text text)
        {
            if (Wrong(text)) text.font = Current;
        }

        // In one of the two fonts but not the chosen one, and the collection's to change.
        private static bool Wrong(TMP_Text text)
        {
            TMP_FontAsset font = text.font;
            if (font == null || (font != handwritten && font != normal)) return false;

            TMP_FontAsset wanted = Current;
            if (font == wanted || wanted == null) return false;
            if (text.gameObject.scene.path.StartsWith("Assets/games/")) return false;
            return !text.TryGetComponent(out KeepFont _);
        }
    }
}
