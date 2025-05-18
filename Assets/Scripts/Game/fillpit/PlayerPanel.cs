using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.fillpit
{
    public class PlayerPanel : YxBaseGamePlayer
    {
       
        /// <summary>
        /// 玩家头像显示对象
        /// </summary>
        public YxBaseTextureAdapter UserIcon = null;
        
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker = null;
      
        /// <summary>
        /// 下注显示
        /// </summary>
        public UILabel BetLabel = null;
     
        /// <summary>
        /// 扑克的位置
        /// </summary>
        public Transform[] PokersTrans = null;

        /// <summary>
        /// 玩家胜利动画
        /// </summary>
        public GameObject WinAnimation;

        /// <summary>
        /// 咪牌标记
        /// </summary>
        public GameObject RubMark;

        [HideInInspector]
        public string Ip = string.Empty;
 
        public PanelPlayerType PanelPlayerType;



        /// <summary>
        /// 记录玩家的状态
        /// 1.下注 3.弃牌,6.跟注,7.踢,8.不踢
        /// </summary>
        [HideInInspector]
        public int PlayerType = -1;

        /// <summary>
        /// 是否有GPS信息
        /// </summary>
        [HideInInspector]
        public bool HasGpsInfo = false;

        /// <summary>
        /// 显示分差
        /// </summary>
        [HideInInspector]
        public bool ShowPointDif = false;

        public CardPoint CardPoint;

        public GameObject BankIcon;

        protected int OpenCardPoint;

        protected int OpenCardMaxPoint;

        /// <summary>
        /// 设置明牌的点数
        /// </summary>
        /// <param name="point"></param>
        public void SetShownCardsPoint(int point)
        {
            if (PlayerType == 3)
                return;
            OpenCardPoint = point;
            CardPoint.SetOpenCardsPoint(point);
        }


        [HideInInspector]
        public int BetMoney;
        /// <summary>
        /// 玩家下的筹码集合
        /// </summary>
        [HideInInspector]
        public List<Bet> Bets = new List<Bet>();

        /// <summary>
        /// 玩家下注
        /// </summary>
        public void PlayerBet(int money, bool changeRoomGold = true, int depth = 99)
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
            Coin -= money;
            var betMgr = App.GetGameManager<FillpitGameManager>().BetMgr;
            GameObject[] bets = betMgr.CreatBetArray(money, 9, transform, depth); //在翻倍场中,会出现下5个注的情况，再此多预留一个，烂底翻倍场出现9的情况


            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.transform.parent = betMgr.BetParent;
                betMgr.BetParent.GetComponent<UIPanel>().depth = 4; //为下注筹码在牌上方飞出,设置层级关系

                bet.BeginMove(
                    bet.transform.localPosition, bet.GetTableRandomPos(),
                    i*App.GetGameData<FillpitGameData>().BetSpeace, BetFinishedType.None,
                    () =>
                    {
                        BetLabel.gameObject.SetActive(true);
                        bet.transform.parent = betMgr.BetParent;
                        betMgr.BetParent.GetComponent<UIPanel>().depth = 2; //将层级重置回正常,为发牌时牌在桌面筹码上方飞出
                    });
                Bets.Add(bet);
            }

            ShowBetLabel();

            Facade.Instance<MusicManager>().Play("bet"); //播放声音

            if (changeRoomGold)
            {
                gameUserInfo.RoomCoin -= money;
            }
        }

        public void SetBankIcon(bool active)
        {
            if (BankIcon)
            {
                BankIcon.SetActive(active);
            }
        }

        public void SetRubMark(bool active)
        {
            if (RubMark == null) return;
            RubMark.SetActive(active);
        }

        public void ShowBetLabel()
        {
            BetLabel.gameObject.SetActive(true);
            BetLabel.text = YxUtiles.ReduceNumber(BetMoney);//App.GetGameData<GlobalData>().GetShowGold(BetMoney);
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
      
        /// <summary>
        /// 当玩家重连进游戏
        /// </summary>
        public void OnPlayerRejoinRoom()
        {
            Coin -= BetMoney;
        }

        

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            BetMoney = 0;
            UserBetPoker.Reset();
            OpenCardPoint = -1;
            OpenCardMaxPoint = -1;

            HidePointLabels();

            BetLabel.text = "";
            BetLabel.gameObject.SetActive(false);
            PanelPlayerType.Reset();

            SetRubMark(false);

            PlayerType = -1;

            WinAnimation.SetActive(false);
            HideGameType();
            HidePointLabels();
            SetBankIcon(false);
            Color color=Color.white;
            SetFoldCardColor(color);
        }


        /// <summary>
        /// 隐藏分数
        /// </summary>
        public void HidePointLabels()
        {
            if (CardPoint == null) return;
            CardPoint.SetCardPointActive(false);
        }

        /// <summary>
        /// 通过名字显示玩家状态
        /// </summary>
        /// <param name="type">当前玩家状态,对应游戏状态</param>
        /// <param name="betgold"></param>
        public virtual void PlayTypeSound(GameRequestType type, int betgold = 0)
        {
            switch (type)
            {
                case GameRequestType.BetSpeak:
                    PlaySound("BetSpeak");      //只有一个，不用随机
                    break;

                case GameRequestType.Bet:
                    break;

                case GameRequestType.Fold:
                    PlaySound("Fold" + Random.Range(1, 4));  //三个声音随机一个
                    break;

                case GameRequestType.FollowSpeak:
                    PlaySound("Follow" + Random.Range(1, 4));       //三个声音随机
                    break;
                case GameRequestType.KickSpeak:
                    PlaySound("KickSpeak" + Random.Range(1, 3));    //两个声音随机
                    break;

                case GameRequestType.NotKick:
                    PlaySound("NotKick" + Random.Range(1, 4));      //三个声音随机
                    break;
                  
                case GameRequestType.BackKick:
                    
                    PlaySound("BackKick" + Random.Range(1, 3));     //两个声音随机
                    break;
            }
        }

        public void ShowGameType(GameRequestType type)
        {
            PanelPlayerType.ShowGameType(type);
        }



        /// <summary>
        /// 隐藏玩家状态窗口
        /// </summary>
        public void HideGameType()
        {
            PanelPlayerType.HideGameType();
        }

        /// <summary>
        /// 玩家播放声音(区别性别)
        /// </summary>
        /// <param name="clipName">声音名字</param>
        public virtual void PlaySound(string clipName)
        {
            Facade.Instance<MusicManager>().Play(string.Format("{0}{1}", clipName, Info.SexI));
        }

        internal virtual void SetReadyBtnActive(bool active)
        {
            
        }

        internal virtual void SetStartBtnActive(bool active)
        {

        }
    
        internal virtual void OnGameStart()
        {
            ReadyStateFlag.SetActive(false);
            SetReadyBtnActive(false);
            SetStartBtnActive(false);
        }

        public virtual void OnPlayerReady()
        {
            ReadyState = true;
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsRoomGame && gdata.IsPlayed) return;
            PlaySound("Ready");
        }

        public virtual void OnGameResult(ISFSObject resultItem)
        {
            Coin = resultItem.GetLong("ttgold");
            ReadyState = false;
        }

        

        /// <summary>
        /// 显示牌差值
        /// </summary>
        /// <param name="maxPoint"></param>
        public void SetMaxPoint(int maxPoint)
        {
            OpenCardMaxPoint = maxPoint;

            if (CardPoint != null)
                CardPoint.SetMaxPoint(maxPoint);
        }

        public void ShowPointLabel()
        {
            if (CardPoint == null) return;
            CardPoint.SetCardPointActive(true);
        }

        public virtual void PlayerWin()
        {
            WinAnimation.SetActive(true);
        }

        public virtual void Speak(GameRequestType rt,int betgold = 0)
        {
            switch (rt)
            {
                case GameRequestType.Fold:
                    if (CardPoint != null)
                    {
                        CardPoint.SetCardPointActive(false);
                    }
                    ReadyState = false;
                    PlayerType = 3;
                    Color color = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1);
                    SetFoldCardColor(color);
                    break;
            }

            PlayTypeSound(rt, betgold);
            ShowGameType(rt);
        }
        void SetObjActive(GameObject obj, bool active)
        {
            if (obj != null) obj.SetActive(active);
        }

        public void SetFoldCardColor(Color color)
        {
            foreach (var pokersTran in PokersTrans)
            {
                var child = pokersTran.GetComponentInChildren<PokerCard>();
                if (child)
                {
                    child.SetColor(color);
                }
            }
        }

        public virtual void SetAllCardPoint(int point)
        {
            
        }

        public virtual void SetSeePokerBtnActive(bool active)
        {
            
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
        /// 1.下注
        /// </summary>
        Bet = 1,      
        /// <summary>
        /// 3.弃牌
        /// </summary>
        Fold = 3,
        /// <summary>
        /// 4.反踢
        /// </summary>
        BackKick = 4,
        /// <summary>
        /// 5.跟注
        /// </summary>
        FollowSpeak = 5,
        /// <summary>
        /// 6.踢
        /// </summary>
        KickSpeak = 6, 
        /// <summary>
        /// 7.不踢
        /// </summary>
        NotKick = 7,

    }
}
