using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DsqmjActionResponseCommon : YkmjActionResponseCommon
    {
        public override void FenzhangAction(ISFSObject data)
        {
            if (data.ContainsKey("cards"))
            {
                var value = 0;
                var chair = 0;
                DataCenter.Game.FenzhangFlag = true;
                var cardsBySeat = data.GetIntArray("cards");
                for (int i = 0; i < cardsBySeat.Length; i++)
                {
                    chair = MahjongUtility.GetChair(i);
                    value = cardsBySeat[i];
                    if (value > 0)
                    {
                        Game.MahjongGroups.CurrGetMahjongWall.PopMahjong();
                        var temp = Game.MahjongCtrl.PopMahjong(value);
                        if (temp != null)
                        {
                            var mahjong = temp.GetComponent<MahjongContainer>();
                            mahjong.Laizi = DataCenter.IsLaizi(value);
                            Game.MahjongGroups.MahjongOther[chair].GetInMahjong(mahjong);
                        }
                    }
                    DataCenter.Players[chair].FenzhangCard = value;
                }
                //隐藏箭头
                Game.TableManager.HideOutcardFlag();
            }
        }
    }
}
