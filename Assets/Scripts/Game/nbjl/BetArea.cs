using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.Rendering;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using Random = System.Random;

/*===================================================
 *文件名称:     BetArea.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-21
 *描述:        	桌面下注区域信息
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class BetArea : RecycleWindow 
    {
        #region UI Param
        [Tooltip("下注区域类型")]
        public string AreaType;
        [Tooltip("状态位")]
        public TrendBit TypeState;
        [Tooltip("倍率信息")]
        public YxBaseLabelAdapter AnteLabel;
        [Tooltip("总下注信息")]
        public YxBaseLabelAdapter TotalBet;
        [Tooltip("本人下注信息")]
        public YxBaseLabelAdapter SelfBet;
        [Tooltip("下注位置")]
        public Transform BetTrans;
        #endregion

        #region Data Param
        [Tooltip("随机范围X")]
        public int RandomRangeX = 40;
        [Tooltip("随机范围Y")]
        public int RandomRangeY = 40;
        [Tooltip("下注点击")]
        public List<EventDelegate> OnBetClick;
        [Tooltip("对应区域胜利")]
        public List<EventDelegate> OnAreaWin;
        [Tooltip("显示分数面板")]
        public List<EventDelegate> OnShowScoreResult;
        /// <summary>
        /// 下注区域的筹码
        /// </summary>
        public List<YxView> Items { set; get; }
        /// <summary>
        /// 倍率区域显示格式
        /// </summary>
        public string RateFormat = "(1:{0})";
        #endregion

        #region Local Data
        /// <summary>
        /// 总分数
        /// </summary>
        private int _totalScore;
        /// <summary>
        /// 本人分数
        /// </summary>
        private int _selfScore;

        /// <summary>
        /// 随机种子
        /// </summary>
        private int _randowSeed;
        #endregion

        #region Life Cycle
        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, int>(AreaType, OnGetBetInfo);
            Facade.EventCenter.AddEventListeners<LocalRequest, TrendData>(LocalRequest.SingleRecord, OnSingleRecord);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqResult, OnReset);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.Init, Init);
            Items = new List<YxView>();
            GetRandomSeed(Environment.TickCount + (int) TypeState);
            Reset();
        }
        #endregion

        #region Function

        /// <summary>
        /// 获得下注赔率
        /// </summary>
        /// <param name="betCount"></param>
        private void OnGetBetInfo(int betCount)
        {
            AnteLabel.TrySetComponentValue(string.Format(RateFormat, betCount));
        }

        private int GetRandomSeed(int lastSeed)
        {
            var tick = Environment.TickCount;
            while ((lastSeed+tick)>int.MaxValue)
            {
                _randowSeed = (int) TypeState + tick;
                return _randowSeed;
            }
            _randowSeed = Environment.TickCount + lastSeed;
            return _randowSeed;
        }

        public Vector3 ChipPosVec
        {
            get
            {
                var pos = BetTrans.localPosition;
                Random rand = new Random(GetRandomSeed(_randowSeed));
                int numX = rand.Next(-RandomRangeX, RandomRangeX);
                int numY = rand.Next(-RandomRangeY, RandomRangeY);
                pos =new Vector3(pos.x+ numX, pos.y+ numY);
                return pos;
            }
        }

        /// <summary>
        /// 点击下注区域
        /// </summary>
        public void OnClickBetArea()
        {
            var manager = App.GetGameManager<NbjlGameManager>();
            if (manager)
            {
                if (manager.CheckBetState())
                {
                    StartCoroutine(OnBetClick.WaitExcuteCalls());
                    if (manager.CheckBet())
                    {
                        App.GetRServer<NbjlGameServer>().UserBet(AreaType, manager.CurrentChipValue);
                    }
                }
            }
        }

        /// <summary>
        /// 单独的回放消息显示
        /// </summary>
        /// <param name="data"></param>
        private void OnSingleRecord(TrendData data)
        {
            var state = data.ResultStates;
            if ((state&(int)TypeState)!=0)
            {
                StartCoroutine(OnAreaWin.WaitExcuteCalls());
            }
        }
        /// <summary>
        /// 显示结果时重置
        /// </summary>
        private void OnReset(int a)
        {
            StartCoroutine(OnShowScoreResult.WaitExcuteCalls());
        }

        private void Init(int i)
        {
            Reset();
        }

        public void ShowBetNum(int total,int self)
        {
            _totalScore += total;
            _selfScore += self;
            SelfBet.TrySetComponentValue(YxUtiles.ReduceNumber(_selfScore));
            TotalBet.TrySetComponentValue(YxUtiles.ReduceNumber(_totalScore));
        }

        /// <summary>
        /// 重置分数
        /// </summary>
        public void Reset()
        {
            _totalScore=0;
            _selfScore =0;
            SelfBet.Text(0);
            TotalBet.Text(0);
            
        }

        #endregion
    }

    /// <summary>
    /// 下注数据
    /// </summary>
    public class BetData:IRecycleData
    {
        /// <summary>
        /// 下注金币
        /// </summary>
        public int Gold { get;private set; }
        /// <summary>
        /// 座位号
        /// </summary>
        public int Seat { get; private set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; private set; }

        public BetData(ISFSObject data)
        {
            Gold = data.GetInt(ConstantData.KeyGold);
            Seat = data.GetInt(ConstantData.KeySeat);
            Position = data.GetUtfString(ConstantData.KeyBetPosition);
        }

        public BetData(string position,int seat,int gold)
        {
            Position = position;
            Seat = seat;
            Gold = gold;
        }
    }

    /// <summary>
    /// 下注历史，用于重复下注防护力
    /// </summary>
    public class BetHistory
    {
        /// <summary>
        /// 局数
        /// </summary>
        public int Round;
        /// <summary>
        /// 本局下注
        /// </summary>
        public int[] CurBets;
        /// <summary>
        /// 历史下注
        /// </summary>
        public int[] HistoryBets;


        public BetHistory()
        {
            CurBets=new int[5];
            HistoryBets=new int[5];
        }

        public void InitHistory()
        {
            Array.Copy(CurBets, HistoryBets, CurBets.Length);
            CurBets=new int[5];
        }

        public int Sum()
        {
            return HistoryBets.Sum();
        }
    }
}
