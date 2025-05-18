using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.hg
{
    public class HgGameManager : YxGameManager
    {
        public EventObject EventObj;

        public List<HgPlayer> SpecialPlayers = new List<HgPlayer>();

        public int CurrentTableCount;

        private HgGameData _gdata
        {
            get { return App.GetGameData<HgGameData>(); }
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
            }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            TableUserShow();
            EventObj.SendEvent("TrendViewEvent", "SetRecord", false);
            EventObj.SendEvent("BetViewEvent", "Init", null);
            EventObj.SendEvent("TableViewEvent", "SetMusic", null);
            EventObj.SendEvent("TrendWindowEvent", "Init", null);
        }

        private void TableUserShow()
        {
            var count = Math.Min(_gdata.GoldRank.Count, SpecialPlayers.Count);
            CurrentTableCount = count;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < _gdata.AllUserInfos.Count; j++)
                {
                    //                    Debug.LogError("gdata.GoldRank[i]" + gdata.GoldRank[i] +"i"+i+ "gdata.AllUserInfos[j].Seat"+ gdata.AllUserInfos[j].Seat+"j"+j);
                    if (_gdata.GoldRank[i] == _gdata.AllUserInfos[j].Seat)
                    {
                        SpecialPlayers[i].Info = _gdata.AllUserInfos[j];
                    }
                }
            }
        }
        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }


        public override void GameStatus(int status, ISFSObject info)
        {
            EventObj.SendEvent("TipViewEvent", "ShowWatch", _gdata.AllowPlay);
            var statusData = info.GetInt("status");
            switch (statusData)
            {
                case (int)GameState.Waiting:
                case (int)GameState.Start:
                case (int)GameState.ZhuangGold:
                    break;
                case (int)GameState.RollDice:
                case (int)GameState.XiaZhu:
                case (int)GameState.Over:
                    EventObj.SendEvent("CardViewEvent", "Creat", null);
                    EventObj.SendEvent("TipViewEvent", "ShowWait", null);
                    break;
            }
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            EventObj.SendEvent("TipViewEvent", "Clear", null);
        }

        //消息容器
        [HideInInspector]
        public Queue<ISFSObject> ResponseQueue = new Queue<ISFSObject>();

        //是否开启消息延迟
        public bool _laterSend = false;

        public bool LaterSend
        {
            get { return _laterSend; }
            set
            {
                _laterSend = value;
                if (!value)
                {
                    while (ResponseQueue.Count > 0 && !LaterSend)
                    {
                        OnServerResponse(ResponseQueue.Dequeue());
                    }
                }
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            if (LaterSend)
            {
                ResponseQueue.Enqueue(response);
            }
            else
            {
                OnServerResponse(response);
            }
        }

        public void OnServerResponse(ISFSObject response)
        {
            var type = response.GetInt("type");
            switch (type)
            {
                case (int)GameResponseState.ZhuangChange:
                    var bankListData = new BankListData(response);
                    EventObj.SendEvent("BankListEvent", "FreshBankList", bankListData);
                    EventObj.SendEvent("GameDataEvent", "FreshBankInfo", bankListData);
                    break;
                case (int)GameResponseState.BeginBet://给goldrank 和ssz
                    Clear();
                    var rankData = new RankData();
                    rankData.SetRank(response);
                    _gdata.GoldRank = rankData.GoldRank;
                    TableUserShow();

                    EventObj.SendEvent("TipViewEvent", "ShowCompare", null);
                    EventObj.SendEvent("TipViewEvent", "ShowTime", response.GetInt("cd"));
                    _gdata.BeginBet = true;
                    EventObj.SendEvent("BetViewEvent", "Show", null);
                    EventObj.SendEvent("CardViewEvent", "Creat", null);

                    EventObj.SendEvent("TrendWindowEvent", "Close", null);
                    break;
                case (int)GameResponseState.StopBet:
                    _gdata.BeginBet = false;
                    EventObj.SendEvent("BetViewEvent", "Hide", null);
                    EventObj.SendEvent("TipViewEvent", "StopBet", null);
                    break;
                case (int)GameResponseState.Bet:
                    EventObj.SendEvent("BetViewEvent", "Bet", response);

                    break;
                case (int)GameResponseState.RollResult://牌值信息
                    CardValue cardValue = new CardValue(response);
                    EventObj.SendEvent("CardViewEvent", "Show", cardValue);
                    break;
                case (int)GameResponseState.GameResult:
                    App.GameData.GStatus = YxEGameStatus.Normal;
                    ResultData resultData = new ResultData(response);
                    _gdata.RecordCardType.Add(resultData.WinType);
                    _gdata.RecordSpot.Add(resultData.WinArea);
                    SpecialPlayers[0].Clear();
                    EventObj.SendEvent("TipViewEvent", "ShowWatch", _gdata.AllowPlay);
                    EventObj.SendEvent("TrendViewEvent", "SetRecord", true);
                    EventObj.SendEvent("TipViewEvent", "ShowResult", resultData);
                    
                    var dic = new Dictionary<string, object>();
                    dic["recordSpot"] = resultData.WinArea;
                    dic["recordCardType"] = resultData.WinType.ToString();
                    EventObj.SendEvent("TrendWindowEvent", "AddOne", dic);
                    
                    break;
                case (int)GameResponseState.FlushBet:
                    EventObj.SendEvent("BetViewEvent", "GroupBet", response);
                    break;
                case (int)GameResponseState.ClearRecord:
                    EventObj.SendEvent("TrendWindowEvent", "Clear", null);
                    break;
            }
        }

        public override void BeginReady()
        {
            base.BeginReady();
            EventObj.SendEvent("CardViewEvent", "Clear", null);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            var userInfo = new HgUserInfo();
            userInfo.Parse(sfsObject.GetSFSObject("user"));
            _gdata.AllUserInfos.Add(userInfo);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            var seat = responseData.GetInt("seat");

            for (int i = 0; i < _gdata.AllUserInfos.Count; i++)
            {
                if (seat == _gdata.AllUserInfos[i].Seat)
                {
                    _gdata.AllUserInfos.RemoveAt(i);
                }
            }
        }


        private void Clear()
        {
            foreach (var user in SpecialPlayers)
            {
                user.gameObject.SetActive(false);
            }
            EventObj.SendEvent("TipViewEvent", "CloseFlash", null);
        }
    }
}
