using System.Collections.Generic;
using UnityEngine;

namespace Assets.Temp
{
    //这把对象当成点了，没有考虑体积。所以一个对象不会横跨多个区。
    public class OrthtreeNode
    {
        public OrthtreeNode parent;
        public List<OrthtreeNode> children = new List<OrthtreeNode>();
        public List<IOrthtreeObj> objs = new List<IOrthtreeObj>();//只有叶子节点会包含

        public Orthtree tree;
        public int depth;
        private float[] pos;
        private float[] size;

        public float[] Pos
        {
            get => pos;
            private set
            {
                OrthtreeUtils.CoptyArray(value, pos);
            }
        }

        public float[] Size
        {
            get => size;
            private set
            {
                OrthtreeUtils.CoptyArray(value, size);
            }
        }

        public OrthtreeNode(Orthtree tree)
        {
            pos = new float[tree.dimension];
            size = new float[tree.dimension];
            this.tree = tree;
        }

        public void Init(OrthtreeNode parent, int depth, float[] pos, float[] size, List<IOrthtreeObj> objs)
        {
            this.parent = parent;
            this.depth = depth;
            this.Size = size;
            this.Pos = pos;
         
            //划分子空间
            bool separate = false;
            if(depth < tree.maxDepth)
            {
                //已经是最小尺寸的维度不再进行划分
                //var childSize = size.Clone() as float[];
                for(int i = 0; i < tree.dimension; i++)
                {
                    if(size[i] > tree.minSizes[i])
                    {
                        separate = true;
                        break;
                    }
                }

                if (separate)
                {
                    SeparateSpace(pos, size, 0, objs);
                }

                ////空间划分
                //for(int i = 0; i < tree.dimension; i++)
                //{
                //    if(size[i] > tree.minSizes[i])
                //    {
                //        pos[i] -= childSize[i] * 0.5f;
                //        SeparateSpace(pos, childSize, objs);
                //        pos[i] += childSize[i];
                //        SeparateSpace(pos, childSize, objs);
                //        pos[i] -= childSize[i] * 0.5f;
                //    }
                //}
            }

            if (!separate)
            {
                this.objs.AddRange(objs);
            }
        }

        /// dimension: 从0开始数
        /// pos:初始传入父节点的位置
        /// size:初始传入父节点的大小
        protected void SeparateSpace(float[] pos, float[] size, int dimension, List<IOrthtreeObj> objs)
        {
            if (dimension >= tree.dimension)
            {
                SeparateSpace(pos, size, objs);
                return;
            }

            //已经是最小尺寸的维度不再进行划分，直接进入下一个维度
            if(size[dimension] <= tree.minSizes[dimension])
            {
                SeparateSpace(pos, size, dimension + 1, objs);
                return;
            }

            size[dimension] *= 0.5f;

            pos[dimension] -= size[dimension] * 0.5f;
            SeparateSpace(pos, size, dimension + 1, objs);
            pos[dimension] += size[dimension];
            SeparateSpace(pos, size, dimension + 1, objs);
            pos[dimension] -= size[dimension] * 0.5f;

            size[dimension] *= 2;
        }

        public void Clear()
        {
            parent = null;
            foreach (var child in children)
            {
                child.Clear();
                tree.RecylceNode(child);
            }
            children.Clear();
            objs.Clear();
        }

        public virtual void DrawGizmos()
        {
            foreach (var child in children)
            {
                child.DrawGizmos();
            }
        }

        public virtual float SqrDistance(OrthtreeNode node)
        {
            return OrthtreeUtils.SqrDistance(node.pos, pos, tree.dimension);
        }

        public void Search(float[] pos, float[] size, List<IOrthtreeObj> outList)
        {
            if(!IsIntersectWithZone(pos, size))
            {
                return;
            }

            //todo 如果一个对象跨越多个区，需要去重。
            outList.AddRange(objs);
            foreach (var child in children)
            {
                child.Search(pos, size, outList);
            }
        }

        //分离空间. pos和size会直接保存使用，objs中的对象分离出去后会从列表里删除
        protected void SeparateSpace(float[] pos, float[] size, List<IOrthtreeObj> objs)
        {
            List<IOrthtreeObj> contains = new List<IOrthtreeObj>();
            for (int i = objs.Count - 1; i >= 0; i--)
            {
                if (IsZoneContanisObj(pos, size, objs[i]))
                {
                    contains.Add(objs[i]);
                    objs.RemoveAt(i);
                }
            }

            if (contains.Count > 0)
            {
                var node = tree.GetNode();
                node.Init(this, depth + 1, pos, size, contains);
                children.Add(node);
            }
        }

        protected bool ContainsObj(IOrthtreeObj obj)
        {
            return IsZoneContanisObj(pos, size, obj);
        }

        protected bool IsIntersectWithZone(float[] pos, float[] size)
        {
            return IsZoneIntersect(this.pos, this.size, pos, size);
        }

        //工具函数，1/2/3维重载去除了for
        protected virtual bool IsZoneContanisObj(float[] pos, float[] size, IOrthtreeObj obj)
        {
            var p = obj.Pos;
            for(int i = 0; i < tree.dimension; i++)
            {
                if (Mathf.Abs(p[i] - pos[i]) > size[i] * 0.5f)
                {
                    return false;
                }
            }
            return true;
        }

        //工具函数，1/2/3维重载去除了for
        protected virtual bool IsZoneIntersect(float[] pos1, float[] size1, float[] pos2, float[] size2)
        {
            for(int i = 0; i < tree.dimension; i++)
            {
                if (Mathf.Abs(pos1[i] - pos2[i]) > (size1[i] + size2[i]) * 0.5f)
                {
                    return false;
                }
            }
            return true;
        }
    }
}