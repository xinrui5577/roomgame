using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.View;
using ResourceManager = YxFramwork.Manager.ResourceManager;

namespace Assets.Scripts.Common.components
{
    /// <inheritdoc />
    /// <summary>
    /// 退出组件
    /// </summary>
    public class QuitPart : MonoBehaviour
    {
        private static QuitPart _instance;
        void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                YxDebug.LogError("已存在组件！");
            }
        }

        private YxView _curMsgBox;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _curMsgBox == null)
            {
                if (App.GameKey == App.Skin.Hall)
                {
                    if (App.History.CurrentHistory() < YxEHistoryPathType.Hall || !App.Config.QuitToLogin)
                    {
                        _curMsgBox = ShowQuitMessagebox();
                    }
                    else
                    {
                        if (ResourceManager.HasRes(App.Skin.Hall, "QuitWindow"))
                        {
                            _curMsgBox = ShowQuitWindow();
                        }
                        else
                        {
                            _curMsgBox = ShowQuitMessagebox();
                        }
                    }
                }
                else
                {
                    _curMsgBox = App.QuitGameWithMsgBox();
                }
            }
        }

        /// <summary>
        /// 退出/切换账号
        /// </summary>
        public YxView ShowQuitMessagebox()
        {
            return YxMessageBox.Show("您确定要退出吗？", "", (mesBox, btnName) =>
            {
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        App.QuitGame();
                        break;
                }
            }, true, YxMessageBox.RightBtnStyle | YxMessageBox.LeftBtnStyle);
        }


        /// <summary>
        /// 退出/切换账号
        /// </summary>
        public YxView ShowQuitWindow()
        {
            return YxMessageBox.Show(null, "QuitWindow", "您确定要退出吗？", "", (mesBox, btnName) =>
            {
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        App.QuitGame();
                        break;
                    case YxMessageBox.BtnMiddle:
                        HallMainController.Instance.ChangeAccount();
                        break;
                }
            }, true, YxMessageBox.RightBtnStyle | YxMessageBox.MiddleBtnStyle | YxMessageBox.LeftBtnStyle);
        }
    }
}
