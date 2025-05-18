using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QjmjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                int op = DataCenter.OperateMenu;
                if (GameUtils.BinaryCheck((int)OperateMenuType.OpreateLaiZiGang, op))
                {
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateLaiZiGang, true));
                }
                Dispatch();
            }
        }
    }
}