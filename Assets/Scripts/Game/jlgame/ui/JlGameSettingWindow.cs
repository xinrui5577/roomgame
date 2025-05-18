using Assets.Scripts.Common.UI;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.Modle;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameSettingWindow : SettingWindow
    {
        public EventObject EventObj;
        public GameObject ChangeRoomBtn;
        public GameObject HupRoomBtn;

        public JlGameGameTable _gdata
        {
            get { return App.GetGameData<JlGameGameTable>(); }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (_gdata.IsCreatRoom)
            {
                HupRoomBtn.SetActive(true);
            }
            else
            {
                ChangeRoomBtn.SetActive(true);
            }
        }

        public void OnChangeRoom()
        {
            EventObj.SendEvent("ServerEvent", "ChangeRoom", null);
        }

        public void OnCreatRoomBack()
        {
            if (_gdata.IsCreatRoom)
            {
                YxMessageBox.Show(
                    "确定解散房间吗?",
                    null,
                    (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                EventObj.SendEvent("ServerEvent", "HupReq", 2);
                                break;
                        }
                    },
                    true,
                    YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
            }
            else
            {
                if (App.GameData.GStatus == YxEGameStatus.Over)
                {
                    App.QuitGameWithMsgBox();
                }
                else
                {
                    YxMessageBox.Show("游戏已经开始游戏结算后可以退出");
                }
            }

        }
    }
}
