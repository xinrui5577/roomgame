namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelChooseXjfd), UIPanelhierarchy.Popup)]

    public class PanelChooseXjfd : UIPanelBase, IUIPanelControl<EvtHandlerArgs>
    {
        public void Open(EvtHandlerArgs args)
        {
            base.Open();
        }

        public void OnSendBtnClick()
        {
            Close();
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetComponent<CcmjMahjongPlayerHand>();
            ccMahHand.SendChoosXjfd();
        }

        public void OnCancelBtnClick()
        {
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetComponent<CcmjMahjongPlayerHand>();
            ccMahHand.ResetPlayerHandMahjong(true);
            GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open();
            Close();
        }
    }
}