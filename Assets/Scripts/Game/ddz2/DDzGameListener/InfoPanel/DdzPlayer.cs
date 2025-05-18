using UnityEngine;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class DdzPlayer : YxBaseGamePlayer
    {

        protected const string SpkBuJiao = "bujiao";
        protected const string Fen = "fen";      //fen
        public const string SpkJiabei = "jiabei";
        public const string SpkBuJiabei = "bujiabei";

        public const string SpkBuChu = "buchu";
        public const string SpkJiaoDz = "jiaodizhu";
        public const string SpkQiangDz = "qiangdizhu";
        public const string SpkBuQiangDz = "buqiang";


        /// <summary>
        /// 手牌剩余张数
        /// </summary>
        internal int CardCount;

        public UITexture HeadImage;


        /// <summary>
        /// 说话标识,显示说话状态
        /// </summary>
        public UISprite ShowSpeakSp;

        /// <summary>
        /// 地主标识
        /// </summary>
        public GameObject LandMark;

        public GameObject DoubleMark;

        public Transform ClockParent;

        public GameObject AutoMark;
      
        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGetRejionGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnUserReady, OnUserReady);

            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrab, OnTypeGrab);                   //叫地主阶段
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);     //进入叫地主阶段
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnAlloCateCds);            //发牌阶段
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);           //一局游戏结束
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);           //加倍结束阶段
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAuto, OnTypeAuto);                   //托管
        }


        /// <summary>
        /// 托管事件
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeAuto(DdzbaseEventArgs args)
        {
            ISFSObject data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (seat != Info.Seat) return;
            
            SetAutoState(data.GetBool(NewRequestKey.KeyTf));
        }

        protected virtual void SetAutoState(bool state)
        {
            if (AutoMark == null) return;
            ((DdzUserInfo)Info).AutoState = state;
            AutoMark.SetActive(state);
        }          


        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();

            if (DoubleMark != null)
            {
                var ddzUserInfo = (DdzUserInfo)Info;
                DoubleMark.SetActive(ddzUserInfo.IsRate);
            }
            ShowSpeakSp.gameObject.SetActive(false);
        }

        protected virtual void OnGetRejionGameInfo(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(NewRequestKey.KeyLandLord))
            {
                int landLord = data.GetInt(NewRequestKey.KeyLandLord);
                LandMark.SetActive(Info.Seat == landLord);
            }
            else
            {
                LandMark.SetActive(false);
            }

            var ddzUserInfo = ((DdzUserInfo) Info);
            if (ddzUserInfo.Network)
            {
                UserOnLine();
            }
            else
            {
                UserIdle();
            }

            SetAutoState(ddzUserInfo.AutoState);
        }


        protected virtual void OnUserReady(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (Info == null || seat != Info.Seat) return;
            ReadyStateFlag.SetActive(true);
            SetSpeakSprite(string.Empty, false);

            DoubleMark.SetActive(false);
            LandMark.SetActive(false);

            var ddzInfo = (DdzUserInfo) Info;
            ddzInfo.AutoState = false;
            ddzInfo.IsRate = false;
        }

        /// <summary>
        /// 先出牌
        /// </summary>
        protected virtual void TypeFirstOut(DdzbaseEventArgs args)
        {
            OnGetDipai(args);

            OnCheckSelectDouble(args.IsfObjData);
        }

        protected virtual void OnCheckSelectDouble(ISFSObject data)
        {
            
        }

        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGetDipai(DdzbaseEventArgs args)
        {
            SetSpeakSprite(string.Empty, false);

            var data = args.IsfObjData;
           
            //判断是给这位玩家发底牌么
            if (data.GetInt(RequestKey.KeySeat) != Info.Seat) return;
            CardCount += data.GetIntArray(RequestKey.KeyCards).Length;
            LandMark.SetActive(true);
        }


        /// <summary>
        /// 玩家失去连接
        /// </summary>
        public void UserIdle()
        {
            HeadImage.color = new Color(.5f, .5f, .5f);
        }

        /// <summary>
        /// 玩家重新连接
        /// </summary>
        public void UserOnLine()
        {
            HeadImage.color = Color.white;
        }

        public void UserJoinRoom()
        {
            HeadImage.color = Color.white;
        }

        protected virtual void OnDoubleOver(DdzbaseEventArgs args)
        {
            if (Info == null) return;
            var data = args.IsfObjData;

            var rates = data.GetIntArray(NewRequestKey.KeyJiaBei);
            var userSeat = Info.Seat%rates.Length;
            bool isDouble = rates[userSeat] > 1;
            string spriteName = isDouble ? SpkJiabei : SpkBuJiabei;
            SetSpeakSprite(spriteName);
            if (DoubleMark != null)
            {
    
                DoubleMark.SetActive(isDouble);
            }

            CancelInvoke("HideJiabeiSp");
            Invoke("HideJiabeiSp", 3f);
        }

        protected void SetSpeakSprite(string spriteName,bool active = true)
        {
            ShowSpeakSp.gameObject.SetActive(active);
            ShowSpeakSp.spriteName = spriteName;
            ShowSpeakSp.MakePixelPerfect();
        }

        /// <summary>
        /// 隐藏加倍说话sp
        /// </summary>
        /// <returns></returns>
        private void HideJiabeiSp()
        {
            string sprName = ShowSpeakSp.spriteName;
            SetSpeakSprite(string.Empty, !(sprName.Equals(SpkJiabei) || sprName.Equals(SpkBuJiabei)));
        }


        protected virtual void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            LandMark.SetActive(false);
            SetAutoState(false);
            SetSpeakSprite(string.Empty, false);
            ReadyState = false;
        }


        /// <summary>
        /// 发牌阶段
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnAlloCateCds(DdzbaseEventArgs args)
        {
            ReadyStateFlag.SetActive(false);
        }


        /// <summary>
        /// 出牌阶段
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTypeOutCard(DdzbaseEventArgs args)
        {
            
        }


        /// <summary>
        /// 进入叫分阶段
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTypeGrabSpeaker(DdzbaseEventArgs args)
        {
            ReadyStateFlag.SetActive(false);    //隐藏准备标记
        }


        /// <summary>
        /// 叫地主
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeGrab(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
           

            if (Info == null)
            {
                return;
            }
            var gdata = App.GetGameData<DdzGameData>();
            int playerSeat = Info.Seat;
            if (seat == gdata.GetEarlyHand(playerSeat))
            {
                SetSpeakSprite(string.Empty, false);
            }

            if (seat != playerSeat)
            {
                return;
            }

            //检验相关数据是否正确
            if (!DDzUtil.IsServDataContainAllKey(
                new[]
                {
                    RequestKey.KeySeat, RequestKey.KeyScore
                }, data))
            {
                return;
            }

            if (ShowSpeakSp == null) return;
            var score = data.GetInt(RequestKey.KeyScore);           

            string spriteName ;
            bool robModel = gdata.RobModel;
            //设置图片
            if (robModel)
            {
                if (score > 0)
                {
                    //有人叫地主
                    spriteName = score == 1 ? SpkJiaoDz : SpkQiangDz;
                }
                else
                {
                    //不抢
                    spriteName = SpkBuQiangDz;
                }
            }
            else
            {
                spriteName = score == 0 ? SpkBuJiao : score + Fen;
            }
            
            SetSpeakSprite(spriteName);
        }

        /// <summary>
        /// 获得上家的座位号
        /// </summary>
        /// <returns></returns>
        protected int GetEarlyHandSeat()
        {
            int playerCount = App.GameData.PlayerList.Length;
            int prePlayerSeat = (Info.Seat + playerCount - 1) % playerCount;        //上家的座位号
            return prePlayerSeat;
        }

        /// <summary>
        /// 获取下家的座位号
        /// </summary>
        /// <returns></returns>
        public int GetLaterHandSeat()
        {
            int playerCount = App.GameData.PlayerList.Length;
            int nextPlayerSeat = (Info.Seat + 1) % playerCount;        //下家的座位号
            return nextPlayerSeat;
        }

        
    }
}