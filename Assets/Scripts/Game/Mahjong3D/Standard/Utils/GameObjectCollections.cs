using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [Serializable]
    public struct GameObjectItem
    {
        public string Name;
        public GameObject GameObject;
    }

    [Serializable]
    public class GameObjectData : DataTable<GameObjectItem> { }

    public class GameObjectCollections : MonoBehaviour
    {
        [Table(typeof(GameObjectItem))]
        public GameObjectData GameObjectData;

        private Dictionary<string, GameObject> mDic = new Dictionary<string, GameObject>();

        private void Awake()
        {
            for (int i = 0; i < GameObjectData.Count; i++)
            {
                mDic.Add(GameObjectData[i].Name, GameObjectData[i].GameObject);
            }
        }

        public T Get<T>(string name) where T : Component
        {
            if (mDic.ContainsKey(name))
            {
                return mDic[name].GetComponent<T>();
            }
            return null;
        }

        public GameObject Get(string name)
        {
            if (mDic.ContainsKey(name)) { return mDic[name]; }
            return null;
        }
    }
}