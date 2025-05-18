using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     CountDown.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:        	点数提示
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class CountDown :BaseMono 
    {
        #region UI Param
        [Tooltip("显示父级")]
        public GameObject ShowParent;

        [Tooltip("圈")]
        public UISprite Round;

        [Tooltip("点数")]
        public YxBaseLabelAdapter Count;

        [Tooltip("结果图片")]
        public UISprite NoticeSprite;
        #endregion

        #region Data Param
        [Tooltip("Cd显示延迟时间")]
        public float CdShowDelayTime=2;
        [Tooltip("数字刷新时间间隔")]
        public float NumFrameTime = 1;
        [Tooltip("显示分数结果阶段")]
        public List<EventDelegate> OnInitShow;

        [Tooltip("下注时间处理")]
        public List<EventDelegate> OnBetState;
        [Tooltip("停止下注处理")]
        public List<EventDelegate> OnEndState;

        [Tooltip("显示牌阶段处理")]
        public List<EventDelegate> OnShowCardState;

        [Tooltip("显示比牌结果阶段")]
        public List<EventDelegate> OnCardsResultShow;

        [Tooltip("显示分数结果阶段")]
        public List<EventDelegate> OnScoreResultShow;
        #endregion

        #region Local Data
        /// <summary>
        ///每秒执行的百分比
        /// </summary>
        private float _percent;
        /// <summary>
        /// 冷却时间
        /// </summary>
        private int _cdTime;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.Cd, OnGetCdInfo);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqBeginBet,OnBetTime);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqEndBet, OnEndBet);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqGiveCards, OnCardShow);
            Facade.EventCenter.AddEventListeners<LocalRequest, TrendData>(LocalRequest.SingleRecord, OnSingleRecord);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqResult, OnScore);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.Init, OnInit);
        }

        #endregion


        #region Function

        /// <summary>
        /// 获得Cd时间
        /// </summary>
        /// <param name="cdTime"></param>
        private void OnGetCdInfo(int cdTime)
        {
            bool state= cdTime != 0;
            ShowParent.TrySetComponentValue(state);
            if (!state)
            {
                return;
            }
            Round.fillAmount = 1;
            _cdTime = cdTime;
            _percent = ((float) 1/cdTime)/50;
            CancelInvoke("CountDownGo");
            CancelInvoke("SpriteAccountGo");
            var countDownTime = CdShowDelayTime - NumFrameTime;
            if (countDownTime<0)
            {
                countDownTime = 0;
            }
            InvokeRepeating("CountDownGo", countDownTime, NumFrameTime);
            InvokeRepeating("SpriteAccountGo",CdShowDelayTime,0.02f);
        }

        private void CountDownGo()
        {
            var time = _cdTime--;
            if(time<0)
            {
                CancelInvoke("CountDownGo");
            }
            else
            {
                Count.TrySetComponentValue(time);
            }
            
        }

        private void SpriteAccountGo()
        {
            Round.fillAmount -= _percent;
            if (Round.fillAmount<=0)
            {
                CancelInvoke("SpriteAccountGo");
            }
        }

        /// <summary>
        /// 投注时间提示
        /// </summary>
        /// <param name="bet"></param>
        private void OnBetTime(int bet)
        {
            StartCoroutine(OnBetState.WaitExcuteCalls());
            Invoke("WaitPlayBeginSound", 1f);
        }

        private void WaitPlayBeginSound()
        {
            CancelInvoke("WaitPlayBeginSound");
            Facade.Instance<MusicManager>().Play(ConstantData.KeySoundBeginBet);
        }

        /// <summary>
        /// 结束下注
        /// </summary>
        /// <param name="bet"></param>
        private void OnEndBet(int bet)
        {
            StartCoroutine(OnEndState.WaitExcuteCalls());
            Invoke("WaitPlayEndSound", 2f);
        }

        private void WaitPlayEndSound()
        {
            CancelInvoke("WaitPlayEndSound");
            Facade.Instance<MusicManager>().Play(ConstantData.KeySoundEndBet);
        }

        /// <summary>
        /// 显示牌阶段
        /// </summary>
        /// <param name="state"></param>
        private void OnCardShow(int state)
        {
            StartCoroutine(OnShowCardState.WaitExcuteCalls());
        }

        /// <summary>
        /// 单独的回放消息显示
        /// </summary>
        /// <param name="data"></param>
        private void OnSingleRecord(TrendData data)
        {
            var win = data.Win;
            NoticeSprite.TrySetComponentValue(win);
            StartCoroutine(OnCardsResultShow.WaitExcuteCalls());
        }

        /// <summary>
        /// 显示分数结果
        /// </summary>
        /// <param name="num"></param>
        private void OnScore(int num)
        {
            StartCoroutine(OnScoreResultShow.WaitExcuteCalls());
        }

        /// <summary>
        /// 初始化消息
        /// </summary>
        /// <param name="num"></param>
        private void OnInit(int num)
        {
            StartCoroutine(OnInitShow.WaitExcuteCalls());
        }
        #endregion
    }
}
