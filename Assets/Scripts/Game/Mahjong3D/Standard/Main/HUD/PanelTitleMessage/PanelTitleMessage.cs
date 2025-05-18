namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum TitleMessageType
    {
        ChangeCardTip,//换牌提示
        DingqueTip,//定缺提示
        BbmjTip,//白板麻将出牌提示    
        HuanAndDq,//定缺和换张提示
        FgChangeCardTip,//确定风杠提示
    }

    [UIPanelData(typeof(PanelTitleMessage), UIPanelhierarchy.EffectAndTip)]
    public class PanelTitleMessage : UIPanelBase, IUIPanelControl<ShowTitleMessageArgs>
    {
        public TitleStyleBase[] TitleStlyles;

        private void Awake()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.HideTitleMessage, HideTitleMessage);
        }

        private void HideTitleMessage(EvtHandlerArgs args)
        {
            for (int i = 0; i < TitleStlyles.Length; i++)
            {
                TitleStlyles[i].gameObject.SetActive(false);
            }
        }

        public override void OnStartGameUpdate()
        {
            for (int i = 0; i < TitleStlyles.Length; i++)
            {
                TitleStlyles[i].OnStartGameUpdate();
            }
        }

        public override void OnReconnectedUpdate()
        {
            for (int i = 0; i < TitleStlyles.Length; i++)
            {
                TitleStlyles[i].OnReconnectUpdate();
            }
        }

        public override void OnEndGameUpdate()
        {
            for (int i = 0; i < TitleStlyles.Length; i++)
            {
                TitleStlyles[i].gameObject.SetActive(false);
            }
            Close();
        }

        public void Open(ShowTitleMessageArgs args)
        {
            base.Open();
            if (null == args) return;
            TitleStyleBase style = null;
            for (int i = 0; i < TitleStlyles.Length; i++)
            {
                if ((int)TitleStlyles[i].Type == args.TitleType)
                {
                    style = TitleStlyles[i];
                }
            }
            if (null != style)
            {
                style.gameObject.SetActive(true);
                style.Params = args.Params;
                style.Show();
            }
        }
    }
}