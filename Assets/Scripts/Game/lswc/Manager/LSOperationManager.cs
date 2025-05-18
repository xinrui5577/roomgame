using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Manager
{
    /// <summary>
    /// 交互控制类，主要控制按钮的触发
    /// </summary>
    public class LSOperationManager : InstanceControl
    {
        public Button BackBtn;

        public Button ExpandBtn;

        public Button SharinkBtn;

        public Button SettingBtn;

        public Button ChangeAnteBtn;

        public Button GoOnBetBtn;

        public Button ClearBtn;

        public Button AllInBtn;

        public void InitListener()
        {
            BackBtn.onClick.AddListener(OnClickBackbtn);
            ExpandBtn.onClick.AddListener(OnClickExpandBtn);
            SharinkBtn.onClick.AddListener(OnclickSharinkBtn);
            SettingBtn.onClick.AddListener(OnClickSettingBtn);
            ChangeAnteBtn.onClick.AddListener(OnClickChangeAnteBtn);
            GoOnBetBtn.onClick.AddListener(OnClickGoOnBtn);
            ClearBtn.onClick.AddListener(OnClickClearBtn);
            AllInBtn.onClick.AddListener(OnClickAllInBtn);
        }

        private void OnClickBackbtn()
        {
            App.GetGameManager<LswcGamemanager>().SystemControl.QuitGame();
        }

        private void OnClickExpandBtn()
        { 
            App.GetGameManager<LswcGamemanager>().UIManager.ShowBetWindow();
        }

        private void OnclickSharinkBtn()
        {
            App.GetGameManager<LswcGamemanager>().UIManager.HideBetWindow();
        }

        private void OnClickSettingBtn()
        {
            App.GetGameManager<LswcGamemanager>().UIManager.ShowSettingWindow();
        }

        private void OnClickChangeAnteBtn()
        {
            App.GetGameData<LswcGameData>().ChangeAnte();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.UIManager.ChangeAnte();
            gameMgr.SystemControl.PlaySuccess(true);
        }

        private void OnClickGoOnBtn()
        {
            var success = false;
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if (gdata.GlobalELswcGameStatu == ELswcGameState.BetState)
            {
                success = gdata.BetAgain();
                if (success)
                {
                    gameMgr.UIManager.SetBetWindow();
                }
            }
            gameMgr.SystemControl.PlaySuccess(success);
        }

        private void OnClickClearBtn()
        {
            var success = false;
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if (gdata.GlobalELswcGameStatu == ELswcGameState.BetState)
            {
                gdata.ClearBets();
                gameMgr.UIManager.SetBetWindow();
                success = true;
            }
            gameMgr.SystemControl.PlaySuccess(success);

        }

        private void OnClickAllInBtn()
        {
            var success = false;
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if (gdata.GlobalELswcGameStatu == ELswcGameState.BetState)
            {
                if (gdata.BetAll())
                {
                    success = true;
                }
                gameMgr.UIManager.SetBetWindow();
            }
            gameMgr.SystemControl.PlaySuccess(success);
        }

        public override void OnExit()
        {
            BackBtn.onClick.RemoveAllListeners();
            ExpandBtn.onClick.RemoveAllListeners();
            SharinkBtn.onClick.RemoveAllListeners();
            SettingBtn.onClick.RemoveAllListeners();
            ChangeAnteBtn.onClick.RemoveAllListeners();
            GoOnBetBtn.onClick.RemoveAllListeners();
            ClearBtn.onClick.RemoveAllListeners();
            AllInBtn.onClick.RemoveAllListeners();
        }
    }
}
