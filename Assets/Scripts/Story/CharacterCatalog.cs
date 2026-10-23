using System;
using System.Collections.Generic;
using UnityEngine;

namespace Collection.Story
{
    // Body parts a clothing item can colour.
    [Flags]
    public enum BodyCoverage
    {
        None = 0,
        Torso = 1 << 0,
        Pelvis = 1 << 1,
        UpperArms = 1 << 2,
        LowerArms = 1 << 3,
        Hands = 1 << 4,
        UpperLegs = 1 << 5,
        LowerLegs = 1 << 6,
        Feet = 1 << 7,
        Neck = 1 << 8,
    }

    // A catalog entry: the name is what players see; the ID is what save files store, so names can change freely.
    public interface ICatalogOption
    {
        string Id { get; set; }
        string Name { get; }
    }

    // Every option available for customizing characters: colours, eye types, masks and clothing.
    // Characters refer to options by their index in these lists; save files refer to them by ID.
    [CreateAssetMenu(menuName = "Game/Character Catalog", fileName = "CharacterCatalog")]
    public class CharacterCatalog : ScriptableObject
    {
        const string IdTooltip = "Stored in save files. Keep it stable once players have saves; the name can change freely. " +
                                 "Filled in from the name when empty.";

        [Serializable]
        public struct ColorOption : ICatalogOption
        {
            public string name;
            [Tooltip(IdTooltip)] public string id;
            public Color color;
            [Tooltip("Shown where a pattern is black. Left fully transparent, it's a darker shade of the colour.")]
            public Color shade;
            public string Id { get => id; set => id = value; }
            public string Name => name;

            public Color Shade => shade.a > 0 ? shade : new Color(color.r * ShadeDarkness, color.g * ShadeDarkness, color.b * ShadeDarkness, 1f);
        }

        // How dark the automatic second colour is, compared to the colour.
        const float ShadeDarkness = 0.55f;

        // A drawn pattern over a body part, measured in metres: white shows the part's colour, black its second colour.
        [Serializable]
        public class Pattern
        {
            [Tooltip("Optional. A seamless greyscale drawing.")]
            public Texture2D texture;
            [Tooltip("Metres the pattern takes before it repeats.")]
            public float size = 0.5f;
            [Tooltip("0 is plain colour, 1 is the full pattern.")]
            [Range(0f, 1f)] public float strength = 1f;
        }

        [Serializable]
        public struct TextureOption : ICatalogOption
        {
            public string name;
            [Tooltip(IdTooltip)] public string id;
            [Tooltip("Uses alpha clipping: pixels below 50% alpha are cut out.")]
            public Texture2D texture;
            public string Id { get => id; set => id = value; }
            public string Name => name;
        }

        [Serializable]
        public class ClothingOption : ICatalogOption
        {
            public string name;
            [Tooltip(IdTooltip)] public string id;
            public string Id { get => id; set => id = value; }
            public string Name => name;
            [Tooltip("Body parts shown in this item's colour. For bottoms, Feet means shoes (shoes colour).")]
            public BodyCoverage coverage;
            [Tooltip("Optional extra pieces: a prefab whose children are named after bones (e.g. Head, Chest). " +
                     "Their children are attached to those bones. For bottoms, pieces named \"Boot...\" use the shoes colour.")]
            public GameObject pieces;
            [Tooltip("Drawn on the item's parts and pieces (not on shoes).")]
            public Pattern pattern = new Pattern();
        }

        public List<ColorOption> skinColors = new List<ColorOption>();
        [Tooltip("Drawn on the bare skin of every character.")]
        public Pattern skinPattern = new Pattern();
        public List<ColorOption> clothColors = new List<ColorOption>();
        public List<ColorOption> hairColors = new List<ColorOption>();
        public List<TextureOption> eyes = new List<TextureOption>();
        public List<TextureOption> masks = new List<TextureOption>();

        [Header("Clothing")]
        public List<ClothingOption> tops = new List<ClothingOption>();
        [Tooltip("Pants and shoes as one set.")]
        public List<ClothingOption> bottoms = new List<ClothingOption>();
        [Tooltip("Worn over the top (jackets, vests, capes). Include a \"None\" entry.")]
        public List<ClothingOption> jackets = new List<ClothingOption>();
        [Tooltip("Include a \"None\" entry.")]
        public List<ClothingOption> hats = new List<ClothingOption>();
        [Tooltip("Include a \"Bald\" entry.")]
        public List<ClothingOption> hairStyles = new List<ClothingOption>();

        public Color Skin(int index) => Pick(skinColors, index).color;
        public Color Cloth(int index) => Pick(clothColors, index).color;
        public Color Hair(int index) => Pick(hairColors, index).color;
        public Color SkinShade(int index) => Pick(skinColors, index).Shade;
        public Color ClothShade(int index) => Pick(clothColors, index).Shade;
        public Color HairShade(int index) => Pick(hairColors, index).Shade;
        public Texture2D Eyes(int index) => Pick(eyes, index).texture;
        public Texture2D Mask(int index) => Pick(masks, index).texture;

        static T Pick<T>(List<T> list, int index)
        {
            return list.Count == 0 ? default : list[Mathf.Clamp(index, 0, list.Count - 1)];
        }

        public static ClothingOption PickClothing(List<ClothingOption> list, int index) => Pick(list, index);

        public static List<string> Ids<T>(List<T> list) where T : ICatalogOption
        {
            var ids = new List<string>(list.Count);
            foreach (var option in list)
                ids.Add(option.Id);
            return ids;
        }

        // "Trousers & Shoes" -> "trousers_shoes"
        public static string MakeId(string name)
        {
            var id = new System.Text.StringBuilder();
            foreach (char ch in (name ?? "").Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(ch))
                    id.Append(ch);
                else if (id.Length > 0 && id[id.Length - 1] != '_')
                    id.Append('_');
            }
            return id.ToString().TrimEnd('_');
        }

        void OnValidate()
        {
            FillIds(nameof(skinColors), skinColors);
            FillIds(nameof(clothColors), clothColors);
            FillIds(nameof(hairColors), hairColors);
            FillIds(nameof(eyes), eyes);
            FillIds(nameof(masks), masks);
            FillIds(nameof(tops), tops);
            FillIds(nameof(bottoms), bottoms);
            FillIds(nameof(jackets), jackets);
            FillIds(nameof(hats), hats);
            FillIds(nameof(hairStyles), hairStyles);
        }

        // Gives entries without an ID one made from their name, and warns about duplicate IDs in a list.
        void FillIds<T>(string listName, List<T> list) where T : ICatalogOption
        {
            var seen = new HashSet<string>();
            for (int i = 0; i < list.Count; i++)
            {
                T option = list[i];
                if (option == null)
                    continue;
                if (string.IsNullOrEmpty(option.Id) && !string.IsNullOrEmpty(option.Name))
                {
                    option.Id = MakeId(option.Name);
                    list[i] = option;
                }
                if (!string.IsNullOrEmpty(option.Id) && !seen.Add(option.Id))
                    Debug.LogWarning($"[CharacterCatalog] Duplicate ID '{option.Id}' in {listName}; saves can't tell these apart.", this);
            }
        }
    }
}
