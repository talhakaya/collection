using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Collection.Story
{
    // Looks of a blocky character (purely visual, no gameplay effect). Options come from a CharacterCatalog.
    //   height:   uniform scale of the whole character, pivoting at the feet.
    //   fatness:  widens the body parts; the torso and pelvis get the full amount, limbs less, head/hands/feet barely.
    //             Shoulders and hips move outward with the wider body so limbs don't sink into it.
    //   clothing: top, bottom (pants + shoes), jacket, hat and hair from the catalog. Each item colours the body parts
    //             it covers (jacket over top over bottom over skin) and can add extra pieces attached to bones.
    //   face:     two eye quads (catalog eye type, gap between them, height on the face) and an optional mask quad.
    // Colours are applied per renderer, so materials stay shared. Bone lengths are never changed, so humanoid
    // animations still fit.
    //
    // In the collection: the main character's head is a television (MorphSphere, ScreenFace) put on the head bone.
    // For it, televisionHead is ticked: the head cube, the face, the hat and the hair are not shown, and the rest
    // applies as for anyone else.
    public class CharacterAppearance : MonoBehaviour
    {
        [FormerlySerializedAs("palette")] public CharacterCatalog catalog;

        [Range(0.75f, 1.3f)] public float height = 1f;
        [Range(0.7f, 1.6f)] public float fatness = 1f;

        public int skinColor;

        public int topType;
        public int shirtColor;
        public int bottomType;
        public int pantsColor;
        public int shoesColor;
        public int jacketType;
        public int jacketColor;
        public int hatType;
        public int hatColor;
        public int hairType;
        public int hairColor;

        public int eyeType;
        [Tooltip("Distance between the centres of the eyes (m).")]
        [Range(0.06f, 0.22f)] public float eyeGap = 0.14f;
        [Tooltip("Height of the eyes relative to the middle of the face (m).")]
        [Range(-0.08f, 0.1f)] public float faceHeight = 0.02f;

        public bool showMask;
        public int maskType;

        [Tooltip("The head is something else, put on the head bone by hand: hides the head cube, face, hat and hair.")]
        public bool televisionHead;

        [Serializable]
        struct ColorPart
        {
            public Renderer renderer;
            public BodyCoverage part;
        }

        [Serializable]
        struct PartBaseline
        {
            public Transform part;
            public Vector3 scale;
            public float fatnessWeight;
        }

        [Serializable]
        struct BoneBaseline
        {
            public Transform bone;
            public Vector3 localPosition;
            public float outwardPerFatness;
        }

        // Authored sizes, positions and parts, captured once from the default (height 1, fatness 1) character.
        [SerializeField, HideInInspector] List<PartBaseline> parts = new List<PartBaseline>();
        [SerializeField, HideInInspector] List<BoneBaseline> bones = new List<BoneBaseline>();
        [SerializeField, HideInInspector] List<ColorPart> colorParts = new List<ColorPart>();
        [SerializeField, HideInInspector] Transform headPart;
        [SerializeField, HideInInspector] Renderer eyeLeft;
        [SerializeField, HideInInspector] Renderer eyeRight;
        [SerializeField, HideInInspector] Renderer mask;

        // Face quads sit just in front of the head so they don't flicker against it; the mask covers the eyes.
        const float EyeOffset = 0.004f;
        const float MaskOffset = 0.008f;

        // How strongly fatness widens each part (by part name).
        static readonly Dictionary<string, float> FatnessWeights = new Dictionary<string, float>
        {
            { "Part_Torso", 1f }, { "Part_Pelvis", 1f }, { "Part_Neck", 0.5f }, { "Part_Head", 0.1f },
            { "Part_LeftUpperArm", 0.5f }, { "Part_RightUpperArm", 0.5f },
            { "Part_LeftLowerArm", 0.35f }, { "Part_RightLowerArm", 0.35f },
            { "Part_LeftHand", 0.15f }, { "Part_RightHand", 0.15f },
            { "Part_LeftUpperLeg", 0.6f }, { "Part_RightUpperLeg", 0.6f },
            { "Part_LeftLowerLeg", 0.4f }, { "Part_RightLowerLeg", 0.4f },
            { "Part_LeftFoot", 0.2f }, { "Part_RightFoot", 0.2f },
        };

        // Which clothing coverage each body part belongs to (by part name; the head is always skin).
        static readonly Dictionary<string, BodyCoverage> CoverageByPart = new Dictionary<string, BodyCoverage>
        {
            { "Part_Torso", BodyCoverage.Torso }, { "Part_Pelvis", BodyCoverage.Pelvis }, { "Part_Neck", BodyCoverage.Neck },
            { "Part_Head", BodyCoverage.None },
            { "Part_LeftUpperArm", BodyCoverage.UpperArms }, { "Part_RightUpperArm", BodyCoverage.UpperArms },
            { "Part_LeftLowerArm", BodyCoverage.LowerArms }, { "Part_RightLowerArm", BodyCoverage.LowerArms },
            { "Part_LeftHand", BodyCoverage.Hands }, { "Part_RightHand", BodyCoverage.Hands },
            { "Part_LeftUpperLeg", BodyCoverage.UpperLegs }, { "Part_RightUpperLeg", BodyCoverage.UpperLegs },
            { "Part_LeftLowerLeg", BodyCoverage.LowerLegs }, { "Part_RightLowerLeg", BodyCoverage.LowerLegs },
            { "Part_LeftFoot", BodyCoverage.Feet }, { "Part_RightFoot", BodyCoverage.Feet },
        };

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        static MaterialPropertyBlock block;

        void Awake()
        {
            // Parts take the character's layer (Player / Actors), so they show up wherever the character does,
            // e.g. in the cave view.
            SetLayer(transform, gameObject.layer);
            Apply();
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            // Transforms can't be changed safely inside OnValidate, so apply right after it.
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                    Apply();
            };
#endif
        }

        public void Apply()
        {
            // Only characters in a scene (or prefab editing mode) are applied; never the prefab asset on disk.
            if (parts.Count == 0 || !gameObject.scene.IsValid())
                return;
            ApplyProportions();
            if (catalog == null)
                return;
            ApplyColors();
            ApplyFace();
            ApplyClothingPieces();
            ApplyOwnHead();
        }

        void ApplyProportions()
        {
            transform.localScale = Vector3.one * height;

            foreach (var p in parts)
            {
                if (p.part == null)
                    continue;
                p.part.localScale = Widen(p.scale, WidenFactor(p.fatnessWeight), p.part.name.Contains("Arm"));
            }

            foreach (var b in bones)
            {
                if (b.bone == null)
                    continue;
                float side = Mathf.Sign(b.localPosition.x);
                b.bone.localPosition = b.localPosition + Vector3.right * (side * b.outwardPerFatness * (fatness - 1f));
            }
        }

        float WidenFactor(float weight) => 1f + (fatness - 1f) * weight;

        // Widens sideways (x) and front-to-back (z) in the character's space. Arm parts run along x, so for them the
        // thickness axes are y and z.
        static Vector3 Widen(Vector3 v, float widen, bool alongX)
        {
            return alongX ? new Vector3(v.x, v.y * widen, v.z * widen) : new Vector3(v.x * widen, v.y, v.z * widen);
        }

        // ------------------------------------------------------------------ colours

        CharacterCatalog.ClothingOption Top => CharacterCatalog.PickClothing(catalog.tops, topType);
        CharacterCatalog.ClothingOption Bottom => CharacterCatalog.PickClothing(catalog.bottoms, bottomType);
        CharacterCatalog.ClothingOption Jacket => CharacterCatalog.PickClothing(catalog.jackets, jacketType);
        CharacterCatalog.ClothingOption Hat => CharacterCatalog.PickClothing(catalog.hats, hatType);
        CharacterCatalog.ClothingOption Hair => CharacterCatalog.PickClothing(catalog.hairStyles, hairType);

        // Jacket over top over bottom over skin.
        public Color PartColor(BodyCoverage part)
        {
            if (part != BodyCoverage.None)
            {
                if (Covers(Jacket, part)) return catalog.Cloth(jacketColor);
                if (Covers(Top, part)) return catalog.Cloth(shirtColor);
                if (Covers(Bottom, part)) return catalog.Cloth(part == BodyCoverage.Feet ? shoesColor : pantsColor);
            }
            return catalog.Skin(skinColor);
        }

        static bool Covers(CharacterCatalog.ClothingOption item, BodyCoverage part)
        {
            return item != null && (item.coverage & part) != 0;
        }

        void ApplyColors()
        {
            block ??= new MaterialPropertyBlock();
            foreach (var p in colorParts)
            {
                if (p.renderer == null)
                    continue;
                p.renderer.GetPropertyBlock(block);
                block.SetColor(BaseColorId, PartColor(p.part));
                p.renderer.SetPropertyBlock(block);
            }
        }

        // ------------------------------------------------------------------ face

        void ApplyFace()
        {
            if (headPart == null)
                return;

            // The face follows the head cube's front, which moves forward when fatness widens the head.
            Vector3 head = headPart.localPosition;
            Vector3 size = headPart.localScale;
            float front = head.z + size.z * 0.5f;
            float eyeY = head.y + faceHeight;

            if (eyeLeft != null && eyeRight != null)
            {
                eyeLeft.transform.localPosition = new Vector3(head.x - eyeGap * 0.5f, eyeY, front + EyeOffset);
                eyeRight.transform.localPosition = new Vector3(head.x + eyeGap * 0.5f, eyeY, front + EyeOffset);
                Texture2D eyeTexture = catalog.Eyes(eyeType);
                SetTexture(eyeLeft, eyeTexture);
                SetTexture(eyeRight, eyeTexture);
            }

            if (mask != null)
            {
                mask.transform.localPosition = new Vector3(head.x, head.y, front + MaskOffset);
                mask.transform.localScale = new Vector3(size.x, size.y, 1f);
                Texture2D maskTexture = catalog.Mask(maskType);
                mask.enabled = showMask && maskTexture != null;
                SetTexture(mask, maskTexture);
            }
        }

        // After the face, which switches the mask on and off itself.
        void ApplyOwnHead()
        {
            if (headPart != null && headPart.TryGetComponent(out Renderer head))
                head.enabled = !televisionHead;
            if (eyeLeft != null)
                eyeLeft.enabled = !televisionHead;
            if (eyeRight != null)
                eyeRight.enabled = !televisionHead;
            if (mask != null && televisionHead)
                mask.enabled = false;
        }

        static void SetTexture(Renderer renderer, Texture2D texture)
        {
            block ??= new MaterialPropertyBlock();
            block.Clear();
            if (texture != null)
                block.SetTexture(BaseMapId, texture);
            renderer.SetPropertyBlock(block);
        }

        // ------------------------------------------------------------------ clothing pieces

        // Rebuilds the extra pieces of the worn items. They're marked DontSave, so they never end up in scenes or
        // prefabs; they're rebuilt from the catalog whenever the appearance is applied.
        void ApplyClothingPieces()
        {
            foreach (var old in GetComponentsInChildren<ClothingPiece>(true))
            {
                if (Application.isPlaying)
                    Destroy(old.gameObject);
                else
                    DestroyImmediate(old.gameObject);
            }

            AttachPieces(Top, catalog.Cloth(shirtColor));
            AttachPieces(Bottom, catalog.Cloth(pantsColor), catalog.Cloth(shoesColor));
            AttachPieces(Jacket, catalog.Cloth(jacketColor));
            if (!televisionHead)
            {
                AttachPieces(Hat, catalog.Cloth(hatColor));
                AttachPieces(Hair, catalog.Hair(hairColor));
            }
        }

        void AttachPieces(CharacterCatalog.ClothingOption item, Color color, Color? bootColor = null)
        {
            if (item == null || item.pieces == null)
                return;
            block ??= new MaterialPropertyBlock();

            foreach (Transform group in item.pieces.transform)
            {
                Transform bone = FindBone(group.name);
                if (bone == null)
                {
                    Debug.LogWarning($"[CharacterAppearance] {item.name}: no bone named '{group.name}'.", this);
                    continue;
                }
                float widen = WidenFactor(BoneFatnessWeight(group.name));
                bool alongX = group.name.Contains("Arm");

                foreach (Transform source in group)
                {
                    var piece = Instantiate(source.gameObject, bone, false);
                    piece.name = source.name;
                    piece.AddComponent<ClothingPiece>();
                    foreach (var t in piece.GetComponentsInChildren<Transform>(true))
                        t.gameObject.hideFlags = HideFlags.DontSave;
                    SetLayer(piece.transform, gameObject.layer);
                    piece.transform.localPosition = Widen(source.localPosition, widen, alongX);
                    piece.transform.localScale = Widen(source.localScale, widen, alongX);

                    Color pieceColor = bootColor.HasValue && source.name.StartsWith("Boot") ? bootColor.Value : color;
                    foreach (var r in piece.GetComponentsInChildren<Renderer>(true))
                    {
                        r.GetPropertyBlock(block);
                        block.SetColor(BaseColorId, pieceColor);
                        r.SetPropertyBlock(block);
                    }
                }
            }
        }

        Transform FindBone(string boneName)
        {
            foreach (var t in GetComponentsInChildren<Transform>(true))
                if (t.name == boneName && t.GetComponent<ClothingPiece>() == null)
                    return t;
            return null;
        }

        // Pieces widen like the body part on their bone.
        float BoneFatnessWeight(string boneName)
        {
            string part = boneName == "Hips" ? "Part_Pelvis"
                : boneName == "Spine" || boneName == "Chest" || boneName == "UpperChest" ? "Part_Torso"
                : "Part_" + boneName;
            return FatnessWeights.TryGetValue(part, out float weight) ? weight : 0f;
        }

        static void SetLayer(Transform root, int layer)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }

        // ------------------------------------------------------------------ baseline

        // Records the current (default-proportioned) sizes, coloured parts and face parts as the baseline.
        // Run once on the character prefab.
        [ContextMenu("Capture Baseline")]
        public void CaptureBaseline()
        {
            parts.Clear();
            bones.Clear();
            colorParts.Clear();
            foreach (var t in GetComponentsInChildren<Transform>(true))
            {
                if (t.GetComponentInParent<ClothingPiece>() != null)
                    continue;
                if (FatnessWeights.TryGetValue(t.name, out float weight))
                    parts.Add(new PartBaseline { part = t, scale = t.localScale, fatnessWeight = weight });

                var renderer = t.GetComponent<Renderer>();
                if (renderer != null && CoverageByPart.TryGetValue(t.name, out var coverage))
                    colorParts.Add(new ColorPart { renderer = renderer, part = coverage });

                switch (t.name)
                {
                    case "Part_Head": headPart = t; break;
                    case "Part_EyeL": eyeLeft = renderer; break;
                    case "Part_EyeR": eyeRight = renderer; break;
                    case "Part_Mask": mask = renderer; break;
                }
            }

            // Shoulders follow the torso's half width, hips roughly half the pelvis's.
            AddBone("LeftShoulder", FindPart("Part_Torso").x * 0.5f);
            AddBone("RightShoulder", FindPart("Part_Torso").x * 0.5f);
            AddBone("LeftUpperLeg", FindPart("Part_Pelvis").x * 0.25f);
            AddBone("RightUpperLeg", FindPart("Part_Pelvis").x * 0.25f);
        }

        void AddBone(string name, float outwardPerFatness)
        {
            foreach (var t in GetComponentsInChildren<Transform>(true))
                if (t.name == name)
                    bones.Add(new BoneBaseline { bone = t, localPosition = t.localPosition, outwardPerFatness = outwardPerFatness });
        }

        Vector3 FindPart(string name)
        {
            foreach (var p in parts)
                if (p.part.name == name)
                    return p.scale;
            return Vector3.zero;
        }
    }
}
