using Collection.Saving;
using TMPro;
using UnityEngine;

namespace Collection.Story
{
    // How many coins the character has, on the screen: shown when a coin is got, for `shownFor` seconds after the
    // last one, then faded away. The coins themselves are in the save slot.
    public class CoinCounter : MonoBehaviour
    {
        public CanvasGroup group;
        public TextMeshProUGUI text;
        public float shownFor = 5f;
        public float fadeTime = 0.8f;

        static CoinCounter inst;
        float sinceLast = float.MaxValue;
        int shown = -1;

        public static int Coins => SaveManager.Slot.story.coins;

        void Awake()
        {
            inst = this;
            group.alpha = 0f;
        }

        public static void Add(int coins)
        {
            SaveManager.Slot.story.coins += coins;
            SaveManager.MarkDirty();
            if (inst != null)
                inst.sinceLast = 0f;
        }

        // Shows the counter without anything being got: for a shop, say.
        public static void Show()
        {
            if (inst != null)
                inst.sinceLast = 0f;
        }

        void Update()
        {
            sinceLast += Time.unscaledDeltaTime;
            group.alpha = Mathf.Clamp01(1f - (sinceLast - shownFor) / fadeTime);
            if (group.alpha <= 0f)
                return;

            int coins = Coins;
            if (coins != shown)
            {
                shown = coins;
                text.text = coins == 1 ? "1 coin" : coins + " coins";
            }
        }
    }
}
