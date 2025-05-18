using System.Collections.Generic;
using YxFramwork.Common.Model;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahFriendScrollController : MonoBehaviour, ISuperScrollerDelegate
    {
        public float CellSize;
        public Transform Title;
        public SuperScroller Scroller;
        public SuperScrollerCellView CellViewPrefab;

        private List<YxBaseUserInfo> mDataList = new List<YxBaseUserInfo>();

        private void Awake()
        {
            Scroller.Delegate = this;
        }

        public void SetData(List<object> list)
        {
            if (null != list && list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    YxBaseUserInfo info = new YxBaseUserInfo();
                    info.Parse((Dictionary<string, object>)list[i]);
                    mDataList.Add(info);
                }
                Scroller.ReloadData();
            }
            Title.gameObject.SetActive(mDataList.Count == 0);
        }

        public SuperScrollerCellView GetCellView(SuperScroller scroller, int dataIndex, int cellIndex)
        {
            MahFriendsCellView cellView = scroller.GetCellView(CellViewPrefab) as MahFriendsCellView;
            cellView.ExCompShow().SetData(mDataList[dataIndex]);
            return cellView;
        }

        public float GetCellViewSize(SuperScroller scroller, int dataIndex)
        {
            return CellSize;
        }

        public int GetNumberOfCells(SuperScroller scroller)
        {
            if (null != mDataList)
            {
                return mDataList.Count;
            }
            return 0;
        }
    }
}