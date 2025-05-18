using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.pludo.View;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGamePlayer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-18
 *描述:        	飞行棋玩家类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoGamePlayer : YxBaseGamePlayer,IColorItem
    {
        #region UI Param
        [Tooltip("房主标识")]
        public GameObject RoomOwnerFlag;
        [Tooltip("托管标识")]
        public GameObject AutoFlag;
        [Tooltip("星星数量")]
        public YxBaseLabelAdapter StarScore;
        [Tooltip("颜色显示相关")]
        public PludoColorItem PludoColorItem;
        [Tooltip("控制")]
        public PludoControlCenter ControlCenter;
        [Tooltip("Cd显示")]
        public YxBaseLabelAdapter CdLabel;
        [Tooltip("Cd进度图片")]
        public UISprite CdPercentSprite;
        [Tooltip("等待打骰子的效果")]
        public TweenScale RollDicTween;
        #endregion

        #region Data Param
        /// <summary>
        /// 星星分数格式
        /// </summary>
        public string StarScoreFormat = "x{0}";
        /// <summary>
        ///  Cd进度
        /// </summary>
        public float CdPercent { private set; get; }

        #endregion

        #region Local Data
        /// <summary>
        /// 玩家数据
        /// </summary>
        protected PludoPlayerInfo CurInfo;
        /// <summary>
        /// 控制
        /// </summary>
        protected PludoControl Control;
        /// <summary>
        /// 当前阶段剩余时间
        /// </summary>
        private float _haveTime;
        /// <summary>
        /// 当前结算全部时间（总时间）
        /// </summary>
        private long _curStateAllTime;
        /// <summary>
        /// 倒计时协程
        /// </summary>
        private Coroutine _cdCoroutine;

        /// <summary>
        /// 可以选择飞机
        /// </summary>
        protected bool CanChoosePlane
        {
            get {return CurInfo.RollDiceData.PlaneIds.Count > 0; }
        }

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            if (ControlCenter == null)
            {
                var centers=Resources.FindObjectsOfTypeAll<PludoControlCenter>();
                if (centers.Length>ConstantData.IntValue)
                {
                    ControlCenter = centers[ConstantData.IntValue];
                }
            }
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.PlaneInit, OnPlaneInit);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.FreshStartNum, OnFreshStartNum);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.PlaneInit, OnPlaneInit);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.FreshStartNum, OnFreshStartNum);
            base.OnDestroy();
        }

        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            CurInfo = GetData<PludoPlayerInfo>();
            if (CurInfo != null)
            {
                Control=ControlCenter.RelatePlayer(CurInfo.PlayerColor);
                Control.name = CurInfo.PlayerColor.ToString();
                if (Control)
                {
                    Control.UpdateView(CurInfo);
                }
                StarScore.TrySetComponentValue(string.Format(StarScoreFormat, CurInfo.StarNum));
                if (PludoColorItem)
                {
                    var colorData=new ColorItemData(CurInfo.PlayerColor);
                    PludoColorItem.UpdateView(colorData);
                }
                ShowOpWithCd();
                RoomOwnerFlag.TrySetComponentValue(CurInfo.IsOwner);
                AutoFlag.TrySetComponentValue(CurInfo.IsAuto);
            }
        }
        #endregion

        #region Function

        protected virtual void ShowOpWithCd()
        {
            _haveTime = CurInfo.StateHaveTime;
            _curStateAllTime = CurInfo.StateAllTime;
            switch (CurInfo.Status)
            {
                case PludoPlayerStatus.RollDic:
                    break;
                case PludoPlayerStatus.ChoosPlane:
                    ShowRollDiceResult();
                    break;
                case PludoPlayerStatus.Sleep:
                    CdReset();
                    break;
            }
            if(_haveTime<=0)
            {
                _haveTime = 0;
                return;
            }
            ShowCdWithTime();
        }

        /// <summary>
        /// 进入空闲状态
        /// </summary>
        public virtual void OnPlayerSleep()
        {
            CurInfo.Status = PludoPlayerStatus.Sleep;
            CurInfo.SetStateTime(ConstantData.IntValue);
            ShowOpWithCd();
        }

        /// <summary>
        /// 当前玩家切换为可打骰子状态
        /// </summary>
        public virtual void OnWaitRollDice()
        {
            CurInfo.Status = PludoPlayerStatus.RollDic;
            CurInfo.SetStateTime(ConstantData.IntValue);
            ShowOpWithCd();
            if (RollDicTween)
            {
                RollDicTween.ResetToBeginning();
                RollDicTween.PlayForward();
            }
        }

        /// <summary>
        /// 玩家选择飞机移动
        /// </summary>
        /// <param name="canChoosePlane">可以选择飞机</param>
        public virtual void OnPlayerChoosePlane(bool canChoosePlane = true)
        {
            CurInfo.Status = PludoPlayerStatus.ChoosPlane;
            CurInfo.SetStateTime(canChoosePlane ? ConstantData.IntValue : PludoGameData.ChoosePlaneCdTime);
            ShowOpWithCd();
        }

        /// <summary>
        /// 显示骰子结果
        /// </summary>
        public virtual void OnShowRollResult()
        {
            OnPlayerChoosePlane(CanChoosePlane);
        }

        public void AutoStateChange(bool state)
        {
            CurInfo.IsAuto = state;
            AutoFlag.TrySetComponentValue(CurInfo.IsAuto);
        }

        /// <summary>
        /// 飞机数据初始化
        /// </summary>
        /// <param name="data">数据，本交互无意义</param>
        private void OnPlaneInit(int data)
        {
            UpdateView(Data);
        }

        /// <summary>
        /// 刷新星星数量
        /// </summary>
        /// <param name="data"></param>
        private void OnFreshStartNum(int data)
        {
            StarScore.TrySetComponentValue(string.Format(StarScoreFormat, CurInfo.StarNum));
        }

        public void Reset(int data)
        {
            CurInfo = null;
        }

        /// <summary>
        /// 显示Cd
        /// </summary>
        private void ShowCdWithTime()
        {
            if (_cdCoroutine!=null)
            {
                StopCoroutine(_cdCoroutine);
            }
            if (CdPercentSprite)
            {
                _cdCoroutine = StartCoroutine(ShowCd());
            }
        }

        IEnumerator ShowCd()
        {
            while (_haveTime>=0)
            {
                CdPercentSprite.fillAmount = _haveTime/_curStateAllTime;
                CdLabel.TrySetComponentValue(_haveTime.ToString(CultureInfo.InvariantCulture));
                yield return new WaitForSeconds(ConstantData.ValueCdSecond);
                _haveTime-= ConstantData.ValueCdSecond;
            }
        }

        /// <summary>
        /// 重置CD
        /// </summary>
        private void CdReset()
        {
            if (_cdCoroutine != null)
            {
                StopCoroutine(_cdCoroutine);
            }
            if (CdPercentSprite)
            {
                CdPercentSprite.fillAmount = ConstantData.IntValue;
            }
            CdLabel.TrySetComponentValue(string.Empty);
        }

        #region 

        #endregion


        #endregion
        /// <summary>
        /// 显示打骰子结果
        /// </summary>
        protected virtual void ShowRollDiceResult()
        {
            CurInfo.RollDiceData.FromPos = transform.parent.localPosition;
            Facade.EventCenter.DispatchEvent(LoaclRequest.RollDiceCallBack, CurInfo.RollDiceData);
        }

        public string GetSpriteName(ItemColor color, ColorItemUiType uiType, string format = ConstantData.DefColorFormat)
        {
            return string.Format(format, color, uiType);
        }

        public void SetColorItem(UISprite colorSprite, string spriteName)
        {
            colorSprite.TrySetComponentValue(spriteName);
        }

        public void SetColorItems(List<UISprite> colorSprites, List<ColorItemUiType> uiTypes)
        {
            var count = Math.Min(colorSprites.Count, uiTypes.Count);
            var curColor = (ItemColor)Info.Seat;
            for (int i = 0; i < count; i++)
            {
                GetSpriteName(curColor, uiTypes[i]);
            }
        }
    }

    public class PludoPlayerInfo : YxBaseGameUserInfo
    {
        /// <summary>
        /// 星星数量
        /// </summary>
        public int StarNum
        {
            get
            {
                var count = 0;
                foreach (var plane in _planes)
                {
                    if (plane.Value.CheckPlaneState(EnumPlaneStatus.Finish))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public bool IsOwner {private set; get; }

        /// <summary>
        /// 游戏状态
        /// </summary>
        private int _status;
        /// <summary>
        /// 玩家颜色
        /// </summary>
        private int _playerColor;
        /// <summary>
        /// 可使用遥控骰子次数
        /// </summary>
        private int _controlDiceTime;

        /// <summary>
        /// 遥控骰子剩余次数
        /// </summary>
        public int ControlDiceTime
        {
            get {return _controlDiceTime; }
        }

        /// <summary>
        /// 飞机
        /// </summary>
        private Dictionary<int,PludoPlaneData> _planes=new Dictionary<int,PludoPlaneData>();

        /// <summary>
        /// 飞机数据
        /// </summary>
        public Dictionary<int, PludoPlaneData> PlaneDic { get { return _planes; } }

        /// <summary>
        /// 打骰子数据
        /// </summary>
        public RollDiceData RollDiceData;

        /// <summary>
        /// 托管状态
        /// </summary>
        public bool IsAuto;

        public PludoPlayerStatus Status
        {
            get
            {
                return (PludoPlayerStatus)_status;
            }
            set
            {
                _status = (int)value;
            }
        }

        /// <summary>
        /// 玩家颜色
        /// </summary>
        public int PlayerColor
        {
            get {return _playerColor;}
        }

        /// <summary>
        /// 阶段剩余时间
        /// </summary>
        public float StateHaveTime { set; get; }

        /// <summary>
        /// 阶段完整时间
        /// </summary>
        public long StateAllTime { set; get;}

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            RollDiceData=new RollDiceData();
            SfsHelper.Parse(userData, ConstantData.KeyPlayerStatus, ref _status);
            SfsHelper.Parse(userData, ConstantData.KeyColor, ref _playerColor);
            SfsHelper.Parse(userData, ConstantData.KeyControlDiceTime, ref _controlDiceTime);
            if (userData.ContainsKey(ConstantData.KeyPlaneInfo))
            {
                var array = userData.GetSFSArray(ConstantData.KeyPlaneInfo);
                SetPlanesInfo(array);
            }

            if (userData.ContainsKey(ConstantData.KeyAuto))
            {
                SfsHelper.Parse(userData, ConstantData.KeyAuto, ref IsAuto);
            }
            if (userData.ContainsKey(ConstantData.KeyChoosePlane))
            {
                var planes = userData.GetIntArray(ConstantData.KeyChoosePlane);
                RollDiceData.OneMoreTime = false;
                RollDiceData.PlaneIds = planes.ToList();
                YxDebug.LogError("可以选择的飞机数量是："+ planes.Length);
                foreach (var id in planes)
                {
                    YxDebug.LogError(id);
                }
                RollDiceData.QuickModel = true;
            }
        }

        /// <summary>
        /// 检测是否为开房者
        /// </summary>
        /// <param name="ownerId"></param>
        public void CheckRoomOwner(string ownerId)
        {
            IsOwner = UserId.Equals(ownerId);
        }

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="finishTime"></param>
        public void SetStateTime(long finishTime=ConstantData.IntDefValue)
        {
            switch (Status)
            {
                case PludoPlayerStatus.RollDic:
                    StateAllTime = PludoGameData.RollDicCdTime;
                    StateHaveTime = StateAllTime - finishTime;
                    break;
                case PludoPlayerStatus.ChoosPlane:
                    StateAllTime = PludoGameData.ChoosePlaneCdTime;
                    StateHaveTime = StateAllTime - finishTime;
                    break;
                case PludoPlayerStatus.Sleep:
                    StateAllTime = ConstantData.IntValue;
                    StateHaveTime= ConstantData.IntValue;
                    break;
            }
        }
        /// <summary>
        /// 设置遥控骰子次数
        /// </summary>
        /// <param name="time"></param>
        public void SetControlDiceTime(int time)
        {
            _controlDiceTime = time;
        }

        /// <summary>
        /// 设置飞机信息
        /// </summary>
        public void SetPlanesInfo(ISFSArray planes)
        {
            var count = planes.Count;
            for (int i = 0; i < count; i++)
            {
                var data = planes.GetSFSObject(i);
                PludoPlaneData planeData = new PludoPlaneData(data,_playerColor);
                var key = planeData.DataId;
                if (_planes.ContainsKey(planeData.DataId))
                {
                    _planes[key] = planeData;
                }
                else
                {
                    _planes.Add(key, planeData);
                }
            }
        }

        /// <summary>
        /// 设置选择信息
        /// </summary>
        /// <param name="chooseId">可选飞机ID</param>
        /// <param name="point">骰子点数</param>
        public void SetChooseInfo(List<int> chooseId,int point)
        {

        }
    }

    /// <summary>
    /// 飞行棋玩家状态
    /// </summary>
    public enum PludoPlayerStatus
    {
        Sleep,                           //无操作状态
        RollDic,                         //摇骰子状态
        ChoosPlane,                      //选择飞机状态
    }
}
