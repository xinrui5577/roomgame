using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.network;
using Assets.Scripts.Game.jlgame.Sound;
using Assets.Scripts.Game.jlgame.ui;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Abstracts;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.Modle
{
    public class JlGameGameTable : YxGameData
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
        [HideInInspector]
        public int CdTime;
        [HideInInspector]
        public bool IsResultWait;

        private int _mrcd;
        private bool _rejoin;

        private int _curSpeaker;

        private int _cd;
        private long _st;
        private long _ct;

        private string _rule;
        private string _hup = "";
        private long _svt;
        private long _hupstart;
        private bool[] _killDragon;
        private bool[] _isDragon;

        private int[] _color1;
        private int[] _color2;
        private int[] _color3;
        private int[] _color4;



        protected override void InitGameData(ISFSObject gameInfo)
        {
            OwnerId = gameInfo.ContainsKey("ownerId") ? gameInfo.GetInt("ownerId") : -1;
            _rule = gameInfo.ContainsKey("rule") ? gameInfo.GetUtfString("rule") : "";
            IsGameStart = gameInfo.ContainsKey(RequestKey.KeyPlaying) && gameInfo.GetBool(RequestKey.KeyPlaying);
            _rejoin = gameInfo.ContainsKey("rejoin") && gameInfo.GetBool("rejoin");
            if (IsGameStart && _rejoin)
            {
                _color1 = gameInfo.GetIntArray("color1");
                _color2 = gameInfo.GetIntArray("color2");
                _color3 = gameInfo.GetIntArray("color3");
                _color4 = gameInfo.GetIntArray("color4");
                _curSpeaker = gameInfo.ContainsKey("curP") ? gameInfo.GetInt("curP") : -1;

                _cd = gameInfo.ContainsKey("cd") ? gameInfo.GetInt("cd") : -1;
                _st = gameInfo.ContainsKey("st") ? gameInfo.GetLong("st") : -1;
                _ct = gameInfo.ContainsKey("ct") ? gameInfo.GetLong("ct") : -1;


                _hup = gameInfo.ContainsKey("hup") ? gameInfo.GetUtfString("hup") : "";
                _svt = gameInfo.ContainsKey("svt") ? gameInfo.GetLong("svt") : 0;
                _hupstart = gameInfo.ContainsKey("hupstart") ? gameInfo.GetLong("hupstart") : 0;

                _killDragon = gameInfo.ContainsKey("killDragon") ? gameInfo.GetBoolArray("killDragon") : null;
                _isDragon = gameInfo.ContainsKey("isDragon") ? gameInfo.GetBoolArray("isDragon") : null;


            }
        }

        protected override YxCreateRoomInfo InitCreateGameData(ISFSObject gameInfo)
        {
            return base.InitCreateGameData(gameInfo);
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            CdTime = cargs2.ContainsKey("-tptout") ? int.Parse(cargs2.GetUtfString("-tptout")) : 300;
            _mrcd = cargs2.ContainsKey("-mrrcd") ? int.Parse(cargs2.GetUtfString("-mrrcd")) : 15;//倒计时大于600显示的值
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new JlGameUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }


        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
            }
        }

        public void FreshTableCard()
        {
            if (IsGameStart && _rejoin)
            {
                var sendObj = SFSObject.NewInstance();
                sendObj.PutIntArray("color1", _color1);
                sendObj.PutIntArray("color2", _color2);
                sendObj.PutIntArray("color3", _color3);
                sendObj.PutIntArray("color4", _color4);
                GetPlayerInfo<JlGameUserInfo>().IsCurSpeaker = _curSpeaker == SelfSeat;
                sendObj.PutBoolArray("killDragon", _killDragon);
                sendObj.PutBoolArray("isDragon", _isDragon);
                EventObj.SendEvent("TableViewEvent", "Rejoin", sendObj);

                var time = _cd-(_ct-_st);
                if (time > 600)
                {
                    time = _mrcd;
                    GetPlayer<JlGamePlayer>(_curSpeaker, true).SetCountNum((int)time, true);
                }
                else
                {
                    GetPlayer<JlGamePlayer>(_curSpeaker, true).SetCountNum((int)time);
                }
                YxClockManager.BeginWaitPlayer(_curSpeaker, time);
            }
            else
            {
                EventObj.SendEvent("TableViewEvent", "Reset", null);
                if (!GetPlayer<JlGamePlayer>().ReadyState)
                {
                    EventObj.SendEvent("TableViewEvent", "Ready", null);
                }
            }
        }

        public void FreshCreatRoomShow()
        {
            var sendObj = SFSObject.NewInstance();
            sendObj.PutUtfString("rule", _rule);
            EventObj.SendEvent("RuleViewEvent", "show", sendObj);

            EventObj.SendEvent("TtResultViewEvent", "TtShow", sendObj);

            App.GameData.GStatus = GetPlayerInfo<JlGameUserInfo>().Cards == null ?YxEGameStatus.Over:YxEGameStatus.PlayAndConfine;

            if (OwnerId != -1)
            {
                IsCreatRoom = true;
            }
            else
            {
                EventObj.SendEvent("ResultViewEvent", "show", sendObj);
                return;
            }

            sendObj.PutInt("roomId", CreateRoomInfo.RoomId);
            sendObj.PutInt("curRound", CreateRoomInfo.CurRound);
            sendObj.PutInt("maxRound", CreateRoomInfo.MaxRound);

            EventObj.SendEvent("ResultViewEvent", "Rule", sendObj);

            EventObj.SendEvent("TableViewEvent", "RoomInfo", sendObj);
        }

        public void RejoinHupUp()
        {
            if (_hup.Equals("")) return;
            var sendObj = SFSObject.NewInstance();
            var time = CdTime - (_svt - _hupstart);
            time = time < 0 ? 0 : time;
            sendObj.PutLong("time", time);
            sendObj.PutUtfString("hup", _hup);
            sendObj.PutUtfString("userId", GetPlayerInfo().UserId);
            EventObj.SendEvent("HupUpViewEvent", "PlayerList", PlayerList);
            EventObj.SendEvent("HupUpViewEvent", "Rejoin", sendObj);
        }

        public void OnFaPai(ISFSObject data)
        {
            var cards = data.GetIntArray(JlGameRequestConstKey.KeyCards);
            YxDebug.LogArray(cards);
            var cardsNum = data.GetUtfString(JlGameRequestConstKey.KeyCardsNum);
            string[] cardNums = cardsNum.Split(',');

            var sendObj = SFSObject.NewInstance();
            sendObj.PutIntArray("cards", cards);
            sendObj.PutUtfStringArray("cardsNum", cardNums);
            sendObj.PutInt("selfSeat", SelfLocalSeat);
            EventObj.SendEvent("TableViewEvent", "Allocate", sendObj);
        }

        public void OnWhoSpeak(ISFSObject data)
        {
            var cd = data.GetInt(JlGameRequestConstKey.KeyCdTime);
            var seat = data.GetInt(RequestKey.KeySeat);
            var activeCards = data.ContainsKey(JlGameRequestConstKey.KeyActiveCards) ? data.GetIntArray(JlGameRequestConstKey.KeyActiveCards) : null;
            var foldNum = data.ContainsKey(JlGameRequestConstKey.KeyFoldNum) ? data.GetInt(JlGameRequestConstKey.KeyFoldNum) : -1;
            var handNum = data.ContainsKey(JlGameRequestConstKey.KeyHandNum) ? data.GetInt(JlGameRequestConstKey.KeyHandNum) : -1;

//            YxDebug.LogArray(activeCards);

            if (cd > 600)
            {
                var count = YxClockManager.WaitPlayerCountDown;
                if (count != null)
                {
                    count.Loop = true;
                }
                cd = _mrcd;
                GetPlayer<JlGamePlayer>(seat, true).SetCountNum(cd,true);
            }
            else
            {
                GetPlayer<JlGamePlayer>(seat, true).SetCountNum(cd);
            }
            
            YxClockManager.BeginWaitPlayer(seat, cd);
           

            if (seat == SelfSeat)
            {
                GetPlayer<JlGameSelfPlayer>(SelfSeat, true).FreshHandCard(activeCards);

                EventObj.SendEvent("SoundEvent", "RemindSound",
                    activeCards != null && activeCards.Length == 0
                        ? new JlGameSound.SoundData(JlGameSound.EnAudio.FoldCard,
                            GetPlayerInfo<JlGameUserInfo>(seat, true).SexI)
                        : new JlGameSound.SoundData(JlGameSound.EnAudio.OutCard,
                            GetPlayerInfo<JlGameUserInfo>(seat, true).SexI));
            }
            else
            {
                GetPlayer<JlGameSelfPlayer>(SelfSeat, true).NoCanClickCard();
            }

            if (foldNum != -1 && handNum != -1)
            {
                GetPlayer<JlGamePlayer>(seat, true).FreshFoldCardShow(foldNum, handNum);
            }
        }

        public void OnOutCard(ISFSObject data)
        {
            var seat = data.GetInt(RequestKey.KeySeat);
            var card = data.GetInt(JlGameRequestConstKey.KeyCard);
            var foldNum = data.ContainsKey(JlGameRequestConstKey.KeyFoldNum) ? data.GetInt(JlGameRequestConstKey.KeyFoldNum) : -1;
            var handNum = data.ContainsKey(JlGameRequestConstKey.KeyHandNum) ? data.GetInt(JlGameRequestConstKey.KeyHandNum) : -1;
            var activeCards = data.ContainsKey(JlGameRequestConstKey.KeyActiveCards) ? data.GetIntArray(JlGameRequestConstKey.KeyActiveCards) : null; //
            var killDragon = data.ContainsKey(JlGameRequestConstKey.KeyKillDragon) ? data.GetBoolArray(JlGameRequestConstKey.KeyKillDragon) : null;
            var isDragon = data.ContainsKey(JlGameRequestConstKey.KeyIsDragon) ? data.GetBoolArray(JlGameRequestConstKey.KeyIsDragon) : null;

            var sendObj = SFSObject.NewInstance();
            sendObj.PutInt("card", card);
            sendObj.PutInt("seat", seat);
            sendObj.PutBoolArray("killDragon", killDragon);
            sendObj.PutBoolArray("isDragon", isDragon);


            EventObj.SendEvent("SoundEvent", "CardSound", new JlGameSound.SoundData(JlGameSound.EnAudio.None, GetPlayerInfo<JlGameUserInfo>(seat, true).SexI, card));
            EventObj.SendEvent("TableViewEvent", "OutCard", sendObj);

//            YxDebug.LogArray(activeCards);
            if (activeCards != null&& activeCards.Length!=0)
            {
                GetPlayer<JlGameSelfPlayer>(SelfSeat, true).FreshHandCard(activeCards, false);
            }
            GetPlayer<JlGamePlayer>(seat, true).FreshFoldCardShow(foldNum, handNum);

            GetPlayer<JlGamePlayer>(seat, true).HideCountDown();
            YxClockManager.StopWaitPlayer();
        }

        public void OnFoldCard(ISFSObject data)
        {
            var seat = data.GetInt(RequestKey.KeySeat);
            var foldNum = data.GetInt(JlGameRequestConstKey.KeyFoldNum);
            var handNum = data.GetInt(JlGameRequestConstKey.KeyHandNum);

            var card = data.ContainsKey(JlGameRequestConstKey.KeyCard) ? data.GetInt(JlGameRequestConstKey.KeyCard) : -1;
            var foldScore = data.ContainsKey(JlGameRequestConstKey.KeyFoldScore) ? data.GetInt(JlGameRequestConstKey.KeyFoldScore) : -1;

            GetPlayer<JlGamePlayer>(seat, true).FreshFoldCardShow(foldNum, handNum, foldScore);

            if (seat == SelfSeat)
            {
                GetPlayer<JlGameSelfPlayer>(seat, true).FoldCard(card);
            }
            EventObj.SendEvent("SoundEvent", "RemindSound", new JlGameSound.SoundData(JlGameSound.EnAudio.HasFoldCard, GetPlayerInfo<JlGameUserInfo>(seat, true).SexI));

            GetPlayer<JlGamePlayer>(seat, true).HideCountDown();
            YxClockManager.StopWaitPlayer();
        }

        public void OnResult(ISFSObject data)
        {
            var ownerSeat = -1;
            if (IsCreatRoom)
            {
                for (int i = 0; i < UserInfoDict.Count; i++)
                {
                    if (OwnerId == UserInfoDict[i].Id)
                    {
                        ownerSeat = UserInfoDict[i].Seat;
                    }
                }
            }

            var result = data.GetSFSArray(JlGameRequestConstKey.KeyResult);
            List<ResultData> resultDatas = new List<ResultData>();

            for (int i = 0; i < result.Count; i++)
            {
                var resultData = new ResultData(result.GetSFSObject(i));
                if (ownerSeat == i)
                {
                    resultData.IsRoomOwner = true;
                }

                if (SelfSeat == i)
                {
                    resultData.IsYouSelf = true;
                }
                resultDatas.Add(resultData);
            }

            if (IsCreatRoom)
            {
                if (CreateRoomInfo.CurRound == CreateRoomInfo.MaxRound)
                {
                    IsResultWait = true;
                    EventObj.SendEvent("ResultViewEvent", "gameOver", true);
                }
                EventObj.SendEvent("ResultViewEvent", "result", resultDatas);
            }
            else
            {
                EventObj.SendEvent("ResultViewEvent", "result", resultDatas);
            }

            EventObj.SendEvent("TableViewEvent", "Reset", null);
        }

        public void OnTrusteeship(ISFSObject data)
        {
            var seat = data.GetInt(RequestKey.KeySeat);
            var trusteeship = data.GetBool(JlGameRequestConstKey.KeyTrusteeshipa);
            if (seat == SelfSeat)
            {
                GetPlayer<JlGameSelfPlayer>(SelfSeat, true).ShowTrusteeshipObj(trusteeship);
            }
            else
            {
                GetPlayer<JlGamePlayer>(seat, true).ShowRob(trusteeship);
            }
        }
    }
}
