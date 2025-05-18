using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    //泉州麻将胡类型
    public enum QzmjOperateType
    {
        No,
        youjin,
        shuangyou,
        sanyou,
        sanjindao
    }

    public class QzmjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                // 游金胡
                if (data.ContainsKey("youjinhu"))
                {
                    var menuType = 0;
                    var opType = (QzmjOperateType)data.GetInt("youjinhu");
                    switch (opType)
                    {
                        case QzmjOperateType.youjin: menuType = (int)OperateMenuType.OperateYoujin; break;
                        case QzmjOperateType.shuangyou: menuType = (int)OperateMenuType.OperateShuangyou; break;
                        case QzmjOperateType.sanyou: menuType = (int)OperateMenuType.OperateShuangyou; break;
                    }
                    mOpMenuCache.Add(new KeyValuePair<int, bool>(menuType, true));
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateHu, false));
                }

                // 三金倒           
                if (data.ContainsKey("sanjinhu"))
                {
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateSanjindao, true));
                    mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OpreateHu, false));
                }
                Dispatch();
            }
        }
    }
}
