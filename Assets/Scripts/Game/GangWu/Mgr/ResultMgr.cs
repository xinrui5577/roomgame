using UnityEngine;
using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.GangWu.Mgr
{
    public class ResultMgr : MonoBehaviour
    {

        [SerializeField]
        private UIGrid _resultGrid = null;

        [SerializeField]
        private GameObject _resultView = null;

        [SerializeField]  
        private  GameObject[] _resultItems = null;

         private readonly List<ResultUserInfo> _resultUserInfoList = new List<ResultUserInfo>();

      
        protected void Start()
        {
            _resultView.SetActive(false);
        }

        /// <summary>
        /// 结算时,处理小结算界面信息
        /// </summary>
        /// <param name="dataArray"></param>
        public void OnGameResult(ISFSArray dataArray)
        {
            DisposalData(dataArray);
            InitBriefSum();
        }
   

        /// <summary>
        /// 信息处理(将数据进行排序)
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        private void DisposalData(ISFSArray dataArray)
        {
            //找出赢家,并整理数组,抛弃无用数据
            foreach (ISFSObject data in dataArray)
            {
                if (!data.ContainsKey("cards")) continue;
                var cards = data.GetIntArray("cards");
                if (cards == null || cards.Length == 0) continue;

                var tempUserInfo = new ResultUserInfo
                {
                    Seat = data.GetInt("seat"),
                    WinVal = data.GetInt("win"),
                    Cards = cards
                };
               
                _resultUserInfoList.Add(tempUserInfo);
            }

            //重新排列显示顺序
            //赢得玩家显示在最上面,输的玩家按从小到大排列
            _resultUserInfoList.Sort((info1, info2) =>
            {
                int val1 = info1.WinVal;
                int val2 = info2.WinVal;
                if (val1 > 0 || val2 > 0)
                {
                    return val2.CompareTo(val1);
                }

                return val1.CompareTo(val2);
            });
        }

        /// <summary>
        /// 初始化和显示小结窗口内容
        /// </summary>
        private void InitBriefSum()
        {
            if (_resultItems == null || _resultItems.Length == 0) 
                return;

            //用于记录有多少个玩家，同时作为数组索引使用 
            int count = 0;
            int length = _resultItems.Length;

            //对小结窗口玩家信息进行初始化
            foreach (var data in _resultUserInfoList)
            {
                var si = _resultItems[count++ % length].GetComponent<ResultItem>();
                
                si.InitSumItem(data);
            }

            if (_resultItems != null)
            {
                //显示玩家信息,隐藏不存在的玩家成员
                for (var i = 0; i < _resultItems.Length; i++)
                {
                    _resultItems[i].SetActive(i < count);
                }
            }
        }

        public void ShowResultView()
        {
            _resultView.SetActive(true);
            _resultGrid.hideInactive = true;
            _resultGrid.repositionNow = true;
            _resultGrid.Reposition();
        }

        /// <summary>
        /// 外挂方法
        /// </summary>
        public void HideResultView()
        {
            _resultView.SetActive(false);
            _resultUserInfoList.Clear();
        }
    }

    public class ResultUserInfo
    {
        /// <summary>
        /// 座位号
        /// </summary>
        public int Seat;

        /// <summary>
        /// 输赢
        /// </summary>
        public int WinVal;

        /// <summary>
        /// 牌数组
        /// </summary>
        public int[] Cards;

        /// <summary>
        /// 是否弃牌了
        /// </summary>
        public bool Fold;
    }
}
