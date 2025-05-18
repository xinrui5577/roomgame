using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);
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
                            //设置手牌状态
                            Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.SingleHu);
                            //设置胡牌
                            var hucard = userInfo.SingleHuCard;
                            var item = Game.MahjongCtrl.PopMahjong(hucard);
                            Game.MahjongGroups.MahjongOther[i].GetInMahjong(item);
                            item.gameObject.SetActive(true);
                        }
                        break;
                }
            }        
        } 

        protected virtual void SetPlayerState()
        {
            MahjongUserInfo userInfo;
            var mahHand = Game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetComponent<XzmjMahjongPlayerHand>();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {            
                userInfo = DataCenter.Players[i];
                var state = userInfo.XuezhanStatue;           
                switch ((XzmjGameStatue)state)
                {
                    case XzmjGameStatue.huanZhanging:
                        {
                            if (i == 0)
                            {
                                GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
                                {
                                    CtrlState = true,
                                    StateFlagType = (int)PlayerStateFlagType.SelectCard
                                });
                                Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.ExchangeCards, 3);
                            }
                        }
                        break;
                    case XzmjGameStatue.huanZhangOver:
                        {
                            if (i == 0)
                            {
                                var value = 3;
                                DataCenter.LeaveMahjongCnt -= value;
                                //换张之后的重连需要显示扣下的牌                                
                                Game.MahjongGroups.PopMahFromCurrWall(value);
                                Game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(new int[3] { 17, 18, 19 });
                            }
                        }
                        break;
                    case XzmjGameStatue.duanMening:
                        {
                            Game.MahjongGroups.SwitchGorup[i].OnReset();
                            if (i == 0)
                            {
                                mahHand.SetHandCardState(HandcardStateTyps.Dingqueing);
                                GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
                                {
                                    CtrlState = true,
                                    StateFlagType = (int)PlayerStateFlagType.Selecting
                                });
                            }
                        }
                        break;
                    case XzmjGameStatue.duanMenOver:
                    case XzmjGameStatue.end:
                        {
                            if (i == 0)
                            {
                                int color = userInfo.HuanCardType;
                                // 本家缺门花色置灰
                                xzMahHand.ChangeHandMahGray(color);
                                //到自己出牌时，提示打牌                             
                                if (DataCenter.ReconectCardState == 0 && DataCenter.SelfCurrOp)
                                {
                                    Game.MahjongGroups.MahjongHandWall[0].SetLastCardPos(DataCenter.GetInMahjong);
                                }
                            }
                        }
                        break;
                }
                if (i == 0 && state < (int)XzmjGameStatue.end)
                {
                    //换张和定缺未结束，不允许出牌
                    //Game.MahjongGroups.PlayerToken = false;
                    GameCenter.Controller.ForbbidToken = true;
                }
            }
        }
    }
}
