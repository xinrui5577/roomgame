using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     BankerManger.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-27
 *描述:        	庄位
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class BankerInfo : BaseMono 
    {
        #region UI Param
        [Tooltip("庄家信息")]
        public YxBaseGamePlayer PlayerInfo;
        [Tooltip("申请上庄按钮")]
        public UIButton ApplyBankerBtn;
        [Tooltip("申请下庄按钮")]
        public UIButton ApplyDownBtn;

        [Tooltip("上庄限制文本")]
        public YxBaseLabelAdapter BankerLimit;

        #endregion
        #region Data Param
        [Tooltip("系统庄名称")]
        public string SystemBankerName="系统庄家";
        [Tooltip("系统装默认携带金币")]
        public int SystemBankerValue=500000000;
        [Tooltip("系统庄座位号")]
        public int SystemSeat = -1;
        [Tooltip("系统庄性别")]
        public int SystemSex = 0;
        [Tooltip("上庄下限显示格式")]
        public string BankerLaberlFormat = "上庄需要{0}金币";
        [Tooltip("庄家状态变化处理")]
        public List<EventDelegate> OnBankerChange;
        [Tooltip("上庄状态变化处理")]
        public List<EventDelegate> OnApplyChange; 
        #endregion

        #region Local Data
        /// <summary>
        /// 默认庄家信息
        /// </summary>
        private NbjlPlayerInfo _defBankerInfo;
        /// <summary>
        /// 当前玩家是否为庄家
        /// </summary>
        public bool IsBanker
        {
           get; private set;
        }

        /// <summary>
        /// 是否为申请上庄状态
        /// </summary>
        public bool IsApply
        {
            get; private set;
        }

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, NbjlPlayerInfo>(LocalRequest.BankerInfo, OnBankerInfo);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.BankerLimit, OnFreshBankLimit);
            Facade.EventCenter.AddEventListeners<LocalRequest, bool>(LocalRequest.ApplyStateChange, OnApplyStateChange);
            Facade.EventCenter.AddEventListeners<LocalRequest, bool>(LocalRequest.BankerState, OnBankerStateChange);
            SetDefBankerInfo();
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LocalRequest, NbjlPlayerInfo>(LocalRequest.BankerInfo, OnBankerInfo);
            Facade.EventCenter.RemoveEventListener<LocalRequest, int>(LocalRequest.BankerLimit, OnFreshBankLimit);
            Facade.EventCenter.RemoveEventListener<LocalRequest, bool>(LocalRequest.ApplyStateChange, OnApplyStateChange);
            Facade.EventCenter.RemoveEventListener<LocalRequest, bool>(LocalRequest.BankerState, OnBankerStateChange);
            base.OnDestroy();
        }

        #endregion

        #region Function

        /// <summary>
        /// 设置默认庄家信息
        /// </summary>
        private void SetDefBankerInfo()
        {
            _defBankerInfo = new NbjlPlayerInfo();
            _defBankerInfo.NickM = SystemBankerName;
            _defBankerInfo.Seat = SystemSeat;
            _defBankerInfo.SexI = SystemSex;
            _defBankerInfo.CoinA = SystemBankerValue;
        }

        /// <summary>
        /// 刷新信息显示
        /// </summary>
        /// <param name="data"></param>
        private void OnBankerInfo(NbjlPlayerInfo data)
        {
            RefreshInfo(data ?? _defBankerInfo);
        }

        /// <summary>
        /// 申请上庄状态变化
        /// </summary>
        /// <param name="isApply"></param>
        private void OnApplyStateChange(bool isApply)
        {
            IsApply = isApply;
            StartCoroutine(OnApplyChange.WaitExcuteCalls());
        }

        /// <summary>
        /// 上庄状态变化
        /// </summary>
        /// <param name="isBanker"></param>
        private void OnBankerStateChange(bool isBanker)
        {
            IsBanker = isBanker;
            StartCoroutine(OnBankerChange.WaitExcuteCalls());
        }

        /// <summary>
        /// 刷新上庄限制信息
        /// </summary>
        /// <param name="bankLimit">上庄限制</param>
        private void OnFreshBankLimit(int bankLimit)
        {
            BankerLimit.TrySetComponentValue(string.Format(BankerLaberlFormat,YxUtiles.ReduceNumber(bankLimit)));
        }

        /// <summary>
        /// 刷新信息
        /// </summary>
        private void RefreshInfo(NbjlPlayerInfo info)
        {
            var newInfo = new NbjlPlayerInfo(info);
            var curInfo = PlayerInfo.Info;
            if (curInfo == null)
            {
                PlayerInfo.Info = newInfo;
            }
            else
            {
                if (newInfo.NickM != curInfo.NickM || newInfo.CoinA != curInfo.CoinA || newInfo.Seat != curInfo.Seat)
                {
                    PlayerInfo.Info = newInfo;
                }
            }
        }

        /// <summary>
        /// 申请上庄按钮点击
        /// </summary>
        public void OnClickApplyBtn()
        {
            App.GetGameManager<NbjlGameManager>().ApplyBanker();
        }

        /// <summary>
        /// 申请下庄按钮点击
        /// </summary>
        public void OnClickDownBtn()
        {
            App.GetGameManager<NbjlGameManager>().ApplyQuitBanker();
        }

        #endregion
    }
}
