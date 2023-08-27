using UnityEngine;
using Random = UnityEngine.Random;
public class Stage : MonoBehaviour
{
    public static Stage Instance { get; private set; }
    public Vector3 areaSize;//立方体空间的大小

    public float activeAreaXMax;
    public float activeAreaXMin;
    public float activeAreaYMax;
    public float activeAreaYMin;
    public float activeAreaZMax;
    public float activeAreaZMin;

    void Awake()
    {
        Instance = this;
        activeAreaXMax = areaSize.x * 0.5f;
        activeAreaXMin = -activeAreaXMax;
        activeAreaYMax = areaSize.y * 0.5f;
        activeAreaYMin = -activeAreaYMax;
        activeAreaZMax = areaSize.z * 0.5f;
        activeAreaZMin = -activeAreaZMax;
    }

    public bool IsPosInArea(Vector3 pos)
    {
        return pos.x <= activeAreaXMax && pos.x >= activeAreaXMin &&
               pos.y <= activeAreaYMax && pos.y >= activeAreaYMin &&
               pos.z <= activeAreaZMax && pos.z >= activeAreaZMin;
    }

    public void KeepCubInArea(ref Vector3 pos, float halfSize)
    {
        var t = pos.x + halfSize;
        if (t > activeAreaXMax)
        {
            pos.x += activeAreaXMax - t;
        }
        t = pos.x - halfSize;
        if (t < activeAreaXMin)
        {
            pos.x += activeAreaXMin - t;
        }

        t = pos.y + halfSize;
        if (t > activeAreaYMax)
        {
            pos.y += activeAreaYMax - t;
        }
        t = pos.y - halfSize;
        if (t < activeAreaYMin)
        {
            pos.y += activeAreaYMin - t;
        }

        t = pos.z + halfSize;
        if (t > activeAreaZMax)
        {
            pos.z += activeAreaZMax - t;
        }
        t = pos.z - halfSize;
        if (t < activeAreaZMin)
        {
            pos.z += activeAreaZMin - t;
        }
    }

    public Vector3 GetRandomPos()
    {
        return new Vector3(
            Random.Range(activeAreaXMin, activeAreaXMax),
            Random.Range(activeAreaYMin, activeAreaYMax),
            Random.Range(activeAreaZMin, activeAreaZMax));
    }

    void OnDestroy()
    {
        Instance = null;
    }
}