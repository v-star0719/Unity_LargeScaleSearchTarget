using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Core;
using Unity.VisualScripting;
using UnityEngine;

public class KDTreeNode
{
    public Monster value;
    public KDTreeNode left;
    public KDTreeNode right;
    public override string ToString()
    {
        return value != null ? value.ToString() : "null";
    }
}

public class KDTree
{
    private const int ARRAY_SIZE = 1024;
    private KDTreeNode root;
    private List<Monster> tempLeftList = new List<Monster>(1000);
    private List<Monster> tempRightList = new List<Monster>(1000);
    private Monster[] monsters = new Monster[ARRAY_SIZE];
    private Pool<KDTreeNode> nodePool;

    public KDTree()
    {
        nodePool = new Pool<KDTreeNode>(()=>new KDTreeNode());
    }

    public void Build(List<Monster> list)
    {
        if (list.Count > monsters.Length)
        {
            var newSize = (list.Count / ARRAY_SIZE) * ARRAY_SIZE;
            if (newSize < list.Count)
            {
                newSize += ARRAY_SIZE;
            }
            monsters = new Monster[newSize];
        }

        for (var i = 0; i < list.Count; i++)
        {
            monsters[i] = list[i];
            monsters[i].InitKd();
        }
        BuildInner(ref root, monsters, 0, list.Count - 1, 0);
    }

    private void BuildInner(ref KDTreeNode root, Monster[] monsters, int start, int end, int d)
    {
        if (start > end)
        {
            return;
        }

        root = nodePool.Get();
        root.left = null;
        root.right = null;
        if (start == end)
        {
            root.value = monsters[start];
            return;
        }

        var count = end - start + 1;
        var mid1 = (end + start) >> 1;
        var mid2 = mid1;
        if ((count & 1) == 0)
        {
            mid2++;
        }
        //012345
        var mid = -1;
        var tStart = start;
        var tEnd = end;
        while (mid != mid1 && mid != mid2)
        {
            mid = Partition(monsters, tStart, tEnd, d);
            if (mid > mid1)
            {
                tEnd = mid - 1;
            }
            else
            {
                tStart = mid + 1;
            }
        }

        root.value = monsters[mid];
        d = d < 2 ? d + 1 : 0;
        BuildInner(ref root.left, monsters, start, mid - 1, d);
        BuildInner(ref root.right, monsters, mid + 1, end, d);
    }

    public static int Partition(Monster[] array, int start, int end, int d)
    {
        var c = (end + start) >> 1;
        if (c < 0)
        {
            Debug.LogError("=");
        }
        Swap(array, c, end);

        //中间值放到最后
        var n = array[end].kd[d];
        int i = start - 1;//把比n小放前面

        //循环结束后，i指向最后一个比n小的数。如果没有比n小的，指向start-1。
        for (int j = start; j < end; j++)
        {
            if (array[j].kd[d] <= n)
            {
                i++;
                Swap(array, i, j);
            }
        }

        i++;
        Swap(array, i, end);
        return i;
    }

    public void Display()
    {
        if (root == null)
        {
            return;
        }
        List<KDTreeNode> nodes = new List<KDTreeNode>();
        List<KDTreeNode> nextNodes = new List<KDTreeNode>();
        if (root != null)
        {
            nodes.Add(root);
        }

        StringBuilder sb = new StringBuilder("");
        int d = 0;
        while (nodes.Count > 0)
        {
            sb.AppendFormat("d = {0}: ", d);
            foreach (var node in nodes)
            {
                sb.AppendFormat("{0} ({1}), ", node.value.kd[d], node.value.name);
                if (node.left != null)
                {
                    nextNodes.Add(node.left);
                }
                if (node.right != null)
                {
                    nextNodes.Add(node.right);
                }
            }
            sb.Append("\n");

            nodes.Clear();
            var t = nodes;
            nodes = nextNodes;
            nextNodes = t;
            d = d < 2 ? d + 1 : 0;
        }
        Debug.Log(sb.ToString());
    }

    public void Clear()
    {
        if (root != null)
        {
            ClearInner(root);
            root = null;
        }
    }

    private static int searchTimes = 0;
    public Monster SearchNearestInRange(Vector3 pos, float radius)
    {
        float nearest = float.MaxValue;
        Monster rt = null;
        searchTimes = 0;
        SearchNearestInRangeInner(pos, radius, root, 0, ref nearest, ref rt);
        //Debug.Log("searchTimes " + searchTimes);
        return rt;
    }

    //范围搜索
    private void SearchNearestInRangeInner(Vector3 pos, float radius, KDTreeNode node, int d, ref float nearestDist, ref Monster target)
    {
        searchTimes++;
        var v = node.value.kd[d];
        if (pos[d] - radius < v && node.left != null)
        {
            SearchNearestInRangeInner(pos, radius, node.left, d < 2 ? d + 1 : 0, ref nearestDist, ref target);
        }
        if (pos[d] + radius > v && node.right != null)
        {
            SearchNearestInRangeInner(pos, radius, node.right, d < 2 ? d + 1 : 0, ref nearestDist, ref target);
        }

        if (pos[d] - radius < v && v < pos[d] + radius)
        {
            var dist = Utils.SqrDistance(pos, node.value.transform.position);
            if (dist < nearestDist && dist < radius * radius)
            {
                target = node.value;
                nearestDist = dist;
            }
        }
    }

    struct SearchResult
    {
        public Monster m;
        public float dist;

        public SearchResult(Monster m, float dist)
        {
            this.m = m;
            this.dist = dist;
        }
    }

    public Monster SearchNearest(Vector3 pos, float attackRange)
    {
        searchTimes = 0;
        var rt = SearchNearestInner(pos, root, 0);
        //Debug.Log("searchTimes " + searchTimes);
        return rt.m;
    }

    //只范围最近的
    private SearchResult SearchNearestInner(Vector3 pos, KDTreeNode node, int d)
    {
        searchTimes++;
        var v = node.value.kd[d];
        if (node.left == null && node.right == null)
        {
            return new SearchResult(node.value, Vector3.Distance(pos, node.value.transform.position));
        }

        SearchResult sr;
        if (pos[d] < v)
        {
            if (node.left != null)
            {
                sr = SearchNearestInner(pos, node.left, d < 2 ? d + 1 : 0);
                //如果当前最近距离半径和右边相交，则进入右边搜索
                if (pos[d] + sr.dist > v && node.right != null)
                {
                    var rt = SearchNearestInner(pos, node.right, d < 2 ? d + 1 : 0);
                    if (rt.dist < sr.dist)
                    {
                        sr = rt;
                    }
                }
            }
            else
            {
                sr = SearchNearestInner(pos, node.right, d < 2 ? d + 1 : 0);
                //如果当前最近距离半径和左边相交，则进入左边搜索
                if (pos[d] - sr.dist < v && node.left != null)
                {
                    var rt = SearchNearestInner(pos, node.left, d < 2 ? d + 1 : 0);
                    if (rt.dist < sr.dist)
                    {
                        sr = rt;
                    }
                }
            }
        }
        else
        {
            if (node.right != null)
            {
                sr = SearchNearestInner(pos, node.right, d < 2 ? d + 1 : 0);
                //如果当前最近距离半径和左边相交，则进入左边搜索
                if (pos[d] - sr.dist < v && node.left != null)
                {
                    var rt = SearchNearestInner(pos, node.left, d < 2 ? d + 1 : 0);
                    if (rt.dist < sr.dist)
                    {
                        sr = rt;
                    }
                }
            }
            else
            {
                sr = SearchNearestInner(pos, node.left, d < 2 ? d + 1 : 0);
                //如果当前最近距离半径和右边相交，则进入右边搜索
                if (pos[d] + sr.dist > v && node.right != null)
                {
                    var rt = SearchNearestInner(pos, node.right, d < 2 ? d + 1 : 0);
                    if (rt.dist < sr.dist)
                    {
                        sr = rt;
                    }
                }
            }
        }

        var dist = Vector3.Distance(pos, node.value.transform.position);
        if (dist < sr.dist)
        {
            sr.dist = dist;
            sr.m = node.value;
        }

        return sr;
    }

    public void ClearInner(KDTreeNode node)
    {
        if (node.left != null)
        {
            ClearInner(node.left);
            node.left = null;
        }
        if (node.right != null)
        {
            ClearInner(node.right);
            node.right = null;
        }
        node.value = null;
        nodePool.Recycle(node);
    }
    
    public static void Swap(Monster[] array, int i, int j)
    {
        var n = array[i];
        array[i] = array[j];
        array[j] = n;
    }
}
