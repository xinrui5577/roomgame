using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.pludo.View;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Abstracts;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGameData.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:         飞行棋游戏数据
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoGameData : YxGameData
    {

        #region Data Param
        /// <summary>
        /// 地图常规格长度(外围环绕一圈)
        /// </summary>
        public static int MapNormalLenth = 24;
        [Tooltip("创建房间状态事件")]
        public List<EventDelegate> CreateRoomStateAction = new List<EventDelegate>();

        [Tooltip("游戏开始后事件(返回按钮显示)")]
        public List<EventDelegate> GameRoundStateAction = new List<EventDelegate>();

        [Tooltip("托管按钮显示状态")]
        public List<EventDelegate> AutoStateAction = new List<EventDelegate>();

        [Tooltip("遥控骰子")]
        public List<EventDelegate> ControlDiceAction = new List<EventDelegate>();

        /// <summary>
        /// 房间信息
        /// </summary>
        public RoomInfo RoomInfo { private set; get; }
        /// <summary>
        /// 创建房间状态
        /// </summary>
        public bool IsCreateRoom
        {
            get
            {
                if (RoomInfo != null)
                {
                    return RoomInfo.IsCreateRoom;
                }
                else
                {
                    return false;
                }

            }
        }
        /// <summary>
        /// 是否显示返回按钮
        /// </summary>
        public bool CouldShowBack { private set; get; }
        /// <summary>
        /// 是否可用遥控骰子
        /// </summary>
        public bool CouldShowControlDice { private set; get; }


        /// <summary>
        /// 主要玩家
        /// </summary>
        public PludoGameSelfPlayer MainUser { get { return GetPlayer<PludoGameSelfPlayer>(ConstantData.CurUiSeat); } }
        /// <summary>
        /// 主要玩家信息
        /// </summary>
        public PludoPlayerInfo MainUserInfo { get { return GetPlayerInfo<PludoPlayerInfo>(ConstantData.CurUiSeat); } }

        /// <summary>
        /// 主要玩家是否可以打骰子
        /// </summary>
        public bool MainPlayerRollDice
        {
            get
            {
                if (MainUser)
                {
                    return MainUser.CouldRollDice;
                }
                return false;
            }
        }
        /// <summary>
        /// 主要玩家是否可以起飞
        /// </summary>
        public bool MainPlayerCouldFly
        {
            get
            {
                if (MainUser)
                {
                    return MainUser.CouldFly;
                }
                return false;
            }
        }

        /// <summary>
        /// 遥控骰子消耗
        /// </summary>
        public int ControlPointConsume
        {
            private set;get;
        }

        /// <summary>
        /// 当前执行操作的玩家
        /// </summary>
        public PludoGamePlayer CurOpUser { set; get; }

        private PludoPlayerInfo CurUserInfo
        {
            get { return CurOpUser.GetInfo<PludoPlayerInfo>(); }
        }
        /// <summary>
        /// 地图信息
        /// </summary>
        public PludoMapInfo MapInfo { private set; get; }
        /// <summary>
        /// 选择飞机结果
        /// </summary>
        public ChoosePlaneResult ChooseResult { private set; get; }
        /// <summary>
        /// 小结算数据
        /// </summary>
        public PludoGameResultData ResultData { private set; get; }
        /// <summary>
        /// 投票数据
        /// </summary>
        public HandUpData HandData { private set; get; }

        public SettingInfo SettingInfo
        {
            get
            {
                SettingInfo info = new SettingInfo();
                if (RoomInfo != null)
                {
                    info.IsCreatRoom = IsCreateRoom;
                    info.IsOwener = RoomInfo.OwnerId == MainUserInfo.UserId;
                    info.IsGameStart = RoomInfo.IsGameStart;
                    info.UserId = int.Parse(MainUserInfo.UserId);
                }
                return info;
            }
        }

        /// <summary>
        /// 大结算数据
        /// </summary>
        public PludoGameOverData OverData { private set; get; }
        /// <summary>
        /// 托管状态
        /// </summary>
        public bool AutoState { private set; get; }

        /// <summary>
        /// 投票时间
        /// </summary>
        public static int HandUpTime { private set; get; }
        /// <summary>
        /// 打骰子默认时长
        /// </summary>
        public static int RollDicCdTime { private set; get; }

        /// <summary>
        /// 选择飞机默认时长
        /// </summary>
        public static int ChoosePlaneCdTime { private set; get; }
        /// <summary>
        /// 遥控骰子次数
        /// </summary>
        public static int ControlDiceNum { private set; get; }

        #endregion

        #region Local Data
        /// <summary>
        /// 地图数据
        /// </summary>
        private List<PludoMapItemData> _mapList = new List<PludoMapItemData>();

        #endregion


        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            GetRoomInfo(gameInfo);
            GetMapInfo(gameInfo);
            GetOtherInfo(gameInfo);
            CheckCreateRoomState();
            CheckGameRoundState();
        }
        #region Life Cycle

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            var keys = cargs2.GetKeys();
            var keyCount = keys.Length;
            HandUpTime = ConstantData.ValueHupDefTime;
            ChoosePlaneCdTime = ConstantData.ValueCpDefTime;
            RollDicCdTime = ConstantData.ValueRollDicDefTime;
            for (int i = 0; i < keyCount; i++)
            {
                var itemKey = keys[i];
                switch (itemKey)
                {
                    case ConstantData.KeyCargsCpDicTime:
                        ChoosePlaneCdTime = int.Parse(cargs2.GetUtfString(itemKey));
                        YxDebug.LogError("自定义选择飞机时间：" + ChoosePlaneCdTime);
                        break;
                    case ConstantData.KeyCargsRollDicTime:
                        RollDicCdTime = int.Parse(cargs2.GetUtfString(itemKey));
                        YxDebug.LogError("自定义打骰子时间：" + RollDicCdTime);
                        break;
                    case ConstantData.KeyCargsHupTime:
                        HandUpTime = int.Parse(cargs2.GetUtfString(itemKey));
                        YxDebug.LogError("自定义投票时间：" + HandUpTime);
                        break;
                    case ConstantData.KeyCargsControlDice:
                        ControlDiceNum = int.Parse(cargs2.GetUtfString(itemKey));
                        CouldShowControlDice = ControlDiceNum > ConstantData.IntValue;
                        YxDebug.LogError("遥控骰子数量：" + ControlDiceNum);
                        break;
                }
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ControlDiceAction.WaitExcuteCalls());
            }
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            PludoPlayerInfo baseGameUserInfo = new PludoPlayerInfo();
            baseGameUserInfo.Parse(userData);
            baseGameUserInfo.CheckRoomOwner(OwnersId.ToString());
            return baseGameUserInfo;
        }

        #endregion

        #region Function

        /// <summary>
        /// 获取地图信息
        /// </summary>
        /// <param name="data"></param>
        private void GetMapInfo(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyMapInfo))
            {
                var mapInfo = data.GetSFSArray(ConstantData.KeyMapInfo);
                if (mapInfo != null)
                {
                    var selfColor = MainUserInfo.PlayerColor;

                    var count = mapInfo.Count;
                    MapInfo = new PludoMapInfo();
                    //全部数据
                    var values = Enum.GetValues(typeof(ItemColor));
                    var valueCount = values.Length;
                    var safePos = new Dictionary<int, List<PludoMapItemData>>();
                    var readyPos = new Dictionary<int, List<PludoMapItemData>>();
                    var startPosDic = new Dictionary<int, int>();
                    _mapList = new List<PludoMapItemData>();
                    for (int i = 0; i < count; i++)
                    {
                        var itemData = mapInfo.GetSFSObject(i);
                        PludoMapItemData mapData = new PludoMapItemData(itemData);
                        _mapList.Add(mapData);
                    }
                    for (int i = 0; i < valueCount; i++)
                    {
                        int item = (int)values.GetValue(i);
                        List<PludoMapItemData> list = new List<PludoMapItemData>();
                        List<PludoMapItemData> readyList = new List<PludoMapItemData>();
                        safePos.Add(item, list);
                        readyPos.Add(item, readyList);
                        startPosDic.Add(item, GetMapStartIdWithColor(item));
                    }
                    //环绕区(公用区域)
                    var commomArea = new List<PludoMapItemData>();
                    var startIndex = startPosDic[selfColor];
                    for (int i = 0; i < MapNormalLenth; i++)
                    {
                        commomArea.Add(_mapList[(i + startIndex) % MapNormalLenth]);
                    }
                    //准备区
                    for (int i = 0; i < valueCount; i++)
                    {
                        var color = (selfColor + i) % valueCount;
                        var list = new List<PludoMapItemData>();
                        for (int index = 0; index < ConstantData.ValueReadyAreaCount; index++)
                        {
                            PludoMapItemData stayMapData = new PludoMapItemData(color, GetReadyId(color, index), (int)EnumMapItemType.ReadyArea);
                            list.Add(stayMapData);
                        }
                        readyPos[color] = list;
                    }

                    //安全区
                    for (int i = MapNormalLenth; i < count; i++)
                    {
                        PludoMapItemData mapData = _mapList[i];
                        safePos[mapData.ItemColor].Add(mapData);
                    }
                    MapInfo.CommomArea = commomArea;
                    MapInfo.ReadyArea = readyPos;
                    MapInfo.SafeArea = safePos;
                    MapInfo.StartDataId = startPosDic;
                    MapInfo.CurColor = selfColor;
                }
            }
        }

        /// <summary>
        /// 根据颜色确定对应地图起始点的Id
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int GetMapStartIdWithColor(int color)
        {
            var checkType = (EnumMapItemType)(ConstantData.ValueBeginStateBase * Math.Pow(ConstantData.ValueBeginStatePerChange, color));
            var startIndex = _mapList.FindIndex(itemData => itemData.CheckMapState(checkType));
            if (startIndex <= ConstantData.IntDefValue)
            {
                Debug.LogError(string.Format("找不到对应颜色{0}的起始点,检测类型{1}", (ItemColor)color, checkType));
                return startIndex;
            }
            else
            {
                var id = _mapList[startIndex].MapDataId;
                return id;
            }

        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="data"></param>
        private void GetRoomInfo(ISFSObject data)
        {
            RoomInfo = new RoomInfo(data);
            RoomInfo.CreateRoomInfo = CreateRoomInfo;
            RoomInfo.RoomName = RoomName;
            for (int i = 0; i < SeatTotalCount; i++)
            {
                var playerInfo = GetPlayerInfo<PludoPlayerInfo>();
                playerInfo.CheckRoomOwner(RoomInfo.OwnerId);
            }
        }

        /// <summary>
        /// 获取其它信息
        /// </summary>
        private void GetOtherInfo(ISFSObject data)
        {
            var consumeNum = 0;
            SfsHelper.Parse(data, ConstantData.KeyControlPointConsume, ref consumeNum);
            ControlPointConsume = consumeNum;
            CheckAutoState();
            Facade.EventCenter.DispatchEvent(LoaclRequest.SettingInfoChange, SettingInfo);
        }

        /// <summary>
        /// 解析打骰子结果
        /// </summary>
        /// <param name="data"></param>
        private void ParseRollResult(ISFSObject data)
        {
            var point = ConstantData.IntDefValue;
            var more = false;
            var useControl = false;
            var cashCount = ConstantData.IntDefValue;
            SfsHelper.Parse(data, ConstantData.KeyMoreTime, ref more);
            SfsHelper.Parse(data, ConstantData.KeyRollPoint, ref point);
            SfsHelper.Parse(data, ConstantData.KeyCash, ref cashCount);
            SfsHelper.Parse(data, ConstantData.KeyUseControlDice, ref useControl);
            var list = new List<int>();
            if (data.ContainsKey(ConstantData.KeyChoosePlane))
            {
                var planes = data.GetIntArray(ConstantData.KeyChoosePlane);
                YxDebug.LogError("可以选择的飞机数量是：" + planes.Length);
                foreach (var id in planes)
                {
                    YxDebug.LogError(id);
                }
                list.AddRange(planes);
                
            }
            CurUserInfo.RollDiceData = new RollDiceData()
            {
                QuickModel = false,
                OneMoreTime = more,
                ShowPoint = point,
                PlaneIds = list.ToList(),
                UseControlDice= useControl,
                HaveCashNum=cashCount,
                Sex =CurUserInfo.SexI,
                ShowAni= CurUserInfo.Seat==SelfSeat,
                CostCashNum=ControlPointConsume
            };

        }

        /// <summary>
        /// 获取当前操作信息（重连使用）
        /// </summary>
        /// <param name="data"></param>
        public void GetOperation(ISFSObject data)
        {
            long svt = 0;
            long st = 0;
            SfsHelper.Parse(data, ConstantData.KeyServerNowTime, ref svt);
            SfsHelper.Parse(data, ConstantData.KeyStateStartTime, ref st);
            var finish = svt - st;
            SetCurPlayer(data);
            var point = ConstantData.IntDefValue;
            SfsHelper.Parse(data, ConstantData.KeyRollPoint, ref point);
            CurUserInfo.RollDiceData.ShowPoint = point;
            CurUserInfo.SetStateTime(finish);
        }

        /// <summary>
        /// 设置当前玩家
        /// </summary>
        /// <param name="data"></param>
        public void SetCurPlayer(ISFSObject data)
        {
            var curOpSeat = ConstantData.IntDefValue;
            SfsHelper.Parse(data, ConstantData.KeyCurrentPlayer, ref curOpSeat);
            CurOpUser = GetPlayer<PludoGamePlayer>(curOpSeat, true);
            if (data.ContainsKey(ConstantData.KeyControlDiceTime))
            {
                var canuseTime = ConstantData.IntDefValue;
                SfsHelper.Parse(data, ConstantData.KeyControlDiceTime, ref canuseTime);
                CurUserInfo.SetControlDiceTime(canuseTime);
            }

        }

        /// <summary>
        /// 牌局开始
        /// </summary>
        /// <param name="data"></param>
        public void OnGameStart(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyPlaneInfo))
            {
                var planeArray = data.GetSFSArray(ConstantData.KeyPlaneInfo);
                var count = Math.Min(planeArray.Count, SeatTotalCount);
                for (int i = 0; i < count; i++)
                {
                    var userInfo = GetPlayerInfo<PludoPlayerInfo>(i, true);
                    if (userInfo != null)
                    {
                        userInfo.SetPlanesInfo(planeArray.GetSFSArray(i));
                    }
                }
                if (RoomInfo.IsCreateRoom)
                {
                    RoomInfo.CreateRoomInfo.CurRound++;
                    CheckGameRoundState();
                }
            }
            Facade.EventCenter.DispatchEvent(LoaclRequest.SettingInfoChange, SettingInfo);
        }

        /// <summary>
        /// 打骰子回调
        /// </summary>
        /// <param name="data"></param>
        public void OnShowRollResult(ISFSObject data)
        {
            SetCurPlayer(data);
            ParseRollResult(data);
        }

        /// <summary>
        /// 选择飞机消息数据处理
        /// </summary>
        /// <param name="data"></param>
        public void OnChoosePlane(ISFSObject data)
        {
            ChooseResult = new ChoosePlaneResult();
            if (data.ContainsKey(ConstantData.KeyChangeInfo))
            {
                var infoArray = data.GetSFSArray(ConstantData.KeyChangeInfo);
                var count = infoArray.Count;
                ChooseResult.PlaneDataDic = new Dictionary<int, Dictionary<int, PludoPlaneData>>();
                for (int i = 0; i < count; i++)
                {
                    var dataItem = infoArray.GetSFSObject(i);
                    var seat = dataItem.GetInt(RequestKey.KeySeat);
                    var playerColor = GetPlayerInfo<PludoPlayerInfo>(seat, true).PlayerColor;
                    var planeDic = new Dictionary<int, PludoPlaneData>();
                    for (int j = 0; j < ConstantData.ValuePlaneCount; j++)
                    {
                        if (dataItem.ContainsKey(j.ToString()))
                        {
                            var jData = dataItem.GetSFSObject(j.ToString());
                            var planeData = new PludoPlaneData(jData, playerColor);
                            planeDic.Add(planeData.DataId, planeData);
                        }
                    }
                    ChooseResult.PlaneDataDic.Add(seat, planeDic);
                }
            }
            SfsHelper.Parse(data, ConstantData.KeyCurrentPlayer, ref ChooseResult.OperationSeat);
            SfsHelper.Parse(data, ConstantData.KeyPlaneId, ref ChooseResult.OperationPlaneId);
        }

        /// <summary>
        /// 小结算数据处理
        /// </summary>
        /// <param name="data"></param>
        public void OnGameResult(ISFSObject data)
        {
            ResultData = new PludoGameResultData();
            ResultData.ResultList = new List<PludoGameResultItemData>();
            if (data.ContainsKey(RequestKey.KeyPlayerList))
            {
                var array = data.GetSFSArray(RequestKey.KeyPlayerList);
                var count = Math.Min(array.Count, SeatTotalCount);
                for (int i = 0; i < count; i++)
                {
                    var resultItemData = new PludoGameResultItemData(array.GetSFSObject(i));
                    var player = GetPlayer<PludoGamePlayer>(resultItemData.ServerSeat, true);
                    if (player)
                    {
                        resultItemData.SetOtherData(player.GetInfo<PludoPlayerInfo>(), ConstantData.ValuePlaneCount, resultItemData.ServerSeat == SelfSeat);
                        player.Coin = resultItemData.TotalGold;
                        player.ReadyState = false;
                        player.GetInfo<PludoPlayerInfo>().IsAuto = false;
                        player.AutoStateChange(false);
                    }
                    ResultData.ResultList.Add(resultItemData);
                }
                ResultData.ResultList.Sort((a, b) =>
                {
                    if (a.ScoreNum < b.ScoreNum)
                    {
                        return 1;
                    }
                    else if (a.ScoreNum == b.ScoreNum)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }
            var svt = 0L;
            SfsHelper.Parse(data, ConstantData.KeyServerNowTime, ref svt);
            ResultData.Time = GetNowTime(svt);
            ResultData.RoomInfo = RoomInfo;
            CheckAutoState();
        }

        /// <summary>
        /// 获取大结算数据
        /// </summary>
        /// <param name="data"></param>
        public void OnGetGameOverData(ISFSObject data)
        {
            OverData = new PludoGameOverData();
            var winnner = 0;
            SfsHelper.Parse(data, ConstantData.KeyWinner, ref winnner);
            OverData.OverDatas = new List<PludoGameOverItemData>();
            if (data.ContainsKey(RequestKey.KeyUserList))
            {
                var array = data.GetSFSArray(RequestKey.KeyUserList);
                var count = Math.Min(array.Count, SeatTotalCount);
                for (int i = 0; i < count; i++)
                {
                    var overInfo = new PludoGameOverItemData(array.GetSFSObject(i));
                    var playerInfo = GetPlayerInfo<PludoPlayerInfo>(overInfo.ServerSeat, true);
                    overInfo.RelatePlayerInfo(playerInfo, RoomInfo.OwnerId);
                    OverData.OverDatas.Add(overInfo);
                }
            }
            else
            {
                for (int i = 0; i < SeatTotalCount; i++)
                {
                    var playerInfo = GetPlayerInfo<PludoPlayerInfo>(i, true);
                    var overInfo = new PludoGameOverItemData(null);
                    overInfo.RelatePlayerInfo(playerInfo, RoomInfo.OwnerId);
                    OverData.OverDatas.Add(overInfo);
                }
            }
            OverData.RoomInfo = RoomInfo;
            var svt = 0L;
            SfsHelper.Parse(data, ConstantData.KeyServerNowTime, ref svt);
            OverData.Time = GetNowTime(svt);
        }

        /// <summary>
        /// 托管状态变化
        /// </summary>
        /// <param name="data"></param>
        public void OnAutoStateChange(ISFSObject data)
        {
            bool tuoGuan = false;
            int seat = ConstantData.IntValue;
            SfsHelper.Parse(data, ConstantData.KeyAuto, ref tuoGuan);
            SfsHelper.Parse(data, RequestKey.KeySeat, ref seat);
            GetPlayer<PludoGamePlayer>(seat, true).AutoStateChange(tuoGuan);
            CheckAutoState();
        }

        /// <summary>
        /// 投票数据
        /// </summary>
        /// <param name="data"></param>
        public void OnGetHandUpDataInGame(ISFSObject data)
        {
            var type = 0;
            var id = 0;
            SfsHelper.Parse(data, RequestKey.KeyType, ref type);
            SfsHelper.Parse(data, RequestKey.KeyId, ref id);
            switch ((HandUpStatus)type)
            {
                case HandUpStatus.Start:
                    StarHandData(new[] { id.ToString() }, HandUpTime);
                    break;
                case HandUpStatus.Agree:
                case HandUpStatus.DisAgree:
                    if (HandData.HandUpDic.ContainsKey(id.ToString()))
                    {
                        var itemData = HandData.HandUpDic[id.ToString()];
                        if (itemData != null)
                        {
                            itemData.SetHandState(type);
                            Facade.EventCenter.DispatchEvent(LoaclRequest.HandUpLocalMessage, itemData);
                        }
                    }
                    else
                    {
                        Debug.LogError("未找到本地投票数据！！！id 为:" + id);
                    }

                    break;
            }
        }

        /// <summary>
        /// 获得重连投票数据
        /// </summary>
        /// <param name="data"></param>
        public void GetHandUpDataRejoin(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyHandsUp))
            {
                var hup = "";
                long svt = 0;
                long hst = 0;
                SfsHelper.Parse(data, ConstantData.KeyHandsUp, ref hup);
                SfsHelper.Parse(data, ConstantData.KeyServerNowTime, ref svt);
                SfsHelper.Parse(data, ConstantData.KeyHupStartTime, ref hst);
                var haveTime = HandUpTime - (svt - hst);
                haveTime = Math.Max(ConstantData.IntValue, haveTime);
                string[] ids = hup.Split(ConstantData.KeyHupSpliteFlag);
                StarHandData(ids, haveTime);
            }
        }

        /// <summary>
        /// 开始投票
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="haveTime"></param>
        private void StarHandData(string[] ids, long haveTime)
        {
            if (ids.Length == ConstantData.IntValue)
            {
                Debug.LogError("投票Id数组为空，请校验");
                return;
            }
            HandData = new HandUpData();
            HandData.HandUpDic = new Dictionary<string, HandUpItemData>();
            var startId = ids[ConstantData.IntValue];
            for (int i = 0; i < SeatTotalCount; i++)
            {
                var playerInfo = GetPlayerInfo<PludoPlayerInfo>(i, true);
                var handItem = new HandUpItemData(playerInfo);
                handItem.SetHandState(ids.Contains(playerInfo.UserId) ? (int)HandUpStatus.Agree : (int)HandUpStatus.Wait);
                HandData.HandUpDic.Add(playerInfo.UserId, handItem);
            }
            HandData.HandUpDic[startId].SetHandState((int)HandUpStatus.Start);
            HandData.SetPathData(haveTime, MainUserInfo.UserId, startId);
            Facade.EventCenter.DispatchEvent(LoaclRequest.HandUpLocalStart, ConstantData.IntValue);
        }
        /// <summary>
        /// 检测局数和房间状态，控制返回按钮显示
        /// </summary>
        private void CheckGameRoundState()
        {
            CouldShowBack = !IsCreateRoom || RoomInfo.CreateRoomInfo.CurRound <= ConstantData.IntValue;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(GameRoundStateAction.WaitExcuteCalls());
            }
        }
        /// <summary>
        /// 检测创建房间状态
        /// </summary>
        private void CheckCreateRoomState()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CreateRoomStateAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 检测托管状态
        /// </summary>
        private void CheckAutoState()
        {
            YxDebug.LogError("MainUserInfo.IsAuto:"+ MainUserInfo.IsAuto);
            AutoState = MainUserInfo.IsAuto;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AutoStateAction.WaitExcuteCalls());
            }
        }


        #endregion
        #region Static Func

        /// <summary>
        /// 获得飞机旋转角度
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to"></param>
        /// <param name="offsetZ"></param>
        /// <returns></returns>
        public static Quaternion GetQuaternion(Vector3 from, Vector3 to, float offsetZ = ConstantData.ValueRoateOffsetZ)
        {
            Vector3 direction = to - from;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += offsetZ;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        /// <summary>
        /// 获得准备去数据ID
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="index">准备区索引</param>
        /// <returns></returns>
        public static int GetReadyId(int color, int index)
        {
            return ConstantData.ValueReadyMapItemBase + color * ConstantData.ValueReadyMapItemColorBase + index;
        }
        /// <summary>
        /// 获得当前服务器时间
        /// </summary>
        /// <param name="time">服务器当前时间</param>
        /// <returns>转换时间</returns>
        public static string GetNowTime(long time)
        {
            DateTime s = new DateTime(1970, 1, 1, 8, 0, 0);
            s = s.AddSeconds(time);
            return s.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion
    }

    /// <summary>
    /// 房间信息
    /// </summary>
    public class RoomInfo
    {
        /// <summary>
        /// 房间基本信息，基于框架
        /// </summary>
        public YxCreateRoomInfo CreateRoomInfo;
        /// <summary>
        /// 房间类型：-1 创建房间 其它 娱乐房
        /// </summary>
        private int _roomType;
        /// <summary>
        /// 房间规则
        /// </summary>
        private string _rule;
        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomName;

        public string Rule
        {
            get
            {
                return _rule;
            }
        }

        public bool IsCreateRoom
        {
            get { return _roomType == ConstantData.CreateRoomType; }
        }

        /// <summary>
        /// 是否为最后一局
        /// </summary>
        public bool IsLastRound
        {
            get
            {
                if (IsCreateRoom)
                {
                    YxDebug.LogError(string.Format("当前局数是:{0},总局数是：{1}", CreateRoomInfo.CurRound, CreateRoomInfo.MaxRound));
                    return CreateRoomInfo.CurRound >= CreateRoomInfo.MaxRound;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public bool IsGameStart
        {
            get
            {
                if (IsCreateRoom)
                {
                    return CreateRoomInfo.CurRound > ConstantData.IntValue;
                }
                else
                {
                    return false;
                }
            }
        }

        public string OwnerId
        {
            get
            {
                if (IsCreateRoom)
                {
                    return CreateRoomInfo.OwnerId.ToString();
                }
                else
                {
                    return ConstantData.IntValue.ToString();
                }
            }
        }

        public RoomInfo(ISFSObject data)
        {
            Parse(data);
        }

        private void Parse(ISFSObject data)
        {
            SfsHelper.Parse(data, ConstantData.KeyRoomType, ref _roomType);
            SfsHelper.Parse(data, ConstantData.KeyRule, ref _rule);
        }
    }


}
