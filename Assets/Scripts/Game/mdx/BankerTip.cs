using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.mdx
{
    public class BankerTip : MonoBehaviour
    {
        public string StringFormat = "[{0}] 成为庄家";
        public void ShowBankerTip()
        {
            var gdata = App.GetGameData<MdxGameData>();
            int bankerSeat = gdata.BankSeat;
            if (bankerSeat < 0) return;
            gameObject.SetActive(true);
            var info = gdata.GetPlayerInfo(bankerSeat, true);
            if (info == null) return;
            string bankerName = info.NickM;
            var labels = transform.GetComponentsInChildren<UILabel>();

            var tws = transform.GetComponentsInChildren<TweenScale>();
            int len = labels.Length;
            for (int i = 0; i < len; i++)
            {
                labels[i].text = string.Format(StringFormat, bankerName);
                var t = tws[i];
                t.ResetToBeginning();
                t.PlayForward();
            }
        }

        public void OnTweenFinish()
        {
            gameObject.SetActive(false);
        }
    }
}
