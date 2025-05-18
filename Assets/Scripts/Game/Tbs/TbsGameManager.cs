using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Tbs
{
    public class TbsGameManager : YxGameManager
    {
        public BankerBtnMgr BankerBtnMgr;
        public ChooseGuoMgr ChooseGuoMgr;
        public DiceManager DiceManager;
        public BetManager BetManager;
        public ResultMgr ResultMgr;
        public MenuMgr MenuMgr;
        public HupMgr HupMgr;
        public PokerMgr PokerMgr;
        public TimeMgr TimeMgr;
        public Waring Waring;
        public CheckBankerMgr CbMgr;
        public GameOverMgr GameOverMgr;

        /// <summary>
        /// 锅内金币显示
        /// </summary>
        public UILabel GuoGoldLabel;

        public UILabel BetTotalLabel;

        public UILabel BetTimeLabel;

        public UILabel BankerLimitLabel;

        public UILabel BooldLabel;
        public UILabel RoomId;
        public UILabel CurrentRound;
        /// <summary>
        /// 通杀通赔对象
        /// </summary>
        public GameObject AllKill;
        /// <summary>
        /// 当前局数
        /// </summary>
        private int _currentRound;
        /// <summary>
        /// 最大局数
        /// </summary>
        private int _maxRound;
        /// <summary>
        /// 结算数组
        /// </summary>
        private string[] _result;
        /// <summary>
        /// 总钱数赋值
        /// </summary>
        private string[] _ttGold;
        /// <summary>
        /// 点数信息
        /// </summary>
        private int[] _dianValues;

        private bool _isOpening;
        /// <summary>
        /// 翻牌间隔
        /// </summary>
        public float OpenInterval;
        /// <summary>
        /// 翻牌计时器
        /// </summary>
        private float _openTimer;
        /// <summary>
        /// 翻牌索引
        /// </summary>
        private int _openIndex;
        /// <summary>
        /// 结束翻牌索引
        /// </summary>
        private int _endOpenIndex;

        protected void Start()
        {
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer ? UIRoot.Scaling.Constrained : UIRoot.Scaling.ConstrainedOnMobiles;
            App.GetGameData<TbsGameData>().DealV = new int[4][];
        }
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            YxDebug.Log("gameinfo");

            var gdata = App.GetGameData<TbsGameData>();
            var gserver = App.GetRServer<TbsRemoteController>();

            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //获取房间配置
            var roomId = gameInfo.ContainsKey("rid") ? gameInfo.GetInt("rid") : 0;
            var round = gameInfo.ContainsKey("round") ? gameInfo.GetInt("round") : 0;
            var maxRound = gameInfo.ContainsKey("maxRound") ? gameInfo.GetInt("maxRound") : 0;
            //设置开房的显示
            SetCreatRoomData(roomId, round, maxRound);

            BetManager.SetAnte(gdata.Ante, gdata.AnteRate.Split('#'));

            gdata.GetPlayer(gdata.BankerSeat, true).Coin -= gdata.GuoGold + gdata.Boold;
            //初始化玩家信息
            InitUser(gameInfo);
            //设置庄家
            SetBanker(gdata.BankerSeat, false);
            //设置间隔
            DiceManager.setRollTime(gameInfo.GetInt("rolltime"));
            //自动准备
            gserver.ReadyGame();
            //当前牌数
            int cardNum = gameInfo.ContainsKey("cardNum") ? gameInfo.GetInt("cardNum") : 52;
            //是否在游戏中
            if (gameInfo.ContainsKey("status") && gameInfo.GetInt("status") > (int)GameRequestType.Banker)
            {
                gdata.GStatus=YxEGameStatus.PlayAndConfine;
            }

            if (gameInfo.ContainsKey("opData") && gameInfo.GetSFSObject("opData").GetInt("type") >= (int)GameRequestType.Allocate)
            {
                cardNum += 8;
            }
            //是否需要切牌
            if ((gameInfo.ContainsKey("status") && gameInfo.GetInt("status") > (int)GameRequestType.CutPoker) || cardNum < 52)
            {
                PokerMgr.BeginCut(false, cardNum);
            }
            RefreshLabel();

            //重连逻辑
            if (gameInfo.ContainsKey("rejoin") && gameInfo.GetBool("rejoin"))
            {
                if (gameInfo.ContainsKey("opData") && gameInfo.GetSFSObject("opData").GetInt("type") != (int)GameRequestType.Result)
                {
                    if (gameInfo.GetSFSObject("opData").ContainsKey("cd"))
                    {
                        int cd = gameInfo.GetSFSObject("opData").GetInt("cd");

                        var rtime = (int)(cd - (gameInfo.GetLong("ct") - gameInfo.GetLong("st")));
                        rtime = rtime < 0 ? 0 : rtime;
                        gameInfo.GetSFSObject("opData").PutInt("cd", rtime);
                    }
                    GameResponseStatus(gameInfo.GetSFSObject("opData").GetInt("type"), gameInfo.GetSFSObject("opData"));
                }
            }

            gserver.DealHupInfo(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }
        /// <summary>
        /// 间隔执行
        /// </summary>
        public Queue<ISFSObject> ResponseSfsObjects;
        /// <summary>
        /// 队列条数
        /// </summary>
        protected int ResponseQueueCount;
        /// <summary>
        /// 计时器
        /// </summary>
        protected float Timer;
        /// <summary>
        /// 时间间隔
        /// </summary>
        public float TimeSpace;

        protected void Update()
        {
            OpenPokerUpdate();
            if (ResponseSfsObjects == null || ResponseSfsObjects.Count <= 0)
            {
                Timer = TimeSpace + 1;
                return;
            }
            Timer += Time.deltaTime;
            if (Timer > TimeSpace || ((GameRequestType)ResponseSfsObjects.Peek().GetInt("type")) == GameRequestType.Bet)
            {
                Timer = 0f;
                if ((GameRequestType)ResponseSfsObjects.Peek().GetInt("type") != GameRequestType.Bet)
                {
                    ResponseQueueCount--;
                }
                ExecuteServerResponse(ResponseSfsObjects.Dequeue());
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            if (!App.GetRServer<TbsRemoteController>().HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }

            if (!(BetManager.IsBeginBet && (GameRequestType)type == GameRequestType.Bet))
            {
                if ((GameRequestType)type != GameRequestType.Bet)
                {
                    ResponseQueueCount++;
                }
                ResponseSfsObjects.Enqueue(response);
                if (ResponseQueueCount > 3)
                {
                    App.ReBackLogin();
                }
                return;
            }

            ExecuteServerResponse(response);
        }

        /// <summary>
        /// 执行交互
        /// </summary>
        public void ExecuteServerResponse(ISFSObject data)
        {
            var type = data.GetInt("type");
            var gdata = App.GetGameData<TbsGameData>();
            switch ((GameRequestType)type)
            {
                case GameRequestType.Banker:
                    if (gdata.BankerSeat != -1 && gdata.BankerSeat != data.GetInt("bank"))
                    {
                        if (gdata.GuoGold > 0)
                        {
                            Facade.Instance<MusicManager>().Play("cutpot");
                        }
                        CbMgr.OpenPanel(data.GetUtfString("bankname"), data.GetInt("guogold"), gdata.Boold, data.GetInt("cd"));

                        if (gdata.GetPlayerInfo(gdata.BankerSeat, true) != null)
                        {
                            gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA += data.GetInt("guogold") > 0 ? data.GetInt("guogold") : 0;
                            gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA += gdata.Boold;
                        }
                    }

                    if (data.GetInt("bank") < 0)
                    {
                        gdata.GuoGold = 0;
                        gdata.Boold = 0;
                        RefreshLabel();
                        gdata.BankerSeat = -1;
                        return;
                    }

                    SetBanker(data.GetInt("bank"), false);
                    gdata.GuoGold = data.GetInt("gold");
                    gdata.Boold = gdata.GuoGold;
                    gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA -= gdata.GuoGold;
                    gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA -= gdata.Boold;
                    TimeMgr.BeginCountDown(data.GetInt("cd"), "等待开局");
                    RefreshLabel();
                    break;
                case GameRequestType.SetGuo:

                    ResultMgr.CloseResult();
                    TimeMgr.CloseTime();
                    PokerMgr.HideHandPoker();

                    gdata.IsGameStart = true;

                    gdata.GetPlayer<TbsPlayer>(gdata.BankerSeat, true).IsExit = false;
                    ChooseGuoMgr.ClosePanel();
                    gdata.GuoGold = data.GetInt("gold");
                    GuoGoldLabel.text = YxUtiles.ReduceNumber(gdata.GuoGold);// .ToString();
                    gdata.GStatus = YxEGameStatus.PlayAndConfine;

                    BankerBtnMgr.SetBtn(data.GetInt("status") == 3 ? BankerBtn.CutPoker : BankerBtn.None);

                    if (data.GetInt("status") == 3)
                    {
                        TimeMgr.BeginCountDown(data.GetInt("cd"), "等待庄家切牌",
                            true, data.ContainsKey("svst") ? data.GetLong("svst") : 0, data.ContainsKey("svct") ? data.GetLong("svct") : 0);
                    }
                    break;
                case GameRequestType.CutPoker:
                    PokerMgr.HideHandPoker();
                    gdata.GetPlayer<TbsPlayer>(gdata.BankerSeat, true).IsExit = false;
                    StartCoroutine(PokerMgr.Reset(0.2f, PokerMgr.BeginCut));
                    BankerBtnMgr.SetBtn(BankerBtn.None);
                    TimeMgr.CloseTime();
                    break;
                case GameRequestType.BeginBet:

                    PokerMgr.HideHandPoker();
                    gdata.GetPlayer<TbsPlayer>(gdata.BankerSeat, true).IsExit = false;
                    YxDebug.Log("开始下注!!");
                    DiceManager.DiceBox.gameObject.SetActive(true);
                    DiceManager.BeginTime = Time.time;

                    Facade.Instance<MusicManager>().Play("pleasebet");
                    BetManager.IsBeginBet = true;
                    BankerBtnMgr.SetBtn(BankerBtn.RollDice);

                    TimeMgr.BeginCountDown(data.GetInt("cd"), "请下注,打骰不要",
                        true, data.ContainsKey("svst") ? data.GetLong("svst") : 0, data.ContainsKey("svct") ? data.GetLong("svct") : 0);
                    break;
                case GameRequestType.Bet:
                    PokerMgr.HideHandPoker();
                    AddBet(data.GetInt("gold"), data.GetInt("seat"));
                    break;
                case GameRequestType.StopBet:
                    PokerMgr.HideHandPoker();
                    YxDebug.Log("停止下注!!");
                    BetManager.IsBeginBet = false;
                    TimeMgr.CloseTips();
                    TimeMgr.StopCountTime();
                    DiceManager.SetDiceV(data.GetInt("dice1"), data.GetInt("dice2"));
                    DiceManager.BeginRollDice(2f);
                    BankerBtnMgr.SetBtn(BankerBtn.None);

                    DiceManager.PlayDiceAnim();

                    break;
                case GameRequestType.RollDice:

                    PokerMgr.HideHandPoker();

                    TimeMgr.StopCountTime();
                    DiceManager.SetDiceV(data.GetInt("dice1"), data.GetInt("dice2"));
                    DiceManager.BeginRollDice(2f);
                    BankerBtnMgr.SetBtn(BankerBtn.None);

                    break;
                case GameRequestType.Allocate:
                    PokerMgr.HideHandPoker();

                    ISFSArray sfsa = data.GetSFSArray("cards");
                    for (int i = 0; i < sfsa.Count; i++)
                    {
                        gdata.DealV[i] = sfsa.GetSFSObject(i).GetIntArray("cards");
                        gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.SetCardV(gdata.DealV[i][0], gdata.DealV[i][1]);
                    }
                    Deal();
                    TimeMgr.BeginCountDown(data.GetInt("cd"), "等待开牌",
                        true, data.ContainsKey("svst") ? data.GetLong("svst") : 0, data.ContainsKey("svct") ? data.GetLong("svct") : 0);
                    break;
                case GameRequestType.OpenPoker:
                    //重连发牌
                    if (data.ContainsKey("cards"))
                    {
                        PokerMgr.HideHandPoker();

                        ISFSArray cards = data.GetSFSArray("cards");
                        for (int i = 0; i < cards.Count; i++)
                        {
                            gdata.DealV[i] = cards.GetSFSObject(i).GetIntArray("cards");
                            gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.SetCardV(gdata.DealV[i][0], gdata.DealV[i][1]);
                        }
                        Deal(false);
                    }
                    if (data.ContainsKey("seat"))
                    {
                        OpenPoker(data.GetInt("seat"));
                    }
                    else
                    {
                        OpenPoker();
                    }
                    break;
                case GameRequestType.Result:

                    string gold = data.GetUtfString("gold");
                    string ttgold = data.GetUtfString("ttGold");

                    ResultMgr.SetData(data.GetInt("dice1") + data.GetInt("dice2"), data.GetInt("start"));
                    PlayAllKill(data.GetInt("tongsha"), gold.Split(','), ttgold.Split(','), data.GetIntArray("values"));
                    gdata.GStatus = YxEGameStatus.Normal;
                    TimeMgr.BeginCountDown(data.GetInt("cd"), "等待开局",
                        false, data.ContainsKey("svst") ? data.GetLong("svst") : 0, data.ContainsKey("svct") ? data.GetLong("svct") : 0);
                    break;
                //开始掷骰子动画
                case GameRequestType.StartRollDice:
                    DiceManager.PlayDiceAnim();
                    break;
                case GameRequestType.GameStart:
                    FreshRound();
                    var users = data.GetSFSArray("users");
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (gdata.BankerSeat == -1) return;
                        if (gdata.BankerSeat == users.GetSFSObject(i).GetInt("seat"))
                        {
                            gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA = users.GetSFSObject(i).GetLong("ttgold") - gdata.GuoGold;
                            gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA -= gdata.Boold;
                        }
                        else
                        {
                            gdata.GetPlayerInfo(gdata.BankerSeat, true).CoinA = users.GetSFSObject(i).GetLong("ttgold");
                        }
                    }
                    Debug.LogError("刷新游戏面板");
                    break;
                default:
                    YxDebug.Log("不存在的服务器交互!");
                    break;
            }
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);

            var gdata = App.GetGameData<TbsGameData>();

            if (gdata.UserInfoDict.Count <= 1)
            {
                gdata.IsNobody = true;
                TimeMgr.ShowTips("等待其他玩家进入...");
                TimeMgr.CloseTime();
            }
            else
            {
                gdata.IsNobody = false;
            }
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            var gdata = App.GetGameData<TbsGameData>();
            if (gdata.UserInfoDict.Count > 1)
            {
                TimeMgr.CloseTips();
            }
        }
        /// <summary>
        /// 设置创建显示的参数
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="curRound"></param>
        /// <param name="maxRound"></param>
        public void SetCreatRoomData(int roomId, int curRound, int maxRound)
        {
            if (roomId != 0)
            {
                var gdata = App.GetGameData<TbsGameData>();
                _currentRound = curRound;
                _maxRound = maxRound;
                RoomId.gameObject.SetActive(true);
                CurrentRound.gameObject.SetActive(true);
                gdata.IsCreatRoom = true;
                MenuMgr.InviteBtn.SetActive(true);
                gdata.RoomType = roomId;
            }
            RoomId.text = roomId.ToString(CultureInfo.InvariantCulture);
            CurrentRound.text = string.Format("{0}/{1}", curRound, maxRound);
        }
        /// <summary>
        /// 每次开始下注刷新局数信息
        /// </summary>
        public void FreshRound()
        {
            _currentRound++;
            CurrentRound.text = string.Format("{0}/{1}", _currentRound, _maxRound);
        }
        /// <summary>
        /// 播放通杀通赔动画
        /// </summary>
        /// <param name="kill"></param>
        /// <param name="result"></param>
        /// <param name="ttgold"></param>
        /// <param name="values"></param>
        public void PlayAllKill(int kill, string[] result, string[] ttgold, int[] values)
        {
            _result = result;
            _ttGold = ttgold;
            _dianValues = values;

            var gdata = App.GetGameData<TbsGameData>();
            switch (kill)
            {
                case -1:
                    AllKill.GetComponent<UISprite>().spriteName = "banker_fail";
                    Facade.Instance<MusicManager>().Play("tp");
                    break;
                case 0:
                    foreach (var player in gdata.PlayerList)
                    {
                        player.GetComponent<TbsPlayer>().UserBetPoker.PokerToBetUnder();
                    }
                    //分筹码
                    DistributeBetToBanker(_result);
                    return;
                case 1:
                    AllKill.GetComponent<UISprite>().spriteName = "banker_win";
                    Facade.Instance<MusicManager>().Play("ts");
                    break;
            }

            AllKill.GetComponent<TweenScale>().ResetToBeginning();

            AllKill.GetComponent<TweenScale>().PlayForward();
        }
        /// <summary>
        /// 通赔通杀动画结束
        /// </summary>
        public void AllKillFinish()
        {
            AllKill.GetComponent<TweenScale>().ResetToBeginning();
            AllKill.transform.localScale = Vector3.zero;

            switch (DiceManager.CurDiceType)
            {
                case DiceType.Deal:
                    BeginDistribute();
                    break;
                case DiceType.Result:
                    DiceManager.BeginRollDice(2f);
                    break;
                default:
                    YxDebug.Log("不存在的掷骰子类型!");
                    break;
            }
        }
        /// <summary>
        /// 分发筹码到庄家
        /// </summary>
        public void DistributeBetToBanker(string[] result)
        {
            bool isDis = false;
            var gdata = App.GetGameData<TbsGameData>();
            for (int i = gdata.BankerSeat + 1; i < gdata.BankerSeat + gdata.PlayerList.Length; i++)
            {
                int gold = int.Parse(result[i % gdata.PlayerList.Length]);
                if (gold < 0)
                {
                    isDis = true;
                    gdata.GetPlayer<TbsPlayer>(i % gdata.PlayerList.Length).UserBetRegion.SendExistBet(gdata.GetPlayer<TbsPlayer>(gdata.BankerSeat).UserBetRegion, gold);
                }
            }
            // 如果不需要分筹码到庄家则直接进入从庄家分到闲家
            if (!isDis)
            {
                DistributeBetToPlayer();
            }
            else
            {
                Invoke("DistributeBetToPlayer", 1f);
            }
        }

        /// <summary>
        /// 分发筹码到闲家
        /// </summary>
        public void DistributeBetToPlayer()
        {
            bool isDis = false;
            var gdata = App.GetGameData<TbsGameData>();
            var count = gdata.UserInfoDict.Count;
            for (int i = gdata.BankerSeat + 1; i < gdata.BankerSeat + count; i++)
            {
                int gold = int.Parse(_result[i % gdata.PlayerList.Length]);
                if (gold > 0)
                {
                    isDis = true;
                    gdata.GetPlayer<TbsPlayer>(gdata.BankerSeat).UserBetRegion.SendNewBet(gdata.GetPlayer<TbsPlayer>(i % count).UserBetRegion, gold);
                }
            }

            if (!isDis)
            {
                BetUnderTheTable();
            }
            else
            {
                Invoke("BetUnderTheTable", 1f);
            }
        }

        /// <summary>
        /// 开始分发
        /// </summary>
        public void BeginDistribute()
        {
            var gdata = App.GetGameData<TbsGameData>();

            foreach (var player in gdata.PlayerList)
            {
                player.GetComponent<TbsPlayer>().UserBetPoker.PokerToBetUnder();
            }
            //分筹码
            DistributeBetToBanker(_result);
        }

        /// <summary>
        /// 翻所有人的牌
        /// </summary>
        public void OpenPoker()
        {
            _isOpening = true;
            var gdata = App.GetGameData<TbsGameData>();
            _openIndex = gdata.BankerSeat + 1 >= gdata.PlayerList.Length ? 0 : gdata.BankerSeat + 1;
            _endOpenIndex = _openIndex + 4 % gdata.PlayerList.Length;
            _openTimer = -1f;
        }

        public void OpenPokerUpdate()
        {
            if (!_isOpening)
                return;

            _openTimer += Time.deltaTime;

            if (_openTimer < OpenInterval)
                return;

            _openTimer = 0f;

            var gdata = App.GetGameData<TbsGameData>();
            gdata.GetPlayer<TbsPlayer>(_openIndex, true).UserBetPoker.TurnOverCard();
            _openIndex++;
            _openIndex %= gdata.PlayerList.Length;

            if (_openIndex == _endOpenIndex)
            {
                _isOpening = false;
            }
        }
        /// <summary>
        /// 翻指定人的牌
        /// </summary>
        /// <param name="seat"></param>
        public void OpenPoker(int seat)
        {
            App.GetGameData<TbsGameData>().GetPlayer<TbsPlayer>(seat, true).UserBetPoker.TurnOverCard();
        }

        /// <summary>
        /// 所有筹码下桌
        /// </summary>
        public void BetUnderTheTable()
        {
            bool isDis = false;
            var gdata = App.GetGameData<TbsGameData>();
            var userInfo = gdata.UserInfoDict;

            foreach (var info in userInfo)
            {
                var user = info.Key;
                var player = gdata.GetPlayer<TbsPlayer>(user);
                if (player.UserBetRegion._allBets.Count > 0)
                {
                    isDis = true;
                    player.UserBetRegion.AllBetBackBirth();
                }

            }
            if (!isDis)
            {
                ResultMgr.OpenResult(_result, _ttGold, _dianValues, 8f);
            }
            else
            {
                Invoke("OpenResult", 1f);
            }
        }

        public void OpenResult()
        {
            ResultMgr.OpenResult(_result, _ttGold, _dianValues, 8f);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            BetTotalLabel.text = 0.ToString(CultureInfo.InvariantCulture);
            BetTimeLabel.text = 0.ToString(CultureInfo.InvariantCulture);
            var gdata = App.GetGameData<TbsGameData>();
            gdata.BetTotal = 0;
            gdata.BetTime = 0;
            gdata.IsDeal = false;

            BetManager.Reset();
            BetManager.ReSubtractBet();

            foreach (var player in gdata.PlayerList)
            {
                player.GetComponent<TbsPlayer>().Reset();
            }
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void Deal(bool process = true)
        {

            float processTime = 0.15f;

            processTime = process ? processTime : 0f;
            var gdata = App.GetGameData<TbsGameData>();

            for (int i = gdata.DealFirstSeat; i < gdata.DealFirstSeat + gdata.PlayerList.Length * 2; i++)
            {
                int trueIndex = i % gdata.PlayerList.Length;
                GameObject gob;
                if (i - gdata.DealFirstSeat >= gdata.PlayerList.Length)
                {
                    gob = PokerMgr.Deal(gdata.GetPlayer<TbsPlayer>(trueIndex, true).UserBetPoker.RightPokerPos, i * processTime);
                    gdata.GetPlayer<TbsPlayer>(trueIndex, true).UserBetPoker.RightPoker = gob;
                }
                else
                {
                    gob = PokerMgr.Deal(gdata.GetPlayer<TbsPlayer>(trueIndex, true).UserBetPoker.LeftPokerPos, i * processTime);
                    gdata.GetPlayer<TbsPlayer>(trueIndex, true).UserBetPoker.LeftPoker = gob;
                }
                PokerMgr.SetPokerDepth(gob,
                PokerMgr.PokerMinDepth + PokerMgr.PokerNum + 10 + i + BetManager.BetRegionMaxBet);

                gob.transform.parent = gdata.GetPlayer<TbsPlayer>(trueIndex, true).UserBetPoker.transform;
            }

            gdata.IsDeal = true;
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<TbsGameData>();
            var userDict = gdata.UserInfoDict;
            if (userDict.Count > 2)
            {

                foreach (var keyValue in userDict)
                {
                    var lseat = keyValue.Key;
                    var info = gdata.GetPlayerInfo<TbsUserInfo>(lseat);
                    if (info != null)
                    {
                        if (info.BetGold > 0)
                        {
                            //补注
                            ISFSObject data = new SFSObject();
                            data.PutInt("type", (int)GameRequestType.Bet);
                            data.PutInt("gold", info.BetGold);
                            data.PutInt("seat", info.Seat);
                            ExecuteServerResponse(data);
                        }
                    }
                }
            }
            else
            {
                TimeMgr.ShowTips("等待其他玩家进入...");
                gdata.IsNobody = true;
            }
        }

        public void RefreshLabel()
        {
            YxDebug.Log("更新Label!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var gdata = App.GetGameData<TbsGameData>();
            GuoGoldLabel.text = YxUtiles.ReduceNumber(gdata.GuoGold);
            BetTotalLabel.text = YxUtiles.ReduceNumber(gdata.BetTotal);
            BetTimeLabel.text = gdata.BetTime.ToString(CultureInfo.InvariantCulture);
            BankerLimitLabel.text = YxUtiles.ReduceNumber(gdata.BankerLimit);
            BooldLabel.text = YxUtiles.ReduceNumber(gdata.Boold);
        }

        /// <summary>
        /// 强类型用户信息
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public TbsUserInfo GetUserInfo(ISFSObject userData)
        {
            var tbsUserInfo = new TbsUserInfo
            {
                NickM = userData.GetUtfString(RequestKey.KeyName),
                Seat = userData.GetInt(RequestKey.KeySeat),
                Id = userData.GetInt(RequestKey.KeyId),
                //                IsReady = userData.GetBool(RequestKey.KeyState),
                CoinA = userData.GetLong(RequestKey.KeyTotalGold),
                Country = userData.GetUtfString(RequestKey.KeyCountry),
                SexI = userData.GetShort("sex"),
                AvatarX = userData.GetUtfString("avatar"),
                Network = userData.GetBool("network"),
                BetGold = userData.GetInt("betgold")
            };

            return tbsUserInfo;
        }
        [HideInInspector]
        public string[] BetTypeNames = { "庄家", "初门", "天门", "末门" };

        /// <summary>
        /// 设置庄家
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="isChooseGuo"></param>
        public void SetBanker(int seat, bool isChooseGuo = true)
        {
            var gdata = App.GetGameData<TbsGameData>();

            if (gdata.BankerSeat < 0)
            {
                return;
            }

            gdata.GuoGold = 0;

            for (int i = gdata.BankerSeat; i < gdata.BankerSeat + gdata.PlayerList.Length; i++)
            {
                int index = i % gdata.PlayerList.Length;
                if (gdata.GetPlayer<TbsPlayer>(index,true) == null) return;
                gdata.GetPlayer<TbsPlayer>(index, true).UserBetRegion.BetTypeLabel.text = BetTypeNames[i - gdata.BankerSeat];
                gdata.GetPlayer<TbsPlayer>(index, true).BankerIcon.SetActive(i == gdata.BankerSeat);
            }

            if (gdata.BankerSeat == App.GameData.SelfSeat)
            {
                if (isChooseGuo)
                {
                    //固定锅的金币,直接发送锅金币为底注的50倍
                    IDictionary data = new Dictionary<string, object>();
                    data.Add("gold", BetManager.Ante * 50);
                    App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.SetGuo, data);
                }
                BetManager.ClosePanel();
                BankerBtnMgr.OpenPanel(BankerBtn.CutPoker);
            }
            else
            {
                BankerBtnMgr.IsAuto = false;
                BetManager.OpenPanel();
                BankerBtnMgr.ClosePanel();
            }
        }
        /// <summary>
        /// 添加筹码
        /// </summary>
        public void AddBet(int gold, int seat)
        {
            var gdata = App.GetGameData<TbsGameData>();
            gdata.BetTotal += gold;
            RefreshLabel();

            gdata.GetPlayerInfo(seat, true).CoinA -= gold;
            gdata.GetPlayer<TbsPlayer>(seat, true).IsExit = false;

            if (gold > 0)
            {
                //播放下注音效
                Facade.Instance<MusicManager>().Play("bet");

                var values = BetManager.GetBetsValues(gold);
                YxDebug.Log("下注长度 == " + values.Length);

                foreach (int value in values)
                {
                    GameObject bet = BetManager.GetBet(-1, value);
                    gdata.GetPlayer<TbsPlayer>(seat, true).UserBetRegion.AddBet(bet, seat, value, int.Parse(bet.name));
                }
            }
            else
            {
                GameObject bet = BetManager.GetBet(-1, Mathf.Abs(gold));
                gdata.GetPlayer<TbsPlayer>(seat, true).UserBetRegion.SubBet(gold, int.Parse(bet.name));
            }

            if (seat == App.GameData.SelfSeat)
            {
                BetManager.ChangeBetCount(gold);
            }
        }
    }
}
