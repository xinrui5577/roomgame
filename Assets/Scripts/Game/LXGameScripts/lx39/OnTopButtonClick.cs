using UnityEngine;
using YxFramwork.View;
using YxFramwork.Common;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 获取按钮,并为按钮注册点击事件
    /// </summary>
    public class OnTopButtonClick : MonoBehaviour
    {
        protected GameObject Close;
        protected GameObject Setting;
        public GameObject Trusteeship;
        public GameObject CancelTrusteeship;
        

        public void OnQuitClick()
        {
            if (App.GetGameManager<LXGameManager>().CanQuit)
            {
                YxMessageBox.Show("是否要退出游戏？", null, (box, btnName) =>
                {
                if (btnName.Equals(YxMessageBox.BtnLeft))
                {
                    App.QuitGame();
                }
                }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }
        }
        public void OnSettingClick()
        {
            EventDispatch.Dispatch((int)EventID.GameEventId.SettingBtnClick);
        }
        public void OnTrusteeshipClick(GameObject go)
        {
            go.SetActive(false);
            if (CancelTrusteeship != null)
                CancelTrusteeship.SetActive(true);
            EventDispatch.Dispatch((int)EventID.GameEventId.OnTrusteeshipOpen);
        }
        public void OnCancelTrusteeshipClick(GameObject go)
        {
            go.SetActive(false);
            if (Trusteeship != null)
                Trusteeship.SetActive(true);
            EventDispatch.Dispatch((int)EventID.GameEventId.OnCancelTrusteeship);
        }
    }
}