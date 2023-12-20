using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Assets.Temp
{
    public class TestOrthtree : MonoBehaviour
    {
        public int objCount;
        public bool testBtn;
        public Vector3 size;
        private List<IOrthtreeObj> objs;
        private Orthtree tree;

        public Transform huner;
        public float huntRadius;
        private List<IOrthtreeObj> targets;

        private void Start()
        {
            objs = new List<IOrthtreeObj>();

            var objPrefab = transform.GetChild(0);
            for(int i = 0; i < objCount; i++)
            {
                var obj = Instantiate(objPrefab, transform);
                obj.localPosition = new Vector3(
                    UnityEngine.Random.Range(0, size.x) - size.x * 0.5f,
                    UnityEngine.Random.Range(0, size.y) - size.y * 0.5f,
                    UnityEngine.Random.Range(0, size.z) - size.z * 0.5f
                    );
                objs.Add(obj.GetComponent<TestOrthtreeObj>());
            }
            objPrefab.gameObject.SetActive(false);

            //foreach (var child in GetComponentsInChildren<TestOrthtreeObj>())
            //{
            //    objs.Add(child);
            //}

            tree = new Octree(10, new float[]{1, 1, 1});
        }

        public void Update()
        {
            if (testBtn)
            {
                testBtn = false;
                Test();
            }
        }

        private void Test()
        {
            tree.Clear();
            tree.Build(OrthtreeUtils.Vector3ToArray(transform.position), OrthtreeUtils.Vector3ToArray(size), objs.ToList());
            targets = tree.Search(huner.transform.position, huntRadius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(huner.position, huntRadius);
            Gizmos.DrawWireCube(transform.position, size);
            tree?.DrawGizmos();

            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var t = target as TestOrthtreeObj;
                    Gizmos.DrawLine(t.transform.position, huner.position);
                }
            }
        }
    }
}
