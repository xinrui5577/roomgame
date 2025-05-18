using System.Collections;
using Assets.Scripts.Game.slyz.Network.Protocol;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.slyz
{
    public class SlyzGameManager : YxGameManager {

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            // 显示带入余额 注下注 刷新奖池
            if (App.GetGameData<SlyzGameData>().GMessage.OnShowAllGoldInfo != null)
            {
                App.GetGameData<SlyzGameData>().GMessage.OnShowAllGoldInfo();
            }
            if (App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod != null)
            {
                App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod();
            }
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int type, ISFSObject gameInfo)
        { 
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<SlyzGameData>();
            var player = gdata.GetPlayer(gdata.SelfLocalSeat);
            switch (type)
            {
                case SlyzRequestCmd.StartGame:
                {
                        gdata.SetStartData(response);
                        // 更新玩家余额 但是此时先不刷新 等牌局翻牌完成再刷新
                        player.Info.CoinA = response.GetLong("ttgold");
                        if (App.GetGameData<SlyzGameData>().GMessage.OnGameStart != null)
                            App.GetGameData<SlyzGameData>().GMessage.OnGameStart();
                    }
                    break;
                case SlyzRequestCmd.JackpotChange:
                    {
                        // 更新奖池
                        gdata.Caichi = response.GetInt("caichi"); ;
                        if (App.GetGameData<SlyzGameData>().GMessage.OnShowAllGoldInfo != null)
                        App.GetGameData<SlyzGameData>().GMessage.OnShowAllGoldInfo();
                }
                    break;
                case SlyzRequestCmd.GetMessage:
                {
                    var data = response.GetUtfString("data");
                    StartCoroutine(ShowNotice(data));
                }
                    break;
                default:
                    break;
            }
        }

        private IEnumerator ShowNotice(string data)
        {
            //18-01-08 16:50:56,奖励名称,2000,235,游客_225891
            yield return new WaitForSeconds(1f);
            Facade.Instance<MusicManager>().Play("Winning");
            App.GetGameData<SlyzGameData>().AddPrizeData(data);
        }


        public override void UserOut(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
        }
    }
}
