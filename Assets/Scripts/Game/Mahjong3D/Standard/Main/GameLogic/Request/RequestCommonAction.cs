using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class RequestCommonAction : AbsCommandAction
    {
        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SPlayerReady, OnPlayerReady);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SDismissRoom, OnDismissRoom);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SThrowoutCard, OnThrowoutCard);
        }

        public void OnPlayerReady(EvtHandlerArgs args)
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                GameCenter.Network.SendRequest(MahjongUtility.GameKey + "." + RequestCmd.Ready, sfs);
                return null;
            });
        }

        public void OnThrowoutCard(EvtHandlerArgs args)
        {
            int card = args.Conver<C2SThrowoutCardArgs>().Card;
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                //UserContorl.ClearSelectCard();
                //GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTip(null));
                //GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
                //DataCenter.ThrowoutCard = card;
                //Game.MahjongGroups.MahjongHandWall[0].ThrowOut(card);
                //var item = Game.MahjongGroups.MahjongThrow[0].GetInMahjong(card);
                //Game.TableManager.ShowOutcardFlag(item);
                //MahjongUtility.PlayMahjongSound(0, card);               

                sfs.PutInt(RequestKey.KeyType, NetworkProls.ThrowoutCard);
                sfs.PutInt(RequestKey.KeyOpCard, card);
                return sfs;
            });
        }

        public void OnDismissRoom(EvtHandlerArgs args)
        {
            int dismissType = args.Conver<C2SDismissRoomArgs>().DismissType;
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                var dataCenter = GameCenter.DataCenter;
                if (dataCenter.IsGamePlaying)
                {
                    sfs.PutUtfString("cmd", "dismiss");
                    sfs.PutInt(RequestKey.KeyType, dismissType);
                    GameCenter.Network.SendRequest("hup", sfs);
                }
                else
                {
                    if (dataCenter.OneselfData.IsOwner)
                    {
                        GameCenter.Network.SendRequest("dissolve", sfs);
                    }
                    else
                    {
                        MahjongUtility.ReturnToHall();
                    }
                }
                return null;
            });
        }
    }
}
