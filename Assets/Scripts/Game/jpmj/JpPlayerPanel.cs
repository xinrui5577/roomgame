using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    public class JpPlayerPanel : PlayersPnl
    {
        [SerializeField]
        protected DnxbCtl DnxbCtlGob;

        public override void SetBanker(int chair)
        {
            base.SetBanker(chair);
            JpDnxbCtrl.SetDnxb(DnxbCtlGob, UtilData.BankerSeat, UtilData.PlayerSeat);
        }
    }
}
