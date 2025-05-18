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

namespace Assets.Scripts.Game.sssjp.ImgPress.Main
{
    public class SssjpGameManager : YxGameManager
    {
        public TurnResult TurnRes;
        public MatchMgr MatchMgr;
        public GameObject SwatAnim;
        public HistoryResultMgr HistoryMgr;
        public ChoiseMgr ChoiseMgr;
        public Dealer Dealer;
        public WeiXinInviteMgr WeiXinInviteBtn;
        public ClockCd ClockCd;
        public DismissRoomMgr DismissRoomMgr;
        public SummaryMgr SummaryMgr;
        public ResultMgr ResultMgr;
        public RoomRuleView RoomRuleView;
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
                var data = gameInfo.GetSFSObject("user");
                InitUserGameState(data);
                bool selfReadyState = data.ContainsKey("state") && data.GetBool("state");
                SettingMenu.OnSelfReady(selfReadyState);
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

            //初始化庄家标识
            if (gameInfo.ContainsKey("banker"))
            {
                int bankerSeat = gameInfo.GetInt("banker");
                gdata.BankerSeat = bankerSeat;
                gdata.IsBankerModel = true;
                gdata.GetPlayer<SssPlayer>(bankerSeat, true).SetBankerMarkActive();
            }

            if (gdata.ShowAutoReadyTime)
            {
                int lastTime = GetRemainingTime(gdata.ReadyTime, gameInfo);
                gdata.GetPlayer<SelfPanel>().CountDownReadyTime(lastTime);
            }
        }

        /// <summary>
        /// 获取剩余时间
        /// </summary>
        /// <param name="maxTime"></param>
        /// <param name="gameInfo"></param>
        /// <returns></returns>
        int GetRemainingTime(int maxTime, ISFSObject gameInfo)
        {
            var passTime = gameInfo.GetLong("ct") - gameInfo.GetLong("st");
            return maxTime - (int)passTime;
        }

        public void InitUserGameState(ISFSObject data)
        {
            var gdata = App.GetGameData<SssGameData>();
            int seat = data.GetInt("seat");
            SssPlayer panel = gdata.GetPlayer<SssPlayer>(seat, true);
            bool selfReadyState = data.ContainsKey("state") && data.GetBool("state");
            panel.SetReadyStatue(selfReadyState);


            if (data.ContainsKey("isput"))
            {
                Dealer.FaPai(panel);  //为玩家创建13张手牌
                int isput = data.GetInt("isput");
                if (isput > 0)
                {
                    panel.MoveHandCardNoAnim();
                }
                else
                {
                    //播放等待动画
                    panel.RepositionCards();
                    panel.Waitting();
                }
            }
            //比牌阶段,所有人直接显示所有手牌
            else if (data.ContainsKey("playerinfo"))
            {
                var matchInfo = GetMatchInfoOnInitUserGameState(data);

                Dealer.FaPai(panel);
                panel.MoveHandCardNoAnim();
                panel.ShowAllHandPoker(matchInfo);
                if (matchInfo.Special > (int)CardType.none)
                {
                    panel.HandCardsType.SetSpecialMarkActive(true);
                    panel.HandCardsType.ShowTotalScore(matchInfo.TtScore);
                }
                else
                {
                    panel.GetGameInfoInitUser(matchInfo);
                }
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
                int score = data.GetInt("score");

                if (score > 0)
                {
                    ResultMgr.WinSeats.Add(seat);
                    panel.ShowResultLabel(score);
                }
                else if (score < 0)
                {
                    ResultMgr.LoseSeats.Add(seat);
                    panel.ShowResultLabel(score);
                }
            }
        }


        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<SssGameData>();

            if (gameInfo.ContainsKey("showGoldRate"))
            {
                gdata.ShowGoldRate = gameInfo.GetInt("showGoldRate");
            }

            if (gameInfo.ContainsKey("rule"))
            {
                RoomRuleView.InitRoomRuleInfo(gameInfo);
            }

            if (gameInfo.ContainsKey("ownerId"))
            {
                _ownerId = gameInfo.GetInt("ownerId");
            }

            RoomInfo.ShowRoomInfo(gameInfo);
            //获取房间配置
            if (gdata.IsRoomGame)
            {
                WeiXinInviteBtn.ChatInviteBtn.SetActive(!gdata.IsPlayed);
                HistoryMgr.MaxCount = gameInfo.GetInt("maxRound");
            }
            else
            {
                WeiXinInviteBtn.ChatInviteBtn.SetActive(false);
            }

            if (gameInfo.ContainsKey("state"))
            {
                int state = gameInfo.GetInt("state");
                if (state > 1)
                {
                    int cd = gameInfo.GetInt("cd");
                    int remainingTime = GetRemainingTime(cd, gameInfo);
                    ClockCd.BeginCountDown(remainingTime, (int)gameInfo.GetLong("st"), false);     //开始记时
                    Debug.Log("<color=#00FF2BFF>" + "重连服务器时间擢" + gameInfo.GetLong("ct") + "</color>");
                    YxClockManager.BeginToCountDown(remainingTime);
                }
                else
                {
                    ChoiseMgr.HideChoiseView();     //隐藏选牌界面
                }
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
        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            var user = sfsObject.GetSFSObject("user");
            int serverSeat = user.GetInt("seat");
            bool readyState = user.ContainsKey("state") && user.GetBool("state");
            App.GameData.GetPlayer<SssPlayer>(serverSeat, true).SetReadyStatue(readyState);
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
            if (BeginParticle != null)
            {
                BeginParticle.gameObject.SetActive(true);
                BeginParticle.Play();
            }
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

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            App.GameData.GStatus = YxFramwork.Enums.YxEGameStatus.PlayAndConfine;
            Reset();
            PlayersCardReset();
        }

        /// <summary>
        /// 发牌阶段过程控制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerator DealPokers(ISFSObject data)
        {

            if (BeginParticle != null)
            {
                yield return new WaitForSeconds(3);
                BeginParticle.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(1);
            }

            ClockCd.BeginCountDown(App.GetGameData<SssGameData>().PutTime, App.GetGameData<SssGameData>().PutTime, true);     //开始记时

            //播放发牌动画
            Dealer.FaPai(data.GetIntArray("seats"));    //发牌
            yield return new WaitForSeconds(1.5f);      //玩家看到发牌动画

            //显示发牌过程
            ChoiseMgr.ShowChoiseView(data);    //显示和刷新选牌数据
            PlayOnesSound("start_poker", App.GameData.GetPlayerInfo().SexI);
        }



        /// <summary>
        /// 游戏数据重置
        /// </summary>
        public void Reset()
        {
            _dealPokers = false;
            TurnRes.Reset();

            ResultMgr.Reset();
            ChoiseMgr.Reset();
            ClockCd.StopCountDown();

            foreach (GameObject bet in Bets)
            {
                Destroy(bet);
            }

            Bets.Clear();

            var gdata = App.GetGameData<SssGameData>();

            gdata.IsPlaying = false;
        }

        public void PlayersCardReset()
        {
            var gdata = App.GameData;
            foreach (var player in gdata.PlayerList)
            {
                var user = (SssPlayer)player;
                user.Reset();
            }
            MatchMgr.Reset();
            StopAllCoroutines();
            Dealer.Reset();
        }

        void ResetAllPlayerState()
        {
            var gdata = App.GetGameData<SssGameData>();

            foreach (var player in gdata.PlayerList)
            {
                player.ReadyState = false;
            }
        }


        public void PlayOnesSound(string audioName, int sexI)
        {
            var source = sexI == 0 ? "woman" : "man";
            Facade.Instance<MusicManager>().Play(audioName, source);
        }

        public List<UserMatchInfo> GetMatchInfoList(ISFSObject info)
        {
            var handCardInfoList = new List<UserMatchInfo>();
            if (info.ContainsKey("playerinfo"))
            {
                ISFSArray infoArray = info.GetSFSArray("playerinfo");
                handCardInfoList.AddRange(from ISFSObject item in infoArray select GetMatchInfo(item));
            }

            return handCardInfoList;
        }

        /// <summary>
        /// 初始化玩家的手牌信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        UserMatchInfo GetMatchInfo(ISFSObject info)
        {
            UserMatchInfo userMatchInfo = ParseMatchInfo(info);

            ISFSArray cardInfos = info.GetSFSArray("duninfo") ?? info.GetSFSObject("duns").GetSFSArray("duninfo");

            AddDunInfo(ref userMatchInfo, cardInfos);

            //检测
            YxDebug.LogArray(userMatchInfo.Cards);
            if (userMatchInfo.Cards.Count != 13)
            {
                Debug.LogError("手牌不是十三张!!");
                string errorMsg = string.Empty;
                foreach (var card in userMatchInfo.Cards)
                {
                    errorMsg += card + " , ";
                }
                Debug.LogError(errorMsg);
            }

            ISFSObject userScore = info.GetSFSObject("userscore");

            AddScoreInfo(ref userMatchInfo, userScore);

            if (info.ContainsKey("daqiang"))
            {
                userMatchInfo.Shoot = GetShootInfo(info);
            }

            return userMatchInfo;
        }

        UserMatchInfo GetMatchInfoOnInitUserGameState(ISFSObject info)
        {
            UserMatchInfo userMatchInfo = ParseMatchInfo(info);
            ISFSArray playerInfo = info.GetSFSArray("playerinfo");
            ISFSArray dunInfo = playerInfo.GetSFSObject(0).GetSFSArray("duninfo");
            AddDunInfo(ref userMatchInfo, dunInfo);

            ISFSObject scoreInfo = playerInfo.GetSFSObject(1);
            AddScoreInfo(ref userMatchInfo, scoreInfo);
            var shootInfo = GetShootInfo(info);
            userMatchInfo.Shoot = shootInfo;
            return userMatchInfo;
        }



        UserMatchInfo ParseMatchInfo(ISFSObject info)
        {
            UserMatchInfo userMatchInfo = new UserMatchInfo
            {
                Seat = info.GetInt("seat"),
                Special = info.GetInt("special"),
                Swat = false,
                Cards = new List<int>(),
                DunTypeList = new List<int>(),
                NormalScores = new List<int>(),
                AddScore = new List<int>(),
                Shoot = new ShootInfo()
            };
            return userMatchInfo;
        }


        void AddDunInfo(ref UserMatchInfo matchInfo, ISFSArray cardInfo)
        {
            YxDebug.Log(" ========== " + matchInfo.Seat + " ========== ");
            foreach (ISFSObject info in cardInfo)
            {
                matchInfo.DunTypeList.Insert(0, info.GetInt("type"));
                int[] cards = info.GetIntArray("cards");
                YxDebug.LogArray(cards);
                foreach (int card in cards)
                {
                    matchInfo.Cards.Insert(0, card);
                }
            }
        }


        void AddScoreInfo(ref UserMatchInfo matchInfo, ISFSObject scoreInfo)
        {
            matchInfo.Swat = scoreInfo.ContainsKey("quanleida") && scoreInfo.GetBool("quanleida");

            matchInfo.TtScore = scoreInfo.GetInt("score");

            ISFSArray scoreArray = scoreInfo.GetSFSArray("dunscore");
            foreach (ISFSObject score in scoreArray)
            {
                matchInfo.NormalScores.Insert(0, score.GetInt("normal"));
                matchInfo.AddScore.Insert(0, score.GetInt("add"));
            }

            if (scoreInfo.ContainsKey("basesocre"))
            {
                matchInfo.ShootScore = scoreInfo.GetIntArray("basesocre");
            }
        }


        ShootInfo GetShootInfo(ISFSObject info)
        {
            int[] shootSeats = info.GetIntArray("daqiang");
            int[] beShoot = info.GetIntArray("bedaqiang");
            int shootCount = beShoot == null ? 0 : beShoot.Length;
            ShootInfo shoot = new ShootInfo
            {
                Seat = info.GetInt("seat"),
                ShootTargs = shootSeats,
                BeShootCount = shootCount,
                ShootCount = shootSeats.Length
            };

            return shoot;
        }

        /// <summary>
        /// 将时钟移回桌面
        /// </summary>
        public void MoveClock()
        {
            ClockCd.HaveWarning = false;
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
            //重置数据
            Reset();
            PlayersCardReset();
            ResetAllPlayerState();

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

        public void ShowSwatAnim()
        {
            SwatAnim.SetActive(true);
            Invoke("HideSwatAnim", 3f);
        }

        void HideSwatAnim()
        {
            SwatAnim.SetActive(false);
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

            if (!App.GetRServer<SssjpGameServer>().HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化完成!!");
                return;
            }
            switch (type)
            {
                case GameRequestType.Cards:
                    //重置手牌
                    PlayersCardReset();

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
                    ClockCd.HaveWarning = true;
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
                        Dealer.FaPai(panel);
                        panel.StopWaitting();
                        panel.MoveHandCardNoAnim();
                    }

                    List<UserMatchInfo> handCardsList = GetMatchInfoList(response);
                    MatchMgr.MatchCards(handCardsList);         //比牌
                    HistoryMgr.GetHistoryInfo(handCardsList);   //添加历史记录
                    break;

                case GameRequestType.FinishChoise:

                    int choiseSeat = response.GetInt("seat");

                    SssPlayer fcPanel = gdata.GetPlayer<SssPlayer>(choiseSeat, true);
                    fcPanel.FinishChoiseCards();
                    break;

                case GameRequestType.Result:
                    //先缓存结果数据
                    mISFSObject = response;
                    //ResultMgr.ResultFlyChips(reposnse);
                    //HistoryMgr.AddHistoryInfo(response);
                    //if (response.ContainsKey("banker"))
                    //{
                    //    gdata.BankerSeat = response.GetInt("banker");
                    //}
                    //App.GameData.GStatus = YxFramwork.Enums.YxEGameStatus.Normal;
                    break;
                case GameRequestType.AllowReady:
                    //foreach (var user in gdata.PlayerList)
                    //{
                    //    var player = (SssPlayer)user;
                    //    player.OnAllowReady();
                    //}
                    //Reset();
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
            if (localSeat == gdata.SelfLocalSeat)
            {
                //PlayersCardReset();
                SettingMenu.OnSelfReady(true);
            }
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
            ResetAllPlayerState();
            PlayersCardReset();

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
            App.GetRServer<SssjpGameServer>().OnAllowEnter();
            YxWindowManager.ShowWaitFor();
            return base.OnChangeRoom();
        }

        // 一局结果数据缓存对象
        private ISFSObject mISFSObject;

        public void OnSingleGameOver()
        {
            var datas = App.GetGameData<SssGameData>();
            //分数特效
            ResultMgr.ResultFlyChips(mISFSObject);
            HistoryMgr.AddHistoryInfo(mISFSObject);
            if (mISFSObject.ContainsKey("banker"))
            {
                datas.BankerSeat = mISFSObject.GetInt("banker");
            }
            App.GameData.GStatus = YxFramwork.Enums.YxEGameStatus.Normal;
            //准备 重置数据
            foreach (var user in datas.PlayerList)
            {
                var player = (SssPlayer)user;
                player.OnAllowReady();
            }
            Reset();
        }

        //public override void UserIdle(int localSeat, ISFSObject responseData)
        //{
        //    base.UserIdle(localSeat, responseData);
        //    //var gdata = App.GameData;
        //    //gdata.GetPlayer<SssPlayer>(localSeat).OnUserIdle();
        //}

        //public override void UserOnLine(int localSeat, ISFSObject responseData)
        //{
        //    base.UserOnLine(localSeat, responseData);
        //    //var gdata = App.GameData;
        //    //gdata.GetPlayer<SssPlayer>(localSeat).OnUserOnline();
        //}

        /// <summary>
        /// 游戏没有开始前离开游戏，需要告诉服务器离开时状态
        /// </summary>
        public void SendLeaveState(int state)
        {
            SssPlayer panel = App.GetGameData<SssGameData>().GetPlayer<SssPlayer>(0);
            var type = panel.IsReady ? 1 : 0;
            var apiInfo = new Dictionary<string, object>()
            {
                { "roomId", RoomInfo.RoomID },
                { "type", type }
            };
            if (state == 0)
            {
                Facade.Instance<TwManager>().SendAction("group.leaveRoom", apiInfo, null);
            }
            else
            {
                Facade.Instance<TwManager>().SendAction("group.leaveGame", apiInfo, null);
            }
        }
    }

    [Serializable]
    public class LabelStyle
    {
        public Color NormalColor = Color.white;
        public bool ApplyGradient;
        public Color GradientBottom = Color.white;
        public Color GradientTop = Color.white;
        public UILabel.Effect EffectStyle = UILabel.Effect.None;
        public Color EffectColor = Color.white;
        public Vector2 EffectDistance = new Vector2(2, 2);
    }
}
