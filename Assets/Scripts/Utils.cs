using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Utils
{
    public static float SqrDistance(Vector3 v1, Vector3 v2)
    {
        var x = v1.x - v2.x;
        var y = v1.y - v2.y;
        var z = v1.z - v2.z;
        return x * x + y * y + z * z;
    }
}
