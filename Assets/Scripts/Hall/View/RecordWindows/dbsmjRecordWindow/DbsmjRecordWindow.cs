using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using System.Collections;
using YxFramwork.Controller;
using System;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsmjRecordWindow : YxTabPageWindow
    {
        [SerializeField]
        protected UIGrid _infoGrid;
        /// <summary>
        /// 无数据时显示图片
        /// </summary>
        [SerializeField]
        protected GameObject _nullTexture;
        //[SerializeField]
        //private UIPanel _showPanel;
        /// <summary>
        /// 记录Item
        /// </summary>
        [SerializeField]
        protected GameObject Item;

        private string gk;
        public UIScrollView ScrollView;

        private int _curPageNum = 1;   
        private int itemIndex = 0;
        
        protected override void OnAwake()
        {
            CreateTabels();
            if (ScrollView != null)
            {
                ScrollView.onStoppedMoving = OnDragFinished;
            }
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            
            if (constraint.y <= 1f && itemIndex < 30)
            {
                var dic = new Dictionary<string, object>();
                dic["game_key_c"] = gk;
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManger>().SendAction("gameHistoryReplay", dic, OnGetRecordInfo);
            }
        }

        //-----------------------------------------------------------------------

        private bool CreateTabels()
        {
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            foreach (var keyValue in dict)
            {
                var gmodel = keyValue.Value;
                var rk = gmodel.RoomKind;
                if (rk < 1) continue;
                gamelist.Add(gmodel);
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var gd = gamelist[i];
                var gk = gd.GameKey;
                var tdata = new TabData
                {
                    Name = gd.GameName,
                    Data = gk
                };
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return false;

            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView(-1);
            return true;
        }

        public override void OnTableClick(YxTabItem tableView)
        {
            ResetPosition();
            if (!tableView.GetToggle().value) return;
            var tdata = tableView.GetData<TabData>();
            if (tdata == null) return;
            gk = tdata.Data.ToString();
            YxWindowManager.ShowWaitFor();
            var apiInfo = new Dictionary<string, object>() { { "game_key_c", gk }, { "p", 1 } };
            Facade.Instance<TwManger>().SendAction("gameHistoryReplay", apiInfo, OnGetRecordInfo); 
                      
#if DBSMJ
            //暂时修改
            Transform nldld = _tabGrid.transform.FindChild("1");
            nldld.gameObject.SetActive(false);
#endif
        }

        public void OnGetRecordInfo(object data)
        {
            if (data == null)
            {
                return;
            }

            var dataDic = (List<object>)data;
            YxWindowManager.HideWaitFor();
            List<DbsmjRecordData> records = new List<DbsmjRecordData>();
           
            foreach (var item in dataDic)
            {
                DbsmjRecordData rd = new DbsmjRecordData();
                rd.Parse(itemIndex, (IDictionary)item, gk);
                records.Add(rd);
                itemIndex++;
            }

            InitForDbsmj(records);
            SetOrder((int)YxWindowManager.YxWinLayer.WindowLayer);
        }

        public void ResetPosition()
        {
            while (_infoGrid.transform.childCount > 0)
            {
                DestroyImmediate(_infoGrid.GetChild(0).gameObject);
            }

            SpringPanel.Begin(ScrollView.gameObject, Vector3.zero, 10000);
            _curPageNum = 1;           
            itemIndex = 0;
        }

        public void InitForDbsmj(List<DbsmjRecordData> datas)
        {
            DbsmjRecordItem item;
            foreach (var data in datas)
            {
                item = NGUITools.AddChild(_infoGrid.gameObject, Item).GetComponent<DbsmjRecordItem>();
                item.gameObject.SetActive(true);
                item.Init(data);
            }
            if (_nullTexture != null) _nullTexture.SetActive(datas.Count <= 0);
            _infoGrid.repositionNow = true;
        }
    }
}