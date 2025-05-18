using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Utils;
using Sfs2X.Entities.Data;
using Assets.Scripts.Game.bjlb.View;
using YxFramwork.Common.Model;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.bjlb
{
    public class BjlGameData:YxGameData
    { 
        public  bool BeginBet = false;
        public  int CurrentCanInGold = -1;
        public  long ThisCanInGold = 0;
        public  long ResultBnakerTotal = 0;
        public  int ResultUserTotal = 0;
        public Vector2 ScreenSize = new Vector2(1334, 750);
        public int BankSeat;
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public bool IsGaming = false;
        
        //玩家选择筹码
        public List<GameObject> Cards = new List<GameObject>();
        //押注钱数
        public int[] ZNumber;

        public TrendData TrendConfig;

        public BjlPlayer CurrentBanker;

        /// <summary>
        /// 自己是不是庄家
        /// </summary>
        public bool IsBanker
        {
            get
            {
                return CurrentBanker != null && CurrentBanker.Info != null && CurrentBanker.Info.Seat == GetPlayerInfo().Seat;
            }
        }

        /// <summary>
        /// 是否能退出游戏
        /// </summary>
        public bool CouldOut
        {
            get
            {
                return !IsGaming && BankSeat != SelfSeat;
            }
        }

        public void SetGameStatus(YxEGameStatus status)
        {
            if (IsBanker)
            {
                GStatus = YxEGameStatus.PlayAndConfine;
                return;
            }

            GStatus = status;
        }

        protected override void OnStart()
        {
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            if (gameInfo.ContainsKey("banker"))
            {
                BankSeat = gameInfo.GetInt("banker");
            }
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new BjlUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        } 
    } 
}

