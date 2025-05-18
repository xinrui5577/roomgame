/** 
 *文件名称:     BankLogDetailView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-05-29 
 *描述:         银行赠送记录详情
 *历史记录: 
*/

using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankLogDetailView :YxNguiWindow
    {
        #region UI Param
        [Tooltip("赠送人昵称")]
        public UILabel SenderNick;
        [Tooltip("赠送人ID")]
        public UILabel SenderID;
        [Tooltip("接收人昵称")]
        public UILabel GetterNick;
        [Tooltip("接收人ID")]
        public UILabel GetterId;
        [Tooltip("赠送数量")]
        public YxBaseLabelAdapter SendNum;
        [Tooltip("赠送时间")]
        public UILabel SendTime;
        #endregion
        #region Data Param
        #endregion
        #region Local Data
        #endregion
        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data!=null&&Data is BankLogItemData)
            {
                var data = Data as BankLogItemData;
                string senderNick;
                string senderId;
                string getterNick;
                string getterId;
                var userInfo = UserInfoModel.Instance.UserInfo;
                if (userInfo==null)
                {
                    YxDebug.LogError("玩家信息为空！");
                    return;
                }
                if (data.SendType>=0)
                {
                    senderNick = userInfo.NickM;
                    senderId = userInfo.UserId;
                    getterNick = data.Nick;
                    getterId = data.UserId;
                }
                else
                {
                    senderNick = data.Nick;
                    senderId = data.UserId;
                    getterNick = userInfo.NickM;
                    getterId = userInfo.UserId;
                }
                SenderNick.TrySetComponentValue(senderNick);
                SenderID.TrySetComponentValue(senderId);
                GetterNick.TrySetComponentValue(getterNick);
                GetterId.TrySetComponentValue(getterId);
                SendNum.TrySetComponentValue(data.Value, "1");
                SendTime.TrySetComponentValue(data.Time);
            }
            else
            {
                YxDebug.LogError("Data is null");
            }
        }

        #endregion 
        #region Function
        #endregion 
		
    }
}
