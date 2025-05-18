using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.brtbsone
{
    public class Pool<T> where T : MonoBehaviour
    {
        private List<T> _poolList;
        private T _prefab;
        private Transform _parent;
        private int _maxNum;
        private int _index;

        public Pool(int count, T prefab, Transform parent)
        {
            _maxNum = count;
            _poolList = new List<T>();
            _prefab = prefab;
            _parent = parent;
            _index = 0;
        }

        public T GetOne()
        {
            if (_poolList.Count < _maxNum)
            {
                FillList();
            }
            var obj = _poolList[_index++ % _maxNum];
            return obj;
        }

        private void FillList()
        {
            for (int i = 0; i < _maxNum; i++)
            {
                var obj = CreateOne();
                _poolList.Add(obj);
                obj.name = i + "";
                obj.gameObject.SetActive(false);
            }
        }

        private T CreateOne()
        {
            var obj = Object.Instantiate(_prefab);
            T t = obj.GetComponent<T>();
            if (_parent != null)
            {
                obj.transform.parent = _parent;
            }
            return t;
        }

        public void HideAllPrefab()
        {
            foreach (var one in _poolList)
            {
                one.gameObject.SetActive(false);
            }
            _index = 0;
        }
    }
}