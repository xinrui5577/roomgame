using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jsys
{
    public class ResultUIManager : MonoBehaviour
    {
        public Text TotalInText;
        public Text WinText;
        public Image ResultPanel;

        public void Awake()
        {
            SetTotalInLabel(0);
            SetWinLabel(0);
        }

        public bool isTrue = true;
        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameFinish()
        {
            var gdata = App.GetGameData<JsysGameData>();
            var gameMgr = App.GetGameManager<JsysGameManager>();
            gdata.isOut = true;
            //隐藏开奖动画
            gameMgr.AnimationMgr.HideAnimation();
            gameMgr.AnimationMgr.HideBetPanel();
            //结算板
            if (gdata.IsShark == false)
            {
                ShowJieSuan();
            }
            SetTotalInLabel(-gdata.Ante);
            SetWinLabel(gdata.Gold);
            gameMgr.BetPanelMgr.ShowIgetMoney(gdata.WinGold);
            //更新路子显示
            gameMgr.HistoryMgr.ShowNewHistory(gdata.EndAnimal);
            if (gdata.IsShark)
            {
                gdata.IsShark = false;
                gameMgr.TurnGroupsMgr.PlayGame();
                isTrue = false;
            }
            if (isTrue)
            {
                Invoke("ChuXian", 4f);
            }
            isTrue = true;
            gdata.IsShark = false;
            gdata.FishIdx = 1;
        }

        protected void SetWinLabel(int gold)
        {
            WinText.text = YxUtiles.GetShowNumberToString(gold);
        }
        protected void SetTotalInLabel(int gold)
        {
            TotalInText.text = YxUtiles.GetShowNumberToString(gold);
        }
        public void ChuXian()
        {
            App.GetGameManager<JsysGameManager>().BetPanelMgr.GameBeginXizhu();
        }

        //显示开奖结算页面
        public void ShowJieSuan()
        {
            if (!ResultPanel.gameObject.activeSelf)
            {
                ResultPanel.gameObject.SetActive(true);
            }
        }
        //隐藏结算面板显示
        public void HideJieSuanUI()
        {
            if (ResultPanel.gameObject.activeSelf)
                ResultPanel.gameObject.SetActive(false);
        }
    }
}
