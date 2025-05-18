namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class AiAgency : ShortcutsPart
    {
        private int[] FilterOperate =
        {
           OperateKey.OpreateHu,
           OperateKey.OpreateTing,
           OperateKey.OpreateGang,
           OperateKey.OperateLigang,
        };

        private SwitchCombination mSwitchCombination;

        public override void OnInitalization()
        {
            mSwitchCombination = GameCenter.Shortcuts.SwitchCombination;
            GameCenter.EventHandle.Subscriber((int)EventKeys.AiAgency, AiAgencyCtrl);
        }

        public void AiAgencyCtrl(EvtHandlerArgs args)
        {
            var data = args as AiAgencyArgs;           
            if (data.State)
            {
                mSwitchCombination.Open((int)GameSwitchType.AiAgency);
                //如果到自己打牌就出牌
                if (GameCenter.DataCenter.CurrOpChair == 0)
                {
                    GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                    {
                        param.Card = GameCenter.DataCenter.GetInMahjong;
                    });
                }
            }
            else
            {
                mSwitchCombination.Close((int)GameSwitchType.AiAgency);
            }
        }

        /// <summary>
        /// 托管时拦截普通消息
        /// </summary> 
        public bool Holdup(int op)
        {
            bool flag = true;
            if (op == 0) return flag;
            for (int i = 0; i < FilterOperate.Length; i++)
            {
                if (GameUtils.BinaryCheck(FilterOperate[i], op))
                {
                    flag = false;
                }
            }
            return flag;
        }
    }
}