using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Core;

namespace Kernel.Core
{
    public class Pool<T> where T : class
    {
        public List<T> cacheList = new List<T>();
        private Func<T> createFunc;

        public Pool(Func<T> create)
        {
            createFunc = create;
        }

        public T Get()
        {
            T rt;
            if (cacheList.Count > 0)
            {
                var i = cacheList.Count - 1;
                rt = cacheList[i];
                cacheList.RemoveAt(i);
                return rt;
            }

            rt = createFunc();
            return rt;
        }

        public void Recycle(T t)
        {
            var ipool = t as IPoolItem;
            ipool?.OnRecycle();
            cacheList.Add(t);
        }
    }
}
