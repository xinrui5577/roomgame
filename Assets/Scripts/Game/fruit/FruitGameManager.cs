using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
// ReSharper disable All

namespace Assets.Scripts.Game.fruit
{
    public class FruitGameManager : YxGameManager
    {
        /// <summary>
        /// 押注后获得押注结果
        /// </summary>
        /// <param name="resultData">押注结果</param>
        public void OnGetChipResult(ISFSObject resultData)
        {
            if (resultData == null) return;
            if (!resultData.ContainsKey("type") || resultData.GetInt("type") != 1) return;
            var winMoney = resultData.GetInt("win");
            var gdata = App.GameData;
            var playerInfo = gdata.GetPlayerInfo();
            playerInfo.WinCoin = winMoney;
            SlotInfo.LotteryPoint = resultData.GetInt("rs");

            var args = new UiTeventArgs { AnimState = LightItemCtrl.ItemAnimState.End, HasWin = winMoney > 0 };
            //如果是luckpoint
            if (resultData.ContainsKey("randType"))
            {
                args.RandType = resultData.GetInt("randType");

                YxDebug.LogError("args.RandType" + args.RandType);

                var luckPoints = resultData.GetIntArray("all");
                SlotInfo.GoodLuckPoints.Clear();
                foreach (var luckPoint in luckPoints)
                {
                    SlotInfo.GoodLuckPoints.Add(luckPoint);
                }
            }
            Facade.EventCenter.DispatchEvent(FruitEventType.ChipUpdate, args);
        }

        /// <summary>
        /// 比大小后获得结果
        /// </summary>
        /// <param name="resultData">比大小结果</param>
        public void OnGetCasinoResult(ISFSObject resultData)
        {
            if (resultData == null) return;
            if (!resultData.ContainsKey("type") || resultData.GetInt("type") != 2) return;
            var gold = resultData.GetInt("gold");
            var dx = resultData.GetInt("dx");
            var rs = resultData.GetInt("rs");

            SlotInfo.CasinoNum = rs;
            var gdata = App.GameData;
            var playerInfo = gdata.GetPlayerInfo();
            playerInfo.WinCoin = 0;
            switch (dx)
            {
                case 0:  //“猜小”
                    if (rs < 8)  //勝利
                    {
                        playerInfo.WinCoin = gold;
                    }
                    break;
                default:  //“猜大”
                    if (rs > 7)  //勝利
                    {
                        playerInfo.WinCoin = gold;
                    }
                    break;
            }
            Facade.EventCenter.DispatchEvent<FruitEventType, object>(FruitEventType.CasinoResult);
        }

        /// <summary>
        /// 重新开始游戏后获得玩家信息
        /// </summary>
        /// <param name="resultData">存放玩家信息</param>
        public void OnRegameUserInofo(ISFSObject resultData)
        {
            if (resultData == null) return;
            if (!resultData.ContainsKey("type") || resultData.GetInt("type") != 3) return;
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            player.Coin = resultData.GetLong("totalGold");
            player.WinCoin = 0;
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            SlotInfo.ClearFruitSlotListToZero();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int type, ISFSObject gameInfo)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch (type)
            {
                //押水果返回比赛的结果
                case 1:
                    OnGetChipResult(response);
                    break;
                //押大小返回的结果
                case 2:
                    OnGetCasinoResult(response);
                    break;
                //重新开始获得用户总金币数
                case 3:
                    OnRegameUserInofo(response);
                    break;
            }
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
