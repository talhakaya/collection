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
        // Coins that are the character's already but are still flying to it: not counted on the screen yet.
        int onTheWay;

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

        // Coins that have come out of something and are on their way to the character (Coin). They are in the slot
        // from this moment, all at once: the flight is for show, and leaving in the middle of it loses nothing.
        // The screen counts each as it gets there (Arrived).
        public static void Sent(int coins)
        {
            SaveManager.Slot.story.coins += coins;
            SaveManager.MarkDirty();
            if (inst != null)
                inst.onTheWay += coins;
        }

        public static void Arrived()
        {
            if (inst == null)
                return;
            inst.onTheWay = Mathf.Max(0, inst.onTheWay - 1);
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

            int coins = Coins - onTheWay;
            if (coins != shown)
            {
                shown = coins;
                text.text = coins == 1 ? "1 coin" : coins + " coins";
            }
        }
    }
}
