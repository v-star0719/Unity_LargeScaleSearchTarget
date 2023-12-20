using UnityEngine;

namespace Assets.Temp
{
    public class OctreeNode : OrthtreeNode
    {
        public OctreeNode(Orthtree tree) : base(tree)
        {
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            Gizmos.DrawWireCube(new Vector3(Pos[0], Pos[1], Pos[2]), new Vector3(Size[0], Size[1], Size[2]));
        }

        protected override bool IsZoneContanisObj(float[] pos, float[] size, IOrthtreeObj obj)
        {
            var p = obj.Pos;
            return Mathf.Abs(pos[0] - p[0]) <= size[0] * 0.5f &&
                   Mathf.Abs(pos[1] - p[1]) <= size[1] * 0.5f &&
                   Mathf.Abs(pos[2] - p[2]) <= size[2] * 0.5f;
        }

        protected override bool IsZoneIntersect(float[] pos1, float[] size1, float[] pos2, float[] size2)
        {
            return Mathf.Abs(pos1[0] - pos2[0]) <= (size1[0] + size2[0]) * 0.5f &&
                   Mathf.Abs(pos1[1] - pos2[1]) <= (size1[1] + size2[1]) * 0.5f &&
                   Mathf.Abs(pos1[2] - pos2[2]) <= (size1[2] + size2[2]) * 0.5f;
        }

        public override float SqrDistance(OrthtreeNode node)
        {
            var f0 = node.Pos[0] - Pos[0];
            var f1 = node.Pos[1] - Pos[1];
            var f2 = node.Pos[2] - Pos[2];
            return f0 * f0 + f1 * f1 + f2 * f2;
        }
    }
}