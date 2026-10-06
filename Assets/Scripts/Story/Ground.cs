using UnityEngine;

namespace Collection.Story
{
    // What there is to stand on under a place (Under): one answer for everything that puts something down on the ground
    // (the character getting up after a fall, getting off the bicycle, the bicycle being stood up, a saved place
    // being checked).
    //
    // It is the nearest thing below, not the highest thing there: in a cave the highest is the land over the
    // cave. Triggers are not ground.
    public static class Ground
    {
        // The height of the nearest thing within `reach` metres below `from`. `not`: whose own colliders, and
        // those of everything under them, do not count (the character itself, the bicycle). False when there is
        // nothing.
        public static bool Under(Vector3 from, float reach, LayerMask mask, out float height, params Transform[] not)
        {
            height = float.MinValue;
            foreach (RaycastHit hit in Physics.RaycastAll(from, Vector3.down, reach, mask, QueryTriggerInteraction.Ignore))
            {
                if (hit.point.y <= height || Among(hit.collider.transform, not))
                    continue;
                height = hit.point.y;
            }
            return height > float.MinValue;
        }

        // The height of the land's surface at a place: of the terrain there, wherever it is at the moment (the
        // desert's comes up out of the sea). What is over or under the land - a cave's floor, a rock - is not
        // in it. Far below everything where there is no land.
        public static float Land(Vector3 at)
        {
            foreach (Terrain tile in Terrain.activeTerrains)
            {
                Vector3 corner = tile.transform.position;
                Vector3 size = tile.terrainData.size;
                float x = (at.x - corner.x) / size.x;
                float z = (at.z - corner.z) / size.z;
                if (x < 0f || x > 1f || z < 0f || z > 1f)
                    continue;
                return corner.y + tile.terrainData.GetInterpolatedHeight(x, z);
            }
            return float.MinValue;
        }

        static bool Among(Transform what, Transform[] not)
        {
            foreach (Transform one in not)
                if (one != null && what.IsChildOf(one))
                    return true;
            return false;
        }
    }
}
