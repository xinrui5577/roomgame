/** 
 *文件名称:     CopyNoticeWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-05-01 
 *描述:    
 *历史记录: 
*/

using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.Agency
{
    public class CopyNoticeWindow : YxNguiWindow
    {
        #region UI Param

        public UILabel SHowNotice;

        public int StartTime=3;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle
        protected override void OnFreshView()
        {
            base.OnFreshView();
            Invoke("CloseAndOpenWechat",StartTime);
        }

        public void CloseAndOpenWechat()
        {
            Close();
            Application.OpenURL("weixin://");
            //WeChatApi api = Facade.Instance<WeChatApi>();
            //api.InitWechat();
            //if (api.CheckWechatValidity())
            //{
            //    Application.OpenURL("weixin://");
            //}
        }

        #endregion

        #region Function

        #endregion

    }
}
