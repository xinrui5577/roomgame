using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.BtnCtrlPanel
{
    public class JiabeiBtnListener : ServEvtListener
    {
        /// <summary>
        /// 加倍按钮
        /// </summary>
        [SerializeField]
        protected GameObject JiabeiCtrlBtns;

        protected override void OnAwake()
        {
            //Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            //Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
        }
        

        public override void RefreshUiInfo()
        {
          
        }

        protected  void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是选择加倍阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyState) && data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusDouble)
            {
                var selfRate = data.GetSFSObject(RequestKey.KeyUser).GetInt(NewRequestKey.KeyRate);
                //if(selfRate==0 && App.GetGameData<GlobalData>().GetSelfSeat!=data.GetInt(NewRequestKey.KeyLandLord)) JiabeiCtrlBtns.SetActive(true);
                if (selfRate == 0) JiabeiCtrlBtns.SetActive(true);
            }

        }


        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果加倍
            if (data.GetBool(NewRequestKey.KeyJiaBei))
            {
                var landSeat = data.GetInt(RequestKey.KeySeat);
                var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
                if (!(App.GetGameData<GlobalData>().JiaBeiType == 2 && selfSeat == landSeat))
                {
                    if (JiabeiCtrlBtns != null) JiabeiCtrlBtns.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 点击加倍按钮
        /// </summary>
        public void OnClickDoubtn()
        {
            GlobalData.ServInstance.SendDouble(2);
            JiabeiCtrlBtns.SetActive(false);
        }

        /// <summary>
        /// 点击不加倍按钮
        /// </summary>
        public void OnClickNoDoubtn()
        {
            GlobalData.ServInstance.SendDouble(1);
            JiabeiCtrlBtns.SetActive(false);
        }

    }
}
