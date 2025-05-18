using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class DdzSelfPlayer : DdzPlayer
    {

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
        }
     


        private void OnTypePass(DdzbaseEventArgs args)
        {
            int speakerSeat = args.IsfObjData.GetInt(RequestKey.KeySeat);

            if (speakerSeat == GetLaterHandSeat()) return;

            int selfSeat = Info.Seat;
            SetSpeakSprite(SpkBuChu, selfSeat == speakerSeat);
        }


        /// <summary>
        /// 出牌阶段
        /// </summary>
        /// <param name="args"></param>
        protected override void OnTypeOutCard(DdzbaseEventArgs args)
        {
            if (Info == null) return;

            var data = args.IsfObjData;
            var speakerSeat = data.GetInt(RequestKey.KeySeat);
            bool isMe = Info.Seat == speakerSeat;


            //刷新手牌个数
            if (!isMe)
            {
                if(speakerSeat == GetEarlyHandSeat())
                {
                    SetSpeakSprite(string.Empty, false);
                }
                return;
            }
            var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
            CardCount = CardCount - cdsLen;

            //少于3张报牌数
            if (CardCount > 2)
                return;
            App.GetGameData<DdzGameData>().OnHdcdsChange(speakerSeat);
        }

        protected override void SetAutoState(bool state)
        {
            base.SetAutoState(state);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeySelfAuto, state);       //广播托管事件
        }



        protected override void OnAlloCateCds(DdzbaseEventArgs args)
        {
            base.OnAlloCateCds(args);
            var data = args.IsfObjData;

            CardCount = data.GetIntArray(GlobalConstKey.C_Cards).Length;
        }

     

        public void OnClickCancelAutoPlayBtn()
        {
            App.GetRServer<DdzGameServer>().SendAutoPlayState(false);
        }

        protected override void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            base.OnTypeOneRoundOver(args);
            OnClickCancelAutoPlayBtn();
        }

        protected override void OnGetRejionGameInfo(DdzbaseEventArgs args)
        {
            base.OnGetRejionGameInfo(args);
            UserOnLine();
        }


    }
}
