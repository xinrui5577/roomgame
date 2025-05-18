using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.bjlb
{
    public class BjlGameServer : YxGameServer
    { 
        //--------------------------------
        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic["bank"] = OnBank;
            callBackDic["buy"] = OnBuy;
        }
      
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (App.GetGameData<BjlGameData>().CouldOut)
                {

                    YxMessageBox.Show(new YxMessageBoxData
                    {
                        Msg = "确定要退出游戏吗!?",
                        IsTopShow = false,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                App.QuitGame();
                            }
                        },
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    });
                }
                else
                {
                    YxMessageBox.DynamicShow(new YxMessageBoxData
                    {
                        Msg = "正在游戏中,无法退出游戏!?",
                        IsTopShow = false,
                        Delayed = 5,
                    });
                }
            }
        }

        public virtual void OnBank(ISFSObject requestData)
        {
            var gdata = App.GetGameData<BjlGameData>();
            YxWindowManager.HideWaitFor();
            if (requestData.GetBool("result"))
            {
                var self = gdata.GetPlayer();
                self.Info.Parse(requestData);
                self.UpdateView();
            }
        }

        public virtual void OnBuy(ISFSObject requestData)
        {
            var gdata = App.GetGameData<BjlGameData>();
            YxWindowManager.HideWaitFor();
            if (requestData.GetBool("result"))
            {
                var self = gdata.GetPlayer();
                self.Info.Parse(requestData);
                self.UpdateView();
            }          
        }

        protected void SetStopBet()
        {
            App.GetGameManager<BjlGameManager>().ProgressCtrl.StopClock(RequestType.EndBet);
        }


        public void OnGetPlayers(ISFSObject responseData)
        {
            if (responseData.ContainsKey("playerlist"))
            {
                string[] playerStr = responseData.GetUtfStringArray("playerlist");
                App.GetGameManager<BjlGameManager>().UserListCtrl.RefreshPlayer(playerStr);
            }
        }
      
        public void UserBet(int table, int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", RequestType.Bet);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void  ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyBanker);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyQuit);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void BankL(int gold, string password)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("gold", gold);
            sfsObject.PutUtfString("password", password);
            SendRequest(new ExtensionRequest(GameKey + "bank", sfsObject));
        }
        public void Exchange(int cash)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("cash", cash);
           SendRequest(new ExtensionRequest(GameKey + "buy", sfsObject));
        }
        public void Reward()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
         SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
    } 
 
}

