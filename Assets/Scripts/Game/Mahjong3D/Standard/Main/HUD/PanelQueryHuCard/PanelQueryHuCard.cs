namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelQueryHuCard), UIPanelhierarchy.Popup)]
    public class PanelQueryHuCard : UIPanelBase, IUIPanelControl<QueryHuArgs>
    {
        public StyleQueryHu StyleQueryHu;

        public override void OnInit()
        {
            StyleQueryHu.OnInit();
        }

        public override void OnEndGameUpdate()
        {
            Close();
        }

        public void Open(QueryHuArgs args)
        {
            base.Open();
            StyleQueryHu.Open(args);
        }
    }
}