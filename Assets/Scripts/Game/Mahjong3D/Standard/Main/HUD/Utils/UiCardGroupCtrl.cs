using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UiCardGroupCtrl : MonoBehaviour
    {
        public float Interval;
        public Vector2 Size = Vector2.zero;

        private List<UiCardGroup> mGroups = new List<UiCardGroup>();       
        private float mGroupsLength;
        private float mStartPos;    

        public void AddUiCdGroup(UiCardGroup cardGroup)
        {
            cardGroup.transform.SetParent(transform);          
            cardGroup.transform.localScale = Vector3.one;
            mGroups.Add(cardGroup);
        }

        //0 以左对齐，1 中心对齐
        public void Sort(int sortType)
        {
            mStartPos = 0;
            mGroupsLength = 0;
            for (int i = 0; i < mGroups.Count; i++)
            {
                UiCardGroup cardGroup = mGroups[i];
                mStartPos = mStartPos + cardGroup.Width / 2;
                cardGroup.transform.localPosition = new Vector3(mStartPos, 0);
                mStartPos = mStartPos + cardGroup.Width / 2;
                mStartPos += Interval;
                mGroupsLength += cardGroup.Width;
            }
            mGroupsLength += (mGroups.Count - 1) * Interval;
            float addDis = 0;
            if (sortType == 0)
            {
                addDis = -Size.x / 2;
            }
            else if (sortType == 1)
            {
                addDis = -mGroupsLength / 2;
            }
            for (int i = 0; i < mGroups.Count; i++)
            {
                UiCardGroup cardGroup = mGroups[i];
                cardGroup.transform.localPosition += new Vector3(addDis, 0);
            }
        }

        public void Clear()
        {
            foreach (UiCardGroup uiCdGroup in mGroups)
            {
                Destroy(uiCdGroup.gameObject);
            }
            mGroups.Clear();
        }

        /// <summary>
        /// 立刻清除旧的信息
        /// </summary>
        public void ClearImmediate()
        {
            foreach (UiCardGroup uiCdGroup in mGroups)
            {
                DestroyImmediate(uiCdGroup.gameObject);
            }
            mGroups.Clear();
        }
    }
}
