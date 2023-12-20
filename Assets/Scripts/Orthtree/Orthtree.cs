using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Temp
{
    //正交树，1维是线段，2叉；2维是平面，3维是空间
    public class Orthtree
    {
        public int maxDepth;//根节点是0，如果最大是10，算上根节点会有11层。
        public int dimension;//从1开始
        public float[] minSizes;
        public OrthtreeNode root;

        protected Queue<OrthtreeNode> recycledNodes = new Queue<OrthtreeNode>();

        public Orthtree(int maxDepth, float[] minSizes, int dimension)
        {
            this.maxDepth = maxDepth;
            this.dimension = dimension;
            this.minSizes = minSizes;
        }

        public void Build(float[] pos, float[] size, List<IOrthtreeObj> objs)
        {
            root = GetNode();
            root.Init(null, 0, pos, size, objs);
            //Debug.LogError("node count: " + n);
        }

        public void Build(Vector3 position, Vector3 size, List<IOrthtreeObj> objs)
        {
            Build(OrthtreeUtils.Vector3ToArray(position), OrthtreeUtils.Vector3ToArray(size), objs);
        }

        public void DrawGizmos()
        {
            root?.DrawGizmos();
        }

        //private int n = 0;
        public OrthtreeNode GetNode()
        {
            if (recycledNodes.Count > 0)
            {
                return recycledNodes.Dequeue();
            }

            //n++;
            return CreateNewNode();
        }

        public void RecylceNode(OrthtreeNode node)
        {
            recycledNodes.Enqueue(node);
        }

        public void Clear()
        {
            if (root == null)
            {
                return;
            }
            root.Clear();
            RecylceNode(root);
            root = null;
            //Debug.LogError("RecycleNode " + recycledNodes.Count);
        }

        public List<IOrthtreeObj> Search(float[] pos, float[] size)
        {
            List<IOrthtreeObj> objs = new List<IOrthtreeObj>();
            root.Search(pos, size, objs);
            return objs;
        }

        public List<IOrthtreeObj> Search(Vector3 pos, float radius)
        {
            var sz = radius * 2;
            var position = OrthtreeUtils.Vector3ToArray(pos);
            var rt = Search(position, new float[] { sz, sz, sz });

            var sqrRadius = radius * radius;
            for (var i = rt.Count - 1; i >= 0; i--)
            {
                rt[i].PickValue = OrthtreeUtils.SqrDistance(position, rt[i].Pos, dimension);
                if (rt[i].PickValue > sqrRadius)
                {
                    rt.RemoveAt(i);
                }
            }

            return rt;
        }

        public IOrthtreeObj SearchNearest(Vector3 pos, float radius)
        {
            var sz = radius * 2;
            var position = OrthtreeUtils.Vector3ToArray(pos);
            var list = Search(position, new float[] { sz, sz, sz });

            var sqrRadius = radius * radius;
            IOrthtreeObj rt = null;
            foreach (var obj in list)
            {
                obj.PickValue = OrthtreeUtils.SqrDistance(position, obj.Pos, dimension);
                if (obj.PickValue <= sqrRadius && (rt == null || rt.PickValue > obj.PickValue))
                {
                    rt = obj;
                }
            }
            return rt;
        }


        protected virtual OrthtreeNode CreateNewNode()
        {
            return new OrthtreeNode(this);
        }
    }
}
