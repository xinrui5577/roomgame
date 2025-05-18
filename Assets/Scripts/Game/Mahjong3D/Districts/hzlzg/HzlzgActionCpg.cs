using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HzlzgActionCpg : ActionCpg
    {
        public override void ResponseLaiZiGangAction(ISFSObject data)
        {
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            var chair = DataCenter.CurrOpChair;
            var card = data.TryGetInt(RequestKey.KeyCard);

            Game.MahjongGroups.MahjongHandWall[chair].RemoveMahjong(card);
            var item = Game.MahjongGroups.MahjongOther[chair].GetInMahjong(card);
            item.Laizi = DataCenter.IsLaizi(card);
            item.gameObject.SetActive(true);
           
            MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, PoolObjectType.gang);
            if (chair != 0)
            {
                GameCenter.Shortcuts.MahjongQuery.AddRecordMahjong(card);
            }
            else
            {
                DataCenter.Players[0].HardCards.Remove(card);
            }
        }
    }
}
