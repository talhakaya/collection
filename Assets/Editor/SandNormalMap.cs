using System.IO;
using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
    // The desert sand's normal map, made from its colour picture: a gentle relief from its own light and dark,
    // read as height. (The glitter is not in it: that is SandSparkles, on the ground.)
    //
    // Collection > Story > Make Sand Normal Map writes it next to the picture and puts it on the desert's terrain
    // layer.
    public static class SandNormalMap
    {
        const string Picture = "Assets/Textures/Terrain/Desert_Sand_Seamless.png";
        const string Output = "Assets/Textures/Terrain/Desert_Sand_Normal.png";
        const string Layer = "Assets/Story/Terrain/DesertSandLayer.terrainlayer";

        // How steep the relief is.
        const float Relief = 2.5f;

        [MenuItem("Collection/Story/Make Sand Normal Map")]
        public static void Make()
        {
            var picture = new Texture2D(2, 2);
            if (!File.Exists(Picture) || !picture.LoadImage(File.ReadAllBytes(Picture)))
            {
                Debug.LogError("SandNormalMap: no picture at " + Picture);
                return;
            }

            int w = picture.width, h = picture.height;
            Color[] colours = picture.GetPixels();
            var height = new float[w * h];
            for (int i = 0; i < height.Length; i++)
                height[i] = colours[i].grayscale;

            // From the slope of the height either way; the picture repeats, so the slope at an edge takes the other
            // edge's texels.
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float left = height[y * w + (x + w - 1) % w], right = height[y * w + (x + 1) % w];
                    float down = height[((y + h - 1) % h) * w + x], up = height[((y + 1) % h) * w + x];
                    Vector3 n = new Vector3((left - right) * Relief, (down - up) * Relief, 1f).normalized;
                    pixels[y * w + x] = new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f);
                }
            }

            var texture = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(Output, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(Output);

            var importer = (TextureImporter)AssetImporter.GetAtPath(Output);
            importer.textureType = TextureImporterType.NormalMap;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.anisoLevel = 4;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();

            var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(Layer);
            if (layer != null)
            {
                layer.normalMapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Output);
                layer.normalScale = 1f;
                EditorUtility.SetDirty(layer);
                AssetDatabase.SaveAssets();
            }
            Debug.Log("SandNormalMap: written to " + Output);
        }
    }
}
