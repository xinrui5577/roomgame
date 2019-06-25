/** 
 *文件名称:     MatchItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-19 
 *描述:         
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using Assets.Scripts.Hall.View.PageListWindow;
using Assets.Scripts.Tea;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MatchWindow
{
    public class MatchItem :YxView
    {
        #region UI Param
        [Tooltip("标题")]
        public UILabel Title;
        [Tooltip("开始时间")]
        public UILabel StartTime;
        [Tooltip("结束时间")]
        public UILabel EndTime;
        [Tooltip("游戏状态组：0.未开始 1.已开始 2.已结束")]
        public GameObject[] StateGroup;
        [Tooltip("详情预设")]
        public YxView DetailItem;
        [Tooltip("布局")]
        public UIGrid Grid;
        [Tooltip("Tween执行者")]
        public UIPlayTween TweenPlay;
        [Tooltip("当前玩家信息容器")]
        public GameObject SelfContainer;
        [Tooltip("当前玩家的信息")]
        public UILabel SelfInfo;
        [Tooltip("状态变化监听对象")]
        public GameObject Target;
        #endregion
        #region Data Param
        [Tooltip("金币名称")]
        public string GoldName = "金币";
        [Tooltip("元宝名称")]
        public string CashName = "元宝";
        [Tooltip("群积分")]
        public string GroupCoinName = "群积分";
        [Tooltip("比赛详情请求")]
        public string KeyDetailAction = "matchDetail";
        [Tooltip("状态变化监听方法名称")]
        public string OnStateChangeFunc= "OnMatchStateChange";
        [Tooltip("上限提示")]
        public string UpperLimitNotice = "非常抱歉!!!当前房间最高上限需要{0}{1}，您的{1}有{2}大于房间的最大上限不能进入!";
        [Tooltip("下限提示")]
        public string LowerLimitNotice = "非常抱歉!!!当前房间最低下限需要{0}{1}，您的{1}只有{2}小于房间的最低下限不能进入!";
        [Tooltip("创建房间窗口名称")]
        public string CreateRoomWindowName = "DefCreateRoomWindow";
        
        #endregion

        #region Local Data
        /// <summary>
        /// 当前比赛的简略信息
        /// </summary>
        private MatchItemData _curData;
        /// <summary>
        /// tween播放状态
        /// </summary>
        private bool _tweenState=false;

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            if(Data==null)
            {
                return;
            }
            else if(Data is MatchItemData)
            {
                _curData=Data as MatchItemData;
                if (TweenPlay)
                {
                    if (_tweenState)
                    {
                        if (_curData.NeedCloseWhenLoad)
                        {
                            _tweenState = false;
                            TweenPlay.Play(_tweenState);
                        }
                        else
                        {
                            _curData.NeedCloseWhenLoad = true;
                        }                       
                    }
                }
                ShowMainUi();
            }
            else if(Data is Dictionary<string,object>)
            {    
                ShowDetailInfo();
            }
        }
        #endregion

        #region Function
        /// <summary>
        /// 加入比赛
        /// </summary>
        public void JoinMatch()
        {
            if(_curData!=null)
            {
                var gameKey = _curData.GameKey;
                var gameType = _curData.GameType;
                YxDebug.LogError("加入比赛");
                YxDebug.LogError(string.Format("当前比赛的游戏是：{0}房间类型是：{1}", gameKey, gameType));
                var dic = GameListModel.Instance.GameUnitModels;
                if (dic.ContainsKey(gameKey))
                {
                    if (_curData.IsQucikGame)
                    {
                        OnOpenQuickGame();
                    }
                    else
                    {
                       OnOpenCreateWindow();
                    }
                }
                else
                {
                    YxMessageBox.Show("咋整地，没配置游戏就想比赛啊!");
                }

            }
        }

        protected virtual void ShowMainUi()
        {
            if (_curData!=null)
            {
                YxTools.TrySetComponentValue(Title, _curData.Title);
                YxTools.TrySetComponentValue(StartTime, _curData.StartTime);
                YxTools.TrySetComponentValue(EndTime, _curData.EndTime);
                if (StateGroup!=null)
                {
                    int state = (int) _curData.State;
                    if (StateGroup.Length> state)
                    {
                        for (int i = 0,count= StateGroup.Length; i < count; i++)
                        {
                            StateGroup[i].SetActive(false);
                        }
                        StateGroup[state].SetActive(true);
                    } 
                }
            }
        }

        private void ShowDetailInfo()
        {
            MatchDetailData detailData=new MatchDetailData(Data,typeof(MatchRewardData));
            if (detailData.MatchState!=_curData.State)
            {
                _curData.State = detailData.MatchState;
                if (Target)
                {
                    Target.SendMessage(OnStateChangeFunc,new object[]
                    {
                        this,_curData
                    }
                , SendMessageOptions.RequireReceiver);
                }
            }
            if (YxTools.TrySetComponentValue(SelfContainer, !string.IsNullOrEmpty(detailData.SelfInfo)))
            {
                YxTools.TrySetComponentValue(SelfInfo, detailData.SelfInfo);
            }
     
            if (Grid)
            {
                bool stateChange = !detailData.MatchState.Equals(_curData.State);
                bool exist = Grid.transform.childCount != 0;
                if (!exist)
                {
                    FreshGrid(detailData.DataItems);
                }
                else
                {
                    if (stateChange)
                    {
                        FreshGrid(detailData.DataItems);
                    }
                }                      
            }
        }
        public void ShowDetail()
        {
            if (!_tweenState)
            {
                if (_curData != null)
                {
                    Facade.Instance<TwManger>().SendAction(
                    KeyDetailAction,
                    new Dictionary<string, object>()
                    { {_curData.KeyID,_curData.ID} },
                    UpdateView
                    );
                }
            }
            if (TweenPlay)
            {
                _tweenState = !_tweenState;
                TweenPlay.Play(_tweenState);
            }
        }

        private YxView GetChildView(int index)
        {
            if (Grid.transform.childCount <= index)
            {
                return YxWindowUtils.CreateItem(DetailItem, Grid.transform);
            }
            return Grid.transform.GetChild(index).GetComponent<YxView>();
        }

        private void FreshGrid(List<YxData> details)
        {
            if (Grid)
            {
                var dataCount = details.Count;
                for (int i = 0; i < dataCount; i++)
                {
                    var item = GetChildView(i);
                    item.gameObject.SetActive(true);
                    item.UpdateView(details[i]);
                }
                var itemCount = Grid.transform.childCount;
                if (itemCount> dataCount)
                {
                    for (int i = dataCount; i < itemCount; i++)
                    {
                       GetChildView(i).gameObject.SetActive(false);
                    }
                }
                Grid.repositionNow = true;
            }
        }

        private void OnOpenCreateWindow()
        {
            if (_curData!=null)
            {
                var win = CreateOhterWindowWithT<CreateRoomWindow>(CreateRoomWindowName);
                win.GameKey = _curData.GameKey;
            }
        }

        private void OnOpenQuickGame()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            EnumCostType costType = _curData.CostType;
            var maxCost = _curData.MaxValue;
            var minCost = _curData.MinVlaue;
            double haveValue = 0;
            string costName = "";
            if(maxCost>0)
            {
                switch (costType)
                {
                    case EnumCostType.Cash:
                        haveValue = userInfo.CashA;
                        costName = CashName;
                        break;
                    case EnumCostType.Gold:
                        haveValue = YxUtiles.GetShowNumber(userInfo.CoinA + userInfo.BankCoin);
                        costName = GoldName;
                        break;
                    case EnumCostType.TempCoin:
                        haveValue = int.MaxValue;   
                        break;
                    case EnumCostType.GroupCoin:
                        costName = GroupCoinName;
                        haveValue = int.MaxValue;
                        break;
                    default:
                        haveValue = int.MaxValue;
                        break;
                }
                if (minCost> haveValue)
                {
                    YxMessageBox.Show(string.Format(LowerLimitNotice,minCost,costName,haveValue));
                    return;
                }
                if (maxCost < haveValue)
                {
                    YxMessageBox.Show(string.Format(UpperLimitNotice, minCost, costName, haveValue));
                    return;
                }
            }
            RoomUnitModel model=new RoomUnitModel(null);
            model.TypeId = _curData.GameType.ToString();
            model.GameKey = _curData.GameKey;
            RoomListController.Instance.OnDirectGame(model);
        }

        #endregion

    }

    /// <summary>
    /// 比赛数据
    /// </summary>
    public class MatchItemData:YxData
    {
        /// <summary>
        /// Key比赛ID
        /// </summary>
        private const string KeyId= "id";
        /// <summary>
        /// Key比赛名称（标题）
        /// </summary>
        private const string KeyTitle = "title_m";
        /// <summary>
        /// Key开始时间
        /// </summary>
        private const string KeyStartTime = "start_dt";
        /// <summary>
        /// Key结束时间
        /// </summary>
        private const string KeyEndTime = "end_dt";
        /// <summary>
        /// Key 比赛状态
        /// </summary>
        private const string KeyStatus = "status_i";
        /// <summary>
        /// key gamekey
        /// </summary>
        private const string KeyGameKey= "game_key_c";
        /// <summary>
        /// key 房间类型
        /// </summary>
        private const string KeyGameType= "game_type";
        /// <summary>
        /// key 消耗资源最大值
        /// </summary>
        private const string KeyGameMaxValue = "gold_max_q";
        /// <summary>
        /// key 消耗资源最小值
        /// </summary>
        private const string KeyGameMinValue = "gold_min_q";
        /// <summary>
        /// Key 资源消耗类型
        /// </summary>
        private const string KeyGameCostType = "item_id";
        /// <summary>
        /// 创建房间标识小于等于-1的房间都为创建房间模式
        /// </summary>
        private const int _createRoomFlag = -1;
        /// <summary>
        /// 比赛ID
        /// </summary>
        private string _id;
        /// <summary>
        /// 比赛标题    
        /// </summary>
        private string _title;
        /// <summary>
        /// 比赛开始时间（精确到分钟，格式为：2017-11-29 14:35:00）
        /// </summary>
        private string _startTime;
        /// <summary>
        /// 比赛结束时间（精确到分钟，格式为：2017-11-29 14:35:00）
        /// </summary>
        private string _endTime;
        /// <summary>
        /// 比赛状态
        /// 0：未开始
        /// 1：进行中
        /// 2：已结束
        /// </summary>
        private EnumMatchState _state;
        /// <summary>
        /// 游戏gamekey
        /// </summary>
        private string _gameKey;
        /// <summary>
        /// 游戏类型（房卡房间：-1，娱乐模式：0~n）
        /// </summary>
        private int _gameType;
        /// <summary>
        /// 消耗最小值
        /// </summary>
        private float _minVlaue;
        /// <summary>
        /// 消耗最大值
        /// </summary>
        private float _maxValue;
        /// <summary>
        /// 消耗类型
        /// </summary>
        private EnumCostType _costType;

        /// <summary>
        /// 加载后是否需要关闭
        /// </summary>
        public bool NeedCloseWhenLoad=true;

        public string KeyID
        {
            get { return KeyId; }
        }

        public string ID
        {
            get
            {
                return _id; 
            }
        }

        public string Title
        {
            get
            {
                return _title;                 
            }
        }

        public string StartTime
        {
            get
            {
                return _startTime;
            }
        }

        public string EndTime
        {
            get
            {
                return _endTime;               
            }
        }

        public EnumMatchState State
        {
            get
            {
                return _state;                
            }
            set { _state = value; }
        }

        public string GameKey
        {
            get { return _gameKey; }
        }

        public int GameType
        {
            get { return _gameType;}
        }

        public EnumCostType CostType
        {
            get
            {
                return _costType;
            }
        }

        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
        }

        public float MinVlaue
        {
            get
            {
                return _minVlaue;
            }
        }

        /// <summary>
        /// 是否为娱乐模式
        /// </summary>
        public bool IsQucikGame
        {
            get { return _gameType>_createRoomFlag; }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            YxTools.TryGetValueWitheKey(dic, out _id, KeyId);
            YxTools.TryGetValueWitheKey(dic, out _title, KeyTitle);
            YxTools.TryGetValueWitheKey(dic, out _startTime, KeyStartTime);
            YxTools.TryGetValueWitheKey(dic, out _endTime, KeyEndTime);
            YxTools.TryGetValueWitheKey(dic, out _gameKey, KeyGameKey);
            YxTools.TryGetValueWitheKey(dic, out _gameType, KeyGameType);
                YxTools.TryGetValueWitheKey(dic, out _maxValue, KeyGameMaxValue);
            YxTools.TryGetValueWitheKey(dic, out _minVlaue, KeyGameMinValue);
            var status = 0;
            YxTools.TryGetValueWitheKey(dic, out status, KeyStatus);
            _state = (EnumMatchState)status;
            var costType = "";
            YxTools.TryGetValueWitheKey(dic, out costType, KeyGameCostType);
            _costType = YxTools.GetCostTypeByString(costType);
        }

        public MatchItemData(object data) : base(data)
        {
        }
    }

    /// <summary>
    /// 比赛详情
    /// </summary>
    public class MatchDetailData:PageRequestData
    {
        /// <summary>
        /// Key self info
        /// </summary>
        private string _keySelf = "self";
        /// <summary>
        /// Key 比赛状态
        /// </summary>
        private string _keyStatus = "status_i";
        /// <summary>
        /// 当前玩家信息
        /// </summary>
        private string _selfInfo;
        /// <summary>
        /// 比赛状态 
        /// </summary>
        private EnumMatchState _status;

        public string SelfInfo
        {
            get
            {
                return _selfInfo;
            }
        }

        public EnumMatchState MatchState
        {
            get { return _status; }
            set { _status = value; }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            YxTools.TryGetValueWitheKey(dic, out _selfInfo,_keySelf);
            if (dic.ContainsKey(_keyStatus))
            {
                _status = (EnumMatchState)Enum.Parse(typeof(EnumMatchState), dic[_keyStatus].ToString());
            }
        }

        public MatchDetailData(object data, Type type) : base(data, type)
        {
        }
    }

    /// <summary>
    /// 比赛状态
    /// 0：未开始
    /// 1：比赛中
    /// 2：已结束
    /// </summary>
    public enum EnumMatchState
    {
        Before=0,
        Matching=1,
        End=2,
    }
}
