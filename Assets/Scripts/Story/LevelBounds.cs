using Unity.Cinemachine;
using UnityEngine;

namespace Collection.Story
{
    // The edges of a level: walls that nothing walks, rides or tumbles through, and the same box for the cameras,
    // so the view does not go out past the land's edge to where there is nothing.
    //
    // The box is worked out here, as the scene starts, from the level's terrain tiles: the rectangle they cover
    // between them, drawn in by `inset`. So a level whose land is made bigger (StoryTerrainBuilder) needs nothing
    // changed here. The walls are four box colliders; the cameras' box is a trigger collider, given to a
    // CinemachineConfiner3D on each of `cameras`.
    //
    // The walls stand whatever the land is doing (they are there in the sea before the land comes up, far out).
    public class LevelBounds : MonoBehaviour
    {
        [Tooltip("How far in from the outer edge of the land the walls stand (m). The land's last stretch goes down under the sea; the walls are best in that stretch.")]
        public float inset = 14f;
        [Tooltip("How near the walls the middle of the picture may come (m): to the sides, toward the camera, and away from it. Less than the picture reaches that way from its middle, so the character can still be seen at the wall.")]
        public float viewSide = 7f;
        public float viewNear = 3f;
        public float viewFar = 5f;
        [Tooltip("The distance of the camera from the character that those three are given for (m): the outdoor camera's. A camera nearer or further has them scaled to its own.")]
        public float viewsAtDistance = 19.8f;
        [Tooltip("The walls' top and bottom (m).")]
        public float top = 80f;
        public float bottom = -40f;
        public float thickness = 6f;
        [Tooltip("The Cinemachine cameras kept inside. Empty: every one in the scene that follows something (the outdoor and the cave camera).")]
        public CinemachineCamera[] cameras;

        void Awake()
        {
            // The rectangle the land covers.
            bool any = false;
            float minX = 0f, maxX = 0f, minZ = 0f, maxZ = 0f;
            foreach (Terrain tile in FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Vector3 corner = tile.transform.position;
                Vector3 size = tile.terrainData.size;
                if (!any)
                {
                    minX = corner.x; maxX = corner.x + size.x; minZ = corner.z; maxZ = corner.z + size.z;
                    any = true;
                    continue;
                }
                minX = Mathf.Min(minX, corner.x);
                maxX = Mathf.Max(maxX, corner.x + size.x);
                minZ = Mathf.Min(minZ, corner.z);
                maxZ = Mathf.Max(maxZ, corner.z + size.z);
            }
            if (!any)
            {
                Debug.LogWarning("LevelBounds: no terrain in the scene to take the level's edges from.", this);
                return;
            }

            minX += inset; maxX -= inset; minZ += inset; maxZ -= inset;
            float middleX = (minX + maxX) * 0.5f, middleZ = (minZ + maxZ) * 0.5f;
            float width = maxX - minX, depth = maxZ - minZ;
            float middleY = (top + bottom) * 0.5f, height = top - bottom;

            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;

            // Each wall's inner face on the rectangle's edge, and long enough to close the corners.
            Wall("Wall West", new Vector3(minX - thickness * 0.5f, middleY, middleZ), new Vector3(thickness, height, depth + thickness * 2f));
            Wall("Wall East", new Vector3(maxX + thickness * 0.5f, middleY, middleZ), new Vector3(thickness, height, depth + thickness * 2f));
            Wall("Wall South", new Vector3(middleX, middleY, minZ - thickness * 0.5f), new Vector3(width + thickness * 2f, height, thickness));
            Wall("Wall North", new Vector3(middleX, middleY, maxZ + thickness * 0.5f), new Vector3(width + thickness * 2f, height, thickness));

            // The cameras' boxes, one for each: a camera stands behind and above what it follows, so its box is the
            // walls' rectangle moved by that much, and drawn in by how far the picture reaches each way from the
            // character - so that the character can walk on to the wall, toward the picture's edge, while the
            // picture itself stops short of showing what is beyond.
            CinemachineCamera[] kept = cameras;
            if (kept == null || kept.Length == 0)
                kept = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (CinemachineCamera camera in kept)
            {
                // Of all the scene's cameras, only those that follow: a cutscene's camera is put where it is
                // meant to be.
                if (camera == null || camera.Follow == null)
                    continue;

                Vector3 offset = camera.TryGetComponent(out CinemachineFollow follow) ? follow.FollowOffset : Vector3.zero;
                // Scaled to the camera's distance.
                float reach = offset.magnitude > 0.01f && viewsAtDistance > 0f ? offset.magnitude / viewsAtDistance : 1f;
                float west = minX + viewSide * reach + offset.x, east = maxX - viewSide * reach + offset.x;
                float south = minZ + viewNear * reach + offset.z, north = maxZ - viewFar * reach + offset.z;

                var volume = new GameObject("Camera Volume (" + camera.name + ")");
                // A trigger, on the layer rays pass by, so it is in nothing's way.
                volume.layer = 2;
                volume.transform.SetParent(transform, false);
                volume.transform.position = new Vector3((west + east) * 0.5f, middleY, (south + north) * 0.5f);
                var box = volume.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = new Vector3(Mathf.Max(1f, east - west), height, Mathf.Max(1f, north - south));

                if (!camera.TryGetComponent(out CinemachineConfiner3D confiner))
                    confiner = camera.gameObject.AddComponent<CinemachineConfiner3D>();
                confiner.BoundingVolume = box;
            }
        }

        void Wall(string name, Vector3 at, Vector3 size)
        {
            var wall = new GameObject(name);
            wall.layer = gameObject.layer;
            wall.transform.SetParent(transform, false);
            wall.transform.position = at;
            wall.AddComponent<BoxCollider>().size = size;
        }
    }
}
