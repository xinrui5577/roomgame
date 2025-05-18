using System;
using System.Collections.Generic; 
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Assets.Scripts.Game.Texas.Main;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas
{
    public class PlayerPanel : YxBaseGamePlayer
    {
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker; 
        /// <summary>
        /// 下注显示
        /// </summary>
        public UILabel BetLabel;
        /// <summary>
        /// 大盲注显示对象
        /// </summary>
        public GameObject Blinds;
          
        /// <summary>
        /// 玩家状态显示
        /// </summary>
        public UISprite GameType;
        /// <summary>
        /// 赢家特效
        /// </summary>
        public GameObject WinEffect;

        private PlayerGameType _curGameType = PlayerGameType.None;
        /// <summary>
        /// 玩家当前游戏状态
        /// </summary>
        public PlayerGameType CurGameType
        {
            set
            {
                if (!Enum.IsDefined(typeof(PlayerGameType), value))
                    throw new ArgumentOutOfRangeException("value");

                _curGameType = value;
                if (CurGameType != PlayerGameType.None && CurGameType != PlayerGameType.Call)
                {
                    GameType.spriteName = "ut_" + CurGameType;
                    GameType.gameObject.SetActive(true);
                    GameType.MakePixelPerfect();
                }
                else
                    GameType.gameObject.SetActive(false);
            }
            get { return _curGameType; }
        }

        public void HideGameType()
        {
            if (!ReadyState)
                return;
            if (CurGameType == PlayerGameType.AllIn || CurGameType == PlayerGameType.Fold)
                return;
            GameType.gameObject.SetActive(false);
        }
     
	 
        [HideInInspector]
        public long BetMoney;
        /// <summary>
        /// 玩家下的筹码集合
        /// </summary>
        public List<Bet> Bets = new List<Bet>();
        /// <summary>
        /// 玩家下注
        /// </summary>
        public void PlayerBet(long money,bool changeRoomG = true)
        {
            var gameUserInfo = GetData<YxBaseGameUserInfo>();
            if (gameUserInfo == null)
            {
                YxDebug.LogError("没有用户信息!!");
                return;
            }

            if (money <= 0)
            {
                return;
            }

            BetMoney += money;
            var gMgr = App.GetGameManager<TexasGameManager>();
            var bets = gMgr.BetMagr.CreatBetArray(money, 4, transform);
            var gdata = App.GetGameData<TexasGameData>();
            for (var i = 0; i < bets.Length; i++)
            {
                var bet = bets[i].GetComponent<Bet>();
                bet.BeginMove(bet.transform.localPosition, BetLabel.transform.localPosition,
                    i * gdata.BetSpeace, BetFinishedType.Hide,
                    () => { BetLabel.gameObject.SetActive(true); });
                Bets.Add(bet);
            }

            BetLabel.text = YxUtiles.ReduceNumber(BetMoney);
            Facade.Instance<MusicManager>().Play("bet");

            if (changeRoomG)
            {
                RoomCoin -= money;      
            }
            
            if (gameUserInfo.RoomCoin <= 0)
            {
                Facade.Instance<MusicManager>().Play("allin");
                CurGameType = PlayerGameType.AllIn;
            }
            else
            {
                CurGameType = PlayerGameType.Call;
            }
        }
   

        /// <summary>
        /// 显示赢的牌型
        /// </summary>
        /// <param name="winCards"></param>
        public void ShowWinPoker(int[] winCards)
        {

            Color dark = new Color(0.7f, 0.7f, 0.7f);

            UserBetPoker.SelectCards(winCards,dark);

            

            foreach (var pokerCard in App.GetGameManager<TexasGameManager>().PublicPokers)
            {
                foreach (int cardValue in winCards)
                {
                    if (pokerCard.Id == cardValue)
                    {
                        pokerCard.Selected();
                    }
                }
            }

            foreach (var pokerCard in App.GetGameManager<TexasGameManager>().PublicPokers)
            {
                if (!pokerCard.IsSelect)
                {
                    pokerCard.SetColor(dark);
                }
            }
        }
        /// <summary>
        /// 设置层次关系
        /// </summary>
        /// <param name="depth"></param>
        public void SetPlayerDepth(int depth)
        {
            var wis = gameObject.GetComponentsInChildren<UIWidget>(true);
            foreach (UIWidget wi in wis)
            {
                wi.depth += depth;
            }
        }

        public void ShowHandCards(int[] cards)
        {
            UserBetPoker.SetCardsValue(cards);
        }

        public void SelectHandCards(int[] cards,Color unselectedColor)
        {
            UserBetPoker.SelectCards(cards, unselectedColor);
        }

        protected override void SetOnLineState(bool isOnline)
        {
            base.SetOnLineState(ReadyState && isOnline);
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            BetMoney = 0;
            CurGameType = PlayerGameType.None;
            ReadyState = false;
            WinEffect.SetActive(false);
            GameType.gameObject.SetActive(false);
            Blinds.SetActive(false);
            UserBetPoker.Reset();
            BetLabel.text = string.Empty;
            BetLabel.gameObject.SetActive(false);
        }

        protected override void SetReadyState(bool isReady)
        {
            if (ReadyStateFlag == null) return;
            ReadyStateFlag.SetActive(!isReady);
        }
   
    }
    /// <summary>
    /// 玩家游戏状态
    /// </summary>
    public enum PlayerGameType
    {
        None,           //正常状态
        SeePoker,       //让牌
        Call,           //跟注
        Fold,           //弃牌
        AllIn,          //全下
    }
}
