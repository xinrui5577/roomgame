using Assets.Scripts.Common.Windows;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongYxUserInfoWindow : YxUserInfoWindow
    {
        public GameObject FkItem;
        public GameObject ExpressMask;

        protected override void OnFreshView()
        {
            var userInfo = GetData<YxBaseUserInfo>();
            if (userInfo == null) { return; }
            MahjongFreshUserInfoView(userInfo);
            FreshExpressionView(userInfo.Seat);
            if (SpreadBtn != null)
            {
                var needShow = userInfo.SpreadCode;
                SpreadBtn.SetActive(needShow == 0);
            }
            if (ChangePwdBtn != null)
            {
                ChangePwdBtn.SetActive(!string.IsNullOrEmpty(userInfo.LoginName));
            }
        }

        protected void MahjongFreshUserInfoView(YxBaseUserInfo userInfo)
        {
            PlayerInfo.Info = userInfo;
            var gdata = App.GameData;
            //if (gdata == null || userInfo.Seat == gdata.SelfSeat) { return; }
            var parm = new Dictionary<string, object>();
            parm["userId"] = userInfo.UserId;
            parm["gameKey"] = App.GameKey;
            Facade.Instance<TwManager>().SendAction("gamePartnerInfo", parm, obj =>
            {
                if (PlayerInfo == null) { return; }
                var dict = obj as Dictionary<string, object>;
                if (dict == null) { return; }
                userInfo = GetData<YxBaseUserInfo>();
                if (userInfo == null) { return; }
                //userInfo.Parse(dict);
                //PlayerInfo.Info = userInfo;     

                var mahjongInfo = PlayerInfo as MahjongYxBaseDetailedPlayer;
                if (mahjongInfo != null)
                {
                    var fkNum = GetValue(dict, "item2_q");
                    var flag = !string.IsNullOrEmpty(fkNum) && GameCenter.DataCenter.Room.RoomType == MahRoomType.FanKa;
                    FkItem.SetActive(flag);

                    mahjongInfo.SetFk(fkNum);
                    mahjongInfo.SetId(GetValue(dict, "user_id"));
                }

            }, false, null, false);
        }

        private string GetValue(Dictionary<string, object> dic, string key)
        {
            object obj;
            if (dic.TryGetValue(key, out obj))
            {
                if (obj != null) return obj.ToString();
            }
            return "";
        }

        protected override void FreshExpressionView(int serverSeat)
        {
            if (ExpressionWin != null)
            {
                ExpressionWin.transform.localScale = Vector3.one;
                ExpressionWin.gameObject.SetActive(true);
                ExpressionWin.AttackIndex = serverSeat;

                var flag = App.GameData.SelfSeat == serverSeat;
                ExpressMask.SetActive(flag);

                var win = ExpressionWin as MahjongExpressionWindow;
                win.IsSelf = flag;
            }
        }
    }
}