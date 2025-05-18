using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    /// <summary>
    /// 记录界面
    /// </summary>
    public class RecordWindow : YxTabPageWindow
    {
        /// <summary>
        ///  信息的Grid
        /// </summary>
        [SerializeField]
        UIGrid _infoGrid;
        /// <summary>
        /// 无数据时显示图片
        /// </summary>
        [SerializeField]
        private GameObject _nullTexture;
        /// <summary>
        /// 显示panel
        /// </summary>
        [SerializeField]
        private UIPanel _showPanel;
        /// <summary>
        /// 记录Item
        /// </summary>
        [SerializeField]
        private GameObject Item;
        protected override void OnAwake()
        {
            CreateTabels();
        }
        private void CreateTabels()
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
            if (TabDatas.Length < 1) return;
            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView(-1);
        }

        public override void OnTableClick(YxTabItem tableView)
        {
            if (!tableView.GetToggle().value) return;
            var tdata = tableView.GetData<TabData>();
            if (tdata == null) return;
            var gk = tdata.Data.ToString();
            var apiInfo = new Dictionary<string, object>() { { "game_key_c", gk }, { "p", 1 } };
            Facade.Instance<TwManager>().SendAction("gameHistoryReplay", apiInfo, OnGetRecordInfo);
        }
        /// <summary>
        /// 初始化回放数据
        /// </summary>
        /// <param name="datas"></param>
        public void Init(List<RecordData> datas)
        {
            while (_infoGrid.transform.childCount>0)
            {
                DestroyImmediate(_infoGrid.GetChild(0).gameObject);
            }
            ResetPosition();
            RecordItem item;
            foreach (var data in datas)
            {
                item = NGUITools.AddChild(_infoGrid.gameObject, Item).GetComponent<RecordItem>();
                item.gameObject.SetActive(true);
                item.Init(data);
            }
            if (_nullTexture!=null) _nullTexture.SetActive(datas.Count <= 0);
            _infoGrid.repositionNow = true;
        }
        private void ResetPosition()
        {
            SpringPanel.Begin(_showPanel.gameObject, Vector3.zero, 10000);
        }

        private void OnGetRecordInfo(object data)
        {
            if (data == null)
            {
                return;
            }
            var dataDic = (List<object>)data;
            List<RecordData> records = new List<RecordData>();
            int i = 0;
            foreach (var item in dataDic)
            {
                RecordData rd = new RecordData();
                rd.Parse(i, (IDictionary)item);
                records.Add(rd);
                i++;
            }
            Init(records);
            SetOrder((int)YxWindowManager.YxWinLayer.WindowLayer);
        }
    }
}
