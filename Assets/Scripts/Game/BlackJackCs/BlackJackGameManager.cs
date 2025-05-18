using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class BlackJackGameManager : YxGameManager
    {
        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public List<PokerCard> PublicPokers = new List<PokerCard>();

        protected override void OnStart()
        {
            base.OnStart();
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;


            //限制最大40帧
            Application.targetFrameRate = 40;
            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Facade.Instance<MusicManager>().Play("background");
        }
      
    
        //public BjPlayerPanel[] BjSeatSort;

        /// <summary>
        /// 本家的座位号,有服务器获得的服务器座位
        /// </summary>
        [HideInInspector]
        public int SelfSeat = -1;

        
        /// <summary>
        /// 自己的对象
        /// </summary>
        [HideInInspector]
        public BjPlayerPanel SelfPlayer;

        [HideInInspector]
        public List<GameObject> Bets = new List<GameObject>();

        /// <summary>
        /// 结算的总时长
        /// </summary>
        [HideInInspector]
        public int ResultCd = 0;

        /// <summary>
        /// 临时使用的SoundKey,以后会集成到大厅框架中
        /// </summary>
        [HideInInspector]
        public string SoundKey;

       
        public BetBtnMgr BetBtnMgr;
        public SpeakMgr SpeakMgr;
        public DealPokerMgr DealPokerMgr;
        public BetMgr BetMgr;
        public ResultMgr ResultMgr;
        //public ClockCD ClockCd;

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<BlackJackGameData>();
            //初始化自己的信息
            if(gameInfo.ContainsKey("user"))
            {
                ISFSObject selfInfo = gameInfo.GetSFSObject(RequestKey.KeyUser);
                
                SelfSeat = selfInfo.GetInt(RequestKey.KeySeat);
                SelfPlayer = gdata.GetPlayer<BjPlayerPanel>(SelfSeat, true);
            
                SelfPlayer.NameLabel.Color = Color.yellow;
                

               
                bool isGaming = gdata.IsGameing;
                bool isReady = selfInfo.GetBool("state");
                SelfPlayer.SetReadyBtnActive(!isGaming && !isReady);
                SelfPlayer.SetReadyMarkActive(!isGaming && isReady );
            }


            //初始化其他玩家信息
            if(gameInfo.ContainsKey("users"))
            {
                //初始化每个玩家的信息
                ISFSArray users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
                foreach (ISFSObject user in users)
                {
                    int seat = user.GetInt(RequestKey.KeySeat);
                    var someonePanel = gdata.GetPlayer<BjPlayerPanel>(seat, true);

                    someonePanel.gameObject.SetActive(true);
                    someonePanel.SetReadyBtnActive(false);                      //隐藏其他玩家的准备按钮
                    someonePanel.SetReadyMarkActive(user.GetBool("state") && !user.ContainsKey("bet") && !user.ContainsKey("cards"));     //设置玩家的准备状态

                    if (user.ContainsKey("bet"))
                    {
                        someonePanel.BetMoney = user.GetInt("bet");
                    }

                    if(user.ContainsKey("cards"))
                    {
                        int[] cards = user.GetIntArray("cards");
                        DealPokerMgr.DealPokerNoAnim(seat, cards);
                    }
                }

                if(gameInfo.ContainsKey("bankcards"))
                {
                    int[] bankCards = gameInfo.GetIntArray("bankcards");
                    DealPokerMgr.DealPokerNoAnim(gdata.BjBankerBanker, bankCards);
                }
            }
        }


        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {

            BetBtnMgr.SetAddBetBtns(gameInfo.GetSFSObject("cargs2"));

            if (gameInfo.ContainsKey("state"))
            {
                App.GetGameData<BlackJackGameData>().IsGameing = gameInfo.GetBool("playing");
            }

            if (gameInfo.ContainsKey("showGoldRate"))
            {
                App.GetGameData<BlackJackGameData>().ShowGoldRate = gameInfo.GetInt("showGoldRate");
            }
        }

     
        public override void GameResponseStatus(int type, ISFSObject data)
        {

            var gdata = App.GetGameData<BlackJackGameData>();

            if (!gdata.IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }

            switch ((GameRequestType)type)
            {
                case GameRequestType.StartAnte:

                    DealPokerMgr.CleanPokers();        //清理掉上一把没有清理掉的手牌
                    //ClockCd.BeginCountDown(data.GetInt("cd"));
                    YxClockManager.BeginToCountDown(data.GetInt("cd"));
                    SpeakMgr.BetViewActive(true);

                    //游戏开始,隐藏准备按键和标志
                    var anteUsers = gdata.PlayerList;
                    foreach (var yxBaseGamePlayer in anteUsers)
                    {
                        var anteUser = (BjPlayerPanel)yxBaseGamePlayer;
                        anteUser.SetReadyBtnActive(false);
                        anteUser.SetReadyMarkActive(false);
                    }
                  break;


                case GameRequestType.EndAnte:

                    SpeakMgr.ShowNothing();
                    YxClockManager.StopToCountDown();
                    break;


                case GameRequestType.Ante:

                    int anteSeat = data.GetInt("seat");
                    int anteGold = data.GetInt("gold");
                    gdata.GetPlayer<BjPlayerPanel>(anteSeat, true).PlayerBet(anteGold);
                    if (anteSeat == SelfSeat && anteGold > 0)
                    {
                        gdata.IsPlaying = true;
                        BetBtnMgr.CheckColdClickAnte();
                    }
                    break;


                case GameRequestType.Allocate:

                    if (data.ContainsKey("cards"))
                    {
                        ISFSArray allocateArray = data.GetSFSArray("cards");

                        foreach (ISFSObject player in allocateArray)
                        {
                            int[] allocateSeat = { player.GetInt("seat") };
                            int[] allowcateCards = player.GetIntArray("cards");
                            DealPokerMgr.EnqueueCardInfo(allocateSeat, allowcateCards);
                            DealPokerMgr.BeginDealCards();      //开始发牌
                        }
                    }

                    if (data.ContainsKey("bankcards"))
                    {
                        int[] albankerCards = data.GetIntArray("bankcards");

                        DealPokerMgr.EnqueueCardInfo(gdata.BjBankerBanker, albankerCards);
                        DealPokerMgr.BeginDealCards();
                    }

                    if (data.ContainsKey("card"))
                    {
                        int cardSeat = data.GetInt("seat");
                        DealPokerMgr.EnqueueCardInfo(cardSeat, data.GetInt("card"));
                        gdata.GetPlayer<BjPlayerPanel>(cardSeat, true).StopCountDown();

                        if (gdata.IsMyTurn(cardSeat))
                        {
                            SpeakMgr.DoubleBtnEnable(false);
                            SpeakMgr.InsuranceBtnEnable(false);
                        }

                    }
                    SpeakMgr.ShowNothing();
                    break;


                case GameRequestType.Result:
                    gdata.IsGameing = false;
                    gdata.IsPlaying = false;
                    ResultMgr.InitResultView(data);
                    ResultMgr.ShowResultView();

                    if (data.ContainsKey("users"))
                    {
                        ISFSArray resultUsers = data.GetSFSArray("users");
                        foreach (ISFSObject resultUser in resultUsers)
                        {
                            int resultSeat = resultUser.GetInt("seat");
                            var panel = gdata.GetPlayer<BjPlayerPanel>(resultSeat, true);
                            if (panel.Info == null)
                                continue;
                            panel.Coin = resultUser.GetLong("ttgold");

                            if (resultSeat == SelfSeat)
                            {
                                ResultMgr.InitResultView(resultUser);
                            }
                        }
                    }
                    Invoke("ResetGame", 6);
                    break;


                case GameRequestType.Speak:

                    int speakSeat = data.GetInt("seat");
                    float speakCd = data.GetInt("cd");
                    gdata.GetPlayer<BjPlayerPanel>(speakSeat, true).BeginCountDown(speakCd);

                    if (speakSeat != SelfSeat)
                    {
                        YxDebug.Log(" === no my self === " + SelfSeat);
                        SpeakMgr.ShowNothing();
                        break;
                    }

                    SpeakMgr spkMgr = SpeakMgr;
                    spkMgr.SpeakViewActive(true);

                    //显示双倍和保险按键
                    if (data.ContainsKey("addrate"))
                    {
                        //双倍按钮的显示和是否能按
                        spkMgr.DoubleBtn.SetActive(data.GetBool("addrate"));
                        spkMgr.DoubleBtnEnable(SelfPlayer.BetMoney / 2 <= gdata.GetSelfPanel().Coin);

                        //保险按钮的显示和是否能按
                        spkMgr.InsuranceBtn.SetActive(data.GetBool("insurance"));
                        spkMgr.InsuranceBtnEnable(SelfPlayer.BetMoney <= gdata.GetSelfPanel().Coin);
                    }

                    break;


                case GameRequestType.Stand:

                    int standSeat = data.GetInt("seat");
                    gdata.GetPlayer<BjPlayerPanel>(standSeat).StopCountDown();
                    if (standSeat == SelfSeat)
                    {
                        SpeakMgr.ShowNothing();
                    }

                    break;


                case GameRequestType.AddRate:

                    if (data.ContainsKey("seat"))
                    {
                        int addRateSeat = data.GetInt("seat");
                        gdata.GetPlayer<BjPlayerPanel>(addRateSeat).BuyDouble(data.GetInt("gold"));
                        if (addRateSeat == SelfSeat)
                        {
                            SpeakMgr.SpeakViewActive(true);
                            SpeakMgr.DoubleBtnEnable(false);
                        }
                    }
                    break;

                case GameRequestType.BankerAllocate:

                    SpeakMgr.ShowNothing();

                    if (data.ContainsKey("cards"))
                    {
                        gdata.BjBankerBanker.ShowBankerCard0(data.GetIntArray("cards")[0]);
                        DealPokerMgr.DealAllPoker();
                    }

                    //庄家要牌
                    if (data.ContainsKey("card"))
                    {
                        DealPokerMgr.EnqueueCardInfo(gdata.BjBankerBanker, data.GetInt("card"));
                        DealPokerMgr.BeginDealCards();
                    }

                    break;

                case GameRequestType.Insurance:

                    if (data.ContainsKey("seat"))
                    {
                        int insuranceSeat = data.GetInt("seat");
                        gdata.GetPlayer<BjPlayerPanel>(insuranceSeat).BuyInsurance(data.GetInt("gold"));

                        if (insuranceSeat == SelfSeat)
                        {
                            SpeakMgr.SpeakViewActive(true);
                            SpeakMgr.InsuranceBtnEnable(false);
                        }
                    }

                    break;
            }
        }


        /// <summary>
        /// 游戏数据重置
        /// </summary>
        /// <param name="init">是否直接清理玩家手牌,不播放动画</param>
        public virtual void Reset(bool init = false)
        {
            PublicPokers = new List<PokerCard>();

            DealPokerMgr.Reset();
            SpeakMgr.Reset();
            BetMgr.Reset();
            YxClockManager.StopToCountDown();

            var gdata = App.GetGameData<BlackJackGameData>();
            var playerList = gdata.PlayerList;

            foreach(GameObject bet in Bets)
            {
                Destroy(bet);
            }
            Bets.Clear();

            if (init)
            {
                YxDebug.LogError("Reset but && Init");
                foreach (var yxBaseGamePlayer in playerList)
                {
                    var player = (BjPlayerPanel) yxBaseGamePlayer;
                    player.Reset();
                }
                foreach (PokerCard publicPoker in PublicPokers)
                {
                    Destroy(publicPoker.gameObject);
                }
                PublicPokers.Clear();
                gdata.BjBankerBanker.Reset();
            }
            else
            {
                foreach (var yxBaseGamePlayer in playerList)
                {
                    var player = (BjPlayerPanel) yxBaseGamePlayer;
                    player.ThrowPokersWithAnim();
                    player.Reset();
                }
                var banker = gdata.BjBankerBanker;
                banker.ThrowPokersWithAnim();
                banker.Reset();
            }
            SelfPlayer.SetReadyBtnActive(true);
        }

        protected void ResetGame()
        {
            CancelInvoke("ResetGame");
      
            Reset();
            ResultMgr.HideResultView();
        }
  
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            App.GetGameData<BlackJackGameData>().IsGameInfo = false;
            //初始化房间信息
            InitRoom(gameInfo);

            //初始化玩家信息
            InitUser(gameInfo);

            App.GetGameData<BlackJackGameData>().IsGameInfo = true;

            YxWindowManager.HideWaitFor();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            OnGetGameInfo(gameInfo);
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            App.GameData.GetPlayer<BjPlayerPanel>(localSeat).OnUserReady();
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            var gdata = App.GetGameData<BlackJackGameData>();
            gdata.IsGameing = true;
            var playerList = gdata.PlayerList;
            int len = playerList.Length;
            for (int i = 0; i < len; i++)
            {
                ((BjPlayerPanel) playerList[i]).OnGameBegin();
            }
        }
    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        /// <summary>
        /// 0.空
        /// </summary>
        None = 0,
        /// <summary>
        /// 1.申请上庄
        /// </summary>
        ApplyBanker = 1,

        /// <summary>
        /// 2.申请下庄
        /// </summary>
        ApplyQuit = 2,

        /// <summary>
        /// 3.换庄
        /// </summary>
        BankerChange = 3,

        /// <summary>
        /// 4.开始下注
        /// </summary>
        StartAnte = 4,
        /// <summary>
        /// 5.结束下注
        /// </summary>
        EndAnte = 5,
        /// <summary>
        /// 6.下注
        /// </summary>
        Ante = 6,
        /// <summary>
        /// 7.发牌
        /// </summary>
        Allocate = 7,
        /// <summary>
        /// 8.结算
        /// </summary>
        Result = 8,
        /// <summary>
        /// 12.指定玩家说话
        /// </summary>
        Speak = 12,
        /// <summary>
        /// 13.要牌
        /// </summary>
        Hit = 13,
        /// <summary>
        /// 14.停止要牌
        /// </summary>
        Stand = 14,
        /// <summary>
        /// 15.加倍,买双倍,发送交互给服务器
        /// </summary>
        AddRate = 15,
        /// <summary>
        /// 16.庄家发牌
        /// </summary>
        BankerAllocate = 16,
        /// <summary>
        /// 17.保险,买保险发送给服务器
        /// </summary>
        Insurance = 17,
    }
}
