using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.network;
using Assets.Scripts.Game.biji.Sound;
using Assets.Scripts.Game.biji.ui;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.biji.Modle
{
    public class BjGameTable : YxGameData
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

        private bool _rejoin;
        private bool _quickGame;
        private string _rule;
        private int _putTime;
        private string _hup = "";
        private long _svt;
        private long _hupstart;
        private ResultData _resultData;
        private List<CompareData> _compareDatas;


        protected override void InitGameData(ISFSObject gameInfo)
        {
            OwnerId = gameInfo.ContainsKey("ownerId") ? gameInfo.GetInt("ownerId") : -1;
            _rule = gameInfo.ContainsKey("rule") ? gameInfo.GetUtfString("rule") : "";
            _putTime = gameInfo.ContainsKey("puttime") ? gameInfo.GetInt("puttime") : -1;
            _rejoin = gameInfo.ContainsKey("rejoin") && gameInfo.GetBool("rejoin");
            if (_rejoin)
            {
                _hup = gameInfo.ContainsKey("hup") ? gameInfo.GetUtfString("hup") : "";
                _svt = gameInfo.ContainsKey("svt") ? gameInfo.GetLong("svt") : 0;
                _hupstart = gameInfo.ContainsKey("hupstart") ? gameInfo.GetLong("hupstart") : 0;
            }
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            CdTime = cargs2.ContainsKey("-tptout") ? int.Parse(cargs2.GetUtfString("-tptout")) : 100;
            var xiPai = cargs2.ContainsKey("-xipai") ? cargs2.GetUtfString("-xipai") : "0";
            var hasXiPai = int.Parse(xiPai) == 1;

            var touXiang = cargs2.ContainsKey("-touxiang") ? cargs2.GetUtfString("-touxiang") : "0";
            var hasTouXiang = int.Parse(touXiang) == 1;

            var hongHei = cargs2.ContainsKey("-honghei") ? cargs2.GetUtfString("-honghei") : "0";
            var hasHongHei = int.Parse(hongHei) == 1;

            var quickGame= cargs2.ContainsKey("-quickgame") ? cargs2.GetUtfString("-quickgame") : "0";
            _quickGame = int.Parse(quickGame) == 1;

            var laiZi = cargs2.ContainsKey("-laizi") ? cargs2.GetUtfString("-laizi") : "0";

            var ratime = cargs2.ContainsKey("-ratime") ? cargs2.GetUtfString("-ratime") : "0";//正常结算时间
            var qatime = cargs2.ContainsKey("-qatime") ? cargs2.GetUtfString("-qatime") : "0";//急速模式结算事件
            var plenmax = cargs2.ContainsKey("-plenmax") ? cargs2.GetUtfString("-plenmax") : "0";
            var hasStart = int.Parse(plenmax) != 0;

            var haslaiZi = int.Parse(laiZi) == 1;
            var sendObj = SFSObject.NewInstance();
            var autoTime = ratime;
            if (_quickGame)
            {
                autoTime = qatime;
            }
            sendObj.PutBool("giveUp", hasTouXiang);
            sendObj.PutBool("hongHei", hasHongHei);
            sendObj.PutInt("autoTime", int.Parse(autoTime));//结算自动准备时间
            sendObj.PutBool("xiPai", hasXiPai);
            sendObj.PutBool("start", hasStart);

            EventObj.SendEvent("TableViewEvent", "Rule", sendObj);
            EventObj.SendEvent("SelectCardsViewEvent", "Rule", sendObj);
            EventObj.SendEvent("ResultViewEvent", "Rule", sendObj);
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new BjUserInfo();
            userInfo.Parse(userData);
            GetPlayer<BjPlayer>(userInfo.Seat, true).ShowPlayerGold(userInfo.CoinA);
            return userInfo;
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "PutCards":
                    ReJoinPutCard();
                    break;
                case "Result":
                    OnGameResult(data.Data);
                    break;
            }
        }

        public void ReJoinPutCard()
        {
            if (_rejoin)
            {
                foreach (var userInfo in UserInfoDict)
                {
                    var user = (BjUserInfo)userInfo.Value;

                    if (user.HasPutCard)
                    {
                        GetPlayer<BjPlayer>(user.Seat, true).WaitCompareCard();
                    }
                    else
                    {
                        if (user.HasTouXiang)
                        {
                            GetPlayer<BjPlayer>(user.Seat, true).ShowGiveUp();
                            GetPlayer<BjPlayer>(user.Seat, true).WaitCompareCard();
                        }
                        else
                        {
                            if (user.Seat == SelfSeat)
                            {
                                var sendObj = SFSObject.NewInstance();
                                sendObj.PutIntArray("cards", GetPlayerInfo<BjUserInfo>().Cards);
                                AllocateCards(sendObj);
                            }

                            GetPlayer<BjPlayer>(user.Seat, true).CallStartSettleCards();
                        }
                    }
                }
            }
        }

        public void FreshUserOwnerIcon()
        {
            foreach (var userInfo in UserInfoDict)
            {
                var user = userInfo.Value;
                if (OwnerId == user.Id)
                {
                    GetPlayer<BjPlayer>(user.Seat, true).ShowRoomOwnerIcon();
                }
            }
        }

        public void FreshRoomShow()
        {
            //            App.GameData.GStatus = GetPlayerInfo<JlGameUserInfo>().Cards == null ? YxEGameStatus.Over : YxEGameStatus.PlayAndConfine;
            if (!GetPlayer<BjPlayer>().ReadyState)
            {
                EventObj.SendEvent("TableViewEvent", "Ready", null);
            }
            else
            {
                foreach (var userInfo in UserInfoDict)
                {
                    var user = userInfo.Value;
                    GetPlayer<BjPlayer>(user.Seat, true).ReadyState = false;
                }
            }

            if (OwnerId != -1)
            {
                IsCreatRoom = true;
            }
            else
            {
                return;
            }
            var sendObj = SFSObject.NewInstance();
            sendObj.PutUtfString("rule", _rule);
            sendObj.PutInt("roomId", CreateRoomInfo.RoomId);
            sendObj.PutInt("curRound", CreateRoomInfo.CurRound);
            sendObj.PutInt("maxRound", CreateRoomInfo.MaxRound);
            EventObj.SendEvent("TableViewEvent", "RoomInfo", sendObj);

            EventObj.SendEvent("RecordViewEvent", "Join", CreateRoomInfo.CurRound);
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
            EventObj.SendEvent("HupUpViewEvent", "Rejoin", sendObj);
        }

        public void ToStartGame()
        {
            EventObj.SendEvent("TableViewEvent", "Start", null);
        }

        public void AllocateCards(object data)
        {
            var response = (ISFSObject)data;
            var cards = response.GetIntArray("cards");
            var sendObj = SFSObject.NewInstance();
            sendObj.PutIntArray("cards", cards);
            sendObj.PutInt("cd", _putTime);
            YxDebug.LogArray(cards);
            EventObj.SendEvent("TipEvent", "StartGame", null);
            EventObj.SendEvent("SoundEvent", "PlayerEffect", new BjSound.SoundData(BjSound.EnAudio.StartGame));

            //            foreach (var user in UserInfoDict)
            //            {
            //                var player = (BjUserInfo)user.Value;
            //                YxClockManager.BeginWaitPlayer(player.Seat, _putTime);
            //            }

            EventObj.SendEvent("SelectCardsViewEvent", "Start", sendObj);
        }

        public void PutCards(ISFSObject response)
        {
            PutCardsData putCardsData = new PutCardsData(response);
            if (putCardsData.Seat == SelfSeat)
            {
                GetPlayer<BjPlayer>(putCardsData.Seat, true).SetPutCards(putCardsData.PutCards, putCardsData.RealCards);
                EventObj.SendEvent("SelectCardsViewEvent", "Close", null);
            }
            GetPlayer<BjPlayer>(putCardsData.Seat, true).WaitCompareCard();
        }

        public void CompareCards(ISFSObject response)
        {
            var result = response.ContainsKey("result") ? response.GetSFSArray("result") : null;
            if (result != null)
            {
                _compareDatas = new List<CompareData>();
                for (int i = 0; i < result.Count; i++)
                {
                    var obj = result.GetSFSObject(i);
                    CompareData compareData = new CompareData(obj);

                    {
                        _compareDatas.Add(compareData);
                    }
                }

                StartCoroutine(ShowCompare());
                EventObj.SendEvent("RecordViewEvent", "CompareData", _compareDatas);
            }
        }

        IEnumerator ShowCompare()
        {
            foreach (var user in UserInfoDict)
            {
                var player = (BjUserInfo)user.Value;
                if (GetPlayer<BjPlayer>(player.Seat, true).DaosCards == null)
                {
                    GetPlayer<BjPlayer>(player.Seat, true).WaitCompareCard();
                }
            }
            EventObj.SendEvent("SelectCardsViewEvent", "Close", null);

            var compareDatas = _compareDatas;
            EventObj.SendEvent("TipEvent", "Compare", null);
            EventObj.SendEvent("SoundEvent", "PlayerEffect", new BjSound.SoundData(BjSound.EnAudio.StartCompare));

            var dicDaoSort =new Dictionary<int, List<int>>();
            var firstLoad = new List<int>();
            for (int i = 0; i < compareDatas.Count; i++)
            {
                firstLoad.Add(compareDatas[i].DaosScore[0]);
            }
            firstLoad.Sort();
            dicDaoSort[0] = firstLoad;
            var secondLoad = new List<int>();
            for (int i = 0; i < compareDatas.Count; i++)
            {
                secondLoad.Add(compareDatas[i].DaosScore[1]);
            }
            secondLoad.Sort();
            dicDaoSort[1] = secondLoad;
            var endLoad = new List<int>();
            for (int i = 0; i < compareDatas.Count; i++)
            {
                endLoad.Add(compareDatas[i].DaosScore[2]);
            }
            endLoad.Sort();
            dicDaoSort[2] = endLoad;

            var excuteTime = 0.08f;

            if (!_quickGame)
            {
                excuteTime = 0.15f;
            }
            yield return new WaitForSeconds(excuteTime * 8);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < dicDaoSort[j].Count; k++)
                {
                    for (int i = 0; i < compareDatas.Count; i++)
                    {
                        if (compareDatas[i].DaosScore[j]== dicDaoSort[j][k])
                        {
                            var i1 = i;
                            int[] daoCards = compareDatas[i].DaosCards.Count != 0 ? compareDatas[i].DaosCards[j] : null;
                            int[] daoRealCards = compareDatas[i].DaoRealCards.Count != 0 ? compareDatas[i].DaoRealCards[j] : null;

                            var daoTypes = -1;
                            if (compareDatas[i].DaosTypes.Count != 0)
                            {
                                daoTypes = compareDatas[i].DaosTypes[j];
                            }
                            GetPlayer<BjPlayer>(compareDatas[i].Seat, true).SetCompareData(daoCards, daoRealCards, daoTypes, excuteTime, (enAudio) =>
                            {
                                EventObj.SendEvent("SoundEvent", "PersonSound", new BjSound.SoundData(enAudio, GetPlayer<BjPlayer>(compareDatas[i1].Seat, true).Info.SexI));
                            });
                            yield return new WaitForSeconds(excuteTime * 3);
                        }
                    }
                }
                for (int i = 0; i < compareDatas.Count; i++)
                {
                    if (compareDatas[i].DaosScore.Count != 0)
                    {
                        GetPlayer<BjPlayer>(compareDatas[i].Seat, true).SetDaoScore(compareDatas[i].DaosScore[j]);
                    }
                }

                if (_quickGame)
                {
                    yield return new WaitForSeconds(excuteTime * 8);
                }
                else
                {
                    yield return new WaitForSeconds(excuteTime * 4);
                }
            }
            yield return new WaitForSeconds(excuteTime * 10);
            for (int i = 0; i < compareDatas.Count; i++)
            {
                GetPlayer<BjPlayer>(compareDatas[i].Seat, true).SetXiPaiData(compareDatas[i].XiPaiScore, compareDatas[i].XiPaiInfos);
            }
        }


        public void Result(ISFSObject response)
        {
            _resultData = new ResultData(response);
            EventObj.SendEvent("TableViewEvent", "Result", null);
            EventObj.SendEvent("RecordViewEvent", "ResultData", _resultData);
            EventObj.SendEvent("RecordViewEvent", "RoomData", CreateRoomInfo);

        }

        public void OnGameResult(object data)
        {
            StartCoroutine(ResultShow(data));
        }

        IEnumerator ResultShow(object data)
        {
            if (!_quickGame)
            {
                var coinParent = (Transform)data;
                var loseSeats = _resultData.LoseSeats;
                var winSeats = _resultData.WinSeats;
                if (winSeats.Count != 0)
                {
                    for (int i = 0; i < loseSeats.Count; i++)
                    {
                        GetPlayer<BjPlayer>(loseSeats[i], true).ResultLoseCoinFly(coinParent);
                    }

                    var coinCount = loseSeats.Count * 13;
                    var winCoinCount = Mathf.Floor(coinCount / winSeats.Count);
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < winSeats.Count; i++)
                    {
                        var coinNum = winCoinCount;
                        if (i == winSeats.Count - 1 && coinCount - (int)(winCoinCount * (i + 1)) > 0)
                        {
                            coinNum = coinCount - (int)(winCoinCount * i);
                        }
                        GetPlayer<BjPlayer>(winSeats[i], true).ResulWinCoinFly(coinParent, (int)coinNum);

                    }
                }
                yield return new WaitForSeconds(0.7f);
            }

            EventObj.SendEvent("ResultViewEvent", "ResultData", _resultData);

            var sendObj = SFSObject.NewInstance();
            sendObj.PutInt("selfSeat", SelfSeat);
            sendObj.PutInt("roomId", CreateRoomInfo.RoomId);
            sendObj.PutInt("currentRound", CreateRoomInfo.CurRound);
            sendObj.PutInt("maxRound", CreateRoomInfo.MaxRound);

            EventObj.SendEvent("ResultViewEvent", "RoomData", sendObj);

            EventObj.SendEvent("ResultViewEvent", "PlayerList", PlayerList);

            EventObj.SendEvent("ResultViewEvent", "Show", _compareDatas);
        }

        public void GiveUp(ISFSObject response)
        {
            var seat = response.GetInt("seat");
            GetPlayer<BjPlayer>(seat, true).WaitCompareCard();
            GetPlayer<BjPlayer>(seat, true).ShowGiveUp();
        }

        public void Error(ISFSObject response)
        {
            EventObj.SendEvent("SelectCardsViewEvent", "Error", null);
        }
    }
}
