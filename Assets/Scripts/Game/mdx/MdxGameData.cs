using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.mdx
{
    public class MdxGameData:YxGameData
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
        public int[] MaxBet;

        public MdxPlayer CurrentBanker;

        public int[] DiceVals;

        /// <summary>
        /// 上庄下限金额
        /// </summary>
        public int MinApplyBanker
        {
            set { _minApplyBanker = value <= 0 ? 3000000 : value; }
            get { return _minApplyBanker; }
        }

        private int _minApplyBanker = 3000000;

        /// <summary>
        /// 上庄上限金额
        /// </summary>
        public int MaxApplyBanker
        {
            set { _maxApplyBanker = value <= 0 ? int.MaxValue : value; }
            get { return _maxApplyBanker; }
        }

       private int _maxApplyBanker = int.MaxValue;

        /// <summary>
        /// 自己是不是庄家
        /// </summary>
        public bool IsBanker
        {
            get
            {
                return CurrentBanker != null && CurrentBanker.Info != null && BankSeat == SelfSeat;
            }
        }

        /// <summary>
        /// 是否能退出游戏
        /// </summary>
        public bool CouldOut
        {
            get
            {
                return !IsGaming && !IsBanker;
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

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new MdxUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }
    } 
}

