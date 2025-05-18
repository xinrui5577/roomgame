using System;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.BaiTuan
{
    public class BtwPlayer : YxBaseGamePlayer
    {
        public UILabel BankTime;

        public void SetBankerTime(int time)
        {
            if (BankTime == null) return;
            BankTime.text = time.ToString();
            BankTime.gameObject.SetActive(true);
        }

        public void HideBankerTime()
        {
            if (BankTime == null) return;
            BankTime.gameObject.SetActive(false);
        }
        public void AddBankerTime()
        {
            if (BankTime == null) return;
            var temp = Int32.Parse(BankTime.text);
            BankTime.text = (++temp).ToString();
        }
    }
}