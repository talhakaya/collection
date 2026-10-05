using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Collection.Story
{
    // Shows CharacterAppearance's catalog indices as named dropdowns: colours with a swatch, eyes and masks with a
    // texture preview.
    [CustomEditor(typeof(CharacterAppearance)), CanEditMultipleObjects]
    class CharacterAppearanceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var catalogProp = serializedObject.FindProperty("catalog");
            EditorGUILayout.PropertyField(catalogProp);
            var catalog = catalogProp.objectReferenceValue as CharacterCatalog;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Body", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fatness"));

            if (catalog == null)
            {
                EditorGUILayout.HelpBox("Assign a Character Catalog to choose colours, clothing, eyes and masks.", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            ColorPopup("Skin", serializedObject.FindProperty("skinColor"), catalog.skinColors);
            var televisionHead = serializedObject.FindProperty("televisionHead");
            EditorGUILayout.PropertyField(televisionHead);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clothing", EditorStyles.boldLabel);
            ClothingPopup("Top", "topType", catalog.tops);
            ColorPopup("   Colour", serializedObject.FindProperty("shirtColor"), catalog.clothColors);
            ClothingPopup("Bottom", "bottomType", catalog.bottoms);
            ColorPopup("   Pants Colour", serializedObject.FindProperty("pantsColor"), catalog.clothColors);
            ColorPopup("   Shoes Colour", serializedObject.FindProperty("shoesColor"), catalog.clothColors);
            ClothingPopup("Jacket", "jacketType", catalog.jackets);
            ColorPopup("   Colour", serializedObject.FindProperty("jacketColor"), catalog.clothColors);
            if (televisionHead.boolValue && !televisionHead.hasMultipleDifferentValues)
            {
                // No head of its own: nothing to choose for a hat, hair or face.
                serializedObject.ApplyModifiedProperties();
                return;
            }

            ClothingPopup("Hat", "hatType", catalog.hats);
            ColorPopup("   Colour", serializedObject.FindProperty("hatColor"), catalog.clothColors);
            ClothingPopup("Hair", "hairType", catalog.hairStyles);
            ColorPopup("   Colour", serializedObject.FindProperty("hairColor"), catalog.hairColors);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Face", EditorStyles.boldLabel);
            TexturePopup("Eyes", serializedObject.FindProperty("eyeType"), catalog.eyes);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eyeGap"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("faceHeight"));
            var showMask = serializedObject.FindProperty("showMask");
            EditorGUILayout.PropertyField(showMask);
            using (new EditorGUI.DisabledScope(!showMask.boolValue && !showMask.hasMultipleDifferentValues))
                TexturePopup("Mask", serializedObject.FindProperty("maskType"), catalog.masks);

            serializedObject.ApplyModifiedProperties();
        }

        void ClothingPopup(string label, string property, List<CharacterCatalog.ClothingOption> options)
        {
            Popup(label, serializedObject.FindProperty(property), options.Select(o => o.name).ToList(), rect => { });
        }

        static void ColorPopup(string label, SerializedProperty index, List<CharacterCatalog.ColorOption> options)
        {
            Popup(label, index, options.Select(o => o.name).ToList(), rect =>
                EditorGUI.DrawRect(rect, options[Mathf.Clamp(index.intValue, 0, options.Count - 1)].color));
        }

        static void TexturePopup(string label, SerializedProperty index, List<CharacterCatalog.TextureOption> options)
        {
            Popup(label, index, options.Select(o => o.name).ToList(), rect =>
            {
                var texture = options[Mathf.Clamp(index.intValue, 0, options.Count - 1)].texture;
                if (texture != null)
                    GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            });
        }

        static void Popup(string label, SerializedProperty index, List<string> names, System.Action<Rect> preview)
        {
            if (names.Count == 0)
            {
                EditorGUILayout.LabelField(label, "(catalog list is empty)");
                return;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                var contents = names.Select((n, i) => new GUIContent(string.IsNullOrEmpty(n) ? "Option " + i : n)).ToArray();
                EditorGUI.showMixedValue = index.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                int value = EditorGUILayout.Popup(new GUIContent(label), Mathf.Clamp(index.intValue, 0, names.Count - 1), contents);
                if (EditorGUI.EndChangeCheck())
                    index.intValue = value;
                EditorGUI.showMixedValue = false;
                preview(GUILayoutUtility.GetRect(36, EditorGUIUtility.singleLineHeight, GUILayout.Width(36)));
            }
        }
    }
}
