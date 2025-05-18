using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class ObjectCachePool : MonoBehaviour
    {
        public static ObjectCachePool Singleton
        {
            get
            {
                if (_Singleton == null)
                {
                    GameObject obj = new GameObject("ObjectCachePool");
                    obj.AddComponent<ObjectCachePool>();
                }

                return _Singleton;
            }
        }

        private static ObjectCachePool _Singleton;

        private Dictionary<string, Stack<GameObject>> _cachePool = new Dictionary<string, Stack<GameObject>>();

        void Awake() { _Singleton = this; }

        public GameObject Pop(string name)
        {
            GameObject temp;

            if (!string.IsNullOrEmpty(name) && name.Contains("(Clone)"))
            {
                if (_cachePool.ContainsKey(name) && _cachePool[name].Count > 0)
                {
                    temp = _cachePool[name].Pop();
                    temp.SetActive(true);
                    return temp;
                }
            }

            return null;
        }

        public void Push(string name, GameObject obj)
        {
            if (string.IsNullOrEmpty(name) || null == obj) return;

            if (_cachePool.ContainsKey(name))
                _cachePool[name].Push(obj);
            else
            {
                _cachePool[name] = new Stack<GameObject>();
                _cachePool[name].Push(obj);
            }

            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
        }

    }
}