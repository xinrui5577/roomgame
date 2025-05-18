
using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                if (data.ContainsKey("youjin"))
                {
                    //隐藏过按钮
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateHideGuo, false));
                    //显示游金按钮
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateYoujin, true));
                    DataCenter.Players[0].SetTinglist(data.TryGetIntArray("youjin"));
                    //禁止出牌
                    Game.MahjongGroups.PlayerToken = false;
                }
                Dispatch();
            }
        }
    }
}
