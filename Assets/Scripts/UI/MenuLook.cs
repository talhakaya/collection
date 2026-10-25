using UnityEngine;

namespace Collection.UI
{
    /// <summary>
    /// How the collection's menus look and move, for Menus: Resources/Menu/Menu Look. Read every
    /// frame, so changing it while playing shows at once (except the number of title copies,
    /// which is read as a screen opens).
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/Menu Look")]
    public class MenuLook : ScriptableObject
    {
        [Header("Dancing title")]
        [Tooltip("The title's size on the 1080-high canvas.")]
        public float titleSize = 150f;
        [Tooltip("How far down from the top of the screen the title's middle is.")]
        public float titleFromTop = 150f;
        [Tooltip("Every letter's slot, as a part of the title's size: the same for all, so that none moves as the capital goes along.")]
        public float letterSlot = 0.66f;
        [Tooltip("Seconds the capital stays on each letter.")]
        public float letterTime = 0.18f;
        [Tooltip("A second capital going along the title at the same time, this many times as fast as the first, so that most of the time two letters are capitals. 0 for none.")]
        public float secondCapitalSpeed = 1.5f;

        [Header("Title copies")]
        [Tooltip("How many coloured copies circle behind the title.")]
        [Range(0, 20)] public int copies = 6;
        [Tooltip("How far from the title the nearest copy circles (canvas pixels).")]
        public float copyDistance = 9f;
        [Tooltip("How much further out each copy circles than the one before (canvas pixels).")]
        public float copyStep = 9f;
        [Tooltip("Turns a second.")]
        public float spin = 0.7f;
        [Tooltip("How far behind each copy is of the one before (a part of a turn).")]
        public float lag = 0.07f;
        [Tooltip("How many times a second the copies are moved: few is jerky, like Crime Factory's 24.")]
        public float stepsPerSecond = 24f;
        [Tooltip("The chance, each step, that a copy changes to another of the colours.")]
        [Range(0f, 1f)] public float recolourChance = 0.04f;
        public Color[] colours =
        {
            new Color(1f, 0.27f, 0.3f), new Color(1f, 0.6f, 0.15f), new Color(1f, 0.92f, 0.25f),
            new Color(0.3f, 0.9f, 0.42f), new Color(0.25f, 0.6f, 1f), new Color(0.7f, 0.38f, 1f),
        };

        [Header("Buttons")]
        [Tooltip("Pictures a second as a button plays through its nine.")]
        public float frameRate = 15f;
        [Tooltip("How white a selected button is.")]
        [Range(0f, 1f)] public float selectedWhiteness = 0.17f;
        [Tooltip("Seconds a chosen button that opens another screen stays white first.")]
        public float chosenPause = 0.15f;
    }
}
