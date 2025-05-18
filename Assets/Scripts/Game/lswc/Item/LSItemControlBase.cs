using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSItemControlBase : MonoBehaviour
    {
        [HideInInspector]
        public List<LSItemBase> Items;

        protected float _raidus;

        public void InitItems()
        {
            Items = new List<LSItemBase>();
            AddItems();
            SetRadius();
            InitOther();
            InitPosition();
        }

        protected virtual void InitOther()
        {

        }

        private void AddItems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                LSItemBase newItem = transform.GetChild(i).GetComponent<LSItemBase>();
                Items.Add(newItem);
            }
        }

        public virtual void SetRadius()
        {
            _raidus = LSConstant.Num_DefaultRadius;
        }

        public void InitPosition()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].InitItem(i,_raidus);
            }
        }

        public void ChangeItem(int i,int j)
        {
            LSItemBase item = Items[i];
            Items[i] = Items[j];
            Items[j] = item;        
        }

        public LSItemBase GetCheckColorItem(int index)
        {
            foreach (var item in Items)
            {
                if (index == item.index)
                {
                    return item;
                }
            }
            YxDebug.LogError("找不到对应的item,查找索引是：" + index);
            YxDebug.LogError("打印所有item "); 
            YxDebug.LogArray(Items);
            return null;
        }
    }
}
