using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.brnn.brnn_skin03
{
    public class TablePlayer : YxBaseGamePlayer
    {

        public TweenPosition Tweenpos;

        public Vector3 BetStartPos;

        /// <summary>
        /// 玩家下注
        /// </summary>
        public void TablePlayerBet(int p , int gold)
        {
            var betCtrl = App.GetGameManager<BrnnGameManager>().BetCtrl;
            betCtrl.InstantiateChip(p, BetStartPos, gold);
            if (Info != null)
            {
                Coin -= gold;
            }

            OnPlayerBet();
        }
         
        public void OnPlayerBet()
        {
            if (Tweenpos.enabled)
                return;
            Tweenpos.ResetToBeginning();
            Tweenpos.PlayForward();
        }
      
    }
}
