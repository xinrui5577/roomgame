using System.Linq;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class JiabeiBtnListener : ServEvtListener
    {
        /// <summary>
        /// 加倍按钮
        /// </summary>
        [SerializeField]
        protected GameObject JiabeiCtrlBtns;

        /// <summary>
        /// 托管状态
        /// </summary>
        private bool _autoState;

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDouble, OnDouble);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
            Facade.EventCenter.AddEventListeners<string, bool>(GlobalConstKey.KeySelfAuto, OnSelfAuto);

        }

        private void OnSelfAuto(bool state)
        {
            _autoState = state;
        }


        public override void RefreshUiInfo()
        {
          
        }

        protected  void OnRejoinGame(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是选择加倍阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyGameStatus) &&
                data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusDouble)
            {
                var selfRate = data.GetSFSObject(RequestKey.KeyUser).GetInt(NewRequestKey.KeyRate);
                //if(selfRate==0 && App.GetGameData<DdzGameData>().SelfSeat!=data.GetInt(NewRequestKey.KeyLandLord)) JiabeiCtrlBtns.SetActive(true);
                if (selfRate == 0) JiabeiCtrlBtns.SetActive(true);
            }
            else
            {
                JiabeiCtrlBtns.SetActive(false);
            }
        }


        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut( DdzbaseEventArgs args)
        {
            if (_autoState)     //托管状态不加倍
            {
                App.GetRServer<DdzGameServer>().SendDouble(1);
                return;
            }


            var data = args.IsfObjData;
            //如果加倍
            if (!data.ContainsKey(NewRequestKey.KeyJiaBei) || !data.ContainsKey(NewRequestKey.JiaBeiSeat))
            {
                YxDebug.LogError("不能加倍");
                return;
            }

            var jiabeiSeats = data.GetIntArray(NewRequestKey.JiaBeiSeat);
            //判断是否是自己要显示加倍
            var selfSeat = App.GetGameData<DdzGameData>().SelfSeat;
            bool isSelfSeatJiabei = jiabeiSeats.Any(jiabeiSeat => jiabeiSeat == selfSeat);
            
            if (isSelfSeatJiabei && data.GetBool(NewRequestKey.KeyJiaBei) && JiabeiCtrlBtns != null)
                JiabeiCtrlBtns.SetActive(true);


                /*
                                var landSeat = data.GetInt(RequestKey.KeySeat);
                                var selfSeat = App.GetGameData<DdzGameData>().SelfSeat;
                                if (!(App.GetGameData<DdzGameData>().JiaBeiType == 2 && selfSeat == landSeat))
                                {
                                    if (JiabeiCtrlBtns != null) JiabeiCtrlBtns.SetActive(true);
                                }*/
        }


        /// <summary>
        /// 当收到加倍信息
        /// </summary>
        private void OnDouble(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<DdzGameData>().SelfSeat)
            {
                JiabeiCtrlBtns.SetActive(false);
            }
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected void OnDoubleOver(DdzbaseEventArgs args)
        {
            JiabeiCtrlBtns.SetActive(false);
        }


/*        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            JiabeiCtrlBtns.SetActive(false);
        }*/

        /// <summary>
        /// 点击加倍按钮
        /// </summary>
        public void OnClickDoubtn()
        {
            App.GetRServer<DdzGameServer>().SendDouble(2);
        }

        /// <summary>
        /// 点击不加倍按钮
        /// </summary>
        public void OnClickNoDoubtn()
        {
            App.GetRServer<DdzGameServer>().SendDouble(1);
        }

    }
}
