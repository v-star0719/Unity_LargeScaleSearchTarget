using System.Collections.Generic;
using UnityEngine;

//场景立方体盒子上，x，y，z都是负的那个角作为0号格子。这样其他格子减去他的坐标都是正的。
public class GridX : MonoBehaviour
{
    public static GridX Instance;
    public GridCell[,,] cells;
    public int cellSize;//格子大小 
    public GameObject markPrefab;
    private int countX;
    private int countY;
    private int countZ;
    private int countXY;

    private int lastCellSize;
    private List<GameObject> markList = new List<GameObject>();

    private Vector3 bottomPos;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        if (lastCellSize != cellSize)
        {
            lastCellSize = cellSize;
            ResetGrids();
        }
    }

    private void ResetGrids()
    {
        foreach (var monster in MonsterManager.Instance.Monsters)
        {
            monster.CellData.Clear();
        }

        var areaSize = Stage.Instance.areaSize;
        countX = Mathf.Max(1, Mathf.CeilToInt(areaSize.x / cellSize));
        countY = Mathf.Max(1, Mathf.CeilToInt(areaSize.y / cellSize));
        countZ = Mathf.Max(1, Mathf.CeilToInt(areaSize.z / cellSize));
        cells = new GridCell[countX,countY,countZ];

        countXY = countX * countY;
        bottomPos.x = -countX * cellSize * 0.5f;
        bottomPos.y = -countY * cellSize * 0.5f;
        bottomPos.z = -countZ * cellSize * 0.5f;

        var index = 0;
        for (int i = 0; i < countZ; i++)
        {
            for (int j = 0; j < countY; j++)
            {
                for (int k = 0; k < countX; k++)
                {
                    var cell = new GridCell();
                    cell.gridPos = new Vector3Int(k, j, i);
                    cell.pos = new Vector3(
                        (k + 0.5f) * cellSize + bottomPos.x,
                        (j + 0.5f) * cellSize + bottomPos.y,
                        (i + 0.5f) * cellSize + bottomPos.z
                        );
                    //Debug.Log($"{cell.index} {cell.pos} ");
                    //Debug.Log($"{GetCellIndex(cell.pos)} {GetCellPos(cell.index)}");
                    cells[k, j, i] = cell;

                    GameObject mark;
                    if (index < markList.Count)
                    {
                        mark = markList[index];
                    }
                    else
                    {
                        mark = Instantiate(markPrefab);
                        mark.transform.parent = transform;
                        markList.Add(mark);
                    }
                    mark.transform.position = cell.pos;

                    index++;
                }
            }
        }

        for (var i = markList.Count - 1; i >= index; i--)
        {
            Destroy(markList[i]);
            markList.RemoveAt(i);
        }
    }

    //void OnPostRender()
    //{
    //    GL.LoadProjectionMatrix(transform.localToWorldMatrix);
    //    GL.Begin(GL.LINES);
    //    GL.Vertex3(1, 0, 1);
    //    GL.Vertex3(1, 0, -1);
    //    GL.Vertex3(-2, 0, 1);
    //    GL.Vertex3(-2, 0, -1);
    //    GL.End();
    //}

    public GridCell GetCell(Vector3 pos)
    {
        var gridPos = WorldPosToGridPos(pos);
        return GetCell(gridPos.x, gridPos.y, gridPos.z);
        
        //var index = GetCellIndex(pos);
        //if (index >= 0 && index < cells.Count)
        //{
        //    return cells[index];
        //}
        //return null;
    }

    public GridCell GetCell(int x, int y, int z)
    {
        if (0 <= x && x < countX &&
            0 <= y && y < countY &&
            0 <= z && z < countZ)
        {
            return cells[x, y, z];
        }

        return null;
    }

    public Vector3Int WorldPosToGridPos(Vector3 pos)
    {
        return new Vector3Int(
            (int)((pos.x - bottomPos.x) / cellSize),
            (int)((pos.y - bottomPos.y) / cellSize),
            (int)((pos.z - bottomPos.z) / cellSize)
            );
    }

    public int GetCellIndex(Vector3 pos)
    {
        int x = (int)((pos.x - bottomPos.x) / cellSize);
        int y = (int)((pos.y - bottomPos.y) / cellSize);
        int z = (int)((pos.z - bottomPos.z) / cellSize) ;
        return x + y * countX + z * countXY;
    }

    public Vector3 GetCellPos(int index)
    {
        int z = countXY == 1 ? 0 : index / (countXY);
        index = index - (countXY) * z;

        int y = countY == 1 ? 0 : index / countX;
        int x = index - y * countX;
        return new Vector3(
            (x + 0.5f) * cellSize + bottomPos.x,
            (y + 0.5f) * cellSize + bottomPos.y,
            (z + 0.5f) * cellSize + bottomPos.z
            );
    }

    void OnDrawGizmos()
    {
        if (SearchCenter.Instance == null || SearchCenter.Instance.searchType != SearchType.Grid)
        {
            return;
        }

        var cellCube = new Vector3(cellSize, cellSize, cellSize);
        foreach (var cell in cells)
        {
            Gizmos.DrawWireCube(cell.pos, cellCube);
            foreach (var monster in cell.monsters)
            {
                Gizmos.DrawLine(cell.pos, monster.transform.position);
            }
        }
    }
}

public class GridCell
{
    public Vector3 pos;
    public Vector3Int gridPos;
    public List<Monster> monsters = new List<Monster>(30);

    public void RemoveMonster(MonsterCellData data)
    {
        if (data.monsterListIndex < 0)
        {
            return;
        }

        var last = monsters.Count - 1;
        if (last < 0)
        {
            Debug.LogError("no monster in grid cell: " + gridPos);
            return;
        }

        if (data.monsterListIndex != last)
        {
            var m = monsters[last];
            m.CellData.monsterListIndex = data.monsterListIndex;
            monsters[data.monsterListIndex] = monsters[last];
        }
        monsters.RemoveAt(last);
        data.cell = null;
        data.monsterListIndex = -1;
    }

    public void AddMonster(MonsterCellData data)
    {
        monsters.Add(data.monster);
        data.monsterListIndex = monsters.Count - 1;
        data.cell = this;
    }
}

public class MonsterCellData
{
    public int monsterListIndex = -1; //在单元格的怪物列表中的索引
    public GridCell cell;
    public Monster monster;

    public void CheckCellChange()
    {
        var pos = monster.transform.position;
        var newCell = GridX.Instance.GetCell(pos);
        if (cell != newCell)
        {
            if (cell != null)
            {
                cell.RemoveMonster(this);
            }

            if (newCell != null)
            {
                newCell.AddMonster(this);
            }

            cell = newCell;
        }
    }

    public void Leave()
    {
        cell?.RemoveMonster(this);
    }

    public void Clear()
    {
        monsterListIndex = -1;
        cell = null;
    }
}