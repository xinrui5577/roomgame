using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 玩家自己的信息ui处理
    /// </summary>
    public class PlayerSelfListener : PlayerInfoListener
    {
        private int UpSeatInt;
        private int OwnSeatInt;
        protected override void OnAwake()
        {
            gameObject.SetActive(false);
            base.OnAwake();
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
        }
        private void UpSeatV()
        {
            int PlayerMaxNumInt = App.GetGameData<GlobalData>().PlayerMaxNum;
            if (PlayerMaxNumInt == 3)
            {
                UpSeatInt = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            }
            else if (PlayerMaxNumInt == 2)
            {
                UpSeatInt = App.GetGameData<GlobalData>().GetRightPlayerSeat;
            }
        }
        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var curSeat = data.GetInt(RequestKey.KeySeat);
            if (curSeat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                SetSpeakSpState(false);
                var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
                var curselfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) - cdsLen;
                UserDataTemp.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);
                App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);

                App.GetGameData<GlobalData>().OnSomePlayerHdcdsChange(curSeat, curselfCdsNum);
            }
            else if (curSeat == UpSeatInt)
                SetSpeakSpState(false);
        }
        /// <summary>
        /// 如果是自己叫pass则显示“不要”，如果是商家出牌则消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var curSeat = args.IsfObjData.GetInt(RequestKey.KeySeat);
            if (curSeat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                SetSpeakSpState(true, SpkBuChu);
            }
            else if (curSeat == UpSeatInt)
                SetSpeakSpState(false);
        }
        protected override void SetUserInfo(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            UserDataTemp = GetHostUserData(data);

            RefreshUiInfo();
        }
        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            base.OnGetDipai(sender, args);
            var selfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, selfCdsNum);
        }
        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected override void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            base.OnAlloCateCds(sender, args);
            var selfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, selfCdsNum);
        }
        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            base.RefreshUiInfo();
            UpSeatV();
            OwnSeatInt = App.GetGameData<GlobalData>().GetSelfSeat;
            gameObject.SetActive(true);
        }
    }
}
