using UnityEngine;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class Pool<T> where T : MonoBehaviour
    {

        private T[] _poolArray;

        private readonly int _creatCount;

        private int _usedIndex;

        private readonly T _prefab;

        private readonly Transform _parent;

        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        public Pool(T prefab, int count)
            : this(prefab, count, 0, null)
        {

        }

        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        /// <param name="creatCount">扩展个数</param>
        public Pool(T prefab, int count, int creatCount)
            : this(prefab, count, creatCount, null)
        {


        }

        /// <summary>
        /// 初始化池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="count">初始化预制体个数</param>
        /// <param name="creatCount">扩展个数</param>
        /// <param name="parent">父层级</param>
        public Pool(T prefab, int count, int creatCount, Transform parent)
        {
            _parent = parent;
            _poolArray = new T[count];
            _prefab = prefab;
            _creatCount = creatCount;
            _usedIndex = 0;
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
            var obj = _poolArray[_usedIndex++ % length];
            if (obj == null) obj = CreatOne();
            return obj;
        }



        /// <summary>
        /// 填充数组
        /// </summary>
        /// <param name="len0"></param>
        /// <param name="len1"></param>
        void FillArray(int len0, int len1)
        {
            int count = len1 - len0;
            string name = typeof(T).Name;
            for (int i = len0; i < count; i++)
            {
                var obj = CreatOne();
                obj.name = name + i;
                _poolArray[i] = obj;
            }
        }

        /// <summary>
        /// 创建一个物体
        /// </summary>
        /// <returns></returns>
        public T CreatOne()
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
