using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.brnn3d
{
    public class Brnn3dGameData : YxGameData
    {
        public int BetMoney;//玩家下注的钱数
        public int BetPos;//玩家在桌子上下注的位置
        public int BetPosSelf;//自己下注的位置
        public int DicNum;//骰子的点数
        public ISFSArray BankList;//庄家列表信息
        public bool IsBet;//是否可以下注
        public int Frame = 1;//记录游戏运行多少局
        public int Bundle;//记录游戏玩了多少把
        public int Bkmingold;//上庄最低限制
        /// <summary>
        /// 筹码类型
        /// </summary>
        public int CoinType = 0;
        public int SendCardPosition;//发牌的位置

        public ISFSArray Cards;//获得牌组信息
        public ISFSArray Nn;//获得开奖时各个座位的信息

        public int B;//庄家的座位

        public bool IsOut=true;
        


        public Dictionary<int, Transform> PaiAllShow = new Dictionary<int, Transform>(); //牌是否全显示完全


        public bool IsKai = true;//路子信息界面是否是开的

        /// <summary>
        /// 提示信息文档
        /// </summary>
        public string ShangZhuangMoneyLos = "玩家申请上庄最少{0}金币";
        public string NextXiaZuang = "您当前正在庄上，下局开始前自动下庄！！！";

        public bool BeginBet = false;
        public int CurrentCanInGold = -1;
        public int MiniApplyBanker = 50000;
        public long ThisCanInGold = 0;

        public bool IsMusicOn = true;

        public List<string> RadioList;

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            BankList = gameInfo.GetSFSArray("bankers");
            B = gameInfo.GetInt("banker");
            Bkmingold = gameInfo.GetInt("bkmingold");
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var user = new Brnn3DUserInfo();
            user.Parse(userData);
            return user;
        } 
    }
}
