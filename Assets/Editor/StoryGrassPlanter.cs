using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Collection.EditorTools
{
    // Covers the grassy field in grass: the prefabs in Assets/Story/Terrain/Grass, planted on every terrain in the
    // open scene as terrain details drawn with GPU instancing.
    //
    // Details and not "trees": as trees every plant was a draw call of its own (the SRP Batcher takes them, and
    // then they are not instanced) - some 2800 in view at once. As instanced details the same grass is a few
    // dozen.
    //
    // Collection > Story > Plant Field Grass, with the field's scene open. It replaces whatever details and trees
    // the terrains had, so it can be run again after changing the numbers here or the land. Clear Field Grass
    // takes it all off.
    //
    // Short grass goes everywhere; tall grass and flowers grow in clumps, each where its own field of Perlin noise
    // is high. Nothing is planted under the water or right at its edge, on steep ground, or where the things under
    // `KeepClear` stand.
    public static class StoryGrassPlanter
    {
        const string Folder = "Assets/Story/Terrain/Grass/";
        const string WaterName = "Ponds";
        static readonly string[] KeepClear = { "Rocks", "Ramps", "Barrels", "Artifacts" };

        // What is planted. `PerSquareMetre` is how many plants; `Clumps` is how much of the noise it grows in, 1
        // for everywhere (0.45 is about a third of the land, as the noise is mostly near its middle); `ClumpSize`
        // is the metres across a clump. The heights and widths are times the prefab's own size. The short grass's
        // patch is 2.5 m wide, so at two to the square metre the patches run well into one another.
        struct Kind
        {
            public string Prefab;
            public float PerSquareMetre, Clumps, ClumpSize, MinHeight, MaxHeight, MinWidth, MaxWidth;
        }

        static readonly Kind[] Kinds =
        {
            new Kind { Prefab = "FieldGrass.prefab", PerSquareMetre = 2f, Clumps = 1f, ClumpSize = 1f, MinHeight = 0.55f, MaxHeight = 0.95f, MinWidth = 0.9f, MaxWidth = 1.25f },
            new Kind { Prefab = "FieldGrassTall.prefab", PerSquareMetre = 0.8f, Clumps = 0.46f, ClumpSize = 17f, MinHeight = 0.7f, MaxHeight = 1.1f, MinWidth = 0.9f, MaxWidth = 1.2f },
            new Kind { Prefab = "FieldFlowers.prefab", PerSquareMetre = 0.6f, Clumps = 0.4f, ClumpSize = 11f, MinHeight = 0.9f, MaxHeight = 1.3f, MinWidth = 0.9f, MaxWidth = 1.3f },
        };

        // How far above the water the grass starts (m), the steepest ground it grows on (degrees), and how far from
        // the things under `KeepClear` it stays (m).
        const float AboveWater = 0.12f, Steepest = 38f, Clearance = 0.25f;
        // How far away the grass is drawn (m). The camera looks down from about 20 m, so this is well out of sight.
        const float DrawnTo = 60f;
        // The squares the plants are counted in: this many along a terrain's edge (half a metre each on a 64 m
        // terrain), drawn in patches of `PerPatch` squares a side.
        const int Squares = 128, PerPatch = 16;
        const int Seed = 7919;

        [MenuItem("Collection/Story/Plant Field Grass")]
        public static void Plant()
        {
            Plant(null);
        }

        // As above, but only where `grows` says so, for a place's x and z (StoryFieldBuilder: the map's grass).
        public static void Plant(System.Func<float, float, bool> grows)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length == 0)
            {
                Debug.LogError("StoryGrassPlanter: no terrain in the open scene.");
                return;
            }

            var prototypes = new DetailPrototype[Kinds.Length];
            for (int k = 0; k < Kinds.Length; k++)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + Kinds[k].Prefab);
                if (prefab == null)
                {
                    Debug.LogError($"StoryGrassPlanter: no prefab at {Folder + Kinds[k].Prefab}.");
                    return;
                }
                prototypes[k] = new DetailPrototype
                {
                    prototype = prefab,
                    usePrototypeMesh = true,
                    renderMode = DetailRenderMode.VertexLit,
                    useInstancing = true,
                    useDensityScaling = false,
                    minHeight = Kinds[k].MinHeight,
                    maxHeight = Kinds[k].MaxHeight,
                    minWidth = Kinds[k].MinWidth,
                    maxWidth = Kinds[k].MaxWidth,
                    noiseSpread = 0.3f,
                    noiseSeed = Seed + k,
                    healthyColor = Color.white,
                    dryColor = Color.white,
                };
            }

            GameObject waterObject = GameObject.Find(WaterName);
            float dryAbove = waterObject != null ? waterObject.transform.position.y + AboveWater : float.NegativeInfinity;

            var clear = new List<Bounds>();
            foreach (string name in KeepClear)
            {
                GameObject parent = GameObject.Find(name);
                if (parent == null)
                    continue;
                foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>())
                {
                    Bounds bounds = renderer.bounds;
                    bounds.Expand(Clearance * 2f);
                    clear.Add(bounds);
                }
            }

            int planted = 0;
            foreach (Terrain terrain in terrains)
            {
                TerrainData data = terrain.terrainData;
                Vector3 corner = terrain.transform.position;
                Vector3 size = data.size;
                // Its own numbers for each tile, from where it is, so a tile's grass does not depend on the order
                // the tiles come in.
                var random = new System.Random(Seed + Mathf.RoundToInt(corner.x) * 73 + Mathf.RoundToInt(corner.z) * 179);

                Undo.RegisterCompleteObjectUndo(data, "Plant Field Grass");
                data.SetTreeInstances(new TreeInstance[0], false);
                data.treePrototypes = new TreePrototype[0];
                data.SetDetailResolution(Squares, PerPatch);
                data.SetDetailScatterMode(DetailScatterMode.InstanceCountMode);
                data.detailPrototypes = prototypes;

                float squareArea = size.x / Squares * (size.z / Squares);
                for (int k = 0; k < Kinds.Length; k++)
                {
                    Kind kind = Kinds[k];
                    float perSquare = kind.PerSquareMetre * squareArea;
                    var counts = new int[Squares, Squares];
                    for (int iz = 0; iz < Squares; iz++)
                    {
                        for (int ix = 0; ix < Squares; ix++)
                        {
                            // A whole number of plants that comes to `perSquare` on average.
                            int count = Mathf.FloorToInt(perSquare + (float)random.NextDouble());
                            if (count == 0)
                                continue;

                            float u = (ix + 0.5f) / Squares, v = (iz + 0.5f) / Squares;
                            float wx = corner.x + u * size.x, wz = corner.z + v * size.z;
                            if (kind.Clumps < 1f)
                            {
                                float noise = Mathf.PerlinNoise(31.7f * (k + 1) + wx / kind.ClumpSize, 57.3f * (k + 1) + wz / kind.ClumpSize);
                                if (noise < 1f - kind.Clumps)
                                    continue;
                            }

                            float wy = corner.y + data.GetInterpolatedHeight(u, v);
                            if (wy < dryAbove || data.GetSteepness(u, v) > Steepest)
                                continue;
                            if (InAny(clear, wx, wz) || (grows != null && !grows(wx, wz)))
                                continue;

                            counts[iz, ix] = count;
                            planted += count;
                        }
                    }
                    data.SetDetailLayer(0, 0, k, counts);
                }

                data.RefreshPrototypes();
                EditorUtility.SetDirty(data);

                terrain.detailObjectDistance = DrawnTo;
                terrain.detailObjectDensity = 1f;
                terrain.Flush();
                EditorUtility.SetDirty(terrain);
            }

            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(terrains[0].gameObject.scene);
            Debug.Log($"StoryGrassPlanter: {planted} plants on {terrains.Length} terrains.");
        }

        [MenuItem("Collection/Story/Clear Field Grass")]
        public static void Clear()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            foreach (Terrain terrain in terrains)
            {
                TerrainData data = terrain.terrainData;
                Undo.RegisterCompleteObjectUndo(data, "Clear Field Grass");
                data.SetTreeInstances(new TreeInstance[0], false);
                data.treePrototypes = new TreePrototype[0];
                data.detailPrototypes = new DetailPrototype[0];
                EditorUtility.SetDirty(data);
                terrain.Flush();
            }

            AssetDatabase.SaveAssets();
            if (terrains.Length > 0)
                EditorSceneManager.MarkSceneDirty(terrains[0].gameObject.scene);
            Debug.Log($"StoryGrassPlanter: grass taken off {terrains.Length} terrains.");
        }

        // Whether a place is over or under any of the boxes, looked at from above.
        static bool InAny(List<Bounds> boxes, float x, float z)
        {
            foreach (Bounds box in boxes)
            {
                if (x > box.min.x && x < box.max.x && z > box.min.z && z < box.max.z)
                    return true;
            }
            return false;
        }
    }
}
