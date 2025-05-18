using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jsys
{
    public class HandselManager : MonoBehaviour
    {
        public Text BonuText;

        //        private float _currTimer;
        //
        //        private const float TimerInver = 0.05f;
        private int _lastWinning;
        //设置彩金数量
        public void SetIwiningText(int iwining)
        {
            BonuText.text = YxUtiles.GetShowNumberToString(iwining);
        }

        protected void Update()
        {
            var gdata = App.GetGameData<JsysGameData>();
            if (_lastWinning == gdata.Winning)
            {
                return;
            }
            _lastWinning = gdata.Winning;
            SetIwiningText(_lastWinning);
            App.GetGameManager<JsysGameManager>().BetPanelMgr.ShowiWiningText(gdata.Winning);
            //            var gameMgr = App.GetGameManager<JsysGameManager>();
            //            var turnGroupsMgr = gameMgr.TurnGroupsMgr;
            //            Debug.LogError(turnGroupsMgr.GameConfig.TurnTableState + "    ||||  " + (int)GameConfig.GoldSharkState.Marquee);
            //            if (turnGroupsMgr.GameConfig.TurnTableState == (int)GameConfig.GoldSharkState.Marquee)
            //            {
            //                _currTimer += Time.deltaTime;
            //                if (_currTimer > TimerInver)
            //                {
            //                    turnGroupsMgr.GameConfig.BonuNumber = Random.Range(5000, 50000);
            //                    BonuText.text = turnGroupsMgr.GameConfig.BonuNumber + "";
            //                    Debug.LogError(turnGroupsMgr.GameConfig.BonuNumber);
            //                    gameMgr.BetPanelMgr.ShowiWiningText(gdata.Winning);
            //                    _currTimer = 0;
            //                }
            //            }
        }
    }
}
