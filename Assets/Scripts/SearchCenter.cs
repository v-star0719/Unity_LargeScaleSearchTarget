using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Temp;
using UnityEngine;

public enum SearchType
{
    Normal,
    KDTreeRange,
    KDTreeNearest,
    Grid,
    Octree,
}

public class SearchCenter : MonoBehaviour
{
    public static SearchCenter Instance { get; private set; }

    public int searchTimes;
    public long searchCostTime;
    public long buildCostTime;
    public SearchType searchType;
    public bool searchCostTimeChanged;

    private Queue<Hero> requests = new Queue<Hero>();
    private KDTree kdTree;
    private Orthtree orthtree;
    private List<IOrthtreeObj> orthtreeObjs = new List<IOrthtreeObj>();

    public void Awake()
    {
        Instance = this;
        kdTree = new KDTree();
        orthtree = new Octree(10, new float[] { 1, 1, 1 });
    }

    public void Update()
    {
        if (requests.Count < HeroManager.Instance.Heros.Count)
        {
            return;
        }

        var ticks = System.DateTime.Now.Ticks;
        if (searchType == SearchType.KDTreeRange || searchType == SearchType.KDTreeNearest)
        {
            kdTree.Clear();
            kdTree.Build(MonsterManager.Instance.Monsters);
        }
        if (searchType == SearchType.Octree)
        {
            orthtree.Clear();
            orthtreeObjs.Clear();
            foreach (var monster in MonsterManager.Instance.Monsters)
            {
                orthtreeObjs.Add(monster);
            }
            orthtree.Build(transform.position, Stage.Instance.areaSize, orthtreeObjs);
        }
        buildCostTime = (DateTime.Now.Ticks - ticks) / 10;

        while (requests.Count > 0)
        {
            var hero = requests.Dequeue();
            if (!hero.isDestroyed)
            {
                SearchMultiTimes(hero);
            }
        }

        searchCostTime = (System.DateTime.Now.Ticks - ticks) / 10;
        searchCostTimeChanged = true;
    }

    private void SearchMultiTimes(Hero hero)
    {
        for (int i = 0; i < searchTimes; i++)
        {
            switch (searchType)
            {
                case SearchType.Normal:
                    hero.SetTarget(SearchNormal(hero));
                    break;
                case SearchType.KDTreeRange:
                    hero.SetTarget(SearchKDTreeNearestInRange(hero));
                    break;
                case SearchType.KDTreeNearest:
                    hero.SetTarget(SearchKDTreeNearest(hero));
                    break;
                case SearchType.Grid:
                    hero.SetTarget(SearchGrid(hero));
                    break;
                case SearchType.Octree:
                    hero.SetTarget(SearchOctree(hero));
                    break;
            }
        }
    }

    public void Request(Hero hero)
    {
        requests.Enqueue(hero);
    }

    private Monster SearchNormal(Hero hero)
    {
        var monsters = MonsterManager.Instance.Monsters;
        Monster target = null;
        float d = Single.MaxValue;
        Vector3 pos = hero.transform.position;
        var ar = hero.AttackRange * hero.AttackRange;
        foreach (var m in monsters)
        {
            var dt = Utils.SqrDistance(pos, m.transform.position);
            if (dt < ar && dt < d)
            {
                d = dt;
                target = m;
            }
        }

        return target;
    }

    private Monster SearchKDTreeNearest(Hero hero)
    {
        return kdTree.SearchNearest(hero.transform.position, hero.AttackRange);
    }

    private Monster SearchKDTreeNearestInRange(Hero hero)
    {
        return kdTree.SearchNearestInRange(hero.transform.position, hero.AttackRange);
    }

    private Monster SearchGrid(Hero hero)
    {
        var heroPos = hero.transform.position;
        var gridPos = GridX.Instance.WorldPosToGridPos(heroPos);
        var attackRangeGridCount = Mathf.CeilToInt(hero.AttackRange / GridX.Instance.cellSize);
        var ar = hero.AttackRange * hero.AttackRange;
        var arSqrInt = attackRangeGridCount * attackRangeGridCount;
        Monster target = null;
        float dist = Single.MaxValue;
        int x, y, z;
        var cells = GridX.Instance.cells;

        SearchGridHelper(gridPos.x, gridPos.y, gridPos.z, heroPos, ar, ref dist, ref target);
        
        //从内向外一圈一圈搜索
        for (int m = 1; m < attackRangeGridCount; m++)
        {
            //一圈的格子构成一个立方体。每次遍历相对的两个面
            //yz方向的面，x值是固定的
            for (y = -m; y <= m; y++)
            {
                for (z = -m; z <= m; z++)
                {
                    SearchGridHelper(-m + gridPos.x, y + gridPos.y, z + gridPos.z, heroPos, ar, ref dist, ref target);
                    SearchGridHelper( m + gridPos.x, y + gridPos.y, z + gridPos.z, heroPos, ar, ref dist, ref target);
                }
            }

            //xy方向的面，z值是固定的。跳过上面已经遍历的格子
            for (x = -m + 1; x < m ; x++)
            {
                for (y = -m; y <= m; y++)
                {
                    SearchGridHelper(x + gridPos.x, y + gridPos.y, m + gridPos.z, heroPos, ar, ref dist, ref target);
                    SearchGridHelper(x + gridPos.x, y + gridPos.y,-m + gridPos.z, heroPos, ar, ref dist, ref target);
                }
            }

            //xz方向的面，y值是固定的。跳过上面已经遍历的格子
            for (x = -m + 1; x < m ; x++)
            {
                for (z = -m + 1; z < m; z++)
                {
                    SearchGridHelper(x + gridPos.x, m + gridPos.y, z + gridPos.z, heroPos, ar, ref dist, ref target);
                    SearchGridHelper(x + gridPos.x,-m + gridPos.y, z + gridPos.z, heroPos, ar, ref dist, ref target);
                }
            }
            if (target != null)
            {
                break;
            }
        }
        return target;
    }

    private void SearchGridHelper(int x, int y, int z, Vector3 heroPos, float attackRangeSqr, ref float minDict, ref Monster target)
    {
        var cell = GridX.Instance.GetCell(x, y, z);
        if (cell == null)
        {
            return;
        }

        if (Utils.SqrDistance(cell.pos, heroPos) > attackRangeSqr)
        {
            return;
        }

        foreach (var monster in cell.monsters)
        {
            var d = Utils.SqrDistance(monster.transform.position, heroPos);
            if (d < minDict)
            {
                minDict = d;
                target = monster;
            }
        }
    }

    private Monster SearchOctree(Hero hero)
    {
        return orthtree.SearchNearest(hero.transform.position, hero.AttackRange) as Monster;
    }

    public void NextSearchType()
    {
        searchType++;
        if (searchType > SearchType.Octree)
        {
            searchType = SearchType.Normal;
        }
    }

    void OnDrawGizmos()
    {
        if (searchType == SearchType.KDTreeNearest || searchType == SearchType.KDTreeRange)
        {
            if (kdTree != null)
            {
                kdTree.DrawGizmos();
            }
        }

        if (searchType == SearchType.Octree)
        {
            orthtree.DrawGizmos();
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
