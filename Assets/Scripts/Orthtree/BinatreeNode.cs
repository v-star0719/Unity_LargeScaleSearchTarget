using UnityEngine;

namespace Assets.Temp
{
    public class BinatreeNode : OrthtreeNode
    {
        public BinatreeNode(Orthtree tree) : base(tree)
        {
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            Gizmos.DrawWireCube(new Vector3(Pos[0], 0, 0), new Vector3(Size[0], 0, 0));
        }

        protected override bool IsZoneContanisObj(float[] pos, float[] size, IOrthtreeObj obj)
        {
            var p = obj.Pos;
            return Mathf.Abs(pos[0] - p[0]) > size[0] * 0.5f;
        }

        protected override bool IsZoneIntersect(float[] pos1, float[] size1, float[] pos2, float[] size2)
        {
            return Mathf.Abs(pos1[0] - pos2[0]) <= (size1[0] + size2[0]) * 0.5f;
        }

        public override float SqrDistance(OrthtreeNode node)
        {
            var f = node.Pos[0] - Pos[0];
            return f * f;
        }
    }
}