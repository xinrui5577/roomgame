using System.Collections.Generic;
using Assets.Scripts.Game.Texas.Mgr;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Texas.skin01
{
    public class BetMgrSkin1 : BetMgr
    {
        public override void CollectBet()
        {
            base.CollectBet();
            SmallBet.GetComponent<RefreshSmallBetBg>().ShowSmallBetBg();
        }

        public override void CollectBetValue()
        {
            base.CollectBetValue();
            SmallBet.GetComponent<RefreshSmallBetBg>().ShowSmallBetBg();
        }

        public override void Reset()
        {
            base.Reset();
            SmallBet.GetComponent<RefreshSmallBetBg>().HideSmallBetBg();
        }

		public override void SendBetToWin(List<ISFSObject> wins)
		{
			base.SendBetToWin (wins);
			SmallBet.GetComponent<RefreshSmallBetBg>().HideSmallBetBg();
        }
	}
}