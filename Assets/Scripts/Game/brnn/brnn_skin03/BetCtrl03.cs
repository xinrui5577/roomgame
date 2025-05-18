using Assets.Scripts.Game.brnn.brnn_skin02;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn.brnn_skin03
{
    public class BetCtrl03 : BetCtrl02 {


        public override void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var tablePlayerMgr = App.GetGameManager<BrnnGameManager03>().TabelPlayerMgr;
            var gdata = App.GameData;
            var selfSeat = gdata.SelfSeat;

            int maxGold = 0;

            foreach (ISFSObject item in sfsArray)
            {
                int p = item.GetInt("p");
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;
               
                if (tablePlayerMgr.TableBet(seat, gold, p))
                {
                    continue;
                }

                if (gold >= maxGold)
                {
                    maxGold = gold;
                }

                if (startPos.Length > 0)
                {
                    InstantiateChip(p, startPos[0], gold);
                }
            }

            PlayAmazedSoud(maxGold, ref MaxGroupBet);
        }

        public override void Bet(ISFSObject responseData)
        {
            int p = responseData.GetInt("p");
            int gold = responseData.GetInt("gold");
            int seat = responseData.GetInt("seat");
            var gdata = App.GetGameData<BrnnGameData>();
            if (seat == gdata.SelfSeat)
            {
                SelfMenBet(p, gold);
                return;
            }
            var tablePlayerMgr = App.GetGameManager<BrnnGameManager03>().TabelPlayerMgr;
            if (tablePlayerMgr.TableBet(seat, gold, p))
            {
                return;
            }
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                var random = UnityEngine.Random.Range(0, 100) % len;
                InstantiateChip(p, startPos[random], gold);
                Facade.Instance<MusicManager>().Play("Bet");
            }
        }
    }
}
