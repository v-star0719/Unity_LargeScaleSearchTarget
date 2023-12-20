using System;
using UnityEngine;

namespace Assets.Temp
{
    public class OrthtreeUtils
    {
        public static void SpacePartion(Vector3 pos, Vector3 size, Action<Vector3, Vector3> onPartion)
        {
            Vector3 sz = size * 0.5f;
            onPartion(new Vector3(pos.x - sz.x, pos.y, pos.z - sz.z), sz);
            onPartion(new Vector3(pos.x + sz.x, pos.y, pos.z - sz.z), sz);
            //
            onPartion(new Vector3(pos.x, pos.y + sz.y, pos.z - sz.z), sz);
            onPartion(new Vector3(pos.x, pos.y - sz.y, pos.z - sz.z), sz);
            ////
            onPartion(new Vector3(pos.x - sz.x, pos.y, pos.z + sz.z), sz);
            onPartion(new Vector3(pos.x + sz.x, pos.y, pos.z + sz.z), sz);
            //
            onPartion(new Vector3(pos.x, pos.y + sz.y, pos.z + sz.z), sz);
            onPartion(new Vector3(pos.x, pos.y - sz.y, pos.z + sz.z), sz);
        }


        public static float[] Multiply(float[] ar, float m)
        {
            var rt = new float[ar.Length];
            for (int i = 0; i < ar.Length; i++)
            {
                rt[i] = ar[i] * m;
            }
            return rt;
        }

        public static void CoptyArray(float[] source, float[] dest)
        {
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = source[i];
            }
        }

        public static float[] Vector3ToArray(Vector3 v)
        {
            return new []{v.x, v.y, v.z};
        }

        public static void Vector3ToArray(Vector3 v, float[] array)
        {
            array[0] = v.x;
            array[1] = v.y;
            array[2] = v.z;
        }

        public static float SqrDistance(float[] pos1, float[] pos2, int dimension)
        {
            float rt = 0;
            for(int i = 0; i < dimension; i++)
            {
                var f = pos1[i] - pos2[i];
                rt += f * f;
            }
            return rt;
        }
    }
}