using UnityEngine;
using System.Collections;
using YxFramwork.Framework;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brtbsone
{
    public class BrttzTablePlayer : YxBaseGamePlayer
    {
        public TweenPosition TweenPos;
        public Vector3 BetStartPos;

        /// <summary>
        /// 玩家下注
        /// </summary>
        public void TablePlayerBet(int p, int gold)
        {
            var betCtrl = App.GetGameManager<BrttzGameManager>().BetCtrl;
            betCtrl.InstantiateChip(p, BetStartPos, gold);
            if (Info != null)
            {
                Coin -= gold;
            }
            OnPlayerBet();
        }

        public void TableSelfBet(int gold)
        {
            if (Info != null)
            {
                Coin -= gold;
            }
            OnPlayerBet();
        }

        public void OnPlayerBet()
        {
            if (TweenPos == null) return;
            if (TweenPos.enabled)
                return;
            TweenPos.ResetToBeginning();
            TweenPos.PlayForward();
        }
    }
}