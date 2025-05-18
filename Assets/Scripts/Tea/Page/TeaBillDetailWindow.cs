using Assets.Scripts.Common.Windows;
using Assets.Scripts.Hall.View.RecordWindows;
using UnityEngine;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     TeaBillDetailWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-17
 *描述:        	茶馆账单结算详情界面(账单Item结构完全可以搞定当前的视图显示，因此只做了一个中间逻辑转换)
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaBillDetailWindow : YxNguiWindow
    {
        #region UI Param
        [Tooltip("主View")]
        public YxView MainView;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data != null&&MainView)
            {
                MainView.UpdateView(Data);
            }
        }

        #endregion

        #region Function

        #endregion
    }
}
