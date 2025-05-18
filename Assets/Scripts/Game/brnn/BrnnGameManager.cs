using System;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn
{
    public class BrnnGameManager : YxGameManager
    {
        //--------------------------------
        public UserListCtrl UserListCtrl;
        public BetCtrl BetCtrl;
        public ProgressCtrl ProgressCtrl;
        public CardsCtrl CardsCtrl;
        public ResultWindow ResultWin;
        public ShowNumCtrl ShowNumCtrl;
        public ResultListCtrl ResultListCtrl;
        public ParticleCtrl ParticleCtrl;
        public ApplyCtrl ApplyCtrl;
        public ShowText ShowText;

       

        public override void OnGetGameInfo(ISFSObject requestData)
        {
            ShowNumCtrl.Init();
            UserListCtrl.RefreshBanker(requestData);
            CheckReJion(requestData);
            ProgressCtrl.SetNum(requestData);
            BetCtrl.ResetChip();
            if(requestData.ContainsKey("curbkround"))
            {
                App.GetGameData<BrnnGameData>().CurrentBanker.SetBankerTime(requestData.GetInt("curbkround"));
            }
        }

     


        public virtual void OnGetPlayers(ISFSObject responseData)
        {
            if (!responseData.IsNull("playerlist"))
            {
                string[] playerStr = responseData.GetUtfStringArray("playerlist");
                UserListCtrl.RefreshPlayer(playerStr);
            }
        }


        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            
        }



        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            switch (type)
            {
                case RequestType.Bet:
                    BetCtrl.Bet(response);
                    ProgressCtrl.RefreshNum(response);
                    ShowNumCtrl.RefreshNum(response);
                    if (ResultWin != null) { ResultWin.Hide();}
                    break;
                case RequestType.BankerList:
                    UserListCtrl.RefreshBanker(response);
                    ProgressCtrl.SetNum(response);
                    BetCtrl.ShowChip();
                    break;
                case RequestType.BeginBet:
                    CardsCtrl.CancelInvoke();
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    gdata.BeginBet = true;
                    CardsCtrl.ReSetPonits();
                    CardsCtrl.ReSetGiveCardsStatus();
                    ShowNumCtrl.Reset();
                    ReSetGame();
                    BetCtrl.ShowChip();
                    BetCtrl.Reset();
                    ProgressCtrl.ReSetCountdown(16);
                    ProgressCtrl.BeginCountdown();
                    ApplyCtrl.RefreshBanker();
                    break;
                case RequestType.EndBet:
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    
                    CardsCtrl.GetCards(response);
                    ProgressCtrl.EndCountdown();
                    break;
                case RequestType.GiveCards:
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    CardsCtrl.BeginGiveCards(response);
                    UserListCtrl.RefreshBanker(response);
                    break;
                case RequestType.Result:
                    
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    CardsCtrl.ReceiveResult(response);
                    if (ResultWin != null)
                    {
                        ResultWin.ShowResultView(response);
                    }
                    UserListCtrl.RefreshBanker(response);
                    gdata.GetPlayer<BrnnPlayer>().ReSet();
                    ApplyCtrl.HideApplyBanker();
                    ProgressCtrl.SetNum(response);
                    break;
                case RequestType.GroupBet:

                    BetCtrl.GroupBet(response);
                    ProgressCtrl.GroupRefreshNum(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    break;
            }

            OnGetPlayers(response);
        }

             

        protected void CheckReJion(ISFSObject requestData)
        {
            long st = requestData.GetLong("st");
            long ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 15)
                {
                    CardsCtrl.ReSetPonits();
                    App.GetGameData<BrnnGameData>().BeginBet = true;
                    BetCtrl.ShowChip();
                    ProgressCtrl.ReSetCountdown(Convert.ToInt32(ct - st));
                    ProgressCtrl.BeginCountdown();
                }
                ShowNumCtrl.SetNum(requestData);
            }
        }

        public void ReSetGame()
        {
            foreach (var temp in CommonObject.CurrentChipList)
            {
                if (temp == null) continue;
                temp.SetActive(false);
                DestroyObject(temp);

            }
            CommonObject.CurrentChipList.Clear();
            foreach (var temp in CommonObject.CardArray2)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in CommonObject.CardArray0)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in CommonObject.CardArray1)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }
            }
            foreach (var temp in CommonObject.CardArray3)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in CommonObject.CardArray4)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }
            }
        }
    }
}

