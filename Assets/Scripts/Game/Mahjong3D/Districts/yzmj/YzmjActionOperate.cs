using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class YzmjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                //单王调 双王调
                if (data.ContainsKey("wangdiao"))
                {
                    int type = data.GetInt("wangdiao");
                    if (type == 1)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateWangdiao, true));
                    }
                    else if (type == 2)
                    {
                        mOpMenuCache.Add(new KeyValuePair<int, bool>((int)OperateMenuType.OperateSanWangdiao, true));
                    }
                }
                Dispatch();
            }
        }
    }
}