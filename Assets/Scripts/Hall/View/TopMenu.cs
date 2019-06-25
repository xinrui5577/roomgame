/** 
 *文件名称:     TopMenu.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-12-01 
 *描述:         界面中货币的状态信息(临时处理，后期使用大厅提供工具)
 *历史记录: 
*/

using System;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View
{
    public class TopMenu : YxWindow
    {
        /// <summary>
        /// 用户金币
        /// </summary>
        public UILabel UserGold;
        [Tooltip("金币adapter")]
        public NguiLabelAdapter UserGoldAdapter;
        /// <summary>
        /// 用户元宝
        /// </summary>
        public UILabel UserCash;
        /// <summary>
        /// 房卡
        /// </summary>
        public UILabel RoomCard;
        protected override void OnEnableEx()
        {
            RefreshInfo();
        }

        public void OnUserDataChange()
        {
            if (gameObject.activeInHierarchy)
            {
                RefreshInfo();
            }
        }

        private void RefreshInfo()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            YxTools.TrySetComponentValue(UserGold, userInfo.CoinA.ToString());
            YxTools.TrySetComponentValue(UserGoldAdapter, userInfo.CoinA,"1");
            YxTools.TrySetComponentValue(UserCash, userInfo.CashA.ToString());
            YxTools.TrySetComponentValue(RoomCard, UserInfoModel.Instance.BackPack.GetItem("item2_q").ToString());
        }
    }
}
