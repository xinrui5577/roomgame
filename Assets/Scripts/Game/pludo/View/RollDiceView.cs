using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     RollDiceView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-03
 *描述:        	游戏操作
 *              
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class RollDiceView : PludoFreshView 
    {
        #region UI Param
        [Tooltip("打骰子动画")]
        public UISpriteAnimation RollDiceAni;
        [Tooltip("打骰子图片")]
        public UISprite RollSrpite;
        [Tooltip("打骰子位置")]
        public TweenPosition RollTweenPos;
        [Tooltip("云动画")]
        public UISpriteAnimation CloudAni;
        [Tooltip("云图片")]
        public UISprite CloudSprite;
        [Tooltip("再来一次")]
        public TweenAlpha OneMoreTime;
        [Tooltip("消耗钻石数量Tips")]
        public UILabel CostLabel;
        [Tooltip("消耗钻石数量（按钮消耗显示）")]
        public UILabel ShowCost;
        #endregion

        #region Data Param
        [Tooltip("点数音频格式")]
        public string PointSoundFormat = "Point{0}";
        [Tooltip("再来一次显示延迟时间")]
        public float OneMoreTimeWaitShowTime = 1f;
        [Tooltip("遥控骰子显示延迟时间")]
        public float ControlDiceWaitShowTime = 0.5f;
        [Tooltip("显示遥控骰子事件")]
        public List<EventDelegate> OnShowControlDiceAction=new List<EventDelegate>();
        #endregion

        #region Local Data
        /// <summary>
        /// 本地数据
        /// </summary>
        private RollDiceData _curData;
        /// <summary>
        /// 再来一次等待yield
        /// </summary>
        private WaitForSeconds _waiteForOneMoreTimeYield;
        /// <summary>
        /// 遥控骰子等待yield
        /// </summary>
        private WaitForSeconds _waiteForControlDiceYield;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            _waiteForOneMoreTimeYield = new WaitForSeconds(OneMoreTimeWaitShowTime);
            _waiteForControlDiceYield=new WaitForSeconds(ControlDiceWaitShowTime);
            Facade.EventCenter.AddEventListeners<LoaclRequest,RollDiceData>(LoaclRequest.RollDiceCallBack,UpdateView);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.NeedToReady, ResetOnRoundFinish);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.FreshCostNumAction, SetConsumeNum);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, RollDiceData>(LoaclRequest.RollDiceCallBack, UpdateView);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.NeedToReady, ResetOnRoundFinish);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.FreshCostNumAction, SetConsumeNum);
            base.OnDestroy();
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData=Data as RollDiceData;
            if (_curData!=null)
            {
                Reset();
                if (_curData.QuickModel)
                {
                    OnPlayRollDiceFinish();
                }
                else
                {
                    if (_curData.UseControlDice)
                    {
                        if (gameObject.activeInHierarchy)
                        {
                            if (_showControlCor!=null)
                            {
                               StopCoroutine(_showControlCor);
                            }
                            _showControlCor=StartCoroutine(ShowControlDice());
                        }
                    }
                    else
                    {
                        MoveDice();
                    }
                }
            }
        }

        #endregion

        #region Function

        private Coroutine _showControlCor;
        IEnumerator ShowControlDice()
        {
            if (_curData.ShowAni)
            {
                CostLabel.TrySetComponentValue(YxUtiles.ReduceNumber(_curData.CostCashNum));
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnShowControlDiceAction.WaitExcuteCalls());
                }
            }
            ConstantData.PlaySoundBySex(_curData.Sex, ConstantData.KeyControlDice);
            yield return _waiteForControlDiceYield;
            MoveDice();
        }

        /// <summary>
        /// 骰子移动
        /// </summary>
        private void MoveDice()
        {
            RollTweenPos.@from = _curData.FromPos;
            RollTweenPos.to = _curData.ToPos;
            RollTweenPos.PlayForward();
            RollDiceAni.Play();
        }

        private Coroutine _waitForOneMoreTimeCor;
        public void OnPlayRollDiceFinish()
        {
            RollDiceAni.Pause();
            RollSrpite.TrySetComponentValue(string.Format(ConstantData.KeyRollDicePointFormat, _curData.ShowPoint));
            RollSrpite.MakePixelPerfect();
            if (!_curData.QuickModel)
            {
                ConstantData.PlaySoundBySex(_curData.Sex,string.Format(PointSoundFormat, _curData.ShowPoint));
                CloudAni.Play();
                if (_curData.OneMoreTime)
                {
                    if (gameObject.activeInHierarchy)
                    {
                        if (_waitForOneMoreTimeCor != null)
                        {
                            StopCoroutine(_waitForOneMoreTimeCor);
                        }
                        _waitForOneMoreTimeCor = StartCoroutine(WaitShowOneMoreTime());
                    }
                }
            }
            else
            {
                CloudSprite.spriteName = ConstantData.KeyDefaultSpriteName;
            }
        }

        /// <summary>
        /// 再来一次相关显示
        /// </summary>
        /// <returns></returns>
        IEnumerator  WaitShowOneMoreTime()
        {
            OneMoreTime.ResetToBeginning();
            yield return _waiteForOneMoreTimeYield;
            OneMoreTime.PlayForward();
            ConstantData.PlaySoundBySex(_curData.Sex, ConstantData.KeyRollAgin);
        }

        /// <summary>
        /// 小结算后重置打骰子相关显示
        /// </summary>
        /// <param name="data"></param>
        private void ResetOnRoundFinish(int data)
        {
            Reset();
        }

        /// <summary>
        /// 设置消耗显示
        /// </summary>
        /// <param name="consume"></param>
        private void SetConsumeNum(int consume)
        {
            ShowCost.TrySetComponentValue(YxUtiles.ReduceNumber(consume));
        }

        private void Reset()
        {
            if (_showControlCor != null)
            {
                StopCoroutine(_showControlCor);
            }
            if (_waitForOneMoreTimeCor != null)
            {
                StopCoroutine(_waitForOneMoreTimeCor);
            }
            RollDiceAni.Pause();
            CloudAni.Pause();
            RollSrpite.TrySetComponentValue(ConstantData.KeyDefaultSpriteName);
            RollTweenPos.ResetToBeginning();
            CloudSprite.spriteName = ConstantData.KeyDefaultSpriteName;
        }
        #endregion
    }

    /// <summary>
    /// 打骰子数据
    /// </summary>
    public class RollDiceData
    {
        /// <summary>
        /// 起始点
        /// </summary>
        public Vector3 FromPos;
        /// <summary>
        /// 终点
        /// </summary>
        public Vector3 ToPos;
        /// <summary>
        /// 显示点数
        /// </summary>
        public int ShowPoint;
        /// <summary>
        /// 再来一次
        /// </summary>
        public bool OneMoreTime;
        /// <summary>
        /// 飞机数据（可操作飞机ID）
        /// </summary>
        public List<int> PlaneIds;
        /// <summary>
        /// 快速模式，重连显示
        /// </summary>
        public bool QuickModel;
        /// <summary>
        /// 剩余钻石数量
        /// </summary>
        public int HaveCashNum;
        /// <summary>
        /// 使用遥控骰子
        /// </summary>
        public bool UseControlDice;
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex;
        /// <summary>
        /// 是否显示动画（使用遥控骰子的人，显示扣除钻石提示）
        /// </summary>
        public bool ShowAni;
        /// <summary>
        /// 消耗钻石数量
        /// </summary>
        public int CostCashNum;

        public RollDiceData()
        {
            PlaneIds=new List<int>();
        }
    }
}
