using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     NbjlPlayer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:        	百家乐玩家。因为玩家信息数据结构变化，新增n局累计下注金额与n局下注累计获胜次数
 *              ，因此，改变玩家结构
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class NbjlPlayer : YxBaseGamePlayer 
    {
        #region UI Param 
        [Tooltip("n局累计下注金额")]
        public YxBaseLabelAdapter AccumulateBet;

        [Tooltip("n局累计下注获胜数量")]
        public YxBaseLabelAdapter AccumulateWinCount;
        [Tooltip("对应排行名次显示")]
        public int RankShowBgNum = 3;
        [Tooltip("排行名次背景格式")]
        public string RankBgFormat = "rank_{0}";
        [Tooltip("排行榜背景")]
        public UISprite RankBg;
        [Tooltip("排行榜名次，文本")]
        public YxBaseLabelAdapter RankNumLabel;
        [Tooltip("其它名次物体")]
        public GameObject OtherRankObj;
        [Tooltip("筹码位置")]
        public GameObject ChipPos;
        [Tooltip("飞字移动效果")]
        public TweenPosition FlyTween;
        [Tooltip("飞字预设")]
        public FlyLabel Prefab;
        [Tooltip("飞字父级物体")]
        public GameObject FlyParent ;
        #endregion
        #region Data Param
        [Tooltip("累计下注格式")]
        public string AccomulateBetFormat="￥{0}";
        [Tooltip("累计胜利格式")]
        public string AccomulateWinFormat = "{0}局";
        [Tooltip("飞字效果")]
        public string FlyLabelForMat = "{0}:{1}";

        public Vector3 ChipPosVec
        {
            get { return ChipPos.transform.localPosition; }
        }

        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            NbjlPlayerInfo playerInfo =Info as NbjlPlayerInfo;
            if(playerInfo==null)
            {
                return;
            }
            var rankNum = playerInfo.RankNum;
            bool inRankNum = rankNum < RankShowBgNum;
            if (inRankNum)
            {
                RankBg.TrySetComponentValue(string.Format(RankBgFormat, rankNum));
            }
            OtherRankObj.TrySetComponentValue(!inRankNum);
            RankNumLabel.TrySetComponentValue((rankNum+1).ToString());
            SetNick(playerInfo.NickM);
            AccumulateBet.TrySetComponentValue(string.Format(AccomulateBetFormat,YxUtiles.ReduceNumber(playerInfo.AccumulateBet)));
            AccumulateWinCount.TrySetComponentValue(string.Format(AccomulateWinFormat,playerInfo.AccumulateWin));
        }

        #endregion

        #region Function

        public void ShowBet(string noticeName,int gold)
        {
            if (FlyTween)
            {
                FlyTween.ResetToBeginning();
                FlyTween.PlayForward();
                if(FlyParent&&Prefab)
                {
                   var obj= FlyParent.AddChild(Prefab.gameObject);
                    obj.SetActive(true);
                    var flyLabel = obj.GetComponent<FlyLabel>();
                    flyLabel.SetLabel(string.Format(FlyLabelForMat,noticeName,YxUtiles.ReduceNumber(gold)));
                }
            }
            if (Info != null)
            {
                Info.CoinA -= gold;
                SetCoin(Info.CoinA);
            }
        }

        /// <summary>
        /// 刷新金币显示数量
        /// </summary>
        public void FreshCoinA()
        {
            SetCoin(Info.CoinA);
        }

        #endregion
    }

    public class NbjlPlayerInfo : YxBaseGameUserInfo,IRecycleData
    {
        /// <summary>
        /// 累计N局下注数量
        /// </summary>
        public int AccumulateBet { get; private set;}
        /// <summary>
        /// 累计N局胜利次数
        /// </summary>
        public int AccumulateWin { get; private set; }

        /// <summary>
        /// 各门下注信息
        /// </summary>
        public int[] BetGolds { get; private set; }

        /// <summary>
        /// 排名
        /// </summary>
        public int RankNum { get; set; }

        public NbjlPlayerInfo(NbjlPlayerInfo info)
        {
            CoinA = info.CoinA;
            Seat = info.Seat;
            NickM = info.NickM;
            SexI = info.SexI;
            AvatarX = info.AvatarX;
        }

        public NbjlPlayerInfo()
        {

        }

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            if (userData.ContainsKey(ConstantData.KeyUserName))
            {
                NickM = userData.GetUtfString(ConstantData.KeyUserName);
            }
            AccumulateBet= userData.GetInt(ConstantData.KeyAccumulateBet);
            AccumulateWin = userData.GetInt(ConstantData.KeyAccumulateWin);
            if (userData.ContainsKey(ConstantData.KeyBetGolds))
            {
                BetGolds = userData.GetIntArray(ConstantData.KeyBetGolds);
            }
        }

        public void FreshDatas(NbjlOnLinePlayerData data)
        {
            AccumulateBet = data.AccumulateBet;
            AccumulateWin = data.AccumulateWin;
            WinCount = data.WinCount;
            Seat = data.Seat;
            CoinA = data.TotalGold;
            NickM = data.UserName;
        }
    }

    public class NbjlOnLinePlayerData
    {
        /// <summary>
        /// 累计N局下注数量
        /// </summary>
        public int AccumulateBet { get; private set; }
        /// <summary>
        /// 累计N局胜利次数
        /// </summary>
        public int AccumulateWin { get; private set; }

        /// <summary>
        /// 胜利总次数
        /// </summary>
        public int WinCount { get; private set; }

        /// <summary>
        /// 座位号
        /// </summary>
        public int Seat { get; private set; }

        /// <summary>
        /// 玩家ID
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// 总金币
        /// </summary>
        public long TotalGold { get; private set; }

        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string UserName { get; private set; }


        public virtual void Parse(ISFSObject onlineData)
        {
            AccumulateBet = onlineData.GetInt(ConstantData.KeyAccumulateBet);
            AccumulateWin = onlineData.GetInt(ConstantData.KeyAccumulateWin);
            WinCount = onlineData.GetInt(ConstantData.KeyWinCount);
            Seat= onlineData.GetInt(ConstantData.KeySeat);
            UserId = onlineData.GetInt(ConstantData.KeyUserId);
            TotalGold = onlineData.GetLong(ConstantData.KeyTotalGold);
            UserName = onlineData.GetUtfString(ConstantData.KeyUserName);
        }
    }
}
