using UnityEngine;

namespace Collection.Story
{
    // Posing a body in code, over whatever its Animator has it doing: for the moves there is no animation clip
    // for (the roll, riding the bicycle). To be used after the Animator has posed the body for the frame
    // (LateUpdate), parents before children.
    public static class Pose
    {
        // Turns a bone about an axis given in the world's directions. Nothing for a bone the body has not got.
        public static void Bend(Transform bone, Vector3 axis, float degrees)
        {
            if (bone != null)
                bone.rotation = Quaternion.AngleAxis(degrees, axis) * bone.rotation;
        }

        public static void Bend(Animator body, HumanBodyBones bone, Vector3 axis, float degrees)
        {
            Bend(body.GetBoneTransform(bone), axis, degrees);
        }
    }
}
