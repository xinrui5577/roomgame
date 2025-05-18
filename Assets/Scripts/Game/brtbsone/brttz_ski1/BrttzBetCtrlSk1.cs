using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using YxFramwork.Framework.Core;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brtbsone.brttz_ski1
{
    public class BrttzBetCtrlSk1 : BetCtrl
    {
        public override void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var gdata = App.GetGameData<BrttzGameData>();
            var selfSeat = gdata.GetPlayerInfo().Seat;
            var manager = App.GetGameManager<BrttzGameManager>();

            int maxGold = 0;
            // ReSharper disable once TooWideLocalVariableScope
            foreach (ISFSObject item in sfsArray)
            {
                string p = item.GetUtfString("p");
                int target = GetInt(p);
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;
                if (manager.PlayerManager.TablePlayerBet(seat, gold, target))
                {
                    continue;
                }

                if (gold >= maxGold)
                {
                    maxGold = gold;
                }
                OtherMenBet(target, gold);
            }

            PlayAmazedSoud(maxGold, ref MaxGroupBet);
        }

        public override void Bet(ISFSObject responseData)
        {
            var gold = responseData.GetInt(Parameter.Gold);
            var seat = responseData.GetInt(Parameter.Seat);
            var p = responseData.GetUtfString("p");
            int target = GetInt(p);
            var gdata = App.GetGameData<BrttzGameData>();
            var manager = App.GetGameManager<BrttzGameManager>();
            if (seat == gdata.SelfSeat)
            {
                SelfMenBet(target, gold);
                gdata.ThisCanInGold = gdata.ThisCanInGold - gold;
                App.GetGameManager<BrttzGameManager>().CanQuitGame = false;
                manager.PlayerManager.TablePlayerBet(seat, gold, target, true);
                return;
            }
            if (manager.PlayerManager.TablePlayerBet(seat, gold, target))
            {
                return;
            }
            if (gold > gdata.AnteRate[gdata.AnteRate.Count - 1])
            {
                OtherMenBet(target, gold);
                return;
            }
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                //InstantiateChip(ChipCfg.DeskAreas[target], startPos[0], gold);
                InstantiateChip(target, startPos[0], gold);
                Facade.Instance<MusicManager>().Play("Bet");
            }
        }
    }
}
