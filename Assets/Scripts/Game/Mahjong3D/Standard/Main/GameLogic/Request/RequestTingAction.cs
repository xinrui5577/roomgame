using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class RequestTingAction : AbsCommandAction
    {
        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2STing, OnTing);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SNiuting, OnNiuting);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SLiangdaoTing, C2SLiangdaoTing);
        }

        public void OnTing(EvtHandlerArgs args)
        {
            var param = args.Conver<C2STingArgs>();
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, param.Prol);
                sfs.PutInt(RequestKey.KeyOpCard, param.Card);
                return sfs;
            });
        }

        public void OnNiuting(EvtHandlerArgs args)
        {
            var param = args.Conver<C2STingArgs>();
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, param.Prol);
                sfs.PutInt(RequestKey.KeyOpCard, param.Card);
                sfs.PutIntArray("niu", param.LiangCards);
                return sfs;
            });
        }

        public void C2SLiangdaoTing(EvtHandlerArgs args)
        {
            var param = args.Conver<C2STingArgs>();
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.LiangDao);
                sfs.PutIntArray(RequestKey.KeyCardsArr, param.LiangCards);
                return sfs;
            });
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.ThrowoutCard);
                sfs.PutInt(RequestKey.KeyOpCard, param.Card);
                return sfs;
            });
        }
    }
}
