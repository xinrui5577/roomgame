using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NamjActionOperate : ActionOperate
    {
        public override void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                if (data.ContainsKey("qiduitings"))
                {
                    var list = DataCenter.OneselfData.TingList;
                    DataCenter.OneselfData.SetTinglist(data.GetIntArray("qiduitings"));
                    GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(list);
                }
                Dispatch();
            }
        }
    }
}
