using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class ObjContainerItem
    {
        public int Count;
        public GameObject[] ObjArray;
    }

    [System.Serializable]
    public class ObjContainer
    {
        /// <summary>
        /// 对象集合
        /// </summary>
        public ObjContainerItem[] ItemArray;

        public GameObject[] GetObject(int count)
        {
            ObjContainerItem item = null;
            for (int i = 0; i < ItemArray.Length; i++)
            {
                if (ItemArray[i].Count == count)
                {
                    item = ItemArray[i];
                }
            }
            if (null != item)
            {
                return item.ObjArray;
            }
            return null;
        }

        public T[] GetComponent<T>(int count) where T : Component
        {
            ObjContainerItem item = null;
            for (int i = 0; i < ItemArray.Length; i++)
            {
                if (ItemArray[i].Count == count)
                {
                    item = ItemArray[i];
                }
            }
            if (null == item) return null;
            T[] arr = new T[item.Count];
            for (int i = 0; i < item.Count; i++)
            {
                arr[i] = item.ObjArray[i].GetComponent<T>();
            }
            return arr;
        }
    }
}