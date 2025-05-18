using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DlmjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                int op = DataCenter.OperateMenu;
                if (GameUtils.BinaryCheck((int)OperateMenuType.OpreateTing, op))
                {
                    if (DataCenter.ConfigData.NiuBi)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateNiu, true));
                    }
                }
                Dispatch();
            }
        }
    }
}
