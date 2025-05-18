using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIItemsManager : MonoBehaviour
    {
        public List<GameObject> Store = new List<GameObject>();

        public GameObject Prefab { get; private set; }

        public Transform Parent { get; set; }

        /// <summary>
        /// 隐藏仓库时， 隐藏所有子对象
        /// </summary>
        public bool CloseAndHideItems { get; set; }

        private void Awake()
        {
            Prefab = Store[0];
        }

        private void OnDisable()
        {
            if (!CloseAndHideItems)
            {
                HideItems();
            }          
        }

        public void HideItems()
        {
            for (int i = 0; i < Store.Count; i++)
            {
                Store[i].SetActive(false);
            }
        }

        public T GetItem<T>(int index) where T : Object
        {
            GameObject go = null;
            if (index < Store.Count)
            {
                go = Store[index];
            }
            else
            {
                go = CreateItem();
            }
            if (go != null)
            {
                go.SetActive(true);
                return go.GetComponent<T>();
            }
            return null;
        }

        private GameObject CreateItem()
        {
            var obj = Instantiate(Prefab);

            if (Parent == null) Parent = transform;
            obj.transform.ExSetParent(Parent);
            Store.Add(obj);
            return obj;
        }
    }
}