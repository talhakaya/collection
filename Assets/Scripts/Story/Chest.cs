using System.Collections;
using UnityEngine;

namespace Collection.Story
{
    // A chest of coins. It is opened like something is talked to: its NPCTrigger (on the same object, with no
    // conversation) shows the prompt and calls Open from onActivate.
    //
    // Opening it: the character stands still, the view goes to the chest's own camera, the lid swings up, the
    // coins come out one after the other and fly to the character, and the view goes back. It stays open in the
    // save slot.
    public class Chest : Spendable
    {
        [Tooltip("The lid's hinge: turned about its own left-to-right line.")]
        public Transform lid;
        public float openAngle = 110f;
        [Tooltip("Seconds the lid takes.")]
        public float lidTime = 0.7f;
        [Tooltip("The chest's Cinemachine camera, a child of it, switched off: where the opening is seen from.")]
        public GameObject view;
        [Tooltip("Seconds for the view to get there before the lid moves.")]
        public float viewTime = 0.9f;

        [Header("Coins")]
        public int coins = 20;
        public Coin coin;
        [Tooltip("Where the coins come out.")]
        public Transform mouth;
        [Tooltip("Seconds between one coin and the next.")]
        public float coinsApart = 0.06f;
        [Tooltip("The face the character makes as the lid opens.")]
        public string emote = "smile";

        bool opened;

        void Start()
        {
            if (view != null)
                view.SetActive(false);
            if (!Spent)
                return;
            opened = true;
            lid.localRotation = Quaternion.Euler(-openAngle, 0f, 0f);
            NothingToOpen();
        }

        void NothingToOpen()
        {
            if (TryGetComponent(out NPCTrigger trigger))
                trigger.enabled = false;
            foreach (Collider part in GetComponents<Collider>())
                if (part.isTrigger)
                    part.enabled = false;
        }

        // For the NPCTrigger's onActivate.
        public void Open()
        {
            if (opened)
                return;
            opened = true;
            Spend();
            NothingToOpen();
            StartCoroutine(Opening());
        }

        IEnumerator Opening()
        {
            InputMan input = Main.inst.input;
            input.held = true;
            if (view != null)
                view.SetActive(true);
            yield return new WaitForSeconds(viewTime);

            ScreenFace face = FindFirstObjectByType<ScreenFace>();
            if (face != null && !string.IsNullOrEmpty(emote))
                face.Show(emote);

            // Up past where it stops and back a little, like a lid thrown open.
            for (float t = 0f; t < lidTime; t += Time.deltaTime)
            {
                float done = t / lidTime;
                float swing = 1f - Mathf.Pow(1f - done, 3f) + Mathf.Sin(done * Mathf.PI) * 0.12f;
                lid.localRotation = Quaternion.Euler(-openAngle * swing, 0f, 0f);
                yield return null;
            }
            lid.localRotation = Quaternion.Euler(-openAngle, 0f, 0f);

            if (coin != null && coins > 0)
            {
                Coin.Spill(coin, coins, mouth != null ? mouth.position : transform.position + Vector3.up * 0.5f, coinsApart);
                yield return new WaitForSeconds(coins * coinsApart + 1.2f);
            }

            if (view != null)
                view.SetActive(false);
            input.held = false;
        }
    }
}
