using System.Collections.Generic;
using System.IO;
using Collection.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Collection.EditorTools
{
    // Builds the grassy field from a drawn map: Assets/Story/Terrain/Field Map.jpg, looked at from above with the
    // top of the picture away from the camera, stretched over all the field's terrain tiles.
    //
    //   dark green   grass, planted (StoryGrassPlanter)
    //   light green  the grass's ground with nothing on it
    //   white        path
    //   blue         water: the land goes down under the ponds' level, which is the same everywhere
    //   black        mountain: rises steeply, far above the character, too steep to walk up
    //   pink         a house: walls, a roof and a floor, gone into like a cave (CaveZone)
    //   dark red     a house's door, as a gap in the wall nearest it
    //   yellow       where a game's cube stands (the children of "Artifacts", in the order they come)
    //
    // Collection > Story > Build Field From Map, with the field's scene open. It redoes everything each time, so
    // the map can be drawn again and the field rebuilt from it: the heights, the paint and the grass of every
    // tile, the houses (the object "Village"), and where the cubes are. The other things standing on the land
    // (rocks, ramps, barrels, the bicycle, the villager and the character) are kept where they are across the
    // land and moved up or down with it; any that end up in the water, a mountain or a house are listed in the
    // console.
    public static class StoryFieldBuilder
    {
        const string MapPath = "Assets/Story/Terrain/Field Map.jpg";
        const string TerrainFolder = "Assets/Story/Terrain/Grass/";
        const string MaterialFolder = "Assets/Story/Materials/";
        const string VillageName = "Village";
        const string WaterName = "Ponds";
        static readonly string[] Movable = { "Rocks", "Ramps", "Barrels", "Artifacts" };
        static readonly string[] MovableAlone = { "Villager", "Bicycle", "Player" };

        enum Kind { Grass, Bare, Path, Water, Mountain, House, Door, Cube }

        // The colours drawn with; every pixel is taken to be the nearest of them.
        static readonly (Kind kind, Color32 colour)[] Palette =
        {
            (Kind.Grass, new Color32(59, 64, 34, 255)),
            (Kind.Bare, new Color32(180, 199, 144, 255)),
            (Kind.Path, new Color32(255, 255, 255, 255)),
            (Kind.Water, new Color32(132, 172, 180, 255)),
            (Kind.Mountain, new Color32(0, 0, 0, 255)),
            (Kind.House, new Color32(196, 136, 146, 255)),
            (Kind.Door, new Color32(89, 51, 48, 255)),
            (Kind.Cube, new Color32(243, 229, 128, 255)),
        };

        // Heights, all in metres. The land stands `LandAbove` over the water, with bumps of up to `LandBumps` that
        // are `BumpSize` across, and comes down to just under the water over `ShoreOver`; under the water it goes
        // on down to `WaterDepth` over `DeepOver`.
        const float LandAbove = 2.2f, LandBumps = 0.6f, BumpSize = 30f, ShoreOver = 6f, ShoreUnder = 0.4f;
        const float WaterDepth = 2.5f, DeepOver = 5f;
        // A mountain starts as a cliff, `CliffHeight` high over `CliffOver` (far too steep to walk up, and taller than
        // the character), then goes on up at `Rise` metres a metre, to `MountainHeight` at most, with
        // `MountainBumps` more on top, `MountainBumpSize` across. Behind the cliff it rises less steeply than the
        // camera looks down (45 degrees), or a mountain behind the character would come between it and the camera.
        const float CliffHeight = 3.5f, CliffOver = 1.5f, Rise = 0.75f, MountainHeight = 24f, MountainBumps = 1.5f, MountainBumpSize = 20f;
        // The terrain tiles' own height range, which has to take the mountains.
        const float TerrainBottom = -10f, TerrainHeight = 40f;
        // Grass is planted only this far inside the dark green, so its patches stay off the paths.
        const float GrassInset = 0.8f;
        // The edges of the path and the mountain paint are soft over about this much.
        const float PathEdge = 0.6f, MountainEdge = 1f;

        // Houses: wall height and thickness, the door's width and height, and how far the ground around one is
        // levelled.
        const float WallHeight = 4f, Wall = 0.4f, DoorWidth = 2.6f, DoorHeight = 3f, Levelled = 1.5f, LevelledBlend = 3f;
        // The smallest drawn house or cube, in pixels, so specks left at the edges of other colours are not taken
        // for one.
        const int SmallestHouse = 60, SmallestCube = 15;

        // The map, as kinds, and distances in metres to the edges of the kinds that need them: positive inside,
        // negative outside.
        static int mapWidth, mapHeight;
        static Kind[] map;
        static float[] land, mountain, path, grass;
        static Rect area;
        static float metresPerPixel, water;

        struct House
        {
            public Rect rect;
            public Vector2 door;
        }

        [MenuItem("Collection/Story/Build Field From Map")]
        public static void Build()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length == 0)
            {
                Debug.LogError("StoryFieldBuilder: no terrain in the open scene.");
                return;
            }
            if (!File.Exists(MapPath))
            {
                Debug.LogError($"StoryFieldBuilder: no map at {MapPath}.");
                return;
            }
            GameObject waterObject = GameObject.Find(WaterName);
            if (waterObject == null)
            {
                Debug.LogError($"StoryFieldBuilder: no \"{WaterName}\" in the open scene.");
                return;
            }
            water = waterObject.transform.position.y;

            float minX = float.MaxValue, minZ = float.MaxValue, maxX = float.MinValue, maxZ = float.MinValue;
            foreach (Terrain terrain in terrains)
            {
                Vector3 corner = terrain.transform.position, size = terrain.terrainData.size;
                minX = Mathf.Min(minX, corner.x);
                minZ = Mathf.Min(minZ, corner.z);
                maxX = Mathf.Max(maxX, corner.x + size.x);
                maxZ = Mathf.Max(maxZ, corner.z + size.z);
            }
            area = Rect.MinMaxRect(minX, minZ, maxX, maxZ);

            ReadMap();
            List<House> houses = FindHouses();
            List<Vector2> cubes = FindCubes();

            // Where everything stands above the land now, to keep it there on the new land.
            var standing = new List<(Transform thing, float above)>();
            foreach (string name in Movable)
            {
                GameObject parent = GameObject.Find(name);
                if (parent != null)
                    foreach (Transform child in parent.transform)
                        standing.Add((child, child.position.y - GroundAt(terrains, child.position)));
            }
            foreach (string name in MovableAlone)
            {
                GameObject thing = GameObject.Find(name);
                if (thing != null)
                    standing.Add((thing.transform, thing.transform.position.y - GroundAt(terrains, thing.transform.position)));
            }

            TerrainLayer[] layers =
            {
                AssetDatabase.LoadAssetAtPath<TerrainLayer>(TerrainFolder + "GrassFieldLayer.terrainlayer"),
                Layer("PathLayer", "Assets/Textures/Terrain/Path_Seamless.png", 6f),
                Layer("MountainLayer", "Assets/Textures/Terrain/Mountain_Seamless.png", 16f),
            };

            foreach (Terrain terrain in terrains)
            {
                TerrainData data = terrain.terrainData;
                Undo.RegisterCompleteObjectUndo(data, "Build Field From Map");
                Undo.RecordObject(terrain.transform, "Build Field From Map");
                Vector3 position = terrain.transform.position;
                terrain.transform.position = new Vector3(position.x, TerrainBottom, position.z);
                data.size = new Vector3(data.size.x, TerrainHeight, data.size.z);
                data.terrainLayers = layers;
                Shape(terrain, houses);
                Paint(terrain);
                terrain.Flush();
                EditorUtility.SetDirty(data);
            }

            Undo.IncrementCurrentGroup();
            BuildVillage(houses);
            foreach ((Transform thing, float above) in standing)
            {
                Undo.RecordObject(thing, "Build Field From Map");
                Vector3 at = thing.position;
                thing.position = new Vector3(at.x, GroundAt(terrains, at) + above, at.z);
            }
            PlaceCubes(terrains, cubes, standing);

            StoryGrassPlanter.Plant((x, z) => Sample(grass, x, z) >= GrassInset);
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(terrains[0].gameObject.scene);

            foreach ((Transform thing, float _) in standing)
            {
                Kind kind = KindAt(thing.position.x, thing.position.z);
                if (kind == Kind.Water || kind == Kind.Mountain || kind == Kind.House || kind == Kind.Door)
                    Debug.LogWarning($"StoryFieldBuilder: {thing.name} is now in {kind}, at {thing.position}.", thing);
            }
            Debug.Log($"StoryFieldBuilder: built {terrains.Length} tiles from the map, {houses.Count} houses, {cubes.Count} cube places.");
        }

        static void ReadMap()
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(MapPath));
            mapWidth = texture.width;
            mapHeight = texture.height;
            Color32[] pixels = texture.GetPixels32();
            Object.DestroyImmediate(texture);

            var kinds = new Kind[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                Color32 c = pixels[i];
                int best = int.MaxValue;
                foreach ((Kind kind, Color32 colour) in Palette)
                {
                    int r = c.r - colour.r, g = c.g - colour.g, b = c.b - colour.b;
                    int distance = r * r + g * g + b * b;
                    if (distance < best)
                    {
                        best = distance;
                        kinds[i] = kind;
                    }
                }
            }

            // Each pixel becomes the commonest kind around it, to lose the odd pixel at the edges between colours
            // (a blur between two colours can be nearest to a third).
            map = new Kind[kinds.Length];
            var counts = new int[Palette.Length];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    System.Array.Clear(counts, 0, counts.Length);
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                            counts[(int)kinds[Mathf.Clamp(y + dy, 0, mapHeight - 1) * mapWidth + Mathf.Clamp(x + dx, 0, mapWidth - 1)]]++;
                    int most = (int)kinds[y * mapWidth + x];
                    for (int k = 0; k < counts.Length; k++)
                        if (counts[k] > counts[most])
                            most = k;
                    map[y * mapWidth + x] = (Kind)most;
                }
            }

            metresPerPixel = area.width / mapWidth;
            land = Distances(k => k != Kind.Water);
            mountain = Distances(k => k == Kind.Mountain);
            path = Distances(k => k == Kind.Path);
            grass = Distances(k => k == Kind.Grass);
        }

        // How far each pixel is from the edge of where `inside` holds (m), positive inside and negative outside.
        static float[] Distances(System.Func<Kind, bool> inside)
        {
            int count = map.Length;
            var isInside = new bool[count];
            for (int i = 0; i < count; i++)
                isInside[i] = inside(map[i]);
            float[] toOutside = Chamfer(isInside, true);
            float[] toInside = Chamfer(isInside, false);
            var signed = new float[count];
            for (int i = 0; i < count; i++)
                signed[i] = (isInside[i] ? toOutside[i] - 0.5f : 0.5f - toInside[i]) * metresPerPixel;
            return signed;
        }

        // The distance in pixels from each pixel where isInside is `from` to the nearest where it isn't: two
        // sweeps across the picture, near enough for land.
        static float[] Chamfer(bool[] isInside, bool from)
        {
            const float Straight = 1f, Diagonal = 1.4142f;
            int w = mapWidth, h = mapHeight;
            var d = new float[w * h];
            for (int i = 0; i < d.Length; i++)
                d[i] = isInside[i] == from ? 1e6f : 0f;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (d[i] == 0f)
                        continue;
                    if (x > 0) d[i] = Mathf.Min(d[i], d[i - 1] + Straight);
                    if (y > 0)
                    {
                        d[i] = Mathf.Min(d[i], d[i - w] + Straight);
                        if (x > 0) d[i] = Mathf.Min(d[i], d[i - w - 1] + Diagonal);
                        if (x < w - 1) d[i] = Mathf.Min(d[i], d[i - w + 1] + Diagonal);
                    }
                }
            }
            for (int y = h - 1; y >= 0; y--)
            {
                for (int x = w - 1; x >= 0; x--)
                {
                    int i = y * w + x;
                    if (d[i] == 0f)
                        continue;
                    if (x < w - 1) d[i] = Mathf.Min(d[i], d[i + 1] + Straight);
                    if (y < h - 1)
                    {
                        d[i] = Mathf.Min(d[i], d[i + w] + Straight);
                        if (x < w - 1) d[i] = Mathf.Min(d[i], d[i + w + 1] + Diagonal);
                        if (x > 0) d[i] = Mathf.Min(d[i], d[i + w - 1] + Diagonal);
                    }
                }
            }
            return d;
        }

        // The pixels of one colour that touch, each group as its pixels' positions.
        static List<List<Vector2Int>> Groups(System.Func<Kind, bool> of)
        {
            var groups = new List<List<Vector2Int>>();
            var seen = new bool[map.Length];
            var stack = new Stack<Vector2Int>();
            for (int start = 0; start < map.Length; start++)
            {
                if (seen[start] || !of(map[start]))
                    continue;
                var group = new List<Vector2Int>();
                seen[start] = true;
                stack.Push(new Vector2Int(start % mapWidth, start / mapWidth));
                while (stack.Count > 0)
                {
                    Vector2Int p = stack.Pop();
                    group.Add(p);
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int x = p.x + dx, y = p.y + dy;
                            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
                                continue;
                            int i = y * mapWidth + x;
                            if (seen[i] || !of(map[i]))
                                continue;
                            seen[i] = true;
                            stack.Push(new Vector2Int(x, y));
                        }
                    }
                }
                groups.Add(group);
            }
            return groups;
        }

        // Each pink shape as the rectangle round it, with its door where the dark red in it is.
        static List<House> FindHouses()
        {
            var houses = new List<House>();
            foreach (List<Vector2Int> group in Groups(k => k == Kind.House || k == Kind.Door))
            {
                if (group.Count < SmallestHouse)
                    continue;
                Vector2Int min = group[0], max = group[0];
                Vector2 door = Vector2.zero;
                int doorPixels = 0;
                foreach (Vector2Int p in group)
                {
                    min = Vector2Int.Min(min, p);
                    max = Vector2Int.Max(max, p);
                    if (map[p.y * mapWidth + p.x] == Kind.Door)
                    {
                        door += p;
                        doorPixels++;
                    }
                }
                Vector2 from = ToWorld(min.x, min.y), to = ToWorld(max.x + 1, max.y + 1);
                var house = new House { rect = Rect.MinMaxRect(from.x, from.y, to.x, to.y) };
                // No door drawn: one in the middle of the side toward the camera.
                house.door = doorPixels > 0 ? ToWorld(door.x / doorPixels + 0.5f, door.y / doorPixels + 0.5f) : new Vector2(house.rect.center.x, house.rect.yMin);
                houses.Add(house);
            }
            return houses;
        }

        // The middle of each yellow dot, from the top of the map down.
        static List<Vector2> FindCubes()
        {
            var cubes = new List<Vector2>();
            foreach (List<Vector2Int> group in Groups(k => k == Kind.Cube))
            {
                if (group.Count < SmallestCube)
                    continue;
                Vector2 sum = Vector2.zero;
                foreach (Vector2Int p in group)
                    sum += p;
                cubes.Add(ToWorld(sum.x / group.Count + 0.5f, sum.y / group.Count + 0.5f));
            }
            cubes.Sort((a, b) => b.y.CompareTo(a.y));
            return cubes;
        }

        static Vector2 ToWorld(float px, float py)
        {
            return new Vector2(area.xMin + px / mapWidth * area.width, area.yMin + py / mapHeight * area.height);
        }

        // A value of one of the distance maps at a place, between its pixels.
        static float Sample(float[] values, float x, float z)
        {
            float px = Mathf.Clamp((x - area.xMin) / area.width * mapWidth - 0.5f, 0f, mapWidth - 1.001f);
            float py = Mathf.Clamp((z - area.yMin) / area.height * mapHeight - 0.5f, 0f, mapHeight - 1.001f);
            int x0 = (int)px, y0 = (int)py;
            float fx = px - x0, fy = py - y0;
            int i = y0 * mapWidth + x0;
            float bottom = Mathf.Lerp(values[i], values[i + 1], fx);
            float top = Mathf.Lerp(values[i + mapWidth], values[i + mapWidth + 1], fx);
            return Mathf.Lerp(bottom, top, fy);
        }

        static Kind KindAt(float x, float z)
        {
            int px = Mathf.Clamp(Mathf.FloorToInt((x - area.xMin) / area.width * mapWidth), 0, mapWidth - 1);
            int py = Mathf.Clamp(Mathf.FloorToInt((z - area.yMin) / area.height * mapHeight), 0, mapHeight - 1);
            return map[py * mapWidth + px];
        }

        static float Height(float x, float z, List<House> houses)
        {
            float landHeight = water + LandAbove + (Mathf.PerlinNoise(x / BumpSize + 41.3f, z / BumpSize + 17.9f) - 0.5f) * 2f * LandBumps;

            // Level under and round the houses, at the land's own height without its bumps.
            float levelled = 0f;
            foreach (House house in houses)
            {
                float dx = Mathf.Max(house.rect.xMin - x, 0f, x - house.rect.xMax);
                float dz = Mathf.Max(house.rect.yMin - z, 0f, z - house.rect.yMax);
                float outside = Mathf.Sqrt(dx * dx + dz * dz);
                levelled = Mathf.Max(levelled, 1f - Mathf.SmoothStep(0f, 1f, (outside - Levelled) / LevelledBlend));
            }
            landHeight = Mathf.Lerp(landHeight, water + LandAbove, levelled);

            float s = Sample(land, x, z);
            float height;
            if (s >= 0f)
            {
                float t = Mathf.Clamp01(s / ShoreOver);
                height = Mathf.Lerp(water - ShoreUnder, landHeight, 1f - (1f - t) * (1f - t));
            }
            else
            {
                float t = Mathf.Clamp01(-s / DeepOver);
                height = water - ShoreUnder - (WaterDepth - ShoreUnder) * Mathf.SmoothStep(0f, 1f, t);
            }

            float m = Sample(mountain, x, z);
            if (m > 0f)
            {
                float t = Mathf.Clamp01(m / CliffOver);
                float cliff = (1f - (1f - t) * (1f - t)) * CliffHeight;
                float slope = Mathf.Min(Mathf.Max(0f, m - CliffOver) * Rise, MountainHeight - CliffHeight);
                float bumps = Mathf.PerlinNoise(x / MountainBumpSize + 7.1f, z / MountainBumpSize + 93.4f) * MountainBumps;
                height += cliff + slope + bumps * Mathf.Clamp01((m - CliffOver) / 6f);
            }
            return height;
        }

        static void Shape(Terrain terrain, List<House> houses)
        {
            TerrainData data = terrain.terrainData;
            int resolution = data.heightmapResolution;
            Vector3 corner = terrain.transform.position, size = data.size;
            var heights = new float[resolution, resolution];
            for (int iz = 0; iz < resolution; iz++)
            {
                for (int ix = 0; ix < resolution; ix++)
                {
                    float x = corner.x + ix / (resolution - 1f) * size.x;
                    float z = corner.z + iz / (resolution - 1f) * size.z;
                    heights[iz, ix] = Mathf.Clamp01((Height(x, z, houses) - corner.y) / size.y);
                }
            }
            data.SetHeights(0, 0, heights);
        }

        // Grass everywhere, the path's ground on the paths and the mountain's on the mountains.
        static void Paint(Terrain terrain)
        {
            TerrainData data = terrain.terrainData;
            int resolution = data.alphamapResolution;
            Vector3 corner = terrain.transform.position, size = data.size;
            var weights = new float[resolution, resolution, 3];
            for (int iz = 0; iz < resolution; iz++)
            {
                for (int ix = 0; ix < resolution; ix++)
                {
                    float x = corner.x + (ix + 0.5f) / resolution * size.x;
                    float z = corner.z + (iz + 0.5f) / resolution * size.z;
                    float rock = Mathf.SmoothStep(0f, 1f, Sample(mountain, x, z) / MountainEdge + 0.5f);
                    float dirt = Mathf.SmoothStep(0f, 1f, Sample(path, x, z) / PathEdge + 0.5f) * (1f - rock);
                    weights[iz, ix, 0] = 1f - rock - dirt;
                    weights[iz, ix, 1] = dirt;
                    weights[iz, ix, 2] = rock;
                }
            }
            data.SetAlphamaps(0, 0, weights);
        }

        static TerrainLayer Layer(string name, string texturePath, float tileSize)
        {
            string path = TerrainFolder + name + ".terrainlayer";
            var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            if (layer != null)
                return layer;
            layer = new TerrainLayer
            {
                diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath),
                tileSize = new Vector2(tileSize, tileSize),
                smoothness = 0f,
                metallic = 0f,
            };
            AssetDatabase.CreateAsset(layer, path);
            return layer;
        }

        static Material HouseMaterial(string name, Color colour)
        {
            string path = MaterialFolder + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
                return material;
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", colour);
            material.SetFloat("_Smoothness", 0f);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        // Each house: a floor, four walls with a gap for the door in one, a roof, a light, and a CaveZone over its
        // inside, so going in changes the view to the cave view as a cave does. The walls and floor are on the
        // Cave layer, which is all that view draws; the roof and the wall nearest the camera (the south one) are
        // on CaveCeiling, which it leaves out, so the inside can be seen from above and in front.
        static void BuildVillage(List<House> houses)
        {
            GameObject old = GameObject.Find(VillageName);
            if (old != null)
                Undo.DestroyObjectImmediate(old);
            var village = new GameObject(VillageName);
            Undo.RegisterCreatedObjectUndo(village, "Build Field From Map");

            Material wall = HouseMaterial("House Wall", new Color(0.77f, 0.53f, 0.57f));
            Material roof = HouseMaterial("House Roof", new Color(0.55f, 0.33f, 0.36f));
            Material floor = HouseMaterial("House Floor", new Color(0.42f, 0.3f, 0.22f));
            int cave = LayerMask.NameToLayer("Cave"), ceiling = LayerMask.NameToLayer("CaveCeiling");
            float ground = water + LandAbove;

            for (int h = 0; h < houses.Count; h++)
            {
                Rect r = houses[h].rect;
                Vector2 door = houses[h].door;
                var house = new GameObject($"House {h + 1}").transform;
                house.SetParent(village.transform, false);
                house.position = new Vector3(r.center.x, ground, r.center.y);

                // Which side the door is in: the one its dot is nearest.
                float[] toSide = { door.y - r.yMin, r.yMax - door.y, door.x - r.xMin, r.xMax - door.x };
                int side = 0;
                for (int i = 1; i < toSide.Length; i++)
                    if (toSide[i] < toSide[side])
                        side = i;

                Box(house, "Floor", new Vector3(r.center.x, ground - 0.1f, r.center.y), new Vector3(r.width, 0.3f, r.height), floor, cave);
                Box(house, "Roof", new Vector3(r.center.x, ground + WallHeight + 0.15f, r.center.y), new Vector3(r.width + 0.6f, 0.3f, r.height + 0.6f), roof, ceiling);

                WallAlongX(house, "South Wall", r.xMin, r.xMax, r.yMin + Wall / 2f, ground, side == 0 ? door.x : (float?)null, wall, ceiling);
                WallAlongX(house, "North Wall", r.xMin, r.xMax, r.yMax - Wall / 2f, ground, side == 1 ? door.x : (float?)null, wall, cave);
                WallAlongZ(house, "West Wall", r.yMin + Wall, r.yMax - Wall, r.xMin + Wall / 2f, ground, side == 2 ? door.y : (float?)null, wall, cave);
                WallAlongZ(house, "East Wall", r.yMin + Wall, r.yMax - Wall, r.xMax - Wall / 2f, ground, side == 3 ? door.y : (float?)null, wall, cave);

                var inside = new GameObject("Inside");
                inside.transform.SetParent(house, false);
                var light = new GameObject("Light").AddComponent<Light>();
                light.transform.SetParent(inside.transform, false);
                light.transform.position = new Vector3(r.center.x, ground + WallHeight - 1f, r.center.y);
                light.type = LightType.Point;
                light.color = new Color(1f, 0.75f, 0.5f);
                light.intensity = 4f;
                light.range = Mathf.Max(r.width, r.height) * 0.75f + 3f;
                light.shadows = LightShadows.None;

                var zone = new GameObject("Cave Zone");
                zone.transform.SetParent(house, false);
                zone.transform.position = new Vector3(r.center.x, ground + WallHeight / 2f, r.center.y);
                var box = zone.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = new Vector3(r.width - Wall * 2f, WallHeight, r.height - Wall * 2f);
                zone.AddComponent<CaveZone>().onlyInside = inside;
            }
        }

        // A wall from x0 to x1 at z, with a door gap at `door` along it if there is one.
        static void WallAlongX(Transform house, string name, float x0, float x1, float z, float ground, float? door, Material material, int layer)
        {
            foreach ((float from, float to, float bottom) in Pieces(x0, x1, door))
                Box(house, name, new Vector3((from + to) / 2f, ground + (bottom + WallHeight) / 2f, z), new Vector3(to - from, WallHeight - bottom, Wall), material, layer);
        }

        static void WallAlongZ(Transform house, string name, float z0, float z1, float x, float ground, float? door, Material material, int layer)
        {
            foreach ((float from, float to, float bottom) in Pieces(z0, z1, door))
                Box(house, name, new Vector3(x, ground + (bottom + WallHeight) / 2f, (from + to) / 2f), new Vector3(Wall, WallHeight - bottom, to - from), material, layer);
        }

        // A wall's pieces along it: the whole of it, or the two sides of a door and the bit above it.
        static IEnumerable<(float from, float to, float bottom)> Pieces(float from, float to, float? door)
        {
            if (door == null || to - from < DoorWidth + 1f)
            {
                yield return (from, to, 0f);
                yield break;
            }
            float middle = Mathf.Clamp(door.Value, from + DoorWidth / 2f + 0.5f, to - DoorWidth / 2f - 0.5f);
            yield return (from, middle - DoorWidth / 2f, 0f);
            yield return (middle + DoorWidth / 2f, to, 0f);
            yield return (middle - DoorWidth / 2f, middle + DoorWidth / 2f, DoorHeight);
        }

        static void Box(Transform parent, string name, Vector3 centre, Vector3 size, Material material, int layer)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.layer = layer;
            box.transform.SetParent(parent, false);
            box.transform.position = centre;
            box.transform.localScale = size;
            box.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        // The cubes on the yellow dots, as high above the ground as each stood before.
        static void PlaceCubes(Terrain[] terrains, List<Vector2> places, List<(Transform thing, float above)> standing)
        {
            GameObject parent = GameObject.Find("Artifacts");
            if (parent == null)
                return;
            int count = Mathf.Min(places.Count, parent.transform.childCount);
            for (int i = 0; i < count; i++)
            {
                Transform cube = parent.transform.GetChild(i);
                float above = standing.Find(s => s.thing == cube).above;
                var at = new Vector3(places[i].x, 0f, places[i].y);
                at.y = GroundAt(terrains, at) + above;
                cube.position = at;
            }
            if (places.Count != parent.transform.childCount)
                Debug.LogWarning($"StoryFieldBuilder: the map has {places.Count} cube places and there are {parent.transform.childCount} cubes; {count} placed.");
        }

        static float GroundAt(Terrain[] terrains, Vector3 at)
        {
            foreach (Terrain terrain in terrains)
            {
                Vector3 corner = terrain.transform.position, size = terrain.terrainData.size;
                if (at.x >= corner.x && at.x <= corner.x + size.x && at.z >= corner.z && at.z <= corner.z + size.z)
                    return corner.y + terrain.terrainData.GetInterpolatedHeight((at.x - corner.x) / size.x, (at.z - corner.z) / size.z);
            }
            return at.y;
        }
    }
}
