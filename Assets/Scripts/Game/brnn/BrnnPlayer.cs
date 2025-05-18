using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.brnn
{
    public class BrnnPlayer : YxBaseGamePlayer
    {
        public UILabel BnakTime;
        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            if (App.GetGameData<BrnnGameData>().ThisCanInGold == 0)
            {
                ReSet();
            }
        }

        public void ReSet()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            gdata.ThisCanInGold = App.GameData.GetPlayerInfo().CoinA / gdata.MaxNiuRate;
        }

        public void OpenUrl()
        {
            Application.OpenURL("http://www.kawuxing.com/chess/index.php/Payment/index");
        }

        public void SetBankerTime(int time)
        {
            if (BnakTime == null) return;
            BnakTime.text = time.ToString();
            BnakTime.gameObject.SetActive(true);
        }

        public void HideBankerTime()
        {
            if (BnakTime == null) return;
            BnakTime.text = string.Empty;
        }
    }
}