using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Collection.Controls;

namespace Collection.EditorTools
{
	/// <summary>
	/// Helpers for moving a game's button prompts onto the collection's shared glyphs
	/// (InputPrompts). The glyphs are drawn inline by TextMesh Pro, so a text that holds a
	/// prompt has to be a TextMesh Pro text; these turn the old kinds into one in place,
	/// keeping the object and everything else on it.
	///
	/// Only texts with a prompt in them need this. The rest of a game's text stays as it is.
	/// </summary>
	public static class InputPromptTools
	{
		// Every character a prompt text is likely to hold. The font assets are static, so a
		// character outside this set shows as a missing glyph until it is added here and the
		// asset is deleted and made again.
		private const string Characters =
			" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"
			+ "ÇçĞğİıÖöŞşÜü‘’“”…–—";

		private const string CurrentSdfShaderPath = "Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader";

		/// <summary>
		/// The TextMesh Pro font asset for a font file, made next to it the first time
		/// ("victor-pixel.ttf" gives "victor-pixel SDF.asset"). Static, so playing the game
		/// never writes to it.
		/// </summary>
		public static TMP_FontAsset FontAssetFor(Font font)
		{
			string fontPath = AssetDatabase.GetAssetPath(font);
			if (string.IsNullOrEmpty(fontPath) || !fontPath.StartsWith("Assets/"))
			{
				// Unity's built-in font has no file to make an asset from.
				return TMP_Settings.defaultFontAsset;
			}

			string assetPath = Path.ChangeExtension(fontPath, null) + " SDF.asset";
			TMP_FontAsset asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
			if (asset != null)
			{
				return asset;
			}

			asset = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
			asset.TryAddCharacters(Characters, out string missing);
			asset.atlasPopulationMode = AtlasPopulationMode.Static;

			// The project has two shaders of this name, and the one Shader.Find gives (an old
			// copy under TextMesh Pro/Resources) is too old for this TextMesh Pro: it draws a
			// bar under every glyph and lacks properties the text components set.
			Shader current = AssetDatabase.LoadAssetAtPath<Shader>(CurrentSdfShaderPath);
			if (current != null)
			{
				asset.material.shader = current;
			}

			AssetDatabase.CreateAsset(asset, assetPath);
			asset.material.name = Path.GetFileNameWithoutExtension(assetPath) + " Material";
			AssetDatabase.AddObjectToAsset(asset.material, asset);
			foreach (Texture2D atlas in asset.atlasTextures)
			{
				atlas.name = Path.GetFileNameWithoutExtension(assetPath) + " Atlas";
				AssetDatabase.AddObjectToAsset(atlas, asset);
			}

			EditorUtility.SetDirty(asset);
			AssetDatabase.SaveAssets();
			if (!string.IsNullOrEmpty(missing))
			{
				Debug.Log($"InputPromptTools: {assetPath} has no glyph for: {missing}");
			}

			return asset;
		}

		/// <summary>
		/// Replaces a legacy UI Text with a TextMeshProUGUI on the same object, carrying over
		/// what the two have in common. Line positions can move by a pixel or two, since the
		/// two lay text out from different font measurements - look at the result.
		/// </summary>
		public static TextMeshProUGUI Convert(Text old)
		{
			GameObject go = old.gameObject;
			string text = old.text;
			Font font = old.font;
			int fontSize = old.fontSize;
			FontStyle style = old.fontStyle;
			Color color = old.color;
			TextAnchor anchor = old.alignment;
			bool richText = old.supportRichText;
			bool wrap = old.horizontalOverflow == HorizontalWrapMode.Wrap;
			bool truncate = old.verticalOverflow == VerticalWrapMode.Truncate;
			float lineSpacing = old.lineSpacing;
			bool raycastTarget = old.raycastTarget;
			bool enabled = old.enabled;

			Object.DestroyImmediate(old, true);

			TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
			tmp.font = FontAssetFor(font);
			tmp.fontSize = fontSize;
			tmp.fontStyle = ToFontStyles(style);
			tmp.color = color;
			tmp.alignment = ToAlignment(anchor);
			tmp.richText = richText;
			tmp.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
			tmp.overflowMode = truncate ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
			tmp.lineSpacing = (lineSpacing - 1f) * 100f;
			tmp.raycastTarget = raycastTarget;
			tmp.enabled = enabled;
			tmp.text = text;
			EditorUtility.SetDirty(go);
			return tmp;
		}

		/// <summary>
		/// Replaces a legacy 3D TextMesh with a TextMeshPro on the same object. A TextMesh's
		/// size is its font size times its character size, in tenths of a unit; that is the
		/// TextMeshPro font size it gets.
		/// </summary>
		public static TextMeshPro Convert(TextMesh old)
		{
			GameObject go = old.gameObject;
			string text = old.text;
			Font font = old.font;
			float size = (old.fontSize > 0 ? old.fontSize : 13) * old.characterSize;
			FontStyle style = old.fontStyle;
			Color color = old.color;
			TextAnchor anchor = old.anchor;
			TextAlignment alignment = old.alignment;
			float lineSpacing = old.lineSpacing;
			bool richText = old.richText;

			MeshRenderer renderer = go.GetComponent<MeshRenderer>();
			int sortingLayer = renderer != null ? renderer.sortingLayerID : 0;
			int sortingOrder = renderer != null ? renderer.sortingOrder : 0;

			Object.DestroyImmediate(old, true);

			TextMeshPro tmp = go.AddComponent<TextMeshPro>();
			tmp.font = FontAssetFor(font);
			tmp.fontSize = size;
			tmp.fontStyle = ToFontStyles(style);
			tmp.color = color;
			tmp.richText = richText;
			tmp.textWrappingMode = TextWrappingModes.NoWrap;
			tmp.overflowMode = TextOverflowModes.Overflow;
			tmp.lineSpacing = (lineSpacing - 1f) * 100f;

			// A TextMesh grows from its anchor; a rect of no size with its pivot there and the
			// same alignment does the same.
			tmp.rectTransform.sizeDelta = Vector2.zero;
			tmp.alignment = ToAlignment(anchor, alignment);
			tmp.sortingLayerID = sortingLayer;
			tmp.sortingOrder = sortingOrder;
			tmp.text = text;
			EditorUtility.SetDirty(go);
			return tmp;
		}

		/// <summary>
		/// For a sprite that is nothing but a picture of a button: empties the sprite and puts
		/// the prompt in its place, as a TextMesh Pro child with an InputPromptText. The
		/// object and its renderer stay, so whatever moves, tints or shows and hides the
		/// sprite does the same to the prompt, which takes the renderer's colour.
		///
		/// keyHeight is how tall a key cap comes out, in world units.
		/// </summary>
		public static InputPromptText ReplaceSpriteWithPrompt(SpriteRenderer sprite, string template, float keyHeight, bool shadowed)
		{
			// A key cap is 74 of the sheet's pixels, drawn at font size / 68 per pixel, and a
			// 3D text's units are a tenth of its font size.
			const float KeyHeightPerFontSize = 74f / 68f * 0.1f;

			sprite.sprite = null;

			var go = new GameObject("prompt");
			go.layer = sprite.gameObject.layer;
			go.transform.SetParent(sprite.transform, false);
			Vector3 scale = sprite.transform.lossyScale;
			go.transform.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f);

			TextMeshPro tmp = go.AddComponent<TextMeshPro>();
			tmp.rectTransform.sizeDelta = Vector2.zero;
			tmp.alignment = TextAlignmentOptions.Center;
			tmp.textWrappingMode = TextWrappingModes.NoWrap;
			tmp.overflowMode = TextOverflowModes.Overflow;
			tmp.fontSize = keyHeight / KeyHeightPerFontSize;
			tmp.color = sprite.color;
			tmp.sortingLayerID = sprite.sortingLayerID;
			tmp.sortingOrder = sprite.sortingOrder;
			tmp.text = template;

			InputPromptText prompt = go.AddComponent<InputPromptText>();
			prompt.template = template;
			prompt.shadowed = shadowed;
			prompt.colourFrom = sprite;
			EditorUtility.SetDirty(sprite);
			return prompt;
		}

		private static FontStyles ToFontStyles(FontStyle style)
		{
			switch (style)
			{
				case FontStyle.Bold: return FontStyles.Bold;
				case FontStyle.Italic: return FontStyles.Italic;
				case FontStyle.BoldAndItalic: return FontStyles.Bold | FontStyles.Italic;
				default: return FontStyles.Normal;
			}
		}

		private static TextAlignmentOptions ToAlignment(TextAnchor anchor)
		{
			switch (anchor)
			{
				case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
				case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
				case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
				case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
				case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
				default: return TextAlignmentOptions.BottomRight;
			}
		}

		/// For a TextMesh: where the block sits comes from the anchor, how its lines are
		/// justified from the alignment.
		private static TextAlignmentOptions ToAlignment(TextAnchor anchor, TextAlignment alignment)
		{
			VerticalAlignmentOptions vertical;
			switch (anchor)
			{
				case TextAnchor.UpperLeft:
				case TextAnchor.UpperCenter:
				case TextAnchor.UpperRight: vertical = VerticalAlignmentOptions.Top; break;
				case TextAnchor.MiddleLeft:
				case TextAnchor.MiddleCenter:
				case TextAnchor.MiddleRight: vertical = VerticalAlignmentOptions.Middle; break;
				default: vertical = VerticalAlignmentOptions.Bottom; break;
			}

			HorizontalAlignmentOptions horizontal;
			switch (alignment)
			{
				case TextAlignment.Center: horizontal = HorizontalAlignmentOptions.Center; break;
				case TextAlignment.Right: horizontal = HorizontalAlignmentOptions.Right; break;
				default: horizontal = HorizontalAlignmentOptions.Left; break;
			}

			return (TextAlignmentOptions)((int)horizontal | (int)vertical);
		}
	}
}
