using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 记录上局每个位置的下注信息
    /// </summary>
    public class RecordCtrl : MonoBehaviour
    {
        /// <summary>
        /// 控制整体的显示
        /// </summary>
        public GameObject ShowParent;
        /// <summary>
        /// 每个位置的下注钱数
        /// </summary>
        public List<RecordItem> EachPosBetList;

        public UILabel TimeCurrent;

        private readonly List<List<int>> _recordsFourList=new List<List<int>>();
        private static RecordCtrl _instance;
        private int _index = 0;
        public static RecordCtrl GetInstance()
        {
            return _instance ?? (_instance = new RecordCtrl());
        }

        protected void Awake()
        {
            _instance = this;
            var nullData = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < 4; i++)
            {
                _recordsFourList.Add(nullData);
            }
        }
        void Update()
        {
            TimeCurrent.text = DateTime.Now.ToString("yyyy-MM-dd   HH:mm:ss");
        }

        public void CtrlShowPanel()
        {
            ShowParent.SetActive(!ShowParent.activeSelf);
        }
        public void AssignmentLastRound()
        {
            var last = new List<int>(App.GetGameManager<CschGameManager>().RightBottomMgr.UpBetValue);

            _recordsFourList[_index] = last;

            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < EachPosBetList.Count; i++)
                {
                    EachPosBetList[i].Labels[j].text = "￥" + YxUtiles.GetShowNumberForm(_recordsFourList[(_index - j + 4) % 4][i]);
                }
            }

            _index=(_index+1)%4;
        }
    }
}
