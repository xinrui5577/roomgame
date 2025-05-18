using UnityEngine;
using System.Collections;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.brtbsone
{
    public class BrttzGameData : YxGameData
    {
        public long ResultBnakerTotal = 0;
        public int ResultUserTotal = 0;
        public long CurrentCanInGold = -1;
        public long ThisCanInGold = 0;
        public bool BeginBet = false;
        public BrttzPlayer BankerPlayer;
        public int BankSeat;
        public int MiniApplyBanker = 50000;

        public YxBaseUserInfo CurrentBanker = new YxBaseUserInfo();
        public List<GameObject> CurrentBankerList = new List<GameObject>();
        //押注钱数
        public int[] ZNumber;

        public bool IsBanker
        {
            get
            {
                var info = BankerPlayer.Info;
                return info != null && info.Seat == GetPlayerInfo().Seat;
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

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            if (gameInfo.ContainsKey("bankLimit"))
                MiniApplyBanker = gameInfo.GetInt("bankLimit");
        }
    }
}