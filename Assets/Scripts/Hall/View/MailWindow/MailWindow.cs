/** 
 *文件名称:     MailWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-12 
 *描述:         邮件窗口
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MailWindow
{
    public class MailWindow:YxPageListWindow
    {
        #region UI Param

        public YxView DetailPanel;

        #endregion

        #region Data Param
        /// <summary>
        /// 比赛详情请求
        /// </summary>
        public string ActionName = "mailDetail";
        /// <summary>
        /// key请求ID
        /// </summary>
        public string KeyParamId = "mid";
        #endregion

        #region Local Data

        /// <summary>
        /// 当前记录比赛详情
        /// </summary>
        private MailDetailInfo _detailInfo;

        private MailItem selectItem;
        #endregion
        #region Life Cycle

        protected override Type GetItemType()
        {
            return typeof(MailItemData);
        }


        #endregion

        #region Function

        public void ShowDetailInfo(string id,MailItem Item)
        {
            selectItem = Item;
            var dic = new Dictionary<string, object>()
            {
                {KeyParamId, id}
            };
            YxTools.SendActionWithCacheKey(ActionName,dic,SuccessCall, YxTools.GetCacahKey(ActionName, dic));
        }

        private void SuccessCall(object msg)
        {
            if (msg != null)
            {
                _detailInfo = new MailDetailInfo(msg);
                if (_detailInfo.Status != selectItem.Status)
                {
                    selectItem.Status = _detailInfo.Status;
                    selectItem.RefreshView();
                }
                DetailPanel.Show();
                DetailPanel.UpdateView(_detailInfo);
            }
        }
        #endregion
    }   
}
