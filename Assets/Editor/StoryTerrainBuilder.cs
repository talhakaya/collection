using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Collection.EditorTools
{
    // Makes the story's land bigger: terrain tiles all round the one the level was built on, out to `Rings` tiles
    // each way, their heights from one field of Perlin noise so they run into one another, bent at the middle
    // tile's edges to meet it where it is, sunk into the sea at the outer edge, and kept under the sea in a bay
    // where the giant television stands.
    //
    // Collection > Story > Rebuild Surrounding Terrain, with the level's scene open. It replaces the tiles it made
    // before (the object "More Terrain" and the assets in Assets/Story/Terrain/Tiles); the middle tile is not
    // touched.
    public static class StoryTerrainBuilder
    {
        const string RootName = "More Terrain";
        const string Folder = "Assets/Story/Terrain/Tiles";
        const string MiddleName = "Terrain World_2_2";

        // Tiles each way from the middle one: 2 makes 5 by 5.
        const int Rings = 2;
        // The noise: hills `Broad` metres across and `BroadHeight` high, with smaller bumps on them.
        const float Broad = 46f, BroadHeight = 7.5f, Fine = 13f, FineHeight = 1.3f, Lift = 1.4f;
        const float SeedX = 137.3f, SeedZ = 71.9f;
        // Metres over which a new tile comes round to the middle tile's edge.
        const float MeetOver = 18f;
        // Metres over which the land goes down into the sea at the outer edge, and how deep.
        const float ShoreOver = 26f, SeaBed = -3f;
        // The bay: under the sea within BayRadius of the television, back to land by BayRadius + BayShore.
        const float BayRadius = 26f, BayShore = 16f;

        [MenuItem("Collection/Story/Rebuild Surrounding Terrain")]
        public static void Rebuild()
        {
            GameObject middleObject = GameObject.Find(MiddleName);
            Terrain middle = middleObject != null ? middleObject.GetComponent<Terrain>() : null;
            if (middle == null)
            {
                Debug.LogError($"StoryTerrainBuilder: no terrain called \"{MiddleName}\" in the open scene.");
                return;
            }

            TerrainData source = middle.terrainData;
            Vector3 size = source.size;
            Vector3 corner = middle.transform.position;
            int resolution = source.heightmapResolution;
            float[,] middleHeights = source.GetHeights(0, 0, resolution, resolution);

            GameObject television = GameObject.Find("Giant Television");
            Vector3 bay = television != null ? television.transform.position : new Vector3(0f, 0f, 1e6f);

            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets/Story/Terrain", "Tiles");

            GameObject root = GameObject.Find(RootName);
            if (root == null)
                root = new GameObject(RootName);
            for (int i = root.transform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(root.transform.GetChild(i).gameObject);
            root.transform.position = Vector3.zero;

            float outerMinX = corner.x - Rings * size.x, outerMaxX = corner.x + (Rings + 1) * size.x;
            float outerMinZ = corner.z - Rings * size.z, outerMaxZ = corner.z + (Rings + 1) * size.z;

            for (int tz = -Rings; tz <= Rings; tz++)
            {
                for (int tx = -Rings; tx <= Rings; tx++)
                {
                    if (tx == 0 && tz == 0)
                        continue;

                    Vector3 tileCorner = corner + new Vector3(tx * size.x, 0f, tz * size.z);
                    var heights = new float[resolution, resolution];
                    for (int z = 0; z < resolution; z++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            float wx = tileCorner.x + x * size.x / (resolution - 1);
                            float wz = tileCorner.z + z * size.z / (resolution - 1);

                            float height = Lift
                                + (Mathf.PerlinNoise(SeedX + wx / Broad, SeedZ + wz / Broad) - 0.5f) * BroadHeight
                                + (Mathf.PerlinNoise(SeedZ + wx / Fine, SeedX + wz / Fine) - 0.5f) * FineHeight;

                            // Round to the middle tile's own edge near it.
                            float nearX = Mathf.Clamp(wx, corner.x, corner.x + size.x);
                            float nearZ = Mathf.Clamp(wz, corner.z, corner.z + size.z);
                            float away = Vector2.Distance(new Vector2(wx, wz), new Vector2(nearX, nearZ));
                            if (away < MeetOver)
                            {
                                float edge = Sample(middleHeights, resolution, (nearX - corner.x) / size.x, (nearZ - corner.z) / size.z) * size.y + corner.y;
                                height = Mathf.Lerp(edge, height, Mathf.SmoothStep(0f, 1f, away / MeetOver));
                            }

                            // Into the sea at the outer edge.
                            float inFromEdge = Mathf.Min(Mathf.Min(wx - outerMinX, outerMaxX - wx), Mathf.Min(wz - outerMinZ, outerMaxZ - wz));
                            height = Mathf.Lerp(SeaBed, height, Mathf.SmoothStep(0f, 1f, inFromEdge / ShoreOver));

                            // And in the television's bay.
                            float fromBay = Vector2.Distance(new Vector2(wx, wz), new Vector2(bay.x, bay.z));
                            height = Mathf.Lerp(SeaBed, height, Mathf.SmoothStep(0f, 1f, (fromBay - BayRadius) / BayShore));

                            heights[z, x] = Mathf.Clamp01((height - corner.y) / size.y);
                        }
                    }

                    string name = $"Tile {tx + Rings} {tz + Rings}";
                    string path = $"{Folder}/{name}.asset";
                    TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
                    if (data == null)
                    {
                        data = new TerrainData();
                        AssetDatabase.CreateAsset(data, path);
                    }
                    data.heightmapResolution = resolution;
                    data.size = size;
                    data.terrainLayers = source.terrainLayers;
                    data.SetHeights(0, 0, heights);
                    EditorUtility.SetDirty(data);

                    GameObject tile = Terrain.CreateTerrainGameObject(data);
                    tile.name = name;
                    tile.layer = middleObject.layer;
                    tile.transform.SetParent(root.transform, false);
                    tile.transform.position = tileCorner;
                    Terrain terrain = tile.GetComponent<Terrain>();
                    terrain.materialTemplate = middle.materialTemplate;
                    terrain.groupingID = middle.groupingID;
                    terrain.allowAutoConnect = true;
                    terrain.shadowCastingMode = middle.shadowCastingMode;
                    terrain.heightmapPixelError = middle.heightmapPixelError;
                    terrain.basemapDistance = middle.basemapDistance;
                    terrain.drawInstanced = middle.drawInstanced;
                }
            }

            middle.allowAutoConnect = true;
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(root.scene);
            Debug.Log($"StoryTerrainBuilder: {(2 * Rings + 1) * (2 * Rings + 1) - 1} tiles round \"{MiddleName}\", {size.x * (2 * Rings + 1)} m across.");
        }

        // A height (0..1) from the middle tile's heightmap at a place given as parts of its width and depth.
        static float Sample(float[,] heights, int resolution, float u, float v)
        {
            float x = Mathf.Clamp01(u) * (resolution - 1), z = Mathf.Clamp01(v) * (resolution - 1);
            int x0 = Mathf.FloorToInt(x), z0 = Mathf.FloorToInt(z);
            int x1 = Mathf.Min(x0 + 1, resolution - 1), z1 = Mathf.Min(z0 + 1, resolution - 1);
            float fx = x - x0, fz = z - z0;
            return Mathf.Lerp(Mathf.Lerp(heights[z0, x0], heights[z0, x1], fx), Mathf.Lerp(heights[z1, x0], heights[z1, x1], fx), fz);
        }
    }
}
