using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace Collection.EditorTools
{
    // Makes a TextMesh Pro font from handwriting: a picture of the characters, each in its own square of a grid,
    // light ink on transparent - written over a template of the same grid with guide letters and lines in it
    // (Handwriting Template.png, next to the source), so that each character sits on its square's baseline.
    //
    // Collection > Fonts > Build Handwritten Font reads Assets/Fonts/Handwriting/Font_Raw.png and writes
    // Assets/Resources/Fonts/Handwriting SDF.asset: a distance field font, so the letters stay sharp at any size
    // and the outline, shadow and glow of TextMesh Pro's usual material work on them. Each character is as wide
    // as its ink, with `SideGap` either side; it sits on the template's baseline wherever it was written in its
    // square. What was not written falls back on the usual font.
    public static class HandwrittenFontBuilder
    {
        const string Folder = "Assets/Fonts/Handwriting/";
        const string Source = Folder + "Font_Raw.png";
        // In Resources, for the menus to load (CollectionFonts).
        const string Output = "Assets/Resources/Fonts/Handwriting SDF.asset";

        // The template: its characters in order, ten to a row of squares this big, and its guide lines' heights
        // from the bottom of a square.
        public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        public const int Columns = 10, Cell = 256;
        public const int Descender = 36, Baseline = 80, XHeight = 160, CapHeight = 220;

        // The atlas, how far out from the ink the distance field goes (pixels), the space either side of a
        // character, and the width of a space - both as parts of the capital height.
        const int AtlasSize = 2048;
        const int Spread = 12;
        const float SideGap = 0.07f;
        const float SpaceWidth = 0.42f;
        // Ink lighter than this is not ink.
        const byte InkFrom = 100;

        [MenuItem("Collection/Fonts/Build Handwritten Font")]
        public static void Build()
        {
            var picture = new Texture2D(2, 2);
            if (!File.Exists(Source) || !picture.LoadImage(File.ReadAllBytes(Source)))
            {
                Debug.LogError("HandwrittenFontBuilder: no picture at " + Source);
                return;
            }

            int width = picture.width, height = picture.height;
            Color32[] pixels = picture.GetPixels32();
            float cap = CapHeight - Baseline;
            int sideGap = Mathf.RoundToInt(cap * SideGap);

            var atlas = new byte[AtlasSize * AtlasSize];
            var glyphs = new List<Glyph>();
            var characters = new List<TMP_Character>();
            // Shelves: across the atlas, then up a row as high as the tallest on it.
            int shelfX = 0, shelfY = 0, shelfHeight = 0;

            for (int i = 0; i < Characters.Length; i++)
            {
                int cellX = (i % Columns) * Cell;
                // Rows from the top of the picture; texture rows count from the bottom.
                int cellY = height - (i / Columns + 1) * Cell;
                if (!InkBounds(pixels, width, cellX, cellY, out RectInt ink))
                {
                    Debug.LogWarning("HandwrittenFontBuilder: nothing written for '" + Characters[i] + "'.");
                    continue;
                }

                int boxW = ink.width + Spread * 2, boxH = ink.height + Spread * 2;
                if (shelfX + boxW > AtlasSize)
                {
                    shelfX = 0;
                    shelfY += shelfHeight;
                    shelfHeight = 0;
                }
                if (shelfY + boxH > AtlasSize)
                {
                    Debug.LogError("HandwrittenFontBuilder: the characters do not fit in the atlas.");
                    return;
                }

                DistanceField(pixels, width, height, ink, atlas, shelfX, shelfY);

                uint index = (uint)glyphs.Count + 1;
                var metrics = new GlyphMetrics(ink.width, ink.height, sideGap, ink.yMax - (cellY + Baseline), ink.width + sideGap * 2);
                var rect = new GlyphRect(shelfX + Spread, shelfY + Spread, ink.width, ink.height);
                glyphs.Add(new Glyph(index, metrics, rect, 1f, 0));
                characters.Add(new TMP_Character(Characters[i], glyphs[glyphs.Count - 1]));

                shelfX += boxW;
                shelfHeight = Mathf.Max(shelfHeight, boxH);
            }

            // The space: nothing to draw, only its width.
            var space = new Glyph((uint)glyphs.Count + 1, new GlyphMetrics(0, 0, 0, 0, cap * SpaceWidth), new GlyphRect(0, 0, 0, 0), 1f, 0);
            glyphs.Add(space);
            characters.Add(new TMP_Character(' ', space));

            Texture2D texture = AtlasTexture(atlas);
            Write(texture, glyphs, characters, cap);
        }

        // The rectangle around the ink in one square of the grid.
        static bool InkBounds(Color32[] pixels, int width, int cellX, int cellY, out RectInt ink)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = -1, maxY = -1;
            for (int y = cellY; y < cellY + Cell; y++)
            {
                for (int x = cellX; x < cellX + Cell; x++)
                {
                    if (pixels[y * width + x].a < InkFrom)
                        continue;
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
            ink = maxX < 0 ? default : new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
            return maxX >= 0;
        }

        // The ink's distance field, into the atlas with its bottom left at (atX, atY): a half where the edge of
        // the ink is, more inside, less outside, reaching nothing `Spread` pixels out.
        static void DistanceField(Color32[] pixels, int width, int height, RectInt ink, byte[] atlas, int atX, int atY)
        {
            int w = ink.width + Spread * 2, h = ink.height + Spread * 2;
            var inside = new bool[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int sx = ink.x - Spread + x, sy = ink.y - Spread + y;
                    inside[y * w + x] = sx >= 0 && sy >= 0 && sx < width && sy < height && pixels[sy * width + sx].a >= InkFrom;
                }
            }

            float[] toInk = SquaredDistances(inside, w, h, true);
            float[] toOutside = SquaredDistances(inside, w, h, false);
            float scale = Spread + 1;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int at = y * w + x;
                    float signed = inside[at] ? -(Mathf.Sqrt(toOutside[at]) - 0.5f) : Mathf.Sqrt(toInk[at]) - 0.5f;
                    float value = Mathf.Clamp01(0.5f - signed / (2f * scale));
                    atlas[(atY + y) * AtlasSize + atX + x] = (byte)Mathf.RoundToInt(value * 255f);
                }
            }
        }

        // For every pixel, the squared distance to the nearest pixel that is (`toInside`) or is not inside: the
        // exact Euclidean distance transform of Felzenszwalb and Huttenlocher, a column pass then a row pass.
        static float[] SquaredDistances(bool[] inside, int w, int h, bool toInside)
        {
            const float Far = 1e20f;
            var grid = new float[w * h];
            for (int i = 0; i < grid.Length; i++)
                grid[i] = inside[i] == toInside ? 0f : Far;

            var line = new float[Mathf.Max(w, h)];
            var result = new float[Mathf.Max(w, h)];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++) line[y] = grid[y * w + x];
                Transform1D(line, h, result);
                for (int y = 0; y < h; y++) grid[y * w + x] = result[y];
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++) line[x] = grid[y * w + x];
                Transform1D(line, w, result);
                for (int x = 0; x < w; x++) grid[y * w + x] = result[x];
            }
            return grid;
        }

        static void Transform1D(float[] f, int n, float[] d)
        {
            var v = new int[n];
            var z = new float[n + 1];
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;
            for (int q = 1; q < n; q++)
            {
                float s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2f * q - 2f * v[k]);
                while (s <= z[k])
                {
                    k--;
                    s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2f * q - 2f * v[k]);
                }
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }
            k = 0;
            for (int q = 0; q < n; q++)
            {
                while (z[k + 1] < q) k++;
                float apart = q - v[k];
                d[q] = apart * apart + f[v[k]];
            }
        }

        static Texture2D AtlasTexture(byte[] atlas)
        {
            var texture = new Texture2D(AtlasSize, AtlasSize, TextureFormat.Alpha8, false) { name = "Handwriting Atlas" };
            texture.LoadRawTextureData(atlas);
            texture.Apply(false, false);
            return texture;
        }

        // The font asset, its atlas and its material, all in one file, as TextMesh Pro's own Font Asset Creator
        // makes them. Its tables are set through its serialized fields, which have no public setters.
        static void Write(Texture2D texture, List<Glyph> glyphs, List<TMP_Character> characters, float cap)
        {
            // An em a little more than the capital height and the descender below it.
            float em = cap * 1.42f;
            var face = new FaceInfo
            {
                familyName = "Handwriting",
                styleName = "Regular",
                pointSize = Mathf.RoundToInt(em),
                scale = 1f,
                lineHeight = em * 1.2f,
                ascentLine = cap * 1.15f,
                capLine = cap,
                meanLine = XHeight - Baseline,
                baseline = 0f,
                descentLine = -(Baseline - Descender),
                underlineOffset = -(Baseline - Descender) * 0.5f,
                underlineThickness = cap * 0.06f,
                strikethroughOffset = (XHeight - Baseline) * 0.5f,
                strikethroughThickness = cap * 0.06f,
                superscriptOffset = cap * 0.5f,
                superscriptSize = 0.5f,
                subscriptOffset = -cap * 0.15f,
                subscriptSize = 0.5f,
                tabWidth = cap * SpaceWidth * 4f,
            };

            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Output);
            bool made = font == null;
            if (made)
                font = ScriptableObject.CreateInstance<TMP_FontAsset>();
            font.name = Path.GetFileNameWithoutExtension(Output);
            font.faceInfo = face;
            font.atlasPopulationMode = AtlasPopulationMode.Static;
            // What was not written (accented letters, curly quotes) in the usual font, not as boxes.
            if (TMP_Settings.defaultFontAsset != null)
                font.fallbackFontAssetTable = new List<TMP_FontAsset> { TMP_Settings.defaultFontAsset };

            // The old atlas and material, if this is a rebuild, are replaced.
            if (!made)
            {
                foreach (Object part in AssetDatabase.LoadAllAssetsAtPath(Output))
                    if (part is Texture2D || part is Material)
                        Object.DestroyImmediate(part, true);
            }

            // From the default font's, which has every value the shader needs set, as a new one does not.
            TMP_FontAsset usual = TMP_Settings.defaultFontAsset;
            var material = usual != null ? new Material(usual.material) : new Material(Shader.Find("TextMeshPro/Distance Field"));
            material.name = font.name + " Material";
            material.SetTexture("_MainTex", texture);
            material.SetFloat("_TextureWidth", AtlasSize);
            material.SetFloat("_TextureHeight", AtlasSize);
            material.SetFloat("_GradientScale", Spread + 1);
            material.SetFloat("_WeightNormal", 0f);
            material.SetFloat("_WeightBold", 0.75f);
            font.material = material;
            font.atlasTextures = new[] { texture };

            if (made)
                AssetDatabase.CreateAsset(font, Output);
            AssetDatabase.AddObjectToAsset(texture, font);
            AssetDatabase.AddObjectToAsset(material, font);

            var serialized = new SerializedObject(font);
            serialized.FindProperty("m_AtlasWidth").intValue = AtlasSize;
            serialized.FindProperty("m_AtlasHeight").intValue = AtlasSize;
            serialized.FindProperty("m_AtlasPadding").intValue = Spread;
            serialized.FindProperty("m_AtlasRenderMode").intValue = (int)GlyphRenderMode.SDFAA;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            typeof(TMP_FontAsset).GetField("m_AtlasTexture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(font, texture);

            typeof(TMP_FontAsset).GetField("m_GlyphTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(font, glyphs);
            typeof(TMP_FontAsset).GetField("m_CharacterTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(font, characters);
            font.ReadFontAssetDefinition();

            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(Output);
            Debug.Log($"HandwrittenFontBuilder: {characters.Count} characters into {Output}.");
        }
    }
}
