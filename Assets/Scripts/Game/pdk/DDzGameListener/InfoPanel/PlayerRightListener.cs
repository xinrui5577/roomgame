using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 玩家自己右手方player
    /// </summary>
    public class PlayerRightListener : PlayerOtherListener
    {
        private int UpSeatInt;
        private int OwnSeatInt;
        protected override void OnAwake()
        {
            base.OnAwake();
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            PdkGameManager.AddOnUserIdle(OnUserIdle);
    }
        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            if (UserDataTemp == null) return;
            var data = args.IsfObjData;
            var curSeat = data.GetInt(RequestKey.KeySeat);
            if (curSeat == UpSeatInt)
                SetSpeakSpState(false);
            else if (curSeat == OwnSeatInt)
            {
                SetSpeakSpState(false);
                var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
                var curselfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) - cdsLen;
                UserDataTemp.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);

                var globalUserInfoLeft = App.GetGameData<GlobalData>().GetUserInfo(curSeat);
                if (globalUserInfoLeft != null) globalUserInfoLeft.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);

                App.GetGameData<GlobalData>().OnSomePlayerHdcdsChange(curSeat, curselfCdsNum);

                CdNumLabel.text = curselfCdsNum.ToString(CultureInfo.InvariantCulture);
                if (curselfCdsNum > 0 && curselfCdsNum < 2 & JingDengAnim != null) JingDengAnim.SetActive(true);
            }
        }
        /// <summary>
        /// 如果是自己叫pass则显示“不要”，如果是商家出牌则消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == UpSeatInt)
                SetSpeakSpState(false);
            else if (args.IsfObjData.GetInt(RequestKey.KeySeat) == OwnSeatInt)
            {
                SetSpeakSpState(true, SpkBuChu);
            }
        }
        /// <summary>
        /// 根据玩家自己座位号存储其他玩家信息
        /// </summary>
        /// <param name="servData">服务器信息</param>
        protected override void SetOtherPlayerData(ISFSObject servData)
        {
            UpSeatInt = App.GetGameData<GlobalData>().GetSelfSeat;
            OwnSeatInt = App.GetGameData<GlobalData>().GetRightPlayerSeat;
            var dataDic = GetOtherUsesDic(servData);
            var seat = OwnSeatInt;
            if (dataDic.ContainsKey(seat))
            {
                UserDataTemp = dataDic[seat];
                if (UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                    App.GetGameData<GlobalData>().OnSomePlayerHdcdsChange(OwnSeatInt, UserDataTemp.GetInt(NewRequestKey.KeyCardNum));
                RefreshUiInfo();
            }
        }
        /// <summary>
        /// 当有玩家加入游戏时
        /// </summary>
        protected override void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var user = data.GetSFSObject(RequestKey.KeyUser);
            if (!user.ContainsKey(RequestKey.KeySeat) || user.GetInt(RequestKey.KeySeat) != OwnSeatInt)
                return;
            UpdateUserdata(user);
            RefreshUiInfo();
            SetDuanXianSp(false);
        }
        /// <summary>
        /// 玩家空闲状态时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserIdle(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (DuanxianSp == null || data == null) return;

            var curSeat = data.GetInt(RequestKey.KeySeat);
            var playerMaxNum = App.GetGameData<GlobalData>().PlayerMaxNum;
            if (data.ContainsKey(NewRequestKey.KeyUserIdle) && data.GetBool(NewRequestKey.KeyUserIdle) && curSeat == OwnSeatInt)
            {
                SetDuanXianSp(true);
            }
            else if (data.ContainsKey(NewRequestKey.KeyUserIdle) && !data.GetBool(NewRequestKey.KeyUserIdle) && curSeat == OwnSeatInt)
            {
                SetDuanXianSp(false);
            }
        }
    }
}
