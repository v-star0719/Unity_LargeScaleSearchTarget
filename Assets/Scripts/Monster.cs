using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public float moveCubeAreaSize;
    public float speed = 1f;

    private float halfMoveCubeAreaSize;
    private Vector3 targetPos;
    private Vector3 startPos;
    private float totalTime;
    private float timer;

    public float dyTime;
    public float[] kd = new float[3];
    public MonsterCellData CellData { get; private set; }

    public void StartRunning()
    {
        if (CellData == null)
        {
            CellData = new MonsterCellData()
            {
                monster = this
            };
        }
        dyTime = 0;
        halfMoveCubeAreaSize = moveCubeAreaSize * 0.5f;
        MoveToPos();
    }

    private Vector3 GetMovePos()
    {
        var pos = transform.position;
        Stage.Instance.KeepCubInArea(ref pos, halfMoveCubeAreaSize);
        pos.x += Random.Range(0, moveCubeAreaSize) - halfMoveCubeAreaSize;
        pos.y += Random.Range(0, moveCubeAreaSize) - halfMoveCubeAreaSize;
        pos.z += Random.Range(0, moveCubeAreaSize) - halfMoveCubeAreaSize;
        return pos;
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= totalTime)
        {
            transform.position = targetPos;
            MoveToPos();
        }
        else
        {
            var r = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, r, 10);
            transform.position = Vector3.Lerp(startPos, targetPos, timer / totalTime);
        }
        CellData.CheckCellChange();
    }

    private void MoveToPos()
    {
        startPos = transform.position;
        targetPos = GetMovePos();
        timer = 0;
        totalTime = Vector3.Distance(startPos, targetPos) / speed;
    }

    public void Hit()
    {
        Dy();
    }

    public void Dy()
    {
        CellData.Leave();
        MonsterManager.Instance.OnMonsterDead(this);
    }

    public void InitKd()
    {
        var pos = transform.position;
        kd[0] = pos.x;
        kd[1] = pos.y;
        kd[2] = pos.z;
    }

    public override string ToString()
    {
        return gameObject.name + " " + transform.position.ToString();
    }
}
