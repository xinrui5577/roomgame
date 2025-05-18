using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Assets.Scripts.Game.jh.network;
using Assets.Scripts.Game.jh.Sound;
using Assets.Scripts.Game.jh.Public;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Abstracts;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Manager;
using Random = System.Random;

namespace Assets.Scripts.Game.jh.Modle
{

    public enum RoomStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init,
        /// <summary>
        /// 准备
        /// </summary>
        Ready,
        /// <summary>
        /// 开始
        /// </summary>
        CanStart,
        /// <summary>
        /// 发牌
        /// </summary>
        FaPai,
        /// <summary>
        /// 比牌
        /// </summary>
        BiPai,
        /// <summary>
        /// 结束
        /// </summary>
        Over
    }

    public class JhGameTable : YxGameData
    {
        public EventObject EventObj;
        //创建房间的人
        [HideInInspector]
        public int OwnerId;
        
        //是不是 创建房间
        [HideInInspector]
        public bool IsCreatRoom;
        [HideInInspector]
        public string SoundKey;
        //当前操作玩家
        [HideInInspector] 
        public int CurrenPlayer = -1;
        //庄家
        [HideInInspector]
        public int BankerSeat = -1;
        [HideInInspector]
        public int SingleBet;
        [HideInInspector]
        public int TotalBet;
        //过去的时间
        [HideInInspector]
        public long LastTime;
        [HideInInspector]
        public double CdTimeA;
        [HideInInspector]
        public RoomStatus RStatus;

        public int MaxUserCnt;

        public float KaiFangCdTime = 15;

        [HideInInspector]
        public int ReadyTime = 20;
        [HideInInspector]
        public int BetTime = 15;
        [HideInInspector]
        public bool IsCanCompare;
        [HideInInspector]
        public bool IsCanLook;
        [HideInInspector]
        public int CurRound;
        [HideInInspector]
        public int maxRound;
        [HideInInspector]
        public bool IsAutoFollow;

        protected int BiPaiBeiShu;

        protected int ReadyOutTime;

        public JhHupUpInfo HupUp;

        public Coroutine ResultDelay;

        public string SendObjAddName;
        
        public double CdTime
        {
            set { CdTimeA = value; }
            get { return IsCreatRoom ? KaiFangCdTime : CdTimeA; }
        }


        protected override void InitGameData(ISFSObject gameInfo)
        {

            OwnerId = gameInfo.ContainsKey("ownerId") ? gameInfo.GetInt("ownerId") : -1;

            IsGameStart = gameInfo.ContainsKey(RequestKey.KeyPlaying) && gameInfo.GetBool(RequestKey.KeyPlaying);

            if (IsGameStart)
            {
                BankerSeat = gameInfo.GetInt(JhRequestConstKey.KeyBanker);
                if (gameInfo.ContainsKey("currentP"))
                {
                    gameInfo.GetSFSObject(RequestKey.KeyUser).PutInt("currentP", gameInfo.GetInt("currentP"));
                    CurrenPlayer = gameInfo.GetInt("currentP");
                }

                TotalBet = gameInfo.ContainsKey("totalBet") ? gameInfo.GetInt("totalBet") : 0;

                LastTime = gameInfo.GetLong("lasttime");

                CdTime = gameInfo.GetInt("cdTime");

                IsCanCompare = gameInfo.ContainsKey(JhRequestConstKey.KeyCompare) &&
                               gameInfo.GetBool(JhRequestConstKey.KeyCompare);

                IsCanLook = gameInfo.ContainsKey(JhRequestConstKey.KeyLook) &&
                            gameInfo.GetBool(JhRequestConstKey.KeyLook);

                SingleBet = gameInfo.ContainsKey("minGold") ? gameInfo.GetInt("minGold") : AnteRate[0];

                CurRound = gameInfo.GetInt("jhround");

            }
            ReadyOutTime = gameInfo.GetInt("");
            maxRound = gameInfo.GetInt("maxLun");

            BiPaiBeiShu = gameInfo.GetInt("bpbeishu");

            if (gameInfo.ContainsKey("status"))
            {
                RStatus = (RoomStatus) gameInfo.GetInt("status");
            }

            HupUp.FrashHupUpUser();

            IsCreatRoom = CreateRoomInfo != null;

            

        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
        //    SingleBet = AnteRate[0];
            var time = cargs2.ContainsKey("-tptout") ? cargs2.GetUtfString("-tptout") : "300";
            HupUp.HupTime = int.Parse(time);

            if (IsCreatRoom)
            {
                JhRuleInfo info = (JhRuleInfo) CreateRoomInfo;
                EventObj.SendEvent("RuleInfoEvent", "RuleInfo", info.Cargs);
            }            
        }

        protected override YxCreateRoomInfo InitCreateGameData(ISFSObject gameInfo)
        {
            JhRuleInfo info = new JhRuleInfo
            {
                CurRound = gameInfo.ContainsKey("round") ? gameInfo.GetInt("round") : -1,
                MaxRound = gameInfo.ContainsKey("maxRound") ? gameInfo.GetInt("maxRound") : -1,
                RoomId = gameInfo.GetInt(RequestKey.KeyRoomId),
                RuleInfo = gameInfo.ContainsKey("rule") ? gameInfo.GetUtfString("rule") : ""
            };
            info.SetCargs(gameInfo.GetSFSObject("cargs2"));

            return info;
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new JhUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }

        public void SetGameStatus()
        {
            if (ResultDelay != null)
            {
                StopCoroutine(ResultDelay);
            }
            SendGameStatusToTableView();
            SendGameStatusToPlayerView();
        }

        protected void SendGameStatusToTableView()
        {
            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("SingleBet", SingleBet);
            JhUserInfo user = GetPlayerInfo<JhUserInfo>();
            if (RStatus <= RoomStatus.CanStart)
            {
                if (user.IsReady == false)
                {
                    sendObj.PutInt("ShowReady",1);
                }
            }
            else
            {
                sendObj.PutInt("TotalBeat", TotalBet);
            }

            sendObj.PutBool("IsFangKa", IsCreatRoom);
            if (IsCreatRoom)
            {
                if (BankerSeat == SelfSeat)
                {
                    if (RStatus != RoomStatus.CanStart)
                    {
                        sendObj.PutBool("ShowStart", false);
                    }
                    else
                    {
                        sendObj.PutBool("ShowStart", true);
                    }
                    
                }
                sendObj.PutInt("RoomId", CreateRoomInfo.RoomId);
                sendObj.PutInt("MaxJu", CreateRoomInfo.MaxRound);
                sendObj.PutInt("CurJu", CreateRoomInfo.CurRound);
            }

            sendObj.PutInt("MaxLun", maxRound);
            sendObj.PutInt("CurLun", CurRound);

            EventObj.SendEvent("TableViewEvent", "Status", sendObj);
        }


        protected void SendGameStatusToPlayerView()
        {
            ISFSObject sendObj = SFSObject.NewInstance();
            ISFSArray sendArray = SFSArray.NewInstance();
            foreach (var info in UserInfoDict)
            {
                JhUserInfo userInfo = (JhUserInfo)info.Value;
                if (userInfo != null)
                {
                    ISFSObject obj = userInfo.GetSfsObject(RStatus);
                    obj.PutInt("Chair", GetLocalSeat(userInfo.Seat));
                    sendArray.AddSFSObject(obj);
                }
            }

            if (RStatus > RoomStatus.CanStart)
            {
                sendObj.PutInt("CurChair", GetLocalSeat(CurrenPlayer));
                sendObj.PutDouble("CdTime", CdTime);
                sendObj.PutLong("LastTime", LastTime);

                JhUserInfo currP = GetPlayerInfo<JhUserInfo>(CurrenPlayer, true);
                SetCurrenPlayerBeatMinAndMax(currP, sendObj);
                
                sendObj.PutInt("Banker", GetLocalSeat(BankerSeat));

                if (TotalBet != 0)
                {
                    ISFSArray betArray = SFSArray.NewInstance();
                    int ttBet = TotalBet;
                    for (int i = AnteRate.Count - 1; i >= 0; i--)
                    {
                        if (ttBet >= AnteRate[i])
                       { 
                            ISFSObject arrO = SFSObject.NewInstance();
                            arrO.PutInt("ChipValue",AnteRate[i]);
                            arrO.PutInt("ChipIndex", i);
                            arrO.PutInt("ChipCnt", ttBet / AnteRate[i]);
                            betArray.AddSFSObject(arrO);
                            ttBet = ttBet % AnteRate[i];
                       }
                    }
                    sendObj.PutSFSArray("ChipList", betArray);
                }

            }
            
            sendObj.PutSFSArray("Players",sendArray);
            sendObj.PutIntArray("Antes", AnteRate.ToArray());
            sendObj.PutInt("SingleBet", SingleBet);
            sendObj.PutBool("IsPlaying", RStatus>RoomStatus.CanStart);

            SetSfsUserContrl(sendObj);

            EventObj.SendEvent("PlayersViewEvent", "Status", sendObj);
        }

        public void OnReady(int  localSaet,ISFSObject data)
        {
            JhUserInfo uInfo = GetPlayerInfo<JhUserInfo>(localSaet);
            uInfo.IsReady = true;
            if (localSaet == 0)
            {
                EventObj.SendEvent("TableViewEvent","Ready",null);
            }

            EventObj.SendEvent("PlayersViewEvent", "Ready", localSaet);

            EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(JhSound.EnAudio.Ready, uInfo.SexI));
        }

        public void OnFapai(ISFSObject data)
        {
            RStatus = RoomStatus.FaPai;

            SingleBet = AnteRate[0];

            LastTime = JhFunc.GetTimeStamp(false);

            BankerSeat = data.GetInt(JhRequestConstKey.KeyBanker);
            int[] playing = data.GetIntArray(JhRequestConstKey.KeyPlayings);
            
            int[] playingChair = new int[playing.Length];
            for(int i = 0;i<playing.Length;i++)
            {
                playingChair[i] = GetLocalSeat(playing[i]);
                JhUserInfo uInfo = GetPlayerInfo<JhUserInfo>(playing[i],true);
                uInfo.IsPlayingGame = true;
                uInfo.CoinA -= SingleBet;
                TotalBet += SingleBet;
            }


            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Banker",GetLocalSeat(BankerSeat));
            sendObj.PutIntArray("Playing",playingChair);
            sendObj.PutInt("ChipValue", SingleBet);
            sendObj.PutInt("ChipIndex", AnteRate.IndexOf(SingleBet));
            EventObj.SendEvent("PlayersViewEvent", "Fapai", sendObj);

            SendGameStatusToTableView();

            EventObj.SendEvent("SoundEvent", "PlayerEffect", new JhSound.SoundData(JhSound.EnAudio.Card));

        }

        public void OnCurPlayer(ISFSObject response)
        {
            LastTime = JhFunc.GetTimeStamp(false);

            CurrenPlayer = response.GetInt("p");
            CurRound = response.GetInt("round");
            CdTime = response.GetInt("cd");
            SingleBet = response.GetInt("minGold");
            
            if (response.ContainsKey("look"))
            {
                IsCanLook = response.GetBool("look");
            }
            if (response.ContainsKey("compare"))
            {
                IsCanCompare = response.GetBool("compare");
            }

            JhUserInfo curUser = GetPlayerInfo<JhUserInfo>(CurrenPlayer, true);
            if (response.ContainsKey("gzyz"))
            {
                curUser.IsGzyz = response.GetBool("gzyz");
            }

            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Chair", GetLocalSeat(CurrenPlayer));
            SetSfsUserContrl(sendObj);
            sendObj.PutDouble("CdTime", CdTime);
            sendObj.PutInt("SingleBet", SingleBet);
            if (CurrenPlayer == SelfSeat)
            {
                if (!curUser.IsGzyz&&IsAutoFollow)
                {
                    sendObj.PutDouble("CdTime", 2.0f);
                }
            }

            SetCurrenPlayerBeatMinAndMax(curUser, sendObj);

            EventObj.SendEvent("PlayersViewEvent", "CurrPlayer", sendObj);


            sendObj.PutInt("MaxLun", maxRound);
            sendObj.PutInt("CurLun", CurRound);
            EventObj.SendEvent("TableViewEvent", "CurrPlayer", sendObj);
        }

        public override void ResetData(){
            
            base.ResetData();
            foreach (var userInfo in UserInfoDict)
            {
                JhUserInfo use = (JhUserInfo) userInfo.Value;
                use.ResetUserStatus();
            }
            SingleBet = 0;
            TotalBet = 0;
            IsCanLook = false;
            LastTime = 0;
            CdTime = 0;
            IsCanCompare = false;
            IsAutoFollow = false;
            CurRound = 0;
        }

        public void OnGenZhu(ISFSObject response)
        {
            int genSeat = response.GetInt(JhRequestConstKey.KeySeat);
            int gold = response.GetInt("gold");

            JhUserInfo uinfo = GetPlayerInfo<JhUserInfo>(genSeat,true);
            uinfo.CoinA -= gold;
            TotalBet += gold;
            

            EventObj.SendEvent("TableViewEvent", "Bet", TotalBet);

            bool isAdd =    false;
            if (uinfo.IsLook)
            {
                if (gold/ 2 != SingleBet)
                    isAdd = true;
            }
            else
            {
                if (gold != SingleBet)
                    isAdd = true;
            }

            if (!isAdd)
            {
                uinfo.AddCnt++;
            }


            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Chair",GetLocalSeat(genSeat));
            SetSfsObjChips(uinfo,sendObj,gold);

            sendObj.PutInt("Gold",gold);
            EventObj.SendEvent("PlayersViewEvent", "Bet", sendObj);


            EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(isAdd ? JhSound.EnAudio.Add : JhSound.EnAudio.Follow, uinfo.SexI, uinfo.AddCnt>3?3:uinfo.AddCnt));
            EventObj.SendEvent("SoundEvent", "PlayerEffect", new JhSound.SoundData(JhSound.EnAudio.Chip));

        }

        public void OnQiPai(ISFSObject response)
        {
            int qpSeat = response.GetInt("seat");
            JhUserInfo uinfo = GetPlayerInfo<JhUserInfo>(qpSeat,true);
            uinfo.IsGiveUp = true;

            EventObj.SendEvent("PlayersViewEvent", "GiveUp", GetLocalSeat(qpSeat));

            Random rd = new Random();
            var index = rd.Next(1, 3);
            EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(JhSound.EnAudio.GiveUp, uinfo.SexI, index));
        }

        public void OnLookCards(ISFSObject response)
        {
            LastTime = JhFunc.GetTimeStamp(false);

            int seat = response.GetInt("seat");
            JhUserInfo uinfo = GetPlayerInfo<JhUserInfo>(seat,true);
            uinfo.IsLook = true;
            if (SelfSeat == seat)
            {
                uinfo.Cards = response.GetIntArray("cards");
            }

            if (response.ContainsKey("gzyz"))
            {
                uinfo.IsGzyz = response.GetBool("gzyz");
            }

            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("LookChair",GetLocalSeat(seat));

            if (SelfSeat == seat)
            {
                sendObj.PutIntArray("Cards", uinfo.Cards);
            }

            if (SelfSeat == seat&&SelfSeat==CurrenPlayer)
            {
                sendObj.PutBool("IsCurPlayer", true);
                SetSfsUserContrl(sendObj);
            }
            
            EventObj.SendEvent("PlayersViewEvent", "Look", sendObj);

            EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(JhSound.EnAudio.Look, uinfo.SexI));
        }

        public void OnCompare(ISFSObject response)
        {
            int winner = response.GetInt("winner");
            int loster = response.GetInt("loster");
            int seat = response.GetInt(RequestKey.KeySeat);
            int gold = response.GetInt("gold");

            JhUserInfo losterInfo = GetPlayerInfo<JhUserInfo>(loster,true);
            JhUserInfo seatInfo = GetPlayerInfo<JhUserInfo>(seat, true);
            seatInfo.CoinA -= gold;
            TotalBet += gold;
            losterInfo.IsFail = true;

            EventObj.SendEvent("TableViewEvent", "Bet", TotalBet);

            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Winner",GetLocalSeat(winner));
            sendObj.PutInt("Loster",GetLocalSeat(loster));
            sendObj.PutInt("Chair", GetLocalSeat(seat));
            sendObj.PutInt("Gold", gold);
            SetSfsObjChips(seatInfo, sendObj, gold, BiPaiBeiShu);


            EventObj.SendEvent("PlayersViewEvent", "Compare", sendObj);

            EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(JhSound.EnAudio.Compare, seatInfo.SexI));

        }

        public int GetSeat(int chair)
        {
            return (chair + SelfSeat)%MaxUserCnt;
        }


        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Compare":
                    OnCompareClick(data.Data);
                    break;
                case "ShowUserDetail":
                    OnShowUserDetail(data.Data);
                    break;
                case "AutoFollow":
                    OnAutoFollow();
                    break;
                case "ShowResult":
                    if (ResultSfs != null)
                    {
                        EventObj.SendEvent("ReultViewEvent", "Result", ResultSfs);
                    }
                    
                    break;
            }
        }

        protected void OnAutoFollow()
        {
            IsAutoFollow = !IsAutoFollow;
            ISFSObject sendObj = SFSObject.NewInstance();
            if (IsAutoFollow)
            {
                //判断当前用户是否是自己
                if (CurrenPlayer == SelfSeat)
                {
                    EventObj.SendEvent("ServerEvent", "FollowReq", null);
                }
            }
            else
            {
                if (CurrenPlayer == SelfSeat)
                {
                    sendObj.PutDouble("CdTime", CdTime - (double)(JhFunc.GetTimeStamp(false) - LastTime)/1000.0f);
                }
            }
            sendObj.PutBool("IsCurrPlayer", CurrenPlayer == SelfSeat);
            sendObj.PutBool("IsFollow", IsAutoFollow);
            SetSfsUserContrl(sendObj);
            EventObj.SendEvent("PlayersViewEvent", "AutoFollow", sendObj);
        }


        public void OnShowCard(ISFSObject response)
        {
            int seat = response.GetInt(JhRequestConstKey.KeySeat);
            JhUserInfo player = GetPlayerInfo<JhUserInfo>(seat, true);
            player.IsShowCards = true;
            player.Cards = response.GetIntArray("cards");

            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Chair", GetLocalSeat(seat));
            sendObj.PutIntArray("Cards", player.Cards);
            SetSfsUserContrl(sendObj);
            EventObj.SendEvent("PlayersViewEvent", "LiangPai", sendObj);

        }
        private void OnShowUserDetail(object data)
        {
            
        }

        private void OnCompareClick(object data)
        {
            List<int> chair = new List<int>();
            foreach (var info in UserInfoDict)
            {
                JhUserInfo userInfo = info.Value as JhUserInfo;
                if (userInfo != null && userInfo.IsPlaying()&&userInfo.Seat!=SelfSeat)
                {
                    chair.Add(GetLocalSeat(userInfo.Seat));
                }
            }

            EventObj.SendEvent("PlayersViewEvent", "CompareUsers", chair); 
        }

        protected ISFSArray ResultSfs;
        public void OnResult(ISFSObject response)
        {
            RStatus = RoomStatus.Over;
            int[] cards = response.GetIntArray("cards");
            JhUserInfo self = GetPlayerInfo<JhUserInfo>();
            self.Cards = cards;
            ISFSArray users = response.GetSFSArray("users");
            for (int i = 0; i < users.Count; i++)
            {
                ISFSObject itemData = users.GetSFSObject(i);
                int seat = itemData.GetInt("seat");
                JhUserInfo user = GetPlayerInfo<JhUserInfo>(seat, true);
                user.SetResult(itemData);
            }

            if (response.ContainsKey("compare"))
            {
                ISFSArray arr = response.GetSFSArray("compare");
                for (int i = 0; i < arr.Size(); i++)
                {
                    ISFSObject obj = arr.GetSFSObject(i);
                    int seat = obj.GetInt("seat");
                    JhUserInfo uInfo = GetPlayerInfo<JhUserInfo>(seat, true);
                    uInfo.Cards = obj.GetIntArray("cards");
                    int[] cards1 = obj.GetIntArray("cards");
                    Debug.LogError(" result cards " + uInfo.NickM + "  " + cards1[0] + " " + cards1[1] + " " + cards1[2]);
                }
            }


            bool isWinner = false;
            ResultSfs = SFSArray.NewInstance();
            foreach (var useinfo in UserInfoDict)
            {
                if (useinfo.Value.IsPlayingGame)
                {
                    JhUserInfo jhUserInfo = ((JhUserInfo) useinfo.Value);
                    ISFSObject obj = jhUserInfo.GetResultSfsObject();
                    obj.PutInt("Chair", useinfo.Key);
                    
                    if (useinfo.Key == 0 && jhUserInfo.IsWinner)
                    {
                        isWinner = true;
                    }
                    obj.PutBool("IsWinner", isWinner);
                    ResultSfs.AddSFSObject(obj);
                }
            }

            EventObj.SendEvent("PlayersViewEvent", "Result", ResultSfs);

            ResultDelay = StartCoroutine(SendToResultView(ResultSfs));

            EventObj.SendEvent("SoundEvent", "PlayerEffect", new JhSound.SoundData(isWinner ? JhSound.EnAudio.Win : JhSound.EnAudio.Lost));
        }
        private IEnumerator SendToResultView(ISFSArray arr)
        {
            yield return new WaitForSeconds(1);
            EventObj.SendEvent("ReultViewEvent", "Result", arr);
            EventObj.SendEvent("PlayersViewEvent", "Reset", null);
            ResultDelay = null;
        }
        public void On20Compare(ISFSObject response)
        {
            RStatus = RoomStatus.BiPai;
            int winner = response.GetInt("winseat");
            string msg = response.GetUtfString("msg");

            List<int> lostList = new List<int>();
            foreach (var userInfo in UserInfoDict)
            {
                JhUserInfo jhUser = (JhUserInfo)userInfo.Value;
                if (jhUser.Seat != winner)
                {
                    jhUser.IsFail = true;
                    lostList.Add(userInfo.Key);
                }
            }
            ISFSObject sendObj=new SFSObject();
            sendObj.PutUtfString("msg", msg);
            sendObj.PutIntArray("lostChair", lostList.ToArray());

            EventObj.SendEvent("PlayersViewEvent", "AllBiPai", sendObj);

            EventObj.SendEvent("SoundEvent", "PlayerEffect", new JhSound.SoundData(JhSound.EnAudio.CompareAnimate));
        }

        public void OnGameReady(ISFSObject response)
        {
            ResetData();
            RStatus = RoomStatus.Ready;
            SendGameStatusToTableView();
            SendGameStatusToPlayerView();
            EventObj.SendEvent("ReultViewEvent", "Ready",null);
        }

        public void OnGuZhuYiZhi(ISFSObject response)
        {
            int gold = response.GetInt("goldinc");
            bool isWin = response.GetBool("result");

            JhUserInfo biUser = GetPlayerInfo<JhUserInfo>(CurrenPlayer, true);
            biUser.CoinA -= gold;
            TotalBet += gold;
            biUser.IsGzyz = false;
            List<int> lostList = new List<int>();
            if (isWin)
            {
                foreach (var userInfo in UserInfoDict)
                {
                    JhUserInfo jhUser = (JhUserInfo) userInfo.Value;
                    if (jhUser != biUser)
                    {
                        jhUser.IsFail = true;
                        lostList.Add(GetLocalSeat(jhUser.Seat));
                    }
                }
            }
            else
            {
                biUser.IsFail = true;
                lostList.Add(GetLocalSeat(CurrenPlayer));
            }

            if (response.ContainsKey("fancha"))
            {
                ISFSArray arr = response.GetSFSArray("fancha");
                for (int i = 0; i < arr.Count; i++)
                {
                    ISFSObject obj = arr.GetSFSObject(i);
                    int seat = obj.GetInt("seat");
                    int fancha = obj.GetInt("gold");
                    JhUserInfo fanUser = GetPlayerInfo<JhUserInfo>(seat, true);
                    fanUser.CoinA += fancha;

                }
            }

            ISFSObject sendObj = SFSObject.NewInstance();
            sendObj.PutInt("Chair",GetLocalSeat(CurrenPlayer));
            sendObj.PutInt("Gold",gold);
            sendObj.PutBool("isWin",isWin);
            sendObj.PutUtfString("Name",biUser.Name);
            sendObj.PutIntArray("LostList", lostList.ToArray());
            //如果存在 反差值的情况 刷新金币
            if (response.ContainsKey("fancha"))
            {
                //刷新金币
                ISFSArray arr = SFSArray.NewInstance();
                foreach (var info in UserInfoDict)
                {
                    JhUserInfo jhInfo = (JhUserInfo) info.Value;
                    if (jhInfo.IsPlaying())
                    {
                        ISFSObject obj = SFSObject.NewInstance();
                        obj.PutInt("Chair",GetLocalSeat(jhInfo.Seat));
                        obj.PutLong("Gold",jhInfo.CoinA);
                        arr.AddSFSObject(obj);
                    }
                }
                sendObj.PutSFSArray("FanCha",arr);
            }

            EventObj.SendEvent("PlayersViewEvent", "GZYZ", sendObj);

            EventObj.SendEvent("SoundEvent", "PlayerEffect", new JhSound.SoundData(JhSound.EnAudio.CompareAnimate));

            EventObj.SendEvent("TableViewEvent", "Bet", TotalBet);
        }

        /****
         * 公共 sfs 组合
         */
        protected void SetSfsUserContrl(ISFSObject sendObj)
        {
            JhUserInfo userInfo = GetPlayerInfo<JhUserInfo>();
            sendObj.PutBool("IsCanLook", IsCanLook && !userInfo.IsLook);
            //如果是开发 不用在意金币
            bool compare = IsCanCompare&&(IsCreatRoom||(userInfo.IsLook ? SingleBet * 2 * BiPaiBeiShu : SingleBet * BiPaiBeiShu) < userInfo.CoinA);
            sendObj.PutBool("IsCanCompare", compare);
            sendObj.PutBool("IsUserPlaying",userInfo.IsPlaying());
            sendObj.PutBool("ShowCard", userInfo.ShowCards());
            sendObj.PutBool("IsGzyz",userInfo.IsGzyz);
            sendObj.PutBool("IsAutoFollow",IsAutoFollow);
        }


        protected void SetSfsObjChips(JhUserInfo userInfo,ISFSObject sendObj,int gold,int baseCnt = 1)
        {
            JhSendObjAdd.GetChipData data = new JhSendObjAdd.GetChipData();
            data.Gold = gold;
            data.SendObj = sendObj;
            data.UserInfo = userInfo;
            data.AnteRate = AnteRate;
            EventObj.SendEvent("SendObjAddEvent", "GetChip", data);
        }

        protected void SetCurrenPlayerBeatMinAndMax(JhUserInfo userInfo, ISFSObject sendObj)
        {
            JhSendObjAdd.GetBeatMinAndMix data = new JhSendObjAdd.GetBeatMinAndMix();
            data.UserInfo = userInfo;
            data.SignleBet = SingleBet;
            data.PlayerGold = IsCreatRoom ? JhFunc.MaxInt : userInfo.CoinA;
            data.SendObj = sendObj;
            EventObj.SendEvent("SendObjAddEvent","GetBeatMinAndMax",data);
        }

        public void OnStart(ISFSObject response)
        {
            RStatus = (RoomStatus)response.GetInt("status");
            if (RStatus == RoomStatus.CanStart)
            {
                int seat = response.GetInt("seat");
                if (seat == SelfSeat)
                {
                    EventObj.SendEvent("TableViewEvent", "Start", true);
                }
            }
            else
            {
                int seat = response.GetInt("seat");
                if (seat == SelfSeat)
                {
                    EventObj.SendEvent("TableViewEvent", "Start", false);
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (string.IsNullOrEmpty(SendObjAddName))
            {
                SendObjAddName = "SendObjAdd";
            }

            GameObject obj = Instantiate(ResourceManager.LoadAsset(SendObjAddName));

            obj.transform.parent = transform;
            obj.SetActive(true);
        }
    }


}
