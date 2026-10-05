using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace Collection.EditorTools
{
	/// <summary>
	/// Builds the TextMesh Pro sprite assets that InputPrompts draws button prompts with, from
	/// the two glyph sheets in Assets/Textures/Input. Run it again after changing a sheet
	/// (redrawing a glyph, adding one): the assets are rebuilt in place, so nothing that
	/// points at them breaks.
	///
	/// A glyph is named by its sprite in the sheet, and that name is what InputPrompts looks
	/// up, so a new glyph also needs a line in InputPrompts' tables.
	/// </summary>
	public static class InputGlyphAssetBuilder
	{
		private const string SheetFolder = "Assets/Textures/Input/";
		private const string AssetFolder = "Assets/Resources/Input/";

		// The sheet is drawn so that a key cap, 74 pixels tall, comes out a little taller
		// than the letters of the text it sits in.
		private const int PointSize = 68;

		// Where the middle of a sheet cell sits above the text's baseline, in sheet pixels:
		// about the middle of a capital letter.
		private const int CentreAboveBaseline = 24;

		// Empty pixels kept on each side of a glyph, so neighbours in a line do not touch.
		private const int SidePadding = 4;

		[MenuItem("Collection/Build Input Glyph Assets")]
		public static void Build()
		{
			Build("Font_Input_Base.png", "InputGlyphs.asset");
			Build("Font_Input.png", "InputGlyphsShadow.asset");
			AssetDatabase.SaveAssets();
		}

		private static void Build(string sheetName, string assetName)
		{
			string sheetPath = SheetFolder + sheetName;
			Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(sheetPath);
			if (texture == null)
			{
				Debug.LogError($"InputGlyphAssetBuilder: no texture at {sheetPath}.");
				return;
			}

			var sheetSprites = new List<Sprite>();
			foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(sheetPath))
			{
				if (o is Sprite sprite) sheetSprites.Add(sprite);
			}

			// Top row first, left to right - the order they are in on the sheet.
			sheetSprites.Sort((a, b) => a.rect.y != b.rect.y ? b.rect.y.CompareTo(a.rect.y) : a.rect.x.CompareTo(b.rect.x));

			string assetPath = AssetFolder + assetName;
			TMP_SpriteAsset asset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(assetPath);
			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
				AssetDatabase.CreateAsset(asset, assetPath);
			}

			var serialized = new SerializedObject(asset);
			serialized.FindProperty("m_Version").stringValue = "1.1.0";
			serialized.ApplyModifiedPropertiesWithoutUndo();

			asset.spriteSheet = texture;
			asset.spriteInfoList = new List<TMP_Sprite>();
			asset.spriteGlyphTable.Clear();
			asset.spriteCharacterTable.Clear();

			// Its own face info is what makes a glyph the same size next to any font: without
			// it the size would follow each font asset's point size.
			FaceInfo face = asset.faceInfo;
			face.pointSize = PointSize;
			face.scale = 1f;
			face.lineHeight = PointSize;
			face.ascentLine = PointSize * 0.8f;
			face.descentLine = -PointSize * 0.2f;
			face.baseline = 0f;
			asset.faceInfo = face;

			Color32[] pixels = texture.GetPixels32();
			for (int i = 0; i < sheetSprites.Count; i++)
			{
				Sprite sprite = sheetSprites[i];
				RectInt cell = new RectInt((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);

				// Only as wide as the drawing, so a narrow glyph does not carry its cell's
				// empty sides into the line; the full cell height, so every glyph keeps the
				// middle line the sheet was drawn to.
				FindInkColumns(pixels, texture.width, cell, out int minX, out int maxX);
				int x = Mathf.Max(cell.xMin, minX - SidePadding);
				int width = Mathf.Min(cell.xMax, maxX + 1 + SidePadding) - x;

				var glyphRect = new GlyphRect(x, cell.y, width, cell.height);
				var metrics = new GlyphMetrics(width, cell.height, 0f, cell.height * 0.5f + CentreAboveBaseline, width);
				var glyph = new TMP_SpriteGlyph((uint)i, metrics, glyphRect, 1f, 0, sprite);
				asset.spriteGlyphTable.Add(glyph);
				asset.spriteCharacterTable.Add(new TMP_SpriteCharacter(0xFFFE, glyph) { name = sprite.name, scale = 1f });
			}

			Material material = asset.material;
			if (material == null)
			{
				material = new Material(Shader.Find("TextMeshPro/Sprite")) { name = "TextMeshPro/Sprite Material" };
				material.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(material, asset);
				asset.material = material;
			}

			// The project has two shaders of this name; Shader.Find gives an old copy under
			// TextMesh Pro/Resources that lacks properties the text components set.
			Shader current = AssetDatabase.LoadAssetAtPath<Shader>("Assets/TextMesh Pro/Shaders/TMP_Sprite.shader");
			if (current != null)
			{
				material.shader = current;
			}

			material.SetTexture(ShaderUtilities.ID_MainTex, texture);

			asset.UpdateLookupTables();
			EditorUtility.SetDirty(material);
			EditorUtility.SetDirty(asset);
			Debug.Log($"InputGlyphAssetBuilder: {assetPath} built with {sheetSprites.Count} glyphs.");
		}

		private static void FindInkColumns(Color32[] pixels, int textureWidth, RectInt cell, out int minX, out int maxX)
		{
			minX = cell.xMax;
			maxX = cell.xMin - 1;
			for (int y = cell.yMin; y < cell.yMax; y++)
			{
				for (int x = cell.xMin; x < cell.xMax; x++)
				{
					if (pixels[y * textureWidth + x].a <= 8) continue;
					if (x < minX) minX = x;
					if (x > maxX) maxX = x;
				}
			}

			if (maxX < minX)
			{
				minX = cell.xMin;
				maxX = cell.xMax - 1;
			}
		}
	}
}
