using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn3d
{
    public class SettleMentUI : MonoBehaviour
    {
        public Text SelfWinText;
//        public Text SelfBackText;
        public Text BankerWinText;
//        public Text BankerBackText;

        public Transform SettleMent;
         
        public void SetSettleMentUI()
        {
            if (!SettleMent.gameObject.activeSelf)
            {
                SettleMent.gameObject.SetActive(true);
            }
            var gdata = App.GetGameData<Brnn3dGameData>();
            if (SelfWinText != null)
            {
                var self = gdata.GetPlayerInfo<Brnn3DUserInfo>();
                SelfWinText.text = YxUtiles.GetShowNumberForm(self.WinCoin);
                Facade.Instance<MusicManager>().Play(self.WinCoin > 0 ? "win" : "lost");
            }
            else
            {
                if (SelfWinText != null) SelfWinText.text = "";
            }
            if (BankerWinText != null)
            {
                var gameMgr = App.GetGameManager<Brnn3DGameManager>();
                var banker = gameMgr.TheUpUICtrl.TheBankersManager.Banker;
                BankerWinText.text = YxUtiles.GetShowNumberForm(banker.WinCoin);
            }
            else
            {
                if (BankerWinText != null) BankerWinText.text = "";
            }
        }

        public void HideSettleMentUI()
        {
            Invoke("WaitToHideSettelMentUI", 8f);
        }

        void WaitToHideSettelMentUI()
        {
            if (SettleMent.gameObject.activeSelf)
            {
                SettleMent.gameObject.SetActive(false);
            }
            App.GetGameManager<Brnn3DGameManager>().TheBeiShuMode.PlayBeiShuEff();
        }


    }
}

