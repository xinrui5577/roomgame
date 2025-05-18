using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Tool;
using Assets.Scripts.Game.BaiTuan.skin02;

namespace Assets.Scripts.Game.BaiTuan
{
    public class BtwGameData : YxGameData
    {
        public string CToken;
        public YxBaseUserInfo CurrentBanker = new YxBaseUserInfo();
        public bool BeginBet = false;
        public long CurrentCanInGold = -1;
        public int MiniApplyBanker = 500000;
        public long ThisCanInGold = 0;

        public bool IsMusicOn = true;

        public List<string> RadioList;
        public List<GameObject> CurrentChipList = new List<GameObject>(); //所有筹码
        public List<GameObject> CurrentCardList = new List<GameObject>();//所有牌
        public GameObject[] LeftCardArray = new GameObject[2];
        public GameObject[] DownCardArray = new GameObject[2];
        public GameObject[] RightCardArray = new GameObject[2];
        public GameObject[] TopCardArray = new GameObject[2];
        public List<GameObject> CurrentBankerList = new List<GameObject>();
        public List<GameObject> CurrentPlayerList = new List<GameObject>();
        public GameObject[] CurrentShowChip;
        public int BankSeat;
        public BtwPlayer BankerPlayer;
        /// <summary>
        /// 是否正在游戏
        /// </summary>
        public bool IsGameing = false;

        private int _showGoldRate = 1;

        //押注钱数
        public int[] ZNumber;

        /// <summary>
        /// 显示筹码的面值
        /// </summary>
        public int ShowGoldRate
        {
            set { _showGoldRate = value > 0 ? value : 1; }
            get { return _showGoldRate; }
        }

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
            MiniApplyBanker = gameInfo.ContainsKey("bankLimit") ? gameInfo.GetInt("bankLimit") : MiniApplyBanker;

            ProgressCtrl02 temp = App.GetGameManager<BtwGameManager>().ProgressCtrl as ProgressCtrl02;
            if (temp != null)
                temp.SetBankerLimitLabel(MiniApplyBanker);
        }
    }
}