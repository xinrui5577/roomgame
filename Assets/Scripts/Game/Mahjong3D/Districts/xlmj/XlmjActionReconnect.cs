using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XlmjActionReconnect : XzmjActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            var currChair = DataCenter.CurrOpChair;
            //设置发牌点
            Game.MahjongGroups.SetSendCardSPos(DataCenter.BankerSeat, DataCenter.OneselfData.Seat);
            //设置抓牌点
            Game.MahjongGroups.SetCatchCardStartPos(DataCenter.SaiziPoint);
            var removeCardNum = DataCenter.Room.MahjongCount - DataCenter.LeaveMahjongCnt;
            Game.MahjongGroups.PopMahFromCurrWall(removeCardNum);
            //设置麻将牌
            MahjongUserInfo player;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                player = DataCenter.Players[i];
                //设置出牌
                Game.MahjongGroups.MahjongThrow[i].GetInMahjong(player.OutCards.ToArray());
                //设置吃碰杠牌
                Game.MahjongGroups.MahjongCpgs[i].SetCpgArray(player.CpgDatas.ToArray());
                //设置手牌 
                Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(player.HardCards);
                //听牌
                if (DataCenter.Players[i].IsAuto)
                {
                    Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.Ting);
                }
                //设置补张
                if (!player.BuzhangCards.ExIsNullOjbect() && player.BuzhangCards.Length > 0)
                {
                    var num = player.BuzhangCards.Length;
                    DataCenter.LeaveMahjongCnt -= num;
                    Game.MahjongGroups.PopMahFromCurrWall(num);
                    Game.MahjongGroups.MahjongOther[i].GetInMahjong(player.BuzhangCards);
                }
            }
            //设置自己手中的赖子牌
            Game.MahjongGroups.PlayerHand.SetLaizi(DataCenter.Game.LaiziCard);
            //更新牌桌
            UpdateMahjongTable();

            SetPlayerState();
            MahjongUserInfo userInfo;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                var state = userInfo.XuezhanStatue;
                switch ((XzmjGameStatue)state)
                {
                    case XzmjGameStatue.hu:
                        {
                            var hucardlist = userInfo.HuCardsList;
                            userInfo.HuCardsList.AddRange(hucardlist);
                            if (hucardlist.ExIsNullOjbect()) continue;
                            for (int j = 0; j < hucardlist.Count; j++)
                            {
                                var item = Game.MahjongCtrl.PopMahjong(hucardlist[j]).GetComponent<MahjongContainer>();
                                Game.MahjongGroups.MahjongOther[i].GetInMahjong(item);
                                item.gameObject.SetActive(true);
                            }

                            DataCenter.Players[i].IsAuto = true;
                            Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.SingleHu);
                            DataCenter.LeaveMahjongCnt -= hucardlist.Count;
                            Game.MahjongGroups.PopMahFromCurrWall(hucardlist.Count);
                        }
                        break;
                }
            }

            SetReconectCardState();
            MahjongQueryHuTip();
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}
