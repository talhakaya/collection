using System.Collections;
using UnityEngine;

namespace Collection.Story
{
    // A barrel, broken by rolling into it (PlayerRoll) or riding into it (Bicycle).
    //
    // It is built of its pieces - the staves round it and its lid - standing still, with one collider for the
    // whole. Breaking it takes that collider away and lets the pieces go, each a rigid body of its own, thrown
    // the way the character was going; after a few seconds they shrink away. Sometimes coins come out. Its bottom
    // stays on the ground for good, to show a barrel stood there, and the save slot remembers it broken: coming
    // back to the level, only the bottom is there.
    public class Barrel : Spendable
    {
        [Tooltip("The collider of the whole barrel.")]
        public Collider body;
        [Tooltip("What flies apart. Each has a collider, switched off until then.")]
        public Transform[] pieces;

        [Header("Breaking")]
        [Tooltip("How much of the character's speed the pieces take.")]
        public float carry = 0.7f;
        [Tooltip("How hard they are thrown outward and up (m/s).")]
        public float burst = 2.5f;
        [Tooltip("Seconds before the pieces go.")]
        public float piecesLast = 4f;

        [Header("Coins")]
        [Range(0f, 1f)] public float coinChance = 0.4f;
        public int coinsFewest = 1;
        public int coinsMost = 3;
        public Coin coin;

        bool broken;

        void Start()
        {
            if (!Spent)
                return;
            broken = true;
            body.enabled = false;
            foreach (Transform piece in pieces)
                if (piece != null)
                    Destroy(piece.gameObject);
        }

        // `velocity`: the speed of what hit it. `by`: its collider, which the pieces do not get in the way of.
        // False when it was broken already.
        public bool Break(Vector3 velocity, Collider by)
        {
            if (broken)
                return false;
            broken = true;
            Spend();
            body.enabled = false;

            Vector3 middle = body.bounds.center;
            foreach (Transform piece in pieces)
            {
                if (piece == null)
                    continue;
                Collider part = piece.GetComponent<Collider>();
                if (part != null)
                {
                    part.enabled = true;
                    if (by != null)
                        Physics.IgnoreCollision(part, by);
                }

                Vector3 outward = piece.position - middle;
                outward.y = 0f;
                outward = outward.sqrMagnitude > 0.0001f ? outward.normalized : Random.insideUnitSphere;

                var rigid = piece.gameObject.AddComponent<Rigidbody>();
                rigid.mass = 0.6f;
                rigid.linearVelocity = velocity * (carry * Random.Range(0.6f, 1.2f))
                    + outward * (burst * Random.Range(0.4f, 1f))
                    + Vector3.up * (burst * Random.Range(0.5f, 1.3f));
                rigid.angularVelocity = Random.insideUnitSphere * 8f;
                StartCoroutine(Go(piece));
            }

            if (coin != null && Random.value < coinChance)
                Coin.Spill(coin, Random.Range(coinsFewest, coinsMost + 1), middle, 0f);
            return true;
        }

        IEnumerator Go(Transform piece)
        {
            yield return new WaitForSeconds(piecesLast * Random.Range(0.8f, 1.2f));
            Vector3 size = piece.localScale;
            for (float t = 0f; t < 0.4f; t += Time.deltaTime)
            {
                piece.localScale = size * (1f - t / 0.4f);
                yield return null;
            }
            Destroy(piece.gameObject);
        }
    }
}
