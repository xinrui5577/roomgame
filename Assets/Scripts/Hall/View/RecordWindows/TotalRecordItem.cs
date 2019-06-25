/** 
 *文件名称:     TotalRecordItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-10-20 
 *描述:         总战绩Item（无回放相关数据）
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class TotalRecordItem : YxView
    {
        [Tooltip("记录ID")]
        public UILabel RoundNum;
        [Tooltip("房卡ID")]
        public UILabel RoomId;
        [Tooltip("对战时间")]
        public UILabel Time;
        [Tooltip("头像Grid")]
        public UIGrid HeadGrid;
        [Tooltip("背景图")]
        public UISprite BgSprite;
        [Tooltip("游戏名称（根据游戏需求，资源控制是否显示游戏名称）")]
        public UILabel GameName;
        [Tooltip("头像预设")]
        public HeadItem HeadItemPrefab;
        [Tooltip("默认背景高度")]
        public int BaseHeight=220;
        [Tooltip("局数格式")]
        public string RoundFormat= "第{0}局";
        /// <summary>
        /// 回放开关
        /// </summary>
        public static bool PlayBack;
        public string CurGameKey
        {
            get
            {
                if(_curItemData!=null)
                {
                    return _curItemData.GameKey;
                }
                return "";
            }
        }

        public string CurRoomId
        {
            get
            {
                if (_curItemData != null)
                {
                    return _curItemData.RealRoomId;
                }
                return "";
            }
        }

        public string DetailFunc
        {
            get
            {
                if (_curItemData != null)
                {
                    return _curItemData.FunctionName;
                }
                return "";
            }
        }

        public bool DetailPlayBack
        {
            get
            {
                if(_curItemData!=null)
                {
                    return PlayBack && PlayBack == _curItemData.PlayBack;
                }
                return false;
            }
        }

        /// <summary>
        /// 本条数据
        /// </summary>
        private TotalRecordItemData _curItemData;
        public void ShowItem(string roundNum,string selfId)
        {
            _curItemData.ShowRoundNum = roundNum;
            SetItemInfo(_curItemData);
            SetHeads(_curItemData.HeadDatas);
            YxTools.TrySetComponentValue(GameName, _curItemData.GameName);
            float moveY = HeadGrid.cellHeight*((HeadGrid.GetChildList().Count-1)/HeadGrid.maxPerLine);
            if (BgSprite)
            {
                BgSprite.height = (int) moveY+ BaseHeight;
            }
        }
        /// <summary>
        /// 设置时间，房卡号，局数相关信息
        /// </summary>
        /// <param name="itemData"></param>
        private void SetItemInfo(TotalRecordItemData itemData)
        {
            Time.text = itemData.Time;
            RoomId.text = itemData.ShowRoomId;
            RoundNum.text = string.Format(RoundFormat,Id);
        }

        /// <summary>
        /// 设置头像相关信息
        /// </summary>
        /// <param name="heads"></param>
        private void SetHeads(List<object> heads)
        {
            for (int i = 0,lenth= heads.Count; i < lenth; i++)
            {
               var view= YxTools.GetChildView(i,HeadItemPrefab ,HeadGrid.transform);
                view.UpdateView(heads[i]);
            }
            HeadGrid.repositionNow = true;
        }

        protected override void OnFreshView()
        {
            CheckItemData();
        }

        protected bool CheckItemData()
        {
            var data = Data as TotalRecordItemData;
            if (data != null)
            {
                _curItemData = data;
                ShowItem(Id, App.UserId);
                return true;
            }
            return false;
        }

        protected override void OnHide()
        {
            var list = HeadGrid.GetChildList();
            foreach (var item in list)
            {
                item.gameObject.SetActive(false);
            }
        }

        public override void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }
    }

    public class TotalRecordItemData:YxData
    {
        /// <summary>
        /// Key游戏gamekey
        /// </summary>
        private const string KeyGameKey = "game_key_c";
        /// <summary>
        /// Key回放局数
        /// </summary>
        private const string KeyRound = "round";
        /// <summary>
        /// Key请求接口
        /// </summary>
        private const string KeyFunction= "function";
        /// <summary>
        /// Key房间ID
        /// </summary>
        private const string KeyRoomId = "room_id";
        /// <summary>
        ///  Key结算数据
        /// </summary>
        private const string KeyOverInfo = "overinfo";
        /// <summary>
        /// Key创建时间
        /// </summary>
        private const string KeyCreateTime = "create_dt";
        /// <summary>
        /// Key回放标识
        /// </summary>
        private const string KeyPlayBack = "playback";
        /// <summary>
        /// Key游戏名称
        /// </summary>
        private const string KeyGameName = "game_name";
        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId;
        /// <summary>
        /// 对战时间
        /// </summary>
        private string _createTime;
        /// <summary>
        /// 请求方法名称
        /// </summary>
        private string _functionName;
        /// <summary>
        /// 游戏gamekey
        /// </summary>
        private string _gamekey;
        /// <summary>
        /// 游戏名称
        /// </summary>
        private string _gameName;
        /// <summary>
        /// 详情局数
        /// </summary>
        private int _detailRoundNum;
        /// <summary>
        /// 显示局数
        /// </summary>
        private string _showRound;
        /// <summary>
        /// 回放标识：0无回放；1有回放
        /// </summary>
        private int _playBack;
        /// <summary>
        /// 头像数据
        /// </summary>
        private List<object> _headDatas=new List<object>();

        public string ShowRoomId
        {
            get
            {
                return _roomId.Substring(_roomId.Length - 6, 6);
            }
        }

        public string RealRoomId
        {
            get { return _roomId; }
        }

        public string Time
        {
            get { return _createTime; }
        }

        public List<object> HeadDatas
        {
            get
            {
                return _headDatas;
            }
        }

        public string ShowRoundNum
        {
            get
            {
                return _showRound;   
            }
            set
            {
                _showRound = value;
            }
        }

        public int DetailRoundNum
        {
            get
            {
                return _detailRoundNum;
            }
        }


        public bool PlayBack
        {
            get
            {
                return _playBack==1;
            }
        }

        public string FunctionName
        {
            get
            {
                return _functionName;
            }
        }

        public string GameKey
        {
            get
            {
                return _gamekey;
            }
        }

        public string GameName
        {
            get
            {
                return _gameName;
            }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            YxTools.TryGetValueWitheKey(dic, out _roomId, KeyRoomId);
            YxTools.TryGetValueWitheKey(dic, out _createTime, KeyCreateTime);
            YxTools.TryGetValueWitheKey(dic, out _headDatas, KeyOverInfo);
            YxTools.TryGetValueWitheKey(dic, out _detailRoundNum, KeyRound);
            YxTools.TryGetValueWitheKey(dic, out _functionName, KeyFunction);
            YxTools.TryGetValueWitheKey(dic, out _gamekey,KeyGameKey);
            YxTools.TryGetValueWitheKey(dic, out _gameName, KeyGameName);
            if (dic.ContainsKey(KeyPlayBack))
            {
                YxDebug.LogError("存在回放标识");
                var success= int.TryParse(dic[KeyPlayBack].ToString(), out _playBack);
                if (!success)
                {
                    _playBack = 1;
                }
            }
            else
            {
                _playBack = 1;
            }
            
        }

        public TotalRecordItemData(object data) : base(data)
        {
            
        }
    }
}
