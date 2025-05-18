using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.lhc
{
    public class LhcGameData : YxPhpGameData
    {
        public YxBaseGamePlayer CurrentBanker;
        public List<int> BetAllows = new List<int>();
        public List<int> SelfBets=new List<int>();
        public Dictionary<int,int> AllBets=new Dictionary<int, int>();
        public int BetPosNum;
        public int CurrentBet;
        public string LoseAnteDes;
        public int LotteryNum;
        public int LastLotteryNum;
        public int ResultTime;
//        public string CurrentTime;
        public bool WaitLottery;
        public string Desc;
        public List<int> LastRecord;
        public object HistoryData;
      
        public string[] BetStrings;
        [SerializeField]
        public List<PosColor> BetPosColors;
        public Dictionary<string, object> BetDic;
        public int BankAdd;//上庄条件 必须要比庄家的上庄的值多这些
        public string LoseAntes;
        public int StopBetTime;

        protected override void InitGameData(Dictionary<string, object> gameInfo)
        {
            base.InitGameData(gameInfo);
            if (gameInfo.ContainsKey("errorMessage"))
            {
                App.GetGameManager<LhcGameManager>().ErrorMessage.ShowBtnAndMsg(gameInfo["errorMessage"].ToString());
                return;
            }
            HistoryData = gameInfo.ContainsKey("historyData") ? gameInfo["historyData"] : null;

            ResultTime = gameInfo.ContainsKey("resultTime") ? int.Parse(gameInfo["resultTime"].ToString()) : 0;
//            CurrentTime = gameInfo.ContainsKey("startTime") ? gameInfo["startTime"].ToString() : "";
            WaitLottery = gameInfo.ContainsKey("waitLottery") && bool.Parse(gameInfo["waitLottery"].ToString());
            Desc = gameInfo.ContainsKey("desc") ? gameInfo["desc"].ToString() : "";
            var lastRecord = gameInfo.ContainsKey("lastRecord") ? gameInfo["lastRecord"].ToString() : "";
            if (!string.IsNullOrEmpty(lastRecord))
            {
                var strings = lastRecord.Split(',');
                foreach (var str in strings)
                {
                    LastRecord.Add(int.Parse(str));
                }
            }
            var cargs2 = gameInfo.ContainsKey("cargs2") ? gameInfo["cargs2"] : null; 
            var cargs = cargs2 as Dictionary<string, object>;
            if (cargs != null)
            {
                BankAdd = cargs.ContainsKey("-bankAdd") ? int.Parse(cargs["-bankAdd"].ToString()) * 10000 : 100000;
                LoseAntes = cargs.ContainsKey("-loseAntes") ? cargs["-loseAntes"].ToString() : "";
                StopBetTime= cargs.ContainsKey("-stopbet") ?int.Parse(cargs["-stopbet"].ToString()) : 10;
            }

            var banker = gameInfo.ContainsKey("banker") ? gameInfo["banker"]: null;
            PararmBankInfo(banker);

            LoseAnteDes = gameInfo.ContainsKey("loseAnteDes") ? gameInfo["loseAnteDes"].ToString() : "";
            LotteryNum = gameInfo.ContainsKey("lotteryNum") ? int.Parse(gameInfo["lotteryNum"].ToString()) : 0;
            LastLotteryNum = gameInfo.ContainsKey("lastLotteryNum")
                ? int.Parse(gameInfo["lastLotteryNum"].ToString())
                : 0;
            var betAllowsStr = gameInfo.ContainsKey("betAllows") ? gameInfo["betAllows"].ToString() : "";
            PararmBetAllows(betAllowsStr);
            var selfBet = gameInfo.ContainsKey("selfBet") ? gameInfo["selfBet"] : null;
            PararmSelfBets(selfBet);

            var betDatas = gameInfo.ContainsKey("betDatas") ? gameInfo["betDatas"] : null;
            PararmBetDatas(betDatas);
            
            var allBet = gameInfo.ContainsKey("allBet") ? gameInfo["allBet"] : null;

            PararmAllBets(allBet);
        }
        /// <summary>
        /// 刷新界面庄稼或者玩家
        /// </summary>
        /// <param name="response"></param>
        public void FreshPlayerView(Dictionary<string, object> response)
        {
            var banker = response.ContainsKey("banker") ? response["banker"] : null;
            PararmBankInfo(banker);

            var user = response.ContainsKey("user") ? response["user"] : null;
            PararmCurUserInfo(user);
        }

        private void PararmBankInfo(object banker)
        {
            if (banker is bool) return; 
            if(banker==null) return;
            var bankeInfo = banker as Dictionary<string, object>;
            YxBaseGameUserInfo baseGameUserInfo = new YxBaseGameUserInfo();
            baseGameUserInfo.Parse(bankeInfo);
            CurrentBanker.Info = baseGameUserInfo;
            if (CurrentBanker.Info.Seat != SelfSeat)
            {
                App.GetGameManager<LhcGameManager>().ChangeBankBtnState();
            }
        }

        private void PararmCurUserInfo(object curUser)
        {
            if (curUser == null) return;
            var playerInfo = GetPlayerInfo();
            playerInfo.Parse(curUser as Dictionary<string,object>);
            GetPlayer().UpdateView(playerInfo);
        }

        public void PararmBetAllows(string str)
        {
            if (string.IsNullOrEmpty(str))return;
            BetAllows.Clear();
            var betAllows = str.Split(',');
            foreach (var betAllow in betAllows)
            {
                BetAllows.Add(int.Parse(betAllow));
            }
        }

        public void PararmSelfBets(object selfBet)
        {
            SelfBets.Clear();
            if (selfBet == null)
            {
                for (int i = 1; i < BetPosNum + 1; i++)
                {
                    SelfBets.Add(0);
                }
                return;
            }
            var selfBets = selfBet as Dictionary<string, object>;
            if (selfBets != null)
            {
                for (int i = 1; i < BetPosNum + 1; i++)
                {
                    SelfBets.Add(int.Parse(selfBets[i.ToString()].ToString()));
                }
            }
        }

        public void PararmAllBets(object allBet)
        {
            AllBets.Clear();
            if (allBet == null)
            {
                return;
            }
            var allBetList = allBet as List<object>;
          
            if (allBetList != null)
            {
                for (int i = 0; i < allBetList.Count; i++)
                {
                    var allBetData = allBetList[i] as Dictionary<string, object>;
                    if (allBetData != null)
                        AllBets[int.Parse(allBetData["bets_seat"].ToString())] =
                            int.Parse(allBetData["bets_sum"].ToString());
                }
            }
        }

        public void PararmBetDatas(object betData)
        {
            if (betData is Dictionary<string, object>)
            {
                BetDic = betData as Dictionary<string, object>;
             
            }
        }

        public long ApplyBankCondition()
        {
            return GetPlayerInfo().CoinA - BankAdd - CurrentBanker.Coin;
        }
    }
    [Serializable]
    public class PosColor
    {
        public string Pos;
        public int Value;
        public string Color;
    }
}
