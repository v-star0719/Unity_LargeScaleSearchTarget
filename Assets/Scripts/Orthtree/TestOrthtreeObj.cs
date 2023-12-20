using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Temp
{
    public class TestOrthtreeObj : MonoBehaviour, IOrthtreeObj
    {
        public float[] Pos { get; private set; }
        public float[] Size { get; private set; }
        public float PickValue { get; set; }

        public void Start()
        {
            Pos = OrthtreeUtils.Vector3ToArray(transform.position);
            Size = OrthtreeUtils.Vector3ToArray(transform.lossyScale);
        }

        void Update()
        {
            OrthtreeUtils.Vector3ToArray(transform.position, Pos);
            OrthtreeUtils.Vector3ToArray(transform.lossyScale, Size);
        }
    }
}
