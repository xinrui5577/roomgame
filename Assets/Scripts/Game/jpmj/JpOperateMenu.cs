using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Game.jpmj
{
    public class JpOperateMenu : OpreateMenu
    {
        [SerializeField]
        protected GameObject GuoBtn;

        [SerializeField] protected JpMahjongPlayerHard JpMjPlayerHard;

        public override void OnGuoClick()
        {
            if (JpMjPlayerHard.IsHdCds17 && JpMjPlayerHard.IsAllhdCdCantOut())
            {
                YxMessageBox.Show("手牌中没有可以打出的牌，请选择其他操作");
                return;
            }

            base.OnGuoClick();
        }
    }
}
