/** 
 *文件名称:     RecordDetailWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-03-23 
 *描述:         战绩详情界面
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class RecordDetailWindow : YxPageListWindow
    {
        #region UI Param
        #endregion 
        #region Data Param
        /// <summary>
        ///key 游戏key
        /// </summary>
        private string _keyGameKey = "game_key_c";
        /// <summary>
        /// key真实房间号
        /// </summary>
        private string _keyRoomId = "room_id";
        #endregion 
        #region Local Data
        /// <summary>
        ///  当前游戏gamekey
        /// </summary>
        private string _curGamekey;
        /// <summary>
        ///  当前记录roomId
        /// </summary>
        private string _curRoomId;
        /// <summary>
        /// 临时数据
        /// </summary>
        private RecordDetailData _curData;
        #endregion
        #region Life Cycle

        public string CurGameKey
        {
            get { return _curGamekey; }
        }

        protected override Type GetItemType()
        {
            return typeof(RecordDetialItemData);
        }

        #endregion
        #region Function

        public void SendFirstAction(string gamekey, string roomId, bool playBack)
        {
            _curRoomId = roomId;
            _curGamekey = gamekey;
            RecordDetailItem.PlayBack = playBack;
            Show();
            FirstRequest();
        }

        protected override void SetActionDic()
        {
            base.SetActionDic();
            ActionParam[_keyGameKey] = _curGamekey;
            ActionParam[_keyRoomId] = _curRoomId;
        }

        protected override void OnActionCallBackDic()
        {
            _curData = new RecordDetailData(Data, GetItemType());
            RecordDetailItem.Host = _curData.WebHost;
            DealPageData(_curData);
        }

        /// <summary>
        /// 使用内嵌网页打开回放
        /// </summary>
        /// <param name="openWindowName"></param>
        /// <param name="url"></param>
        public void ShowUrlByWebView(string openWindowName, string url)
        {
            MainYxView.OpenWindowWithData(openWindowName, url);
        }

        public void OpenUrl(string url)
        {
            YxDebug.Log(string.Format("Url is: {0}", url));
            Application.OpenURL(url);
        }
        #endregion

    }

    public class RecordDetailData : PageRequestData
    {
        /// <summary>
        /// key主体数据(detail：临时数据)
        /// </summary>
        private string _keyDetail = "detail";
        /// <summary>
        /// Key 房间号
        /// </summary>
        private string _keyRoomId = "room_id";
        /// <summary>
        /// Key host
        /// </summary>
        private string _keyWebHost = "web_host";
        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId;
        /// <summary>
        /// web host
        /// </summary>
        private string _webHost;

        public string RealRoomId
        {
            get { return _roomId; }
        }

        public string ShowRoomId
        {
            get { return _roomId.Substring(_roomId.Length - 6, 6); }
        }

        public string WebHost
        {
            get { return _webHost; }
        }

        public RecordDetailData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            dic.TryGetValueWitheKey(out _roomId, _keyRoomId);
            dic.TryGetValueWitheKey(out _webHost, _keyWebHost);
        }

        protected override void TryGetList(Dictionary<string, object> dic)
        {
            base.TryGetList(dic);
            if (dic.ContainsKey(_keyDetail))
            {
                List<object> list;
                dic.TryGetValueWitheKey(out list, _keyDetail);
                Items = GetDatas(list);
            }
        }
    }
}
