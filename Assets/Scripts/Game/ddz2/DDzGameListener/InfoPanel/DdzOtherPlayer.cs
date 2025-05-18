using UnityEngine;
using System.Linq;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class DdzOtherPlayer : DdzPlayer
    {

        public GameObject SelectingMark;

        public GameObject WarningAnim;

        public UILabel CardCountLabel;


        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);             
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);
        }
      

        protected override void SetAutoState(bool state)
        {
            if (AutoMark == null) return;
            base.SetAutoState(state);
            if (!state) return;

            //播放动画
            var anim = AutoMark.GetComponent<UISpriteAnimation>();
            if (anim == null) return;
            anim.Play();
        }

        protected override void OnGetRejionGameInfo(DdzbaseEventArgs args)
        {
            base.OnGetRejionGameInfo(args);
            var player = GetPlayerInfo(args.IsfObjData);
            if (player == null) return;
            if (!player.ContainsKey(NewRequestKey.KeyCardNum)) return;
            CardCount = player.GetInt(NewRequestKey.KeyCardNum);
            SetCardCountLabel(CardCount);
        }
  


        ISFSObject GetPlayerInfo(ISFSObject sfsObj)
        {
            if(!sfsObj.ContainsKey(RequestKey.KeyUserList)) return null;

            ISFSArray array = sfsObj.GetSFSArray(RequestKey.KeyUserList);
            int thisSeat = Info.Seat;

            return (from ISFSObject item in array let seat = item.GetInt(RequestKey.KeySeat) where seat == thisSeat select item).FirstOrDefault();
        }


        /// <summary>
        /// 发牌阶段
        /// </summary>
        /// <param name="args"></param>
        protected override void OnAlloCateCds(DdzbaseEventArgs args)
        {
            base.OnAlloCateCds(args);
            var data = args.IsfObjData;
           
            if (Info == null) return;
            //此处不用判断是不是给自己发牌，因为服务器实际只给 游戏玩家自己发牌，其他玩家不发，但是每个玩家的牌数发的是一样的。所以可以赋值，擦
            var count = data.GetIntArray(GlobalConstKey.C_Cards).Length;
            CardCount = count;
            SetCardCountLabel(CardCount);
        }
            
       


        protected override void OnCheckSelectDouble(ISFSObject data)
        {
            base.OnCheckSelectDouble(data);
            if (data.ContainsKey(NewRequestKey.JiaBeiSeat))
            {
                var jiabeiSeats = data.GetIntArray(NewRequestKey.JiaBeiSeat);
                if (Info != null)
                {
                    var thisPlayerSeat = Info.Seat;
                    if (jiabeiSeats.Any(jiabeiSeat => jiabeiSeat == thisPlayerSeat))
                    {
                        SetSpeakSprite(string.Empty, false);
                    }
                }
            }
            SelectingMark.SetActive(false);
        }


        protected override void OnDoubleOver(DdzbaseEventArgs args)
        {
            base.OnDoubleOver(args);
            SelectingMark.SetActive(false);
        }


        /// <summary>
        /// 单局游戏结束
        /// </summary>
        /// <param name="args"></param>
        protected override void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            base.OnTypeOneRoundOver(args);
            LandMark.SetActive(false);
            WarningAnim.gameObject.SetActive(false);
            SetSpeakSprite(string.Empty, false);
            CardCount = 0;
            ReadyStateFlag.SetActive(false);
            SetCardCountLabel(-1);
        }


        /// <summary>
        /// 如果是自己叫pass则显示"不要",如果是上家,隐藏说话标识
        /// </summary>
        /// <param name="args"></param>
        private void OnTypePass(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var speakerSeat = data.GetInt(RequestKey.KeySeat);

            if (speakerSeat == Info.Seat)
            {
                SetSpeakSprite(SpkBuChu);
            }
            else if (speakerSeat == GetEarlyHandSeat())
            {
                SetSpeakSprite(string.Empty, false);
            }
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
            var thisSeat = Info.Seat;
            bool isMe = thisSeat == speakerSeat;

            //刷新手牌个数
            if (!isMe)
            {
                if (speakerSeat == GetEarlyHandSeat())
                {
                    SetSpeakSprite(string.Empty, false);
                }
                return;
            }
            var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
            CardCount = CardCount - cdsLen;
            SetCardCountLabel(CardCount);

            //少于3张报牌数,发出警告
            if (CardCount > 2)
                return;
            ShowLampAnim(CardCount > 0);
            App.GetGameData<DdzGameData>().OnHdcdsChange(speakerSeat);
        }


        /// <summary>
        /// 设置警告动画Active
        /// </summary>
        /// <param name="active"></param>
        private void ShowLampAnim(bool active)
        {
            if (WarningAnim == null) return;
            WarningAnim.SetActive(active);
        }

        /// <summary>
        /// 设置手牌信息
        /// </summary>
        /// <param name="cardCount"></param>
        void SetCardCountLabel(int cardCount)
        {
            if (CardCountLabel == null) return;
            CardCountLabel.text = cardCount.ToString();
            CardCountLabel.gameObject.SetActive(cardCount > 0);
        }
    }
}