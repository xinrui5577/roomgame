using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [Serializable]
    public class ItemPositionData
    {
        public int Amount;
        public Vector3[] Position;

        public Vector3 GetPosition(int number)
        {
            if (Position.Length > number)
            {
                return Position[number];
            }
            return Vector3.zero;
        }
    }

    public class HeadItemsCtrl : MonoBehaviour
    {
        public ItemPositionData[] Datas;
        private ItemPositionData mData;

        public void Play(List<HeadTextItem> list)
        {
            HeadTextItem obj;
            //获取数据
            if (null == mData)
            {
                for (int i = 0; i < Datas.Length; i++)
                {
                    if (Datas[i].Amount == list.Count)
                    {
                        mData = Datas[i];
                    }
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                obj = list[i];
                var item = MahjongUtility.GetYxGameData().GetPlayerInfoItem<PlayerInfoItem>(i);
                obj.ExSetParent(item.transform);
                obj.transform.SetAsFirstSibling();
                obj.GetComponent<RectTransform>().anchoredPosition3D = mData.GetPosition(i);
                var rect = item.GetComponent<RectTransform>();
                var v3 = rect.anchoredPosition3D;
            }
        }

        public void Reset(List<HeadTextItem> list)
        {
            if (null == list) return;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ExSetParent(transform);
                list[i].gameObject.SetActive(false);
            }
        }
    }
}