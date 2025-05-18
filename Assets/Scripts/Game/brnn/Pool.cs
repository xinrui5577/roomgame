using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class Pool<T> where T : MonoBehaviour
    {

        private T[] _poolArray;

        private readonly int _creatCount;

        private int _usedIndex;

        private readonly T _prefab;

        private readonly Transform _parent;

        /// <summary>
        /// 是否直接填充池中对象
        /// </summary>
        private readonly bool _fillPool;

        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        /// <param name="fillpool">是否直接填充池中对象</param>
        public Pool(T prefab, int count,bool fillpool)
            : this(prefab, count, 0, null,fillpool)
        {
        }

        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        /// <param name="creatCount">扩展个数</param>
        /// <param name="fillpool">是否直接填充池中对象</param>
        public Pool(T prefab, int count, int creatCount,bool fillpool)
            : this(prefab, count, creatCount, null, fillpool)
        {
        }


        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        /// <param name="creatCount">扩展个数</param>
        /// <param name="parent">父层级</param>
        /// <param name="fillpool">是否直接填充池中对象</param>
        public Pool(T prefab, int count, int creatCount, Transform parent,bool fillpool)
        {
            _parent = parent;
            _poolArray = new T[count];
            _prefab = prefab;
            _creatCount = creatCount;
            _usedIndex = 0;
            _fillPool = fillpool;
            FillArray(0, count);
        }



        public T GetOne()
        {
            int length = _poolArray.Length;
            if (_usedIndex >= length)
            {
                if (_creatCount > 0)
                {
                    int len1 = _creatCount + length;
                    var tempArray = new T[len1];
                    _poolArray.CopyTo(tempArray, 0);
                    _poolArray = tempArray;
                    FillArray(length, len1);
                }
                else
                {
                    _usedIndex = 0;
                }
            }
            int index = _usedIndex%length;
            var obj = _poolArray[index];
            if (obj == null)
            {
                _poolArray[index] = CreateOne();
                obj = _poolArray[index];
            }
            _usedIndex++;
            return obj;
        }



        /// <summary>
        /// 填充数组
        /// </summary>
        /// <param name="len0"></param>
        /// <param name="len1"></param>
        void FillArray(int len0, int len1)
        {
            if (!_fillPool) return;
            int count = len1 - len0;
            string name = typeof(T).Name;
            for (int i = len0; i < count; i++)
            {
                var obj = CreateOne();
                obj.name = name + i;
                _poolArray[i] = obj;
            }
        }

        /// <summary>
        /// 创建一个物体
        /// </summary>
        /// <returns></returns>
        public T CreateOne()
        {
            var obj = Object.Instantiate(_prefab);
            T comp = obj.GetComponent<T>();
            if (_parent != null)
            {
                obj.transform.parent = _parent;
            }
            return comp;
        }

        /// <summary>
        /// 隐藏所有对象
        /// </summary>
        public void HideAll()
        {
            foreach (var item in _poolArray)
            {
                item.gameObject.SetActive(false);
            }
            _usedIndex = 0;
        }

    }
}
