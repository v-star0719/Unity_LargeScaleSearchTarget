using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SearchType
{
    Normal,
    KDTreeRange,
    KDTreeNearest,
    Grid,
}

public class SearchCenter : MonoBehaviour
{
    public static SearchCenter Instance { get; private set; }

    public int searchTimes;
    public long costTime;
    public SearchType searchType;
    private Queue<Hero> requests = new Queue<Hero>();
    private KDTree kdTree;

    public void Awake()
    {
        Instance = this;
        kdTree = new KDTree();
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
            kdTree.Build(MonsterManager.Instance.Monsters);
        }

        while (requests.Count > 0)
        {
            var hero = requests.Dequeue();
            if (!hero.isDestroyed)
            {
                SearchMultiTimes(hero);
            }
        }

        costTime = (System.DateTime.Now.Ticks - ticks) / 10;

        if (searchType == SearchType.KDTreeRange || searchType == SearchType.KDTreeNearest)
        {
            kdTree.Clear();
        }
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
        var gridPos = Grid.Instance.WorldPosToGridPos(heroPos);
        var attackRangeGridCount = Mathf.CeilToInt(hero.AttackRange / Grid.Instance.cellSize);
        var ar = hero.AttackRange * hero.AttackRange;
        var arSqrInt = attackRangeGridCount * attackRangeGridCount;
        Monster target = null;
        float dist = Single.MaxValue;
        int x, y, z;
        var cells = Grid.Instance.cells;

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
        var cell = Grid.Instance.GetCell(x, y, z);
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

    public void NextSearchType()
    {
        searchType++;
        if (searchType > SearchType.Grid)
        {
            searchType = SearchType.Normal;
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
