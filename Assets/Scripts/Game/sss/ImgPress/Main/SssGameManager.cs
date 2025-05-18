using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.sss.ImgPress.Main
{
    public class SssGameManager : YxGameManager
    {
        public TurnResult TurnRes;
        public MatchMgr MatchMgr;
        public SwatAnim SwatAnim;
        public HistoryResultMgr HistoryMgr;
        public ChoiseMgr ChoiseMgr;
        public Dealer Dealer;
        public WeiXinInviteMgr WeiXinInviteBtn;
        public ClockCd ClockCd;
        public DismissRoomMgr DismissRoomMgr;
        public SummaryMgr SummaryMgr;
        public ResultMgr ResultMgr;
        public RuleInfoView RuleInfoView;
        public SettingMenu SettingMenu;

        public ChoiseMgr[] ChoiseWay;

        public ParticleSystem BeginParticle;
        //----------------------------------------------------------------------------------------------
        public Transform ClockParent;

        protected override void OnStart()
        {
            base.OnStart();
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;
        }


        private int _ownerId = -1;

        public bool IsRoomOwner
        {
            get
            {
                var gdata = App.GetGameData<SssGameData>();
                var self = gdata.GetPlayerInfo();
                return gdata.IsRoomGame && int.Parse(self.UserId) == _ownerId;
            }
        }


        [HideInInspector]
        public List<GameObject> Bets = new List<GameObject>();

        /// <summary>
        /// 结算的总时长
        /// </summary>
        [HideInInspector]
        public int ResultCd = 0;

        public RoomInfo RoomInfo;

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {

            if (gameInfo.ContainsKey("user"))
            {
                InitUserGameState(gameInfo.GetSFSObject("user"));
            }

            //初始化其他玩家信息
            if (gameInfo.ContainsKey("users"))
            {
                //初始化每个玩家的信息
                ISFSArray users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
                foreach (ISFSObject user in users)
                {
                    InitUserGameState(user);
                }

                ResultMgr.DoResult();
            }
            var gdata = App.GetGameData<SssGameData>();
            gdata.GetPlayer<SelfPanel>().SetReadyBtnActive(!gdata.IsPlaying);

            //初始化庄家标识
            if (gameInfo.ContainsKey("banker"))
            {
                int bankerSeat = gameInfo.GetInt("banker");
                gdata.BankerSeat = bankerSeat;
                gdata.IsBankerModel = true;
                gdata.GetPlayer<SssPlayer>(bankerSeat, true).SetBankerMarkActive();
            }
        }

        public void InitUserGameState(ISFSObject data)
        {
            var gdata = App.GetGameData<SssGameData>();
            if (data.ContainsKey("isput"))
            {
                SssPlayer user = gdata.GetPlayer<SssPlayer>(data.GetInt("seat"), true);
                Dealer.FaPai(user);  //为玩家创建13张手牌
                int isput = data.GetInt("isput");
                if (isput > 0)
                {
                    user.MoveHandCardNoAnim();
                }
                else
                {
                    //播放等待动画
                    user.RepositionCards();
                    user.Waitting();
                }
            }

            //比牌阶段,所有人直接显示所有手牌
            else if (data.ContainsKey("duns"))
            {
                SssPlayer user = gdata.GetPlayer<SssPlayer>(data.GetInt("seat"), true);
                Dealer.FaPai(user);
                user.MoveHandCardNoAnim();
                ISFSObject duns = data.GetSFSObject("duns");
                ISFSArray dunsInfo = duns.GetSFSArray("duninfo");
                var cardsValList = dunsInfo.Cast<ISFSObject>().SelectMany(dun => dun.GetIntArray("cards")).ToList();
                user.ShowAllHandPoker(cardsValList);
            }


            //自己会有手牌,是选牌阶段
            if (data.ContainsKey("cards"))
            {
                if (data.ContainsKey("isput") && data.GetInt("isput") == 0)
                    ChoiseMgr.ShowChoiseView(data);
            }

            //比牌阶段初始化
            if (data.ContainsKey("userscore"))
            {
                TurnRes.InitTurnResultInfo(data.GetSFSObject("userscore"));
                TurnRes.ShowAllResultItem();
            }

            if (data.ContainsKey("score"))
            {
                int seat = data.GetInt("seat");
                int score = data.GetInt("score");

                if (score > 0)
                {
                    ResultMgr.WinSeats.Add(seat);
                    gdata.GetPlayer<SssPlayer>(seat, true).ShowResultLabel(score);
                }
                else if (score < 0)
                {
                    ResultMgr.LoseSeats.Add(seat);
                    gdata.GetPlayer<SssPlayer>(seat, true).ShowResultLabel(score);
                }
            }
        }


        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<SssGameData>();
            if (gameInfo.ContainsKey("cargs2"))
            {
                DissectCargs2(gameInfo.GetSFSObject("cargs2"));
            }

            if (gameInfo.ContainsKey("showGoldRate"))
            {
                gdata.ShowGoldRate = gameInfo.GetInt("showGoldRate");
            }

            if (gameInfo.ContainsKey("status"))
            {
                int status = gameInfo.GetInt("status");
                gdata.IsPlaying = status > 0;
            }
            if (gameInfo.ContainsKey("rule"))
            {
                string ruleInfo = gameInfo.GetUtfString("rule");
                gdata.RuleInfo = ruleInfo;
                RuleInfoView.InitRuleInfo(ruleInfo);
            }

            if (gameInfo.ContainsKey("puttime"))
            {
                gdata.PutTime = gameInfo.GetInt("puttime");
            }


            if (gameInfo.ContainsKey("ownerId"))
            {
                _ownerId = gameInfo.GetInt("ownerId");
            }

            //获取房间配置
            if (gameInfo.ContainsKey("rid"))
            {
                gdata.IsRoomGame = true;

                RoomInfo.ShowRoomInfo(gameInfo);
                WeiXinInviteBtn.ChatInviteBtn.SetActive(!gdata.IsPlayed);
                HistoryMgr.MaxCount = gameInfo.GetInt("maxRound");
                RuleInfoView.ShowRuleInfoBtn.SetActive(true);
            }
            else
            {
                gdata.IsRoomGame = false;
                RoomInfo.gameObject.SetActive(false);
                WeiXinInviteBtn.ChatInviteBtn.SetActive(false);
                RuleInfoView.ShowRuleInfoBtn.SetActive(false);
            }

            if (ChoiseWay != null)
            {
                int len = ChoiseWay.Length;
                for (int i = 0; i < len; i++)
                {
                    ChoiseWay[i].Init();
                }
            }

        }

        /// <summary>
        /// 解析Cargs配置信息
        /// </summary>
        /// <param name="cargs"></param>
        void DissectCargs2(ISFSObject cargs)
        {
            var gdata = App.GetGameData<SssGameData>();
            if (cargs.ContainsKey("-resulttype"))
            {
                gdata.SwatModel = false;
            }
            if (cargs.ContainsKey("-daqiang"))
            {
                gdata.ShootScore = int.Parse(cargs.GetUtfString("-daqiang"));
            }
            if (cargs.ContainsKey("-ante"))
            {
                gdata.Ante = int.Parse(cargs.GetUtfString("-ante"));
            }
            if (cargs.ContainsKey("-usetype"))
            {
                //处理是否是代开模式
                gdata.DaiKai = cargs.GetInt("-usetype") == gdata.UserType;
            }
            if (cargs.ContainsKey("-specialtypes"))
            {
                //添加特殊牌型过滤
                string types = cargs.GetUtfString("-specialtypes");
                string[] typeArr = types.Split('#');
                if (typeArr.Length > 0)
                {
                    foreach (string s in typeArr)
                    {
                        gdata.SpecialTypes.Add(Int32.Parse(s));    
                    }
                }
            }
        }


        public void OnGameStart(ISFSObject data)
        {

            //隐藏准备按钮
            var gdata = App.GetGameData<SssGameData>();
            int[] seats = data.GetIntArray("seats");
            foreach (int seat in seats)
            {
                gdata.GetPlayer<SssPlayer>(seat, true).OnGameStart();
            }
            //播放开始特效
            BeginParticle.gameObject.SetActive(true);
            BeginParticle.Play();
            Facade.Instance<MusicManager>().Play("start");
        }

        private bool _dealPokers;
        /// <summary>
        /// 发牌
        /// </summary>
        /// <param name="data"></param>
        public void DealPoker(ISFSObject data)
        {
            if (_dealPokers)
                return;

            _dealPokers = true;
            StopAllCoroutines();
            //发牌
            StartCoroutine(DealPokers(data));
        }

        /// <summary>
        /// 发牌阶段过程控制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerator DealPokers(ISFSObject data)
        {
            yield return new WaitForSeconds(3);
            BeginParticle.gameObject.SetActive(false);

            ClockCd.BeginCountDown(App.GetGameData<SssGameData>().PutTime);     //开始记时

            //播放发牌动画
            Dealer.FaPai(data.GetIntArray("seats"));    //发牌
            yield return new WaitForSeconds(1.5f);      //玩家看到发牌动画

            //显示发牌过程
            ChoiseMgr.ShowChoiseView(data);    //显示和刷新选牌数据
            PlayOnesSound("start_poker", App.GameData.SelfSeat);
        }



        /// <summary>
        /// 游戏数据重置
        /// </summary>
        public void Reset()
        {
            _dealPokers = false;
            TurnRes.Reset();
            MatchMgr.Reset();
            Dealer.Reset();
            ResultMgr.Reset();
            ChoiseMgr.Reset();
            ClockCd.StopCountDown();

            foreach (GameObject bet in Bets)
            {
                Destroy(bet);
            }

            Bets.Clear();

            var gdata = App.GetGameData<SssGameData>();

            foreach (var player in gdata.PlayerList)
            {
                var user = (SssPlayer)player;
                user.Reset();
            }

            gdata.IsPlaying = false;
        }



        public void PlayOnesSound(string audioName, int seat)
        {
            var sex = App.GetGameData<SssGameData>().GetPlayerInfo(seat, true).SexI;
            var source = sex == 0 ? "woman" : "man";
            Facade.Instance<MusicManager>().Play(audioName, source);
        }

        public List<UserMatchInfo> GetCardsInfoList(ISFSObject info)
        {
            var handCardInfoList = new List<UserMatchInfo>();
            if (info.ContainsKey("playerinfo"))
            {
                ISFSArray infoArray = info.GetSFSArray("playerinfo");
                handCardInfoList.AddRange(from ISFSObject item in infoArray select GetCardsInfo(item));
            }

            return handCardInfoList;
        }

        /// <summary>
        /// 初始化玩家的手牌信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        UserMatchInfo GetCardsInfo(ISFSObject info)
        {
            UserMatchInfo userMatchInfo = new UserMatchInfo
            {
                Seat = info.GetInt("seat"),
                Special = info.GetInt("special"),
                Cards = new List<int>(),
                DunTypeList = new List<int>(),
                NormalScores = new List<int>(),
                AddScore = new List<int>(),
                TtScore = info.GetInt("score")
            };

            ISFSArray cardInfos = info.GetSFSArray("duninfo");
            foreach (ISFSObject cardInfo in cardInfos)
            {
                userMatchInfo.DunTypeList.Add(cardInfo.GetInt("type"));
                int[] cards = cardInfo.GetIntArray("cards");
                foreach (int card in cards)
                {
                    userMatchInfo.Cards.Add(card);
                }
            }

            ISFSObject userScore = info.GetSFSObject("userscore");
            ISFSArray scoreArray = userScore.GetSFSArray("dunscore");
            foreach (ISFSObject score in scoreArray)
            {
                userMatchInfo.NormalScores.Add(score.GetInt("normal"));
                userMatchInfo.AddScore.Add(score.GetInt("add"));
            }

            if (info.ContainsKey("daqiang"))
            {
                userMatchInfo.Shoot = GetShootInfo(info);
            }

            if (userScore.ContainsKey("basesocre"))
            {
                userMatchInfo.ShootScore = userScore.GetIntArray("basesocre");
            }

            return userMatchInfo;
        }

        ShootInfo GetShootInfo(ISFSObject info)
        {
            int[] shootSeats = info.GetIntArray("daqiang");

            ShootInfo shoot = new ShootInfo
            {
                Seat = info.GetInt("seat"),
                ShootTargs = shootSeats,
                ShootCount = shootSeats.Length
            };

            return shoot;
        }

        /// <summary>
        /// 将时钟移回桌面
        /// </summary>
        public void MoveClock()
        {
            MoveClock(ClockParent);
        }

        /// <summary>
        /// 将时钟移到制定位置
        /// </summary>
        public void MoveClock(Transform parent)
        {
            ClockCd.MoveClock(parent);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            Reset();
            //初始化房间信息
            InitRoom(gameInfo);
            //初始化玩家信息
            InitUser(gameInfo);

            if (gameInfo.ContainsKey("hup"))
            {
                DismissRoomMgr.ShowDismissViewOnJion(gameInfo);
            }
            SettingMenu.SetRoomModelList();

            YxWindowManager.HideWaitFor();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }


        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<SssGameData>();
            YxDebug.Log("Request == " + type);

            if (!App.GetRServer<SssGameServer>().HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化完成!!");
                return;
            }

            switch (type)
            {
                case GameRequestType.Cards:

                    if (!gdata.IsPlayed)
                    {
                        gdata.IsPlaying = true;
                        SettingMenu.SetRoomModelList();
                    }
                    gdata.IsPlaying = true;

                    SetChoiseModel(gdata.ChoiseModel);


                    WeiXinInviteBtn.ChatInviteBtn.SetActive(false);    //隐藏微信邀请按钮
                    OnGameStart(response);

                    ClockCd.MoveClock(ClockParent);
                    DealPoker(response);
                    if (gdata.IsBankerModel)
                    {
                        foreach (var user in gdata.PlayerList)
                        {
                            var player = (SssPlayer)user;
                            player.SetBankerMarkActive();
                        }
                    }
                    gdata.CurRound++;
                    if (gdata.IsRoomGame)
                    {
                        RoomInfo.UpdataCurRound();     //如果是开房模式,刷新房间数据
                    }
                    break;

                case GameRequestType.Match:

                    //停止选牌
                    StopAllCoroutines();
                    ClockCd.StopCountDown();
                    ChoiseMgr.HideChoiseView();     //隐藏选牌界面

                    //刷新玩家的手牌
                    foreach (var user in gdata.PlayerList)
                    {
                        var player = (SssPlayer)user;

                        if (player.Info != null && player.IsReady)
                        {
                            Dealer.FaPai(player);
                        }
                    }

                    //将玩家手牌恢复到正确的位置
                    ISFSArray playersInfo = response.GetSFSArray("playerinfo");
                    foreach (ISFSObject player in playersInfo)
                    {
                        int stopSeat = player.GetInt("seat");
                        SssPlayer panel = gdata.GetPlayer<SssPlayer>(stopSeat, true);
                        panel.StopWaitting();
                        panel.MoveHandCardNoAnim();
                    }

                    List<UserMatchInfo> handCardsList = GetCardsInfoList(response);

                    MatchMgr.MatchCards(handCardsList);         //比牌
                    HistoryMgr.GetHistoryInfo(handCardsList);   //添加历史记录

                    break;

                case GameRequestType.FinishChoise:

                    int choiseSeat = response.GetInt("seat");

                    SssPlayer fcPanel = gdata.GetPlayer<SssPlayer>(choiseSeat, true);
                    fcPanel.FinishChoiseCards();
                    break;

                case GameRequestType.Result:

                    ResultMgr.ResultFlyChips(response);
                    HistoryMgr.AddHistoryInfo(response);
                    if (response.ContainsKey("banker"))
                    {
                        gdata.BankerSeat = response.GetInt("banker");
                    }

                    break;
                case GameRequestType.AllowReady:
                    Reset();
                    foreach (var user in gdata.PlayerList)
                    {
                        var player = (SssPlayer)user;
                        player.OnAllowReady();
                    }
                    break;

                case GameRequestType.CouldStart:

                    if (App.GameData.SelfSeat == 0)
                    {
                        App.GetGameData<SssGameData>().GetPlayer<SssPlayer>().OnCouldStart();
                    }

                    break;
            }
        }


        /// <summary>
        /// 设置选牌模式
        /// </summary>
        /// <param name="p"></param>
        private void SetChoiseModel(int p)
        {
            if (ChoiseWay == null || ChoiseWay.Length < 1)
            {
                return;
            }

            int len = ChoiseWay.Length;
            ChoiseMgr = ChoiseWay[p % len];
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            YxDebug.Log("Seat " + responseData.GetInt("seat") + " is ready!");

            var gdata = App.GetGameData<SssGameData>();
            int readySeat = responseData.GetInt("seat");
            if (!gdata.IsPlaying)
            {
                //设置玩家的准备状态
                SssPlayer panel = gdata.GetPlayer<SssPlayer>(readySeat, true);
                panel.IsReady = true;
                panel.OnUserReady();
            }
        }

        public override int OnChangeRoom()
        {
            var gdata = App.GetGameData<SssGameData>();
            gdata.CurRound = 0;
            Reset();

            int length = gdata.PlayerList.Length;

            //重置玩家
            for (int i = 0; i < length; i++)
            {
                SssPlayer player = gdata.GetPlayer<SssPlayer>(i);
                player.Reset();
                player.gameObject.SetActive(false);
            }

            //重置历史记录
            HistoryMgr.Reset();

            App.GetRServer<SssGameServer>().OnAllowEnter();
            YxWindowManager.ShowWaitFor();
            return base.OnChangeRoom();
        }
    }
}
