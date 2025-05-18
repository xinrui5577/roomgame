using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetRectMode : MonoBehaviour
    {
        [SerializeField]
        protected EBetRectType _betRectType;
          
        public void OnMouseDown()
        {
            Facade.Instance<MusicManager>().Play("chouma");
            var gdata = App.GetGameData<Brnn3dGameData>();
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            var goldNum = gdata.AnteRate[gdata.CoinType];
            gdata.IsOut = false;
            var noteUI = gameMgr.TheNoteUI;
            if (gdata.IsBet == false)
            {
                noteUI.Note("此时不能下注！");
                return;
            }
            if (IsPointerOverUIObject())
            {
                return;
            }
            if (gdata.BankList == null || gdata.BankList.Size() == 0)
            {
                noteUI.Note("没有庄家不能下注！");
                return;
            }
            else
            {
                var banekr = gdata.GetPlayerInfo(gdata.B,true);
                if (goldNum > banekr.CoinA * 0.1)
                {
                    noteUI.Note("最大筹码已超出庄家拥有钱的1/10！");
                }
            }
            if (gameMgr.TheUpUICtrl.TheBankersManager.BankerIsSelf())
            {
                noteUI.Note("庄家是自己，不能下注！");
                return;
            }
            var self = gdata.GetPlayerInfo();
            if (goldNum > self.CoinA * 0.1)
            {
                noteUI.Note("最大筹码已超出您拥有钱的1/10！");
            } 
            if (self.CoinA < goldNum)
            {
                noteUI.Note("已到最大下注数！");
            }
            var betRect = (int) _betRectType;
            gdata.BetPosSelf = betRect;
            App.GetRServer<Brnn3dGameServer>().UserBet(betRect, goldNum);
            //BetMode.Instance.InstanceCoinDemo(GameConfig.Instance.iChouMaType, 3, App.GetGameData<GlobalData>().CurrentUser.Seat);
            gdata.GStatus = YxEGameStatus.PlayAndConfine;
        }

        public bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        public enum EBetRectType
        {
            EastRect = 0,
            SouthRech,
            WestRect,
            NorthRect = 3
        }
    }
}


