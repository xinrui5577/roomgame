using System;
using System.Collections.Generic;
using Assets.Scripts.Game.GangWu.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.GangWu
{
    public class PlayerPanel : YxBaseGamePlayer
    {
        /// <summary>
        /// 玩家头像显示对象
        /// </summary>
        public YxBaseTextureAdapter UserIcon;
        
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker;
     
        /// <summary>
        /// 下注显示
        /// </summary>
        public UILabel BetLabel;
        

        /// <summary>
        /// 扑克的位置信息
        /// </summary>
        public Transform[] PokersTrans;

        public GameObject Mask;


        private int _betMoney;

        /// <summary>
        /// 玩家下注金额
        /// </summary>
        public int BetMoney
        {
            set
            {
                _betMoney = value;
                BetLabel.text = YxUtiles.ReduceNumber(value);
                BetLabel.gameObject.SetActive(value > 0);
            }
            get { return _betMoney; }
        }

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
                //如果状态值不是枚举中的成员，发送错误报告
                if (!Enum.IsDefined(typeof(PlayerGameType), value))
                    throw new ArgumentOutOfRangeException("value");

                _curGameType = value;
                
            }
            get { return _curGameType; }
        }
        

        /// <summary>
        /// 玩家下的筹码集合
        /// </summary>
        [HideInInspector]
        public List<Bet> Bets = new List<Bet>();
        /// <summary>
        /// 玩家下注
        /// </summary>
        public void PlayerBet(int money,bool changeRoomG = true)
        {
            if (Info == null)
            {
                YxDebug.LogError("没有用户信息!!");
                return;
            }

            if (money <= 0)
            {
                return;
            }
            BetMoney += money;

            GameObject[] bets = App.GetGameManager<GangWuGameManager>().BetMgr.CreatBetArray(money, 4, transform);

            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.BeginMove(bet.transform.localPosition, BetLabel.transform.localPosition,
                    i*App.GetGameData<GangwuGameData>().BetSpeace, BetFinishedType.Hide,
                    () => { BetLabel.gameObject.SetActive(true); });
                Bets.Add(bet);
            }

            BetLabel.text = YxUtiles.ReduceNumber(BetMoney);//App.GetGameData<GlobalData>().GetShowGold(BetMoney);
            Facade.Instance<MusicManager>().Play("bet");

            if (changeRoomG)
            {
                Coin -= money;
            }
            
            if (Coin <= 0)
            {
                Facade.Instance<MusicManager>().Play("allin");
                CurGameType = PlayerGameType.AllIn;
            }
           
            RefreshPanel();
        }

        /// <summary>
        /// 刷新界面信息
        /// </summary>
        public void RefreshPanel()
        {
            BetLabel.gameObject.SetActive(false);
            if (Info == null)
            {
                GameType.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 显示赢的牌型
        /// </summary>
        /// <param name="winCards"></param>
        public void ShowWinPoker(int[] winCards)
        {

            Color dark = new Color(0.7f, 0.7f, 0.7f);

            foreach (int cardValue in winCards)
            {
                if (UserBetPoker.LeftPoker != null && UserBetPoker.LeftPoker.Id == cardValue)
                {
                    UserBetPoker.LeftPoker.Selected();
                }

                if (UserBetPoker.RightPoker != null && UserBetPoker.RightPoker.Id == cardValue)
                {
                    UserBetPoker.RightPoker.Selected();
                }
            }

            if (UserBetPoker.LeftPoker != null && !UserBetPoker.LeftPoker.IsSelect)
            {
                UserBetPoker.LeftPoker.SetColor(dark);
            }
            if (UserBetPoker.RightPoker != null && !UserBetPoker.RightPoker.IsSelect)
            {
                UserBetPoker.RightPoker.SetColor(dark);
            }
            var publicPokers = App.GetGameManager<GangWuGameManager>().PublicPokers;
            foreach (var pokerCard in publicPokers)
            {
                foreach (int cardValue in winCards)
                {
                    if (pokerCard.Id == cardValue)
                    {
                        pokerCard.Selected();
                    }
                }
            }

            foreach (var pokerCard in publicPokers)
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
       

        public void ShowGameType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                GameType.gameObject.SetActive(false);
                return;
            }
            GameType.spriteName = "ut_" + typeName;
            GameType.gameObject.SetActive(true);
            Facade.Instance<MusicManager>().Play(typeName);
        }


        /// <summary>
        /// 设置玩家的准备状态
        /// </summary>
        /// <param name="state">准备状态</param>
        internal virtual void SetPlayerReadyState(bool state)
        {
            ReadyState = state;
            Mask.SetActive(!state);
        }


        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            BetMoney = 0;
            foreach(Bet bet in Bets)
            {
                if (bet != null && bet.gameObject != null)
                    Destroy(bet.gameObject);
            }
            Bets.Clear();
            CurGameType = PlayerGameType.None;
            WinEffect.SetActive(false);
            GameType.gameObject.SetActive(false);
            RefreshPanel();
            BetLabel.gameObject.SetActive(false);
            UserBetPoker.Reset();
            SetPlayerReadyState(false);
        }

    }
    /// <summary>
    /// 玩家游戏状态
    /// </summary>
    public enum PlayerGameType
    {
        /// <summary>
        /// 0.正常状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 1.让牌
        /// </summary>
        SeePoker,      
        /// <summary>
        /// 2.跟注
        /// </summary>
        Call,          
        /// <summary>
        /// 3.弃牌
        /// </summary>
        Fold,           
        /// <summary>
        /// 4.全下
        /// </summary>
        AllIn,          
        /// <summary>
        /// 5.加注
        /// </summary>
        AddBet,
        /// <summary>
        /// 6.跟注
        /// </summary>
        Follow,
    }
}
